INSERT INTO [dbo].[ExportGuides]
           ([ID],[Name],[Mode],[ExportFileType],[ProfileMask],[ProfileType])
     VALUES
           (9008,'Exportación avanzada de tareas',1,2,'adv_Tasks',5)
GO

UPDATE sysroParameters SET Data='346' WHERE ID='DBVersion'
GO


