using System.Collections;
using System.Collections.Generic;
using MergeO.MergeCriteria;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace MergeO.UnitTests.MergeCriteria
{
    [TestFixture]
    public static class InterpolateListsTests
    {
        [Test]
        public static void DoWrite_SortsUnordered()
        {
            var first = UnorderedInts1();
            var second = UnorderedInts2();
            var comparer = Comparer<int>.Default;
            var sorted = new List<int>(first);
            sorted.AddRange(second);
            sorted.Sort(comparer);

            var mergeCriteria = new InterpolateLists<int>(string.Empty, comparer);
            var merged = (IList)mergeCriteria.DoWrite(first, second, null);

            Assert.That(sorted.Count, Is.EqualTo(merged.Count));
            CollectionAssert.AreEqual(sorted, merged);
        }

        private static List<int> UnorderedInts1()
        {
            return new List<int>(new[]
            {
                5,4,3,2,1
            });
        }

        private static List<int> UnorderedInts2()
        {
            return new List<int>(new[]
            {
                4,3,5,2,1
            });
        }
    }
}
