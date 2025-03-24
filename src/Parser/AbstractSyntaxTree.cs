namespace TSqlFormatter.Parser
{
    public class AbstractSyntaxTree
    {
        List<Node> statements {get; set;}

        public AbstractSyntaxTree()
        {
            statements = new List<Node>();
        }

        // syntactic analysis and building ast
        public void BuildAST(List<Token> tokens)
        {
            
            foreach(var token in tokens)
            {
                switch(token.TokenType)
                {
                    case TokenType.Keyword:
                        break;
                    case TokenType.Identifier:
                        break;
                    case TokenType.UnknownIdentifier:
                        break;
                    case TokenType.Operator:
                        break;
                    case TokenType.Delimiter:
                        break;
                    case TokenType.Integer:
                        break;
                }       
            }
        }
    }
}