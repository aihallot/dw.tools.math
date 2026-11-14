# Math Discovery — runnable selected CLR examples

These examples run inside the repository's independent source-project consumer. They are evidence for **selected CLR operations**, not invocation authorization or broker routing.

## Run

From the Math repository root, with the repository's pinned toolchain and required restore inputs:

```text
dotnet run --project tests/consumers/m8-discovery-clr-seed/m8-discovery-clr-seed.csproj -c Release
```

Source: [M8 Discovery CLR seed](../../../../tests/consumers/m8-discovery-clr-seed/Program.cs). This consumer uses project references; it is **not** evidence that a fresh external consumer installed the immutable private packages.

## Selected examples and asserted outcomes

| Selected ID | Code and evidence | Exact scope |
| --- | --- | --- |
| `math.quantity.exact-pipeline.evaluate` | The seed calls the selected static `ExactQuantityPipeline.Evaluate(IEnumerable<ExactPipelineStep>)` with four typed steps | `5 km / 2 min = 150 km/h` exactly; base `125/3 m/s`, correct dimension, four snapshots |
| `math.replay.try-evaluate` | The seed calls `ExactReplayRunner.TryRun` on those steps, twice with a bounded cache and then with changed assumptions | Final `Exact` receipts; same-context second call is a cache hit; changed assumptions yield a distinct key and cache miss |
| `math.ir.canonical.encode` | The seed encodes an `IrExactScalar(1/2)` and decodes the resulting canonical document | Exact numerator and denominator remain `1` and `2`; no binary-floating conversion |
| `math.ir.hash.semantic` | The seed hashes the exact node before and after canonical round-trip, and hashes identical symbols with different passive labels | Structural identity survives round-trip and passive-label changes; presentation hash distinguishes labels |

All four selected methods have typed source-project calls. This does **not** mean four independently installed package examples; the existing packaged verifier covers immutable `NUGET_LOCAL` archives and different invocation probes.

## Resource and authorization boundaries

- Input must be explicitly typed and dimensionally valid. The exact pipeline permits a maximum of 16 steps and 256 decimal digits per rational component, with zero-offset linear units only.
- Replay cache capacity is bounded to 1–32 entries. Cache identity includes context versions; the example uses local context labels and does not invoke a provider.
- `math.ir.hash.semantic` is structural, not algebraic equivalence. Exact and finite binary64 input contracts must not be conflated.
- All four are `AgentCallable` with `runtime-only` authorization. No item is `BrokerReady`; this reference grants no runtime permission, package installation, AURA adoption or publication.

## Qualification references

- Canonical source: `docs/discovery/qualification/m8-discovery-manifest.json`
- Bundled machine-readable copy: [manifest.json](manifest.json)
- Method signatures and limits: [selected-capabilities.md](selected-capabilities.md)
- Separated versions: [skill-version.json](skill-version.json)
- Existing independent example audit: `docs/discovery/qualification/m8-skill-example-coverage-audit.md` (historical assessment before positive Composition examples)
- Source consumer proof: `.aura/workflow/reports/RS204/attempt-001.json` and `.aura/workflow/reports/RS205/attempt-001.json`

No M8 acceptance transition is claimed by this reference alone.
