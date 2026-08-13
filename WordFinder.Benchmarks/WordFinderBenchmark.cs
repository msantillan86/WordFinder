using BenchmarkDotNet.Attributes;

namespace WordFinder.Benchmarks
{
    [MemoryDiagnoser]
    [RankColumn]
    public class WordFinderBenchmark
    {
        private Application.WordFinder _finder;
        private List<string> _wordStream;

        [Params(100, 1000, 10000)]
        public int WordStreamSize { get; set; } = 100; 

        [GlobalSetup]
        public void Setup()
        {
            var rand = new Random(42);
            var matrix = new List<string>();

            for (int i = 0; i < 64; i++)
            {
                char[] row = new char[64];
                for (int j = 0; j < 64; j++)
                {
                    row[j] = (char)rand.Next('A', 'Z' + 1);
                }
                matrix.Add(new string(row));
            }

            matrix[0] = "CHILL" + matrix[0].Substring(5);
            matrix[1] = "CASA" + matrix[1].Substring(4);
            matrix[2] = "PLAYA" + matrix[2].Substring(5);
            matrix[3] = "PERRO" + matrix[3].Substring(5);

            char[] filaVertical = matrix[10].ToCharArray(); filaVertical[10] = 'G'; matrix[10] = new string(filaVertical);
            filaVertical = matrix[11].ToCharArray(); filaVertical[10] = 'A'; matrix[11] = new string(filaVertical);
            filaVertical = matrix[12].ToCharArray(); filaVertical[10] = 'T'; matrix[12] = new string(filaVertical);
            filaVertical = matrix[13].ToCharArray(); filaVertical[10] = 'O'; matrix[13] = new string(filaVertical);
            // ----------------------------------------------------------------------------------

            _finder = new Application.WordFinder(matrix);

            _wordStream = new List<string>(WordStreamSize);
            for (int i = 0; i < WordStreamSize; i++)
            {
                int wordLength = rand.Next(4, 9);
                char[] word = new char[wordLength];
                for (int j = 0; j < wordLength; j++)
                {
                    word[j] = (char)rand.Next('A', 'Z' + 1);
                }
                _wordStream.Add(new string(word));
            }

            for (int i = 0; i < 15; i++) _wordStream.Add("CHILL");
            for (int i = 0; i < 10; i++) _wordStream.Add("CASA");
            for (int i = 0; i < 8; i++) _wordStream.Add("PLAYA");
            for (int i = 0; i < 5; i++) _wordStream.Add("PERRO");
            for (int i = 0; i < 3; i++) _wordStream.Add("GATO");
        }

        [Benchmark]
        public List<string> TestFindPerformance()
        {
            return _finder.Find(_wordStream).ToList();
        }
    }
}