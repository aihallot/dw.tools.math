# Net10 Math package API, native and RID support matrix — RS077

This is an explicit evidence boundary, not a multi-platform release
announcement.

| Surface | Evidence | Excluded promise |
| --- | --- | --- |
| .NET 10 | Seven locally inspected managed IL-only net10 product DLLs | Other TFMs |
| Windows x64 | Prior RS076 ran win-x64 with SDK 10.0.402 | Other Windows architectures |
| Linux x64 / ARM64 | No RS077 Linux host test | Release support not qualified |
| macOS x64 / ARM64 | No RS077 macOS host test | Release support not qualified |
| Public source API | Current assembly public type/method/field SHA-256 | Cross-version source compatibility not qualified |
| Binary ABI | Current exact DLL SHA-256 and managed PE metadata | Cross-version ABI not qualified |
| Native assets | Foreign runtimes and native ZIP components refused | General OS loader behavior not certified |
| Resource budgets | NuGet <= 4 MiB, <= 256 ZIP entries, <= 16 MiB unpacked; <= 10,000 public members, <= 2 MiB signature bytes | No portable latency or throughput SLA |
| Local consumers | Pinned local NuGet source-free consumer and existing exact sample | Remote public package publication |
| Rights | README and owner notices included in distribution | No open-source license choice |
| Stable 1.0 | Not yet qualified | G-M5 and owner authorization required |

RuntimeInformation.RuntimeIdentifier identifies the host where the code
is actually run. It does not certify a matrix of other operating systems.
The RS077 qualification receipt records real execution RID, SDK,
seven independently verified current public API hashes and DLL hashes.
