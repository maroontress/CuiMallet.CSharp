namespace Maroontress.Cui.Impl;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// The default implementation of <see cref="OptionSchema"/> interface.
/// </summary>
public sealed class OptionSchemaImpl : OptionSchema
{
    /// <summary>
    /// The empty option schema.
    /// </summary>
    public static readonly OptionSchema Empty = new OptionSchemaImpl();

    private const int HeadingSeparatorLength = 2;
    private const int MaxHeadingLength = 32;
    private const int IndentUnit = 4;

    private static readonly Action<Option> NoAction = o => { };

    private readonly ImmutableList<Spec> all;

    private readonly ImmutableDictionary<string, Spec> nameMap;

    private readonly ImmutableDictionary<char, Spec> shortNameMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionSchemaImpl"/> class.
    /// </summary>
    public OptionSchemaImpl()
    {
        all = [];
        nameMap = ImmutableDictionary<string, Spec>.Empty;
        shortNameMap = ImmutableDictionary<char, Spec>.Empty;
    }

    private OptionSchemaImpl(IEnumerable<Spec> p)
    {
        all = p.ToImmutableList();
        nameMap = p.ToImmutableDictionary(o => o.Name);
        shortNameMap = p.Where(o => o.ShortName.HasValue)
            .ToImmutableDictionary(o => o.ShortName.GetValueOrDefault());
    }

    private delegate string HeadingProvier(string n, string a);

    /// <inheritdoc/>
    public OptionSchema Add(
        string name,
        char? shortName,
        string argumentName,
        string description,
        Action<RequiredArgumentOption> action)
    {
        CheckName(name);
        CheckShortName(shortName);
        CheckDuplication(name, shortName);
        var spec = new RequiredArgumentOptionSpec(
            name, shortName, argumentName, description, action);
        return new OptionSchemaImpl(all.Append(spec));
    }

    /// <inheritdoc/>
    public OptionSchema Add(
        string name,
        char? shortName,
        string description,
        Action<Option> action)
    {
        CheckName(name);
        CheckShortName(shortName);
        CheckDuplication(name, shortName);
        var spec = new OptionSpec(
            name, shortName, description, action);
        return new OptionSchemaImpl(all.Append(spec));
    }

    /// <inheritdoc/>
    public OptionSchema Add(
        string name,
        char? shortName,
        string argumentName,
        string description)
    {
        return Add(name, shortName, argumentName, description, NoAction);
    }

