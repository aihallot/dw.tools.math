# Exact symbolic transformations — local Math preview.3

This is a provider-independent, deliberately narrow set of exact algebraic operations. No AngouriMath, MathNet.Symbolics, Python, AURA or external solver is a required product runtime.

Proved forms:
- Expand (x+a)^2 into x^2+2a*x+a^2 for one free real IR symbol x and bounded exact rational a.
- Factor that exact, canonical three-term shape back into (x+a)^2, refusing a mismatched square coefficient.
- Substitute only a matching free real symbol within a restricted closed exact IR syntax, refusing bound variables and any source restriction that might otherwise be silently dropped.
- Normalize sqrt(x^2) to abs(x) over reals. Return x only under a typed nonnegative assumption, carried in an IrRestrictedExpression.

Provenance is a fixed local rule identifier, not an external CAS proof. All transformations return statuses, and invalid/unrecognized shapes are refused. Existing typed IR conditions must be preserved through the canonical math-ir/1 codec; no unchecked text, principal complex branch or implicit binary64 is admitted.

Qualification requires a genuine compiled absent-contract RED, 28 independent GREEN tests and eight local NuGet-only consumer probes, followed by the three generated local product package identities and post-packaging SHA-256 receipts. These packages are not publicly distributed and actual archive digests may differ between builds.

The tested subset intentionally does not include general capture-avoiding substitution under binders: bound variables are rejected rather than renamed. General polynomial factorization, algorithmic simplification, arbitrary domains, all contradictory assumptions, resource-policy enforcement and translation of foreign provider output remain out of scope.

The next accepted boundary M4-W01-C03-T2 must independently test these explicit deferrals and either admit a separately proven implementation or retain typed unsupported statuses. A successful RS057 does not complete the phase or the M4 release.
