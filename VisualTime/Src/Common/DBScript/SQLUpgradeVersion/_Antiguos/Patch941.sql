IF EXISTS (SELECT * FROM sysroParameters where id = 'CCode' AND cast(Data as nvarchar(20)) = 'vsl3177')
BEGIN

update ImportGuides set iscustom = 1 where id = 12

update ImportGuides set Version = 2 where id = 12

update ImportGuides set Concept = 'VSLWorkSheets' where id = 12

update ImportGuides set FeatureAliasID = 'Calendar' where id = 12

update ImportGuides set RequieredFunctionalities = 'Calendar.DataLink.Imports.Schedule' where id = 12

IF NOT EXISTS (SELECT 1 FROM ImportGuidesTemplates WHERE id = '9')
BEGIN
    

INSERT INTO [dbo].[ImportGuidesTemplates]
           ([ID]
           ,[IDParentGuide]
           ,[Name]
           ,[Profile]
           ,[Parameters]
           ,[PostProcessScript]
           ,[PreProcessScript])
     VALUES
           (9
           ,12
           ,'Carga de partes de trabajo'
           ,''
           ,NULL
           ,NULL
           ,NULL)

END
end
GO
-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='941' WHERE ID='DBVersion'
GO
