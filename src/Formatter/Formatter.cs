using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSqlSharp.Parser;

namespace TSqlSharp.Formating
{
    public class Formatter
    {


        public string Format(TSqlFragment tsqlFragment)
        {
            var statements = GetStatements(tsqlFragment);
            var indentSpace = 2;
            var indentLevel = new Stack<int>();

            indentLevel.Push(0);
            string formatted = "";
            foreach (var statement in statements)
            {
                bool newLine = false;
                bool space = false;
                bool inBrackets = false;

                bool IsConditional = false;

                var nested = new Stack<int>();

                for (int i = statement.FirstTokenIndex; i <= statement.LastTokenIndex; i++)
                {

                    var token = statement.ScriptTokenStream[i];

                    // if its a keyword then do this. 
                    if (Tokens.Keywords.Contains(token.Text.ToUpper()) && token.TokenType != TSqlTokenType.As && token.TokenType != TSqlTokenType.Or && token.TokenType != TSqlTokenType.And)
                    {
                        if (indentLevel.Peek() != 0)
                        {
                            indentLevel.Pop();
                        }

                        if (IsConditional)
                        {
                            indentLevel.Pop();
                            IsConditional = false;
                        }

                        formatted += token.Text.ToUpper();
                        // if this item or the next item is not a keyword then go to next line
                        if (!statement.ScriptTokenStream[i + 1].IsKeyword() && !statement.ScriptTokenStream[i + 2].IsKeyword())
                        {
                            newLine = true;
                        }
                        else
                        {
                            space = true;
                        }

                        indentLevel.Push(indentSpace);
                    }
                    else if (token.TokenType == TSqlTokenType.As)
                    {
                        formatted += token.Text.ToUpper();
                        space = true;
                    }
                    else if (token.TokenType == TSqlTokenType.And || token.TokenType == TSqlTokenType.Or)
                    {
                        formatted += "\n";
                        formatted += "".PadLeft(indentLevel.Peek());
                        formatted += token.Text.ToUpper();
                        newLine = true;
                        IsConditional = true;
                        //indentLevel.Push(indentSpace);

                    }
                    else if (token.TokenType == TSqlTokenType.WhiteSpace)
                    {
                    }
                    else
                    {


                        if (Tokens.Identifiers.Contains(token.Text.ToUpper()))
                        {
                            formatted += token.Text.ToUpper();
                        }
                        else if (TSqlTokenType.LeftParenthesis == token.TokenType)
                        {
                            inBrackets = true;
                            formatted += token.Text;
                            nested.Push(1);
                        }
                        else if (TSqlTokenType.RightParenthesis == token.TokenType)
                        {
                            nested.Pop();
                            formatted += token.Text;
                            if (nested.Count == 0)
                            {
                                inBrackets = false;
                            }

                        }
                        else
                        {
                            formatted += token.Text;
                            if (!SearchNextTwoIndexes(i, statement.LastTokenIndex, statement.ScriptTokenStream, TSqlTokenType.Comma))
                            {
                                space = true;
                            }


                            if (token.TokenType == TSqlTokenType.Comma)
                            {
                                newLine = true;
                            }
                        }

                    }
                    if (inBrackets)
                    {
                        newLine = false;
                    }

                    

                    if (statement.ScriptTokenStream[i + 1].TokenType == TSqlTokenType.Into || statement.ScriptTokenStream[i + 1].TokenType == TSqlTokenType.From || statement.ScriptTokenStream[i + 1].TokenType == TSqlTokenType.Where || statement.ScriptTokenStream[i + 1].TokenType == TSqlTokenType.Order)
                    {
                        newLine = true;
                        indentLevel.Pop();
                    }


                    if (newLine)
                    {
                        formatted += "\n";
                        formatted += "".PadLeft(indentLevel.Peek());
                        newLine = false;
                    }

                    if (space)
                    {
                        formatted += " ";
                        space = false;
                    }

                    

                }
                formatted += "\n";
                // indentLevel.Pop();
                // formatted += "".PadLeft(indentLevel.Peek());

            }

            return formatted;
        }
        
       

        private bool SearchNextTwoIndexes(int i, int LastTokenIndex, IList<TSqlParserToken> ScriptTokenStream, TSqlTokenType tokenType)
        {
            bool exists = false;
            if (i + 2 <= LastTokenIndex)
            {

                if (i + 1 <= LastTokenIndex && ScriptTokenStream[i + 1].TokenType == tokenType)
                {
                    exists = true;
                }
                else if (i + 2 <= LastTokenIndex && ScriptTokenStream[i + 2].TokenType == tokenType)
                {
                    exists = true;
                }
                
            }
            return exists;
        }

        private string NonLineBreaking()
        {
            // TO DO: when specific statements like AS will nt require a line break. 
            return "";
        }
        


        

        public List<TSqlStatement> GetStatements(TSqlFragment tsqlFragment)
        {
            List<TSqlStatement> statements = new List<TSqlStatement>();

            TSqlScript sqlScript = (TSqlScript)tsqlFragment;

            foreach(var batch in sqlScript.Batches)
            {
                foreach(var statement in batch.Statements)
                {
                    statements.Add(statement);
                }
            }

            return statements;
        }

        
    }

    public class FormatToken
    {
        public int Start {get; set;}
        public int End {get; set;}
        public string? Text {get; set;}
        public string? Type {get;set;}

        public FormatToken(int start, int end, string text, string type)
        {
            Start = start;
            End = end;
            Text = text;
            Type = type;
        }
    }


    
}