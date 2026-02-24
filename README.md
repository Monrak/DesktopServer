# Monrak DesktopServer

[![Platform](https://img.shields.io/badge/platform-Windows-0078d7.svg)](https://www.microsoft.com/windows)
[![PHP](https://img.shields.io/badge/PHP-8.4-777bb4.svg)](https://www.php.net/)
[![License](https://img.shields.io/badge/license-Proprietary-red.svg)](#license)
[![Status](https://img.shields.io/badge/status-Beta-orange.svg)](#)

**Monrak DesktopServer** is a professional, native local development stack for Windows. It provides a streamlined, high-performance environment for modern PHP, MySQL, and PostgreSQL development—designed specifically for developers who prioritize execution speed and system efficiency.

[Features](#-key-features) • [Tech Stack](#-tech-stack) • [Versions](#-version-comparison) • [Getting Started](#-getting-started)

---

## 🏎️ Monrak DesktopServer Go!
**High-Performance PHP & AI Development Engine**

Developed by **Monrak Net Technology**, DesktopServer focuses on speed, stability, and professional-grade tools for both legacy maintenance and modern web application development.

---

## 🎯 Our Mission: Performance-First Development

We believe that professional development environments should be fast, reliable, and accessible on any hardware.

*   **Modern Standards**: Out-of-the-box support for **PHP 8.4**, **PostgreSQL 17**, and **RoadRunner**.
*   **Native Execution**: Unlike Docker or Virtualization, DesktopServer runs natively on Windows for minimum overhead and maximum speed.
*   **Hardware Optimized**: Designed to be lightweight and memory-efficient, giving older machines a "Pro" development experience.

---

## 🚀 Key Features

*   **Unified Stack**: Pre-configured Apache 2.4, MariaDB 11, and modern PHP runtimes.
*   **Go! Edition Engine**: Integrated **RoadRunner** high-performance PHP orchestrator for sub-5ms response times.
*   **Database Excellence**: Professional support for MariaDB 11, MySQL 8.0, and **PostgreSQL 17 (AI-Ready)**.
*   **Native Local SSL**: Built-in Automated Certificate Authority (CA) and SSL/HTTPS management with SAN support—get green locks on `localhost` instantly.
*   **Isolated Environments**: Prevents system-wide path conflicts with smart process isolation.
*   **Real-time Monitoring**: Integrated dashboard for CPU and RAM performance tracking (Go! Edition).
*   **Premium Management**: Custom .NET 9 management UI with system tray integration and live log viewers.

---

## 🛠️ Tech Stack

### core
*   **Management Console**: .NET 9.0 (C# / WinForms)
*   **Web Orchestration**: Apache 2.4 & RoadRunner (Next-Gen PHP)

### Database & Storage
*   **RDBMS**: MariaDB 11, MySQL 8.0, PostgreSQL 17 (with pgvector for AI)
*   **Tools**: phpMyAdmin & pgAdmin 4 Desktop Manager

### Runtimes
*   **PHP**: 5.6 / 7.4 / 8.2 / 8.4+ (Universal support)
*   **Runtime Libraries**: Bundled VC++ 2012-2022 redistributables

---

## 📦 Version Comparison

| Feature | Lite | Pro/Advance | Go! Edition |
| :--- | :---: | :---: | :---: |
| **Web Server** | Apache 2.4 | Apache 2.4 | **RoadRunner** |
| **PHP Support** | Single (7.4) | Multi (5.6-8.2) | **Modern (8.4+)** |
| **DB (SQL)** | MySQL 8.0 | MySQL 8.0 | **MariaDB 11** |
| **DB (Postgre)** | ❌ | ❌ | **PostgreSQL 17** |
| **AI Hub** | ❌ | ❌ | **Integrated** |
| **Monitoring** | Basic | Basic | **Real-time Stats** |
| **Environment** | Common Path | Isolated Paths | **High-Concurrency** |

---

## 🏗️ Getting Started

### Prerequisites
*   Windows 10 / 11 (64-bit)
*   .NET 9.0 Desktop Runtime

### Installation
1.  Download the professional installer for your preferred edition.
2.  Run the setup and follow the high-speed deployment wizard.
3.  Launch **DesktopServer Manager** from the desktop or system tray.

> [!IMPORTANT]
> **Source Builds**: If you are building from source, please refer to the **[BUILD_GUIDE.md](BUILD_GUIDE.md)** for binary dependency instructions (Apache, PHP, etc.), as these are not bundled in the core repository due to size constraints.

---

## 💖 Support the Project

If **Monrak DesktopServer** helps you build better apps faster, consider supporting its development. Your contributions help keep the project alive and growing.

<div align="center">
  <img src="docs/images/promptpay.png" width="300" alt="Support via PromptPay">
  <p><i>Support via PromptPay (Thailand)</i></p>
</div>

---

## 📜 License

Copyright © 2026 **Monrak Net Protocol**. All rights reserved.
Developed for professional excellence by [Monrak Net Technology](https://github.com/monraknet).

---
> [!NOTE]
> Optimized for local development. For production-grade security, follow the included configuration hardening guides for Apache and MySQL.
