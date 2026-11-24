using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace LogHub.Graphs
{
    public static class ServiceGraphBuilder
    {
        public static DependencyGraph Build(
            IServiceCollection services)
        {
            var graph = new DependencyGraph();

            var registrations = services.ToDictionary(
                s => s.ServiceType);

            var nodes = registrations.Keys.ToDictionary(
                type => type,
                type => graph.AddNode(type.Name));

            foreach (var registration in registrations.Values)
            {
                if (registration.ImplementationInstance != null)
                    continue;

                var implementation =
                    registration.ImplementationType
                    ?? throw new NotSupportedException(
                        "Фабричная регистрация не поддерживается.");

                var constructor = implementation
                    .GetConstructors()
                    .Where(c => c.GetParameters().All(p =>
                        registrations.ContainsKey(p.ParameterType)
                        || p.HasDefaultValue))
                    .OrderByDescending(c =>
                        c.GetParameters().Length)
                    .FirstOrDefault();

                if (constructor == null)
                    throw new InvalidOperationException(
                        $"Не найден конструктор {implementation.Name}");

                foreach (var parameter in constructor.GetParameters())
                {
                    if (nodes.TryGetValue(
                        parameter.ParameterType,
                        out var dependency))
                    {
                        graph.AddDependency(
                            nodes[registration.ServiceType],
                            dependency);
                    }
                }
            }

            return graph;
        }
    }
}
