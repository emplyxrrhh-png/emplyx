Namespace DTOs

    Public Class ErrorCodes
        Public Const OK As Integer = 0
        Public Const NO_SESSION As Integer = -1
        Public Const BAD_CREDENTIALS As Integer = -2
        Public Const NOT_FOUND As Integer = -3
        Public Const GENERAL_ERROR As Integer = -4
        Public Const WRONG_MEDIA_TYPE As Integer = -5
        Public Const NOT_LICENSED As Integer = -6
        Public Const SERVER_NOT_RUNNING As Integer = -7
        Public Const NO_LIVE_PORTAL As Integer = -8

        Public Const PUNCH_ERROR_DO_SEQUENCE As Integer = -9
        Public Const PUNCH_ERROR_SEQ_STATUS_OK As Integer = -10
        Public Const PUNCH_ERROR_REPEAT_IN As Integer = -11
        Public Const PUNCH_ERROR_REPEAT_OUT As Integer = -12
        Public Const PUNCH_ERROR_MAX_SEQ_OK As Integer = -13
        Public Const PUNCH_ERROR_MAX_SEQ_ERR As Integer = -14
        Public Const PUNCH_ERROR_NO_SEQUENCE As Integer = -15
        Public Const PUNCH_ERROR_DELETING As Integer = -16
        Public Const PUNCH_ERROR_SAVING As Integer = -17
        Public Const PUNCH_ERROR_FORBIDDEN As Integer = -18

        Public Const REQUEST_PUNCH_ERROR As Integer = -19
        Public Const FORBID_INCORRECT_DATE_BEFORE_PUNCH = -20
        Public Const FORBID_INCORRECT_DATE_FUTURE = -21
        Public Const FORBID_ERROR_SAVE_PUNCH = -22
        Public Const FORBID_ERROR_DO_SEQUENCE = -23

        Public Const USER_FIELDS_ACCESS_DENIED As Integer = -24

        Public Const REQUEST_ERROR_ConnectionError As Integer = -25
        Public Const REQUEST_ERROR_SqlError As Integer = -26
        Public Const REQUEST_ERROR_NoDeleteBecauseNotPending As Integer = -27
        Public Const REQUEST_ERROR_IncorrectDates As Integer = -28
        Public Const REQUEST_ERROR_NoApprovePermissions As Integer = -29
        Public Const REQUEST_ERROR_UserFieldNoRequestVisible As Integer = -30
        Public Const REQUEST_ERROR_NoApproveRefuseLevelOfAuthorityRequired As Integer = -31
        Public Const REQUEST_ERROR_UserFieldValueSaveError As Integer = -32
        Public Const REQUEST_ERROR_InvalidPassport As Integer = -33
        Public Const REQUEST_ERROR_ChangeShiftError As Integer = -34
        Public Const REQUEST_ERROR_VacationsOrPermissionsError As Integer = -35
        Public Const REQUEST_ERROR_ExistsLockedDaysInPeriod As Integer = -36
        Public Const REQUEST_ERROR_ForbiddenPunchError As Integer = -37
        Public Const REQUEST_ERROR_JustifyPunchError As Integer = -38
        Public Const REQUEST_ERROR_RequestMoveNotExist As Integer = -39
        Public Const REQUEST_ERROR_RequestMoveTooMany As Integer = -40
        Public Const REQUEST_ERROR_PlannedAbsencesError As Integer = -41
        Public Const REQUEST_ERROR_PlannedCausesError As Integer = -42
        Public Const REQUEST_ERROR_ExternalWorkResumePartError As Integer = -43
        Public Const REQUEST_ERROR_UserFieldRequired As Integer = -44
        Public Const REQUEST_ERROR_PunchDateTimeRequired As Integer = -45
        Public Const REQUEST_ERROR_CauseRequired As Integer = -46
        Public Const REQUEST_ERROR_DateRequired As Integer = -47
        Public Const REQUEST_ERROR_HoursRequired As Integer = -48
        Public Const REQUEST_ERROR_ShiftRequired As Integer = -49
        Public Const REQUEST_ERROR_RequestRepited As Integer = -50
        Public Const REQUEST_ERROR_PunchExist As Integer = -51
        Public Const REQUEST_ERROR_StartShiftRequired As Integer = -52
        Public Const REQUEST_ERROR_PlannedCausesOverlapped As Integer = -53
        Public Const REQUEST_ERROR_PlannedAbsencesOverlapped As Integer = -54
        Public Const REQUEST_ERROR_TaskRequiered As Integer = -55
        Public Const REQUEST_ERROR_TIME_BETWEEN_PUNCHES As Integer = -56
        Public Const REQUEST_ERROR_TIME_BETWEEN_PUNCHES_OVER As Integer = -57
        Public Const TASK_ALERT_SAVE_ERROR As Integer = -58

        Public Const LOGIN_RESULT_LOW_STRENGHT_ERROR As Integer = -59
        Public Const LOGIN_RESULT_MEDIUM_STRENGHT_ERROR As Integer = -60
        Public Const LOGIN_RESULT_HIGH_STRENGHT_ERROR As Integer = -61
        Public Const LOGIN_PASSWORD_EXPIRED As Integer = -62
        Public Const LOGIN_NEED_TEMPORANY_KEY As Integer = -63
        Public Const LOGIN_TEMPORANY_KEY_EXPIRED As Integer = -64
        Public Const LOGIN_INVALID_VALIDATION_CODE As Integer = -65
        Public Const LOGIN_BLOCKED_ACCESS_APP As Integer = -66
        Public Const LOGIN_TEMPORANY_BLOQUED As Integer = -67
        Public Const LOGIN_GENERAL_BLOCK_ACCESS As Integer = -68
        Public Const LOGIN_INVALID_CLIENT_LOCATION As Integer = -69
        Public Const LOGIN_INVALID_VERSION_APP As Integer = -70
        Public Const LOGIN_INVALID_APP As Integer = -71

        Public Const REQUEST_ERROR_CostCenterRequiered As Integer = -72
        Public Const GENERAL_ERROR_NoPermissions As Integer = -73
        Public Const GENERAL_ERROR_InvalidSecurityToken As Integer = -74

        Public Const REQUEST_ERROR_PlannedHolidaysError As Integer = -75
        Public Const REQUEST_ERROR_PlannedHolidaysOverlapped As Integer = -76
        Public Const REQUEST_ERROR_AnotherHolidayExistInDate As Integer = -77
        Public Const REQUEST_ERROR_AnotherAbsenceExistInDate As Integer = -78
        Public Const REQUEST_ERROR_InHolidayPlanification As Integer = -79
        Public Const REQUEST_ERROR_VacationsOrPermissionsOverlapped As Integer = -80
        Public Const REQUEST_ERROR_CustomError As Integer = -81
        Public Const REQUEST_WARNING_NeedConfirmation As Integer = -82
        Public Const LOGIN_INVALID_APP_ADFS As Integer = -83

        Public Const NO_DOCUMENT_TEMPLATE_SELECTED As Integer = -84
        Public Const NO_DOCUMENT_ATTACHED As Integer = -85

        Public Const USER_OR_EMAIL_NOTFOUND As Integer = -86
        Public Const RECOVERKEY_NOTFOUND As Integer = -87
        Public Const ERROR_CREATING_NOTIFICATION As Integer = -88

        Public Const REQUEST_PENDING_VALIDATION As Integer = -89
        Public Const REQUEST_DENIED_DUE_REQUESTVALIDATION As Integer = -90
        Public Const REQUEST_DENIED_DUE_SERVERDATA As Integer = -91
        Public Const GENERAL_ERROR_MaxCurrentSessionsExceeded As Integer = -92
        Public Const REQUEST_ERROR_ExchangingShifts As Integer = -93
        Public Const PUNCH_NFC_TAG_NOT_FOUND As Integer = -94

        Public Const NO_DOCUMENT_UPLOADED_TO_SIGN As Integer = -95
        Public Const INVALID_MOBILE_NUMBER As Integer = -96
        Public Const EMPTY_FILE As Integer = -97
        Public Const NUMBER_SIGNED_DOC_EXCEEDED As Integer = -98
        Public Const CHANGE_TELECOMMUTING_ERROR As Integer = -99
        Public Const CHANGE_TELECOMMUTING_CHECK_MAX_PERCENTAGE As Integer = -100

        Public Const DAILYRECORD_INCORRECT_DATE_FUTURE = -101
        Public Const DAILYRECORD_INCORRECT_DATE_FROZEN = -102
        Public Const DAILYRECORD_INCORRECT_DATE_ALREADY_EXISTS = -103
        Public Const DAILYRECORD_WRONG_PUNCHES_SEQUENCE = -104
        Public Const DAILYRECORD_ODD_NUMBER_OF_PUNCHES = -105
        Public Const DAILYRECORD_ERROR_SAVING_REQUEST = -106

        Public Const DAILYRECORD_ERROR_LOADING = -107
        Public Const DAILYRECORD_ERROR_LOADING_CALENDAR = -108
        Public Const DAILYRECORD_ERROR_APPROVING_WHEN_MADE_BY_SUPERVISOR = -109
        Public Const DAILYRECORD_ERROR_DOES_NOT_EXISTS = -110
        Public Const DAILYRECORD_ERROR_NO_PUNCHES = -111
        Public Const DAILYRECORD_PUNCHES_OVERLAPED = -112
        Public Const DAILYRECORD_HAS_REPEATED_PUNCHES = -113
        Public Const ERROR_UPLOADING_DOCUMENT_TO_SIGN = -114

        Public Const REQUEST_ERROR_OutOfContract = -115
    End Class

End Namespace