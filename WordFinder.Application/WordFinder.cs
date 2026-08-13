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

            // 1. Procesamos y validamos el stream horizontal en un solo paso
            _horizontalLines = ProcessAndValidateMatrix(matrix, out int colCount);

            _maxWordLength = Math.Max(_horizontalLines.Length, colCount);

            // 2. Transponemos las columnas al Stack de forma segura
            _verticalLines = TransposeColumns(_horizontalLines, colCount);
        }

        public IEnumerable<string> Find(IEnumerable<string> wordstream)
        {
            if (wordstream == null || _horizontalLines.Length == 0)
                return [];

            // Eliminamos duplicados en O(1)
            var uniqueWords = new HashSet<string>(wordstream, StringComparer.OrdinalIgnoreCase);
            var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (string word in uniqueWords)
            {
                if (string.IsNullOrEmpty(word) || word.Length > _maxWordLength) continue;

                int occurrences = 0;

                for (int i = 0; i < _horizontalLines.Length; i++)
                {
                    occurrences += CountSubstrings(_horizontalLines[i], word);
                }

                for (int i = 0; i < _verticalLines.Length; i++)
                {
                    occurrences += CountSubstrings(_verticalLines[i], word);
                }

                if (occurrences > 0)
                    wordCounts[word] = occurrences;
            }

            return wordCounts
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .Take(10);
        }

        #region Helper Methods Privados

        private static string[] ProcessAndValidateMatrix(IEnumerable<string> matrix, out int colCount)
        {
            int estimatedRows = matrix is ICollection<string> col ? col.Count : MaxMatrixSize + 1;
            if (estimatedRows > MaxMatrixSize && matrix is ICollection<string>)
                throw new ArgumentException($"Matrix dimensions must not exceed {MaxMatrixSize}x{MaxMatrixSize}.");

            var tempHorizontal = new string[Math.Min(estimatedRows, MaxMatrixSize + 1)];
            int rowCount = 0;
            colCount = -1;

            foreach (string row in matrix)
            {
                if (rowCount >= MaxMatrixSize)
                    throw new ArgumentException($"Las dimensiones de la matriz no pueden superar {MaxMatrixSize}x{MaxMatrixSize}.");

                if (row == null)
                    throw new ArgumentException("La matriz no puede contener filas nulas.");

                if (colCount == -1)
                {
                    colCount = row.Length;
                    if (colCount > MaxMatrixSize)
                        throw new ArgumentException($"Matrix dimensions must not exceed {MaxMatrixSize}x{MaxMatrixSize}.");

                    if (tempHorizontal.Length > MaxMatrixSize) tempHorizontal = new string[MaxMatrixSize];
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

        private static int CountSubstrings(string source, string value)
        {
            int count = 0;
            int index = 0;

            while ((index = source.IndexOf(value, index, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                count++;
                index++;
            }

            return count;
        }

        #endregion
    }
}