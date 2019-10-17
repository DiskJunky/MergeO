namespace MergeO.Mocks.Models
{
    using System;

    /// <summary>
    /// This holds an object instance with chronological data.
    /// </summary>
    public class ChronologicalItem
    {
        public ChronologicalItem()
            : this(ChronologicalItemBuilder.DefaultMoment)
        {
        }

        public ChronologicalItem(DateTime moment)
        {
            Moment = moment;
        }

        public DateTime Moment { get; set; }
    }
}
