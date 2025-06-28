namespace TSqlFormatter.Parser
{
    public abstract class Expression
    {
        ExpressionType Type {get; set;}
        int Start {get; set;}
        int End {get; set;}

        public void Format()
        {
            
        }
    }

    public enum ExpressionType
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