INSERT [dbo].[sysroUserFields] ([FieldName], [FieldType], [Used], [AccessLevel], [Pos], [Category], [ListValues], [Type], [Description], [AccessValidation], [History], [RequestPermissions], [RequestCriteria], [isSystem], [Alias], [DocumentTemplateId], [ReadOnly], [Unique]) VALUES (N'_translate_socialDesignation', 0, 1, 0, NULL, N'', N'', 1, N'Info.socialDesignation', 0, 0, 0, N'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>', 1, NULL, NULL, 0, 0)
GO

INSERT [dbo].[sysroUserFields] ([FieldName], [FieldType], [Used], [AccessLevel], [Pos], [Category], [ListValues], [Type], [Description], [AccessValidation], [History], [RequestPermissions], [RequestCriteria], [isSystem], [Alias], [DocumentTemplateId], [ReadOnly], [Unique]) VALUES (N'_translate_workPlace', 0, 1, 0, NULL, N'', N'', 1, N'Info.workPlace', 0, 0, 0, N'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>', 1, NULL, NULL, 0, 0)
GO

INSERT [dbo].[sysroUserFields] ([FieldName], [FieldType], [Used], [AccessLevel], [Pos], [Category], [ListValues], [Type], [Description], [AccessValidation], [History], [RequestPermissions], [RequestCriteria], [isSystem], [Alias], [DocumentTemplateId], [ReadOnly], [Unique]) VALUES (N'_translate_headquarters', 0, 1, 0, NULL, N'', N'', 1, N'Info.headquarters', 0, 0, 0, N'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>', 1, NULL, NULL, 0, 0)
GO

INSERT [dbo].[sysroUserFields] ([FieldName], [FieldType], [Used], [AccessLevel], [Pos], [Category], [ListValues], [Type], [Description], [AccessValidation], [History], [RequestPermissions], [RequestCriteria], [isSystem], [Alias], [DocumentTemplateId], [ReadOnly], [Unique]) VALUES (N'_translate_workingShift', 0, 1, 0, NULL, N'', N'', 1, N'Info.workingShift', 0, 0, 0, N'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>', 1, NULL, NULL, 0, 0)
GO

INSERT [dbo].[sysroUserFields] ([FieldName], [FieldType], [Used], [AccessLevel], [Pos], [Category], [ListValues], [Type], [Description], [AccessValidation], [History], [RequestPermissions], [RequestCriteria], [isSystem], [Alias], [DocumentTemplateId], [ReadOnly], [Unique]) VALUES (N'_translate_economicActivity', 0, 1, 0, NULL, N'', N'', 1, N'Info.economicActivity', 0, 0, 0, N'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>', 1, NULL, NULL, 0, 0)
GO

INSERT [dbo].[sysroUserFields] ([FieldName], [FieldType], [Used], [AccessLevel], [Pos], [Category], [ListValues], [Type], [Description], [AccessValidation], [History], [RequestPermissions], [RequestCriteria], [isSystem], [Alias], [DocumentTemplateId], [ReadOnly], [Unique]) VALUES (N'_translate_colectiveInstReg', 0, 1, 0, NULL, N'', N'', 1, N'Info.colectiveInstReg', 0, 0, 0, N'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>', 1, NULL, NULL, 0, 0)
GO

ALTER TABLE GROUPS ADD USR__translate_socialDesignation text
GO

ALTER TABLE GROUPS ADD USR__translate_workPlace text
GO

ALTER TABLE GROUPS ADD USR__translate_headquarters text
GO

ALTER TABLE GROUPS ADD USR__translate_workingShift text
GO

ALTER TABLE GROUPS ADD USR__translate_economicActivity text
GO

ALTER TABLE GROUPS ADD USR__translate_colectiveInstReg text
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1051' WHERE ID='DBVersion'
GO
