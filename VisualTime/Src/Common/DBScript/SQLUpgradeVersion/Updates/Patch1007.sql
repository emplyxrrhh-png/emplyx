  insert into sysroGUI values ('Portal\Company\Collectives','Gui.Collectives','/Collectives',	'Groups.png',NULL,'SecurityV3',NULL,NULL,'1099',NULL,'U:Employees=Read',NULL);
  GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1007' WHERE ID='DBVersion'
GO
