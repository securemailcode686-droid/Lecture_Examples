using System.Linq;
using System.Collections.Generic;
using LogHub.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
    [TestClass]
    public sealed class DependencyGraphTests
    {
        private static (DependencyGraph Graph, Node Root)
            CreateGraph()
        {
            var graph = new DependencyGraph();

            var a = graph.AddNode("A");
            var b = graph.AddNode("B");
            var c = graph.AddNode("C");
            var d = graph.AddNode("D");
            var e = graph.AddNode("E");

            graph.AddDependency(a, b);
            graph.AddDependency(a, c);
            graph.AddDependency(b, d);
            graph.AddDependency(b, e);
            graph.AddDependency(c, e);

            return (graph, a);
        }

        private static string[] Names(
            IEnumerable<Node> nodes)
        {
            return nodes.Select(n => n.Name).ToArray();
        }

        [TestMethod]
        public void DFS_ПравильныйПорядокОбхода()
        {
            var (graph, root) = CreateGraph();

            CollectionAssert.AreEqual(
                new[] { "A", "B", "D", "E", "C" },
                Names(graph.DepthFirst(root)));
        }

        [TestMethod]
        public void BFS_ПравильныйПорядокОбхода()
        {
            var (graph, root) = CreateGraph();

            CollectionAssert.AreEqual(
                new[] { "A", "B", "C", "D", "E" },
                Names(graph.BreadthFirst(root)));
        }

        [TestMethod]
        public void ВложенныеForeach_Независимы()
        {
            var (graph, root) = CreateGraph();
            var result = new List<string>();

            foreach (var service in graph.DepthFirst(root))
            {
                foreach (var dependency in
                    graph.BreadthFirst(service))
                {
                    result.Add(
                        $"{service.Name}->{dependency.Name}");
                }
            }

            CollectionAssert.AreEqual(
                new[]
                {
                    "A->A", "A->B", "A->C",
                    "A->D", "A->E",
                    "B->B", "B->D", "B->E",
                    "D->D",
                    "E->E",
                    "C->C", "C->E"
                },
                result.ToArray());
        }

        [TestMethod]
        public void ГрафСЦиклом_ОбходЗавершается()
        {
            var graph = new DependencyGraph();

            var a = graph.AddNode("A");
            var b = graph.AddNode("B");
            var c = graph.AddNode("C");

            graph.AddDependency(a, b);
            graph.AddDependency(b, c);
            graph.AddDependency(c, a);

            var expected = new[] { "A", "B", "C" };

            CollectionAssert.AreEqual(expected,
                Names(graph.DepthFirst(a).Take(10)));

            CollectionAssert.AreEqual(expected,
                Names(graph.BreadthFirst(a).Take(10)));

            CollectionAssert.AreEqual(expected,
                Names(graph.DepthFirstManual(a).Take(10)));

            CollectionAssert.AreEqual(expected,
                Names(graph.BreadthFirstManual(a).Take(10)));
        }

        [TestMethod]
        public void ЦиклическаяЗависимость_ВозвращаетПуть()
        {
            var graph = new DependencyGraph();

            var a = graph.AddNode("A");
            var b = graph.AddNode("B");
            var c = graph.AddNode("C");

            graph.AddDependency(a, b);
            graph.AddDependency(b, c);
            graph.AddDependency(c, a);

            CollectionAssert.AreEqual(
                new[] { "A", "B", "C", "A" },
                Names(graph.FindCycle()));
        }

        [TestMethod]
        public void ПустойГраф_ОбходВозвращаетПустуюКоллекцию()
        {
            var graph = new DependencyGraph();

            Assert.AreEqual(0,
                graph.DepthFirst(null).Count());

            Assert.AreEqual(0,
                graph.BreadthFirst(null).Count());

            Assert.AreEqual(0,
                graph.DepthFirstManual(null).Count());

            Assert.AreEqual(0,
                graph.BreadthFirstManual(null).Count());

            Assert.AreEqual(0, graph.FindCycle().Count);
        }

        [TestMethod]
        public void ОднаВершина_ОбходВозвращаетЕеОдинРаз()
        {
            var graph = new DependencyGraph();
            var root = graph.AddNode("LogProcessor");

            var expected = new[] { "LogProcessor" };

            CollectionAssert.AreEqual(expected,
                Names(graph.DepthFirst(root)));

            CollectionAssert.AreEqual(expected,
                Names(graph.BreadthFirst(root)));

            CollectionAssert.AreEqual(expected,
                Names(graph.DepthFirstManual(root)));

            CollectionAssert.AreEqual(expected,
                Names(graph.BreadthFirstManual(root)));
        }

        [TestMethod]
        public void DFS_РучнойИтераторСовпадаетСYield()
        {
            var (graph, root) = CreateGraph();

            CollectionAssert.AreEqual(
                Names(graph.DepthFirst(root)),
                Names(graph.DepthFirstManual(root)));
        }

        [TestMethod]
        public void BFS_РучнойИтераторСовпадаетСYield()
        {
            var (graph, root) = CreateGraph();

            CollectionAssert.AreEqual(
                Names(graph.BreadthFirst(root)),
                Names(graph.BreadthFirstManual(root)));
        }

        [TestMethod]
        public void РучныеИтераторы_НеДелятСостояние()
        {
            var (graph, root) = CreateGraph();

            var traversal = graph.DepthFirstManual(root);

            using var first = traversal.GetEnumerator();
            using var second = traversal.GetEnumerator();

            Assert.IsTrue(first.MoveNext());
            Assert.AreEqual("A", first.Current.Name);

            Assert.IsTrue(first.MoveNext());
            Assert.AreEqual("B", first.Current.Name);

            Assert.IsTrue(second.MoveNext());
            Assert.AreEqual("A", second.Current.Name);

            Assert.IsTrue(first.MoveNext());
            Assert.AreEqual("D", first.Current.Name);

            Assert.IsTrue(second.MoveNext());
            Assert.AreEqual("B", second.Current.Name);

            var count = 0;

            foreach (var outer in traversal)
            {
                foreach (var inner in traversal)
                    count++;
            }

            Assert.AreEqual(25, count);
        }

        [TestMethod]
        public void Reset_ВозвращаетИтераторВНачало()
        {
            var (graph, root) = CreateGraph();

            using var iterator =
                graph.DepthFirstManual(root).GetEnumerator();

            Assert.IsTrue(iterator.MoveNext());
            Assert.IsTrue(iterator.MoveNext());

            Assert.AreEqual("B", iterator.Current.Name);

            iterator.Reset();

            Assert.IsTrue(iterator.MoveNext());
            Assert.AreEqual("A", iterator.Current.Name);
        }

        [TestMethod]
        public void ОбщаяЗависимость_НеЯвляетсяЦиклом()
        {
            var (graph, root) = CreateGraph();

            Assert.AreEqual(0, graph.FindCycle().Count);

            Assert.AreEqual(5,
                graph.DepthFirst(root).Count());

            Assert.AreEqual(5,
                graph.BreadthFirst(root).Count());

            Assert.AreEqual(5,
                graph.DepthFirstManual(root).Count());

            Assert.AreEqual(5,
                graph.BreadthFirstManual(root).Count());
        }
    }
}
