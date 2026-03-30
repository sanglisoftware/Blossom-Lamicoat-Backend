IF OBJECT_ID('dbo.m_mixtureform', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[m_mixtureform]
    (
        [id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [formula_master_id] INT NOT NULL,
        [total_mixture] DECIMAL(18,2) NOT NULL,
        [created_date] DATETIME2 NOT NULL CONSTRAINT DF_m_mixtureform_created_date DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [FK_m_mixtureform_m_formula_master] FOREIGN KEY ([formula_master_id]) REFERENCES [dbo].[m_formula_master]([id])
    );
END;
