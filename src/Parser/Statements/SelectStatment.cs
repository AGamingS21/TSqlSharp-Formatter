using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSqlFormatter.Formatter;

namespace TSqlFormatter.Parser
{
    public class SelectStmnt : Statement
    {
        // Next need to make sure all values are filled.
        // How to figure out expressions in columns, where, etc instead of regular text fields?
        public List<Column> Columns { get; set; }
        public FromClause From { get; set; }
        public IntoClause Into { get; set; }
        public WhereClause Where { get; set; }
        public OrderByClause OrderBy { get; set; }
        public GroupByClause GroupBy { get; set; }
        public HavingClause Having { get; set; }
        public DistinctClause Distinct { get; set; }
        public TopClause Top { get; set; }
        private List<FormatToken> FormatTokens { get; set; }

        public SelectStmnt(Microsoft.SqlServer.TransactSql.ScriptDom.SelectStatement selectStatement)
        {
            this.Start = selectStatement.FirstTokenIndex;
            this.End = selectStatement.LastTokenIndex;
            this.Type = StatementType.Select;

            Columns = new List<Column>();
            From = new FromClause();
            Into = new IntoClause();

            // Refactor this into its own methods
            FormatTokens = new List<FormatToken>();
            Format(selectStatement);
            Print(FormatTokens);
        }

        private List<FormatToken> Format(Microsoft.SqlServer.TransactSql.ScriptDom.SelectStatement select)
        {
            int CurrentCol = 0;
            

            
            var expression = (QuerySpecification)select.QueryExpression;

            
            // Columns

            int j = 1;
            foreach (var column in expression.SelectElements)
            {
                var newcolumn = new Column();

                bool isExpression = false;

                for (int i = column.FirstTokenIndex; i <= column.LastTokenIndex; i++)
                {
                    var token = column.ScriptTokenStream[i];


                    // Go through the strings and format
                    if (token.TokenType.ToString() == "WhiteSpace")
                    {
                        newcolumn.value += " ";
                    }
                    else if (token.IsKeyword())
                    {
                        newcolumn.value += token.Text.ToUpper();
                    }
                    // There is an operator so we are inside of an expression
                    else if (Tokens.Operators.Contains(token.Text))
                    {
                        newcolumn.value += token.Text;
                        isExpression = true;
                    }
                    // How to determine if it is a column alias vs actually a known identifier?
                    else if (Tokens.Identifiers.Contains(token.Text.ToUpper()))
                    {
                        newcolumn.value += token.Text.ToUpper();
                    }
                    else
                    {
                        newcolumn.value += token.Text;
                    }

                    // Last key. Look for an alias
                    if (i == column.LastTokenIndex && i != column.FirstTokenIndex)
                    {
                        if (column.ScriptTokenStream[i - 1].TokenType.ToString() == "WhiteSpace" && !Tokens.Operators.Contains(column.ScriptTokenStream[i - 2].Text))
                        {
                            newcolumn.alias = token.Text;

                            newcolumn.value = RemoveAlias(newcolumn.value, newcolumn.alias);
                        }
                    }


                }
                if (j != expression.SelectElements.Count)
                {
                    newcolumn.value += ",";
                }
                j++;

                FormatTokens.Add(new FormatToken(CurrentCol,column.LastTokenIndex,newcolumn.value, "Column"));  
                Columns.Add(newcolumn);


            }
            CurrentCol -= 4;

            // FROM 
            var fromToken = expression.FromClause;
            bool isReference = true;
            for (int i = fromToken.FirstTokenIndex; i <= fromToken.LastTokenIndex; i++)
            {
                var token = fromToken.ScriptTokenStream[i];
                if (token.TokenType != TSqlTokenType.From && token.TokenType != TSqlTokenType.WhiteSpace && token.TokenType != TSqlTokenType.As)
                {

                    // Determine if it is a table reference based on the dot.
                    if (isReference)
                    {
                        From.identifier += token.Text + '.';
                        isReference = false;
                    }
                    else
                    {
                        From.alias = token.Text;
                    }

                    if (token.TokenType == TSqlTokenType.Dot)
                    {
                        isReference = true;
                    }   
                }
            }

            From.identifier = From.identifier.Remove(From.identifier.Length -1);



            // Into
            var intoToken = select.Into;
            
            isReference = true;
            for (int i = intoToken.FirstTokenIndex; i <= intoToken.LastTokenIndex; i++)
            {
                var token = intoToken.ScriptTokenStream[i];
                if (token.TokenType != TSqlTokenType.Into && token.TokenType != TSqlTokenType.WhiteSpace && token.TokenType != TSqlTokenType.As)
                {

                    // Determine if it is a table reference based on the dot.
                    if (isReference)
                    {
                        Into.identifier += token.Text;
                        if(!token.Text.Contains('#'))
                        {
                            Into.identifier += '.'; 
                        }
                        isReference = false;
                    }
                    else
                    {
                        Into.alias = token.Text;
                    }

                    if (token.TokenType == TSqlTokenType.Dot)
                    {
                        isReference = true;
                    }   
                }
            }

            // WHERE
            var newWhereToken = new WhereClause();
            var whereToken = expression.WhereClause;
            for (int i = whereToken.FirstTokenIndex; i <= whereToken.LastTokenIndex; i++)
            {

                var token = whereToken.ScriptTokenStream[i];
                if (token.TokenType.ToString() == "WhiteSpace")
                {
                    newWhereToken.value += " ";
                }
                else if (token.TokenType == TSqlTokenType.Where)
                {
                    newWhereToken.value += token.Text.ToUpper() + "\n\t";
                    i++;
                }
                else if (token.TokenType == TSqlTokenType.And || token.TokenType == TSqlTokenType.Or)
                {
                    newWhereToken.value += "\n\t" + token.Text.ToUpper() + "\n\t";
                }
                else if (token.IsKeyword())
                {
                    newWhereToken.value += token.Text.ToUpper();
                }

                else if (Tokens.Identifiers.Contains(token.Text.ToUpper()))
                {
                    newWhereToken.value += token.Text.ToUpper();
                }
                else
                {
                    newWhereToken.value += token.Text;
                }

            }   
            FormatTokens.Add(new FormatToken(CurrentCol,whereToken.LastTokenIndex,newWhereToken.value, "Where"));
            this.Where = newWhereToken;
            
            // WHERE
            var newOrderByToken = new OrderByClause();
            var orderByToken = expression.OrderByClause;
            for (int i = orderByToken.FirstTokenIndex; i <= orderByToken.LastTokenIndex; i++)
            {

                var token = orderByToken.ScriptTokenStream[i];
                if (token.TokenType.ToString() == "WhiteSpace")
                {
                    newOrderByToken.value += " ";
                }
                else if (token.TokenType == TSqlTokenType.Order)
                {
                    newOrderByToken.value += token.Text.ToUpper() + " ";
                    i++;
                }
                else if (token.TokenType == TSqlTokenType.By)
                {
                    newOrderByToken.value += token.Text.ToUpper() + "\n\t";
                    i++;
                }
                else if (token.TokenType == TSqlTokenType.And || token.TokenType == TSqlTokenType.Or)
                {
                    newOrderByToken.value += "\n\t" + token.Text.ToUpper() + "\n\t";
                }
                else if (token.IsKeyword())
                {
                    newOrderByToken.value += token.Text.ToUpper();
                }

                else if (Tokens.Identifiers.Contains(token.Text.ToUpper()))
                {
                    newOrderByToken.value += token.Text.ToUpper();
                }
                else
                {
                    newOrderByToken.value += token.Text;
                }

            }   
            this.OrderBy = newOrderByToken;
            Console.WriteLine(OrderBy.value);

            return FormatTokens;

        }

