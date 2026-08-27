IF COL_LENGTH('dbo.m_chemical_inword', 'batch_no') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[m_chemical_inword]
    ALTER COLUMN [batch_no] NVARCHAR(100) NOT NULL;
END;
