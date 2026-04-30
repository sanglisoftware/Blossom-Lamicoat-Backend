IF COL_LENGTH('m_chemical_inword', 'attached_file') IS NULL
BEGIN
    ALTER TABLE m_chemical_inword
    ADD attached_file NVARCHAR(500) NULL;
END;
