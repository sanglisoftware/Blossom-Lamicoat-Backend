IF COL_LENGTH('dbo.m_employee', 'daily_salary') IS NULL
BEGIN
    ALTER TABLE [dbo].[m_employee]
    ADD [daily_salary] DECIMAL(18,2) NOT NULL
        CONSTRAINT [DF_m_employee_daily_salary] DEFAULT (0);
END;
