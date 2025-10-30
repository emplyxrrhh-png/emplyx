-- Añade los registros necesarios para crear la exportación de la nómina de Logic.
INSERT INTO Exports(IDExport,Description,Path) 
	VALUES (1,'Nómina Logic Control','C:\EXPORTS.TXT')
GO

INSERT INTO ExportsFormats(IDExport,IdOrder,TypeField,Value,PropertyField,Length,Padding,SeparatorDec,NumDec,DataFormat) 
	VALUES (1,1,0,'-E',0,0,'','',0,'')
GO
INSERT INTO ExportsFormats(IDExport,IdOrder,TypeField,Value,PropertyField,Length,Padding,SeparatorDec,NumDec,DataFormat) 
	VALUES (1,2,8,'',0,4,'','',0,'yyyy')
GO
INSERT INTO ExportsFormats(IDExport,IdOrder,TypeField,Value,PropertyField,Length,Padding,SeparatorDec,NumDec,DataFormat) 
	VALUES (1,3,8,'',0,2,'','',0,'mm')
GO
INSERT INTO ExportsFormats(IDExport,IdOrder,TypeField,Value,PropertyField,Length,Padding,SeparatorDec,NumDec,DataFormat) 
	VALUES (1,4,7,'',0,7,' ','',0,'')
GO
INSERT INTO ExportsFormats(IDExport,IdOrder,TypeField,Value,PropertyField,Length,Padding,SeparatorDec,NumDec,DataFormat) 
	VALUES (1,5,1,'',0,4,'0','',0,'')
GO
INSERT INTO ExportsFormats(IDExport,IdOrder,TypeField,Value,PropertyField,Length,Padding,SeparatorDec,NumDec,DataFormat) 
	VALUES (1,6,4,'',1,3,'0','',0,'')
GO
INSERT INTO ExportsFormats(IDExport,IdOrder,TypeField,Value,PropertyField,Length,Padding,SeparatorDec,NumDec,DataFormat) 
	VALUES (1,7,3,'',2,11,'0','',2,'')
GO

INSERT INTO ExportsPeriods(IDExport,PeriodType,DateInf,DateSup) 
	VALUES (1,3,'01/01/2002','31/01/2002')
GO

ALTER TABLE [dbo].[Employees] ADD [USR_Codigo_Empresa] NVARCHAR(7)
GO

--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='162' WHERE ID='DBVersion'
GO
