using BenchmarkDotNet.Running;

namespace WordFinder.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            // --- TEXTO TEMPORAL PARA VER EL RESULTADO ---
            var test = new WordFinderBenchmark();
            test.Setup();

            Console.WriteLine("--- TOP 10 PALABRAS ENCONTRADAS ---");
            // Forzamos la ejecución para ver qué devuelve
            var resultadoVisible = test.TestFindPerformance();
            foreach (var palabra in resultadoVisible)
            {
                Console.WriteLine(palabra);
            }
            Console.WriteLine("-----------------------------------\n");
            // --------------------------------------------

            // Luego de ver que está bien, dejas correr el Benchmark formal
            BenchmarkRunner.Run<WordFinderBenchmark>();
        }
    }
}