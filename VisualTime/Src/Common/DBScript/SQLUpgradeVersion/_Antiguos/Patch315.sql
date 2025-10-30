
--NUEVA FUNCION ESCALAR DE BDD
CREATE FUNCTION [dbo].[GetValueFromEmployeeUserFieldValues]
(		
@idEmployee int,
@FieldName nvarchar(50),
@Date smalldatetime
)

RETURNS nvarchar(100)

AS
BEGIN

DECLARE @Result nvarchar(100)

SELECT TOP 1 @Result = ISNULL(convert(nvarchar(100),[Value]),'') FROM EmployeeUserFieldValues				
WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND EmployeeUserFieldValues.FieldName = @FieldName AND 
EmployeeUserFieldValues.Date <= @Date ORDER BY Date DESC

RETURN ISNULL(@Result,'')

END
GO

-- Informe Calendario semanal para grupos
CREATE TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly](
	[IDEmployee] [numeric](18, 0) NOT NULL,
	[EMPLEADO] [nvarchar](50) NULL,
	[idGroup] [int] NOT NULL,
	[horasdia1] [numeric](9, 6) NOT NULL,
	[horasdia2] [numeric](9, 6) NOT NULL,
	[horasdia3] [numeric](9, 6) NOT NULL,
	[horasdia4] [numeric](9, 6) NOT NULL,
	[horasdia5] [numeric](9, 6) NOT NULL,
	[horasdia6] [numeric](9, 6) NOT NULL,
	[horasdia7] [numeric](9, 6) NOT NULL,
	[TotalHoras] [numeric](9, 6) NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEEWeekly_horasdia1]  DEFAULT ((0)) FOR [horasdia1]
GO

ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEEWeekly_horasdia2]  DEFAULT ((0)) FOR [horasdia2]
GO

ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEEWeekly_horasdia3]  DEFAULT ((0)) FOR [horasdia3]
GO

ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEEWeekly_horasdia4]  DEFAULT ((0)) FOR [horasdia4]
GO

ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEEWeekly_horasdia5]  DEFAULT ((0)) FOR [horasdia5]
GO

ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEEWeekly_horasdia6]  DEFAULT ((0)) FOR [horasdia6]
GO

ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEEWeekly_horasdia7]  DEFAULT ((0)) FOR [horasdia7]
GO

ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEEWeekly_TotalHoras]  DEFAULT ((0)) FOR [TotalHoras]
GO

