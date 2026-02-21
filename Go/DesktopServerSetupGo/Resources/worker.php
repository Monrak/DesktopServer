<?php
ini_set('display_errors', 'stderr');
ini_set('display_startup_errors', '0');

// Aggressive Output Buffering: Swallow any startup noise
ob_start();

require __DIR__ . '/vendor/autoload.php';

use Spiral\RoadRunner;
use Nyholm\Psr7;

$worker = null;
$psr7 = null;

try {
    $worker = RoadRunner\Worker::create();
    $psr7 = new RoadRunner\Http\PSR7Worker(
        $worker,
        new Psr7\Factory\Psr17Factory(),
        new Psr7\Factory\Psr17Factory(),
        new Psr7\Factory\Psr17Factory()
    );
} catch (\Throwable $e) {
    fwrite(STDERR, "Worker Bootstrap Error: " . $e->getMessage() . "\n");
    exit(1);
}

// Clean the buffer so the protocol stream is pure
ob_end_clean();

$wwwRoot = __DIR__ . '/www';
$phpCgi  = __DIR__ . '/php84/php-cgi.exe';

while ($request = $psr7->waitRequest()) {
    try {
        $path = ltrim($request->getUri()->getPath(), '/');
        
        // === Resolve the file path ===
        $fullPath = $wwwRoot . '/' . ($path ?: '');
        
        // If path points to a directory, look for index files
        if (is_dir($fullPath)) {
            if (file_exists($fullPath . '/index.php')) {
                $fullPath .= '/index.php';
                $path .= ($path ? '/' : '') . 'index.php';
            } elseif (file_exists($fullPath . '/index.html')) {
                $fullPath .= '/index.html';
                $path .= ($path ? '/' : '') . 'index.html';
            }
        }

        // === PHP File Handler (CGI bridge for full legacy compatibility) ===
        if (str_ends_with($fullPath, '.php') && file_exists($fullPath)) {
            
            // Build CGI environment
            $queryString = $request->getUri()->getQuery() ?? '';
            $env = [
                'REQUEST_URI'     => $request->getUri()->getPath() . ($queryString ? '?'.$queryString : ''),
                'REQUEST_METHOD'  => $request->getMethod(),
                'REQUEST_SCHEME'  => 'http',
                'HTTPS'           => 'off',
                'QUERY_STRING'    => $queryString,
                'SCRIPT_FILENAME' => str_replace('/', '\\', $fullPath),
                'SCRIPT_NAME'     => '/' . $path,
                'PHP_SELF'        => '/' . $path,
                'DOCUMENT_ROOT'   => str_replace('/', '\\', $wwwRoot),
                'SERVER_SOFTWARE' => 'Monrak Go/RoadRunner',
                'SERVER_NAME'     => 'localhost',
                'SERVER_ADDR'     => '127.0.0.1',
                'SERVER_PORT'     => '8080',
                'SERVER_PROTOCOL' => 'HTTP/1.1',
                'REDIRECT_STATUS' => '200',
                'GATEWAY_INTERFACE' => 'CGI/1.1',
                'REMOTE_ADDR'     => '127.0.0.1',
                'SYSTEMROOT'      => getenv('SYSTEMROOT') ?: 'C:\\Windows',
                'PATH'            => getenv('PATH') ?: '',
            ];

            // Map ALL incoming headers to HTTP_ equivalents
            foreach ($request->getHeaders() as $name => $values) {
                $key = 'HTTP_' . strtoupper(str_replace('-', '_', $name));
                // RFC 6265: Cookies should be joined by semicolon
                $env[$key] = (strtolower($name) === 'cookie') 
                    ? implode('; ', $values) 
                    : implode(', ', $values);
            }

            // Special handling for content type/length which aren't prefixed with HTTP_ in CGI
            if (isset($env['HTTP_CONTENT_TYPE'])) $env['CONTENT_TYPE'] = $env['HTTP_CONTENT_TYPE'];
            if (isset($env['HTTP_CONTENT_LENGTH'])) $env['CONTENT_LENGTH'] = $env['HTTP_CONTENT_LENGTH'];

            // Run php-cgi.exe
            // Note: We dont pass the filename as arg to php-cgi, it reads SCRIPT_FILENAME from env
            $process = proc_open(
                "\"$phpCgi\" -c \"" . __DIR__ . "/php84/php.ini\"",
                [
                    0 => ['pipe', 'r'],  // stdin
                    1 => ['pipe', 'w'],  // stdout
                    2 => ['pipe', 'w'],  // stderr
                ],
                $pipes,
                dirname($fullPath),
                $env
            );

            if (!is_resource($process)) {
                $resp = new Psr7\Response(500);
                $resp->getBody()->write('<h1>500 Internal Error</h1>Failed to start PHP-CGI process');
                $psr7->respond($resp);
                continue;
            }

            // Send POST body
            $body = (string)$request->getBody();
            $parsedBody = $request->getParsedBody();
            
            // Reconstruct body if it's already parsed (sometimes PSR-7 workers do this)
            // or if it looks like JSON but content-type is form-urlencoded
            if (is_array($parsedBody) && !empty($parsedBody) && 
                str_contains($request->getHeaderLine('Content-Type'), 'application/x-www-form-urlencoded')) {
                $reconstructed = http_build_query($parsedBody);
                if ($reconstructed !== $body) {
                    $body = $reconstructed;
                }
            }

            if ($body !== '') {
                fwrite($pipes[0], $body);
            }
            fclose($pipes[0]);

            // Read CGI output (Headers + Body)
            $output = stream_get_contents($pipes[1]) ?: '';
            $stderr = stream_get_contents($pipes[2]) ?: '';
            fclose($pipes[1]);
            fclose($pipes[2]);
            
            proc_close($process);

            // Log CGI errors to worker stderr (visible in roadrunner.log)
            if ($stderr) {
                fwrite(STDERR, "CGI-Error: $stderr\n");
            }

            // Parse CGI output
            $statusCode = 200;
            $headers = [];
            $responseBody = '';

            // CGI format: "Headers\r\n\r\nBody"
            $parts = preg_split("/\r?\n\r?\n/", $output, 2);
            if (count($parts) === 2) {
                $headerBlock = $parts[0];
                $responseBody = $parts[1];
                
                foreach (explode("\n", $headerBlock) as $headerLine) {
                    $headerLine = trim($headerLine);
                    if ($headerLine === '' || strpos($headerLine, ':') === false) continue;
                    
                    if (stripos($headerLine, 'Status:') === 0) {
                        if (preg_match('/Status:\s*(\d+)/i', $headerLine, $sm)) {
                            $statusCode = (int)$sm[1];
                        }
                    } else {
                        list($hName, $hValue) = explode(':', $headerLine, 2);
                        $headers[trim($hName)][] = trim($hValue);
                    }
                }
            } else {
                // Only body or only headers? Assume body if no obvious headers.
                if (strpos($output, ':') !== false && strpos($output, ':') < 50) {
                    // Looks like headers only
                    $headerBlock = $output;
                    foreach (explode("\n", $headerBlock) as $headerLine) {
                        $headerLine = trim($headerLine);
                        if (strpos($headerLine, ':') !== false) {
                            list($hName, $hValue) = explode(':', $headerLine, 2);
                            $headers[trim($hName)][] = trim($hValue);
                        }
                    }
                } else {
                    $responseBody = $output;
                }
            }

            $resp = new Psr7\Response($statusCode);
            foreach ($headers as $name => $values) {
                // Skip transfer-encoding as PSR-7 manages this
                if (strtolower($name) === 'transfer-encoding') continue;
                $resp = $resp->withHeader($name, $values);
            }
            $resp->getBody()->write($responseBody);

        } else {
            // === Static file fallback ===
            if (!file_exists($fullPath) || is_dir($fullPath)) {
                $resp = new Psr7\Response(404);
                $resp->getBody()->write("<h1>404 Not Found</h1>Monrak Go! Engine");
            } else {
                $resp = new Psr7\Response(200);
                
                // Set Content-Type based on extension
                $ext = strtolower(pathinfo($fullPath, PATHINFO_EXTENSION));
                $mimeTypes = [
                    'html' => 'text/html', 'css' => 'text/css', 'js' => 'application/javascript',
                    'json' => 'application/json', 'png' => 'image/png', 'jpg' => 'image/jpeg',
                    'gif' => 'image/gif', 'svg' => 'image/svg+xml', 'ico' => 'image/x-icon',
                    'woff' => 'font/woff', 'woff2' => 'font/woff2', 'ttf' => 'font/ttf',
                ];
                if (isset($mimeTypes[$ext])) {
                    $resp = $resp->withHeader('Content-Type', $mimeTypes[$ext]);
                }
                
                $content = file_get_contents($fullPath);
                if ($content !== false) {
                    $resp->getBody()->write($content);
                }
            }
        }

        $psr7->respond($resp);
    } catch (\Throwable $e) {
        $psr7->getWorker()->error((string)$e);
    }
}
