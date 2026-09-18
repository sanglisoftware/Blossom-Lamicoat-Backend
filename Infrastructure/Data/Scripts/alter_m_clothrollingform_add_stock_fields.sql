IF COL_LENGTH('dbo.m_clothrollingform', 'fabric_inward_id') IS NULL
    EXEC(N'ALTER TABLE [dbo].[m_clothrollingform] ADD [fabric_inward_id] INT NULL;');

IF COL_LENGTH('dbo.m_clothrollingform', 'roll_no') IS NULL
    EXEC(N'ALTER TABLE [dbo].[m_clothrollingform] ADD [roll_no] NVARCHAR(100) NULL;');

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_m_clothrollingform_m_fabric_inward')
    EXEC(N'ALTER TABLE [dbo].[m_clothrollingform] ADD CONSTRAINT [FK_m_clothrollingform_m_fabric_inward]
        FOREIGN KEY ([fabric_inward_id]) REFERENCES [dbo].[m_fabric_inward]([id]);');

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_m_clothrollingform_roll_no' AND object_id = OBJECT_ID('dbo.m_clothrollingform'))
    EXEC(N'CREATE UNIQUE INDEX [UX_m_clothrollingform_roll_no]
        ON [dbo].[m_clothrollingform]([roll_no]) WHERE [roll_no] IS NOT NULL;');