    /// <inheritdoc/>
    public OptionSchema Add(
        string name, char? shortName, string description)
    {
        return Add(name, shortName, description, NoAction);
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetHelpMessage()
    {
        /*
            |<---- heading length ---->|
            |<-- short heading -->|     description
                                        description (continued)
            |<-------- long heading -------->|
                                        description
                                        description (continued)
        */
        static int NameComparator(Spec s1, Spec s2)
            => string.CompareOrdinal(s1.Name, s2.Name);

        var allHeadings = all.Sort(NameComparator)
            .Select(s => (spec: s, heading: s.GetHelpHeading()))
            .ToList();
        var headingLength = Math.Min(
            allHeadings.Max(h => h.heading.Length)
                + HeadingSeparatorLength,
            MaxHeadingLength);
        var u = IndentUnit - 1;
        headingLength = (headingLength + u) & ~u;

        var b = new StringBuilder();
        var list = new List<string>();
        foreach (var (o, h) in allHeadings)
        {
            var d = o.Description.Split('\n');
            var first = d.First();
            var remaining = d.Skip(1);
            var n = h.Length + HeadingSeparatorLength;
            if (n > headingLength)
            {
                list.Add(h);
                b.Clear();
                b.Append(' ', headingLength);
                b.Append(first);
                list.Add(b.ToString());
            }
            else
            {
                b.Clear();
                b.Append(h);
                b.Append(' ', headingLength - h.Length);
                b.Append(first);
                list.Add(b.ToString());
            }
            foreach (var r in remaining)
            {
                b.Clear();
                b.Append(' ', headingLength);
                b.Append(r);
                list.Add(b.ToString());
            }
        }
        return list;
    }

    /// <inheritdoc/>
    public Setting Parse(params string[] args)
    {
        var queue = new Queue<string>(args);
        var list = new List<Option>(args.Length);
        var map = new Dictionary<RequiredArgumentOptionSpec,
            IEnumerable<string>>();

        IEnumerable<string> GetValues(RequiredArgumentOptionSpec s)
            => map.TryGetValue(s, out var a) ? a : [];

        void AddOption(Func<string> n, OptionSpec s)
        {
            var o = new OptionImpl(n, s, this);
            s.Fire(o);
            list.Add(o);
        }

        void AddRequiredArgumentOption(
            Func<string> n, RequiredArgumentOptionSpec s, string v)
        {
            var a = GetValues(s).Append(v);
            var o = new RequiredArgumentOptionImpl(n, s, this, a);
            s.Fire(o);
            list.Add(o);
            map[s] = a.ToImmutableArray();
        }

        var builder = new Builder(AddOption, AddRequiredArgumentOption);
        while (queue.Count > 0 && ParseOption(queue, builder))
        {
            continue;
        }
        return new SettingImpl(this, queue, list);
    }

    private static void CheckName(string name)
    {
        var match = Regex.Match(name, "[^0-9a-zA-Z-]");
        if (match != Match.Empty)
        {
            var c = match.Value;
            throw new InvalidOptionSchemaException(
                $"The name contains an invalid character: '{c}'");
        }
    }

    private static void CheckShortName(char? shortName)
    {
        if (!shortName.HasValue)
        {
            return;
        }
        var c = shortName.Value;
        if (c > 0x7f || !char.IsLetterOrDigit(c))
        {
            throw new InvalidOptionSchemaException(
                $"The short name '{c}' is an invalid character.");
        }
    }

    private void CheckDuplication(string name, char? shortName)
    {
        if (nameMap.ContainsKey(name))
        {
            throw new InvalidOptionSchemaException(
                $"The name of the Option '{name}' is already added.");
        }
        if (shortName.HasValue
            && shortNameMap.ContainsKey(shortName.Value))
        {
            throw new InvalidOptionSchemaException(
                $"The short name of the Option '{shortName.Value}' "
                + "is already added.");
        }
    }

    private bool ParseOption(Queue<string> queue, Builder builder)
    {
        var s = queue.Peek();
        if (s is "--")
        {
            queue.Dequeue();
            return false;
        }
        if (s.StartsWith("--"))
        {
            queue.Dequeue();
            ParseLongOption(s, queue, builder);
            return true;
        }
        if (s is "-"
            || !s.StartsWith("-", StringComparison.InvariantCulture))
        {
            return false;
        }
        queue.Dequeue();
        ParseShortOption(s, queue, builder);
        return true;
    }

    private void ParseShortOption(
        string s, Queue<string> queue, Builder builder)
    {
        var shortNameQueue = new Queue<char>(s);
        shortNameQueue.Dequeue();

        do
        {
            var c = shortNameQueue.Dequeue();
            var kit = new ParseKit(this, c);
            if (!shortNameMap.TryGetValue(c, out var spec))
            {
                throw kit.NewUnknownOption();
            }
            spec.VisitShortOption(
                kit, builder, shortNameQueue, queue);
        }
        while (shortNameQueue.Count > 0);
    }

    private void ParseLongOption(
        string s, Queue<string> queue, Builder builder)
    {
        var kit = new ParseKit(this, s);
        var n = s.IndexOf('=');
        (var name, var value) = (n < 0)
            ? (s[2..], null)
            : (s[2..n], s[(n + 1)..]);

        if (nameMap.TryGetValue(name, out var spec))
        {
            spec.VisitLongOption(kit, builder, value, queue);
            return;
        }
        var candidates = nameMap.Keys
            .Where(k => k.StartsWith(name))
            .ToArray();
        if (candidates.Length == 0)
        {
            throw kit.NewUnknownOption();
        }
        if (candidates.Length > 1)
        {
            throw kit.NewAmbiguousOption(candidates);
        }
        spec = nameMap[candidates[0]];
        spec.VisitLongOption(kit, builder, value, queue);
    }
}
