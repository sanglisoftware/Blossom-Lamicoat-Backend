IF OBJECT_ID('dbo.m_unit_of_measurement', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[m_unit_of_measurement]
    (
        [id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [name] NVARCHAR(100) NOT NULL,
        [is_active] SMALLINT NULL CONSTRAINT [DF_m_unit_of_measurement_is_active] DEFAULT ((1))
    );
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[m_unit_of_measurement] WHERE [name] = N'KG')
BEGIN
    INSERT INTO [dbo].[m_unit_of_measurement] ([name], [is_active]) VALUES (N'KG', 1);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[m_unit_of_measurement] WHERE [name] = N'LTR')
BEGIN
    INSERT INTO [dbo].[m_unit_of_measurement] ([name], [is_active]) VALUES (N'LTR', 1);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[m_unit_of_measurement] WHERE [name] = N'PCS')
BEGIN
    INSERT INTO [dbo].[m_unit_of_measurement] ([name], [is_active]) VALUES (N'PCS', 1);
END;
