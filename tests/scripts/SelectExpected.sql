DROP TABLE IF EXISTS
  #test 
SELECT
  'testing dateADD' AS test,
   *,
   col,
   col + col,
   1 + 1 AS [add],
   DATEADD(day, 1, GETDATE())AS DATEADD,
   CAST(GETDATE()AS DATE)'castDate' 
INTO
  #test 
FROM
  dbo . [script1] AS s1 
WHERE
  col = 1 
  AND
  1 = 1 
  AND
  'yes' < > 'no' 
ORDER BY
  test 
