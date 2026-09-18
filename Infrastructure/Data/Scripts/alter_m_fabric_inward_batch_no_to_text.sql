IF COL_LENGTH('dbo.m_fabric_inward', 'batch_no') IS NOT NULL
BEGIN
    ALTER TABLE dbo.m_fabric_inward
    ALTER COLUMN batch_no NVARCHAR(100) NOT NULL;
END;
