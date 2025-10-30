INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx9',1,110,'TA',1,'Fast','X','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO

INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx9',1,111,'TA',1,'Interactive','X','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO

INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode])
VALUES('mx9',1,112,'')
GO

INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode])
VALUES('mxV',1,113,'')
GO


-- Informe Identificadores biométricos muestra primeras dos huellas registradas de los empleados para terminales ZK
  ALTER FUNCTION [dbo].[GetEmployeeBiometricsIDLive]
   (				
   	
   )
   RETURNS @ValueTable table(idPassport integer, RXA100_0 datetime,RXA100_1 datetime, RXFFNG_0 datetime, RXFFNG_1 datetime, RXFFAC_0 datetime)
   AS
   BEGIN
 	declare @idPassport integer
 	declare @idPassportAnt integer=0
 	declare @Version as nvarchar(max)
 	declare @BiometricID as integer 
 	declare @LenBiometricData integer 
 	declare @TimeStamp as datetime
	declare @fnumber integer
 	declare @RXA100_0 datetime = null
 	declare @RXA100_1 datetime = null
 	declare @RXFFNG_0 datetime = null
 	declare @RXFFNG_1 datetime = null
	declare @RXFFNG_2 datetime = null
	declare @RXFFNG_3 datetime = null
	declare @RXFFNG_4 datetime = null
	declare @RXFFNG_5 datetime = null
	declare @RXFFNG_6 datetime = null
	declare @RXFFNG_7 datetime = null
	declare @RXFFNG_8 datetime = null
	declare @RXFFNG_9 datetime = null
 	declare @RXFFAC_0 datetime = null
 	
 	declare TableCursor cursor fast_forward for
		select Numero, IDPassport, Version, BiometricID, TimeStamp, LenBiometricData from 
		(
		select ROW_NUMBER() OVER (PARTITION BY IDPassport ORDER BY IdPassport, BiometricID ASC) Numero,  PAM.IDPassport, PAM.Version, PAM.BiometricID, TimeStamp, (len(dbo.f_BinaryToBase64(PAM.biometricdata))) as LenBiometricData from 
		sysroPassports_AuthenticationMethods PAM				
		where PAM.Method =4 and PAM.Enabled=1 And PAM.Version IN ('RXA100','RXFFNG','RXFFAC') And len(dbo.f_BinaryToBase64(PAM.biometricdata))>4
		) aux 
		where Numero <=2
 	open TableCursor
   	
   	fetch next from TableCursor into @fnumber, @idPassport, @Version, @BiometricID,@TimeStamp, @LenBiometricData		
 	while (@@FETCH_STATUS <> -1)
 	begin
 		if @idPassportAnt = 0 set @idPassportAnt=@idPassport
 				
 		if @idPassportAnt<>@idPassport
 			begin
 				insert into @ValueTable values (@idPassportAnt, @RXA100_0, @RXA100_1, @RXFFNG_0, @RXFFNG_1, @RXFFAC_0)
 				set @RXA100_0= null
 				set @RXA100_1= null
 				set @RXFFNG_0= null
 				set @RXFFNG_1= null
 				set @RXFFAC_0= null
 				set @idPassportAnt = @idPassport
 			end
 		if @Version='RXA100' and @BiometricID=0 set @RXA100_0=@TimeStamp
 		if @Version='RXA100' and @BiometricID=1 set @RXA100_1=@TimeStamp
 		if @Version='RXFFNG' and @fnumber=1 set @RXFFNG_0=@TimeStamp
 		if @Version='RXFFNG' and @fnumber=2 set @RXFFNG_1=@TimeStamp
 		if @Version='RXFFAC' and @BiometricID=0 AND @LenBiometricData>4 set @RXFFAC_0=@TimeStamp
 							
 		fetch next from TableCursor into @fnumber, @idPassport, @Version, @BiometricID, @TimeStamp, @LenBiometricData
 	end
 	if @idPassportAnt<>0  Insert into @ValueTable values (@idPassportAnt, @RXA100_0, @RXA100_1, @RXFFNG_0, @RXFFNG_1, @RXFFAC_0)
 	close TableCursor
 	deallocate TableCursor
   	RETURN
   END
GO


-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='474' WHERE ID='DBVersion'
GO
