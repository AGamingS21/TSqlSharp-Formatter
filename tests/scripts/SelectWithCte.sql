 DrOP TABLE 
 IF EXiSTs 
 #Employees
        SELECT 
            -- Testing enter statement
            Username, -- username field 
            ManagerUsername, 
            Username AS OriginalID INTO #Employees FROM dbo.Employees  AS E WITH (NOLOCK)
        
        ;WITH employee_manager_cte AS ( SELECT
            Username,
            ManagerUsername,1 AS [level],
            OriginalID
        FROM #Employees
                                    UNION ALL
                        -- cte union to get maanager
    SELECT
        E.Username,
        CTE.ManagerUsername,
        CTE.level + 1,
        CTE.OriginalID
    FROM #Employees AS E
    INNER JOIN employee_manager_cte AS CTE -- cte table
        ON E.Username = CTE.ManagerUsername -- comment about joining field
                            )

SELECT 
    -- comment about this field.
    cte.Username,cte.ManagerUsername,
    MAX(cte.level),ROW_NUMBER() OVER(ORDER BY EmployeeName) AS rn
FROM employee_manager_cte AS cte
WHERE cte.Username IS not NULL AND -- comment about this and statement
level > 1
group by cte.ManagerUsername, -- comment about this username
Username -- another comment         
having max(cte.level) > 1