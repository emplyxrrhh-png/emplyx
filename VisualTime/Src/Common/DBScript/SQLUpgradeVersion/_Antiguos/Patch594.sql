ALTER TABLE [dbo].[EmployeeContracts] ADD TelecommutingMaxOptionalDays Integer DEFAULT 0;
GO
ALTER TABLE [dbo].[LabAgree] ADD TelecommutingMaxOptionalDays Integer DEFAULT 0;
GO
UPDATE EmployeeContracts SET TelecommutingMaxOptionalDays = TelecommutingMaxDays
GO
UPDATE LabAgree SET TelecommutingMaxOptionalDays = TelecommutingMaxDays
GO

ALTER VIEW [dbo].[sysrovwTelecommutingAgreement]
AS
SELECT * FROM (
SELECT
EC.IDEmployee,
EC.IDContract,
EC.BeginDate AS ContractStart,
EC.EndDate AS ContractEnd,
LA.ID AS LabAgreeId,
CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN (CASE WHEN LA.TelecommutingAgreementStart IS NULL THEN '' ELSE 'LabAgree' END) ELSE 'Contract' END AS TelecommutingAgreementSource,
ISNULL(CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.Telecommuting ELSE EC.Telecommuting END,0) AS Telecommuting,
CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingMandatoryDays ELSE EC.TelecommutingMandatoryDays END AS TelecommutingMandatoryDays,
CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.PresenceMandatoryDays ELSE EC.PresenceMandatoryDays END AS PresenceMandatoryDays,
CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.PeriodType ELSE EC.PeriodType END AS PeriodType,
CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingOptionalDays ELSE EC.TelecommutingOptionalDays END AS TelecommutingOptionalDays,
CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingMaxOptionalDays ELSE EC.TelecommutingMaxOptionalDays END AS TelecommutingMaxOptionalDays,  
CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingMaxDays ELSE EC.TelecommutingMaxDays END AS TelecommutingMaxDays,
CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingMaxPercentage ELSE EC.TelecommutingMaxPercentage END AS TelecommutingMaxPercentage,
CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingAgreementStart ELSE EC.TelecommutingAgreementStart END AS TelecommutingAgreementStart,
CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingAgreementEnd ELSE EC.TelecommutingAgreementEnd END AS TelecommutingAgreementEnd
FROM EmployeeContracts EC
LEFT JOIN LabAgree LA ON EC.IDLabAgree = LA.ID
) AUX
WHERE ContractEnd >= TelecommutingAgreementStart AND TelecommutingAgreementEnd >= ContractStart
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO
UPDATE sysroParameters SET Data='594' WHERE ID='DBVersion'
GO
