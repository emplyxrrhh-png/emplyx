ALTER TABLE LabAgree
ADD ExtraHoursConfiguration smallint NULL
GO
ALTER TABLE LabAgree
ADD ExtraHoursIDCauseSimples NVARCHAR(200) NULL
GO
ALTER TABLE LabAgree
ADD ExtraHoursIDCauseDoubles smallint NULL
GO
ALTER TABLE LabAgree
ADD ExtraHoursIDCauseTriples smallint NULL
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='830' WHERE ID='DBVersion'
GO
