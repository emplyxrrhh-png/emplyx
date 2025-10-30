delete from sysroFeatures where (alias = 'Visits.Definition' or Alias = 'Visits.Program') and Type = 'E'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='625' WHERE ID='DBVersion'
GO
