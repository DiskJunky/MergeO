namespace MergeO.MergeCriteria
{
    using MergeO.Contracts;

    /// <summary>
    /// This describes the default write criteria behaviour of always returning the newer value
    /// regardless of context.
    /// </summary>
    public class AlwaysUseNewer : IMergeCriteria
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlwaysUseNewer"/> class.
        /// </summary>
        public AlwaysUseNewer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlwaysUseNewer"/> class.
        /// </summary>
        /// <param name="activateAt">The activate at.</param>
        public AlwaysUseNewer(string activateAt)
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
            return newValue;
        }
    }
}
