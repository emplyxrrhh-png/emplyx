-- Seed inicial para dominios de seguridad y autorizacion
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM [SeedVersions] WHERE [Key] = 'CoreAuthorizationSeed' AND [Version] = 1)
BEGIN
    MERGE INTO [Clearances] AS target
    USING (VALUES
        ('0ad1bc87-5e61-4258-8f51-2305b7c5401d', 'Publico', 1, 'Acceso general sin restricciones.'),
        ('25dfbb4d-8f45-4a54-958f-07daa4619052', 'Interno', 2, 'Acceso para personal interno y socios de confianza.'),
        ('5d7a9d25-c4b2-4b93-93c6-a39c47da1f40', 'Restringido', 3, 'Acceso reservado a datos confidenciales y criticos.')
    ) AS source (Id, Nombre, Nivel, Descripcion)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Nombre, Nivel, Descripcion, IsActive, CreatedAtUtc, UpdatedAtUtc)
        VALUES (TRY_CAST(source.Id AS uniqueidentifier), source.Nombre, source.Nivel, source.Descripcion, 1, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z');

    MERGE INTO [Modulos] AS target
    USING (VALUES
        ('6f09c219-4c23-4c89-bf13-4f08bc3d24a0', 'SEGURIDAD', 'Seguridad', 'Gestion de usuarios, roles y permisos.', 1),
        ('ab42ec2a-6e5b-4d81-9edc-0c5acb4fd4f5', 'OPERACIONES', 'Operaciones', 'Operativa diaria y contextos de trabajo.', 1),
        ('3a6f9950-9e50-4a22-b417-7fb7a57de5fe', 'LICENCIAS', 'Licenciamiento', 'Administracion de licencias y modulos contratados.', 0)
    ) AS source (Id, Codigo, Nombre, Descripcion, EsCritico)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Codigo, Nombre, Descripcion, EsCritico, CreatedAtUtc, UpdatedAtUtc)
        VALUES (TRY_CAST(source.Id AS uniqueidentifier), source.Codigo, source.Nombre, source.Descripcion, source.EsCritico, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z');

    MERGE INTO [Licencias] AS target
    USING (VALUES
        ('b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29', 'CORE-PRO', 'Licencia Profesional Core', '2025-01-01T00:00:00Z', '2030-12-31T23:59:59Z', 500, 0, 0)
    ) AS source (Id, Codigo, Nombre, InicioVigenciaUtc, FinVigenciaUtc, LimiteUsuarios, EsTrial, IsRevoked)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Codigo, Nombre, InicioVigenciaUtc, FinVigenciaUtc, LimiteUsuarios, EsTrial, IsRevoked, CreatedAtUtc, UpdatedAtUtc)
        VALUES (TRY_CAST(source.Id AS uniqueidentifier), source.Codigo, source.Nombre, TRY_CAST(source.InicioVigenciaUtc AS datetime2), TRY_CAST(source.FinVigenciaUtc AS datetime2), source.LimiteUsuarios, source.EsTrial, source.IsRevoked, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z');

    MERGE INTO [LicenciaModulos] AS target
    USING (VALUES
        ('b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29', '6f09c219-4c23-4c89-bf13-4f08bc3d24a0'),
        ('b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29', 'ab42ec2a-6e5b-4d81-9edc-0c5acb4fd4f5'),
        ('b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29', '3a6f9950-9e50-4a22-b417-7fb7a57de5fe')
    ) AS source (LicenciaId, ModuloId)
    ON target.LicenciaId = TRY_CAST(source.LicenciaId AS uniqueidentifier)
       AND target.ModuloId = TRY_CAST(source.ModuloId AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (LicenciaId, ModuloId, LinkedAtUtc)
        VALUES (TRY_CAST(source.LicenciaId AS uniqueidentifier), TRY_CAST(source.ModuloId AS uniqueidentifier), '2025-01-01T00:00:00Z');

    MERGE INTO [Delegaciones] AS target
    USING (VALUES
        ('8f9033a5-436b-4e1f-9d05-21c27c6c3bf2', 'Delegacion Central', 'CENTRAL', NULL, 'Sede central corporativa.'),
        ('d84145b9-4a7f-4ea9-8b0c-d4b0f8f6c424', 'Delegacion Regional Norte', 'NORTE', '8f9033a5-436b-4e1f-9d05-21c27c6c3bf2', 'Operaciones regionales zona norte.'),
        ('e4d8d94a-9084-46c3-99c1-1b4ef76e6a19', 'Delegacion Regional Sur', 'SUR', '8f9033a5-436b-4e1f-9d05-21c27c6c3bf2', 'Operaciones regionales zona sur.')
    ) AS source (Id, Nombre, Codigo, ParentId, Descripcion)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Nombre, Codigo, Descripcion, ParentId, IsActive, CreatedAtUtc, UpdatedAtUtc)
        VALUES (TRY_CAST(source.Id AS uniqueidentifier), source.Nombre, source.Codigo, source.Descripcion, TRY_CAST(source.ParentId AS uniqueidentifier), 1, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z');

    MERGE INTO [Contextos] AS target
    USING (VALUES
        ('caa6d616-f237-4d2b-a3f1-bf54cd508c35', 'DEFAULT', 'Contexto Principal', 'Contexto operativo principal del sistema.', '8f9033a5-436b-4e1f-9d05-21c27c6c3bf2', 'b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29', '25dfbb4d-8f45-4a54-958f-07daa4619052')
    ) AS source (Id, Clave, Nombre, Descripcion, DelegacionId, LicenciaId, ClearanceId)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Clave, Nombre, Descripcion, DelegacionId, LicenciaId, ClearanceId, IsActive, CreatedAtUtc, UpdatedAtUtc)
        VALUES (TRY_CAST(source.Id AS uniqueidentifier), source.Clave, source.Nombre, source.Descripcion, TRY_CAST(source.DelegacionId AS uniqueidentifier), TRY_CAST(source.LicenciaId AS uniqueidentifier), TRY_CAST(source.ClearanceId AS uniqueidentifier), 1, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z');

    MERGE INTO [ContextoModulos] AS target
    USING (VALUES
        ('caa6d616-f237-4d2b-a3f1-bf54cd508c35', '6f09c219-4c23-4c89-bf13-4f08bc3d24a0'),
        ('caa6d616-f237-4d2b-a3f1-bf54cd508c35', 'ab42ec2a-6e5b-4d81-9edc-0c5acb4fd4f5'),
        ('caa6d616-f237-4d2b-a3f1-bf54cd508c35', '3a6f9950-9e50-4a22-b417-7fb7a57de5fe')
    ) AS source (ContextoId, ModuloId)
    ON target.ContextoId = TRY_CAST(source.ContextoId AS uniqueidentifier)
       AND target.ModuloId = TRY_CAST(source.ModuloId AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (ContextoId, ModuloId, HabilitadoDesdeUtc)
        VALUES (TRY_CAST(source.ContextoId AS uniqueidentifier), TRY_CAST(source.ModuloId AS uniqueidentifier), '2025-01-01T00:00:00Z');

    MERGE INTO [Roles] AS target
    USING (VALUES
        ('3fbdede0-0773-4cb6-9881-9350d0f4955e', 'Administrador Global', 'Acceso total a la plataforma.', 1, '5d7a9d25-c4b2-4b93-93c6-a39c47da1f40'),
        ('a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4', 'Gestor de Delegacion', 'Gestiona recursos dentro de su delegacion.', 0, '25dfbb4d-8f45-4a54-958f-07daa4619052'),
        ('a9d6d2dc-136f-42e4-9b77-75568128ed5f', 'Operador', 'Operativa diaria de la plataforma.', 0, '0ad1bc87-5e61-4258-8f51-2305b7c5401d')
    ) AS source (Id, Nombre, Descripcion, IsSystem, ClearanceId)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Nombre, Descripcion, IsSystem, ClearanceId, CreatedAtUtc, UpdatedAtUtc)
        VALUES (TRY_CAST(source.Id AS uniqueidentifier), source.Nombre, source.Descripcion, source.IsSystem, TRY_CAST(source.ClearanceId AS uniqueidentifier), '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z');

    MERGE INTO [Permisos] AS target
    USING (VALUES
        ('f1d2c6fe-39f8-4832-9e2d-468f4bea7c31', 'SEGURIDAD.USUARIOS.GESTIONAR', 'Gestionar usuarios', '6f09c219-4c23-4c89-bf13-4f08bc3d24a0', 'Usuarios', 1),
        ('8fe47fa7-0fc5-4dae-b426-2e1dd7d5e47c', 'SEGURIDAD.USUARIOS.VER', 'Ver usuarios', '6f09c219-4c23-4c89-bf13-4f08bc3d24a0', 'Usuarios', 0),
        ('76d8cc5f-4224-4ef3-a594-984cd5c50a62', 'SEGURIDAD.ROLES.GESTIONAR', 'Gestionar roles', '6f09c219-4c23-4c89-bf13-4f08bc3d24a0', 'Roles', 1),
        ('d854df08-26d2-4af7-98d3-49816d23c01f', 'OPERACIONES.CONTEXTOS.GESTIONAR', 'Gestionar contextos', 'ab42ec2a-6e5b-4d81-9edc-0c5acb4fd4f5', 'Contextos', 1),
        ('dc6c8079-0af4-4a35-86d6-c9f6a4541bc9', 'OPERACIONES.CONTEXTOS.VER', 'Ver contextos', 'ab42ec2a-6e5b-4d81-9edc-0c5acb4fd4f5', 'Contextos', 0),
        ('c5e0a3ea-4fe8-4f8c-a2a0-3cfc4fb28eb1', 'LICENCIAS.MAESTRO.GESTIONAR', 'Gestionar licencias', '3a6f9950-9e50-4a22-b417-7fb7a57de5fe', 'Licencias', 1)
    ) AS source (Id, Codigo, Nombre, ModuloId, Categoria, EsCritico)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Codigo, Nombre, ModuloId, Categoria, EsCritico, CreatedAtUtc, UpdatedAtUtc)
        VALUES (TRY_CAST(source.Id AS uniqueidentifier), source.Codigo, source.Nombre, TRY_CAST(source.ModuloId AS uniqueidentifier), source.Categoria, source.EsCritico, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z');

    INSERT INTO [RolPermisos] (RolId, PermisoId, GrantedAtUtc)
    SELECT TRY_CAST(rp.RolId AS uniqueidentifier), TRY_CAST(rp.PermisoId AS uniqueidentifier), '2025-01-01T00:00:00Z'
    FROM (VALUES
        ('3fbdede0-0773-4cb6-9881-9350d0f4955e', 'f1d2c6fe-39f8-4832-9e2d-468f4bea7c31'),
        ('3fbdede0-0773-4cb6-9881-9350d0f4955e', '8fe47fa7-0fc5-4dae-b426-2e1dd7d5e47c'),
        ('3fbdede0-0773-4cb6-9881-9350d0f4955e', '76d8cc5f-4224-4ef3-a594-984cd5c50a62'),
        ('3fbdede0-0773-4cb6-9881-9350d0f4955e', 'd854df08-26d2-4af7-98d3-49816d23c01f'),
        ('3fbdede0-0773-4cb6-9881-9350d0f4955e', 'dc6c8079-0af4-4a35-86d6-c9f6a4541bc9'),
        ('3fbdede0-0773-4cb6-9881-9350d0f4955e', 'c5e0a3ea-4fe8-4f8c-a2a0-3cfc4fb28eb1'),
        ('a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4', '8fe47fa7-0fc5-4dae-b426-2e1dd7d5e47c'),
        ('a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4', 'd854df08-26d2-4af7-98d3-49816d23c01f'),
        ('a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4', 'dc6c8079-0af4-4a35-86d6-c9f6a4541bc9'),
        ('a9d6d2dc-136f-42e4-9b77-75568128ed5f', '8fe47fa7-0fc5-4dae-b426-2e1dd7d5e47c'),
        ('a9d6d2dc-136f-42e4-9b77-75568128ed5f', 'dc6c8079-0af4-4a35-86d6-c9f6a4541bc9')
    ) AS rp (RolId, PermisoId)
    WHERE NOT EXISTS (
        SELECT 1 FROM [RolPermisos] existing
        WHERE existing.RolId = TRY_CAST(rp.RolId AS uniqueidentifier)
          AND existing.PermisoId = TRY_CAST(rp.PermisoId AS uniqueidentifier));

    INSERT INTO [DelegacionRoles] (DelegacionId, RolId, LinkedAtUtc)
    SELECT TRY_CAST(dr.DelegacionId AS uniqueidentifier), TRY_CAST(dr.RolId AS uniqueidentifier), '2025-01-01T00:00:00Z'
    FROM (VALUES
        ('8f9033a5-436b-4e1f-9d05-21c27c6c3bf2', '3fbdede0-0773-4cb6-9881-9350d0f4955e'),
        ('8f9033a5-436b-4e1f-9d05-21c27c6c3bf2', 'a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4'),
        ('d84145b9-4a7f-4ea9-8b0c-d4b0f8f6c424', 'a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4'),
        ('e4d8d94a-9084-46c3-99c1-1b4ef76e6a19', 'a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4'),
        ('8f9033a5-436b-4e1f-9d05-21c27c6c3bf2', 'a9d6d2dc-136f-42e4-9b77-75568128ed5f'),
        ('d84145b9-4a7f-4ea9-8b0c-d4b0f8f6c424', 'a9d6d2dc-136f-42e4-9b77-75568128ed5f'),
        ('e4d8d94a-9084-46c3-99c1-1b4ef76e6a19', 'a9d6d2dc-136f-42e4-9b77-75568128ed5f')
    ) AS dr (DelegacionId, RolId)
    WHERE NOT EXISTS (
        SELECT 1 FROM [DelegacionRoles] existing
        WHERE existing.DelegacionId = TRY_CAST(dr.DelegacionId AS uniqueidentifier)
          AND existing.RolId = TRY_CAST(dr.RolId AS uniqueidentifier));

    INSERT INTO [SeedVersions] (Id, [Key], Version, AppliedAtUtc)
    VALUES ('d6e36d75-6f90-4d90-b1b4-0d423b99d8f1', 'CoreAuthorizationSeed', 1, SYSUTCDATETIME());
END

COMMIT;
