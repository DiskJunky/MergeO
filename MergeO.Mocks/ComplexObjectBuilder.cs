namespace MergeO.Mocks
{
    using System;
    using System.Collections.Generic;
    using MergeO.Mocks.Models;

    /// <summary>
    /// This class is used for generating complex objects with particular property values.
    /// </summary>
    public static class ComplexObjectBuilder
    {
        public static ComplexObjectNullableFields GetDefaultsWithNulls()
        {
            return new ComplexObjectNullableFields
            {
                IntValue = 1,
                DoubleValue = 1d,
                DateTimeValue = new DateTime(2000, 1, 1)
            };
        }

        public static ComplexObjectNullableFields GetDefaultAllFieldsFilled()
        {
            var obj = GetDefaultsWithNulls();
            obj.StringValue = "1";
            obj.NullableIntValue = 1;
            obj.NullableDoubleValue = 1d;
            obj.NullableDateTimeValue = new DateTime(2000, 1, 2);
            obj.ByteArrayValue = new byte[] { 0x00 };
            return obj;
        }

        public static List<ComplexObjectNullableFields[]> GetComplexObjectNullibleFieldSet()
        {
            var histories = new List<ComplexObjectNullableFields[]>();

            histories.Add(new [] { GetDefaultsWithNulls(), GetDefaultAllFieldsFilled() } );
            histories.Add(new[] { GetDefaultAllFieldsFilled(), GetDefaultsWithNulls() });

            return histories;
        }
    }
}
