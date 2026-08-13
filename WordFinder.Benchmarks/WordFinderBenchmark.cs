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
            // 1. Creamos una matriz de 64x64 fija para el benchmark
            var matrix = new List<string>();
            string baseRow = "CHILLCOLDWINDPAPASETOCASAMARTEPLAYAGATOZORROLUNAAVIONDEDOTECHO"; // 64 chars
            for (int i = 0; i < 64; i++)
            {
                matrix.Add(baseRow);
            }

            _finder = new Application.WordFinder(matrix);

            // 2. Simulamos un stream gigante (10,000 palabras) con alta repetición
            _wordStream = new List<string>();
            string[] palabrasEjemplo = { "CHILL", "COLD", "WIND", "CASA", "LUNA", "PLAYA", "NO_EXISTE", "LARGA_QUE_NO_ENTRA_EN_MATRIZ_64" };

            var rand = new Random(42);
            for (int i = 0; i < 10000; i++)
            {
                _wordStream.Add(palabrasEjemplo[rand.Next(palabrasEjemplo.Length)]);
            }
        }

        [Benchmark]
        public List<string> TestFindPerformance()
        {
            return _finder.Find(_wordStream).ToList();
        }
    }
}
