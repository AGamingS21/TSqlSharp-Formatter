DROP TABLE IF Exists #test
SELECT 
    *,
    1 + 1 AS [add],
    DATEADD(day, 1, GETDATE()) as dateadd

INTO #test
FROM [dbo].[script1] AS s1