IF COL_LENGTH('dbo.m_chemical_inword', 'unit_of_measurement_id') IS NULL
BEGIN
    ALTER TABLE [dbo].[m_chemical_inword]
    ADD [unit_of_measurement_id] INT NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_m_chemical_inword_m_unit_of_measurement'
)
BEGIN
    ALTER TABLE [dbo].[m_chemical_inword]
    ADD CONSTRAINT [FK_m_chemical_inword_m_unit_of_measurement]
        FOREIGN KEY ([unit_of_measurement_id]) REFERENCES [dbo].[m_unit_of_measurement]([id]);
END;
