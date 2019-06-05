#pragma warning disable SA1118

namespace Maroontress.Cui.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maroontress.Cui;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class BasicTest
    {
        [TestMethod]
        public void NoOptionNoArg()
        {
            var schema = Options.NewSchema();
            var setting = schema.Parse(Array.Empty<string>());
            Assert.AreEqual(0, setting.Arguments.Count());
            Assert.AreEqual(0, setting.Options.Count());
            Assert.AreSame(schema, setting.Schema);
        }

        [TestMethod]
        public void FirstArgumentIsHyphen()
        {
            var schema = Options.NewSchema();
            var setting = schema.Parse("-");
            Assert.AreEqual(1, setting.Arguments.Count());
            Assert.AreEqual("-", setting.Arguments.First());
            Assert.AreEqual(0, setting.Options.Count());
            Assert.AreSame(schema, setting.Schema);
        }

        [TestMethod]
        public void ToStringWithShortName()
        {
            var schema = Options.NewSchema()
                .Add("verbose", 'v', "Be verbose.");
            var setting = schema.Parse("-v");
            Assert.AreEqual(0, setting.Arguments.Count());
            Assert.AreEqual(1, setting.Options.Count());
            Assert.AreEqual("-v", $"{setting.Options.First()}");
        }

        [TestMethod]
        public void ToStringWithLongName()
        {
            var schema = Options.NewSchema()
                .Add("verbose", 'v', "Be verbose.");
            var setting = schema.Parse("--verbose");
            Assert.AreEqual(0, setting.Arguments.Count());
            Assert.AreEqual(1, setting.Options.Count());
            Assert.AreEqual("--verbose", $"{setting.Options.First()}");
        }

        [TestMethod]
        public void ToStringWithLongNameAbbreviations()
        {
            var schema = Options.NewSchema()
                .Add("verbose", 'v', "Be verbose.");
            var setting = schema.Parse("--verb");
            Assert.AreEqual(0, setting.Arguments.Count());
            Assert.AreEqual(1, setting.Options.Count());
            Assert.AreEqual("--verb", $"{setting.Options.First()}");
        }

        [TestMethod]
        public void ToStringWithLongNameEqualArgument()
        {
            var schema = Options.NewSchema()
                .Add("verbose", 'v', "LEVEL", "Be verbose.");
            var setting = schema.Parse("--verb=max");
            Assert.AreEqual(0, setting.Arguments.Count());
            Assert.AreEqual(1, setting.Options.Count());
            Assert.AreEqual("--verb=max", $"{setting.Options.First()}");
        }

        [TestMethod]
        public void HelpMessageOfLongNameOption()
        {
            var list = new List<RequiredArgumentOption>();
            var schema = Options.NewSchema()
                .Add(
                    "very-very-long-name-option",
                    'v',
                    "ARGUMENT",
                    "Specify an argument",
                    v => list.Add(v));

            var helpLines = schema.GetHelpMessage().ToArray();
            var expected = new[]
            {
            /*   |---|---|---|---|---|---|---|---| */
                "-v, --very-very-long-name-option ARGUMENT",
                "                                Specify an argument",
            };
            Assert.IsTrue(expected.SequenceEqual(helpLines));
        }

        [TestMethod]
        public void HelpMessageHasMutipleLines()
        {
            var list = new List<RequiredArgumentOption>();
            var schema = Options.NewSchema()
                .Add(
                    "file",
                    'f',
                    "FILE",
                    "Specify input file\n"
                        + "Example: -f foo.txt, --file=foo.txt",
                    v => list.Add(v));

            var helpLines = schema.GetHelpMessage().ToArray();
            var expected = new[]
            {
            /*   |---|---|---|---|---| */
                "-f, --file FILE     Specify input file",
                "                    Example: -f foo.txt, --file=foo.txt",
            };
            Assert.IsTrue(expected.SequenceEqual(helpLines));
        }

        [TestMethod]
        public void HelpMessageSorting()
        {
            var list = new List<RequiredArgumentOption>();
            var schema = Options.NewSchema()
                .Add(
                    "file",
                    'f',
                    "FILE",
                    "Specify input file",
                    v => list.Add(v))
                .Add(
                    "dir",
                    'd',
                    "DIR",
                    "Change the current directory",
                    v => list.Add(v));

            var helpLines = schema.GetHelpMessage().ToArray();
            var expected = new[]
            {
            /*   |---|---|---|---|---| */
                "-d, --dir DIR       Change the current directory",
                "-f, --file FILE     Specify input file",
            };
            Assert.IsTrue(expected.SequenceEqual(helpLines));
        }

        [TestMethod]
        public void HelpMessageWithNoShortNameOption()
        {
            var schema = Options.NewSchema()
                .Add("verbose", null, "Be verbose")
                .Add("debug", 'd', "Be debug mode");

            var helpLines = schema.GetHelpMessage().ToArray();
            var expected = new[]
            {
            /*   |---|---|---|---| */
                "-d, --debug     Be debug mode",
                "    --verbose   Be verbose",
            };
            Assert.IsTrue(expected.SequenceEqual(helpLines));
        }

        [TestMethod]
        public void ConcatenatingShortNameOptions()
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
            var setting = schema.Parse(new[] { "-vh" });
            Assert.AreSame(schema, setting.Schema);

            var arguments = setting.Arguments;
            Assert.IsFalse(arguments.Any());

            var options = setting.Options.ToArray();
            Assert.AreEqual(2, options.Length);

            void Check(Option o, string name)
            {
                if (o is RequiredArgumentOption)
                {
                    Assert.Fail();
                }
                Assert.AreEqual(name, o.Name);
                Assert.AreSame(schema, o.Schema);
            }

            Check(options[0], "verbose");
            Check(options[1], "help");
        }

        [TestMethod]
        public void ConcatenatedShortNameOptionRequiringArgument()
        {
            var schema = Options.NewSchema()
                .Add(
                    "help",
                    'h',
                    "Show help message and exit")
                .Add(
                    "file",
                    'f',
                    "FILE",
                    "Specify input file");

            var setting = schema.Parse(new[] { "-fh" });
            Assert.AreSame(schema, setting.Schema);

            var arguments = setting.Arguments;
            Assert.IsFalse(arguments.Any());

            var options = setting.Options.ToArray();
            Assert.AreEqual(1, options.Length);

            void Verify(Option o, string name, string arg)
            {
                if (!(o is RequiredArgumentOption v))
                {
                    Assert.Fail();
                    return;
                }
                Assert.AreEqual(name, v.Name);
                Assert.AreEqual(arg, v.ArgumentValue);
                Assert.AreSame(schema, v.Schema);
            }

            Verify(options[0], "file", "h");
        }

        [TestMethod]
        public void MultipleArguments()
        {
            var list = new List<RequiredArgumentOption>();
            var schema = Options.NewSchema()
                .Add(
                    "file",
                    'f',
                    "FILE",
                    "Specify input file",
                    v => list.Add(v));

            var setting = schema.Parse(new[]
            {
                "--file=foo.txt",
                "--file=bar.txt",
            });
            Assert.AreSame(schema, setting.Schema);

            var arguments = setting.Arguments;
            Assert.IsFalse(arguments.Any());

            var options = setting.Options.ToArray();
            Assert.AreEqual(2, options.Length);

            void CheckFirst(Option o)
            {
                if (!(o is RequiredArgumentOption v))
                {
                    Assert.Fail();
                    return;
                }
                Assert.AreEqual("file", v.Name);
                Assert.AreSame(schema, v.Schema);
                Assert.AreEqual("foo.txt", v.ArgumentValue);
                var expectedValues = new[]
                {
                    "foo.txt",
                };
                Assert.IsTrue(expectedValues.SequenceEqual(v.ArgumentValues));
            }

            void CheckSecond(Option o)
            {
                if (!(o is RequiredArgumentOption v))
                {
                    Assert.Fail();
                    return;
                }
                Assert.AreEqual("file", v.Name);
                Assert.AreSame(schema, v.Schema);
                Assert.AreEqual("bar.txt", v.ArgumentValue);
                var expectedValues = new[]
                {
                    "foo.txt",
                    "bar.txt",
                };
                Assert.IsTrue(expectedValues.SequenceEqual(v.ArgumentValues));
            }

            CheckFirst(options[0]);
            CheckSecond(options[1]);
            Assert.IsTrue(list.SequenceEqual(options));
        }
    }
}
