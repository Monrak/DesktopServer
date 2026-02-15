# DesktopServer Build & Dependency Guide

This guide explains how to gather, structure, and bundle the binary dependencies required to build the DesktopServer installers (Lite and Pro).

## 📥 Binary Sources

You can download the official binaries from these trusted sources:

| Component | Source Link | Notes |
| :--- | :--- | :--- |
| **Apache 2.4** | [Apache Lounge](https://www.apachelounge.com/download/) | Use Win64 VS17 version. |
| **PHP** | [PHP for Windows](https://windows.php.net/download/) | Use **Thread Safe (TS)** x64 builds. |
| **MySQL** | [MySQL Community Server](https://dev.mysql.com/downloads/mysql/) | Use the Windows (x86, 64-bit), ZIP Archive. |
| **MariaDB** | [MariaDB Downloads](https://mariadb.org/download/) | (Optional) Alternative to MySQL. |
| **phpMyAdmin** | [phpMyAdmin Downloads](https://www.phpmyadmin.net/downloads/) | Use "-all-languages" zip. |
| **VC++ Redist** | [Microsoft Official](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170) | `vcredist_x64.exe` is required for PHP. |

---

## 🏗️ Zip Structure Requirements

The setup engine expects specific structures inside the `.zip` files. Follow these rules when creating your bundles:

### 1. Apache (`apache24_tmp.zip`)
- **Structure**: Must contain a single folder named `Apache24` at the root of the zip.
- **Inside `Apache24`**: Should be `bin`, `conf`, `error`, `htdocs`, etc.
- **Why**: The installer extracts this and looks specifically for the `Apache24` subfolder to move it to the final destination.

### 2. MySQL (`mysql80_tmp.zip`) / MariaDB
- **Structure**: All database files (bin, data, share, etc.) should be inside a single subfolder (e.g., `mysql-8.0.27-winx64`).
- **Why**: The installer extracts the zip and picks the *first* folder it finds to treat as the database root.
- **MariaDB Note**: To use MariaDB, simply zip the MariaDB folder structure in the same way and name the file `mysql80_tmp.zip`. The manager will treat it as MySQL.

### 3. PHP (`php74.zip`, `php56.zip`, etc.)
- **Structure**: PHP files (`php.exe`, `php7apache2_4.dll`, etc.) should be at the **root of the zip**.
- **Inside**: No subfolders. Just the files.

### 4. phpMyAdmin (`pma_tmp.zip`, `pma56_tmp.zip`)
- **Structure**: Should contain the standard phpMyAdmin folder (e.g., `phpMyAdmin-5.2.1-all-languages`).
- **Inside**: The installer will find this subfolder and rename it to `phpmyadmin` in the `www` directory.

---

## 🔄 Using MariaDB instead of MySQL

If you prefer MariaDB:
1. Download the MariaDB ZIP (Win64).
2. Ensure the files are in a subfolder inside the zip.
3. Name the zip `mysql80_tmp.zip`.
4. Place it in the `Resources` folder of the Setup project.
5. The Manager UI will still label it as "MySQL", but it will correctly start and stop the MariaDB process (`mysqld.exe`).

---

## 🛠️ Build Process Summary

1. **Manager**: Build `DesktopServerManager` (or Pro) in Release mode.
2. **Resource Sync**: Copy the resulting `.exe` to the `Setup/Resources` folder.
3. **Setup**: Place all required `.zip` files and `vcredist_x64.exe` in the `Resources` folder.
4. **Compile**: Rebuild the `DesktopServerSetup` project.

> [!TIP]
> Use the `build_all.ps1` script in the root directory to automate this entire sequence!
