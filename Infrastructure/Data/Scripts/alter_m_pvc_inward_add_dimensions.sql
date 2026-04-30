IF COL_LENGTH('dbo.m_pvc_inward', 'new_rollno') IS NULL
BEGIN
    ALTER TABLE dbo.m_pvc_inward ADD new_rollno NVARCHAR(200) NOT NULL CONSTRAINT DF_m_pvc_inward_new_rollno DEFAULT '';
END

IF COL_LENGTH('dbo.m_pvc_inward', 'gramage_master_id') IS NULL
BEGIN
    ALTER TABLE dbo.m_pvc_inward ADD gramage_master_id INT NULL;
END

IF COL_LENGTH('dbo.m_pvc_inward', 'gramage_name') IS NULL
BEGIN
    ALTER TABLE dbo.m_pvc_inward ADD gramage_name NVARCHAR(200) NOT NULL CONSTRAINT DF_m_pvc_inward_gramage_name DEFAULT '';
END

IF COL_LENGTH('dbo.m_pvc_inward', 'width_master_id') IS NULL
BEGIN
    ALTER TABLE dbo.m_pvc_inward ADD width_master_id INT NULL;
END

IF COL_LENGTH('dbo.m_pvc_inward', 'width_name') IS NULL
BEGIN
    ALTER TABLE dbo.m_pvc_inward ADD width_name NVARCHAR(200) NOT NULL CONSTRAINT DF_m_pvc_inward_width_name DEFAULT '';
END

IF COL_LENGTH('dbo.m_pvc_inward', 'colour_master_id') IS NULL
BEGIN
    ALTER TABLE dbo.m_pvc_inward ADD colour_master_id INT NULL;
END

IF COL_LENGTH('dbo.m_pvc_inward', 'colour_name') IS NULL
BEGIN
    ALTER TABLE dbo.m_pvc_inward ADD colour_name NVARCHAR(200) NOT NULL CONSTRAINT DF_m_pvc_inward_colour_name DEFAULT '';
END

IF COL_LENGTH('dbo.m_pvc_inward', 'bill_date') IS NULL
BEGIN
    ALTER TABLE dbo.m_pvc_inward ADD bill_date DATETIME2 NOT NULL CONSTRAINT DF_m_pvc_inward_bill_date DEFAULT GETDATE();
END

IF COL_LENGTH('dbo.m_pvc_inward', 'received_date') IS NULL
BEGIN
    ALTER TABLE dbo.m_pvc_inward ADD received_date DATETIME2 NOT NULL CONSTRAINT DF_m_pvc_inward_received_date DEFAULT GETDATE();
END

IF COL_LENGTH('dbo.m_pvc_inward', 'attached_file') IS NULL
BEGIN
    ALTER TABLE dbo.m_pvc_inward ADD attached_file NVARCHAR(500) NULL;
END

IF COL_LENGTH('dbo.m_pvc_inward', 'is_active') IS NULL
BEGIN
    ALTER TABLE dbo.m_pvc_inward ADD is_active SMALLINT NULL CONSTRAINT DF_m_pvc_inward_is_active DEFAULT 1;
END
