# RS126 — Math seven-package offline graph and public API fingerprint matrix

**Static source baseline:** RS125 durable Math `main` result `30448ab546e21deb4032acfb018df3f6c996c989`. Version metadata is **declared**, not a new archive or NuGet.org release. All packages below are net10.0 and will be freshly packed in RS126.

## Closed dependency and package roster

| PackageId | Declared version | Direct Math dependencies |
|---|---|---|
| `dw.quantities` | `0.2.0-preview.1` | none |
| `dw.tools.math.ir` | `0.3.0-preview.1` | `dw.quantities` |
| `dw.tools.math.codecs` | `1.0.0-preview.8` | `dw.tools.math.ir` |
| `dw.tools.math.symbolics` | `0.5.0-preview.10` | `dw.tools.math.ir` |
| `dw.tools.math.numerics` | `0.4.0-preview.4` | `dw.tools.math.ir` |
| `dw.tools.math.composition` | `0.3.0-preview.1` | `dw.quantities`, `dw.tools.math.ir` |
| `dw.tools.math.distribution` | `1.0.0-preview.4` | `dw.quantities`, `dw.tools.math.ir`, `dw.tools.math.codecs`, `dw.tools.math.symbolics`, `dw.tools.math.numerics`, `dw.tools.math.composition` |

The distribution package includes a README and NOTICE; this is required evidence of **present documents**, not an endorsement of any license transfer or public rights decision. No AURA, Decision, MCDM, MathNet or arbitrary external provider is a package dependency.

## Independent current consumer

`tests/consumers/m5-w02-c02-t2/m5-managed-api-boundary-consumer.csproj` references only the exact `dw.tools.math.distribution [1.0.0-preview.4]` NuGet package, with no Math ProjectReference. Its `NuGet.Config` clears remote sources.

The program accepts seven archive paths in the order above from ignored `build/artifacts/m5-w02-c02-t2-local-feed`. It runs **eight** BCL and Math API/runtime/math/refusal probes; emits seven `M5-W02-C02-T2-SURFACE-SHA256` records containing observed public API and managed DLL SHA-256; then emits:

`dw.tools.math/m5-managed-api-boundary-consumer/1.0 qualified; probes=8`

RS126 independently records seven whole-ZIP SHA-256 digests and sizes through the BCL read-only package checker. The consumer verifies each `.nupkg` and its direct dependency contracts, including the expected net10 assembly and absence of unsupported native/other-RID or general cross-version guarantees.

## Historical versus fresh and release restrictions

| Authority | Already verified | New RS126 claim only if successful |
|---|---|---|
| M5 gate | Historical seven local packages and three prior source-free consumers | Seven **fresh** current-source archives and one freshly executed eight-probe consumer |
| RS124 | Full solution tests and three representative archives; **first-pass NO** due to one pre-attempt refusal | Do not relabel prior history |
| RS125 | Two independent consumers, 16 probes, **first-pass YES** | Preserve as separate observed analysis/scientific evidence |
| DWF Math | 541 nodes all done; product backlog `0.1.122` done | No new progress, no invented milestone, zero completion achievements |
| Public and external | M7 research/provider deferral and three **unsent** owner requests | No public release, NuGet.org push, rights promotion, recipient acceptance, generated Discovery skill or cross-platform certification |

The RS126 source-pinned receipt and validator must distinguish a **current API surface SHA** from a cross-version ABI compatibility guarantee. A source-free consumer proves that consumer's eight behaviors against these archives on its observed environment, not every mathematical operation, package-version combination or operating system.

Read-only local gate: `dotnet run --file docs/planning/ValidatePostM7DistributionGraph.cs -- --check`. Package byte inspection: same script with `--packages`. No Python, GitHub CI, foreign packages or outgoing requests.
