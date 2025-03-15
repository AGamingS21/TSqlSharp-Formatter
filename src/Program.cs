using System;

using TSqlFormatter.Parser;

namespace TSqlFormatter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"./scripts/script1.sql";
            var text = File.ReadAllText(filePath);

            Lexer lexer = new Lexer();

            lexer.Analysis(text);
        }
    }
}
