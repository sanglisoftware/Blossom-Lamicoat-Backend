IF OBJECT_ID('dbo.m_fabric_stock_return', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[m_fabric_stock_return]
    (
        [id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [fabric_inward_id] INT NOT NULL,
        [qty_mtr] FLOAT NOT NULL,
        [return_date] DATETIME2 NOT NULL,
        [remarks] NVARCHAR(500) NULL,
        [is_active] SMALLINT NOT NULL CONSTRAINT [DF_m_fabric_stock_return_is_active] DEFAULT ((1)),
        CONSTRAINT [FK_m_fabric_stock_return_m_fabric_inward]
            FOREIGN KEY ([fabric_inward_id]) REFERENCES [dbo].[m_fabric_inward]([id])
    );
END;
