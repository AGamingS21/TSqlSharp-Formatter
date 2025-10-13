using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSqlSharp.Vistors;

namespace TSqlSharp.Formating
{
    public class Formatter
    {

        public string Format(string input)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            using (var streamReader = new StreamReader(stream))
            {
                TSql150Parser parser = new TSql150Parser(false);
                IList<ParseError> errors;
                TSqlFragment tree = parser.Parse(streamReader, out errors);

                if (errors.Count > 0)
                {
                    return errors[0].Message;
                }


                // formatVisitor.CreateCommentList(tree.ScriptTokenStream);
                var commentList = tree.ScriptTokenStream.Where(t => t.TokenType == TSqlTokenType.SingleLineComment ||
                        t.TokenType == TSqlTokenType.MultilineComment).ToList();

                CustomSqlFormatter formatVisitor = new CustomSqlFormatter(commentList);
                formatVisitor.FormatAst(tree);
                formatVisitor.PrintRestOfComments();
                return formatVisitor.GetFormattedSql();
            }
        }


    }
}