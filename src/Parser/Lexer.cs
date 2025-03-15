using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSqlFormatter.Parser
{
    // Look into moving to a stream instead of string?
    public class Lexer
    {
        public bool IsKeyword(string str)
        {
            string[] keywords = [
                "select",
                "from",
                "into",
                "insert",
                "delete",
                "set",
                "drop",
                "table",
                "procedure",
                "view"
            ];
            if(keywords.Contains(str.ToLower()))
                return true;
            else
                return false;
        }

        public bool IsValidIdentifier(string str)
        {
            string[] identifiers = [
                "*"
            ];
            if (identifiers.Contains(str))
                return true;
            else
                return false;
        }

        // Checks for whitespace
        public bool IsDelimiter(char chr)
        {
            if(chr == ' ')
                return true;
            else
                return false;
        }

        public void Analysis(string input)
        {
            int len = input.Length;
            int left = 0;
            int right = 1;

            while(right < len && left <= right)
            {
                if(!IsDelimiter(input[right]))
                    right++;

                if(right == len)
                {
                    
                }

                if(IsDelimiter(input[right]) && left == right)
                {
                    right++;
                    left = right;
                }
                else if(IsDelimiter(input[right]) && left != right || right == len && left != right)
                {
                    string subString = input.Substring(left, right - left);

                    if(IsKeyword(subString))
                    {
                        Console.WriteLine($"Token Keyword, Value: {subString}");
                    }
                    else if(IsValidIdentifier(subString))
                    {
                        Console.WriteLine($"Token Valid Identifier, Value: {subString}");
                    }
                    else if(!IsValidIdentifier(subString) && !IsDelimiter(input[right]))
                    {
                        Console.WriteLine($"Token Unknown Identifier, Value: {subString}");
                    }
                        
                    left = right;
                }

            }

        }


        // public void nextToken()
        // {
        //     char nextChar = charReader.peek();
        //     switch (nextChar)
        //     {
        //         case "\"":
        //             return processStringLiteral();
        //         case "0":
        //         case "1":
        //         case "2":
        //         case "9":
        //             return processNumericLiteral();
        //     }
        // }

    }
}
