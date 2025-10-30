Imports System.ComponentModel
Imports System.Runtime.Serialization

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs

    <DataContract>
    <Description("Information related to the request result.")>
    Public Enum ReturnCode

        <EnumMember>
        <Description("_OK")>
        _OK = 0                                 ' OK

        <EnumMember>
        <Description("Credentials or token incorrect")>
        _LoginError = 1                         ' (Error de incio de sesión)

        <EnumMember>
        <Description("Access denied to VisualTime api")>
        _PermissionDenied = 2                   ' (No dispone de permisos para acceder a ws)

        <EnumMember>
        <Description("Password or username is empty")>
        _PasswordUsernameEmpty = 3              ' (nombre o pcontraseña vacio)

        <EnumMember>
        <Description("Unknown error")>
        _UnknownError = 4                       ' (Error indefinido de aplicación)

        <EnumMember>
        <Description("Database connection failed")>
        _ConnectionError = 5                    ' (Error de usuario y contraseña)

        <EnumMember>
        <Description("Sql server error")>
        _SqlError = 6                           ' (Error de conexión a SQL)

        <EnumMember>
        <Description("Employee could not be created")>
        _InvalidEmployee = 7                    ' (No se ha podido crear el empleado)

        <EnumMember>
        <Description("Group could not be created")>
        _InvalidGroup = 8                       ' (No se ha podido crear el grupo)

        <EnumMember>
        <Description("Contract incorrect")>
        _InvalidContract = 9                    ' (No se ha podido crear el contrato)

        <EnumMember>
        <Description("Invalid card")>
        _InvalidCard = 10                       ' (La tarjeta indicada no se ha podido asignar)

        <EnumMember>
        <Description("Access authorizations could not be saved")>
        _AuthorizationError = 11                ' (No se han podido asignar las autorizaciones indicadas)

        <EnumMember>
        <Description("Employee saved without userfields information")>
        _SomeUserFieldsNotSaved = 12            ' (No se han podido guardar todos los campos de la ficha)

        <EnumMember>
        <Description("Invalid cause")>
        _InvalidCause = 13                      ' (La justificación no existe)

        <EnumMember>
        <Description("Contract is already closed")>
        _ContractAlreadyClosed = 14             ' (El contrato ya esta finalizado)

        <EnumMember>
        <Description("Labagree not found")>
        _InvalidLabAgree = 15                   ' (No se encuentra ningun convenio con el identificador)

        <EnumMember>
        <Description("Trying to modify data before close date")>
        _InvalidCloseDate = 16                  ' (Se están modificando datos en periodo de cierre)

        <EnumMember>
        <Description("Mandatory data is missing")>
        _MandatoryDataMissing = 17              ' (No se han informado todos los datos obligatorios)

        <EnumMember>
        <Description("Shift not exists")>
        _InvalidShift = 18                      ' (El horario no existe)

        <EnumMember>
        <Description("Absence is overlapping with another one")>
        _AbsenceOverlapping = 19                ' (La ausencia se solapa con otra ya existente)

        <EnumMember>
        <Description("Period is not correct")>
        _InvalidPeriod = 20                     ' (El periodo de fechas o horas es incorrecto)

        <EnumMember>
        <Description("Calendar could not be loaded")>
        _InvalidCalendarData = 21               ' (Calendario erroneo)

        <EnumMember>
        <Description("Accrual is incorrect")>
        _InvalidAccrualData = 22                ' (Saldo erroneo)

        <EnumMember>
        <Description("Wrong document type")>
        _InvalidDocumentType = 23               ' (Tipo de documento incorrecto)

        <EnumMember>
        <Description("Document data is incorrect")>
        _InvalidDocumentData = 24               ' (Documneto incorrecto)

        <EnumMember>
        <Description("Document could not be saved")>
        _ErrorSavingDocument = 25               ' (No se pudo guardar el documento)

        <EnumMember>
        <Description("Document can not be added by api")>
        _DocumentNotDeliverable = 26            ' (Los documentos de la plantilla no se pueden entregar electrónicamente

        <EnumMember>
        <Description("Document template does not exists")>
        _UnexistentDocumentTemplate = 27        ' (No exsite la plantilla de documento especificada

        <EnumMember>
        <Description("Document size exceeds maximum")>
        _DocumentTooBig = 28                    ' (Documento demasiado grande)

        <EnumMember>
        <Description("Invalid document title")>
        _InvalidDocumentTitle = 29              ' (Nombre de documento no permitido)

        <EnumMember>
        <Description("Could not assign document to employee")>
        _DocumentAlreadyExists = 30             ' (Documento ya asignado al empleado)

        <EnumMember>
        <Description("Invalid punch type")>
        _InvalidPunchType = 31                  ' (Tipo de fichaje no soportado)

        <EnumMember>
        <Description("Error saving punch")>
        _ErrorSavingPunch = 32                  ' (No se pudo guardar el fichaje)

        <EnumMember>
        <Description("Some data could not be loaded")>
        _ImcompleteDataRecovered = 33           ' No se pudieron recuperar algunos datos

        <EnumMember>
        <Description("Movility data incorrect")>
        _InvalidMovility = 34                   ' (Movilidad incorrecta)

        <EnumMember>
        <Description("Invalid contract history")>
        _InvalidContractHistory = 35            ' (Histórico de contratos incorrecto SAGE200c)

        <EnumMember>
        <Description("No contract data")>
        _NoContracts = 36                       ' (No hay contratos)

        <EnumMember>
        <Description("Absence data not found")>
        _AbenceNotFound = 37                    ' (Ausencia a actualizar o borrar no se ha encontrado)

        <EnumMember>
        <Description("Token is not valid")>
        _TokenValidation = 38                   ' Token no se puede validar

        <EnumMember>
        <Description("Begin date is not allowed to be changed")>
        _NotAllowedChangeContractBeginDate = 39 ' No se puede modificar la fecha de inicio del contrato

        <EnumMember>
        <Description("Missing company name")>
        _MissingCompanyName = 40                ' No se puedo recuperar el nombre de compañia (SOAP)

        <EnumMember>
        <Description("Invalid punch date")>
        _InvalidPunchDate = 41                  ' Fecha de fichaje incorrecta (los valores de dateTime -1/0/+1 días en base a shiftDate están permitidos)

        <EnumMember>
        <Description("Punch not found")>
        _PunchNotFound = 42                      ' No se ha encontrado el fichaje a actualizar

        <EnumMember>
        <Description("Unknown error updating punch")>
        _ErrorUpdatingPunch = 43                ' No se ha podido actualizar el fichaje

        <EnumMember>
        <Description("Punch not found")>
        _InvalidPunchKey = 44                   ' No está bien definida la clave del fichaje ("ID" key no encontrado en el objeto)

        <EnumMember>
        <Description("Object is in close date")>
        _InCloseDate = 45                       ' La fecha del fichaje en periodo de cierre

        <EnumMember>
        <Description("Object is in future date")>
        _FutureDateTime = 45                       ' La fecha del fichaje es futura

        <EnumMember>
        <Description("Photo data is incorrect")>
        _InvalidPhotoData = 46               ' (Foto incorrecta)

        <EnumMember>
        <Description("Terminal doesn't exist")>
        _TerminalNotExist = 47               ' (Terminal no existe)

        <EnumMember>
        <Description("Not compatible terminal")>
        _NotCompatibleTerminal = 48               ' (Terminal no compatible)

        <EnumMember>
        <Description("Invalid terminal behavior")>
        _InvalidTerminalBehavior = 49               ' (Terminal no admite AV)

        <EnumMember>
        <Description("Invalid login")>
        _InvalidLogin = 50               ' (Login de usuario no válido o repetido)

        <EnumMember>
        <Description("Invalid Pin Length")>
        _InvalidPinLength = 51               ' (Pin no es numerico o no esta entre 4 y 6 digitos)

        <EnumMember>
        <Description("Manual cannot be false")>
        _InvalidManualValue = 52               ' (Las justificaciones creadas a través de la API siempre deben tener Manual = true)

        <EnumMember>
        <Description("Relevant data would be deleted on contract modification")>
        _ContractDataProtected = 53   ' (No se pueden modificar contratos que impliquen borrado de datos relevantes)

        <EnumMember>
        <Description("The employee used as a template for the planning copy cannot be the employee being created")>
        _NewEmployeeCannotBeSourceOfPlanning = 54   ' (El empleado usado como plantilla para la copia de la planificación no puede ser el empleado que se está creando)

        <EnumMember>
        <Description("Not exists manual cause")>
        _ManualCauseNotExists = 55   ' (No se pueden eliminar justificaciones automáticas o que no existen)

        <EnumMember>
        <Description("Document type must be setted")>
        _DocumentTypeMustBeSetted = 56 ' (Se debe indicar si se quieren obtener documentos de empresa o de empleado)

        <EnumMember>
        <Description("Document does not exists")>
        _UnexistentDocument = 57        ' (No exsite el documento especificado por su externalId)

        <EnumMember>
        <Description("Error deleting document")>
        _ErrorDeletingDocument = 58        ' (No se pudo borrar el documento)

        <EnumMember>
        <Description("ExternalId already exists")>
        _ExternalIdDuplicated = 59        ' (El externalid exsite en otro documento)

        <EnumMember>
        <Description("Document deleted but still exists some documents with the same externalid")>
        _DocumentDeletedButExternalIdStillExists = 60        ' (Se borró un documento con el externalid indicado, pero aún existen otros)

        <EnumMember>
        <Description("Error recovering data")>
        _ErrorRecoveringData = 61        ' (Error general al recuperar datos)

    End Enum

End Namespace