-- Dummy Data
CREATE TABLE TMPDummyRulesGroups (
    DummyData NVARCHAR(MAX)
);

DELETE TMPDummyRulesGroups
GO

INSERT INTO TMPDummyRulesGroups (DummyData) VALUES (
'[
  {
    "Id": 1001,
    "Name": "Grupo de Reglas de Justificación",
    "EditionStatus": 3,
    "Rules": [
      {
        "Id": 1,
        "Name": "Justificación por Enfermedad",
        "Description": "Automatiza la justificación de ausencias por enfermedad con certificado médico",
        "Type": 2,
        "TypeDescription": "Justification",
        "Group": {
          "Id": 1001,
          "Name": "Grupo de Reglas de Justificación"
		    },
        "Tags": ["enfermedad", "justificación", "automática"],
        "EditionStatus": 3,
        "RuleDefinitions": [
          {
            "Id": 101,
            "Description": "Configuración inicial",
            "IdRule": 1,
            "EditionStatus": 3,
            "EmployeeContext": {
              "UserFields": "",
              "ComposeFilter": "All",
              "ComposeMode": "OR",
              "Filters": "",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 501,
                "ShiftName": "Turno Mañana",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 1,
                "EditionStatus": 3,                
                "Order": 1
              },
              {
                "IdShift": 502,
                "ShiftName": "Turno Tarde",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 1,
                "Order": 2
              },
              {
                "IdShift": 504,
                "ShiftName": "Turno Fin de Semana",
                "IdShiftGroup": 51,
                "ShiftGroupName": "Turnos Especiales",
                "IdRule": 1,
                "Order": 3
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "admin",
            "ModifiedDate": "2025-01-15T10:30:00",
            "EffectiveFrom": "2025-01-15T00:00:00",
            "EffectiveUntil": "2025-03-14T23:59:59"
          },
          {
            "Id": 102,
            "Description": "Actualización para incluir certificado digital",
            "IdRule": 1,
            "EditionStatus": 3,
            "EmployeeContext": {
              "UserFields": "",
              "ComposeFilter": "All",
              "ComposeMode": "OR",
              "Filters": "",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 501,
                "ShiftName": "Turno Mañana",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "EditionStatus": 3,                                
                "IdRule": 1,
                "Order": 1
              },
              {
                "IdShift": 502,
                "ShiftName": "Turno Tarde",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 1,
                "Order": 2
              },
              {
                "IdShift": 504,
                "ShiftName": "Turno Fin de Semana",
                "IdShiftGroup": 51,
                "ShiftGroupName": "Turnos Especiales",
                "IdRule": 1,
                "Order": 3
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "admin",
            "ModifiedDate": "2025-03-15T09:45:00",
            "EffectiveFrom": "2025-03-15T00:00:00",
            "EffectiveUntil": null
          }
        ]
      },
      {
        "Id": 2,
        "Name": "Justificación por Asuntos Personales",
        "Description": "Justificación de ausencias por motivos personales",
        "Type": 2,
        "TypeDescription": "Justification",
        "Group": {
          "Id": 1001,
          "Name": "Grupo de Reglas de Justificación"
		    },
        "Tags": ["personal", "justificación", "automática"],
        "EditionStatus": 3,        
        "RuleDefinitions": [
          {
            "Id": 103,
            "Description": "Configuración inicial",
            "IdRule": 2,
            "EditionStatus": 3,
            "EmployeeContext": {
              "UserFields": "Antiguedad>1",
              "ComposeFilter": "Seniority",
              "ComposeMode": "AND",
              "Filters": ">1",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 501,
                "ShiftName": "Turno Mañana",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "EditionStatus": 3,
                "IdRule": 2,
                "Order": 1
              },
              {
                "IdShift": 502,
                "ShiftName": "Turno Tarde",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 2,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "supervisor",
            "ModifiedDate": "2025-01-20T09:15:00",
            "EffectiveFrom": "2025-02-01T00:00:00",
            "EffectiveUntil": "2025-04-30T23:59:59"
          },
          {
            "Id": 104,
            "Description": "Actualización límite de días personales",
            "IdRule": 2,
            "EditionStatus": 3,
            "EmployeeContext": {
              "UserFields": "Antiguedad>1",
              "ComposeFilter": "Seniority",
              "ComposeMode": "AND",
              "Filters": ">1",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 501,
                "ShiftName": "Turno Mañana",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 2,
                "EditionStatus": 3,
                "Order": 1
              },
              {
                "IdShift": 502,
                "ShiftName": "Turno Tarde",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 2,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "hr_manager",
            "ModifiedDate": "2025-05-01T08:30:00",
            "EffectiveFrom": "2025-05-01T00:00:00",
            "EffectiveUntil": null
          }
        ]
      }
    ]
  },
  {
    "Id": 1002,
    "Name": "Grupo de Reglas Diarias",
    "EditionStatus": 3,    
    "Rules": [
      {
        "Id": 3,
        "Name": "Control de Horas Extras",
        "Description": "Regla diaria para control y autorización de horas extras",
        "Type": 3,
        "TypeDescription": "Daily",
        "Group": {
          "Id": 1002,
          "Name": "Grupo de Reglas Diarias"
		    },
        "Tags": ["horas extras", "control", "diaria"],
        "EditionStatus": 3,        
        "RuleDefinitions": [
          {
            "Id": 105,
            "Description": "Configuración inicial",
            "IdRule": 3,
            "EmployeeContext": {
              "UserFields": "Departamento=Producción",
              "ComposeFilter": "Department",
              "ComposeMode": "AND",
              "Filters": "Producción",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 501,
                "ShiftName": "Turno Mañana",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 3,
                "EditionStatus": 3,
                "Order": 1
              },
              {
                "IdShift": 503,
                "ShiftName": "Turno Noche",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 3,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "manager",
            "ModifiedDate": "2025-02-10T14:30:00",
            "EffectiveFrom": "2025-03-01T00:00:00",
            "EffectiveUntil": "2025-06-30T23:59:59"
          },
          {
            "Id": 106,
            "Description": "Actualización para límite de horas extras",
            "IdRule": 3,
            "EditionStatus": 3,
            "EmployeeContext": {
              "UserFields": "Departamento=Producción",
              "ComposeFilter": "Department",
              "ComposeMode": "AND",
              "Filters": "Producción",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 501,
                "ShiftName": "Turno Mañana",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 3,
                "EditionStatus": 3,
                "Order": 1
              },
              {
                "IdShift": 503,
                "ShiftName": "Turno Noche",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 3,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "hr_director",
            "ModifiedDate": "2025-06-25T16:20:00",
            "EffectiveFrom": "2025-07-01T00:00:00",
            "EffectiveUntil": null
          }
        ]
      },
      {
        "Id": 4,
        "Name": "Cómputo de Descansos",
        "Description": "Regla diaria para computar tiempo de descanso",
        "Type": 3,
        "TypeDescription": "Daily",
        "GroupId": 1002,
        "Tags": ["descansos", "cómputo", "diaria"],
        "EditionStatus": 3,        
        "RuleDefinitions": [
          {
            "Id": 107,
            "Description": "Configuración inicial",
            "IdRule": 4,
            "EditionStatus": 3,
            "EmployeeContext": {
              "UserFields": "",
              "ComposeFilter": "All",
              "ComposeMode": "OR",
              "Filters": "",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 502,
                "ShiftName": "Turno Tarde",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 4,
                "EditionStatus": 3,
                "Order": 1
              },
              {
                "IdShift": 503,
                "ShiftName": "Turno Noche",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 4,
                "Order": 2
              },
              {
                "IdShift": 505,
                "ShiftName": "Turno Rotativo",
                "IdShiftGroup": 51,
                "ShiftGroupName": "Turnos Especiales",
                "IdRule": 4,
                "Order": 3
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "supervisor",
            "ModifiedDate": "2025-01-25T11:45:00",
            "EffectiveFrom": "2025-02-01T00:00:00",
            "EffectiveUntil": "2025-05-14T23:59:59"
          },
          {
            "Id": 108,
            "Description": "Actualización descansos por convenio",
            "IdRule": 4,
            "EditionStatus": 3,
            "EmployeeContext": {
              "UserFields": "",
              "ComposeFilter": "All",
              "ComposeMode": "OR",
              "Filters": "",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 502,
                "ShiftName": "Turno Tarde",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 4,
                "EditionStatus": 3,
                "Order": 1
              },
              {
                "IdShift": 503,
                "ShiftName": "Turno Noche",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 4,
                "Order": 2
              },
              {
                "IdShift": 505,
                "ShiftName": "Turno Rotativo",
                "IdShiftGroup": 51,
                "ShiftGroupName": "Turnos Especiales",
                "IdRule": 4,
                "Order": 3
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "supervisor",
            "ModifiedDate": "2025-05-10T13:20:00",
            "EffectiveFrom": "2025-05-15T00:00:00",
            "EffectiveUntil": "2025-09-30T23:59:59"
          },
          {
            "Id": 109,
            "Description": "Nueva política de descansos escalonados",
            "IdRule": 4,
            "EditionStatus": 3,
            "EmployeeContext": {
              "UserFields": "",
              "ComposeFilter": "All",
              "ComposeMode": "OR",
              "Filters": "",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 502,
                "ShiftName": "Turno Tarde",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 4,
                "EditionStatus": 3,
                "Order": 1
              },
              {
                "IdShift": 503,
                "ShiftName": "Turno Noche",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 4,
                "Order": 2
              },
              {
                "IdShift": 505,
                "ShiftName": "Turno Rotativo",
                "IdShiftGroup": 51,
                "ShiftGroupName": "Turnos Especiales",
                "IdRule": 4,
                "Order": 3
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "hr_director",
            "ModifiedDate": "2025-09-20T11:15:00",
            "EffectiveFrom": "2025-10-01T00:00:00",
            "EffectiveUntil": null
          }
        ]
      },
      {
        "Id": 5,
        "Name": "Control de Asistencia",
        "Description": "Regla diaria para control de asistencia y puntualidad",
        "Type": 3,
        "TypeDescription": "Daily",
        "GroupId": 1002,
        "Tags": ["asistencia", "puntualidad", "diaria"],
        "EditionStatus": 3,        
        "RuleDefinitions": [
          {
            "Id": 110,
            "Description": "Configuración inicial",
            "IdRule": 5,
            "EditionStatus": 3,
            "EmployeeContext": {
              "UserFields": "Categoria=Operario",
              "ComposeFilter": "Category",
              "ComposeMode": "AND",
              "Filters": "Operario",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 501,
                "ShiftName": "Turno Mañana",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 5,
                "EditionStatus": 3,
                "Order": 1
              },
              {
                "IdShift": 504,
                "ShiftName": "Turno Fin de Semana",
                "IdShiftGroup": 51,
                "ShiftGroupName": "Turnos Especiales",
                "IdRule": 5,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "admin",
            "ModifiedDate": "2025-03-05T09:30:00",
            "EffectiveFrom": "2025-03-15T00:00:00",
            "EffectiveUntil": "2025-07-31T23:59:59"
          },
          {
            "Id": 111,
            "Description": "Actualización política flexibilidad",
            "IdRule": 5,
            "EditionStatus": 3,
            "EmployeeContext": {
              "UserFields": "Categoria=Operario",
              "ComposeFilter": "Category",
              "ComposeMode": "AND",
              "Filters": "Operario",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 501,
                "ShiftName": "Turno Mañana",
                "IdShiftGroup": 50,
                "ShiftGroupName": "Turnos Estándar",
                "IdRule": 5,
                "EditionStatus": 3,
                "Order": 1
              },
              {
                "IdShift": 504,
                "ShiftName": "Turno Fin de Semana",
                "IdShiftGroup": 51,
                "ShiftGroupName": "Turnos Especiales",
                "IdRule": 5,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "hr_manager",
            "ModifiedDate": "2025-07-20T10:15:00",
            "EffectiveFrom": "2025-08-01T00:00:00",
            "EffectiveUntil": null
          }
        ]
      }
    ]
  }
]
'
)



-- MODELO DE DATOS PARA REGLAS
-- Deshabilitar claves foráneas para evitar errores al eliminar tablas relacionadas
ALTER TABLE [dbo].[Rules] DROP CONSTRAINT [FK_Rules_RulesGroups];
ALTER TABLE [dbo].[RulesDefinitions] DROP CONSTRAINT [FK_RulesDefinitions_Rules];
ALTER TABLE [dbo].[RulesShifts] DROP CONSTRAINT [FK_RulesShifts_RulesDefinitions];
ALTER TABLE [dbo].[EntityTags] DROP CONSTRAINT [FK_EntityTags_Tags];
ALTER TABLE [dbo].[RuleConditions] DROP CONSTRAINT [FK_RuleConditions_RulesDefinitions];
ALTER TABLE [dbo].[RuleActions] DROP CONSTRAINT [FK_RuleActions_RulesDefinitions];

-- Eliminar índices creados
--DROP INDEX [IX_RuleShifts_IdRRuleDefinition] ON [dbo].[RuleShifts];
DROP INDEX [IX_RulesShifts_IdShift_Order] ON [dbo].[RulesShifts];
DROP INDEX [IX_EntityTags_EntityType_EntityId] ON [dbo].[EntityTags];
DROP INDEX [IX_EntityTags_TagId] ON [dbo].[EntityTags];

-- Eliminar tablas en el orden correcto
DROP TABLE IF EXISTS [dbo].[RuleActions];
DROP TABLE IF EXISTS [dbo].[RuleConditions];
DROP TABLE IF EXISTS [dbo].[RulesShifts];
DROP TABLE IF EXISTS [dbo].[RulesDefinitions];
DROP TABLE IF EXISTS [dbo].[EntityTags];
DROP TABLE IF EXISTS [dbo].[Tags];
DROP TABLE IF EXISTS [dbo].[Rules];
DROP TABLE IF EXISTS [dbo].[RulesGroups];

-- Tabla para almacenar los grupos de reglas
CREATE TABLE [dbo].[RulesGroups] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(200) NOT NULL,
	CONSTRAINT [PK_RulesGroups] PRIMARY KEY ([Id]),
);

-- Tabla para almacenar las reglas
CREATE TABLE [dbo].[Rules] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Type] INT NOT NULL, -- Basado en eRuleType 
    [TypeDescription] NVARCHAR(100) NULL,
    [GroupId] INT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Rules] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Rules_RulesGroups] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[RulesGroups] ([Id])
);

CREATE TABLE [dbo].[RulesDefinitions] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [IdRule] INT NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [EmployeeContext] NVARCHAR(MAX) NULL, 
    [XmlDefinition] NVARCHAR(MAX) NULL,
    [ModifiedBy] NVARCHAR(100) NOT NULL,
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [EffectiveFrom] DATETIME NOT NULL,
    [ChangeType] SMALLINT NULL,
    CONSTRAINT [PK_RulesDefinitions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RulesDefinitions_Rules] FOREIGN KEY ([IdRule]) REFERENCES [dbo].[Rules] ([Id])
);

-- Tabla para almacenar los horarios asociados al historial de reglas, incluyendo el orden
CREATE TABLE [dbo].[RulesShifts] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [IdRuleDefinition] INT NOT NULL,
    [IdShift] INT NOT NULL,
    [Order] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_RulesShifts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RulesShifts_RulesDefinitions] FOREIGN KEY ([IdRuleDefinition]) REFERENCES [dbo].[RulesDefinitions] ([Id])
);

-- Índice para mejorar el rendimiento de consultas por IdRuleDefinition
CREATE INDEX [IX_RulesShifts_IdRuleDefinition] ON [dbo].[RulesShifts]([IdRuleDefinition]);

-- Índice compuesto para mejorar el rendimiento en consultas que filtran por horario y ordenan por Order
CREATE INDEX [IX_RulesShifts_IdShift_Order] ON [dbo].[RulesShifts]([IdShift], [Order]);


-- Tabla principal de tags (catálogo general)
CREATE TABLE [dbo].[Tags] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Tags_Name] UNIQUE ([Name])
);

