IF OBJECT_ID('dbo.m_clothrollingform', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[m_clothrollingform]
    (
        [id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [product_name] NVARCHAR(200) NOT NULL,
        [batch_no] NVARCHAR(100) NOT NULL,
        [roll_mtr] DECIMAL(18,2) NOT NULL,
        [defect_mtr] DECIMAL(18,2) NOT NULL,
        [checker_name] NVARCHAR(200) NOT NULL,
        [is_active] SMALLINT NOT NULL CONSTRAINT [DF_m_clothrollingform_is_active] DEFAULT ((1)),
        [created_date] DATETIME2 NOT NULL CONSTRAINT [DF_m_clothrollingform_created_date] DEFAULT (SYSUTCDATETIME())
    );
END;
