IF not exists (select * from [dbo].DocumentTemplates where Scope = 8 AND IsSystem =1)
    INSERT INTO dbo.DocumentTemplates(Id,Name,ShortName,Description,Scope,Area,AccessValidation,BeginValidity,EndValidity,ApprovalLevelRequired,EmployeeDeliverAllowed,SupervisorDeliverAllowed,IsSystem,DefaultExpiration,ExpirePrevious,Compulsory,LOPDAccessLevel,DaysBeforeDelete,Notifications,LeaveDocType)
    VALUES (ISNULL((SELECT (MAX(Id) + 1) FROM DocumentTemplates),1), 'BioCertificate','b_c','',8,1,0,'20200101','20790101',0,0,0,1,0,0,0,0,0,'',null)
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='918' WHERE ID='DBVersion'
GO
