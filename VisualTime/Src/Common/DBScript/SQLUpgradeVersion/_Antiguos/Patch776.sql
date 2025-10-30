--Retain all punches from sysroPunchesTransactions less than 31 days 
SELECT *
INTO #TMPPuchesTransactions
FROM sysroPunchesTransactions
WHERE DATEDIFF(day, TimeStamp, CAST(GETDATE() AS DATE)) < 31 ORDER BY TimeStamp DESC

TRUNCATE TABLE sysroPunchesTransactions

INSERT INTO sysroPunchesTransactions
SELECT *
FROM #TMPPuchesTransactions

DROP TABLE #TMPPuchesTransactions

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='776' WHERE ID='DBVersion'
GO
