using TSqlFormatter.Parser;

namespace TSqlFormatter.Parser
{
    public class Node 
    {
        Token Token {get; set;}
        StatementType statementType {get; set;} 

    }

    
     public enum StatementType
    {
        Select,
        Comment,
        Drop
    }

}