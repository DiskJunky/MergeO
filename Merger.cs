namespace MergeO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MergeO.Contracts;
    using MergeO.Helpers;
    using MergeO.MergeCriteria;

    /// <summary>
    /// This object is used to merge a history of objects into a single instance of that
    /// oject with the most recent version of that data.
    /// </summary>
    public class Merger : IMerger
    {
        #region Private Variables
        /// <summary>
        /// Holdsa a reference to the default write criteria.
        /// </summary>
        private readonly IMergeCriteria _defaultMergeCriteria;
        #endregion

        #region Lifetime Management
        /// <summary>
        /// Instantiates the object with the behaviour of <see cref="AlwaysUseNewer"/> as default.
        /// </summary>
        public Merger() : this(new AlwaysUseNewer()) { }

        /// <summary>
        /// Instantiates the object with the specified default behaviour.
        /// </summary>
        /// <param name="defaultMergeCriteria">The default write criteria.</param>
        public Merger(IMergeCriteria defaultMergeCriteria)
        {
            _defaultMergeCriteria = defaultMergeCriteria;
        }
        #endregion

        #region IMerger Implementation
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
        public T Merge<T>(ICollection<T> history, IComparer<T> historyComparer = null,
                          ICollection<IMergeCriteria> nonDefaultMergeCriteria = null)
        {
            T value = Activator.CreateInstance<T>();
            try
            {
                if (history == null || history.Count < 1)
                {
                    // return the default instance
                    return value;
                }

                // build a dictionary from the non-default ruleset
                var mergeCriteria = new Dictionary<string, IMergeCriteria>();
                if (nonDefaultMergeCriteria != null)
                {
                    mergeCriteria = nonDefaultMergeCriteria.ToDictionary(c => c.ActivateAt, c => c);
                }

                if (history.Count == 1)
                {
                    // there's only the one item to "merge"
                    using (IEnumerator<T> enumerator = history.GetEnumerator())
                    {
                        enumerator.MoveNext();
                        return enumerator.Current;
                    }
                }

                // sort the history before attempting a merge
                List<T> sortedHistory = SortHistory(history, historyComparer);

                // cycle through all items up to the last one
                value = sortedHistory[0];
                for (int i = 1; i < sortedHistory.Count; i++)   // start at 1 as that's the first 'version'
                {
                    // perform merge
                    T newValue = sortedHistory[i];
                    var context = new ContextRoot(value, newValue);
                    value = MergeObjects(value, newValue, mergeCriteria, context);
                }
            }
            catch (Exception e)
            {
                //TODO lOG: "There was a problem trying to merge an Model history.");
                throw;
            }

            return value;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the comparer to use for sorting the object history.
        /// </summary>
        /// <param name="historyComparer">The caller supplied comparer, if any.</param>
        /// <returns>The comparer instance to use, regardless if caller supplied.</returns>
        /// <typeparam name="T">The type of object to merge.</typeparam>
        private IComparer<T> GetComparer<T>(IComparer<T> historyComparer)
        {
            if (historyComparer != null)
            {
                return historyComparer;
            }

            return Comparer<T>.Default;
        }

        /// <summary>
        /// This retrieves the correct write criteria for performing a merge.
        /// </summary>
        /// <param name="breadcrumb">The breadcrumb location to get write crteria for.</param>
        /// <param name="mergeCriteria">The write criteria collection to search through.</param>
        /// <returns>The write criteria to use for the merge.</returns>
        private IMergeCriteria GetMergeCriteria(string breadcrumb, Dictionary<string, IMergeCriteria> mergeCriteria)
        {
            IMergeCriteria criteria = _defaultMergeCriteria;
            if (mergeCriteria.ContainsKey(breadcrumb))
            {
                criteria = mergeCriteria[breadcrumb];
            }

            return criteria;
        }

        /// <summary>
        /// This will merge two object instances and return the newer.
        /// </summary>
        /// <typeparam name="TValue">The data type of the objects to merge.</typeparam>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="nonDefaultMergeCriteria">Any non-default criteria for evaluating the correct new value.</param>
        /// <param name="context">The context on which an instance of <see cref="IMergeCriteria"/> can make a decision.</param>
        /// <param name="breadcrumb">The breadcrumb of the current object path.</param>
        /// <returns>The merged object instances.</returns>
        private TValue MergeObjects<TValue>(TValue oldValue, TValue newValue,
                                            Dictionary<string, IMergeCriteria> nonDefaultMergeCriteria,
                                            ContextRoot context = null,
                                            string breadcrumb = null)
        {
            Type valueType = typeof(TValue);
            return (TValue)MergeObjects(valueType, oldValue, newValue, nonDefaultMergeCriteria, context, breadcrumb);
        }

        /// <summary>
        /// This will merge two object instances and return the newer.
        /// </summary>
        /// <param name="valueType">The type of the value being merged.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="nonDefaultMergeCriteria">Any non-default criteria for evaluating the correct new value.</param>
        /// <param name="context">The context on which an instance of <see cref="IMergeCriteria"/> can make a decision.</param>
        /// <param name="breadcrumb">The breadcrumb of the current object path.</param>
        /// <returns>The merged object instances.</returns>
        private object MergeObjects(Type valueType,
                                    object oldValue, object newValue,
                                    Dictionary<string, IMergeCriteria> nonDefaultMergeCriteria,
                                    ContextRoot context = null,
                                    string breadcrumb = null)
        {
            if (string.IsNullOrWhiteSpace(breadcrumb))
            {
                breadcrumb = valueType.Name;
            }
            //TODO lOG: .Trace($"Merging at [{breadcrumb}]...");

            // are we dealing with a simple data type
            Type underlyingType;
            Type currentType = GetType(oldValue, newValue);
            if (currentType.IsNullable(out underlyingType))
            {
                // unbox the type if needed
                currentType = underlyingType;
            }

            if (currentType.IsBaseType()
                || currentType.IsNullable()
                || currentType.IsByteArray())
            {
                // perform the merge
                var mergeCriteria = GetMergeCriteria(breadcrumb, nonDefaultMergeCriteria);
                object mergedValue = mergeCriteria.DoWrite(oldValue, newValue, context);
                return mergedValue;
            }

            // TODO: deal with enumerables (the call to IsBaseType() above already checks for string)

            // we have a more complex object, pull out the properties
            PropertyInfo[] properties = currentType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
            object merged = Activator.CreateInstance(valueType);
            foreach (PropertyInfo property in properties)
            {
                // skip anything non-mergable
                if (!property.CanRead || !property.CanWrite) continue;

                // get the values to merge
                string propertyBreadcrumb = breadcrumb.Append(property.Name);
                object oldPropertyValue = oldValue != null ? property.GetValue(oldValue) : null;
                object newPropertyValue = newValue != null ? property.GetValue(newValue) : null;

                // do we need to merge at all
                if (oldPropertyValue == null && newPropertyValue == null) continue;

                // we have a value, perform a merge
                object mergedValue = null;
                if (nonDefaultMergeCriteria.ContainsKey(propertyBreadcrumb))
                {
                    // use the write criteria to do the merge
                    var mergeCriteria = GetMergeCriteria(propertyBreadcrumb, nonDefaultMergeCriteria);
                    mergedValue = mergeCriteria.DoWrite(oldPropertyValue, newPropertyValue, context);
                }
                else
                {
                    // do a deep merge
                    mergedValue = MergeObjects(property.PropertyType,
                                               oldPropertyValue,
                                               newPropertyValue,
                                               nonDefaultMergeCriteria,
                                               context,
                                               propertyBreadcrumb);
                }

                //TODO lOG: Trace($"Setting property {propertyBreadcrumb} with: {mergedValue}");
                property.SetValue(merged, mergedValue);
            }

            return merged;
        }

        /// <summary>
        /// Gets the data type based on the available values and information.
        /// </summary>
        /// <typeparam name="TValue">The fall back data type if it cannot be derived from an actual value.</typeparam>
        /// <param name="oldValue">The possible old value.</param>
        /// <param name="newValue">The possible new value.</param>
        /// <returns>The data type based on the available information.</returns>
        private Type GetType<TValue>(TValue oldValue, TValue newValue)
        {
            if (oldValue == null && newValue == null)
            {
                return typeof(TValue);
            }
            if (oldValue == null)
            {
                return newValue.GetType();
            }

            return oldValue.GetType();
        }

        /// <summary>
        /// Sorts the supplied object history by comparer.
        /// </summary>
        /// <param name="history">The history to compare.</param>
        /// <param name="historyComparer">The comparer to use to sort the history.</param>
        /// <returns>The sorted object history.</returns>
        /// <typeparam name="T">The type of object to merge.</typeparam>
        private List<T> SortHistory<T>(ICollection<T> history, IComparer<T> historyComparer = null)
        {
            // sort the history before processing
            var comparer = GetComparer(historyComparer);
            var sortedCollection = new List<T>(history);
            sortedCollection.Sort(comparer);
            return sortedCollection;
        }
        #endregion
    }
}
