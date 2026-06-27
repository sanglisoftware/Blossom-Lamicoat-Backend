IF OBJECT_ID('dbo.m_fabric_inward', 'U') IS NOT NULL
AND COL_LENGTH('dbo.m_fabric_inward', 'grmmsterid') IS NULL
BEGIN
    ALTER TABLE dbo.m_fabric_inward ADD grmmsterid INT NULL;
END

IF OBJECT_ID('dbo.m_fabric_inward', 'U') IS NOT NULL
AND COL_LENGTH('dbo.m_fabric_inward', 'colourmasterid') IS NULL
BEGIN
    ALTER TABLE dbo.m_fabric_inward ADD colourmasterid INT NULL;
END