        private string RemoveAlias(string text, string alias)
        {

            string[] words = text.Split(' ');
            int wordsToRemove = 1;
            if (words[words.Count() - 2] == "AS")
            {
                wordsToRemove++;
            }

            string result = string.Join(" ", words, 0, words.Length - wordsToRemove);


            return result;
        }

        private void Print(List<FormatToken> formatTokenList)
        {
            foreach (var token in formatTokenList)
            {
            }
        }
        
        
        
    }

    public class DistinctClause
    {
        public string type {get; set;}
        public string identifier {get; set;}
        public int start {get; set;}
        public int end {get; set;}
        public string alias {get; set;}
    }

    public class TopClause
    {
        public string type {get; set;}
        public string identifier {get; set;}
        public int start {get; set;}
        public int end {get; set;}
        public string alias {get; set;}
    }

    public class Column
    {
        public string type {get; set;}
        public string value { get; set; } = "";
        public int start {get; set;}
        public int end {get; set;}
        public string alias {get; set;}

    }

    public class FromClause
    {
        public string type {get; set;}
        public string identifier {get; set;}
        public int start {get; set;}
        public int end {get; set;}
        public string alias {get; set;}
    }

    public class IntoClause
    {
        public string type {get; set;}
        public string identifier {get; set;}
        public int start {get; set;}
        public int end {get; set;}
        public string alias {get; set;}
    }

    public class WhereClause
    {
        public string type {get; set;}
        public string value {get; set;}
        public int start {get; set;}
        public int end {get; set;}
    }

    public class OrderByClause
    {
        public string type {get; set;}
        public string value {get; set;}
        public int start {get; set;}
        public int end {get; set;}
        public string alias {get; set;}
    }

    public class GroupByClause
    {
        public string type {get; set;}
        public string identifier {get; set;}
        public int start {get; set;}
        public int end {get; set;}
        public string alias {get; set;}
    }

    public class HavingClause
    {
        public string type {get; set;}
        public string identifier {get; set;}
        public int start {get; set;}
        public int end {get; set;}
        public string alias {get; set;}
    }

    

}

