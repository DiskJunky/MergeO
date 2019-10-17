namespace MergeO.Mocks.Models
{
    using System.Collections.Generic;

    public class ChronologicalItemComparer : IComparer<ChronologicalItem>
    {
        public int Compare(ChronologicalItem x, ChronologicalItem y)
        {
            return x.Moment.CompareTo(y.Moment);
        }
    }
}
