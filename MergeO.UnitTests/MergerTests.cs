namespace MergeO.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using MergeO.Contracts;
    using MergeO.Helpers;
    using MergeO.MergeCriteria;
    using Mocks;
    using Mocks.MergeCriteria;
    using Mocks.Models;
    using NUnit.Framework;

    [TestFixture]
    public class MergerTests
    {
        private Merger _merger = new Merger();

        [TestCase(1)]
        [TestCase(0, 1)]
        [TestCase(0, 1, 2)]
        [TestCase(0, 2, 1)]
        [TestCase(true)]
        [TestCase(false, true)]
        [TestCase(0d, 1d)]
        [TestCase(0d, 1d, 2d)]
        [TestCase(0d, 2d, 1d)]
        [TestCase("1")]
        [TestCase("", "1")]
        [TestCase("", "1", "2")]
        [TestCase("", "2", "1")]
        public void Merger_SortsTypeHistory_ReturnsNewest(params object[] values)
        {
            // value should always be the last one
            var mergedValue = _merger.Merge(values);
            var max = values.Max();
            Assert.AreEqual(max, mergedValue);
        }

        [TestCaseSource(typeof(ChronologicalItemBuilder), nameof(ChronologicalItemBuilder.ChronologicalItemSet))]
        public void Merger_SortsComplexTypeHistory_ReturnsNewest(params object[] values)
        {
            var comparer = ChronologicalItemBuilder.Comparer;
            var typedValues = new List<ChronologicalItem>(values.Cast<ChronologicalItem>());
            var sorted = new List<ChronologicalItem>(typedValues);
            sorted.Sort(comparer);

            var mergedValue = _merger.Merge(typedValues, comparer);
            var expected = sorted[sorted.Count - 1];
            Assert.AreEqual(expected.Moment, mergedValue.Moment);
        }

        [TestCaseSource(typeof(ComplexObjectBuilder), nameof(ComplexObjectBuilder.GetComplexObjectNullibleFieldSet))]
        public void Merger_DefaultMergeCriteria_UsesNewValues(params ComplexObjectNullableFields[] history)
        {
            // the merger will sort the objects, meaning the "highest" item is expected to be merged
            var expected = history.Max();
            var mergedValue = (ComplexObjectNullableFields)_merger.Merge(history);

            Assert.AreEqual(expected.NullableIntValue, mergedValue.NullableIntValue, "Nullible int value is incorrectly merged.");
            Assert.AreEqual(expected.NullableDoubleValue, mergedValue.NullableDoubleValue, "Nullible double value is incorrectly merged.");
            Assert.AreEqual(expected.NullableDateTimeValue, mergedValue.NullableDateTimeValue, "Nullible date time value is incorrectly merged.");
            Assert.AreEqual(expected.StringValue, mergedValue.StringValue, "String value is incorrectly merged.");
            Assert.AreEqual(expected.ByteArrayValue, mergedValue.ByteArrayValue, "ByteArray value is incorrectly merged.");
            Assert.AreEqual(expected.IntValue, mergedValue.IntValue, "Int value is incorrectly merged.");
            Assert.AreEqual(expected.DoubleValue, mergedValue.DoubleValue, "Double value is incorrectly merged.");
            Assert.AreEqual(expected.DateTimeValue, mergedValue.DateTimeValue, "DateTime value is incorrectly merged.");
        }

        [TestCaseSource(typeof(SequencedComplexItemBuilder), nameof(SequencedComplexItemBuilder.CreateMostRecentValueTestSet))]
        public void Merger_DefaultMergeCriteria_UsesNewValues(List<SequencedComplexItem> history)
        {
            // the merger will sort the objects, meaning the "highest" item is expected to be merged
            var expected = history.Max();
            var mergedValue = _merger.Merge(history, historyComparer: null);

            Assert.AreEqual(expected.StringValue, mergedValue.StringValue, "String value is incorrectly merged.");
        }

        [TestCaseSource(typeof(SequencedComplexItemBuilder), nameof(SequencedComplexItemBuilder.CreateMostRecentValueTestSet))]
        public void Merger_NeverOverwriteSequenceID1MergeCriteria_UsesSequenceID1Value(List<SequencedComplexItem> history)
        {
            // the merger will sort the objects, meaning the "highest" item is expected to be merged
            var expected = history.First();         // the builder always puts the first item with index 1
            var alwaysSequenceID1MergeCriteria = new SequencedComplexItemStringValueMergeCriteria_NeverOverwriteSequenceID1();
            var nonDefaultMergeCriteria = new List<IMergeCriteria>(new[] { alwaysSequenceID1MergeCriteria });
            var mergedValue = (SequencedComplexItem)_merger.Merge(history, nonDefaultMergeCriteria: nonDefaultMergeCriteria);

            Assert.AreEqual(expected.StringValue, mergedValue.StringValue, "String value is incorrectly merged.");
        }

        [Test]
        public void Merger_NeverOverwriteNullMergeCriteria_DoesntOverwriteWithNulls()
        {
            var sequence = SequencedComplexItemBuilder.GetNewNullValueSequence();
            string mergeCriteriaKey = BreadcrumbHelper<SequencedComplexItem>.Of(s => s.StringValue);
            var neverOverwriteNullCriteria = new NeverOverwriteOldWithNull(mergeCriteriaKey);
            var nonDefaultMergeCriteria = new List<IMergeCriteria>(new[] {neverOverwriteNullCriteria});

            var mergedValue = (SequencedComplexItem)_merger.Merge(sequence, nonDefaultMergeCriteria: nonDefaultMergeCriteria);

            Assert.AreEqual(sequence[0].StringValue, mergedValue.StringValue);
        }

        [Test]
        public void Merger_MergesNestedObjectValues()
        {
            // BuildSequence() sets the IntValue on the Child property
            var sequence = NestedObjectBuilder.BuildSequence(NestedObjectBuilder.Create(), NestedObjectBuilder.Create());
            var expected = sequence.Max();

            var merged = _merger.Merge(sequence);

            Assert.AreEqual(expected.Child.IntValue, merged.Child.IntValue);
        }

        [Test]
        public void Merger_MergesNestedObjectValues_WithNullNestedObjectForNewValue()
        {
            // BuildSequence() sets the IntValue on the Child property
            var secondValue = NestedObjectBuilder.Create();
            secondValue.Child = null;
            var sequence = NestedObjectBuilder.BuildSequence(NestedObjectBuilder.Create(), secondValue);
            var expected = sequence.Min();

            var merged = _merger.Merge(sequence);

            Assert.AreEqual(expected.Child.IntValue, merged.Child.IntValue);
        }
    }
}
