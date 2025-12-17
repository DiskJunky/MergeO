namespace MergeO.Mocks.Models
{
    using MergeO.Helpers;
    using System;
    using System.Linq;
    using System.Reflection;

    public class AllSupportedPropertyTypesObject : IComparable
    {
        public string StringValue { get; set; }

        public int IntValue { get; set; }

        public int? NullableIntValue { get; set; }

        public double DoubleValue { get; set; }

        public double? NullableDoubleValue { get; set; }

        public DateTime DateTimeValue { get; set; }

        public DateTime? NullableDateTimeValue { get; set; }

        public byte[] ByteArrayValue { get; set; }

        /// <summary>
        /// Sets the property defaults on the current instance.
        /// </summary>
        public void SetPropertyDefaults()
        {
            Type type = GetType();
            foreach (PropertyInfo property in type.GetProperties())
            {
                object defaultValue = property.PropertyType.GetDefault();
                property.SetValue(this, defaultValue);
            }
        }

        public int CompareTo(object obj)
        {
            var typedObj = (AllSupportedPropertyTypesObject)obj;
            int result = string.Compare(StringValue, typedObj.StringValue);
            if (result == 0)
            {
                result = IntValue.CompareTo(typedObj.IntValue);
            }
            if (result == 0)
            {
                result = Nullable.Compare(NullableIntValue, typedObj.NullableIntValue);
            }

            if (result == 0)
            {
                result = DoubleValue.CompareTo(typedObj.DoubleValue);
            }

            if (result == 0)
            {
                result = Nullable.Compare(NullableDoubleValue, typedObj.NullableDoubleValue);
            }

            if (result == 0)
            {
                result = DateTimeValue.CompareTo(typedObj.DoubleValue);
            }

            if (result == 0)
            {
                result = Nullable.Compare(NullableDateTimeValue, typedObj.NullableDateTimeValue);
            }

            if (result == 0)
            {
                // TODO: this is more correctly done by using SequenceCompareTo()
                //       method but this is only available from Standard 2.1 onwards.
                //       Use basic custom implementation for backward compatibility.
                //result =  ByteArrayValue.SequenceEqual(typedObj.ByteArrayValue);
            }

            return result;
        }
    }
}
