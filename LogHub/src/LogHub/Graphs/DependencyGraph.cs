using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LogHub.Graphs
{
    public sealed class DependencyGraph : IEnumerable<Node>
    {
        private readonly Dictionary<string, Node> _nodes =
            new(StringComparer.Ordinal);

        private readonly Dictionary<Node, List<Node>> _edges =
            new();

        public Node AddNode(string name)
        {
            if (_nodes.TryGetValue(name, out var existing))
                return existing;

            var node = new Node(name);

            _nodes.Add(name, node);
            _edges.Add(node, new List<Node>());

            return node;
        }

        public void AddDependency(Node service, Node dependency)
        {
            EnsureNode(service);
            EnsureNode(dependency);

            if (!_edges[service].Contains(dependency))
                _edges[service].Add(dependency);
        }

        public IReadOnlyList<Node> DependenciesOf(Node node)
        {
            EnsureNode(node);
            return _edges[node].AsReadOnly();
        }

        public Node? FindNode(string name)
        {
            return _nodes.TryGetValue(name, out var node)
                ? node
                : null;
        }

        private void EnsureNode(Node node)
        {
            if (!_edges.ContainsKey(node))
                throw new ArgumentException(
                    "Узел не принадлежит графу.", nameof(node));
        }

        // DFS через yield return.
        public IEnumerable<Node> DepthFirst(Node? root)
        {
            if (root == null)
                yield break;

            EnsureNode(root);

            var visited = new HashSet<Node>();
            var stack = new Stack<Node>();

            stack.Push(root);

            while (stack.Count > 0)
            {
                var node = stack.Pop();

                if (!visited.Add(node))
                    continue;

                yield return node;

                var dependencies = _edges[node];

                for (int i = dependencies.Count - 1; i >= 0; i--)
                    stack.Push(dependencies[i]);
            }
        }

        // BFS через yield return.
        public IEnumerable<Node> BreadthFirst(Node? root)
        {
            if (root == null)
                yield break;

            EnsureNode(root);

            var visited = new HashSet<Node>();
            var queue = new Queue<Node>();

            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                if (!visited.Add(node))
                    continue;

                yield return node;

                foreach (var dependency in _edges[node])
                    queue.Enqueue(dependency);
            }
        }

        // Ручные реализации итераторов.
        public IEnumerable<Node> DepthFirstManual(Node? root)
        {
            if (root != null)
                EnsureNode(root);

            return new GraphTraversal(this, root, false);
        }

        public IEnumerable<Node> BreadthFirstManual(Node? root)
        {
            if (root != null)
                EnsureNode(root);

            return new GraphTraversal(this, root, true);
        }

        // Поиск циклической зависимости во всём графе.
        public IReadOnlyList<Node> FindCycle()
        {
            var active = new HashSet<Node>();
            var completed = new HashSet<Node>();
            var path = new List<Node>();

            foreach (var node in _nodes.Values)
            {
                var cycle = Visit(node);

                if (cycle != null)
                    return cycle;
            }

            return Array.Empty<Node>();

            IReadOnlyList<Node>? Visit(Node node)
            {
                if (active.Contains(node))
                {
                    var index = path.IndexOf(node);

                    return path.Skip(index)
                        .Append(node)
                        .ToArray();
                }

                if (completed.Contains(node))
                    return null;

                active.Add(node);
                path.Add(node);

                foreach (var dependency in _edges[node])
                {
                    var cycle = Visit(dependency);

                    if (cycle != null)
                        return cycle;
                }

                path.RemoveAt(path.Count - 1);
                active.Remove(node);
                completed.Add(node);

                return null;
            }
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return _nodes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
