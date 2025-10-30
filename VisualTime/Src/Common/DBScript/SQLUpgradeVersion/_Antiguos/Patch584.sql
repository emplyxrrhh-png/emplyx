ALTER TABLE sysropassports DROP CONSTRAINT DF__sysroPass__Busin__609D3A6E
GO

ALTER TABLE sysropassports ALTER COLUMN businessgrouplist nvarchar(max) 
GO

ALTER TABLE sysropassports ADD CONSTRAINT DF__sysroPass__Busin__609D3A6E DEFAULT ('') FOR businessgrouplist
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO
UPDATE sysroParameters SET Data='584' WHERE ID='DBVersion'
GO
