-- ========================================
-- VISUALTIME DATABASE - INITIAL SETUP
-- ========================================
-- Este script crea las tablas mínimas necesarias para que el sistema
-- de actualización automática de VisualTime pueda funcionar.

USE VisualTime;
GO

-- Tabla de parámetros del sistema
CREATE TABLE sysroParameters (
    ID NVARCHAR(50) PRIMARY KEY,
    Data NVARCHAR(MAX),
    DateCreated DATETIME DEFAULT GETDATE(),
    DateModified DATETIME DEFAULT GETDATE()
);

-- Tabla de passports (usuarios/roles)
CREATE TABLE sysroPassports (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255),
    Description NVARCHAR(255),
    Active BIT DEFAULT 1,
    DateCreated DATETIME DEFAULT GETDATE(),
    DateModified DATETIME DEFAULT GETDATE()
);

-- Tabla de parámetros avanzados
CREATE TABLE sysroLiveAdvancedParameters (
    ParameterName NVARCHAR(255) PRIMARY KEY,
    Value NVARCHAR(MAX),
    Description NVARCHAR(500),
    DateCreated DATETIME DEFAULT GETDATE(),
    DateModified DATETIME DEFAULT GETDATE()
);

-- Tabla de layouts de reportes
CREATE TABLE ReportLayouts (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    LayoutName NVARCHAR(255),
    Description NVARCHAR(MAX),
    IdPassport INT,
    CreationDate DATETIME DEFAULT GETDATE(),
    Parameters NVARCHAR(MAX),
    IsEmergencyReport BIT DEFAULT 0,
    LayoutXMLBinary VARBINARY(MAX),
    LayoutPreviewXMLBinary VARBINARY(MAX),
    Visible BIT DEFAULT 1,
    RequieredFeature NVARCHAR(255),
    RequiredFunctionalities NVARCHAR(255)
);

-- Tabla de configuración GUI
CREATE TABLE sysroGUI (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Edition NVARCHAR(50),
    Component NVARCHAR(255),
    Visible BIT DEFAULT 1,
    Active BIT DEFAULT 1
);

-- ========================================
-- DATOS INICIALES REQUERIDOS
-- ========================================

-- Insertar passport del sistema
INSERT INTO sysroPassports (Description, Name) 
VALUES ('@@ROBOTICS@@System', 'System');

-- Insertar parámetros de versión (necesarios para el sistema de updates)
INSERT INTO sysroParameters (ID, Data) VALUES ('DBVERSION', '1');
INSERT INTO sysroParameters (ID, Data) VALUES ('DXVERSION', '1');

-- Insertar configuración inicial de GUI
INSERT INTO sysroGUI (Edition, Component, Visible, Active) 
VALUES ('Standard', 'MainModule', 1, 1);

-- Parámetros avanzados iniciales
INSERT INTO sysroLiveAdvancedParameters (ParameterName, Value, Description) 
VALUES ('VTLive.DB.Updating', 'false', 'Indicates if database update is in progress');

INSERT INTO sysroLiveAdvancedParameters (ParameterName, Value, Description) 
VALUES ('Application.LogLevel', '0', 'Default application log level');

INSERT INTO sysroLiveAdvancedParameters (ParameterName, Value, Description) 
VALUES ('Application.TraceLevel', '0', 'Default application trace level');

PRINT 'VisualTime database initial setup completed successfully!';
PRINT 'The system will automatically create additional tables on first run.';

GO