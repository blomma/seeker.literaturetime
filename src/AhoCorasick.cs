using System.Collections.Immutable;

namespace seeker.literaturetime;

internal class AhoCorasick
{
    private struct FlatNode
    {
        public int Failure;
        public int ChildrenOffset;
        public int ChildrenCount;
        public int ResultsOffset;
        public int ResultsCount;
    }

    private FlatNode[] _nodes = [];
    private char[] _childChars = [];
    private int[] _childIndices = [];
    private (string TimeKey, string Phrase, int Priority)[] _allResults = [];
    private int[] _transitions = []; // DFA transitions for ASCII 0-255

    // Temporary building structures
    private class TempNode
    {
        public Dictionary<char, TempNode> Children { get; } = new();
        public TempNode? Failure { get; set; }
        public List<(string TimeKey, string Phrase, int Priority)> Results { get; } = new();
        public int Index { get; set; }
    }

    private TempNode? _tempRoot = new();

    public void Add(string timeKey, string phrase, int priority)
    {
        if (_tempRoot == null)
        {
            throw new InvalidOperationException("Automaton already built.");
        }

        var node = _tempRoot;
        foreach (var c in phrase)
        {
            var lc = char.ToLowerInvariant(c);
            if (!node.Children.TryGetValue(lc, out var next))
            {
                next = new TempNode();
                node.Children[lc] = next;
            }
            node = next;
        }
        node.Results.Add((timeKey, phrase, priority));
    }

    public void Build()
    {
        if (_tempRoot == null)
        {
            return;
        }

        var queue = new Queue<TempNode>();
        foreach (var child in _tempRoot.Children.Values)
        {
            child.Failure = _tempRoot;
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
                while (failure != _tempRoot && failure != null && !failure.Children.ContainsKey(c))
                {
                    failure = failure.Failure;
                }

                if (failure != null && failure.Children.TryGetValue(c, out var failureChild))
                {
                    child.Failure = failureChild;
                }
                else
                {
                    child.Failure = _tempRoot;
                }

                child.Results.AddRange(child.Failure.Results);
                queue.Enqueue(child);
            }
        }

        Flatten();
        _tempRoot = null;
    }

    private void Flatten()
    {
        var tempNodes = new List<TempNode>();
        var q = new Queue<TempNode>();
        q.Enqueue(_tempRoot!);

        while (q.Count > 0)
        {
            var n = q.Dequeue();
            n.Index = tempNodes.Count;
            tempNodes.Add(n);
            foreach (var child in n.Children.Values)
            {
                q.Enqueue(child);
            }
        }

        _nodes = new FlatNode[tempNodes.Count];
        _transitions = new int[tempNodes.Count * 256];

        int totalChildren = 0;
        int totalResults = 0;
        foreach (var tn in tempNodes)
        {
            totalChildren += tn.Children.Count;
            totalResults += tn.Results.Count;
        }

        _childChars = new char[totalChildren];
        _childIndices = new int[totalChildren];
        _allResults = new (string, string, int)[totalResults];

        int currentChildOffset = 0;
        int currentResultOffset = 0;

        for (int i = 0; i < tempNodes.Count; i++)
        {
            var tn = tempNodes[i];
            var fn = new FlatNode
            {
                Failure = tn.Failure?.Index ?? 0,
                ChildrenOffset = currentChildOffset,
                ChildrenCount = tn.Children.Count,
                ResultsOffset = currentResultOffset,
                ResultsCount = tn.Results.Count,
            };

            foreach (var kvp in tn.Children)
            {
                _childChars[currentChildOffset] = kvp.Key;
                _childIndices[currentChildOffset] = kvp.Value.Index;
                currentChildOffset++;
            }

            foreach (var res in tn.Results)
            {
                _allResults[currentResultOffset] = res;
                currentResultOffset++;
            }

            _nodes[i] = fn;
        }

        // Build DFA transitions for ASCII
        for (int i = 0; i < tempNodes.Count; i++)
        {
            for (int c = 0; c < 256; c++)
            {
                char ch = (char)c;
                int state = i;
                while (state != 0 && !HasChild(state, ch))
                {
                    state = _nodes[state].Failure;
                }

                if (TryGetChild(state, ch, out int nextState))
                {
                    _transitions[i * 256 + c] = nextState;
                }
                else
                {
                    _transitions[i * 256 + c] = 0;
                }
            }
        }
    }

    private bool HasChild(int stateIndex, char c)
    {
        ref readonly var node = ref _nodes[stateIndex];
        var chars = _childChars.AsSpan(node.ChildrenOffset, node.ChildrenCount);
        return chars.IndexOf(c) != -1;
    }

    private bool TryGetChild(int stateIndex, char c, out int nextState)
    {
        ref readonly var node = ref _nodes[stateIndex];
        var chars = _childChars.AsSpan(node.ChildrenOffset, node.ChildrenCount);
        var index = chars.IndexOf(c);
        if (index != -1)
        {
            nextState = _childIndices[node.ChildrenOffset + index];
            return true;
        }
        nextState = 0;
        return false;
    }

    public void Search(
        ReadOnlySpan<char> text,
        List<(string TimeKey, string Phrase, int Priority)> foundMatches
    )
    {
        int currentState = 0;
        for (var i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (c < 256)
            {
                // Fast path: DFA transition for ASCII/Extended ASCII
                if (c is >= 'A' and <= 'Z')
                {
                    c = (char)(c | 0x20);
                }
                currentState = _transitions[currentState * 256 + c];
            }
            else
            {
                // Slow path: Fallback to failure links for non-ASCII characters
                c = char.ToLowerInvariant(c);
                while (currentState != 0 && !TryGetChild(currentState, c, out _))
                {
                    currentState = _nodes[currentState].Failure;
                }

                if (TryGetChild(currentState, c, out int nextState))
                {
                    currentState = nextState;
                }
            }

            if (currentState == 0)
            {
                continue;
            }

            ref readonly var node = ref _nodes[currentState];
            if (node.ResultsCount > 0)
            {
                for (int r = 0; r < node.ResultsCount; r++)
                {
                    var (timeKey, phrase, priority) = _allResults[node.ResultsOffset + r];
                    var startIndex = i - phrase.Length + 1;
                    if (
                        Matcher.IsBeforeCharValid(text, phrase, startIndex)
                        && Matcher.IsAfterCharValid(text, phrase, startIndex)
                    )
                    {
                        foundMatches.Add((timeKey, phrase, priority));
                    }
                }
            }
        }
    }

    public static AhoCorasick CreateCombinedAutomaton(
        ImmutableDictionary<string, List<string>> oneOf,
        ImmutableDictionary<string, List<string>> generic,
        ImmutableDictionary<string, List<string>> superGeneric
    )
    {
        var ac = new AhoCorasick();
        foreach (var kvp in oneOf)
        {
            foreach (var phrase in kvp.Value)
            {
                ac.Add(kvp.Key, phrase, 1);
            }
        }

        foreach (var kvp in generic)
        {
            foreach (var phrase in kvp.Value)
            {
                ac.Add(kvp.Key, phrase, 2);
            }
        }

        foreach (var kvp in superGeneric)
        {
            foreach (var phrase in kvp.Value)
            {
                ac.Add(kvp.Key, phrase, 3);
            }
        }

        ac.Build();
        return ac;
    }
}
