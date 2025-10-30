-- No borréis esta línea
ALTER FUNCTION [dbo].[GetFullGroupPathName] (@GroupId int)
  	RETURNS varchar(300) 
 AS
 BEGIN
  	declare @path varchar(200);
  	declare @index int;
  	declare @delimiter char(1);
  	declare @i int;
  	declare @resultado varchar(300);
  	declare @id varchar(100);
  	declare @nombre varchar(200);
  	declare @longitud varchar(100);
  	declare @contador int;
  	declare @anterior int;
  	declare @siguiente int;
  	declare @digitos int;
  	set @contador=0
  	set @resultado=''
  	set @index=1
  	set @id=''
  	set @i=1
  	set @delimiter='\'
  	-- Seleccionamos el Path del grupo que nos manda el usuario
  	select @path=Path from Groups where ID=str(@GroupId)
  	--RETURN 'El path es: '+@path
  	-- Miramos la longitud total del Path
  	select @longitud = len(@path)
  	--RETURN 'La longitud es: '+str(@longitud)
  	-- Controlamos el primer valor del grupo
  	while (@contador=0)
  	begin
 		--Recogemos el grupo padre
 		select @index = CHARINDEX(@delimiter,@path)	
 		IF (@index != 0)
  			select @id=left(@path,@index-1)
 		ELSE
 			select @id=@path
  		select @contador=@contador+1
  		select @nombre=Name from Groups where ID=@id
  		select @resultado=@nombre
  	end
 	set @index=1
  	-- Controlamos que hay alguna \ en el path
  	while (@index!=0)
  	begin
  		-- Buscamos la posicion de la \
  		select @index = CHARINDEX(@delimiter,@path,@i)
 		--RETURN 'Posición de la primera barra: '+str(@index)
  		-- Guardamos la localizacion de la primera \
  		select @anterior = @index
  		-- Si encuentra la \
  		if (@index!=0) 
  		begin
  			-- Colocamos el cursor en la siguiente posicion		
  			select @i=@index+1
  			-- Volvemos a buscar \ por si es la ultima
  			select @index = CHARINDEX(@delimiter,@path,@i)
  			--RETURN 'Posicion de la \: '+str(@index)
  			-- Si existe otra barra por delante
  			if (@index!=0)
  			begin
  				-- Restamos 1 porque entre las \ albergan minimo 2 espacios
  				select @digitos=@index-@anterior-1
  				--Comprobamos si el grupo tiene 1 digito
  				if (@digitos=1)
  				begin
  					select @id=right(left(@path,@i),1)
  					-- Buscamos el id en la base de datos
  					select @nombre=Name from Groups where ID=@id
  					--print 'Grupo: '+@nombre
  					-- Concatenamos el valor al resultado final
  					select @resultado=@resultado+' \ '+@nombre
  				end
  				-- Si el id del grupo esta formado por dos digitos
  				if (@digitos=2)
  				begin
  					-- Movemos el puntero para poder recortar el id del grupo
  					select @i=@i+1
  					select @id=right(left(@path,@i),2)
  					select @nombre=Name from Groups where ID=@id
  					-- Concatenamos el valor al resultado final
  					select @resultado=@resultado+' \ '+@nombre
  				end
  				-- Si el id del grupo esta formado por tres digitos
  				if (@digitos=3)
  				begin
  					select @i=@i+2
  					select @id=right(left(@path,@i),3)
  					select @nombre=Name from Groups where ID=@id
  					-- Concatenamos el valor al resultado final
  					select @resultado=@resultado+' \ '+@nombre
  				end
 				-- Si el id del grupo esta formado por cuatro digitos
 				if (@digitos=4)
  				begin
  					select @i=@i+3
  					select @id=right(left(@path,@i),4)
  					select @nombre=Name from Groups where ID=@id
  					-- Concatenamos el valor al resultado final
  					select @resultado=@resultado+' \ '+@nombre
  				end
  			end
  			-- Sino no encuentra \
  			else
  			begin
  				select @digitos=@longitud-@anterior
 				--RETURN 'Digitos: '+str(@digitos)
  				-- Si el id del grupo esta formado por un digito
  				if (@digitos=1)
  				begin
 					-- Movemos el puntero para poder recortar el id del grupo
 					select @i=@i+2
 					select @id=right(left(@path,@i),1)
 					select @nombre=Name from Groups where ID=@id
 					-- Concatenamos el valor al resultado final
 					select @resultado=@resultado+' \ '+@nombre
  				end
  				-- Si el id del grupo esta formado por dos digitos
  				if (@digitos=2)
  				begin
  					select @i=@i+3
  					select @id=right(left(@path,@i),2)
  					select @nombre=Name from Groups where ID=@id
  					-- Concatenamos el valor al resultado final
  					select @resultado=@resultado+' \ '+@nombre
 					--RETURN 'resultado: '+@resultado
  				end
  				-- Si el id del grupo esta formado por tres digitos
  				if (@digitos=3)
  				begin
 					select @i=@i+2
 					select @id=right(left(@path,@i),3)
 					select @nombre=Name from Groups where ID=@id
 					-- Concatenamos el valor al resultado final
 					select @resultado=@resultado+' \ '+@nombre
 					--return @resultado
  				--return 'El nombre es:'+@resultado
  				end
 				-- Si el id del grupo esta formado por cuatro digitos
 				if (@digitos=4)
  				begin
  					select @i=@i+3
  					select @id=right(left(@path,@i),4)
  					select @nombre=Name from Groups where ID=@id
  					-- Concatenamos el valor al resultado final
  					select @resultado=@resultado+' \ '+@nombre
  				end
  			end
  		end
  	end
	IF @resultado IS NULL
    BEGIN
        SET @resultado = '';
    END
  	return (@resultado)
 END
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='682' WHERE ID='DBVersion'
GO
