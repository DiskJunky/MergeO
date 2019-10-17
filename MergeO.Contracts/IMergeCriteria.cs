namespace MergeO.Contracts
{
    /// <summary>
    /// This interface outlines the criteria whereby a condition needs to be applied
    /// on an object such that it determines whether a new value replaces an old.
    /// </summary>
    public interface IMergeCriteria
    {
        /// <summary>
        /// Gets the object graph location (as built by <see cref="BreadcrumbHelper"/>), where the rule applies.
        /// </summary>
        string ActivateAt { get; }

        /// <summary>
        /// This outlines the signature to perform a write based on contextual criteria.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="contextData">The contextual data from which to make a decision as to which value to return.</param>
        /// <returns>The result of the contextual decision. Should be either <paramref name="oldValue"/> or <paramref name="newValue"/>.</returns>
        object DoWrite(object oldValue, object newValue, IContextRoot contextData);
    }
}
