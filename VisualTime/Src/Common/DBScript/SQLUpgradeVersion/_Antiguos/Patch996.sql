 WITH XMLNAMESPACES ('2.0' AS ns)
    UPDATE dailyschedule
    SET layersdefinition = 
        CONVERT(NVARCHAR(MAX), (
            SELECT
                (
                    SELECT 
                        Item.value('@key', 'NVARCHAR(100)') AS [@key],
                        Item.value('@type', 'NVARCHAR(10)') AS [@type],
                        CASE 
                            WHEN Item.value('@type', 'NVARCHAR(10)') = '5' 
                            THEN REPLACE(Item.value('.', 'NVARCHAR(MAX)'), ',', '.') 
                            ELSE Item.value('.', 'NVARCHAR(MAX)') 
                        END AS [text()]
                    FROM nodesData.nodes('/roCollection/Item') AS X(Item)
                    FOR XML PATH('Item'), ROOT('roCollection'), TYPE
                )
        ))
    FROM dailyschedule
    CROSS APPLY (SELECT TRY_CAST(layersdefinition AS XML)) AS ParsedXml(nodesData)
    WHERE layersdefinition LIKE '%type="5"%,%' 
          AND layersdefinition IS NOT NULL 
          AND TRY_CAST(layersdefinition AS XML) IS NOT NULL
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='996' WHERE ID='DBVersion'
GO
