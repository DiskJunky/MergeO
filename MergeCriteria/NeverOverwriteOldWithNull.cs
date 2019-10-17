namespace MergeO.MergeCriteria
{
    using MergeO.Contracts;

    /// <summary>
    /// This class is used for when the new value is null but the old isn't, the it should NOT overwrite
    /// with the 'new' value.
    /// </summary>
    public class NeverOverwriteOldWithNull : IMergeCriteria
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NeverOverwriteOldWithNull"/> class.
        /// </summary>
        public NeverOverwriteOldWithNull()
        { }

        /// <summary>
        /// Instantiates the object where the rule will apply at the specified location in the object graph.
        /// </summary>
        /// <param name="activateAt">The breadcrumb location where the rule should apply in the object graph.</param>
        public NeverOverwriteOldWithNull(string activateAt)
            : this()
        {
            ActivateAt = activateAt;
        }

        /// <summary>
        /// Gets the object graph location (as built by <see cref="BreadcrumbHelper"/>), where the rule applies.
        /// </summary>
        public string ActivateAt { get; }


        /// <summary>
        /// This outlines the signature to perform a write based on contextual criteria.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="contextData">The contextual data from which to make a decision as to which value to return.</param>
        /// <returns>The result of the contextual decision. Should be either <paramref name="oldValue"/> or <paramref name="newValue"/>.</returns>
        public object DoWrite(object oldValue, object newValue, IContextRoot contextData)
        {
            if (newValue == null) return oldValue;
            return newValue;
        }
    }
}
