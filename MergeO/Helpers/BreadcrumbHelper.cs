namespace MergeO.Helpers
{
    using System;
    using System.Linq.Expressions;
    using System.Text;

    /// <summary>
    /// This class is used to help with the creation of breadcrumb strings from an object graph.
    /// </summary>
    public static class BreadcrumbHelper
    {
        /// <summary>
        /// The separator used to denote different segments of the path.
        /// </summary>
        public const string PathSeparator = ".";

        /// <summary>
        /// This appends the specified segment to the existing <paramref name="source"/> path.
        /// </summary>
        /// <param name="source">The source path to append to.</param>
        /// <param name="segments">The segments to append.</param>
        /// <returns>The fully qualified breakcrumb.</returns>
        public static string Append(this string source, params string[] segments)
        {
            if (source == null)
            {
                source = string.Empty;
            }

            var sb = new StringBuilder(source);
            foreach (string s in segments)
            {
                if (string.IsNullOrWhiteSpace(s)) continue;

                sb.AppendFormat("{0}{1}", PathSeparator, s);
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// This class is used to help with the creation of breadcrumb strings from an object graph.
    /// </summary>
    public static class BreadcrumbHelper<T>
    {
        /// <summary>
        /// This converts a proprty lamda to a normalized object graph breadcrumb.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="modelProperty">The model property.</param>
        /// <returns>The breadcrumb for the property path.</returns>
        public static string Of<TProperty>(Expression<Func<T, TProperty>> modelProperty)
        {
            var memberExpression = ReflectionHelper.ToMemberExpression(modelProperty);
            var parentExpression = ReflectionHelper.GetParentExpression(memberExpression);

            // get the default breadcrumb
            string breadcrumb = memberExpression.ToString();

            // replace the source lambda variable name with the data type
            string lambdaName = parentExpression.Name;
            string lamdaTypeName = parentExpression.Type.Name;
            breadcrumb = lamdaTypeName + breadcrumb.Substring(lambdaName.Length);

            // replace the separators with the breadcrumb equivalent
            return breadcrumb.Replace(".", BreadcrumbHelper.PathSeparator);
        }
    }
}
