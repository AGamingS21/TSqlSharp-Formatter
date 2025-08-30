using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;
namespace TSqlSharp.Vistors
{

    public class CustomSqlFormatter : TSqlFragmentVisitor
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private int _indentLevel = 0;
        private const int IndentSize = 4;
        private int LastSeenIndex = 0;
        private IList<TSqlParserToken> _commentList;
        private int StatementStartIndex = 0;
        private bool UnknownNode;
        private List<TSqlFragment> UnkownNodes = new List<TSqlFragment>();
        private Stack<TSqlFragment> recentFragments = new Stack<TSqlFragment>();
        private bool IsParentNode = false;
        public CustomSqlFormatter(IList<TSqlParserToken> commentList)
        {
            _commentList = commentList;
        }
        private string GetIdentifiers(IList<Identifier> identifiers)
        {
            var formatted = "";
            for (int i = 0; i < identifiers.Count; i++)
            {
                formatted += identifiers[i].Value;
                if (i != identifiers.Count - 1)
                {
                    formatted += ".";
                }
            }
            return formatted;
        }
        private void AppendLine(string text = "")
        {
            _sb.AppendLine(new string(' ', _indentLevel * IndentSize) + text);
        }
        private void Append(string text = "")
        {
            _sb.Append(new string(' ', _indentLevel * IndentSize) + text);
        }

        public void PrintRestOfComments()
        {
            foreach (var comment in _commentList)
            {
                _sb.AppendLine();
                Append(comment.Text);
            }
        }

        private void InjectLeadingCommentStatement(IList<TSqlParserToken> scriptStream, int endIndex, bool newLine = false)
        {

            // when peaking to get comments and the index goes over just set it back.
            if (LastSeenIndex > scriptStream.Count)
            {
                LastSeenIndex = scriptStream.Count;
            }

            if (endIndex > scriptStream.Count)
            {
                endIndex = scriptStream.Count;
            }


            for (int i = LastSeenIndex; i < endIndex - 1; i++)
            {
                if (scriptStream[i].TokenType == TSqlTokenType.MultilineComment || scriptStream[i].TokenType == TSqlTokenType.SingleLineComment)
                {
                    if (_commentList.Contains(scriptStream[i]))
                    {
                        if (newLine)
                        {
                            AppendLine(scriptStream[i].Text);
                        }
                        else
                        {
                            Append(scriptStream[i].Text);
                        }

                        _commentList.Remove(scriptStream[i]);
                    }

                }
            }
            LastSeenIndex = endIndex;
        }


        public string GetFormattedSql() => _sb.ToString();

