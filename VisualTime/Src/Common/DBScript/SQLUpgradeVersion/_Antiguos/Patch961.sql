-- No borréis esta línea
CREATE PROCEDURE SetDefaultWebLinks
AS
BEGIN
	TRUNCATE TABLE [dbo].[WebLinks]

	INSERT INTO [dbo].[WebLinks]
			   ([Title]
			   ,[Description]
			   ,[URL]
			   ,[LinkCaption]
			   ,[Order]
			   ,[ShowOnLiveDashboard]
			   ,[ShowOnPortalMenu]
			   ,[ShowOnPortalDashboard])
		 VALUES
			   (N'Visualtime Academy'
			   ,N'Aula virtual para que siempre estés al día. Conocerás todas las novedades que afectan a la gestión del tiempo de los usuarios, normativas, funcionalidades de Visualtime, y mucho más.'
			   ,N'https://www.cegid.com/ib/es/lp/hcm-talent-time-cegid-visualtime-calendario-academy/'
			   ,N'¡Apúntate!'
			   , 1
			   , 1
			   , 0
			   , 0)

	INSERT INTO [dbo].[WebLinks]
			   ([Title]
			   ,[Description]
			   ,[URL]
			   ,[LinkCaption]
			   ,[Order]
			   ,[ShowOnLiveDashboard]
			   ,[ShowOnPortalMenu]
			   ,[ShowOnPortalDashboard])
		 VALUES
			   (N'Soporte'
			   ,N'A través de nuestro servicio de soporte exclusivo para clientes de Cegid Visualtime, puedes solicitar ayuda o asesoramiento personalizado.'
			   ,N'https://www.cegid.com/ib/asistencia-al-cliente/software-gestion-tiempo-visualtime/'
			   ,N'Contacto'
			   , 2
			   , 1
			   , 0
			   , 0)

	DECLARE @Language NVARCHAR(10)

	SELECT @Language = sysroLanguages.LanguageKey FROM sysroPassports
	INNER JOIN sysroLanguages ON sysroLanguages.ID = sysroPassports.IDLanguage
	WHERE Name = 'System'
	
	IF @Language = 'CAT'
	BEGIN
		UPDATE [dbo].[WebLinks] SET 
			[Description] = N'Aula virtual perquè sempre estiguis al dia. Coneixeràs totes les novetats que afecten la gestió del temps dels usuaris, normatives, funcionalitats de Visualtime i molt més.',
			[LinkCaption] = N'Apunta-t’hi!'
		WHERE ID = 1

		UPDATE [dbo].[WebLinks] SET 
			[Title] = N'Suport',
			[Description] = N'A través del nostre servei de suport exclusiu per a clients de Cegid Visualtime, pots sol·licitar ajuda o assessorament personalitzat.',
			[LinkCaption] = N'Contacte'
		WHERE ID = 2
	END
	ELSE IF @Language = 'ENG'
	BEGIN
		UPDATE [dbo].[WebLinks] SET 
			[Description] = N'Virtual classroom to keep you up to date. You will learn about all the latest developments affecting user time management, regulations, Visualtime features, and much more.',
			[LinkCaption] = N'Sign up!'
		WHERE ID = 1

		UPDATE [dbo].[WebLinks] SET 
			[Title] = N'Support',
			[Description] = N'Through our exclusive support service for Cegid Visualtime customers, you can request help or personalized advice.',
			[LinkCaption] = N'Contact'
		WHERE ID = 2
	END
	ELSE IF @Language = 'GAL'
	BEGIN
		UPDATE [dbo].[WebLinks] SET 
			[Description] = N'Aula virtual para que sempre esteas ao día. Coñecerás todas as novidades que afectan a xestión do tempo dos usuarios, normativas, funcionalidades de Visualtime, e moito máis.',
			[LinkCaption] = N'Apúntate!'
		WHERE ID = 1

		UPDATE [dbo].[WebLinks] SET 
			[Title] = N'Soporte',
			[Description] = N'A través do noso servizo de asistencia exclusiva para clientes de Cegid Visualtime podes solicitar axuda ou asesoramento personalizado.',
			[LinkCaption] = N'Contacto'
		WHERE ID = 2
	END
	ELSE IF @Language = 'POR'
	BEGIN
		UPDATE [dbo].[WebLinks] SET 
			[Description] = N'Aula virtual para que esteja sempre atualizado. Ficará a par de todas as novidades que afetam a gestão do tempo dos utilizadores, normas, funcionalidades do Visualtime e muito mais.',
			[LinkCaption] = N'Inscreva-se!'
		WHERE ID = 1

		UPDATE [dbo].[WebLinks] SET 
			[Title] = N'Suporte',
			[Description] = N'Através do nosso serviço de assistência exclusivo para clientes do Cegid Visualtime, pode solicitar ajuda ou aconselhamento personalizado.',
			[URL] = N'https://www.cegid.com/ib/pt/assistencia-ao-cliente/software-gestao-tempo-visualtime/',
			[LinkCaption] = N'Contacto'
		WHERE ID = 2
	END
	ELSE IF @Language = 'ITA'
	BEGIN
		UPDATE [dbo].[WebLinks] SET 
			[Description] = N'Aula virtuale per essere sempre aggiornati. Conoscerai tutte le novità che riguardano la gestione del tempo degli utenti, le normative, le funzioni di Visualtime e altro ancora.',
			[LinkCaption] = N'Iscriviti!'
		WHERE ID = 1

		UPDATE [dbo].[WebLinks] SET 
			[Title] = N'Supporto',
			[Description] = N'Attraverso il nostro servizio di assistenza esclusiva per i clienti di Cegid Visualtime, puoi richiedere assistenza o consulenze personalizzate.',
			[LinkCaption] = N'Contatto'
		WHERE ID = 2
	END
	ELSE IF @Language = 'FRA'
	BEGIN
		UPDATE [dbo].[WebLinks] SET 
			[Description] = N'Classe virtuelle pour vous tenir au courant. Vous découvrirez toutes les nouveautés qui affectent la gestion du temps des utilisateurs, les réglementations, les fonctionnalités de Visualtime, et bien plus encore.',
			[LinkCaption] = N'Participez !'
		WHERE ID = 1

		UPDATE [dbo].[WebLinks] SET 
			[Title] = N'Assistance',
			[Description] = N'Grâce à notre service d´assistance exclusif pour les clients de Cegid Visualtime, vous pouvez demander des conseils personnalisés ou de l´aide.',
			[LinkCaption] = N'Contact'
		WHERE ID = 2
	END
	ELSE IF @Language = 'EKR'
	BEGIN
		UPDATE [dbo].[WebLinks] SET 
			[Description] = N'Ikasgela birtuala, beti eguneratuta egon zaitezen. Erabiltzaileen denboraren kudeaketari, araudiari, Visualtime funtzionalitateei eta askoz gehiagori eragiten dieten albiste guztiak ezagutuko dituzu.',
			[LinkCaption] = N'Izena eman!'
		WHERE ID = 1

		UPDATE [dbo].[WebLinks] SET 
			[Title] = N'Ertaina',
			[Description] = N'Cegid Visualtime bezeroentzako gure laguntza-zerbitzu esklusiboaren bidez, laguntza edo aholkularitza pertsonalizatua eska dezakezu.',
			[LinkCaption] = N'Kontaktua'
		WHERE ID = 2
	END
	ELSE IF @Language = 'SLK'
	BEGIN
		UPDATE [dbo].[WebLinks] SET 
			[Description] = N'Virtuálna učebňa, aby ste mali vždy aktuálne informácie. Dozviete sa o všetkých novinkách, ktoré ovplyvňujú časový manažment používateľov, predpisy, funkcie Visualtime a mnoho ďalšieho.',
			[LinkCaption] = N'zaregistrujte sa!'
		WHERE ID = 1

		UPDATE [dbo].[WebLinks] SET 
			[Title] = N'Podpora',
			[Description] = N'Prostredníctvom našej exkluzívnej služby podpory pre zákazníkov spoločnosti Cegid Visualtime môžete požiadať o individuálnu pomoc alebo radu.',
			[LinkCaption] = N'Kontakt'
		WHERE ID = 2
	END
END
GO

EXEC SetDefaultWebLinks
GO

DROP PROCEDURE SetDefaultWebLinks
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='961' WHERE ID='DBVersion'
GO
