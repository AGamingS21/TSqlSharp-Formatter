namespace TSqlFormatter.Parser
{
    public class AbstractSyntaxTree
    {
        

        public AbstractSyntaxTree()
        {
            
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

        private void Keyword(string value)
        {
            // switch(value.ToLower())
            // {
            //     case StatementType.Select.:
            //         //
            //         break;
            // }
        }
    }
}