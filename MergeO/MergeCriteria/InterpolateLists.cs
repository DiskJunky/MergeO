namespace MergeO.MergeCriteria
{
    using System.Collections.Generic;
    using MergeO.Contracts;

    /// <summary>
    /// This class is used to merge two instances of <see cref="List{T}"/> and
    /// sort them.
    /// </summary>
    /// <seealso cref="MergeO.Contracts.Mergers.IMergeCriteria" />
    /// <typeparam name="T">The data type that we're interpolating lists of.</typeparam>
    public class InterpolateLists<T> : IMergeCriteria
    {
        /// <summary>
        /// Holds a reference to the comparer to use for sorting the list.
        /// </summary>
        private readonly IComparer<T> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolateLists"/> class.
        /// </summary>
        /// <param name="activateAt">The activate at.</param>
        /// <param name="comparer">The comparer to use for sorting the list.</param>
        public InterpolateLists(string activateAt, IComparer<T> comparer)
        {
            ActivateAt = activateAt;
            _comparer = comparer;
        }

        /// <summary>
        /// Gets the object graph location (as built by <see cref="BreadcrumbHelper" />), where the rule applies.
        /// </summary>
        public string ActivateAt { get; }

        /// <summary>
        /// This outlines the signature to perform a write based on contextual criteria.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="contextData">The contextual data from which to make a decision as to which value to return.</param>
        /// <returns>
        /// The result of the contextual decision. Should be either <paramref name="oldValue" /> or <paramref name="newValue" />.
        /// </returns>
        public object DoWrite(object oldValue, object newValue, IContextRoot contextData)
        {
            var oldCollection = (List<T>) oldValue;
            var newCollection = (List<T>) newValue;

            oldCollection.AddRange(newCollection);
            oldCollection.Sort(_comparer);

            return oldCollection;
        }
    }
}
