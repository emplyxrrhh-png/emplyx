
update dbo.[sysroLiveAdvancedParameters] set Value='Ubicacion' where parametername='vst_locationField'
  DECLARE @count INT
  DECLARE @position INT
  SET @count =(select count(*) from Visit_Fields where name='Ubicaci&#243;n')

  IF @count=1 
  BEGIN
	UPDATE Visit_Fields set name='Ubicacion' where name='Ubicaci&#243;n'
  END
  ELSE
  BEGIN
	SET @position = (select isnull(max(position),0) from Visit_Fields where deleted=0)
	insert into Visit_Fields(IDField,Name,[Required],position,deleted,visible,[values]) values (NEWID(),'Ubicacion',0,@position+1,0,0,'')
  END
GO

UPDATE dbo.sysroParameters SET Data='397' WHERE ID='DBVersion'
GO
