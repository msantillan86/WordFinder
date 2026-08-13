# 🧩 WordFinder - High Performance Challenge

A systems-level, high-performance solution developed in **.NET 10** to solve the classic word-search-in-a-grid challenge (up to 64 × 64).

This project follows a Zero Allocation philosophy (minimal heap allocations) and uses native hardware optimizations to process massive incoming word streams in sub-millisecond time slices.

---

## ⚡ Architecture & Performance Decisions

To achieve record processing speed, the design avoids common .NET memory-management bottlenecks:

- **Stack-based processing (`stackalloc` + `Span<char>`):** The class constructor precomputes vertical lines (columns) by transposing the matrix. Instead of creating intermediate arrays on the heap (which add pressure to the garbage collector), the transpose uses temporary memory allocated directly on the **stack**. Because the matrix is validated to be at most 64 × 64, this operation is safe and consumes only a few hundred bytes.
- **Early pruning:** When processing large streaming workloads (e.g., 10,000 words), the `Find` method immediately skips any word whose length exceeds the maximum matrix dimension (`word.Length > _maxWordLength`), saving millions of CPU cycles.
- **O(1) duplicate filtering:** Input stream duplicates are unified up-front using a `HashSet<string>`, ensuring each unique word is searched exactly once in the matrix.
- **Avoid LINQ on the hot path:** The internal search loop uses index-based `for` loops instead of LINQ sugar (e.g., `.Sum()`), preventing hidden allocations (delegates, enumerators) and maximizing iteration performance across horizontal and vertical lines.
- **Runtime-accelerated search:** The code uses `string.IndexOf` with `StringComparison.OrdinalIgnoreCase`. The .NET runtime optimizes this operation with native SIMD instructions where possible, comparing multiple characters in parallel.

---

## 📊 Performance Metrics (Benchmark Results)

Benchmarks were executed scientifically using **BenchmarkDotNet** in a controlled environment (Release mode, no attached debugger).

### Test scenario:
- **Matrix:** maximum allowed size (64 × 64).
- **Input stream:** a large stream of **10,000 words** with a high duplication rate.

Environment summary (example):
- BenchmarkDotNet v0.15.8
- Runtime: .NET 10.0.11
- OS: Windows 10

Example results:

| Method              | Mean     | Error   | StdDev  | Gen0    | Gen1    | Gen2    | Allocated |
|-------------------- |---------:|--------:|--------:|--------:|--------:|--------:|----------:|
| TestFindPerformance | 301.2 us | 5.86 us | 6.51 us | 49.8047 | 49.8047 | 49.8047 | 198.86 KB |

---

## 🛠 How to build & run

Requirements:
- .NET 10 SDK
- (Optional) Visual Studio 2026 or later with ASP.NET workload for the Blazor project

Build & run from the repository root:

1. Restore workloads and packages:

   dotnet workload restore
   dotnet restore

2. Build the solution (Release):

   dotnet build WordFinder.slnx -c Release

3. Run the Blazor WebAssembly demo project:

   dotnet run --project WordFinder.Web

Open the URL shown in the console (http://localhost:nnnn) and test the interactive demo.

---

## 🔗 Links

- LinkedIn: https://www.linkedin.com/in/santillanmatias/
- GitHub: https://github.com/msantillan86/WordFinder

---