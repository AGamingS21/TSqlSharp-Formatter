using TSqlSharp.Formating;
using Microsoft.SqlServer.TransactSql.ScriptDom;


namespace TSqlSharp.Tests;

public class FormatterTests
{
    [Fact]
    public void Format()
    {
        // Arrange
        string output = "";
        string expectedOutput = "";

        string expectedFilePath = @"../../../../../tests/scripts/SelectExpected.sql";


        string filePath = @"../../../../../tests/scripts/Select.sql";


        // Act
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

            var formatter = new Formatter();

            output = formatter.Format(tree);
        }
        using (var streamReader = new StreamReader(expectedFilePath))
        {
            expectedOutput = streamReader.ReadToEnd();
        }
        //Asset
        Assert.Equal(expectedOutput, output); // Check if the result is as expected.

    }
}
