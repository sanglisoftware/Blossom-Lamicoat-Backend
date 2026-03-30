IF COL_LENGTH('dbo.m_inspectionform', 'roll_no') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[m_inspectionform] DROP COLUMN [roll_no];
END

IF COL_LENGTH('dbo.m_inspectionform', 'inspector_name') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[m_inspectionform] DROP COLUMN [inspector_name];
END
