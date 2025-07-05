using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSqlFormatter.Formatter;
using TSqlFormatter.Parser;

namespace TSqlFormatter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"../../../../tests/scripts/Select.sql";

            using (var streamReader = new StreamReader(filePath))
            {
                TSql150Parser parser = new TSql150Parser(false);
                IList<ParseError> errors;
                var tree = parser.Parse(streamReader, out errors);

                if (errors.Count > 0)
                {
                    Console.WriteLine(errors.Count + " Errors occured Exiting now");
                    Environment.Exit(1);
                }

                var formatter = new Formatter.Formatter();

                Console.WriteLine(formatter.Format(tree));



            }



            // var text = File.ReadAllText(filePath);

            // Lexer lexer = new Lexer();

            // var tokens = lexer.Analysis(text);
        }
    }
}
