using MergeO.Contracts;

namespace MergeO
{
    /// <summary>
    /// This is used to provide context for instances of <see cref="IMergeCriteria"/> so that they
    /// know the context of the object graphs being compared, not just the current values in
    /// the comparision.
    /// </summary>
    public class ContextRoot : IContextRoot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextRoot"/> class.
        /// </summary>
        /// <param name="oldValue">The root old value.</param>
        /// <param name="newValue">The root new value.</param>
        public ContextRoot(object oldValue, object newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets or sets the root old value.
        /// </summary>
        public object OldValue { get; set; }

        /// <summary>
        /// Gets or sets the root new value.
        /// </summary>
        public object NewValue { get; set; }
    }
}
