SET NOCOUNT ON;

MERGE dbo.m_grade AS target
USING (VALUES
    (N'First Grade'),
    (N'Second Grade'),
    (N'Bit'),
    (N'Cut Piece')
) AS source (name)
ON LOWER(LTRIM(RTRIM(target.name))) = LOWER(source.name)
WHEN MATCHED THEN UPDATE SET target.is_active = 1
WHEN NOT MATCHED THEN INSERT (name, is_active) VALUES (source.name, 1);
