using System;
using System.Data;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSqlFormatter.Parser;

namespace TSqlFormatter.Formatter
{
    public class Formatter
    {
        private int CurrentCol {get; set;} = 1;

        public void Format(TSqlFragment tsqlFragment)
        {
            var statements = GetStatements(tsqlFragment);

            var formatTokenList = new List<FormatToken>();

            foreach(var statement in statements)
            {   
                switch(statement.GetType().Name)
                    {
                        case "SelectStatement":
                        var test = new TSqlFormatter.Parser.SelectStmnt((SelectStatement)statement);
                            // formatTokenList.AddRange(FormatSelect((SelectStatement)statement));
                            break;
                        case "DropTableStatement":
                            formatTokenList.AddRange(FormatSignleLine(statement));
                            break;
                    }

            }

            // Print(formatTokenList);

        }

        private List<FormatToken> FormatSignleLine(TSqlStatement statement)
        {
            var formatToken = new List<FormatToken>();

            

            
            var tokenText = "";
            for(int i = statement.FirstTokenIndex; i <= statement.LastTokenIndex; i++)
            {
                
                var columnToken = statement.ScriptTokenStream[i];
                if(columnToken.IsKeyword())
                {
                    tokenText += columnToken.Text.ToUpper();    
                }
                else
                {
                    tokenText += columnToken.Text;
                }
                
            }   
            formatToken.Add(new FormatToken(CurrentCol,statement.LastTokenIndex,tokenText, "Column"));  
        

            return formatToken;
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