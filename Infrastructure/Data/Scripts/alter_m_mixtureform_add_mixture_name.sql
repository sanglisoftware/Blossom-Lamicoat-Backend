IF COL_LENGTH('dbo.m_mixtureform', 'mixture_name') IS NULL
BEGIN
    ALTER TABLE [dbo].[m_mixtureform]
    ADD [mixture_name] NVARCHAR(200) NOT NULL
        CONSTRAINT [DF_m_mixtureform_mixture_name] DEFAULT ('');
END
