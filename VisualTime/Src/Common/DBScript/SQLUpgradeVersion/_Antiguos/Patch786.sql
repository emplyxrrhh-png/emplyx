-- No borréis esta línea
 ALTER PROCEDURE [dbo].[ObtainFeaturesFromFilter]      
      @requestTypes nvarchar(max)  
      AS       
     begin    
 	  declare @prequestTypes nvarchar(max) = @requestTypes    
 	  DECLARE @SQLString nvarchar(MAX);      
 			SET @SQLString = 'select Alias,r.EmployeeFeatureId, r.Type, r.IdType from sysroFeatures f inner join sysroRequestType r on f.AliasID = r.IdType and r.IdType IN (' + @requestTypes + ')'  
 	  exec sp_executesql @SQLString      
 	end  
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='786' WHERE ID='DBVersion'
GO
