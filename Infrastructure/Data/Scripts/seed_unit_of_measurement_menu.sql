SET NOCOUNT ON;

DECLARE @ParentTitle NVARCHAR(255) = N'Master';
DECLARE @ParentIcon NVARCHAR(50) = N'ShoppingBag';
DECLARE @ParentId INT;
DECLARE @Sequence INT;

SELECT @ParentId = id
FROM m_menu
WHERE title = @ParentTitle AND parent_id IS NULL;

IF @ParentId IS NULL
BEGIN
    INSERT INTO m_menu (parent_id, icon, path_name, title, sequence)
    VALUES (
        NULL,
        @ParentIcon,
        NULL,
        @ParentTitle,
        ISNULL((SELECT MAX(sequence) + 1 FROM m_menu WHERE parent_id IS NULL), 1)
    );

    SET @ParentId = SCOPE_IDENTITY();
END;

SELECT @Sequence = ISNULL(MAX(sequence) + 1, 1)
FROM m_menu
WHERE parent_id = @ParentId;

MERGE m_menu AS target
USING (
    VALUES
        (@ParentId, N'Ruler', N'/unit-of-measurement', N'Unit Of Measurement', @Sequence)
) AS source (parent_id, icon, path_name, title, sequence)
ON target.parent_id = source.parent_id
   AND target.title = source.title
WHEN MATCHED THEN
    UPDATE SET
        target.icon = source.icon,
        target.path_name = source.path_name
WHEN NOT MATCHED THEN
    INSERT (parent_id, icon, path_name, title, sequence)
    VALUES (source.parent_id, source.icon, source.path_name, source.title, source.sequence);
