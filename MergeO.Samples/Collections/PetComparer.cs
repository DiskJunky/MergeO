using System.Collections.Generic;

namespace MergeO.Samples.Collections
{
    internal class PetComparer : IComparer<Pet>
    {
        public int Compare(Pet x, Pet y)
        {
            return string.Compare(x?.Name, y?.Name);
        }
    }
}
