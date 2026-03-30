IF OBJECT_ID('dbo.m_inspectionform', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[m_inspectionform]
    (
        [id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [manufactured_fabric_product_id] INT NOT NULL,
        [grade_id] INT NOT NULL,
        [mtr] DECIMAL(18,2) NOT NULL,
        [wastage_mtr] DECIMAL(18,2) NOT NULL,
        [created_date] DATETIME2 NOT NULL CONSTRAINT [DF_m_inspectionform_created_date] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [FK_m_inspectionform_m_fproductlist]
            FOREIGN KEY ([manufactured_fabric_product_id]) REFERENCES [dbo].[m_fproductlist]([id]),
        CONSTRAINT [FK_m_inspectionform_m_grade]
            FOREIGN KEY ([grade_id]) REFERENCES [dbo].[m_grade]([id])
    );
END;
