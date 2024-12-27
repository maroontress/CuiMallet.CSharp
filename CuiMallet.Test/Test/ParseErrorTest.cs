namespace Maroontress.Cui.Test;

using Maroontress.Cui;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ParseErrorTest
{
    [TestMethod]
    public void DuplicateLongNameOption()
    {
        var schema = Options.NewSchema()
            .Add(
                "help",
                'h',
                "Show help message and exit");

        Assert.ThrowsException<InvalidOptionSchemaException>(
            () => schema.Add(
                "help",
                'H',
                "Show help message and exit"));
    }

    [TestMethod]
    public void DuplicateShortNameOption()
    {
        var schema = Options.NewSchema()
            .Add(
                "show-help",
                'h',
                "Show help message and exit");

        Assert.ThrowsException<InvalidOptionSchemaException>(
            () => schema.Add(
                "help",
                'h',
                "Show help message and exit"));
    }

    [TestMethod]
    public void ShortNameIsHyphen()
    {
        Assert.ThrowsException<InvalidOptionSchemaException>(
            () => Options.NewSchema()
            .Add(
                "hyphen",
                '-',
                "Short name is '-'"));
    }

    [TestMethod]
    public void ShortNameIsNotAscii()
    {
        Assert.ThrowsException<InvalidOptionSchemaException>(
            () => Options.NewSchema()
            .Add(
                "not-ascii",
                '\x00c0',
                "Not ASCII"));
    }

    [TestMethod]
    public void LongNameContainsEqual()
    {
        Assert.ThrowsException<InvalidOptionSchemaException>(
            () => Options.NewSchema()
            .Add(
                "contains=equals",
                null,
                "Name contains '='"));
    }

    [TestMethod]
    public void NoArgumentWithLongNameOption()
    {
        var schema = Options.NewSchema()
            .Add(
                "file",
                'f',
                "FILE",
                "Specify input file");

        var args = new[]
        {
            "--file",
        };
        Assert.ThrowsException<OptionParsingException>(
            () => _ = schema.Parse(args));
    }

    [TestMethod]
    public void LongNameOptionIsAmbiguous()
    {
        var schema = Options.NewSchema()
            .Add(
                "file",
                'f',
                "FILE",
                "Specify input file")
            .Add(
                "fill",
                'F',
                "Specify fill mode");

        var args = new[]
        {
            "--fil",
        };
        Assert.ThrowsException<OptionParsingException>(
            () => _ = schema.Parse(args));
    }

    [TestMethod]
    public void UnableToGetArgumentWithLongNameOption()
    {
        var schema = Options.NewSchema()
            .Add(
                "help",
                'h',
                "Show help message and exit");

        var args = new[]
        {
            "--help=yes",
        };
        Assert.ThrowsException<OptionParsingException>(
            () => _ = schema.Parse(args));
    }

    [TestMethod]
    public void UnknownOptionWithShortName()
    {
        var schema = Options.NewSchema();

        var args = new[]
        {
            "-h",
        };
        Assert.ThrowsException<OptionParsingException>(
            () => _ = schema.Parse(args));
    }

    [TestMethod]
    public void UnknownOptionWithNoArgShortName()
    {
        var schema = Options.NewSchema()
            .Add(
                "help",
                'h',
                "Show help message and exit");

        var args = new[]
        {
            "-vh",
        };
        Assert.ThrowsException<OptionParsingException>(
            () => _ = schema.Parse(args));
    }

    [TestMethod]
    public void UnknownOptionWithLongName()
    {
        var schema = Options.NewSchema();

        var args = new[]
        {
            "--help",
        };
        Assert.ThrowsException<OptionParsingException>(
            () => _ = schema.Parse(args));
    }

    [TestMethod]
    public void ShortNameOptionRequiringArgumentWithoutArgument()
    {
        var schema = Options.NewSchema()
            .Add(
                "file",
                'f',
                "FILE",
                "Specify input file");

        var args = new[]
        {
            "-f",
        };
        Assert.ThrowsException<OptionParsingException>(
            () => _ = schema.Parse(args));
    }
}
