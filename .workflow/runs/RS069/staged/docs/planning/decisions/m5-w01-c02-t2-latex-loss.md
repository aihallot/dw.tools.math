# M5-W01-C02-T2 — Bounded LaTeX loss provenance and closure

## Scope and earlier authority

T1 qualified a strict, non-executing LaTeX subset in
BoundedLatexImporter, local codecs preview.3. M5-W01-C02-T2
must preserve semantic-loss information, adversarial input
boundaries, and honest import limitations. Do not extend the
underlying parser, execute TeX, or imply complete LaTeX support.

The existing M2 typed Math IR JSON transport and M5-W01-C01
canonical display audit are qualified independent oracles.
T2 may use them without owning a general symbolic CAS.

## Contract

A new Math-only .NET 10 API
LatexImportBoundaryAudit.Audit(string, LatexJuxtapositionConvention)
delegates to the T1 importer. It records:

- The exact qualified importer status (Admitted, Ambiguous,
  NonCanonical, Invalid, Unsupported, BudgetExceeded).
- A separate reason, LatexImportLossKind, distinguishing
  same-notation identity, passive presentation change, an
  explicitly required multiplication convention, ambiguous
  source semantics, noncanonical rational, unsupported shape,
  invalid syntax, resource limits and unverified structural
  identity.
- The SHA-256 of bounded original UTF-8 source bytes, even on
  refusal. This binds the evidence to original input but is
  NEVER a mathematical proof or a warrant to interpret TeX.
  Oversized source has no fingerprint to bound memory costs.
- Only upon admission: existing typed source IR, canonical
  text, semantic structural SHA-256, a versioned audit rule,
  effective caller convention and semantic fidelity evidence.
- Never claims LatexInverseQualified: importing an exact subset
  is not a general inverse to the Math display-only renderer.

The audit independently encodes and decodes the imported Math IR
using the existing canonical JSON codec. It compares canonical
text and structural SHA-256 to the original import, then checks
the existing safe Math display audit. A failed cross-check
refuses with no expression, semantic hash or proof rule.
This is structural fidelity, not an arbitrary algebraic proof.

For x(y), default import remains Ambiguous. With an explicit
Multiplication convention, the audit reports
ExplicitMultiplicationConvention. Other formulas imported with
that same caller preference do NOT spuriously claim to have
needed it. Distinct square(x) and pow(x,2) remain
non-invertibly ambiguous from their identical LaTeX spelling.

## Test and distribution acceptance

Observe one real compiled public-API RED against the prior
codecs preview.3 before installing the T2 source. Recover
that test and run 46 independent targeted GREEN MSTests
covering exact fractions, variables, expression structure,
passive reformat, caller convention, source SHA-256, IR JSON
roundtrip, conflicting square/power representations, hostile
TeX macros, noncanonical rationals, syntax, Unicode, and
bounded input/depth/bit-length refusals.

Run the locked .NET 10 Release solution build, focused full
codecs tests, and relevant IR/quantities regression tests.
Repack the three pinned local Math packages and inspect
actual net10 assemblies, nuspec dependency identities,
archive byte sizes and SHA-256 with a separate C# helper.
Eight isolated NuGet-only consumer probes must succeed with
exactly one pinned codecs preview.4 PackageReference and
transitive IR/quantities without project or external CAS
runtime dependencies.

Upon durable success close only the three T2 child nodes,
T2, and M5-W01-C02. Keep M5-W01 and M5 in-progress;
M5-W01-C03 remains not-ready pending its own acceptance.

## Nonclaims

No arbitrary TeX input, expansion, execution, secure TeX
sandbox, universal sanitizer, independent LaTeX inverse,
semantic algebraic equivalence, arbitrary syntax repair,
implicit binary64 conversion, unknown variable namespace,
generic CAS, public NuGet, stable 1.0 promotion, AURA,
Decision/MCDM integration or dw.discovery skill is admitted.
