namespace MergeO.Helpers
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// This class provides common reflection functionality.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Holds a reference to the <see cref="System.String"/> data type.
        /// </summary>
        private static readonly Type _stringType = typeof(string);

        /// <summary>
        /// Holds a reference to the <see cref="System.Byte"/> array type.
        /// </summary>
        private static readonly Type _byteArray = typeof(byte[]);

        /// <summary>
        /// Gets the name of the field, given an lambda.
        /// <example>
        /// <para>
        /// Usage: "return GetPropertyName(m => m.MyField);"
        /// </para>
        /// <para></para>
        /// <para>Returns: "MyField"</para>
        /// </example>
        /// </summary>
        /// <typeparam name="TModel">The type of model to get the property name for.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="modelProperty">The field expression.</param>
        /// <returns>The name of the field passed in.</returns>
        /// <exception cref="System.ArgumentNullException">modelProperty;Unable to get expression body. Please make sure the lambda passes in a model property.</exception>
        public static string GetPropertyName<TModel, TProperty>(Expression<Func<TModel, TProperty>> modelProperty)
        {
            var expression = ToMemberExpression(modelProperty);
            return expression.Member.Name;
        }

        /// <summary>
        /// This takes the expression given and confirms that it resolves to a proeprty value and returns
        /// the <see cref="MemberExpression"/> representing the property.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="modelProperty">The model property.</param>
        /// <returns>The <see cref="MemberExpression"/> representing the property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="modelProperty"/> - Unable to get expression body. Please make sure the lambda passes in a model property.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="modelProperty"/> - The expression must be a model property</exception>
        public static MemberExpression ToMemberExpression<TModel, TProperty>(Expression<Func<TModel, TProperty>> modelProperty)
        {
            // get the lambda details about the field
            MemberExpression expression = modelProperty.Body as MemberExpression;
            if (expression == null)
            {
                // the model property is possibly a value type so try as a unary expression
                UnaryExpression uExpress = modelProperty.Body as UnaryExpression;
                if (uExpress != null)
                {
                    expression = uExpress.Operand as MemberExpression;
                }
                if (expression == null)
                {
                    // checking the unary expression value didn't work either - there's nothing else we can do
                    throw new ArgumentNullException(nameof(modelProperty), "Unable to get expression body. Please make sure the lambda passes in a model property.");
                }
            }

            if (expression.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentOutOfRangeException(nameof(modelProperty), "The expression must be a model property");
            }

            return expression;
        }

        /// <summary>
        /// Gets the parent expression details.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        /// <returns>The parent expression details.</returns>
        public static ParameterExpression GetParentExpression(MemberExpression memberExpression)
        {
            if (memberExpression == null) throw new ArgumentNullException(nameof(memberExpression));

            var paramExpression = memberExpression.Expression as ParameterExpression;
            if (paramExpression == null)
            {
                paramExpression = GetParentExpression(memberExpression.Expression as MemberExpression);
            }

            return paramExpression;
        }

        #region Type extensions
        /// <summary>
        /// Returns whether or not the type is a <see cref="System.String"/> data type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the data type is a string, false otherwise.</returns>
        public static bool IsString(this Type type)
        {
            if (type == null) return false;

            return type.Equals(_stringType);
        }

        /// <summary>
        /// Determines whether the <see cref="Type"/> is a byte array.
        /// </summary>
        /// <param name="type">The type to interrogate.</param>
        /// <returns>
        ///   <c>true</c> if the type is a byte array; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsByteArray(this Type type)
        {
            if (type == null) return false;

            return type.Equals(_byteArray);
        }

        /// <summary>
        /// This returns whether or not the type is a value type or string data type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is a base type, false otherwise.</returns>
        public static bool IsBaseType(this Type type)
        {
            if (type == null) return false;
            return type.IsValueType || type.IsString();
        }

        /// <summary>
        /// Flags whether or not the data type is nullable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="underlyingType">The underlying data type.</param>
        /// <returns>True if the type is nullable, false otherwise.</returns>
        public static bool IsNullable(this Type type, out Type underlyingType)
        {
            underlyingType = null;
            if (type == null) return false;

            underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null;
        }

        /// <summary>
        /// Flags whether or not the data type is nullable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is nullable, false otherwise.</returns>
        public static bool IsNullable(this Type type)
        {
            Type underlyingType;
            return IsNullable(type, out underlyingType);
        }

        /// <summary>
        /// Gets the default value for the specified type.
        /// </summary>
        /// <param name="type">The type to get a defautl value for.</param>
        /// <returns>The default value for the specified type.</returns>
        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
        #endregion Type extensions

        #region PropertyInfo Extensions
        /// <summary>
        /// Returns whether or not the property type is a <see cref="System.String"/> data type.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>True if the property data type is a string, false otherwise.</returns>
        public static bool IsString(this PropertyInfo property)
        {
            if (property == null) return false;

            return property.PropertyType.IsString();
        }

        /// <summary>
        /// This returns whether or not the property type is a value type or string data type.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>True if the property type is a base type, false otherwise.</returns>
        public static bool IsBaseType(this PropertyInfo property)
        {
            if (property == null) return false;

            return property.PropertyType.IsBaseType();
        }

        /// <summary>
        /// Flags whether or not the property data type is nullable.
        /// </summary>
        /// <param name="property">The property type to check.</param>
        /// <param name="underlyingType">The underlying data type.</param>
        /// <returns>True if the property type is nullable, false otherwise.</returns>
        public static bool IsNullable(this PropertyInfo property, out Type underlyingType)
        {
            underlyingType = null;
            if (property == null) return false;
            return property.PropertyType.IsNullable(out underlyingType);
        }

        /// <summary>
        /// Flags whether or not the property data type is nullable.
        /// </summary>
        /// <param name="property">The property type to check.</param>
        /// <returns>True if the property type is nullable, false otherwise.</returns>
        public static bool IsNullable(this PropertyInfo property)
        {
            Type underlyType;
            return property.IsNullable(out underlyType);
        }
        #endregion

    }
}
