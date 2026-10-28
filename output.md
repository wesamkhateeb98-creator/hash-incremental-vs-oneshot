# Benchmark Report: SHA-256 One-shot vs Incremental

```bash
cd src/HashBenchmark
dotnet run -c Release -- --messages-count 1000 --loop-counter 10
```

| Setting | Value |
|---|---|
| Algorithm | SHA-256 |
| Messages per batch | 1000 |
| Message size | 1024 B |
| Batch size | 1,024,000 B |
| Loops | 10 |
| Runtime | .NET 10.0.13 |
| Delay before loops | 2 seconds |
| Digest (both methods, every loop) | `2226b125c1ae5dc003e11cd918d20b90df8c4847c6a15299079bcae597d98554` |

## Summary (average of 10 loops)

| Method | Time (ms) | Allocated (B) | Batches/s | Messages/s | MB/s | Gen0 | Gen1 | Gen2 | GC pause (ms) |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| One-shot | 4.505 | 1,024,120 | 222 | 221,995 | 217 | 2 | 2 | 2 | 1.831 |
| Incremental | 4.088 | 276 | 245 | 244,612 | 239 | 0 | 0 | 0 | 0.000 |

> GC columns in the summary are **totals** over 10 loops; the other columns are averages.

| Method | Min time (ms) | Max time (ms) |
|---|---:|---:|
| One-shot | 3.366 | 5.764 |
| Incremental | 2.985 | 8.318 |

**Result:** Incremental is **1.10x** the speed of one-shot and allocates **3711x** less memory.

## Per loop

| Loop | Method | Time (ms) | Allocated (B) | Batches/s | Messages/s | MB/s | Gen0 | Gen1 | Gen2 | GC pause (ms) | Digest match |
|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---:|
| 1 | One-shot | 3.982 | 1,024,120 | 251 | 251,162 | 245 | 0 | 0 | 0 | 0.000 | ✅ |
| 1 | Incremental | 2.985 | 232 | 335 | 335,064 | 327 | 0 | 0 | 0 | 0.000 | ✅ |
| 2 | One-shot | 4.796 | 1,024,120 | 208 | 208,494 | 204 | 0 | 0 | 0 | 0.000 | ✅ |
| 2 | Incremental | 3.776 | 232 | 265 | 264,852 | 259 | 0 | 0 | 0 | 0.000 | ✅ |
| 3 | One-shot | 3.534 | 1,024,120 | 283 | 282,933 | 276 | 0 | 0 | 0 | 0.000 | ✅ |
| 3 | Incremental | 3.401 | 232 | 294 | 294,023 | 287 | 0 | 0 | 0 | 0.000 | ✅ |
| 4 | One-shot | 5.618 | 1,024,120 | 178 | 178,006 | 174 | 1 | 1 | 1 | 1.093 | ✅ |
| 4 | Incremental | 4.131 | 456 | 242 | 242,055 | 236 | 0 | 0 | 0 | 0.000 | ✅ |
| 5 | One-shot | 3.585 | 1,024,120 | 279 | 278,971 | 272 | 0 | 0 | 0 | 0.000 | ✅ |
| 5 | Incremental | 3.397 | 232 | 294 | 294,421 | 288 | 0 | 0 | 0 | 0.000 | ✅ |
| 6 | One-shot | 5.483 | 1,024,120 | 182 | 182,369 | 178 | 0 | 0 | 0 | 0.000 | ✅ |
| 6 | Incremental | 3.660 | 232 | 273 | 273,224 | 267 | 0 | 0 | 0 | 0.000 | ✅ |
| 7 | One-shot | 4.022 | 1,024,120 | 249 | 248,639 | 243 | 0 | 0 | 0 | 0.000 | ✅ |
| 7 | Incremental | 3.301 | 232 | 303 | 302,920 | 296 | 0 | 0 | 0 | 0.000 | ✅ |
| 8 | One-shot | 4.896 | 1,024,120 | 204 | 204,232 | 199 | 1 | 1 | 1 | 0.738 | ✅ |
| 8 | Incremental | 4.612 | 456 | 217 | 216,844 | 212 | 0 | 0 | 0 | 0.000 | ✅ |
| 9 | One-shot | 5.764 | 1,024,120 | 173 | 173,491 | 169 | 0 | 0 | 0 | 0.000 | ✅ |
| 9 | Incremental | 8.318 | 232 | 120 | 120,223 | 117 | 0 | 0 | 0 | 0.000 | ✅ |
| 10 | One-shot | 3.366 | 1,024,120 | 297 | 297,106 | 290 | 0 | 0 | 0 | 0.000 | ✅ |
| 10 | Incremental | 3.301 | 232 | 303 | 302,902 | 296 | 0 | 0 | 0 | 0.000 | ✅ |

## Notes

- **Gen2 GC in loops 4 and 8:** the one-shot 1 MB buffer goes on the Large Object Heap. When that heap fills up, a Gen2 collection runs, with pauses of 1.093 ms and 0.738 ms.
- **Incremental 456 B in loops 4 and 8:** small extra allocation in the same loops where the GC ran.
- **Loop 9, Incremental 8.318 ms:** an outlier caused by OS scheduling noise. Trust the average, not a single loop.
