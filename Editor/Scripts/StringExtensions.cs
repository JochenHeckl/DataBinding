using System;

namespace JH.DataBinding
{
    /// <summary>
    /// This implementation is based on (Damerau–Levenshtein distance)[https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance]
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// This implementation is based on <see href="https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance">Damerau–Levenshtein distance</see>
        /// </summary>
        public static int DamerauLevenshteinDistance(this string one, string other)
        {
            var distanceTable = new int[one.Length + 1, other.Length + 1];

            for (var idxOne = 0; idxOne <= one.Length; idxOne++)
            {
                distanceTable[idxOne, 0] = idxOne;
            }

            for (var idxOther = 0; idxOther <= other.Length; idxOther++)
            {
                distanceTable[0, idxOther] = idxOther;
            }

            for (var idxOne = 1; idxOne <= one.Length; idxOne++)
            {
                for (var idxOther = 1; idxOther <= other.Length; idxOther++)
                {
                    int cost = (one[idxOne - 1] == other[idxOther - 1]) ? 0 : 1;

                    var deletionCost = distanceTable[idxOne - 1, idxOther] + 1;
                    var insertionCost = distanceTable[idxOne, idxOther - 1] + 1;
                    var substitutionCost = distanceTable[idxOne - 1, idxOther - 1] + cost;

                    distanceTable[idxOne, idxOther] = Math.Min(
                        Math.Min(deletionCost, insertionCost),
                        substitutionCost
                    );

                    if (
                        (idxOne > 1)
                        && (idxOther > 1)
                        && (one[idxOne - 1] == other[idxOther - 2])
                        && (one[idxOne - 2] == other[idxOther - 1])
                    )
                    {
                        var transpositionCost = distanceTable[idxOne - 2, idxOther - 2] + 1;

                        distanceTable[idxOne, idxOther] = Math.Min(
                            distanceTable[idxOne, idxOther],
                            transpositionCost
                        );
                    }
                }
            }

            return distanceTable[one.Length, other.Length];
        }
    }
}
