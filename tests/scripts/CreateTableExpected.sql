CREATE TABLE #test
(
    col1 INT NOT NULL,
    col2 BIT NULL,
    col3 VARCHAR(50),
    col3 NVARCHAR(MAX),
    col4 FLOAT,
    col5 NUMERIC NULL
)
INSERT INTO #test
(
    col1,
    col2
)
SELECT
    1,
    1