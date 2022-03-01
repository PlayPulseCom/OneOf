using System;
using System.Collections.Generic;
using System.Linq;
using static System.Linq.Enumerable;

namespace Generator
{
    public static class ImplicitHelper
    {
        public static string GenerateImplicitOperators(List<string> genericTargetArgs)
        {
            var combinations = Combinations(genericTargetArgs);
            return string.Join("\n\n", combinations
                .Where(x => x.Length > 0 && x.Length < genericTargetArgs.Count)
                .Select(genericSourceArgs => GenerateImplicitOperator(genericTargetArgs, genericSourceArgs.ToList()))
                .ToList());
        }
        
        private static string GenerateImplicitOperator(List<string> genericTargetArgs, List<string> genericSourceArgs)
        {
            var genericTargetArg = genericTargetArgs.Joined(", ");
            var genericSourceArg = genericSourceArgs.Joined(", ");
            return $@"
        public static implicit operator OneOf<{genericTargetArg}>(OneOf<{genericSourceArg}> o)
        {{
            return o.Match<OneOf<{genericTargetArg}>>(
                {string.Join(",\n                ", genericSourceArgs.Select(MatchArm).ToList())}
            );
        }}
";
        }

        private static string MatchArm(string genericArg) => $"{genericArg.ToLower()} => {genericArg.ToLower()}";
        
        private static IEnumerable<T[]> Combinations<T>(IEnumerable<T> source) {
            if (null == source)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var data = source.ToArray();

            return Enumerable
                .Range(1, 1 << (data.Length))
                .Select(index => data
                    .Where((v, i) => (index & (1 << i)) != 0)
                    .ToArray());
        }
    }
}