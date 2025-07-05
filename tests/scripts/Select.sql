DROP TABLE IF Exists #test
SELECT 
    'testing dateADD' AS test,
    * ,
    col,
    col + col       ,
    1 +             1 AS [add],
    dateAdd (day, 1, getdate()) as dateadd,
    CAST(GETDATE() AS Date) 'castDate'
    

INTO #test
FROM dbo.[script1] AS s1

Where col = 1 and 1=1 and 'yes' <> 'no'

order by 
    test