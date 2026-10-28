# Benchmark Report: SHA-256 One-shot vs Incremental

```bash
dotnet run -c Release -- --messages-count 1000 --loop-counter 10
```

| Setting | Value |
|---|---|
| Algorithm | SHA-256 |
| Messages per batch | 1,000 |
| Message size | 1,024 B |
| Batch size | 1,024,000 B |
| Loops | 10 |
| Runtime | .NET 10.0.13 |
| Delay before loops | 2 seconds |
| Digest | `2226b125c1ae5dc003e11cd918d20b90df8c4847c6a15299079bcae597d98554` |

## Per loop

| Loop | Method | Time (ms) | Allocated (B) | Batches/s | Messages/s | MB/s | GC pause (ms) | Digest match |
|---:|---|---:|---:|---:|---:|---:|---:|:---:|
| 1 | One-shot | 3.803 | 1,024,120 | 263 | 262,957 | 257 | 0.000 | ✅ |
| 1 | Incremental | 3.469 | 232 | 288 | 288,268 | 282 | 0.000 | ✅ |
| 2 | One-shot | 4.207 | 1,024,120 | 238 | 237,722 | 232 | 0.000 | ✅ |
| 2 | Incremental | 3.357 | 232 | 298 | 297,894 | 291 | 0.000 | ✅ |
| 3 | One-shot | 4.080 | 1,024,120 | 245 | 245,068 | 239 | 0.000 | ✅ |
| 3 | Incremental | 3.994 | 232 | 250 | 250,357 | 244 | 0.000 | ✅ |
| 4 | One-shot | 4.696 | 1,024,120 | 213 | 212,965 | 208 | 0.841 | ✅ |
| 4 | Incremental | 3.170 | 456 | 315 | 315,477 | 308 | 0.000 | ✅ |
| 5 | One-shot | 3.757 | 1,024,120 | 266 | 266,177 | 260 | 0.000 | ✅ |
| 5 | Incremental | 3.561 | 232 | 281 | 280,828 | 274 | 0.000 | ✅ |
| 6 | One-shot | 3.252 | 1,024,120 | 308 | 307,503 | 300 | 0.000 | ✅ |
| 6 | Incremental | 3.417 | 232 | 293 | 292,612 | 286 | 0.000 | ✅ |
| 7 | One-shot | 3.323 | 1,024,120 | 301 | 300,924 | 294 | 0.000 | ✅ |
| 7 | Incremental | 3.399 | 232 | 294 | 294,178 | 287 | 0.000 | ✅ |
| 8 | One-shot | 4.345 | 1,024,120 | 230 | 230,165 | 225 | 1.051 | ✅ |
| 8 | Incremental | 3.375 | 456 | 296 | 296,288 | 289 | 0.000 | ✅ |
| 9 | One-shot | 5.655 | 1,024,120 | 177 | 176,844 | 173 | 0.000 | ✅ |
| 9 | Incremental | 7.005 | 232 | 143 | 142,757 | 139 | 0.000 | ✅ |
| 10 | One-shot | 3.809 | 1,024,120 | 263 | 262,564 | 256 | 0.000 | ✅ |
| 10 | Incremental | 3.012 | 232 | 332 | 331,972 | 324 | 0.000 | ✅ |

## Summary (average of 10 loops)

| Method | Time (ms) | Allocated (B) | Batches/s | Messages/s | MB/s | GC pause (ms) |
|---|---:|---:|---:|---:|---:|---:|
| One-shot | 4.093 | 1,024,120 | 244 | 244,349 | 239 | 1.892 |
| Incremental | 3.776 | 276 | 265 | 264,831 | 259 | 0.000 |

> GC pause is the **total** over 10 loops; the other columns are averages.

| Method | Min time (ms) | Max time (ms) |
|---|---:|---:|
| One-shot | 3.252 | 5.655 |
| Incremental | 3.012 | 7.005 |

**Result:** Incremental is **1.08x** the speed of one-shot and allocates **3711x** less memory.
