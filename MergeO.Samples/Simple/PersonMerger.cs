namespace MergeO.Samples.Simple
{
    using MergeO.Contracts;

    internal class PersonMerger
    {
        private IMerger _merger;

        public PersonMerger()
        {
            // defaults to NeverOverwriteOldWithNull() merge pattern
            _merger = new Merger();
        }

        public Person Merge(Person first, Person second)
        {
            return _merger.MergeItems(first, second);
        }
    }
}