-- Tabla de relación entre tags y entidades
CREATE TABLE [dbo].[EntityTags] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [IdTag] INT NOT NULL,
    [EntityType] VARCHAR(50) NOT NULL, -- 'Rule', 'RuleGroup', 'Shift', etc.
    [EntityId] INT NOT NULL,
    CONSTRAINT [PK_EntityTags] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EntityTags_Tags] FOREIGN KEY ([IdTag]) REFERENCES [dbo].[Tags] ([Id]),
    CONSTRAINT [UQ_EntityTags_Entity] UNIQUE ([IdTag], [EntityType], [EntityId])
);

-- Índices para mejorar el rendimiento
CREATE INDEX [IX_EntityTags_EntityType_EntityId] ON [dbo].[EntityTags]([EntityType], [EntityId]);
CREATE INDEX [IX_EntityTags_TagId] ON [dbo].[EntityTags]([IdTag]);

-- Tabla para condiciones de reglas (a futuro ...)
CREATE TABLE [dbo].[RuleConditions] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [IdRuleDefinition] INT NOT NULL,
    [ConditionType] NVARCHAR(100) NOT NULL,
    [ConditionData] NVARCHAR(MAX) NOT NULL,
    [OrderIndex] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_RuleConditions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RuleConditions_RulesDefinitions] FOREIGN KEY ([IdRuleDefinition]) 
        REFERENCES [dbo].[RulesDefinitions] ([Id])
);

-- Tabla para condiciones de reglas (a futuro ...)
CREATE TABLE [dbo].[RuleActions] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [IdRuleDefinition] INT NOT NULL,
    [ActionType] NVARCHAR(100) NOT NULL,
    [ActionData] NVARCHAR(MAX) NOT NULL,
    [OrderIndex] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_RuleActions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RuleActions_RulesDefinitions] FOREIGN KEY ([IdRuleDefinition]) 
        REFERENCES [dbo].[RulesDefinitions] ([Id])
);