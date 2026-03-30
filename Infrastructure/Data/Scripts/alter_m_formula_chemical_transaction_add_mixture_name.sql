IF COL_LENGTH('dbo.m_formula_chemical_transaction', 'mixture_name') IS NULL
BEGIN
    ALTER TABLE [dbo].[m_formula_chemical_transaction]
    ADD [mixture_name] NVARCHAR(200) NOT NULL
        CONSTRAINT [DF_m_formula_chemical_transaction_mixture_name] DEFAULT ('');
END
