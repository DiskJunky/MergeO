namespace MergeO.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// This object is used to merge a history of objects into a single instance of that
    /// oject with the most recent version of that data.
    /// </summary>
    public interface IMerger
    {
        /// <summary>
        /// This merges a history of an object graph into a single object instance of type <typeparamref name="T"/>
        /// that contains the most recent data.
        /// </summary>
        /// <param name="history">The history of object data to merge</param>
        /// <param name="historyComparer">The comparer instance to use for sorting the history before merge. Will
        /// default to the type's default comparer if none specified.</param>
        /// <param name="nonDefaultMergeCriteria">Any object/property-specific criteria where by to determine
        /// when to overwrite the value when evaluating the property's history.</param>
        /// <returns>A single object instance containing the most recent data.</returns>
        /// <typeparam name="T">The type of object to merge.</typeparam>
        T Merge<T>(ICollection<T> history,
                   IComparer<T> historyComparer = null,
                   ICollection<IMergeCriteria> nonDefaultMergeCriteria = null);

        /// <summary>
        /// This merges a history of an object graph into a single object instance of type <typeparamref name="T"/>
        /// that contains the most recent data.
        /// </summary>
        /// <param name="historyComparer">The comparer instance to use for sorting the history before merge. Will
        /// default to the type's default comparer if none specified.</param>
        /// <param name="nonDefaultMergeCriteria">Any object/property-specific criteria where by to determine
        /// when to overwrite the value when evaluating the property's history.</param>
        /// <param name="history">The history of object data to merge, in order.</param>
        /// <returns>A single object instance containing the most recent data.</returns>
        /// <typeparam name="T">The type of object to merge.</typeparam>
        T Merge<T>(IComparer<T> historyComparer = null,
                   ICollection<IMergeCriteria> nonDefaultMergeCriteria = null,
                   params T[] history);

        /// <summary>
        /// This merges a history of an object graph into a single object instance of type <typeparamref name="T"/>
        /// that contains the most recent data.
        /// </summary>
        /// <param name="history">The history of object data to merge, in order.</param>
        /// <returns>A single object instance containing the most recent data.</returns>
        /// <typeparam name="T">The type of object to merge.</typeparam>
        T MergeItems<T>(params T[] history);
    }
}
