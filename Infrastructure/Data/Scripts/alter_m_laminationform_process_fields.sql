IF COL_LENGTH('dbo.m_laminationform', 'pvc_inward_id') IS NULL
    ALTER TABLE dbo.m_laminationform ADD pvc_inward_id INT NULL;

IF COL_LENGTH('dbo.m_laminationform', 'mixture_formula_master_id') IS NULL
    ALTER TABLE dbo.m_laminationform ADD mixture_formula_master_id INT NULL;

IF COL_LENGTH('dbo.m_laminationform', 'mixture_qty') IS NULL
    ALTER TABLE dbo.m_laminationform ADD mixture_qty DECIMAL(18, 2) NOT NULL CONSTRAINT DF_m_laminationform_mixture_qty DEFAULT 0;

IF COL_LENGTH('dbo.m_laminationform', 'final_product_qty_mtr') IS NULL
    ALTER TABLE dbo.m_laminationform ADD final_product_qty_mtr DECIMAL(18, 2) NOT NULL CONSTRAINT DF_m_laminationform_final_product_qty DEFAULT 0;

IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.m_laminationform') AND name = 'chemical_id' AND is_nullable = 0
)
    ALTER TABLE dbo.m_laminationform ALTER COLUMN chemical_id INT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_m_laminationform_m_pvc_inward')
    ALTER TABLE dbo.m_laminationform ADD CONSTRAINT FK_m_laminationform_m_pvc_inward
        FOREIGN KEY (pvc_inward_id) REFERENCES dbo.m_pvc_inward(id);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_m_laminationform_m_formula_master')
    ALTER TABLE dbo.m_laminationform ADD CONSTRAINT FK_m_laminationform_m_formula_master
        FOREIGN KEY (mixture_formula_master_id) REFERENCES dbo.m_formula_master(id);
