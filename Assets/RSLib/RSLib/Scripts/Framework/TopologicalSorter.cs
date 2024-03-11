namespace RSLib.Framework
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Items that are sorted using the topological sort algorithm can implement this interface only if the method
    /// to get their dependencies isn't passed in as an argument, but directly called on the items themselves.
    /// </summary>
    /// <typeparam name="T">Type of the items to sort.</typeparam>
    public interface ITopologicalSortedItem<T>
    {
        /// <summary>
        /// Gets the items this one depends on.
        /// </summary>
        /// <returns>Collection containing the dependencies.</returns>
        IEnumerable<T> GetDependencies();
    }

    /// <summary>
    /// Class containing methods to sort items by dependencies.
    /// Items dependencies are a collection of other items of the same type, that can be retrieved else by having the item type
    /// implementing the ITopologicalSortedItem interface, or by passing in the dependencies getter method as an argument to the Sort method.
    /// Cyclic dependencies are not allowed and an exception will be thrown if one is encountered during the sorting. The log message will use
    /// the item ToString(), so it would be a good thing to override it, to enhance the log context (by displaying something specific to each item).
    /// </summary>
    public static class TopologicalSorter
    {
        /// <summary>
        /// Sort method using the ITopologicalSortedItem.GetDependencies method to get each item dependencies.
        /// </summary>
        /// <typeparam name="T">Type of the items to sort, that must implement ITopologicalSortedItem.</typeparam>
        /// <param name="content">Items to sort.</param>
        /// <returns>IEnumerable of items sorted by their dependencies.</returns>
        public static IEnumerable<T> Sort<T>(IEnumerable<T> content) where T : ITopologicalSortedItem<T>
        {
            return Sort(content, o => o.GetDependencies());
        }

        /// <summary>
        /// Sort method with a custom method to get items dependencies.
        /// </summary>
        /// <typeparam name="T">Type of the items to sort.</typeparam>
        /// <param name="content">Items to sort.</param>
        /// <param name="getDependencies">Method used to get each item dependencies.</param>
        /// <returns>IEnumerable of items sorted by their dependencies.</returns>
        public static IEnumerable<T> Sort<T>(IEnumerable<T> content, System.Func<T, IEnumerable<T>> getDependencies)
        {
            HashSet<T> sorted = new HashSet<T>();
            Dictionary<T, bool> visited = new Dictionary<T, bool>();

            foreach (T item in content)
                Visit(item, getDependencies, sorted, visited);

            return sorted;
        }

        /// <summary>
        /// Recursive method used to sort items, by visiting them and checking their dependencies.
        /// </summary>
        /// <typeparam name="T">Type of the items to sort.</typeparam>
        /// <param name="item">Currently visited item.</param>
        /// <param name="getDependencies">Method used to get each item dependencies.</param>
        /// <param name="sorted">Collection of sorted items, in which items are added after the items they depend on.</param>
        /// <param name="visited">Dictionary to keep track of visited items to handle dependencies, and detect cyclic dependencies.</param>
        private static void Visit<T>(T item, System.Func<T, IEnumerable<T>> getDependencies, HashSet<T> sorted, Dictionary<T, bool> visited)
        {
            if (visited.TryGetValue(item, out bool inProcess))
            {
                if (inProcess)
                {
                    // Cycle dependency detected, retrace cycle to throw an exception with some context (loop is reversed on purpose).

                    List<T> cycleKeys = new List<T>();
                    T[] keysArrayReversed = visited.Keys.ToArray();

                    for (int i = keysArrayReversed.Length - 1; i >= 0; --i)
                    {
                        if (keysArrayReversed[i].Equals(item))
                            break;

                        cycleKeys.Add(keysArrayReversed[i]);
                    }

                    throw new CyclicDependencyException(item, cycleKeys.Cast<object>().Reverse().ToArray());
                }
            }
            else
            {
                visited[item] = true;

                IEnumerable<T> dependencies = getDependencies(item);
                if (dependencies != null)
                    foreach (T dependency in dependencies)
                        Visit(dependency, getDependencies, sorted, visited);

                visited[item] = false;
                sorted.Add(item);
            }
        }

        public class CyclicDependencyException : System.Exception
        {
            public CyclicDependencyException() : this("Cyclic dependency found!") {}
            public CyclicDependencyException(string message) : base(message) {}
            public CyclicDependencyException(object item) : base($"Cyclic dependency found while visiting item {item}!") {}
            public CyclicDependencyException(object item, object[] visited) : base($"Cyclic dependency found while visiting item {item}! (Retraced cycle: {string.Join(", ", visited)}).")  {}
        }
    }
}