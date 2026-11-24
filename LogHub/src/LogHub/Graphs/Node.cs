using System;

namespace LogHub.Graphs
{
    public sealed class Node
    {
        public string Name { get; }

        internal Node(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
