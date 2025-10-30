INSERT INTO [dbo].[notificationmessageparameters]
            ([idnotificationtype],
             [scenario],
             [scope],
             [parameter],
             [parameterlanguagekey],
             [notificationlanguagekey])
VALUES      (39,
             3,
             'Subject',
             0,
             '',
             'AdviceNewPasswordPin')

GO

INSERT INTO [dbo].[notificationmessageparameters]
            ([idnotificationtype],
             [scenario],
             [scope],
             [parameter],
             [parameterlanguagekey],
             [notificationlanguagekey])
VALUES      (39,
             3,
             'Body',
             1,
             'PassportName',
             'AdviceNewPasswordPin')

GO

INSERT INTO [dbo].[notificationmessageparameters]
            ([idnotificationtype],
             [scenario],
             [scope],
             [parameter],
             [parameterlanguagekey],
             [notificationlanguagekey])
VALUES      (39,
             3,
             'Body',
             2,
             'Pin',
             'AdviceNewPasswordPin')

GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='827' WHERE ID='DBVersion'
GO
