using System;
using Microsoft.Extensions.DependencyInjection;

namespace LogHub.Graphs
{
    public static class GraphReport
    {
        public static void Print(IServiceCollection services)
        {
            var graph = ServiceGraphBuilder.Build(services);

            var root = graph.FindNode("LogProcessor")
                ?? throw new InvalidOperationException(
                    "LogProcessor не найден.");

            Console.WriteLine("Граф зависимостей LogHub:");

            foreach (var service in graph.DepthFirst(root))
            {
                foreach (var dependency in
                    graph.DependenciesOf(service))
                {
                    Console.WriteLine(
                        $"{service} -> {dependency}");
                }
            }

            Console.WriteLine();

            Console.WriteLine("DFS (yield return):");
            Console.WriteLine(string.Join(" -> ",
                graph.DepthFirst(root)));

            Console.WriteLine("BFS (yield return):");
            Console.WriteLine(string.Join(" -> ",
                graph.BreadthFirst(root)));

            Console.WriteLine("DFS (ручной итератор):");
            Console.WriteLine(string.Join(" -> ",
                graph.DepthFirstManual(root)));

            Console.WriteLine("BFS (ручной итератор):");
            Console.WriteLine(string.Join(" -> ",
                graph.BreadthFirstManual(root)));

            var cycle = graph.FindCycle();

            Console.WriteLine(cycle.Count == 0
                ? "Циклические зависимости отсутствуют."
                : "Обнаружен цикл: " +
                    string.Join(" -> ", cycle));
        }

        public static void PrintCycleDemo()
        {
            var graph = new DependencyGraph();

            var a = graph.AddNode("ServiceA");
            var b = graph.AddNode("ServiceB");
            var c = graph.AddNode("ServiceC");

            graph.AddDependency(a, b);
            graph.AddDependency(b, c);
            graph.AddDependency(c, a);

            Console.WriteLine();
            Console.WriteLine("Тестовый граф с циклом:");

            Console.WriteLine("DFS: " +
                string.Join(" -> ", graph.DepthFirst(a)));

            Console.WriteLine("BFS: " +
                string.Join(" -> ", graph.BreadthFirst(a)));

            Console.WriteLine("Обнаружен цикл: " +
                string.Join(" -> ", graph.FindCycle()));
        }
    }
}
