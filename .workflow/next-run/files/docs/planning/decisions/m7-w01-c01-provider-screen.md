# M7 precision / interval provider screen — research only

**Scope:** M7-W01-C01-T1 documentary and two independent BCL numerical oracles. No external installation, NuGet download, provider binary inspection, license grant, benchmark or new public API is authorized.

## Existing contracts before considering a new provider

- `src/projects/dw.quantities/ExactRational.cs`: exact integer-ratio arithmetic with already qualified budgets; existing functionality is not an arbitrarily precise transcendental evaluator.
- `src/projects/dw.quantities/ExactBinaryNumber.cs`: represent a received IEEE 754 binary floating-point value exactly; the transport value `double 0.1` is not the decimal rational `1/10`.
- M6 `dw.tools.math.analysis` and `dw.tools.math.scientific`: finite numerical kernels with explicit caller budgets, refusal statuses and local preview package evidence. They do not claim certified interval enclosures or physical uncertainty inference.

## Candidate strategy screen

| Strategy | Potential value | Cost / verification debt | Disposition now |
|---|---|---|---|
| BCL `BigInteger` and existing `ExactRational` | Exact integer/rational oracle and bounded input validation; no additional runtime provider | Intermediate size and execution limits; no transcendental or directed-rounded interval arithmetic | **Reuse for fixtures only**, not a new production precision engine |
| .NET `decimal` fixed-scale arithmetic | Useful finite decimal representation for explicitly bounded decimal cases | Finite exponent/precision, overflow and rounding remain; not arbitrary real precision | **Not a substitute** for arbitrary precision or certified intervals |
| Third-party managed arbitrary-precision real | Possible variable precision and portable managed arithmetic | Confirm precise library/version/source/license, numerical semantics, error budgets, allocation, independent oracles and offline .NET smoke | **Not admitted**; evidence pending |
| Managed interval arithmetic with proved directed bounds | Potential inclusion/rounding semantics for a limited operation set | Inspect implementation, exact outward rounding for each operation, NaN/infinity/domain refusals, trusted assumptions and adversarial oracle corpus | **Not admitted**; no provider pin |
| Native MPFR / Arb class of providers | Possible high-precision or rigorous interval backends | Native assets/RID, binary redistribution and license notices, dependency chain, toolchain, FFI isolation and secure loading; independent source-free consumers | **Not admitted**; native adoption out of scope |

These are capability *categories*, not evaluated product recommendations. No package capability, project health, license or compatibility is asserted without source-backed evidence.

## Independent numerical reference cases

1. `(10000000000000000 + 1) - 10000000000000000 = 1` exactly over integers, while current binary64 evaluation loses the unit and returns `0`; at the large operand the next representable double is `2` away. This illustrates cancellation and representability, not proof of a general condition-number bound.
2. `double 0.1` has exact binary rational `3602879701896397 / 36028797018963968`. The previous representable double lies strictly below `1/10`; exact cross-multiplication verifies `BitDecrement(0.1) < 1/10 < 0.1`. This is one reference enclosure, not universal directed rounding or measurement uncertainty.

The local C# witness is `docs/planning/ValidateM7PrecisionAdmission.cs`, intentionally independent of Math product code.

## Admission questions for T2

Require: an identified external consumer and expected operation/domain; fixed and dynamic precision policy; cancellation and conditioning assessment; exact oracles and interval inclusion adversarial tests; time, allocation and iteration budgets; correct cancellation/no-partial-result behavior; overflow/invalid-domain/NaN/branch handling; stable provider API/commit and license/transitive/notices; `net10.0` RID qualification, architecture isolation and offline rollback; whether a third-party engine earns its maintenance cost.

Decision rule: **defer unless the owner admits a bounded operation set with a reproducible provider qualification and independent source-free consumer**. A deferral must include an explicit review trigger (documented consumer gap, provider evidence, or a failed existing exact/binary64 contract) and must not be advertised as an implemented function. Do not infer sensor error bars or statistical confidence intervals from machine epsilon.

**Resource stop-loss:** no broad provider benchmark, internet scraping or release on T1; one read-only BCL oracle program with two reference cases and structural DWF validation. Later extension requires separate DWF authority.
