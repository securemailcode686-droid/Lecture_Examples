using System.Linq;
using LogHub.Composition;
using LogHub.Graphs;
using LogHub.Processing;
using LogHub.Tests.Doubles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
    [TestClass]
    public sealed class ServiceGraphBuilderTests
    {
        private static DependencyGraph BuildGraph(string sink)
        {
            var config = Configs.With(
                ("sink", sink),
                ("format", "plain"),
                ("minSeverity", "Info"),
                ("importer", "nginx"));

            var services = new ServiceCollection();

            CompositionRoot.ConfigureServices(
                services, config,
                typeof(LogProcessor).Assembly);

            return ServiceGraphBuilder.Build(services);
        }

        [TestMethod]
        public void Граф_СодержитЗависимостиLogProcessor()
        {
            var graph = BuildGraph("console");
            var root = graph.FindNode("LogProcessor");

            Assert.IsNotNull(root);

            CollectionAssert.AreEqual(
                new[]
                {
                    "ILogSink",
                    "SeverityFilter",
                    "ILogFormatter",
                    "ILogImporter"
                },
                graph.DependenciesOf(root)
                    .Select(n => n.Name).ToArray());

            Assert.AreEqual(0, graph.FindCycle().Count);
        }

        [TestMethod]
        public void Граф_УчитываетВыбранныйПриемник()
        {
            var consoleGraph = BuildGraph("console");
            var databaseGraph = BuildGraph("database");

            var consoleSink = consoleGraph.FindNode("ILogSink");
            var databaseSink = databaseGraph.FindNode("ILogSink");

            Assert.IsNotNull(consoleSink);
            Assert.IsNotNull(databaseSink);

            Assert.AreEqual(0,
                consoleGraph.DependenciesOf(consoleSink).Count);

            CollectionAssert.AreEqual(
                new[] { "ILogDatabase" },
                databaseGraph.DependenciesOf(databaseSink)
                    .Select(n => n.Name).ToArray());
        }

        [TestMethod]
        public void Граф_НаходитЦиклВРегистрациях()
        {
            var services = new ServiceCollection();

            services.AddTransient<CycleA>();
            services.AddTransient<CycleB>();

            var graph = ServiceGraphBuilder.Build(services);

            CollectionAssert.AreEqual(
                new[] { "CycleA", "CycleB", "CycleA" },
                graph.FindCycle()
                    .Select(n => n.Name).ToArray());
        }

        private sealed class CycleA
        {
            public CycleA(CycleB dependency) { }
        }

        private sealed class CycleB
        {
            public CycleB(CycleA dependency) { }
        }
    }
}