--Para informe de planificación por franjas Escada que pasa a estandar
CREATE TABLE [dbo].[TmpSchedulingLayerResume](
	[IDGroup] [numeric](18, 0) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Layer0_Value1] [numeric](10, 2) NULL,
	[Layer0_Value2] [numeric](16, 6) NULL,
	[Layer0_Value3] [numeric](16, 6) NULL,
	[Layer0_Value4] [numeric](16, 6) NULL,
	[Layer0_Value5] [numeric](16, 6) NULL,
	[Layer0_Value6] [numeric](16, 6) NULL,
	[Layer0_Value7] [numeric](16, 6) NULL,
	[Layer0_Value8] [numeric](16, 6) NULL,
	[Layer0_Value9] [numeric](16, 6) NULL,
	[Layer0_Value10] [numeric](16, 6) NULL,
	[Layer0_Value11] [numeric](16, 6) NULL,
	[Layer0_Value12] [numeric](16, 6) NULL,
	[Layer0_Value13] [numeric](16, 6) NULL,
	[Layer0_Value14] [numeric](16, 6) NULL,
	[Layer0_Value15] [numeric](16, 6) NULL,
	[Layer0_Value16] [numeric](16, 6) NULL,
	[Layer0_Value17] [numeric](16, 6) NULL,
	[Layer0_Value18] [numeric](16, 6) NULL,
	[Layer0_Value19] [numeric](16, 6) NULL,
	[Layer0_Value20] [numeric](16, 6) NULL,
	[Layer0_Value21] [numeric](16, 6) NULL,
	[Layer0_Value22] [numeric](16, 6) NULL,
	[Layer0_Value23] [numeric](16, 6) NULL,
	[Layer0_Value24] [numeric](16, 6) NULL,
	[Layer0_Value25] [numeric](16, 6) NULL,
	[Layer0_Value26] [numeric](16, 6) NULL,
	[Layer0_Value27] [numeric](16, 6) NULL,
	[Layer0_Value28] [numeric](16, 6) NULL,
	[Layer0_Value29] [numeric](16, 6) NULL,
	[Layer0_Value30] [numeric](16, 6) NULL,
	[Layer0_Value31] [numeric](16, 6) NULL,
	[Layer1_Value1] [numeric](10, 2) NULL,
	[Layer1_Value2] [numeric](16, 6) NULL,
	[Layer1_Value3] [numeric](16, 6) NULL,
	[Layer1_Value4] [numeric](16, 6) NULL,
	[Layer1_Value5] [numeric](16, 6) NULL,
	[Layer1_Value6] [numeric](16, 6) NULL,
	[Layer1_Value7] [numeric](16, 6) NULL,
	[Layer1_Value8] [numeric](16, 6) NULL,
	[Layer1_Value9] [numeric](16, 6) NULL,
	[Layer1_Value10] [numeric](16, 6) NULL,
	[Layer1_Value11] [numeric](16, 6) NULL,
	[Layer1_Value12] [numeric](16, 6) NULL,
	[Layer1_Value13] [numeric](16, 6) NULL,
	[Layer1_Value14] [numeric](16, 6) NULL,
	[Layer1_Value15] [numeric](16, 6) NULL,
	[Layer1_Value16] [numeric](16, 6) NULL,
	[Layer1_Value17] [numeric](16, 6) NULL,
	[Layer1_Value18] [numeric](16, 6) NULL,
	[Layer1_Value19] [numeric](16, 6) NULL,
	[Layer1_Value20] [numeric](16, 6) NULL,
	[Layer1_Value21] [numeric](16, 6) NULL,
	[Layer1_Value22] [numeric](16, 6) NULL,
	[Layer1_Value23] [numeric](16, 6) NULL,
	[Layer1_Value24] [numeric](16, 6) NULL,
	[Layer1_Value25] [numeric](16, 6) NULL,
	[Layer1_Value26] [numeric](16, 6) NULL,
	[Layer1_Value27] [numeric](16, 6) NULL,
	[Layer1_Value28] [numeric](16, 6) NULL,
	[Layer1_Value29] [numeric](16, 6) NULL,
	[Layer1_Value30] [numeric](16, 6) NULL,
	[Layer1_Value31] [numeric](16, 6) NULL,
	[Layer2_Value1] [numeric](10, 2) NULL,
	[Layer2_Value2] [numeric](16, 6) NULL,
	[Layer2_Value3] [numeric](16, 6) NULL,
	[Layer2_Value4] [numeric](16, 6) NULL,
	[Layer2_Value5] [numeric](16, 6) NULL,
	[Layer2_Value6] [numeric](16, 6) NULL,
	[Layer2_Value7] [numeric](16, 6) NULL,
	[Layer2_Value8] [numeric](16, 6) NULL,
	[Layer2_Value9] [numeric](16, 6) NULL,
	[Layer2_Value10] [numeric](16, 6) NULL,
	[Layer2_Value11] [numeric](16, 6) NULL,
	[Layer2_Value12] [numeric](16, 6) NULL,
	[Layer2_Value13] [numeric](16, 6) NULL,
	[Layer2_Value14] [numeric](16, 6) NULL,
	[Layer2_Value15] [numeric](16, 6) NULL,
	[Layer2_Value16] [numeric](16, 6) NULL,
	[Layer2_Value17] [numeric](16, 6) NULL,
	[Layer2_Value18] [numeric](16, 6) NULL,
	[Layer2_Value19] [numeric](16, 6) NULL,
	[Layer2_Value20] [numeric](16, 6) NULL,
	[Layer2_Value21] [numeric](16, 6) NULL,
	[Layer2_Value22] [numeric](16, 6) NULL,
	[Layer2_Value23] [numeric](16, 6) NULL,
	[Layer2_Value24] [numeric](16, 6) NULL,
	[Layer2_Value25] [numeric](16, 6) NULL,
	[Layer2_Value26] [numeric](16, 6) NULL,
	[Layer2_Value27] [numeric](16, 6) NULL,
	[Layer2_Value28] [numeric](16, 6) NULL,
	[Layer2_Value29] [numeric](16, 6) NULL,
	[Layer2_Value30] [numeric](16, 6) NULL,
	[Layer2_Value31] [numeric](16, 6) NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TmpSchedulingLayerResume] ADD  CONSTRAINT [PK_TmpSchedulingLayerResume]  PRIMARY KEY NONCLUSTERED 
(
	[IDGroup] ASC,
	[Date] ASC
)
GO
-- Fin informe de planificación por franjas Escada que pasa a estandar


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='315' WHERE ID='DBVersion'
GO

