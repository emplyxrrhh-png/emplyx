-- FUNCIONES PARA CONVERSION DE IMAGENES A TEXTO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[f_Base64ToBinary]
(
    @Base64 VARCHAR(MAX)
)
RETURNS VARBINARY(MAX)
AS
BEGIN
    DECLARE @Bin VARBINARY(MAX)
    SET @Bin = CAST(N'' AS XML).value('xs:base64Binary(sql:variable("@Base64"))', 'VARBINARY(MAX)')
    RETURN @Bin
END 
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[f_BinaryToBase64]
(
    @bin VARBINARY(MAX)
)
RETURNS VARCHAR(MAX)
AS
BEGIN
    DECLARE @Base64 VARCHAR(MAX)
    SET @Base64 = CAST(N'' AS XML).value('xs:base64Binary(xs:hexBinary(sql:variable("@bin")))', 'VARCHAR(MAX)')
    RETURN @Base64
END
GO

-- TABLAS TEMPORALES PARA MOVESCAPTURES Y ENTRIESCAPTURES
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TMPMovesCaptures](
	[IDMove] [numeric](16, 0) NULL,
	[InCapture] [text] NULL,
	[OutCapture] [text] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TMPEntriesCaptures](
	[IDEntry] [numeric](16, 0) NULL,
	[Capture] [text] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

-- PROCEDIMIENTOS ALMACENADOS QUE PASAN LA INFORMACIÓN DE FOTOS A LAS TABLAS TEMPORALES
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PopulateMovesCaptures] 
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	
	delete dbo.TMPMovesCaptures
	insert into dbo.TMPMovesCaptures (IDMove, InCapture, OutCapture) 
	select mc.IdMove, dbo.f_BinaryToBase64(mc.InCapture), dbo.f_BinaryToBase64(mc.OutCapture) from movescaptures mc, moves m
	where mc.idmove = m.id and (m.checked_in = 0 or m.checked_out = 0)
	SET ARITHABORT OFF;
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PopulateEntriesCaptures] 
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	delete dbo.TMPEntriesCaptures
	insert into dbo.TMPEntriesCaptures (IDEntry, Capture)
	select ec.IDEntry, dbo.f_BinaryToBase64(ec.Capture) from Entriescaptures ec, Entries e where ec.IDEntry = e.ID and e.checked = 0
	SET ARITHABORT OFF;
END
GO

-- Deshabilitamos modo Portal del Empleado en terminales mx7
delete sysroreadertemplates where UPPER(Type) = 'MX7' and ScopeMode Like '%EIP'
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='289' WHERE ID='DBVersion'
GO
