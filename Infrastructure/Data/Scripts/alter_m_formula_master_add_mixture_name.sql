IF COL_LENGTH('dbo.m_formula_master', 'mixture_name') IS NULL
BEGIN
    ALTER TABLE [dbo].[m_formula_master]
    ADD [mixture_name] NVARCHAR(200) NOT NULL
        CONSTRAINT [DF_m_formula_master_mixture_name] DEFAULT ('');
END
