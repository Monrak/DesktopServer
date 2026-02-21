# DesktopServer Build & Dependency Guide

This guide explains how to gather, structure, and bundle the binary dependencies required to build the DesktopServer installers (Lite, Pro, and Go).

## 📥 Binary Sources

You can download the official binaries from these trusted sources:

| Component | Source Link | Notes |
| :--- | :--- | :--- |
| **Apache 2.4** | [Apache Lounge](https://www.apachelounge.com/download/) | Use Win64 VS17 version. |
| **PHP** | [PHP for Windows](https://windows.php.net/download/) | Use **Thread Safe (TS)** x64 builds. |
| **MySQL** | [MySQL Community Server](https://dev.mysql.com/downloads/mysql/) | Use the Windows (x86, 64-bit), ZIP Archive. |
| **MariaDB** | [MariaDB Downloads](https://mariadb.org/download/) | Use **ZIP Archive** (MariaDB 11.x recommended). |
| **phpMyAdmin** | [phpMyAdmin Downloads](https://www.phpmyadmin.net/downloads/) | Use "-all-languages" zip. |
| **VC++ Redist** | [Microsoft Official](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170) | `vcredist_x64.exe` (VS 2015-2022) is required. |
| **RoadRunner** | [Official GitHub](https://github.com/roadrunner-server/roadrunner/releases) | Use Windows-AMD64 version. |
| **PostgreSQL** | [EDB Binaries](https://www.enterprisedb.com/download-postgresql-binaries) | Use Windows x86-64 ZIP Archive (v17.2 recommended). |

---

## 🏗️ Zip Structure (Smart Flattening)

The setup engine in the Go Edition features **Smart Flattening**. If your ZIP contains a nested folder (e.g., `mysql-11.x-winxx/` or `php-8.4.x/`), the installer will automatically detect it and "hoist" the contents to the root of the target directory. 

However, following these rules ensures the fastest installation:

### 1. Apache (`apache24_tmp.zip`)
- **Structure**: Must contain a single folder named `Apache24` at the root of the zip.
- **Inside `Apache24`**: Should be `bin`, `conf`, `error`, `htdocs`, etc.
- **Why**: The installer extracts this and looks specifically for the `Apache24` subfolder to move it to the final destination.

### 2. MySQL (`mysql80_tmp.zip`) / MariaDB
- **Structure**: All database files (bin, data, share, etc.) should be inside a single subfolder (e.g., `mysql-8.0.27-winx64`).
- **Why**: The installer extracts the zip and picks the *first* folder it finds to treat as the database root.
- **MariaDB Note**: To use MariaDB, simply zip the MariaDB folder structure in the same way and name the file `mysql80_tmp.zip`. The manager will treat it as MySQL.

### 3. PHP (`php74.zip`, `php56.zip`, `php84_tmp.zip`, etc.)
- **Structure**: PHP files (`php.exe`, `php7apache2_4.dll`, etc.) should be at the **root of the zip**.
- **Inside**: No subfolders. Just the files.

### 4. phpMyAdmin (`pma_tmp.zip`)
- **Structure**: Should contain the standard phpMyAdmin folder.
- **Inside**: The installer will rename it to `phpmyadmin` in the `www` directory.

### 5. RoadRunner (`roadrunner_tmp.zip`)
- **Structure**: Should contain the `rr.exe` binary at the root (or nested folder).
- **Placement**: `Go/DesktopServerSetupGo/Resources/`.
- **Worker**: The installer handles the `worker.php` and `.rr.yaml` deployment separately.

### 6. PostgreSQL (`postgres_tmp.zip`)
- **Structure**: Standard binary structure (bin, data, etc.). 
- **Auto-Initialization**: The Manager automatically runs `initdb` with `trust` auth on first launch if the data directory is empty.
- **Adminer**: Use the built-in Adminer button in the Go Manager for one-click access (auto-logs in as `postgres`).

## 🐘 PostgreSQL Management Tools

Since phpMyAdmin is for MySQL/MariaDB, you'll need a tool to manage PostgreSQL:
- **pgAdmin 4 v9.12**: [Direct Link](https://ftp.postgresql.org/pub/pgadmin/pgadmin4/v9.12/windows/pgadmin4-9.12-x64.exe) - The most popular desktop management tool.
- **DBeaver**: A universal database tool that supports everything (highly recommended).
- **HeidiSQL**: Lightweight and already familiar to many PHP developers.

---

## 🔄 Using MariaDB instead of MySQL

If you prefer MariaDB:
1. Download the MariaDB ZIP (Win64).
2. Ensure the files are in a subfolder inside the zip.
3. Name the zip `mariadb_tmp.zip` (For Go edition) or `mysql80_tmp.zip` (For Lite/Pro).
4. Place it in the `Resources` folder of the Setup project.
5. The Manager UI will correctly start and stop the MariaDB process (`mariadbd.exe` for Go, `mysqld.exe` for older stacks), and the Uninstaller handles both correctly to prevent file locks.

---

## 🛠️ Build Process Summary

1. **Manager**: Build `DesktopServerManager` (or Pro) in Release mode.
2. **Resource Sync**: Copy the resulting `.exe` to the `Setup/Resources` folder.
3. **Setup**: Place all required `.zip` files and `vcredist_x64.exe` in the `Resources` folder.
4. **Compile**: Rebuild the `DesktopServerSetup` project.

---

## 🏗️ Resource Folder Separation

To keep things organized, each edition has its own `Resources` folder:
- **Lite**: `Lite/DesktopServerSetup/Resources/`
- **Pro**: `Pro/DesktopServerSetupPro/Resources/`
- **Go**: `Go/DesktopServerSetupGo/Resources/`

> [!IMPORTANT]
> For the **Go Edition**, you do **NOT** need `apache24_tmp.zip`. You need **`roadrunner_tmp.zip`**, **`php84_tmp.zip`**, **`mysql80_tmp.zip`** (for MariaDB), **`postgres_tmp.zip`**, and **`pma_tmp.zip`** (for phpMyAdmin).

---

## 💾 Large File Support (Git LFS)

To keep the large binary files (Apache, MySQL, etc.) as part of the project without hitting GitHub's 100MB limit, this repository is configured to use **Git LFS**.

### 1. Installation
If you haven't already, install Git LFS on your system:
- Download from [git-lfs.com](https://git-lfs.com/) or run `git lfs install` in your terminal.

### 2. How it works
The project is configured via `.gitattributes` to track `.zip` and `.exe` files located within any `Resources` folder. When you push these files, Git LFS stores them efficiently while keeping the project structure intact.

### 3. Pushing Binary Updates
When adding or updating binaries:
1. Place the new files in the appropriate `Resources/` folder.
2. Run `git add .` (Git detects LFS tracking automatically).
3. `git commit -m "Add binary dependencies"`
4. `git push` (The binaries will be uploaded to LFS storage).

> [!NOTE]
> GitHub provides a free tier for LFS. If you hit storage limits, you can still use the manual download links provided in this guide.
