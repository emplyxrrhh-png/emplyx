-- No borréis esta línea
CREATE TABLE sysroSupremaEventCodes (
    code INT NOT NULL,
    name VARCHAR(100) NOT NULL,
    PRIMARY KEY (code)
);

INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4097, 'VERIFY_SUCCESS_ID_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4098, 'VERIFY_SUCCESS_ID_FINGERPRINT')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4099, 'VERIFY_SUCCESS_ID_FINGERPRINT_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4100, 'VERIFY_SUCCESS_ID_FACE')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4101, 'VERIFY_SUCCESS_ID_FACE_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4102, 'VERIFY_SUCCESS_CARD')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4103, 'VERIFY_SUCCESS_CARD_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4104, 'VERIFY_SUCCESS_CARD_FINGERPRINT')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4105, 'VERIFY_SUCCESS_CARD_FINGERPRINT_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4106, 'VERIFY_SUCCESS_CARD_FACE')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4107, 'VERIFY_SUCCESS_CARD_FACE_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4112, 'VERIFY_SUCCESS_CARD_FACE_FINGER')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4113, 'VERIFY_SUCCESS_CARD_FINGER_FACE')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4114, 'VERIFY_SUCCESS_ID_FACE_FINGER')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4115, 'VERIFY_SUCCESS_ID_FINGER_FACE')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4118, 'VERIFY_SUCCESS_MOBLIE_CARD')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4119, 'VERIFY_SUCCESS_MOBILE_CARD_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4120, 'VERIFY_SUCCESS_MOBILE_CARD_FINGER')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4121, 'VERIFY_SUCCESS_MOBILE_CARD_FINGER_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4122, 'VERIFY_SUCCESS_MOBILE_CARD_FACE')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4123, 'VERIFY_SUCCESS_MOBiLE_CARD_FACE_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4128, 'VERIFY_SUCCESS_MOBILE_CARD_FACE_FINGER')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4129, 'VERIFY_SUCCESS_MOBILE_CARD_FINGER_FACE')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4133, 'VERIFY_SUCCESS_QR')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4134, 'VERIFY_SUCCESS_QR_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4135, 'VERIFY_SUCCESS_QR_FINGERPRINT')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4136, 'VERIFY_SUCCESS_QR_FINGERPRINT_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4137, 'VERIFY_SUCCESS_QR_FACE')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4138, 'VERIFY_SUCCESS_QR_FACE_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4139, 'VERIFY_SUCCESS_QR_FACE_FINGERPRINT')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4140, 'VERIFY_SUCCESS_QR_FINGERPRINT_FACE')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4865, 'IDENTIFY_SUCCESS_FINGERPRINT')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4866, 'IDENTIFY_SUCCESS_FINGERPRINT_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4867, 'IDENTIFY_SUCCESS_FACE')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4868, 'IDENTIFY_SUCCESS_FACE_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4869, 'IDENTIFY_SUCCESS_FACE_FINGER')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4870, 'IDENTIFY_SUCCESS_FACE_FINGER_PIN')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4871, 'IDENTIFY_SUCCESS_FINGER_FACE')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4872, 'IDENTIFY_SUCCESS_FINGER_FACE_PIN')


DELETE sysroReaderTemplates WHERE Type = 'Suprema'
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'Suprema', 1, N'TA', N'1', N'Blind', N'E', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', 'Remote', 1, NULL, NULL)
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'Suprema', 1, N'TA', N'1', N'Blind', N'S', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', 'Remote', 1, NULL, NULL)
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'Suprema', 1, N'TA', N'1', N'Blind', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', 'Remote', 1, NULL, NULL)
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='927' WHERE ID='DBVersion'
GO
