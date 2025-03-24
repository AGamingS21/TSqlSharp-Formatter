namespace TSqlFormatter.Parser
{
    public class Statement
    {
        StatementType Type {get; set;}
        int Start {get; set;}
        int End {get; set;}
        // Clauses
    }

    public enum StatementType
    {
        Select,
        Drop,
        Create,
        Alter,
        Update,
        Add
    }

    public class Clause
    {
        ClauseType Type {get;set;}
        string Value {get;set;}
        List<string> Columns {get;set;}
    }
    public enum ClauseType
    {
        Select,
        From,
        Where,
        OrderBy,
        GroupBy
    }

}