using TSqlSharp.Formating;
namespace TSqlSharp.Tests;
public class FormatterTests
{
    [Fact]
    public void FormatSelect()
    {
        // Arrange
        string acutalOutput = "";
        string expectedOutput = "";
        string expectedFilePath = @"../../../../../tests/scripts/SelectExpected.sql";
        string acutalFilePath = @"../../../../../tests/scripts/Select.sql";

        // Act
        acutalOutput = GetFormattedString(acutalFilePath);
        expectedOutput = GetExpectedString(expectedFilePath);

        //Assert
        Assert.Equal(expectedOutput, acutalOutput);
    }

    [Fact]
    public void FormatCreateProc()
    {
        // Arrange
        string acutalOutput = "";
        string expectedOutput = "";
        string expectedFilePath = @"../../../../../tests/scripts/CreateProcedureExpected.sql";
        string acutalFilePath = @"../../../../../tests/scripts/CreateProcedure.sql";

        // Act
        acutalOutput = GetFormattedString(acutalFilePath);
        expectedOutput = GetExpectedString(expectedFilePath);

        //Assert
        Assert.Equal(expectedOutput, acutalOutput);
    }

    [Fact]
    public void FormatCreateTable()
    {
        // Arrange
        string acutalOutput = "";
        string expectedOutput = "";
        string expectedFilePath = @"../../../../../tests/scripts/CreateTableExpected.sql";
        string acutalFilePath = @"../../../../../tests/scripts/CreateTable.sql";

        // Act
        acutalOutput = GetFormattedString(acutalFilePath);
        expectedOutput = GetExpectedString(expectedFilePath);

        //Assert
        Assert.Equal(expectedOutput, acutalOutput);
    }

    [Fact]
    public void FormatSelectWithCte()
    {
        // Arrange
        string acutalOutput = "";
        string expectedOutput = "";
        string expectedFilePath = @"../../../../../tests/scripts/SelectWithCteExpected.sql";
        string acutalFilePath = @"../../../../../tests/scripts/SelectWithCte.sql";

        // Act
        acutalOutput = GetFormattedString(acutalFilePath);
        expectedOutput = GetExpectedString(expectedFilePath);

        //Assert
        Assert.Equal(expectedOutput, acutalOutput);
    }
    [Fact]
    public void FormatUnknownStatement()
    {
        // Arrange
        string acutalOutput = "";
        string expectedOutput = "";
        string expectedFilePath = @"../../../../../tests/scripts/DeleteTableExpected.sql";
        string acutalFilePath = @"../../../../../tests/scripts/DeleteTable.sql";

        // Act
        acutalOutput = GetFormattedString(acutalFilePath);
        expectedOutput = GetExpectedString(expectedFilePath);

        //Assert
        Assert.Equal(expectedOutput, acutalOutput);
    }


    private string GetFormattedString(string filePath)
    {
        var output = "";
        using (var streamReader = new StreamReader(filePath))
        {

            var formatter = new Formatter();

            output = formatter.Format(filePath);
        }

        return output;

    }

    private string GetExpectedString(string filePath)
    {
        var output = "";
        using (var streamReader = new StreamReader(filePath))
        {
            output = streamReader.ReadToEnd();
        }

        return output;
    }        
}
