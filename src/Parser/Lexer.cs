using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSqlFormatter.Parser
{
    public class Lexer
    {

        public bool IsKeyword(string str)
        {
            if(Tokens.Keywords.Contains(str.ToUpper()))
                return true;
            else
                return false;
        }

        public bool IsValidIdentifier(string str)
        {
            
            if (Tokens.Identifiers.Contains(str))
                return true;
            else
                return false;
        }

        public bool IsOperator(char str)
        {
            
            if (Tokens.Operators.Contains(str))
                return true;
            else
                return false;
        }

        public bool IsInteger(string str)
        {
            bool isDigit = true;
            foreach(char chr in str)
            {
                if(!Char.IsDigit(chr))
                {
                    isDigit = false;
                    break;
                }
            }
            return isDigit;
        }

        // Checks for delimiters like whitespace or semi colons
        public bool IsDelimiter(char chr)
        {
            if(Tokens.Delimiters.Contains(chr))
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
                    if(IsOperator(input[right]))
                    {
                        Console.WriteLine($"Token Operator, Value: {input[right]}");
                    }
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
                    else if(IsInteger(subString))
                    {
                        Console.WriteLine($"Token Integer, Value: {subString}");
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
