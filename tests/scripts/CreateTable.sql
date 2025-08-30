CReATE Table #test
(
    col1 int NOT NULL,
    col2 bit null,
    col3 varchar(50),
    col3 nvarchar(max),
    col4 float,
    col5 NUMERIC null
)
INSERT INTO #test (col1, col2)
select 
    1,
    1