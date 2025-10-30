-- No borréis esta línea
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DailyCauses]') AND name = N'_dta_index_DailyCauses_c_6_2054298378__K1_K9_K2')
	DROP INDEX [_dta_index_DailyCauses_c_6_2054298378__K1_K9_K2] ON [dbo].[DailyCauses] WITH ( ONLINE = OFF )
GO

IF  NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DailyCauses]') AND name = N'_dta_index_DailyCauses_c_6_2054298378__K1_K9_K2')
	CREATE CLUSTERED INDEX [_dta_index_DailyCauses_c_6_2054298378__K1_K9_K2] ON [dbo].[DailyCauses]
	(
		[IDEmployee] ASC,
		[AccrualsRules] ASC,
		[Date] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='674' WHERE ID='DBVersion'
GO
