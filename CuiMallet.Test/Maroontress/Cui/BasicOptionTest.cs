namespace Maroontress.Cui.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maroontress.Cui;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class BasicOptionTest
    {
        [TestMethod]
        public void WithCallback()
        {
            var list = new List<Option>();
            var schema = Options.NewSchema()
                .Add("help", 'h', "Show help message", o => list.Add(o));

            void Check(params string[] args)
            {
                CheckSchema(
                    schema,
                    () => list.Clear(),
                    () =>
                    {
                        Assert.AreEqual(1, list.Count());
                        if (list.First() is Option e)
                        {
                            CheckArg(e);
                        }
                        else
                        {
                            Assert.Fail();
                        }
                    },
                    args);
            }

            Check("--help", "foo");
            Check("-h", "foo");
            Check("--help", "--", "foo");
            Check("-h", "--", "foo");
            CheckHelpMessage(schema);
        }

        [TestMethod]
        public void WithoutCallback()
        {
            var schema = Options.NewSchema()
                .Add("help", 'h', "Show help message");

            void Check(params string[] args)
            {
                CheckSchema(schema, () => { }, () => { }, args);
            }

            Check("--help", "foo");
            Check("-h", "foo");
            Check("--help", "--", "foo");
            Check("-h", "--", "foo");
            CheckHelpMessage(schema);
        }

        [TestMethod]
        public void Abbreviations()
        {
            var schema = Options.NewSchema()
                .Add(
                    "verbose",
                    'v',
                    "Be verbose")
                .Add(
                    "help",
                    'h',
                    "Show help message and exit");
            var setting = schema.Parse(new[] { "--ver" });
            Assert.AreSame(schema, setting.Schema);

            var arguments = setting.Arguments;
            Assert.IsFalse(arguments.Any());

            var options = setting.Options.ToArray();
            Assert.AreEqual(1, options.Length);

            void Verify(Option o, string name)
            {
                if (o is RequiredArgumentOption)
                {
                    Assert.Fail();
                    return;
                }
                Assert.AreEqual(name, o.Name);
                Assert.AreSame(schema, o.Schema);
            }

            Verify(options[0], "verbose");
        }

        private static void CheckHelpMessage(OptionSchema schema)
        {
            var helpLines = schema.GetHelpMessage().ToArray();
            Assert.AreEqual(1, helpLines.Length);
            var help = helpLines[0];
            Assert.AreEqual(
                "-h, --help  Show help message",
                help);
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
            Assert.AreEqual("help", o.Name);
            Assert.IsTrue(o.ShortName.HasValue);
            Assert.AreEqual('h', o.ShortName.GetValueOrDefault());
            Assert.AreEqual("Show help message", o.Description);
            Assert.AreSame(schema, o.Schema);
            Assert.AreSame(schema, setting.Schema);
            postAction();
        }

        private static void CheckArg(Option o)
        {
            Assert.AreEqual("help", o.Name);
            Assert.IsTrue(o.ShortName.HasValue);
            Assert.AreEqual('h', o.ShortName.GetValueOrDefault());
            Assert.AreEqual("Show help message", o.Description);
        }
    }
}
