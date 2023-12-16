using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T_SqlFormatter.Parser
{
    public class Lexer
    {

        public void nextToken()
        {
            char nextChar = charReader.peek();
            switch (nextChar)
            {
                case "\"":
                    return processStringLiteral();
                case "0":
                case "1":
                case "2":
                    ...
            case "9":
                    return processNumericLiteral();
            }
        }

    }
}
