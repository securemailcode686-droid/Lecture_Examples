using System.Collections;
using System.Collections.Generic;

namespace LogHub.Graphs
{
    internal sealed class GraphTraversal : IEnumerable<Node>
    {
        private readonly DependencyGraph _graph;
        private readonly Node? _root;
        private readonly bool _breadthFirst;

        public GraphTraversal(
            DependencyGraph graph,
            Node? root,
            bool breadthFirst)
        {
            _graph = graph;
            _root = root;
            _breadthFirst = breadthFirst;
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return new GraphEnumerator(
                _graph, _root, _breadthFirst);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
