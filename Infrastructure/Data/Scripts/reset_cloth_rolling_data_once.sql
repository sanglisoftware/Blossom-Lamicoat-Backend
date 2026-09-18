SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;
SET XACT_ABORT ON;

IF OBJECT_ID('dbo.app_data_migration_log', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[app_data_migration_log]
    (
        [migration_key] NVARCHAR(200) NOT NULL PRIMARY KEY,
        [executed_at] DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME())
    );
END;

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[app_data_migration_log]
    WHERE [migration_key] = 'reset_cloth_rolling_for_new_numbering_20260918_v2'
)
BEGIN
    BEGIN TRANSACTION;

    IF COL_LENGTH('dbo.m_laminationform', 'cloth_roll_code_id') IS NOT NULL
        EXEC(N'UPDATE [dbo].[m_laminationform]
            SET [cloth_roll_code_id] = NULL WHERE [cloth_roll_code_id] IS NOT NULL;');

    IF COL_LENGTH('dbo.m_laminationform', 'cloth_rolling_form_id') IS NOT NULL
        EXEC(N'UPDATE [dbo].[m_laminationform]
            SET [cloth_rolling_form_id] = NULL WHERE [cloth_rolling_form_id] IS NOT NULL;');

    DELETE FROM [dbo].[m_clothrollingform];
    DBCC CHECKIDENT ('dbo.m_clothrollingform', RESEED, 0);

    INSERT INTO [dbo].[app_data_migration_log] ([migration_key])
    VALUES ('reset_cloth_rolling_for_new_numbering_20260918_v2');

    COMMIT TRANSACTION;
END;
