alter table dbo.Shifts ADD 
	IDConceptRequestNextYear [smallint] NULL default(0)
GO

update dbo.Shifts set IDConceptRequestNextYear= 0 where IDConceptRequestNextYear is null AND ShiftType=2
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='613' WHERE ID='DBVersion'
GO
