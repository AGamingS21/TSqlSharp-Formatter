using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSqlSharp.Formating;

// using TSqlSharp.Formating;
using TSqlSharp.Vistors;

namespace TSqlSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string select = @"../../../../scripts/Select.sql";
            string createProc = @"../../../../scripts/CreateProcedure.sql";
            string createTable = @"../../../../scripts/CreateTable.sql";
            string selectCte = @"../../../../scripts/SelectWithCte.sql";
            string delete = @"../../../../scripts/DeleteTable.sql";
            string filePath = select;
            var formatter = new Formatter();

            using (var streamReader = new StreamReader(filePath))
            {
                Console.WriteLine(formatter.Format(streamReader.ReadToEnd()));
            }

            

          
        }
    }
}
