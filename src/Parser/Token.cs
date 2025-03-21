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
        public TokenType Type {get; set;}
        public Token(TokenType tokenType, string value)
        {
            this.Type = tokenType;
            this.Value = value;
        }
    }
}