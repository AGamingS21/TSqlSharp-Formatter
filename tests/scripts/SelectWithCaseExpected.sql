DROP TABLE IF EXISTS #Employees
SELECT
    *,
    CASE
        WHEN EmployeeType LIKE 'User%'
            THEN 'User'
    END AS EmployeeType,
    CASE
        WHEN Connection IS NOT NULL
            THEN 1
        ELSE 0
    END AS HasConnection,
    CASE
        WHEN A.EmployeeType = 'User'
        AND A.Path LIKE '%[^a-zA-Z0-9].[^a-zA-Z0-9]%'
            THEN SUBSTRING(Path, CHARINDEX('[', Path), CHARINDEX(']', Path, CHARINDEX(']', Path) + 1) - CHARINDEX('[', Path) + 1)
    END AS UserItem
INTO
    #Actions
FROM
    dbo.Employees
SELECT
    --*
    Path,
    SqlItem
FROM
    #Actions
WHERE
    UserItem = 'User'
-- AND (Path NOT LIKE '%For_each%' OR Path NOT LIKE '%Get_row%')