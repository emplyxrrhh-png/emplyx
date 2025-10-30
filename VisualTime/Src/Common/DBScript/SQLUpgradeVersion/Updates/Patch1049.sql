-- No borréis esta línea


-- MODELO DE DATOS PARA REGLAS

-- Tabla para almacenar los grupos de reglas
CREATE TABLE [dbo].[RulesGroups] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(200) NOT NULL,
	CONSTRAINT [PK_RulesGroups] PRIMARY KEY ([Id]),
);
GO

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
GO

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
GO

-- Tabla para almacenar los horarios asociados al historial de reglas, incluyendo el orden
CREATE TABLE [dbo].[RulesShifts] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [IdRuleDefinition] INT NOT NULL,
    [IdShift] INT NOT NULL,
    [Order] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_RulesShifts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RulesShifts_RulesDefinitions] FOREIGN KEY ([IdRuleDefinition]) REFERENCES [dbo].[RulesDefinitions] ([Id])
);
GO

-- Índice para mejorar el rendimiento de consultas por IdRuleDefinition
CREATE INDEX [IX_RulesShifts_IdRuleDefinition] ON [dbo].[RulesShifts]([IdRuleDefinition]);
GO

-- Índice compuesto para mejorar el rendimiento en consultas que filtran por horario y ordenan por Order
CREATE INDEX [IX_RulesShifts_IdShift_Order] ON [dbo].[RulesShifts]([IdShift], [Order]);
GO


-- Tabla principal de tags (catálogo general)
CREATE TABLE [dbo].[Tags] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Tags_Name] UNIQUE ([Name])
);
GO

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
GO

-- Índices para mejorar el rendimiento
CREATE INDEX [IX_EntityTags_EntityType_EntityId] ON [dbo].[EntityTags]([EntityType], [EntityId]);
GO
CREATE INDEX [IX_EntityTags_TagId] ON [dbo].[EntityTags]([IdTag]);
GO

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
GO

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
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1049' WHERE ID='DBVersion'
GO
