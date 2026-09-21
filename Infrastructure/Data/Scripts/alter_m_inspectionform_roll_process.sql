IF COL_LENGTH('dbo.m_inspectionform', 'lamination_form_id') IS NULL
    ALTER TABLE dbo.m_inspectionform ADD lamination_form_id INT NULL;
IF COL_LENGTH('dbo.m_inspectionform', 'final_product_id') IS NULL
    ALTER TABLE dbo.m_inspectionform ADD final_product_id INT NULL;
IF COL_LENGTH('dbo.m_inspectionform', 'roll_no') IS NULL
    ALTER TABLE dbo.m_inspectionform ADD roll_no NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.m_inspectionform', 'roll_type') IS NULL
    ALTER TABLE dbo.m_inspectionform ADD roll_type NVARCHAR(50) NULL;

UPDATE dbo.m_inspectionform
SET roll_no = CONCAT(N'IR-LEGACY-', RIGHT(N'00000' + CONVERT(NVARCHAR(20), id), 5))
WHERE roll_no IS NULL OR LTRIM(RTRIM(roll_no)) = N'';

UPDATE inspection
SET roll_type = CASE
    WHEN LOWER(LTRIM(RTRIM(grade.name))) = N'bit' THEN N'Bit'
    WHEN LOWER(LTRIM(RTRIM(grade.name))) = N'cut piece' THEN N'Cut Piece'
    ELSE N'Roll'
END
FROM dbo.m_inspectionform inspection
LEFT JOIN dbo.m_grade grade ON grade.id = inspection.grade_id
WHERE inspection.roll_type IS NULL OR LTRIM(RTRIM(inspection.roll_type)) = N'';

ALTER TABLE dbo.m_inspectionform ALTER COLUMN manufactured_fabric_product_id INT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_m_inspectionform_m_laminationform')
    ALTER TABLE dbo.m_inspectionform ADD CONSTRAINT FK_m_inspectionform_m_laminationform
        FOREIGN KEY (lamination_form_id) REFERENCES dbo.m_laminationform(id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_m_inspectionform_m_final_product')
    ALTER TABLE dbo.m_inspectionform ADD CONSTRAINT FK_m_inspectionform_m_final_product
        FOREIGN KEY (final_product_id) REFERENCES dbo.m_final_product(id);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_m_inspectionform_roll_no' AND object_id = OBJECT_ID('dbo.m_inspectionform'))
    CREATE UNIQUE INDEX UX_m_inspectionform_roll_no
        ON dbo.m_inspectionform(roll_no) WHERE roll_no IS NOT NULL;
