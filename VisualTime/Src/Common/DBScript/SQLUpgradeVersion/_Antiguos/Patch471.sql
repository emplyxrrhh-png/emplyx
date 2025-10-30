
alter table [dbo].[Visitor_Fields_Value]
alter column [Value] nvarchar(max)
GO

alter table [dbo].[Visit_Fields_Value]
alter column [Value] nvarchar(max)
GO

drop table Visit_Print_Config
GO
CREATE TABLE [dbo].[Visit_Print_Config](
	[Id] int NOT NULL,
	[Value] [nvarchar](max)
)
GO
CREATE TABLE [dbo].[Visit_Legal_Texts](
	[Id] int NOT NULL,
	[Title1] [nvarchar](max),
	[Value1] [nvarchar](max),
	[Title2] [nvarchar](max),
	[Value2] [nvarchar](max)
)
GO
-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='471' WHERE ID='DBVersion'
GO
