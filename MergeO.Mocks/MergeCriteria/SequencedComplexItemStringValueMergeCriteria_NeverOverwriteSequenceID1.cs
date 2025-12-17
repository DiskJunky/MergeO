namespace MergeO.Mocks.MergeCriteria
{
    using MergeO.Contracts;
    using MergeO.Helpers;
    using Models;
    using System;

    /// <summary>
    /// This is a test instance of <see cref="IMergeCriteria"/> that will check the root
    /// <see cref="SequencedComplexItem"/> and ensure that the value of <see cref="SequencedComplexItem.StringValue"/>
    /// for <see cref="SequencedComplexItem.SequenceID"/> of 1 is never overwritten. Otherwise, newest wins.
    /// </summary>
    public class SequencedComplexItemStringValueMergeCriteria_NeverOverwriteSequenceID1 : IMergeCriteria

    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequencedComplexItemStringValueMergeCriteria_NeverOverwriteSequenceID1"/> class.
        /// </summary>
        public SequencedComplexItemStringValueMergeCriteria_NeverOverwriteSequenceID1()
        {
            // this class has a very specific target class/property
            ActivateAt = BreadcrumbHelper<SequencedComplexItem>.Of(s => s.StringValue);
        }

        /// <summary>
        /// Gets the object graph location (as built by <see cref="T:MergeO.Contracts.Helpers.ObjectBreadcrumbExtensions" />), where the rule applies.
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
        /// <exception cref="System.NotImplementedException"></exception>
        public object DoWrite(object oldValue, object newValue, IContextRoot contextData)
        {
            if (contextData == null) throw new ArgumentNullException(nameof(contextData));

            var oldRoot = (SequencedComplexItem)contextData.OldValue;

            if (oldRoot.SequenceID == 1) return oldValue;
            return newValue;
        }
    }
}
