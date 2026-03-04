IF OBJECT_ID('dbo.m_salary', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[m_salary]
    (
        [id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [employee_id] INT NOT NULL,
        [type] SMALLINT NULL,
        [attendance] INT NOT NULL,
        [extra_hours] DECIMAL(10,2) NOT NULL CONSTRAINT DF_m_salary_extra_hours DEFAULT (0),
        [total_late] DECIMAL(10,2) NOT NULL CONSTRAINT DF_m_salary_total_late DEFAULT (0),
        [half_day] DECIMAL(10,2) NOT NULL CONSTRAINT DF_m_salary_half_day DEFAULT (0),
        [total_salary] DECIMAL(18,2) NOT NULL,
        [created_date] DATETIME2 NOT NULL CONSTRAINT DF_m_salary_created_date DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [FK_m_salary_m_employee] FOREIGN KEY ([employee_id]) REFERENCES [dbo].[m_employee]([id])
    );
END;
