using System.Collections.Immutable;

namespace seeker.literaturetime;

internal class AhoCorasick
{
    private class Node
    {
        public Dictionary<char, Node> Children { get; } = new();
        public Node? Failure { get; set; }
        public List<(string TimeKey, string Phrase)> Results { get; } = new();
    }

    private readonly Node _root = new();

    public void Add(string timeKey, string phrase)
    {
        var node = _root;
        foreach (var c in phrase)
        {
            var lc = char.ToLowerInvariant(c);
            if (!node.Children.TryGetValue(lc, out var next))
            {
                next = new Node();
                node.Children[lc] = next;
            }
            node = next;
        }
        node.Results.Add((timeKey, phrase));
    }

    public void Build()
    {
        var queue = new Queue<Node>();
        foreach (var child in _root.Children.Values)
        {
            child.Failure = _root;
            queue.Enqueue(child);
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var kvp in current.Children)
            {
                var c = kvp.Key;
                var child = kvp.Value;
                var failure = current.Failure;
                while (failure != _root && failure != null && !failure.Children.ContainsKey(c))
                {
                    failure = failure.Failure;
                }

                if (failure != null && failure.Children.TryGetValue(c, out var failureChild))
                {
                    child.Failure = failureChild;
                }
                else
                {
                    child.Failure = _root;
                }

                child.Results.AddRange(child.Failure.Results);
                queue.Enqueue(child);
            }
        }
    }

    public void Search(ReadOnlySpan<char> text, Dictionary<string, string> matches)
    {
        var node = _root;
        for (var i = 0; i < text.Length; i++)
        {
            var c = char.ToLowerInvariant(text[i]);
            while (node != _root && !node.Children.ContainsKey(c))
            {
                node = node.Failure!;
            }

            if (node.Children.TryGetValue(c, out var next))
            {
                node = next;
            }

            if (node.Results.Count > 0)
            {
                foreach (var (timeKey, phrase) in node.Results)
                {
                    if (matches.ContainsKey(timeKey))
                    {
                        continue;
                    }

                    var startIndex = i - phrase.Length + 1;
                    if (
                        Matcher.IsBeforeCharValid(text, phrase, startIndex)
                        && Matcher.IsAfterCharValid(text, phrase, startIndex)
                    )
                    {
                        matches.Add(timeKey, phrase);
                    }
                }
            }
        }
    }

    public static AhoCorasick CreateAutomaton(ImmutableDictionary<string, List<string>> phrases)
    {
        var ac = new AhoCorasick();
        foreach (var kvp in phrases)
        {
            foreach (var phrase in kvp.Value)
            {
                ac.Add(kvp.Key, phrase);
            }
        }
        ac.Build();
        return ac;
    }
}
