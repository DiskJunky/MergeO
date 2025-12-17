namespace MergeO.Mocks.Models
{
    using System;
    using System.Collections;

    public class NestedObject : IComparable
    {
        public NestedObject()
        {
            Child = new AllSupportedPropertyTypesObject();
            Child.SetPropertyDefaults();
        }

        public AllSupportedPropertyTypesObject Child { get; set; }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that
        /// indicates whether the current instance precedes, follows, or occurs in the same position in the
        /// sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has
        /// these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in
        /// the sort order. Zero This instance occurs in the same position in the sort order as
        /// <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the
        /// sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            bool objChildNull = ((NestedObject)obj).Child == null;
            if (Child == null && (obj == null || objChildNull)) return 0;
            if (Child != null && (obj == null || objChildNull)) return -1;
            if (Child == null && (obj != null || !objChildNull)) return 1;

            return Child.CompareTo(((NestedObject)obj).Child);
        }
    }
}
