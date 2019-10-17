namespace MergeO.Mocks
{
    using System;
    using System.Collections.Generic;
    using Models;

    public static class ChronologicalItemBuilder
    {
        public static readonly DateTime DefaultMoment = new DateTime(2000, 1, 1);

        public static readonly ChronologicalItemComparer Comparer = new ChronologicalItemComparer();

        public static readonly ChronologicalItem DefaultItem = new ChronologicalItem(DefaultMoment);

        public static readonly ChronologicalItem DefaultItemPlusOne = new ChronologicalItem(DefaultMoment.AddDays(1));

        public static readonly ChronologicalItem DefaultItemPlusTwo = new ChronologicalItem(DefaultMoment.AddDays(2));

        public static List<object[]> ChronologicalItemSet()
        {
            var set = new List<object[]>();

            // add ordered items
            set.Add(new object[]{ DefaultItem });
            set.Add(new object[]{ DefaultItem, DefaultItemPlusOne });
            set.Add(new object[]{ DefaultItem, DefaultItemPlusOne, DefaultItemPlusTwo });

            // add out of order items
            set.Add(new object[]{ DefaultItemPlusOne, DefaultItem });
            set.Add(new object[]{ DefaultItemPlusOne, DefaultItemPlusTwo, DefaultItem });
            set.Add(new object[]{ DefaultItemPlusTwo, DefaultItemPlusOne, DefaultItem });

            return set;
        }
    }
}
