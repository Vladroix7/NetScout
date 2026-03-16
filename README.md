# 🛡️ NetScout — Network Security Monitoring Tool

A lightweight network security scanner and monitoring tool built with **C# .NET 8 WinForms**, featuring a TCP server, admin dashboard, and multi-user client app.

---

## 📸 Overview

NetScout is split into two applications:

| App | Description |
|-----|-------------|
| **NetScoutServer** | Admin panel + TCP server. Manages users, monitors sessions, handles requests. |
| **NetScoutClient** | User-facing app. Login, register, and run network scans. |

---

## ✨ Features

### 🖥️ Server (Admin Panel)
- Secure admin login (admin-only access)
- View all registered users in a live table
- See who is currently online (Recently Online panel)
- Delete users / Toggle admin status
- Change user passwords
- Password reset request handling
- Dark-themed live server log

### 🔍 Client (Scanner)
- User registration & login
- 4 scan modes:
  - **Ping Scan** — discover live hosts
  - **Port Scan** — check common ports (21, 22, 80, 443, etc.)
  - **OS Detection** — guess OS via TTL values
  - **Aggressive Scan** — ping + ports + OS combined
- Supports single IP, IP range (`192.168.1.1-254`), CIDR (`192.168.1.0/24`), and hostnames
- Right-click any result → **Full Port Scan (1–1024)**
- Double-click a host to open a detailed port viewer
- Connect to any server by IP (works over LAN)

---

## 🔐 Security
- Passwords are **SHA-256 hashed on the client** before being sent over the network
- Server log masks all password fields (`[HASH HIDDEN]`)
- Admin account is protected from deletion or demotion

---

## 🛠️ Tech Stack

- **Language:** C# (.NET 8.0)
- **UI:** Windows Forms (WinForms)
- **Database:** MySQL 8.0
- **Networking:** TCP/IP on port `11111`
- **Auth:** SHA-256 password hashing

---

## ⚙️ Setup

### Requirements
- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MySQL 8.0

### 1. Database
Run the included SQL script to create the database:
```sql
mysql -u root -p < database.sql
```

Default admin account:
- **Email:** `admin@netscout.com`
- **Password:** `admin123`

### 2. Build & Run Server
```bash
cd NetScoutServer
dotnet run
```
Log in with admin credentials. The TCP server starts automatically on port `11111`.

### 3. Build & Run Client
```bash
cd NetScoutClient
dotnet run
```
Enter the server's IP address in the login screen and connect.

---

## 🌐 LAN / Multi-PC Usage

1. Run the **server** on one PC — it listens on all network interfaces
2. Find the server's local IP (e.g. `192.168.1.248`)
3. On any other PC on the same network, run the **client** and enter that IP
4. Register or log in — the admin will see you in the Online panel

> **Firewall:** Make sure port `11111` is allowed through Windows Defender Firewall on the server PC.

---

## 📁 Project Structure

```
NetScout/
├── NetScoutServer/        # Admin panel + TCP server
│   ├── AdminDashboard.cs
│   ├── ServerCore.cs
│   ├── DatabaseHelper.cs
│   └── ServerLoginForm.cs
├── NetScoutClient/        # User client app
│   ├── LoginForm.cs
│   ├── RegisterForm.cs
│   ├── MainDashboard.cs
│   └── PortDetailForm.cs
└── database.sql           # MySQL schema
```

---

## 📄 License

This project was built for educational purposes as a network security monitoring demo.
