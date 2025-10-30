-- No borréis esta línea
--Remember to add the file to the Updates folder 
CREATE CLUSTERED INDEX IX_TMPDetailedCalendarEmployee_Clustered
ON TMPDetailedCalendarEmployee_DailyIncidences (IDReportTask)
GO

CREATE NONCLUSTERED INDEX IX_TMPDetailedCalendarEmployee_ByEmpleadoFecha
ON TMPDetailedCalendarEmployee_DailyIncidences (IDEmpleado, Fecha)
INCLUDE (Tipo, Valor, HoraInicio, HoraFin)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='954' WHERE ID='DBVersion'
GO
