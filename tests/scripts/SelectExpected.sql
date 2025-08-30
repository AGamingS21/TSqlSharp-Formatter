DROP TABLE IF EXISTS #test
SELECT
    'testing dateADD' AS test,
    *,
    col,
    col + col,
    1 + 1 AS [add],
    DATEADD(day, 1, GETDATE()) AS dateadd,
    CAST(GETDATE() AS DATE) AS castDate
INTO
    #test
FROM
    dbo.script1 AS s1
LEFT JOIN #script2 AS s2
    ON s2.test = s1.test
    AND S2.test = S3.test
LEFT JOIN #script3 AS s3
    ON s3.test = s2.test
INNER JOIN #script4 AS s4
    ON s4.test = s3.test
WHERE
    col = 1
    AND 1 = 1
    AND 'yes' <> 'no'
ORDER BY
    test
UPDATE
    #Test
SET
    a = 1
WHERE
    a = 2