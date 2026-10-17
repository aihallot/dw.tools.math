# M6-W01 — bounded local numerical-analysis work-package gate

## Scope and scientific decision

The three accepted phases C01 interpolation/regression, C02 bracketed
roots/Simpson integration and C03 central finite differentiation/scalar
RK4 are already durably complete. This run does not change mathematical
algorithms or fabricate another product task. Instead it independently
requalifies the final provider-neutral net10 Math analysis package,
locally versioned 1.1.0-preview.6, across all three families.

The work-package gate requires all six targeted T1/T2 test filters:
48 C01-T1, 60 C01-T2, 44 C02-T1, 51 C02-T2, 58 C03-T1 and
67 C03-T2 (328 total), the complete analysis test project and four
relevant Math regressions: numerics, IR, quantities and composition.

After a locked Release build, pack the actual local preview.6 archive
and inspect its assembly, nuspec, absence of external dependencies,
byte count and SHA-256. Three independent existing eight-probe
NuGet-only consumers (C01-T2, C02-T2, C03-T2) must be pinned to the
same preview.6 archive, restored from an isolated single local feed,
and prove identical package digest/size across all 24 science probes.
Do not infer additional proofs from consumer success.

## Boundaries and progress

Numerical unit symbols are caller-declared metadata, not automatic
physical conversion. Interpolation and OLS provide no forecast/causal
guarantee. Sign brackets do not prove continuity/root uniqueness,
Simpson refinement does not certify integral error, and difference/RK4
refinement does not prove smoothness, stability, event absence or
rigorous local/global-error bounds. Known stiff, event-driven and PDE
regimes stay outside the admitted differential subset.

Only M6-W01 transitions to done after this gate. The M6 release
remains in progress; M6-W02 scientific families are planned but
not implemented or qualified. Do not publish stable/public NuGet or
claim Linux/macOS, native provider, Aura, Decision or MCDM adoption.
