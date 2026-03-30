IF NOT EXISTS (
    SELECT 1
    FROM sys.tables
    WHERE name = 'm_laminationform'
)
BEGIN
    CREATE TABLE [dbo].[m_laminationform]
    (
        [id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [final_product_id] INT NOT NULL,
        [cloth_roll_code_id] INT NULL,
        [cloth_roll_batch_no] NVARCHAR(100) NOT NULL,
        [pvc_master_id] INT NULL,
        [pvc_batch_no] NVARCHAR(100) NOT NULL,
        [pvc_qty] DECIMAL(18,2) NOT NULL,
        [chemical_id] INT NOT NULL,
        [chemical_qty] DECIMAL(18,2) NOT NULL,
        [bounding] NVARCHAR(50) NOT NULL,
        [worker_id] INT NOT NULL,
        [temperature] DECIMAL(18,2) NOT NULL,
        [process_time] NVARCHAR(100) NOT NULL,
        [created_date] DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [FK_m_laminationform_m_final_product]
            FOREIGN KEY ([final_product_id]) REFERENCES [dbo].[m_final_product]([id]),
        CONSTRAINT [FK_m_laminationform_m_clothrollingform]
            FOREIGN KEY ([cloth_roll_code_id]) REFERENCES [dbo].[m_clothrollingform]([id]),
        CONSTRAINT [FK_m_laminationform_m_pvcproducttable]
            FOREIGN KEY ([pvc_master_id]) REFERENCES [dbo].[m_pvcproducttable]([id]),
        CONSTRAINT [FK_m_laminationform_m_chemical]
            FOREIGN KEY ([chemical_id]) REFERENCES [dbo].[m_chemical]([id]),
        CONSTRAINT [FK_m_laminationform_m_employee]
            FOREIGN KEY ([worker_id]) REFERENCES [dbo].[m_employee]([id])
    );
END
