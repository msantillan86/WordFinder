namespace WordFinder.Application
{
    public class WordFinder
    {
        private const int MaxMatrixSize = 64;
        private readonly string[] _horizontalLines;
        private readonly string[] _verticalLines;
        private readonly int _maxWordLength;

        public WordFinder(IEnumerable<string> matrix)
        {
            if (matrix == null || !matrix.Any())
            {
                _horizontalLines = _verticalLines = [];
                return;
            }

            _horizontalLines = ProcessAndValidateMatrix(matrix, out int colCount);
            _maxWordLength = Math.Max(_horizontalLines.Length, colCount);
            _verticalLines = TransposeColumns(_horizontalLines, colCount);
        }

        public IEnumerable<string> Find(IEnumerable<string> wordstream)
        {
            if (wordstream == null || _horizontalLines.Length == 0)
                return [];

            var wordCounts = CountWordOccurrences(wordstream);

            return wordCounts
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .Take(10);
        }

        #region Helper Methods
        private Dictionary<string, int> CountWordOccurrences(IEnumerable<string> wordstream)
        {
            var uniqueWords = new HashSet<string>(wordstream, StringComparer.OrdinalIgnoreCase);
            var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (string word in uniqueWords)
            {
                if (string.IsNullOrEmpty(word) || word.Length > _maxWordLength) continue;

                int occurrences = 0;
                ReadOnlySpan<char> wordSpan = word.AsSpan();

                for (int i = 0; i < _horizontalLines.Length; i++)
                    occurrences += CountSubstrings(_horizontalLines[i].AsSpan(), wordSpan);

                for (int i = 0; i < _verticalLines.Length; i++)
                    occurrences += CountSubstrings(_verticalLines[i].AsSpan(), wordSpan);

                if (occurrences > 0)
                    wordCounts[word] = occurrences;
            }

            return wordCounts;
        }

        private static string[] ProcessAndValidateMatrix(IEnumerable<string> matrix, out int colCount)
        {
            if (matrix is ICollection<string> col && col.Count > MaxMatrixSize)
            {
                throw new ArgumentException($"Matrix dimensions must not exceed {MaxMatrixSize}x{MaxMatrixSize}.");
            }
            var tempHorizontal = new string[MaxMatrixSize];

            int rowCount = 0;
            colCount = -1;

            foreach (string row in matrix)
            {
                if (rowCount >= MaxMatrixSize)
                {
                    throw new ArgumentException($"Matrix dimensions must not exceed {MaxMatrixSize}x{MaxMatrixSize}.");
                }

                if (row == null)
                {
                    throw new ArgumentException("The matrix cannot contain null rows.");
                }

                if (colCount == -1)
                {
                    colCount = row.Length;
                    if (colCount > MaxMatrixSize)
                    {
                        throw new ArgumentException($"Matrix dimensions must not exceed {MaxMatrixSize}x{MaxMatrixSize}.");
                    }
                }
                else if (row.Length != colCount)
                {
                    throw new ArgumentException("All rows in the matrix must have the same length.");
                }

                tempHorizontal[rowCount++] = row;
            }

            var finalMatrix = new string[rowCount];
            Array.Copy(tempHorizontal, finalMatrix, rowCount);
            return finalMatrix;
        }

        private static string[] TransposeColumns(string[] horizontalLines, int colCount)
        {
            int rowCount = horizontalLines.Length;
            var verticalLines = new string[colCount];

            for (int c = 0; c < colCount; c++)
            {
                Span<char> colBuffer = stackalloc char[rowCount];
                for (int r = 0; r < rowCount; r++)
                {
                    colBuffer[r] = horizontalLines[r][c];
                }
                verticalLines[c] = new string(colBuffer);
            }

            return verticalLines;
        }

        private static int CountSubstrings(ReadOnlySpan<char> source, ReadOnlySpan<char> value)
        {
            int count = 0;

            while (true)
            {
                int index = source.IndexOf(value, StringComparison.OrdinalIgnoreCase);
                if (index == -1) break;

                count++;
                source = source.Slice(index + 1);
            }
            return count;
        }
        #endregion
    }
}