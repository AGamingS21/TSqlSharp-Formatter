using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSqlFormatter.Formatter
{
    public class Formatter
    {
        public void Format(TSqlFragment tsqlFragment)
        {
            var statments = GetStatements(tsqlFragment);

            int maincol = 1;

            foreach(var statment in statments)
            {   
                
            }

        }


        public List<TSqlStatement> GetStatements(TSqlFragment tsqlFragment)
        {
            List<TSqlStatement> statements = new List<TSqlStatement>();

            TSqlScript sqlScript = (TSqlScript)tsqlFragment;

            foreach(var batch in sqlScript.Batches)
            {
                foreach(var statment in batch.Statements)
                {
                    statements.Add(statment);
                }
            }

            return statements;
        }

    }
}