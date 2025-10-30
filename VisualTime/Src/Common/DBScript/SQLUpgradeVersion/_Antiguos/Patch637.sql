CREATE TABLE [dbo].[ShiftsPunchesPattern](
	[IDShift] [smallint] NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[PunchType] [smallint] NOT NULL
 CONSTRAINT [PK_ShiftsPunchesPattern] PRIMARY KEY NONCLUSTERED 
(
	[IDShift] ASC,
	[DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ShiftsPunchesPattern]  WITH NOCHECK ADD  CONSTRAINT [FK_ShiftsPunchesPattern_Shifts] FOREIGN KEY([IDShift])
REFERENCES [dbo].[Shifts] ([ID])
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='637' WHERE ID='DBVersion'
GO
