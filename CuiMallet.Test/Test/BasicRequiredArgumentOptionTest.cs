namespace Maroontress.Cui.Test;

using System;
using System.Collections.Generic;
using System.Linq;
using Maroontress.Cui;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class BasicRequiredArgumentOptionTest
{
    [TestMethod]
    public void WithCallback()
    {
        var list = new List<RequiredArgumentOption>();
        var schema = Options.NewSchema()
            .Add(
                "file",
                'f',
                "FILE",
                "Specify input file",
                o => list.Add(o));

        void Check(params string[] args)
        {
            CheckSchema(
                schema,
                () => list.Clear(),
                () =>
                {
                    Assert.AreEqual(1, list.Count);
                    if (list.First() is RequiredArgumentOption e)
                    {
                        CheckArg(e);
                        CheckValue(e);
                    }
                    else
                    {
                        Assert.Fail();
                    }
                },
                args);
        }

        Check("--file=bar", "foo");
        Check("--file", "bar", "foo");
        Check("-f", "bar", "foo");
        Check("-fbar", "foo");
        Check("--file=bar", "--", "foo");
        Check("--file", "bar", "--", "foo");
        Check("-f", "bar", "--", "foo");
        Check("-fbar", "--", "foo");
        CheckHelpMessage(schema);
    }

    [TestMethod]
    public void WithoutCallback()
    {
        var schema = Options.NewSchema()
            .Add(
                "file",
                'f',
                "FILE",
                "Specify input file");

        void Check(params string[] args)
        {
            CheckSchema(schema, () => { }, () => { }, args);
        }

        Check("--file=bar", "foo");
        Check("--file", "bar", "foo");
        Check("-f", "bar", "foo");
        Check("-fbar", "foo");
        Check("--file=bar", "--", "foo");
        Check("--file", "bar", "--", "foo");
        Check("-f", "bar", "--", "foo");
        Check("-fbar", "--", "foo");
        CheckHelpMessage(schema);
    }

    [TestMethod]
    public void Abbreviations()
    {
        var schema = Options.NewSchema()
            .Add(
                "input",
                'i',
                "FILE",
                "Specify input file")
            .Add(
                "output",
                'o',
                "FILE",
                "Specify output file");

        var setting = schema.Parse(["--in=-"]);
        Assert.AreSame(schema, setting.Schema);

        var arguments = setting.Arguments;
        Assert.IsFalse(arguments.Any());

        var options = setting.Options.ToArray();
        Assert.AreEqual(1, options.Length);

        void Verify(Option o, string name, string arg)
        {
            if (o is not RequiredArgumentOption v)
            {
                Assert.Fail();
                return;
            }
            Assert.AreEqual(name, v.Name);
            Assert.AreEqual(arg, v.ArgumentValue);
            Assert.AreSame(schema, v.Schema);
        }

        Verify(options[0], "input", "-");
    }

    private static void CheckHelpMessage(OptionSchema schema)
    {
        var helpLines = schema.GetHelpMessage().ToArray();
        Assert.AreEqual(1, helpLines.Length);
        var help = helpLines[0];
        Assert.AreEqual(
            "-f, --file FILE     Specify input file",
            help);
    }

    private static void CheckValue(RequiredArgumentOption v)
    {
        Assert.AreEqual("bar", v.ArgumentValue);
        var a = v.ArgumentValues;
        Assert.AreEqual(1, a.Count());
        Assert.AreEqual("bar", a.First());
    }

    private static void CheckArg(RequiredArgumentOption v)
    {
        Assert.AreEqual("file", v.Name);
        Assert.IsTrue(v.ShortName.HasValue);
        Assert.AreEqual('f', v.ShortName.GetValueOrDefault());
        Assert.AreEqual("Specify input file", v.Description);
        Assert.AreEqual("FILE", v.ArgumentName);
    }

    private static void CheckSchema(
        OptionSchema schema,
        Action preAction,
        Action postAction,
        params string[] args)
    {
        preAction();
        var setting = schema.Parse(args);
        var arguments = setting.Arguments.ToArray();
        var options = setting.Options.ToArray();
        Assert.AreEqual(1, arguments.Length);
        Assert.AreEqual("foo", arguments[0]);
        Assert.AreEqual(1, options.Length);
        var o = options[0];
        if (o is RequiredArgumentOption v)
        {
            CheckArg(v);
            CheckValue(v);
        }
        else
        {
            Assert.Fail();
        }

        Assert.AreSame(schema, o.Schema);
        Assert.AreSame(schema, setting.Schema);
        postAction();
    }
}
