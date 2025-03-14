using System;

using TSqlFormatter.Parser;

namespace TSqlFormatter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer();

            lexer.nextToken();
        }
    }
}
