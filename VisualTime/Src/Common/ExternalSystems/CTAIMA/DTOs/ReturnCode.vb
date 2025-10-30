Namespace CTAIMA.Core.DTOs
    Public Enum ReturnCode
        _OK = 0                     ' OK
        _XmlError = 1               ' (El xml de entrada no está bien formateado)
        _ConnectionError = 2        ' (Error de usuario y contraseña)
        _SqlError = 3               ' (Error de conexión a SQL)
        _InvalidDateTimeInterval = 4 ' (Periodo de fechas incorrecto)
        _AnotherExistInDate = 5     ' (Existe otra ausencia en la fecha)
        _DateOutOfContract = 6      ' (Empleado no tiene contrato en la fecha solicitada)
        _InFreezeDate = 7           ' (La base de datos no admite cambios en el periodo seleccionado)
        _InvalidDuration = 8        ' (En una ausencia por horas el periodo no es correcto)
        _InvalidEmployee = 9        ' (El empleado indicado no existe)
        _InvalidCause = 10          ' (La justificación indicada no existe)
        _InvalidZone = 11           ' (La zona del fichaje no existe)
        _InvalidDirection = 12      ' (La dirección del fichaje es incorrecta)
        _InvalidCode = 13           ' (El código de fichaje no existe)
        _LoginError = 14            ' (Error de incio de sesión)
        _UnknownError = 15          ' (Error de incio de sesión)
        _PunchExists = 16           ' (Fichaje ya existente)
        _InvalidOperation = 17      ' (Operación inválida)
        _RequestNotFound = 18       ' (Borrado de solicitud, no existente)
        _PunchNotFound = 19         ' (Borrado de fichajes, no existe)
        _PeriodNotPlanified = 20    ' (Periodo no planificado)
        _PeriodBloqued = 21         ' (Periodo bloqueado)
    End Enum
End Namespace