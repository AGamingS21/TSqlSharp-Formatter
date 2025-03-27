namespace TSqlFormatter.Parser
{
    public class Statement
    {
        StatementType Type {get; set;}
        int Start {get; set;}
        int End {get; set;}
        List<Clause>? Clauses {get; set;}

        public Statement(StatementType statementType, int start, int end, List<Clause> clauses)
        {
            this.Type = statementType;
            this.Start = start;
            this.End = end;
            this. Clauses = clauses;
        }
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
        string? Value {get;set;}
        List<Column>? Columns {get;set;}
    }
    public enum ClauseType
    {
        Select,
        From,
        Where,
        OrderBy,
        GroupBy
    }

    public class Column
    {
        string Value {get; set;}
        string Alias {get; set;}
    }

}