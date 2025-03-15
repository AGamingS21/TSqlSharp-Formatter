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

        // Checks for delimiters like whitespace or semi colons
        public bool IsDelimiter(char chr)
        {
            char[] delimiters = [
                ' '
            ];

            if(delimiters.Contains(chr))
                return true;
            else
                return false;
        }

        public void Analysis(string input)
        {
            int len = input.Length;
            int left = 0;
            int right = 0;

            while(right < len && left <= right)
            {
                if(!IsDelimiter(input[right]))
                    right++;

                if(right != len && IsDelimiter(input[right]) && left == right)
                {
                    right++;
                    left = right;
                }
                else if(right == len && left != right || IsDelimiter(input[right]) && left != right)
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
                    else if(!IsValidIdentifier(subString))
                    {
                        Console.WriteLine($"Token Unknown Identifier, Value: {subString}");
                    }
                        
                    left = right;
                }

            }

        }
    }
}
