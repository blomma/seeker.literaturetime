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
    private (string TimeKey, string Phrase)[] _allResults = [];

    // Temporary building structures
    private class TempNode
    {
        public Dictionary<char, TempNode> Children { get; } = new();
        public TempNode? Failure { get; set; }
        public List<(string TimeKey, string Phrase)> Results { get; } = new();
        public int Index { get; set; }
    }

    private TempNode? _tempRoot = new();

    public void Add(string timeKey, string phrase)
    {
        if (_tempRoot == null) throw new InvalidOperationException("Automaton already built.");
        
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
        node.Results.Add((timeKey, phrase));
    }

    public void Build()
    {
        if (_tempRoot == null) return;

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
        
        int totalChildren = 0;
        int totalResults = 0;
        foreach (var tn in tempNodes)
        {
            totalChildren += tn.Children.Count;
            totalResults += tn.Results.Count;
        }

        _childChars = new char[totalChildren];
        _childIndices = new int[totalChildren];
        _allResults = new (string, string)[totalResults];

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
                ResultsCount = tn.Results.Count
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
    }

    public void Search(ReadOnlySpan<char> text, Dictionary<string, string> matches)
    {
        int currentState = 0;
        for (var i = 0; i < text.Length; i++)
        {
            var c = char.ToLowerInvariant(text[i]);
            
            while (currentState != 0 && !TryGetNextState(currentState, c, out _))
            {
                currentState = _nodes[currentState].Failure;
            }

            if (TryGetNextState(currentState, c, out int nextState))
            {
                currentState = nextState;
            }

            ref readonly var node = ref _nodes[currentState];
            if (node.ResultsCount > 0)
            {
                for (int r = 0; r < node.ResultsCount; r++)
                {
                    var (timeKey, phrase) = _allResults[node.ResultsOffset + r];
                    if (matches.ContainsKey(timeKey)) continue;

                    var startIndex = i - phrase.Length + 1;
                    if (Matcher.IsBeforeCharValid(text, phrase, startIndex) && Matcher.IsAfterCharValid(text, phrase, startIndex))
                    {
                        matches.Add(timeKey, phrase);
                    }
                }
            }
        }
    }

    private bool TryGetNextState(int stateIndex, char c, out int nextState)
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
