# TSqlFormatter



## TO DO NOW
- Get Select Statement into object
    - ~~Columns~~
    - ~~Into~~
    - ~~From~~
    - Where
        - still need to format with line breaks when changing clauses.
    - Order By
    - Group By
    - Having
    - Distinct
    - Top
- make it print nice
- repeast for drop table statement
- generisize
- do for more complex select statements
    - Sub-Queries
- determine next steps


## Steps:
- Create Parser
- Create Abstract Syntax Tree (AST)
- Format Code based on AST

## ToDo
- stream reader vs string lexing?
- get all keywords, identifierrs, operators etc

PLAN:
- Map SQL ScriptDom object to my own object I can easily maniplulate
- Then Properly format
- Try and create a more generic way to do syntactic analysis or formatting.
- Within each class try and have a their own format class??


## Resources
Starting point on how to create a parser
https://www.geeksforgeeks.org/introduction-of-compiler-design/?ref=lbp

Lexical Analyzer
https://www.geeksforgeeks.org/c-lexical-analyser-lexer/


### Notes
#### Lexical Analysis
Token Types
- Keywords
- Identifiers
- Constancts
- Operators
- Special Symbols


#### Other Options
Once created the AST and parser tryout using the microsoft t-sql parser and using that AST for formatting?



When to not Break Line:
- When indented out lines continue until comma or AND or OR or Brackets(This will and an indent)
- Do ni a 
