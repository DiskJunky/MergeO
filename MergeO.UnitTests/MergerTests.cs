using MergeO.MergeCriteria;

namespace MergeO.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Helpers;
    using MergeCriteria;
    using Mocks;
    using Mocks.MergeCriteria;
    using Mocks.Models;
    using NUnit.Framework;

    [TestFixture]
    public class MergerTests
    {
        private readonly Merger _merger = new();

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
            Assert.That(max, Is.EqualTo(mergedValue));
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
            Assert.That(expected.Moment, Is.EqualTo(mergedValue.Moment));
        }

        [TestCaseSource(typeof(ComplexObjectBuilder), nameof(ComplexObjectBuilder.GetComplexObjectNullibleFieldSet))]
        public void Merger_DefaultMergeCriteria_UsesNewValues(ComplexObjectTestCriteria criteria)
        {
            // the merger will sort the objects, meaning the "highest" item is expected to be merged
            var expected = criteria.Expected;
            var mergedValue = _merger.Merge(criteria.History);

            Assert.That(expected.NullableIntValue, Is.EqualTo(mergedValue.NullableIntValue), "Nullible int value is incorrectly merged.");
            Assert.That(expected.NullableDoubleValue, Is.EqualTo(mergedValue.NullableDoubleValue), "Nullible double value is incorrectly merged.");
            Assert.That(expected.NullableDateTimeValue, Is.EqualTo(mergedValue.NullableDateTimeValue), "Nullible date time value is incorrectly merged.");
            Assert.That(expected.StringValue, Is.EqualTo(mergedValue.StringValue), "String value is incorrectly merged.");
            Assert.That(expected.ByteArrayValue, Is.EqualTo(mergedValue.ByteArrayValue), "ByteArray value is incorrectly merged.");
            Assert.That(expected.IntValue, Is.EqualTo(mergedValue.IntValue), "Int value is incorrectly merged.");
            Assert.That(expected.DoubleValue, Is.EqualTo(mergedValue.DoubleValue), "Double value is incorrectly merged.");
            Assert.That(expected.DateTimeValue, Is.EqualTo(mergedValue.DateTimeValue), "DateTime value is incorrectly merged.");
        }

        [TestCaseSource(typeof(SequencedComplexItemBuilder), nameof(SequencedComplexItemBuilder.CreateMostRecentValueTestSet))]
        public void Merger_DefaultMergeCriteria_UsesNewValues(SequencedComplexItemTestCriteria criteria)
        {
            // the merger will sort the objects, meaning the "highest" item is expected to be merged
            var expected = criteria.Expected;
            var mergedValue = _merger.Merge(criteria.SequencedComplexItem, historyComparer: null);

            Assert.That(expected.StringValue, Is.EqualTo(mergedValue.StringValue), "String value is incorrectly merged.");
        }

        [TestCaseSource(typeof(SequencedComplexItemBuilder), nameof(SequencedComplexItemBuilder.CreateMostRecentValueTestSet))]
        public void Merger_NeverOverwriteSequenceID1MergeCriteria_UsesSequenceID1Value(SequencedComplexItemTestCriteria criteria)
        {
            // the merger will sort the objects, meaning the "highest" item is expected to be merged
            var expected = criteria.SequencedComplexItem.First(s => s.SequenceID == 1);
            var alwaysSequenceID1MergeCriteria = new SequencedComplexItemStringValueMergeCriteria_NeverOverwriteSequenceID1();
            var nonDefaultMergeCriteria = new List<IMergeCriteria>(new[] { alwaysSequenceID1MergeCriteria });
            var mergedValue = _merger.Merge(criteria.SequencedComplexItem, nonDefaultMergeCriteria: nonDefaultMergeCriteria);

            Assert.That(expected.StringValue, Is.EqualTo(mergedValue.StringValue), "String value is incorrectly merged.");
        }

        [Test]
        public void Merger_NeverOverwriteNullMergeCriteria_DoesntOverwriteWithNulls()
        {
            var sequence = SequencedComplexItemBuilder.GetNewNullValueSequence();
            string mergeCriteriaKey = BreadcrumbHelper<SequencedComplexItem>.Of(s => s.StringValue);
            var neverOverwriteNullCriteria = new NeverOverwriteOldWithNull(mergeCriteriaKey);
            var nonDefaultMergeCriteria = new List<IMergeCriteria>(new[] { neverOverwriteNullCriteria });

            var mergedValue = _merger.Merge(sequence, nonDefaultMergeCriteria: nonDefaultMergeCriteria);

            Assert.That(sequence[0].StringValue, Is.EqualTo(mergedValue.StringValue));
        }

        [Test]
        public void Merger_MergesNestedObjectValues()
        {
            // BuildSequence() sets the IntValue on the Child property
            var sequence = NestedObjectBuilder.BuildSequence(NestedObjectBuilder.Create(), NestedObjectBuilder.Create());
            var expected = sequence.Max();

            var merged = _merger.Merge(sequence);

            Assert.That(expected.Child.IntValue, Is.EqualTo(merged.Child.IntValue));
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

            Assert.That(expected.Child.IntValue, Is.EqualTo(merged.Child.IntValue));
        }
    }
}
