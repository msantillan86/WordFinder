using BenchmarkDotNet.Attributes;

namespace WordFinder.Benchmarks
{
    [MemoryDiagnoser]
    public class WordFinderBenchmark
    {
        private Application.WordFinder _finder;
        private List<string> _wordStream;

        [GlobalSetup]
        public void Setup()
        {
            // 1. Create a fixed 64x64 matrix for the benchmark
            var matrix = new List<string>();
            string baseRow = "CHILLCOLDWINDPAPASETOCASAMARTEPLAYAGATOZORROLUNAAVIONDEDOTECHO"; // 64 chars
            for (int i = 0; i < 64; i++)
            {
                matrix.Add(baseRow);
            }

            _finder = new Application.WordFinder(matrix);

            // 2. Simulate a large word stream (10,000 words) with high repetition
            _wordStream = new List<string>();
            string[] exampleWords = { "CHILL", "COLD", "WIND", "CASA", "LUNA", "PLAYA", "NOT_FOUND", "TOO_LONG_FOR_64_MATRIX" };

            var rand = new Random(42);
            for (int i = 0; i < 10000; i++)
            {
                _wordStream.Add(exampleWords[rand.Next(exampleWords.Length)]);
            }
        }

        [Benchmark]
        public List<string> TestFindPerformance()
        {
            return _finder.Find(_wordStream).ToList();
        }
    }
}
