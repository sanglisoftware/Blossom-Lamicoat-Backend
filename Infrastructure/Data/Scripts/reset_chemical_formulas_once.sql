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
    WHERE [migration_key] = 'reset_all_chemical_formulas_20260919'
)
BEGIN
    BEGIN TRANSACTION;

    DELETE FROM [dbo].[m_mixtureform];
    DELETE FROM [dbo].[m_formula_chemical_transaction];
    DELETE FROM [dbo].[m_formula_master];

    DBCC CHECKIDENT ('dbo.m_mixtureform', RESEED, 0);
    DBCC CHECKIDENT ('dbo.m_formula_chemical_transaction', RESEED, 0);
    DBCC CHECKIDENT ('dbo.m_formula_master', RESEED, 0);

    INSERT INTO [dbo].[app_data_migration_log] ([migration_key])
    VALUES ('reset_all_chemical_formulas_20260919');

    COMMIT TRANSACTION;
END;
