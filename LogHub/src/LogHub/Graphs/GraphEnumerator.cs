using System;
using System.Collections;
using System.Collections.Generic;

namespace LogHub.Graphs
{
    internal sealed class GraphEnumerator : IEnumerator<Node>
    {
        private readonly DependencyGraph _graph;
        private readonly Node? _root;
        private readonly bool _breadthFirst;

        private readonly LinkedList<Node> _pending = new();
        private readonly HashSet<Node> _visited = new();

        private Node? _current;

        public GraphEnumerator(
            DependencyGraph graph,
            Node? root,
            bool breadthFirst)
        {
            _graph = graph;
            _root = root;
            _breadthFirst = breadthFirst;

            Reset();
        }

        public Node Current =>
            _current ?? throw new InvalidOperationException(
                "Итератор не указывает на элемент.");

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            _current = null;

            while (_pending.First != null)
            {
                var node = _pending.First.Value;
                _pending.RemoveFirst();

                if (!_visited.Add(node))
                    continue;

                var dependencies = _graph.DependenciesOf(node);

                if (_breadthFirst)
                {
                    foreach (var dependency in dependencies)
                        _pending.AddLast(dependency);
                }
                else
                {
                    for (int i = dependencies.Count - 1;
                        i >= 0; i--)
                    {
                        _pending.AddFirst(dependencies[i]);
                    }
                }

                _current = node;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _pending.Clear();
            _visited.Clear();
            _current = null;

            if (_root != null)
                _pending.AddLast(_root);
        }

        public void Dispose()
        {
            _pending.Clear();
            _visited.Clear();
            _current = null;
        }
    }
}
