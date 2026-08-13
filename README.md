# 🧩 WordFinder - High Performance Challenge

Una solución de nivel de ingeniería de sistemas y alta performance desarrollada en **.NET 10** para resolver el clásico desafío de búsqueda de palabras en una matriz de caracteres ($64 \times 64$). 

Este proyecto fue diseñado bajo la premisa de **Zero Allocation** (mínima asignación en el Heap) y optimización de hardware nativa para procesar flujos masivos de datos (*large word streams*) en fracciones de milisegundo.

---

## ⚡ Decisiones de Arquitectura y Rendimiento

Para batir récords de velocidad en el procesamiento, el diseño esquiva los cuellos de botella tradicionales de la gestión de memoria en .NET:

*   **Procesamiento en el Stack (`stackalloc` + `Span<char>`):** El constructor de la clase precalcula las líneas verticales (columnas) transponiendo la matriz. En lugar de crear arrays intermedios en el Heap que sobrecarguen al *Garbage Collector*, la transposición se realiza en bloques de memoria temporal directamente en el **Stack**. Al estar garantizado por validación que la matriz es de máximo $64 \times 64$, esta operación es 100% segura y consume apenas 128 bytes.
*   **Poda Temprana (*Early Pruning*):** En flujos de streaming masivos (ej. 10,000 palabras), el método `Find` descarta instantáneamente cualquier palabra cuya longitud supere el tamaño máximo de la matriz (`word.Length > _maxWordLength`), ahorrando millones de ciclos de reloj innecesarios en la CPU.
*   **Filtrado O(1) de Duplicados:** Siguiendo la regla de negocio del enunciado, los duplicados del flujo de entrada se unifican al inicio mediante un `HashSet<string>`, garantizando que cada palabra única se busque exactamente una sola vez en la matriz.
*   **Eliminación de LINQ en el "Hot Path":** El bucle principal de búsqueda interna evita el azúcar sintáctico de LINQ (`.Sum()`) y utiliza bucles `for` tradicionales indexados, eliminando por completo la alocación oculta de delegados y optimizando el recorrido de los vectores horizontales y verticales.
*   **Búsqueda Vectorizada por Hardware:** Se utiliza `string.IndexOf` con `StringComparison.OrdinalIgnoreCase`. A nivel interno, el Runtime de .NET optimiza esta operación utilizando instrucciones **SIMD** (Single Instruction Multiple Data), comparando múltiples caracteres en paralelo a nivel de hardware.

---

## 📊 Métricas de Rendimiento (Benchmark Resultados)

Las pruebas de rendimiento fueron ejecutadas de forma científica utilizando **BenchmarkDotNet** en un entorno controlado (Modo Release, sin debugger acoplado). 

### Escenario del Test:
*   **Matriz:** Tamaño máximo permitido ($64 \times 64$).
*   **Stream de entrada:** Flujo masivo de **10.000 palabras** con alta tasa de duplicados.

```text
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045)
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400

| Method              | Mean     | Error   | StdDev  | Gen0    | Gen1    | Gen2    | Allocated |
|-------------------- |---------:|--------:|--------:|--------:|--------:|--------:|----------:|
| TestFindPerformance | 301.2 us | 5.86 us | 6.51 us | 49.8047 | 49.8047 | 49.8047 | 198.86 KB |
