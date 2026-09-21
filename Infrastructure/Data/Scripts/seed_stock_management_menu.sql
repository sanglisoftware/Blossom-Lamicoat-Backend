SET NOCOUNT ON;

DECLARE @ParentId INT;

SELECT @ParentId = id FROM m_menu
WHERE title = N'Stock Management' AND parent_id IS NULL;

IF @ParentId IS NULL
BEGIN
    INSERT INTO m_menu (parent_id, icon, path_name, title, sequence)
    VALUES (NULL, N'Warehouse', NULL, N'Stock Management',
        ISNULL((SELECT MAX(sequence) + 1 FROM m_menu WHERE parent_id IS NULL), 1));
    SET @ParentId = SCOPE_IDENTITY();
END;

UPDATE m_menu
SET title = N'Uninspected Finished Stock'
WHERE parent_id = @ParentId
  AND path_name = N'/stock-management?tab=finished'
  AND title = N'Finished Goods Stock';

MERGE m_menu AS target
USING (VALUES
    (@ParentId, N'FlaskConical', N'/stock-management?tab=chemical', N'Chemical Stock', 1),
    (@ParentId, N'Beaker', N'/stock-management?tab=mixture', N'Mixture Stock', 2),
    (@ParentId, N'ScrollText', N'/stock-management?tab=fabric', N'Fabric Stock', 3),
    (@ParentId, N'Layers3', N'/stock-management?tab=pvc', N'PVC Stock', 4),
    (@ParentId, N'PackageCheck', N'/stock-management?tab=finished', N'Uninspected Finished Stock', 5),
    (@ParentId, N'BadgeCheck', N'/stock-management?tab=graded', N'Grade-wise Roll Stock', 6)
) AS source (parent_id, icon, path_name, title, sequence)
ON target.parent_id = source.parent_id AND target.title = source.title
WHEN MATCHED THEN UPDATE SET
    target.icon = source.icon, target.path_name = source.path_name, target.sequence = source.sequence
WHEN NOT MATCHED THEN INSERT (parent_id, icon, path_name, title, sequence)
VALUES (source.parent_id, source.icon, source.path_name, source.title, source.sequence);

INSERT INTO role_menu_permission_transaction
    (role_id, menu_id, create_permission, update_permission, delete_permission)
SELECT 1, menu.id, 1, 1, 1
FROM m_menu menu
WHERE (menu.id = @ParentId OR menu.parent_id = @ParentId)
  AND NOT EXISTS (
      SELECT 1 FROM role_menu_permission_transaction permission
      WHERE permission.role_id = 1 AND permission.menu_id = menu.id
  );
