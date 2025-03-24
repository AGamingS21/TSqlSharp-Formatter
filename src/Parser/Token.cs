namespace TSqlFormatter.Parser
{
    public enum TokenType
    {
        Identifier,
        UnknownIdentifier,
        Keyword,
        Operator,
        Delimiter,
        Integer
        
    }

   
    public class Token
    {
        public string Value {get; set;}
        public TokenType TokenType {get; set;}
        public int Start {get; set;}
        public int End {get; set;}
        public Token(TokenType tokenType, string value, int start, int end)
        {
            this.TokenType = tokenType;
            this.Value = value;
            this.Start = start;
            this.End = end;
        }
        
    }
}