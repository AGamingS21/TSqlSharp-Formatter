using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public bool IsOperator(string str)
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

        public List<Token> Analysis(string input)
        {
            int len = input.Length;
            int left = 0;
            int right = 0;

            List<Token> tokens = new List<Token>();

            while(right < len && left <= right)
            {
                if(!IsDelimiter(input[right]))
                    right++;

                if(right != len && IsDelimiter(input[right]) && left == right)
                {
                    // if(IsOperator(input[right]))
                    // {
                    //     tokens.Add( new Token(TokenType.Operator, input[right].ToString(), left, right) );
                    //     //Console.WriteLine($"Token Operator, Value: {input[right]}");
                    // }
                    right++;
                    left = right;
                }
                else if(right == len && left != right || IsDelimiter(input[right]) && left != right)
                {
                    string subString = input.Substring(left, right - left);

                    if(IsKeyword(subString))
                    {
                        tokens.Add( new Token(TokenType.Keyword, subString, left, right) );
                        //Console.WriteLine($"Token Keyword, Value: {subString}");
                    }
                    else if(IsInteger(subString))
                    {
                        tokens.Add( new Token(TokenType.Integer, subString, left, right) );
                        //Console.WriteLine($"Token Integer, Value: {subString}");
                    }
                    else if(IsValidIdentifier(subString))
                    {
                        tokens.Add( new Token(TokenType.Identifier, subString, left, right) );
                        //Console.WriteLine($"Token Valid Identifier, Value: {subString}");
                    }
                    else if(!IsValidIdentifier(subString))
                    {
                        tokens.Add( new Token(TokenType.UnknownIdentifier, subString, left, right) );
                        //Console.WriteLine($"Token Unknown Identifier, Value: {subString}");
                    }
                        
                    left = right;
                }

            }

            return tokens;

        }
    }
}
