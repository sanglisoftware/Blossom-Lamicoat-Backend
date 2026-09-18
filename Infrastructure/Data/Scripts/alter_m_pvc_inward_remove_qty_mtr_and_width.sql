DECLARE @columnName SYSNAME;
DECLARE @constraintName SYSNAME;
DECLARE @sql NVARCHAR(MAX);

DECLARE columns_to_remove CURSOR LOCAL FAST_FORWARD FOR
    SELECT column_name
    FROM (VALUES ('qty_mtr'), ('width_master_id'), ('width_name')) columns(column_name);

OPEN columns_to_remove;
FETCH NEXT FROM columns_to_remove INTO @columnName;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF COL_LENGTH('dbo.m_pvc_inward', @columnName) IS NOT NULL
    BEGIN
        SELECT @constraintName = dc.name
        FROM sys.default_constraints dc
        INNER JOIN sys.columns c
            ON c.object_id = dc.parent_object_id
            AND c.column_id = dc.parent_column_id
        WHERE dc.parent_object_id = OBJECT_ID('dbo.m_pvc_inward')
          AND c.name = @columnName;

        IF @constraintName IS NOT NULL
        BEGIN
            SET @sql = N'ALTER TABLE dbo.m_pvc_inward DROP CONSTRAINT ' + QUOTENAME(@constraintName);
            EXEC sp_executesql @sql;
        END;

        SET @sql = N'ALTER TABLE dbo.m_pvc_inward DROP COLUMN ' + QUOTENAME(@columnName);
        EXEC sp_executesql @sql;
    END;

    SET @constraintName = NULL;
    FETCH NEXT FROM columns_to_remove INTO @columnName;
END;

CLOSE columns_to_remove;
DEALLOCATE columns_to_remove;
