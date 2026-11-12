# .NET 11 and C# 15 — expanded numerics impact on dw.tools.math

**Status:** research and modernization assessment, not code migration. **Owner scope:** M8-W02-C01, independent of AURA. **Baseline:** Math main `668136168bcb47ac2edbc07682753199396aed35`, backlog `0.1.123`. This plan keeps 13 existing .NET 10 packages and all prior receipts immutable.

## Primary Microsoft authorities

- [Release announcement, 10 November 2026](https://devblogs.microsoft.com/dotnet/announcing-dotnet-11/) — .NET 11 GA, STS support and runtime improvements.
- [Libraries in .NET 11](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/libraries) — decimal IEEE 754 types, generic Complex, partial parsing, conversion, random and vector APIs.
- [Performance improvements in .NET 11](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-11/) — BigInteger representation, JIT, SIMD and measured microbenchmarks.
- [C# 15 language changes](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15) — unions, closed hierarchies and other refactoring candidates.

The .NET 11 general availability announcement is newer than preview wording that may remain in individual documentation pages. Validate concrete API signatures against the **installed GA SDK and reference assemblies**, not snippets alone.

## Highest-value impact candidates

| Rank | Microsoft feature | Math opportunity | Required protection |
|---|---|---|---|
| 1 | `BigInteger` internal 64-bit limb changes | Benchmark `ExactRational` gcd, parsing, multiplication, comparison and canonicalization under the new runtime without rewriting the exact core | Numeric identity, bounded sizes and cross-platform variation |
| 2 | `Decimal32`, `Decimal64`, `Decimal128` | Define **new opt-in approximate decimal scalars**, conversion policies, serialization and precision information | These IEEE decimal floats are **not** exact rationals and not a drop-in for `decimal` |
| 3 | Generic `Complex<T>` | Assess math-numerics generic complex/finite scientific support with explicit domain and precision | Branch cuts, generic constraints, provider interoperability |
| 4 | Correctly rounded conversions and partial numeric parsing | Audit `dw.quantities.expression`, IR and codecs for safe delegation to framework APIs | Preserve exact grammar, canonical text and accepted decimal/binary interpretation |
| 5 | SIMD cross-lane APIs, FP16, JIT and vector optimization | Measure finite vector/matrix kernels, reductions and memory layout | CPU fallback, deterministic tolerance and regressions |
| 6 | Generic `Random` sampling methods | Consider typed statistical sampling where semantics are useful | Seeded reproducibility, distribution contracts and bounds |
| 7 | C# 15 unions / closed hierarchies and JSON support | Assess typed IR result/status modeling and exhaustive cases | Existing serialized IR schema, semantic hashes, AOT and compatibility |
| 8 | Numeric serialization and expanded BCL | Reduce redundant math-codecs infrastructure where independently equivalent | Byte-for-byte canonical output and invalid-input refusal |

The structured and testable candidate matrix is [the associated JSON proposal](net11-numerics-impact-matrix.json).

## Source-grounded starting points

`src/projects/dw.quantities/ExactRational.cs` currently uses `BigInteger`, `GreatestCommonDivisor`, `Pow`, `DivRem` and exact decimal parsing. These are concrete audit targets. **Do not** replace `ExactRational` with `Decimal128`: its exactness and canonical denominator contract must survive.

`src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs` currently serializes explicit exact-rational and binary64 scalar cases in the bounded `math-ir/1` envelope. Union syntax or new numeric JSON converters must not silently rewrite that wire contract.

All remaining packages are **candidates for file/API inventory**, not yet inspected source-level findings in this seed: `dw.quantities.expression`, `dw.quantities.standard`, foundation, composition, numerics, statistics, symbolics, codecs, distribution, analysis and scientific.

## Recommended implementation order within M8-W02

1. **C01 / research and inventory**: enumerate current CLR contracts and framework calls across 13 packages, test projects and source-free consumers; verify GA .NET 11 API signatures and compiler semantics.
2. **C01 / differential oracles**: define exact/decimal/complex/FP16/SIMD test corpora with explicit rounding, tolerances, branch cuts, NaN, signed zero, overflow, cancellation and budgets.
3. **C01 / business-value decision**: classify each proposal as runtime-only gain, API refactor, additive adapter, new versioned type or deferred, with expected benefit and compatibility cost.
4. **C02 / measured code changes**: apply the smallest selected improvement on an independent new .NET 11 package/target line, with no migration of historical immutable archives.
5. **C02 / qualification**: repeat focused tests, benchmark against .NET 10, qualify the actual package graph in a clean source-free consumer and record SDK/RID observations.

The first Discovery sequence M8-W01 retains its agreed priority; numerical Discovery descriptions must remain accurate if M8-W02 later admits new scalar types.

## Explicit nonclaims

No improvement magnitude for Math has been measured. No GA SDK is assumed to be installed on the operator machine. No new numeric type, generic complex implementation, SIMD path, generated skill, package release or other-OS compatibility is claimed delivered by this research run.
