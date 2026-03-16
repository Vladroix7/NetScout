-- NetScout Database Setup
-- Run this in MySQL Workbench or: mysql -u root -p < database.sql

CREATE DATABASE IF NOT EXISTS netscout CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE netscout;

-- ─────────────────────────────────────────────────────────
--  USERS
-- ─────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS Users (
    Id          INT          AUTO_INCREMENT PRIMARY KEY,
    Email       VARCHAR(255) UNIQUE NOT NULL,
    Password    VARCHAR(64)  NOT NULL,          -- SHA-256 hex
    Username    VARCHAR(100) NOT NULL,
    IsAdmin     TINYINT      DEFAULT 0,
    CreatedDate DATETIME     DEFAULT CURRENT_TIMESTAMP,
    LastLogin   DATETIME     NULL
);

-- If you already have a Users table without IsAdmin, run:
-- ALTER TABLE Users ADD COLUMN IF NOT EXISTS IsAdmin TINYINT DEFAULT 0;

-- ─────────────────────────────────────────────────────────
--  SCAN RESULTS
-- ─────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS ScanResults (
    Id           INT          AUTO_INCREMENT PRIMARY KEY,
    UserId       INT          NOT NULL,
    ScanDate     DATETIME     DEFAULT CURRENT_TIMESTAMP,
    ScanType     VARCHAR(50),
    TargetRange  VARCHAR(100),
    TotalDevices INT          DEFAULT 0,
    Status       VARCHAR(50)  DEFAULT 'Completed',
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- ─────────────────────────────────────────────────────────
--  DEVICES
-- ─────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS Devices (
    Id              INT          AUTO_INCREMENT PRIMARY KEY,
    ScanId          INT          NOT NULL,
    IpAddress       VARCHAR(50)  NOT NULL,
    MacAddress      VARCHAR(50),
    DeviceName      VARCHAR(255),
    OperatingSystem VARCHAR(255),
    IsOnline        TINYINT      DEFAULT 1,
    ResponseTime    INT,                        -- ms
    Ttl             INT,
    LastSeen        DATETIME     DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ScanId) REFERENCES ScanResults(Id) ON DELETE CASCADE
);

-- ─────────────────────────────────────────────────────────
--  OPEN PORTS
-- ─────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS OpenPorts (
    Id          INT         AUTO_INCREMENT PRIMARY KEY,
    DeviceId    INT         NOT NULL,
    PortNumber  INT         NOT NULL,
    Protocol    VARCHAR(10) NOT NULL,
    ServiceName VARCHAR(100),
    State       VARCHAR(20) DEFAULT 'Open',
    FOREIGN KEY (DeviceId) REFERENCES Devices(Id) ON DELETE CASCADE
);

-- ─────────────────────────────────────────────────────────
--  SECURITY ISSUES
-- ─────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS SecurityIssues (
    Id           INT          AUTO_INCREMENT PRIMARY KEY,
    DeviceId     INT          NOT NULL,
    IssueType    VARCHAR(100) NOT NULL,
    Severity     ENUM('Low','Medium','High','Critical') NOT NULL,
    Title        VARCHAR(255) NOT NULL,
    Description  TEXT,
    DetectedDate DATETIME     DEFAULT CURRENT_TIMESTAMP,
    IsResolved   TINYINT      DEFAULT 0,
    FOREIGN KEY (DeviceId) REFERENCES Devices(Id) ON DELETE CASCADE
);

-- ─────────────────────────────────────────────────────────
--  SETTINGS
-- ─────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS Settings (
    `Key`        VARCHAR(100) PRIMARY KEY,
    Value        TEXT         NOT NULL,
    LastModified DATETIME     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- ─────────────────────────────────────────────────────────
--  INDEXES (safe for all MySQL 8.0 versions)
-- ─────────────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS netscout_create_indexes;

DELIMITER //
CREATE PROCEDURE netscout_create_indexes()
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='netscout' AND table_name='users'         AND index_name='idx_users_email')     THEN CREATE INDEX idx_users_email     ON Users(Email);        END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='netscout' AND table_name='devices'       AND index_name='idx_devices_scanid')   THEN CREATE INDEX idx_devices_scanid  ON Devices(ScanId);      END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='netscout' AND table_name='devices'       AND index_name='idx_devices_ip')       THEN CREATE INDEX idx_devices_ip      ON Devices(IpAddress);   END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='netscout' AND table_name='openports'     AND index_name='idx_ports_deviceid')   THEN CREATE INDEX idx_ports_deviceid  ON OpenPorts(DeviceId);  END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='netscout' AND table_name='securityissues'AND index_name='idx_issues_deviceid')  THEN CREATE INDEX idx_issues_deviceid ON SecurityIssues(DeviceId); END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='netscout' AND table_name='scanresults'   AND index_name='idx_scans_userid')     THEN CREATE INDEX idx_scans_userid    ON ScanResults(UserId);  END IF;
END //
DELIMITER ;

CALL netscout_create_indexes();
DROP PROCEDURE IF EXISTS netscout_create_indexes;

-- ─────────────────────────────────────────────────────────
--  DEFAULT ADMIN USER
--  Password: admin123 (SHA-256)
-- ─────────────────────────────────────────────────────────
INSERT IGNORE INTO Users (Email, Password, Username, IsAdmin)
VALUES (
    'admin@netscout.com',
    '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',
    'Admin',
    1
);

SELECT 'Database setup complete!' AS Status;
SELECT CONCAT('Users: ', COUNT(*)) AS Info FROM Users;
