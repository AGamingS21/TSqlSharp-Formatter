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

left join #script2 as s2 on s2.test = s1.test AND S2.test = S3.test

left join #script3 as s3 on s3.test = s2.test

INNER join #script4 as s4 on s4.test = s3.test


Where col = 1 and 1=1 and 'yes' <> 'no'

order by 
    test


uPDaTE #Test


sEt 
  a     =   1


where 
  a = 2