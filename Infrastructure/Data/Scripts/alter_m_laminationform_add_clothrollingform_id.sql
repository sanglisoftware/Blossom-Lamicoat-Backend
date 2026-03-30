IF COL_LENGTH('dbo.m_laminationform', 'cloth_roll_code_id') IS NULL
BEGIN
    ALTER TABLE [dbo].[m_laminationform]
    ADD [cloth_roll_code_id] INT NULL;
END

IF COL_LENGTH('dbo.m_laminationform', 'cloth_roll_code') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[m_laminationform]
    ALTER COLUMN [cloth_roll_code] NVARCHAR(100) NULL;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_m_laminationform_m_clothrollingform'
)
BEGIN
    ALTER TABLE [dbo].[m_laminationform]
    ADD CONSTRAINT [FK_m_laminationform_m_clothrollingform]
        FOREIGN KEY ([cloth_roll_code_id]) REFERENCES [dbo].[m_clothrollingform]([id]);
END
