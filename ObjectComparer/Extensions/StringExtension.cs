using System;
using System.Collections.Generic;
using System.Text;

namespace SimilarObjectComparer.Extensions
{
    public static class StringExtension
    {
        public static bool IsEqualTo(this string current, string second, bool ignoreCase = true)
        {
            var comparision = ignoreCase
                ? StringComparison.InvariantCultureIgnoreCase
                : StringComparison.InvariantCulture;

            return string.Compare(current, second, comparision) == 0;
        }
    }
}
