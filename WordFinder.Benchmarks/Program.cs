using BenchmarkDotNet.Running;

namespace WordFinder.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            // --- TEMPORARY OUTPUT TO INSPECT RESULTS ---
            var test = new WordFinderBenchmark();
            test.Setup();
            Console.WriteLine("--- TOP 10 WORDS FOUND ---");
            // Force execution to inspect the result
            var visibleResult = test.TestFindPerformance();
            foreach (var word in visibleResult)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine("-----------------------------------\n");
            // --------------------------------------------

            // After verifying the output above, run the formal benchmark
            BenchmarkRunner.Run<WordFinderBenchmark>();
        }
    }
}