IF OBJECT_ID('dbo.m_fabric_inward', 'U') IS NOT NULL
AND COL_LENGTH('dbo.m_fabric_inward', 'attached_file') IS NULL
BEGIN
    ALTER TABLE dbo.m_fabric_inward ADD attached_file NVARCHAR(500) NULL;
END
