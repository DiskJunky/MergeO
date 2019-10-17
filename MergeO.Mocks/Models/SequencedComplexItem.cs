namespace MergeO.Mocks.Models
{
    using System;

    public class SequencedComplexItem : IComparable
    {
        public int SequenceID { get; set; }

        public string StringValue { get; set; }

        public int CompareTo(object obj)
        {
            var typedObj = (SequencedComplexItem)obj;
            return SequenceID.CompareTo(typedObj.SequenceID);
        }
    }
}
