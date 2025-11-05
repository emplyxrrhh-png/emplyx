-- Rollback para el seed CoreAuthorizationSeed versión 1
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF EXISTS (SELECT 1 FROM [SeedVersions] WHERE [Key] = 'CoreAuthorizationSeed' AND [Version] = 1)
BEGIN
    DELETE FROM [DelegacionRoles]
    WHERE [DelegacionId] IN (
        '8f9033a5-436b-4e1f-9d05-21c27c6c3bf2',
        'd84145b9-4a7f-4ea9-8b0c-d4b0f8f6c424',
        'e4d8d94a-9084-46c3-99c1-1b4ef76e6a19')
      AND [RolId] IN (
        '3fbdede0-0773-4cb6-9881-9350d0f4955e',
        'a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4',
        'a9d6d2dc-136f-42e4-9b77-75568128ed5f');

    DELETE FROM [RolPermisos]
    WHERE [RolId] IN (
        '3fbdede0-0773-4cb6-9881-9350d0f4955e',
        'a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4',
        'a9d6d2dc-136f-42e4-9b77-75568128ed5f');

    DELETE FROM [Roles]
    WHERE [Id] IN (
        '3fbdede0-0773-4cb6-9881-9350d0f4955e',
        'a43b4a24-9d24-4c51-94f0-d5e8b9d7eaf4',
        'a9d6d2dc-136f-42e4-9b77-75568128ed5f');

    DELETE FROM [Permisos]
    WHERE [Id] IN (
        'f1d2c6fe-39f8-4832-9e2d-468f4bea7c31',
        '8fe47fa7-0fc5-4dae-b426-2e1dd7d5e47c',
        '76d8cc5f-4224-4ef3-a594-984cd5c50a62',
        'd854df08-26d2-4af7-98d3-49816d23c01f',
        'dc6c8079-0af4-4a35-86d6-c9f6a4541bc9',
        'c5e0a3ea-4fe8-4f8c-a2a0-3cfc4fb28eb1');

    DELETE FROM [Contextos]
    WHERE [Id] = 'caa6d616-f237-4d2b-a3f1-bf54cd508c35';

    DELETE FROM [Delegaciones]
    WHERE [Id] IN (
        'd84145b9-4a7f-4ea9-8b0c-d4b0f8f6c424',
        'e4d8d94a-9084-46c3-99c1-1b4ef76e6a19',
        '8f9033a5-436b-4e1f-9d05-21c27c6c3bf2');

    DELETE FROM [Licencias]
    WHERE [Id] = 'b7d6c9b1-08a1-4c9a-8ba7-824b6e7f8c29';

    DELETE FROM [Clearances]
    WHERE [Id] IN (
        '0ad1bc87-5e61-4258-8f51-2305b7c5401d',
        '25dfbb4d-8f45-4a54-958f-07daa4619052',
        '5d7a9d25-c4b2-4b93-93c6-a39c47da1f40');

    DELETE FROM [SeedVersions]
    WHERE [Id] = 'd6e36d75-6f90-4d90-b1b4-0d423b99d8f1';
END

COMMIT;
