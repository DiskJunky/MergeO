namespace MergeO.Mocks
{
    using System.Collections.Generic;
    using Models;

    public static class SequencedComplexItemBuilder
    {
        public static SequencedComplexItem GetFilledInstance()
        {
            return new SequencedComplexItem
            {
                StringValue = "1"
            };
        }

        public static SequencedComplexItem GetFieldNullInstance()
        {
            return new SequencedComplexItem();
        }

        /// <summary>
        /// This builds a sorted sequence in the specified order.
        /// </summary>
        /// <param name="sequence">The sequence to order items by.</param>
        /// <returns>The ordered array.</returns>
        public static List<SequencedComplexItem> Build(params SequencedComplexItem[] sequence)
        {
            for (int i = 0; i < sequence.Length; i++)
            {
                sequence[i].SequenceID = i + 1;
            }

            return new List<SequencedComplexItem>(sequence);
        }

        public static List<SequencedComplexItem> GetNewNullValueSequence()
        {
            return Build(GetFilledInstance(), GetFieldNullInstance());
        }

        public static List<SequencedComplexItem> GetNewFilledValueSequence()
        {
            return Build(GetFieldNullInstance(), GetFilledInstance());
        }

        public static List<List<SequencedComplexItem>> CreateMostRecentValueTestSet()
        {
            var set = new List<List<SequencedComplexItem>>();
            set.Add(GetNewNullValueSequence());
            set.Add(GetNewFilledValueSequence());

            return set;
        }
    }
}
