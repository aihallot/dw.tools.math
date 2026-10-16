# Math net10 prerelease support evidence — T1

Status: local qualification only. This table is an evidence distinction,
not an operating-system support announcement.

| Surface | T1 qualification | Limit |
| --- | --- | --- |
| .NET target | net10.0 in seven locally inspected nupkg assemblies | Does not prove any operating system RID |
| Windows x64 | win-x64 observed in RS075 with .NET SDK 10.0.402 | Reobserve host/SDK in RS076 receipt |
| Linux x64/arm64 | Not independently certified | Needs real test host and full restore/run |
| macOS x64/arm64 | Not independently certified | Needs real test host and full restore/run |
| Local NuGet clean consumer | Planned real RS076 proof | No ProjectReference, single pinned package |
| Documentation notices | New distribution README and NOTICE packed | Does not license external software |
| CLI CI recipe | Locked .NET build plus focused test/consumer commands | No GitHub Actions workflow claimed |
| Native dependencies | No external CAS or agent dependencies admitted | Full RID/native audit awaits T2 |
| Public stable 1.0 | Not admitted | Requires C02-T2, C03 and owner authorization |

Actual run-time platform, SDK version, seven archive hashes and local restore
results belong in RS076 scientific receipts, not this static plan.
