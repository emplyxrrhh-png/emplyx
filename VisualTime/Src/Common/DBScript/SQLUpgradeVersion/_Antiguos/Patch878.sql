-- No borréis esta línea
ALTER TABLE [dbo].[Documents] ADD [SignDate] [smalldatetime] NULL
GO

UPDATE [dbo].[Documents] SET SignDate = ISNULL(ReceivedDate, DeliveryDate) WHERE SignStatus = 3
GO

CREATE TABLE [dbo].[sysroTrackedEvents](
[Event] [NVARCHAR](MAX) NOT NULL,
[IDObject] [int] NOT NULL,
[Date] [smalldatetime] NOT NULL,
[IDPassport] [int] NULL
)
GO

INSERT INTO sysroTrackedEvents 
(Event, IDObject, Date)
SELECT 'DocumentSigned',
       ID,
	   ISNULL(ReceivedDate, DeliveryDate)
FROM Documents
WHERE SignStatus = 3

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='878' WHERE ID='DBVersion'

GO