        // This will be used to edit nodes that do not have an overridden explicit visit.
        public void FormatAst(TSqlFragment node)
        {
            node.Accept(this);
        }
        public override void ExplicitVisit(TSqlScript node)
        {
            foreach (var batch in node.Batches)
            {
                foreach (var statement in batch.Statements)
                {
                    UnknownNode = false;
                    IsParentNode = true;
                    statement.Accept(this);

                    if (UnknownNode)
                    {
                        AppendUnknownNode(statement);
                    }
                    StatementStartIndex = _sb.Length;
                }
            }
        }
        private void AppendUnknownNode(TSqlFragment node)
        {
            _sb.Remove(StatementStartIndex, _sb.Length - StatementStartIndex);
            _sb.AppendLine();

            var unkownDelimiter = "-------------------------------------------------";
            var unkownString = $"{unkownDelimiter}\n-- Unable to format for {node.GetType().Name} statement starting at char {_sb.Length}\n";

            foreach (var unknownNode in UnkownNodes)
            {
                unkownString += $"-- Can't format: {unknownNode.GetType().Name}\n";
            }
            unkownString += $"{unkownDelimiter}\n";
            _sb.Append(unkownString);
            UnkownNodes = new List<TSqlFragment>();

            // reprint out statement
            for (int i = node.FirstTokenIndex; i <= node.LastTokenIndex; i++)
            {
                Append(node.ScriptTokenStream[i].Text);
            }


        }
        public override void Visit(TSqlFragment node)
        {
            UnknownNode = true;
            UnkownNodes.Add(node);
        }
        public override void ExplicitVisit(SelectStatement node)
        {
            _sb.AppendLine();

            if (node.WithCtesAndXmlNamespaces != null)
            {
                node.WithCtesAndXmlNamespaces.Accept(this);
                _sb.AppendLine();
            }

            InjectLeadingCommentStatement(node.ScriptTokenStream, node.FirstTokenIndex, true);

            Append("SELECT");
            _sb.AppendLine();
            if (node.QueryExpression is QuerySpecification querySpec)
            {
                FormatQuerySpecification(querySpec, node.Into);
            }
        }
        public void FormatQuerySpecification(QuerySpecification node, SchemaObjectName schemaObjectName = null)
        {
            _indentLevel++;
            for (int i = 0; i < node.SelectElements.Count; i++)
            {
                if (i != node.SelectElements.Count - 1)
                {
                    InjectLeadingCommentStatement(node.ScriptTokenStream, node.SelectElements[i].FirstTokenIndex, true);
                }

                _sb.Append(new string(' ', _indentLevel * IndentSize));
                node.SelectElements[i].Accept(this);

                if (i != node.SelectElements.Count - 1)
                {
                    _sb.Append(",");
                    // determine if there is a comment between this select element and the next.
                    InjectLeadingCommentStatement(node.ScriptTokenStream, node.SelectElements[i + 1].FirstTokenIndex);
                    _sb.AppendLine();
                }

            }
            // Injct the last comment line if it exists. Add 1 to get past the select keyword
            int lastTokenIndex = node.LastTokenIndex;
            for (int i = LastSeenIndex + 1; i < node.LastTokenIndex; i++)
            {
                if (node.ScriptTokenStream[i].IsKeyword())
                {
                    lastTokenIndex = i;
                    i = node.LastTokenIndex;
                }
            }
            InjectLeadingCommentStatement(node.ScriptTokenStream, lastTokenIndex);
            _indentLevel--;
            if (schemaObjectName != null)
            {
                _sb.AppendLine();
                AppendLine($"INTO");
                _indentLevel++;
                Append(GetIdentifiers(schemaObjectName.Identifiers));
                _indentLevel--;
            }

            if (node.FromClause != null)
            {

                node.FromClause.Accept(this);
                // Injct the last comment line if it exists. Add 1 to get past the select keyword
            }

            if (node.WhereClause != null)
            {

                node.WhereClause.Accept(this);

            }
            if (node.OrderByClause != null)
            {
                node.OrderByClause.Accept(this);
            }

            // group by
            if (node.GroupByClause != null)
            {
                node.GroupByClause.Accept(this);
            }

            // having
            if (node.HavingClause != null)
            {
                _sb.AppendLine();
                InjectLeadingCommentStatement(node.ScriptTokenStream, node.LastTokenIndex);
                AppendLine("HAVING");
                _indentLevel++;
                Append();
                node.HavingClause.SearchCondition.Accept(this);
                _indentLevel--;
            }


        }
        public override void ExplicitVisit(CreateTableStatement node)
        {

            Append($"CREATE TABLE {GetIdentifiers(node.SchemaObjectName.Identifiers)}");
            _sb.AppendLine();
            Append("(");
            _sb.AppendLine();
            _indentLevel++;
            node.Definition.Accept(this);
            _indentLevel--;
            _sb.AppendLine();
            Append(")");
            _sb.AppendLine();
        }
        public override void ExplicitVisit(WithCtesAndXmlNamespaces node)
        {
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.FirstTokenIndex);
            if (node.CommonTableExpressions != null)
            {
                for (int i = 0; i < node.CommonTableExpressions.Count; i++)
                {
                    var cte = node.CommonTableExpressions[i];
                    AppendLine($";WITH {cte.ExpressionName.Value} AS (");
                    _indentLevel++;
                    node.CommonTableExpressions[i].QueryExpression.Accept(this);
                    InjectLeadingCommentStatement(node.ScriptTokenStream, node.LastTokenIndex, true);
                    _indentLevel--;
                    _sb.AppendLine();
                    Append($")");

                }


            }
        }
        public override void ExplicitVisit(BinaryQueryExpression node)
        {
            if (node.FirstQueryExpression is QuerySpecification querySpec)
            {
                InjectLeadingCommentStatement(node.ScriptTokenStream, node.FirstTokenIndex);
                Append("SELECT");
                _sb.AppendLine();
                FormatQuerySpecification(querySpec);

            }
            _sb.AppendLine();
            Append(FormatBinaryQueryExpressionType(node.BinaryQueryExpressionType));
            if (node.All)
            {
                _sb.Append(" ALL");
                _sb.AppendLine();
            }

            if (node.SecondQueryExpression is QuerySpecification querySpec2)
            {
                InjectLeadingCommentStatement(node.ScriptTokenStream, querySpec2.FirstTokenIndex);
                _sb.AppendLine();
                Append("SELECT");
                _sb.AppendLine();
                FormatQuerySpecification(querySpec2);

            }

        }
        public override void ExplicitVisit(DropTableStatement node)
        {
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.FirstTokenIndex, true);

            var formatted = "DROP TABLE ";
            formatted += "IF EXISTS ";

            foreach (var objectItem in node.Objects)
            {
                formatted += GetIdentifiers(objectItem.Identifiers);
            }
            Append(formatted);
        }
        public override void ExplicitVisit(TableDefinition node)
        {

            for (int i = 0; i < node.ColumnDefinitions.Count; i++)
            {
                node.ColumnDefinitions[i].Accept(this);
                if (i != node.ColumnDefinitions.Count - 1)
                {
                    _sb.AppendLine(",");
                }
            }
        }
        public override void ExplicitVisit(UpdateStatement node)
        {
            AppendLine();
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.FirstTokenIndex, true);
            Append("UPDATE");
            _sb.AppendLine();
            _indentLevel++;
            node.UpdateSpecification.Target.Accept(this);
            _indentLevel--;
            _sb.AppendLine();
            Append("SET");
            _indentLevel++;
            _sb.AppendLine();
            for (int i = 0; i < node.UpdateSpecification.SetClauses.Count; i++)
            {
                node.UpdateSpecification.SetClauses[i].Accept(this);
            }
            _indentLevel--;

            node.UpdateSpecification.WhereClause.Accept(this);

        }
        public override void ExplicitVisit(IfStatement node)
        {

            // AppendLine(node.SchemaObject.BaseIdentifier.Value);
        }
        public override void ExplicitVisit(UseStatement node)
        {

            // AppendLine(node.SchemaObject.BaseIdentifier.Value);
        }
        public override void ExplicitVisit(CopyStatement node)
        {

            // AppendLine(node.SchemaObject.BaseIdentifier.Value);
        }
        public override void ExplicitVisit(DbccStatement node)
        {

            // AppendLine(node.SchemaObject.BaseIdentifier.Value);
        }
        public override void ExplicitVisit(WhileStatement node)
        {

            // AppendLine(node.SchemaObject.BaseIdentifier.Value);
        }
        public override void ExplicitVisit(InsertStatement node)
        {

            Append($"INSERT INTO ");
            node.InsertSpecification.Target.Accept(this);
            _sb.AppendLine();

            if (node.InsertSpecification.Columns != null)
            {
                Append($"(");
                _sb.AppendLine();
                _indentLevel++;
                var insertSpec = node.InsertSpecification;
                for (int i = 0; i < insertSpec.Columns.Count; i++)
                {
                    _sb.Append(new string(' ', _indentLevel * IndentSize));
                    insertSpec.Columns[i].Accept(this);

                    if (i != insertSpec.Columns.Count - 1)
                    {
                        _sb.Append(",");
                    }
                    _sb.AppendLine();
                }
                _indentLevel--;
                Append($")");
                _sb.AppendLine();
            }
            node.InsertSpecification.InsertSource.Accept(this);
        }
        public override void ExplicitVisit(SelectInsertSource node)
        {
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.FirstTokenIndex, true);
            AppendLine("SELECT");

            if (node.Select is QuerySpecification querySpec)
            {
                FormatQuerySpecification(querySpec);
            }
            else
            {
                node.Select.Accept(this);
            }

        }
        public override void ExplicitVisit(AlterProcedureStatement node)
        {

            Append("ALTER PROCEDURE ");
            Append(GetIdentifiers(node.ProcedureReference.Name.Identifiers));

            // Parameters

            _indentLevel++;
            // statements
            foreach (var statement in node.StatementList.Statements)
            {
                statement.Accept(this);
            }
            _indentLevel--;
        }
        public override void ExplicitVisit(CreateOrAlterProcedureStatement node)
        {

            Append("CREATE OR ALTER PROCEDURE ");
            Append(GetIdentifiers(node.ProcedureReference.Name.Identifiers));

            // Parameters

            _indentLevel++;
            // statements
            foreach (var statement in node.StatementList.Statements)
            {
                statement.Accept(this);
            }
            _indentLevel--;
        }
        public override void ExplicitVisit(CreateProcedureStatement node)
        {
            // Mark the last token index for tracking
            Append("CREATE PROCEDURE ");
            _sb.Append(GetIdentifiers(node.ProcedureReference.Name.Identifiers));

            // Parameters
            _indentLevel++;
            // statements
            foreach (var statement in node.StatementList.Statements)
            {
                statement.Accept(this);
            }
            _indentLevel--;
        }

        public override void ExplicitVisit(BeginEndBlockStatement node)
        {
            // Mark the last token index for tracking
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.FirstTokenIndex);
            for (int i = LastSeenIndex; i < node.LastTokenIndex; i++)
            {
                if (node.ScriptTokenStream[i].TokenType == TSqlTokenType.Begin)
                {
                    LastSeenIndex = i;
                    i = node.LastTokenIndex;
                }
            }

            _indentLevel--;
            _sb.AppendLine();
            AppendLine("AS");
            AppendLine("BEGIN");
            _indentLevel++;

            foreach (var statement in node.StatementList.Statements)
            {
                statement.Accept(this);
            }

            // InjectLeadingCommentStatement(node.ScriptTokenStream, node.LastTokenIndex);
            _indentLevel--;
            _sb.AppendLine();
            Append("END");
            _indentLevel++;
        }



        public override void ExplicitVisit(ColumnReferenceExpression node)
        {

            _sb.Append(GetIdentifiers(node.MultiPartIdentifier.Identifiers));
        }
        public override void ExplicitVisit(ColumnDefinition node)
        {

            Append($"{node.ColumnIdentifier.Value} ");
            node.DataType.Accept(this);
            foreach (var constraint in node.Constraints)
            {
                constraint.Accept(this);
            }
        }
        public override void ExplicitVisit(SqlDataTypeReference node)
        {

            _sb.Append(GetIdentifiers(node.Name.Identifiers).ToUpper());

            foreach (var parameter in node.Parameters)
            {
                _sb.Append($"({parameter.Value.ToUpper()})");
            }
        }


        public override void ExplicitVisit(WhereClause node)
        {
            _sb.AppendLine();
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.FirstTokenIndex, true);
            AppendLine("WHERE");
            _indentLevel++;
            _sb.Append(new string(' ', _indentLevel * IndentSize));
            node.SearchCondition.Accept(this);
            _indentLevel--;
        }
        public override void ExplicitVisit(OrderByClause node)
        {
            _sb.AppendLine();
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.FirstTokenIndex, true);
            AppendLine("ORDER BY");
            _indentLevel++;
            for (int i = 0; i < node.OrderByElements.Count; i++)
            {
                _sb.Append(new string(' ', _indentLevel * IndentSize));
                node.OrderByElements[i].Accept(this);
            }
            _indentLevel--;
        }
        public override void ExplicitVisit(GroupByClause node)
        {
            _sb.AppendLine();
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.FirstTokenIndex);
            AppendLine("GROUP BY");
            _indentLevel++;
            for (int i = 0; i < node.GroupingSpecifications.Count; i++)
            {
                if (i != node.GroupingSpecifications.Count - 1)
                {
                    InjectLeadingCommentStatement(node.ScriptTokenStream, node.GroupingSpecifications[i].FirstTokenIndex, true);
                }

                _sb.Append(new string(' ', _indentLevel * IndentSize));
                node.GroupingSpecifications[i].Accept(this);

                if (i != node.GroupingSpecifications.Count - 1)
                {
                    _sb.Append(",");
                    // determine if there is a comment between this select element and the next.
                    InjectLeadingCommentStatement(node.ScriptTokenStream, node.GroupingSpecifications[i + 1].FirstTokenIndex);
                    _sb.AppendLine();
                }
            }

            // Injct the last comment line if it exists. Add 1 to get past the select keyword check next 5 chars for a comment
            int lastTokenIndex = node.LastTokenIndex + 5;
            for (int i = LastSeenIndex + 1; i < node.LastTokenIndex; i++)
            {
                if (node.ScriptTokenStream[i].IsKeyword())
                {
                    lastTokenIndex = i;
                    i = node.LastTokenIndex;
                }
            }
            InjectLeadingCommentStatement(node.ScriptTokenStream, lastTokenIndex);

            _indentLevel--;
        }
        public override void ExplicitVisit(ExpressionGroupingSpecification node)
        {
            node.Expression.Accept(this);
        }
        public override void ExplicitVisit(FromClause node)
        {
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.FirstTokenIndex);
            _sb.AppendLine();
            AppendLine("FROM");
            _indentLevel++;
            foreach (var tableReference in node.TableReferences)
            {
                tableReference.Accept(this);
            }
            _indentLevel--;
            // Injct the last comment line if it exists. Add 1 to get past the select keyword. Check next 5 tokens
            int lastTokenIndex = node.LastTokenIndex + 5;
            for (int i = LastSeenIndex + 1; i < node.LastTokenIndex; i++)
            {
                if (node.ScriptTokenStream[i].IsKeyword())
                {
                    lastTokenIndex = i;
                    i = node.LastTokenIndex;
                }
            }
            InjectLeadingCommentStatement(node.ScriptTokenStream, lastTokenIndex);
        }
        // public override void ExplicitVisit(SetClause node)
        // {
        //     AssignmentSetClause
        // }
        public override void ExplicitVisit(AssignmentSetClause node)
        {

            Append($"{GetIdentifiers(node.Column.MultiPartIdentifier.Identifiers)} = ");
            node.NewValue.Accept(this);
        }
        public override void ExplicitVisit(SelectScalarExpression node)
        {

            node.Expression.Accept(this);
            if (node.ColumnName != null)
            {

                if (node.ColumnName.Identifier != null)
                {
                    _sb.Append(" AS " + FormatBinaryQuoteType(node.ColumnName.Identifier.QuoteType, node.ColumnName.Identifier.Value));
                }
                else
                {
                    _sb.Append(" AS " + node.ColumnName.Value);
                }

            }
        }
        public override void ExplicitVisit(ScalarExpression node)
        {

            node.Accept(this);
        }
        public override void ExplicitVisit(SelectStarExpression node)
        {

            _sb.Append("*");
        }
        public override void ExplicitVisit(CastCall node)
        {

            _sb.Append("CAST(");
            node.Parameter.Accept(this);

            _sb.Append($" AS {node.DataType.Name.BaseIdentifier.Value.ToUpper()})");
        }


        public override void ExplicitVisit(BooleanComparisonExpression node)
        {
            node.FirstExpression.Accept(this);
            _sb.Append(" " + FormatBooleanComparisonType(node.ComparisonType) + " ");
            node.SecondExpression.Accept(this);
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.LastTokenIndex);
        }
        public override void ExplicitVisit(BooleanIsNullExpression node)
        {
            node.Expression.Accept(this);
            var isNotNull = "";
            if (node.IsNot)
            {
                isNotNull = " NOT";
            }
            _sb.Append($" IS{isNotNull} NULL");
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.LastTokenIndex);
        }

        public override void ExplicitVisit(StringLiteral node)
        {

            _sb.Append("'" + node.Value + "'");
        }

        public override void ExplicitVisit(IntegerLiteral node)
        {

            _sb.Append(node.Value);
        }

        public override void ExplicitVisit(FunctionCall node)
        {

            // _sb.Append(new string(' ', _indentLevel * IndentSize));
            // // node.SearchCondition.Accept(this);
            // _sb.AppendLine();
            _sb.Append($"{node.FunctionName.Value.ToUpper()}(");

            for (int i = 0; i < node.Parameters.Count; i++)
            {
                node.Parameters[i].Accept(this);

                if (i != node.Parameters.Count - 1)
                {
                    _sb.Append(", ");
                }

            }
            _sb.Append(")");
        }
        public override void ExplicitVisit(BinaryExpression node)
        {
            // First Expression
            node.FirstExpression.Accept(this);


            // Binary Type Expression
            _sb.Append(FormatBinaryOperators(node.BinaryExpressionType));

            // Second Expression
            node.SecondExpression.Accept(this);
        }
        public override void ExplicitVisit(BooleanExpression node)
        {

            node.Accept(this);
        }
        public override void ExplicitVisit(BooleanBinaryExpression node)
        {

            node.FirstExpression.Accept(this);
            //_sb.AppendLine();
            _sb.AppendLine();
            Append(FormatBooleanBinaryExpressionType(node.BinaryExpressionType));
            node.SecondExpression.Accept(this);
        }
        public override void ExplicitVisit(QualifiedJoin node)
        {
            node.FirstTableReference.Accept(this);
            _indentLevel--;
            _sb.AppendLine();

            Append(FormatJoinTypes(node.QualifiedJoinType));
            node.SecondTableReference.Accept(this);
            InjectLeadingCommentStatement(node.ScriptTokenStream, node.SearchCondition.FirstTokenIndex);
            _indentLevel++;
            _sb.AppendLine();
            Append("ON ");
            node.SearchCondition.Accept(this);

        }
        public override void ExplicitVisit(NamedTableReference node)
        {

            InjectLeadingCommentStatement(node.ScriptTokenStream, node.LastTokenIndex);
            Append(GetIdentifiers(node.SchemaObject.Identifiers));
            if (node.Alias != null)
            {
                _sb.Append($" AS {FormatBinaryQuoteType(node.Alias.QuoteType, node.Alias.Value)}");
            }
            foreach (var tableHint in node.TableHints)
            {
                _sb.Append($" WITH ({tableHint.HintKind.ToString().ToUpper()})");
            }
        }
        public override void ExplicitVisit(NullableConstraintDefinition node)
        {

            if (!node.Nullable)
            {
                _sb.Append(" NOT NULL");
            }
            else
            {
                _sb.Append(" NULL");
            }
        }
        public override void ExplicitVisit(ExpressionWithSortOrder node)
        {
            node.Expression.Accept(this);
            _sb.Append(FormatSortOrder(node.SortOrder));
        }
        private string FormatJoinTypes(QualifiedJoinType joinType)
        {
            var join = "JOIN ";
            switch (joinType)
            {
                case QualifiedJoinType.LeftOuter:
                    return $"LEFT {join}";
                case QualifiedJoinType.RightOuter:
                    return $"RIGHT {join}";
                case QualifiedJoinType.Inner:
                    return $"INNER {join}";
                case QualifiedJoinType.FullOuter:
                    return $"FULL {join}";
                default:
                    throw new Exception($"Unknown Join Type {joinType}");
            }

        }
        private string FormatBooleanComparisonType(BooleanComparisonType booleanComparisonType)
        {
            switch (booleanComparisonType)
            {
                case BooleanComparisonType.Equals:
                    return "=";
                case BooleanComparisonType.GreaterThan:
                    return ">";
                case BooleanComparisonType.LessThan:
                    return "<";
                case BooleanComparisonType.LessThanOrEqualTo:
                    return "<=";
                case BooleanComparisonType.GreaterThanOrEqualTo:
                    return ">=";
                case BooleanComparisonType.NotEqualToBrackets:
                    return "<>";
                case BooleanComparisonType.NotEqualToExclamation:
                    return "<!";
                default:
                    throw new Exception($"Unidentified BooleanComparisonType: {booleanComparisonType}");

            }

        }
        private string FormatBooleanBinaryExpressionType(BooleanBinaryExpressionType expressionType)
        {
            switch (expressionType)
            {
                case BooleanBinaryExpressionType.And:
                    return "AND ";
                case BooleanBinaryExpressionType.Or:
                    return "OR ";
                default:
                    throw new Exception($"Unidentified BinaryExpressionType: {expressionType}");
            }
            ;
        }
        private string FormatBinaryOperators(BinaryExpressionType expressionType)
        {
            switch (expressionType)
            {
                case BinaryExpressionType.Add:
                    return " + ";
                case BinaryExpressionType.Subtract:
                    return " - ";
                case BinaryExpressionType.Multiply:
                    return " * ";
                case BinaryExpressionType.Divide:
                    return " / ";
                case BinaryExpressionType.Modulo:
                    return " % ";
                default:
                    throw new Exception($"Unidentified BinaryExpressionType: {expressionType}");
            }
            ;
        }

        private string FormatBinaryQueryExpressionType(BinaryQueryExpressionType expressionType)
        {
            switch (expressionType)
            {
                case BinaryQueryExpressionType.Union:
                    return "UNION";
                case BinaryQueryExpressionType.Intersect:
                    return "INTERSECT";
                case BinaryQueryExpressionType.Except:
                    return "EXCEPT";
                default:
                    throw new Exception($"Unidentified BinaryExpressionType: {expressionType}");
            }
            ;
        }
        private string FormatBinaryQuoteType(QuoteType type, string value)
        {
            switch (type)
            {
                case QuoteType.DoubleQuote:
                    return $"'{value}'";
                case QuoteType.SquareBracket:
                    return $"[{value}]";
                case QuoteType.NotQuoted:
                    return value;
                default:
                    throw new Exception($"Unidentified BinaryExpressionType: {type}");
            }
            ;
        }
        private string FormatSortOrder(SortOrder type)
        {
            switch (type)
            {
                case SortOrder.Ascending:
                    return " ASC";
                case SortOrder.Descending:
                    return " DESC";
                case SortOrder.NotSpecified:
                    return "";
                default:
                    throw new Exception($"Unidentified BinaryExpressionType: {type}");
            };
        }

        
        
    }
}