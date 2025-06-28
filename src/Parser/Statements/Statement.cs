namespace TSqlFormatter.Parser
{
    public abstract class Statement
    {
        public StatementType Type {get; set;}
        public int Start {get; set;}
        public int End {get; set;}

        public void Format()
        {

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

    // public class Clause
    // {
    //     ClauseType Type {get;set;}
    //     string? Value {get;set;}
    //     List<Column>? Columns {get;set;}
    // }
    // public enum ClauseType
    // {
    //     Select,
    //     From,
    //     Where,
    //     OrderBy,
    //     GroupBy
    // }

}