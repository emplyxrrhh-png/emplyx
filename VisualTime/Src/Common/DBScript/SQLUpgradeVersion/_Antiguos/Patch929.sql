--Remember to add the file to the Updates folder 
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (4095, 'DEVICE NETWORK DISCONNECTED')
GO
INSERT INTO sysroSupremaEventCodes (code, name) VALUES (13056, 'LINK_CONNECTED')
GO
UPDATE Terminals SET SerialNumber = Location WHERE Type = 'Virtual'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='929' WHERE ID='DBVersion'
GO
