-- Seed inicial para dominios de seguridad y autorización
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM [SeedVersions] WHERE [Key] = 'CoreAuthorizationSeed' AND [Version] = 1)
BEGIN
    -- Clearances
    MERGE INTO [Clearances] AS target
    USING (VALUES
        ('0ad1bc87-5e61-4258-8f51-2305b7c5401d', 'Público', 1, 'Acceso general sin restricciones.'),
        ('25dfbb4d-8f45-4a54-958f-07daa4619052', 'Interno', 2, 'Acceso para personal interno y socios de confianza.'),
        ('5d7a9d25-c4b2-4b93-93c6-a39c47da1f40', 'Restringido', 3, 'Acceso reservado a datos confidenciales y críticos.')
    ) AS source (Id, Nombre, Nivel, Descripcion)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Nombre, Nivel, Descripcion, IsActive, CreatedAtUtc, UpdatedAtUtc)
        VALUES (TRY_CAST(source.Id AS uniqueidentifier), source.Nombre, source.Nivel, source.Descripcion, 1, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z');

    -- Licencia principal
    MERGE INTO [Licencias] AS target
    USING (VALUES
        ('b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29', 'CORE-PRO', 'Licencia Profesional Core')
    ) AS source (Id, Codigo, Nombre)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Codigo, Nombre, InicioVigenciaUtc, FinVigenciaUtc, LimiteUsuarios, EsTrial, IsRevoked, CreatedAtUtc, UpdatedAtUtc)
        VALUES (TRY_CAST(source.Id AS uniqueidentifier), source.Codigo, source.Nombre, '2025-01-01T00:00:00Z', '2030-12-31T23:59:59Z', 500, 0, 0, '2025-01-01T00:00:00Z', '2025-01-01T00:00:00Z');

    -- Delegaciones
    MERGE INTO [Delegaciones] AS target
    USING (VALUES
        ('8f9033a5-436b-4e1f-9d05-21c27c6c3bf2', 'Delegación Central', 'CENTRAL', NULL),
        ('d84145b9-4a7f-4ea9-8b0c-d4b0f8f6c424', 'Delegación Regional Norte', 'NORTE', '8f9033a5-436b-4e1f-9d05-21c27c6c3bf2'),
        ('e4d8d94a-9084-46c3-99c1-1b4ef76e6a19', 'Delegación Regional Sur', 'SUR', '8f9033a5-436b-4e1f-9d05-21c27c6c3bf2')
    ) AS source (Id, Nombre, Codigo, ParentId)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Nombre, Codigo, Descripcion, ParentId, IsActive, CreatedAtUtc, UpdatedAtUtc)
        VALUES (
            TRY_CAST(source.Id AS uniqueidentifier),
            source.Nombre,
            source.Codigo,
            CASE source.Codigo WHEN 'CENTRAL' THEN 'Sede central corporativa.' ELSE CONCAT('Operaciones regionales ', source.Codigo) END,
            TRY_CAST(source.ParentId AS uniqueidentifier),
            1,
            '2025-01-01T00:00:00Z',
            '2025-01-01T00:00:00Z');

    -- Contexto base
    MERGE INTO [Contextos] AS target
    USING (VALUES
        ('caa6d616-f237-4d2b-a3f1-bf54cd508c35', 'DEFAULT', 'Contexto Principal', 'Contexto operativo principal del sistema.', '8f9033a5-436b-4e1f-9d05-21c27c6c3bf2', 'b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29', '25dfbb4d-8f45-4a54-958f-07daa4619052')
    ) AS source (Id, Clave, Nombre, Descripcion, DelegacionId, LicenciaId, ClearanceId)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Clave, Nombre, Descripcion, DelegacionId, LicenciaId, ClearanceId, IsActive, CreatedAtUtc, UpdatedAtUtc)
        VALUES (
            TRY_CAST(source.Id AS uniqueidentifier),
            source.Clave,
            source.Nombre,
            source.Descripcion,
            TRY_CAST(source.DelegacionId AS uniqueidentifier),
            TRY_CAST(source.LicenciaId AS uniqueidentifier),
            TRY_CAST(source.ClearanceId AS uniqueidentifier),
            1,
            '2025-01-01T00:00:00Z',
            '2025-01-01T00:00:00Z');

    -- Permisos
    MERGE INTO [Permisos] AS target
    USING (VALUES
        ('f1d2c6fe-39f8-4832-9e2d-468f4bea7c31', 'SEGURIDAD.USUARIOS.GESTIONAR', 'Gestionar usuarios', 'Seguridad', 'Usuarios', 1),
        ('8fe47fa7-0fc5-4dae-b426-2e1dd7d5e47c', 'SEGURIDAD.USUARIOS.VER', 'Ver usuarios', 'Seguridad', 'Usuarios', 0),
        ('76d8cc5f-4224-4ef3-a594-984cd5c50a62', 'SEGURIDAD.ROLES.GESTIONAR', 'Gestionar roles', 'Seguridad', 'Roles', 1),
        ('d854df08-26d2-4af7-98d3-49816d23c01f', 'OPERACIONES.CONTEXTOS.GESTIONAR', 'Gestionar contextos', 'Operaciones', 'Contextos', 1),
        ('dc6c8079-0af4-4a35-86d6-c9f6a4541bc9', 'OPERACIONES.CONTEXTOS.VER', 'Ver contextos', 'Operaciones', 'Contextos', 0),
        ('c5e0a3ea-4fe8-4f8c-a2a0-3cfc4fb28eb1', 'OPERACIONES.LICENCIAS.GESTIONAR', 'Gestionar licencias', 'Operaciones', 'Licencias', 1)
    ) AS source (Id, Codigo, Nombre, Modulo, Categoria, EsCritico)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Codigo, Nombre, Modulo, Categoria, EsCritico, CreatedAtUtc, UpdatedAtUtc)
        VALUES (
            TRY_CAST(source.Id AS uniqueidentifier),
            source.Codigo,
            source.Nombre,
            source.Modulo,
            source.Categoria,
            source.EsCritico,
            '2025-01-01T00:00:00Z',
            '2025-01-01T00:00:00Z');

    -- Roles
    MERGE INTO [Roles] AS target
    USING (VALUES
        ('3fbdede0-0773-4cb6-9881-9350d0f4955e', 'Administrador Global', 'Acceso total a la plataforma.', 1),
        ('a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4', 'Gestor de Delegación', 'Gestiona recursos dentro de su delegación.', 0),
        ('a9d6d2dc-136f-42e4-9b77-75568128ed5f', 'Operador', 'Operativa diaria de la plataforma.', 0)
    ) AS source (Id, Nombre, Descripcion, IsSystem)
    ON target.Id = TRY_CAST(source.Id AS uniqueidentifier)
    WHEN NOT MATCHED THEN
        INSERT (Id, Nombre, Descripcion, IsSystem, CreatedAtUtc, UpdatedAtUtc)
        VALUES (
            TRY_CAST(source.Id AS uniqueidentifier),
            source.Nombre,
            source.Descripcion,
            source.IsSystem,
            '2025-01-01T00:00:00Z',
            '2025-01-01T00:00:00Z');

    -- Rol permisos
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
        SELECT 1
        FROM [RolPermisos] existing
        WHERE existing.RolId = TRY_CAST(rp.RolId AS uniqueidentifier)
          AND existing.PermisoId = TRY_CAST(rp.PermisoId AS uniqueidentifier));

    -- Delegación roles
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
        SELECT 1
        FROM [DelegacionRoles] existing
        WHERE existing.DelegacionId = TRY_CAST(dr.DelegacionId AS uniqueidentifier)
          AND existing.RolId = TRY_CAST(dr.RolId AS uniqueidentifier));

    INSERT INTO [SeedVersions] (Id, [Key], Version, AppliedAtUtc)
    VALUES ('d6e36d75-6f90-4d90-b1b4-0d423b99d8f1', 'CoreAuthorizationSeed', 1, SYSUTCDATETIME());
END

COMMIT;
