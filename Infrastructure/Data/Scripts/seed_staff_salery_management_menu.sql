SET NOCOUNT ON;

DECLARE @ParentTitle NVARCHAR(255) = N'Staff Salery Management';
DECLARE @ParentIcon NVARCHAR(50) = N'Wallet';
DECLARE @ParentId INT;

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
END
ELSE
BEGIN
    UPDATE m_menu
    SET icon = @ParentIcon,
        path_name = NULL
    WHERE id = @ParentId;
END;

MERGE m_menu AS target
USING (
    VALUES
        (@ParentId, N'FileText', N'/salary-form', N'Salery Form', 1),
        (@ParentId, N'ClipboardList', N'/salary-report', N'Salery Report', 2),
        (@ParentId, N'CalendarDays', N'/daily-attendence', N'Daily Attendence', 3),
        (@ParentId, N'BarChart3', N'/attendence-report', N'Attendence Report', 4),
        (@ParentId, N'BadgeIndianRupee', N'/credit-salary', N'Credit Salery', 5)
) AS source (parent_id, icon, path_name, title, sequence)
ON target.parent_id = source.parent_id
   AND target.title = source.title
WHEN MATCHED THEN
    UPDATE SET
        target.icon = source.icon,
        target.path_name = source.path_name,
        target.sequence = source.sequence
WHEN NOT MATCHED THEN
    INSERT (parent_id, icon, path_name, title, sequence)
    VALUES (source.parent_id, source.icon, source.path_name, source.title, source.sequence);
