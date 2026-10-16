# M5-W02-C01-T1 — exact numerical and symbolic public console vertical

## Independent accepted recipe

The public .NET 10 library dw.tools.math.distribution exposes
ExactMathVerticalRunner.Run for a tightly bounded family: canonical
where(sub(square(x),c),ne(x,k)), with real free x, exact rationals c,k,
and one original domain exclusion. The sample:
where(sub(square(x),1),ne(x,1)) at x=2, without AURA or external CAS.

The runner reuses, without duplicating implementations:
- CanonicalMathNotation.Parse for typed Math IR and original assumption.
- SymbolicExactDifferentiation.TryDifferentiate on the original BODY only,
  and preserves x!=1 separately; the derivative is 2x, NOT an equation solver.
- SymbolicExactPolynomialCalculus.FiniteLimit for exact point value 3,
  promoted to polynomial point evaluation only after checking original domain.
- SymbolicExactRealSolving.Solve(x²=1) for the complete unrestricted candidate
  list [-1,1]; SymbolicRootAudit.Verify on the ORIGINAL restricted x²-1=0
  admits -1, excludes 1, and does not falsely claim a complete original audit.
- RationalBinary64Conversion.TryConvert under RequireExact default, or an
  explicitly chosen AllowRounded policy with exact absolute rounding error.
- MathOperationResult.Exact / Approximate to retain different compositional
  precision types even when binary64 represents the integer 3 exactly.
- IrCanonicalJsonCodec and semantic SHA-256 for roundtrip typed original IR
  and no silent change of assumptions or identities.

Passing a derivative or other IR in place of the original equation to the
root stage returns IncompatiblePipeline without any mathematical result.
Excluded evaluation points, unsupported shapes, irrational root candidate
families, noncanonical source, invalid policy and unauthorized rounding also
return typed non-admission with no partial success. The exact proof is limited
to the stated providers, grammar and original-domain conditions.

## Console and release boundaries

samples/m5-exact-vertical is a runnable .NET 10 console that has a single
**pinned NuGet PackageReference** to the locally packed distribution library.
It prints a deterministic JSON summary and has eight standalone probes:
parse/replay, derivative, exact evaluation, original roots, explicit binary64,
pipeline incompatibility, rounding consent and foreign dependency checks.

Before adding the runner, a truly compiled absent-public-runner MSTest RED
must be observed against the new distribution contract type. All GREEN tests
and focused core regressions must pass. The new distribution assembly and
its six versioned local Math dependencies are independently packed, checked
for actual net10 assembly/nuspec, byte sizes and SHA-256, and restored through
a local-only isolated NuGet feed. No stable 1.0 publication or cross-OS
matrix is implied.

This is T1 only. M5-W02-C01-T2 will independently verify unsupported
integration/pipeline/provider/assumption/loss boundaries. C01, W02 and M5
remain open; public remote publication and sibling-agent adoption await gates.
