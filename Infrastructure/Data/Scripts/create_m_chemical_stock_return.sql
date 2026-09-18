IF OBJECT_ID(N'dbo.m_chemical_stock_return', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.m_chemical_stock_return
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        chemical_master_id INT NOT NULL,
        qty FLOAT NOT NULL,
        return_date DATETIME2 NOT NULL CONSTRAINT DF_m_chemical_stock_return_date DEFAULT GETDATE(),
        remarks NVARCHAR(500) NULL,
        is_active SMALLINT NULL CONSTRAINT DF_m_chemical_stock_return_active DEFAULT 1,
        CONSTRAINT FK_m_chemical_stock_return_chemical
            FOREIGN KEY (chemical_master_id) REFERENCES dbo.m_chemical(id)
    );
END;
