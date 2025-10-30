-- No borréis esta línea
INSERT INTO sysroLiveAdvancedParameters (ParameterName, Value) VALUES ('Documents.A3PayrollTemplate', '{"CalibrationX":0,"CalibrationY":0,"Height":9,"RecognitionType":0,"ReferenceOffsetX":-40,"ReferenceOffsetY":-12,"ReferenceWord":"D.N.I.","Width":70}')
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='963' WHERE ID='DBVersion'
GO
