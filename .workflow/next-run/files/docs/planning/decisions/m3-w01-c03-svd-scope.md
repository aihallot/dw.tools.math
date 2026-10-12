# M3-W01-C03 — bounded numerical factorization admission

**Decision target:** RS043, closing the M3-W01 numerical-provider and
matrix work package after independent boundary qualification.

## Observed prior authority

- C01 admitted the **managed** Math.NET.Numerics 5.0.0 provider on an
  observed Windows x64 host, not native BLAS or every platform.
- RS040/RS041 admitted provider-neutral dense/sparse matrices and explicit
  rational-to-binary64 loss reporting, with independent numerical tests.
- RS042 admitted partial-pivot LU, bounded dense solve with a residual,
  full-Q Householder QR, and **only the 2x2 singular-value spectrum**.
  The separately loaded managed-provider program performed a full SVD
  **as an independent oracle**, not as a public Math numerical API.

## Scientific admission decision

Keep the public stable scope of M3-W01 restricted to its independently
tested finite-binary64 contracts. RS043 may add an optional 2x2 spectral
condition estimate to a successful solve, but **must not** conflate it
with a universal spectral condition computation or a certified bound
on solution accuracy.

The default LU pivot / unpivoted QR rank policy is relative to the
largest entry magnitude, at a declared default factor 1e-12. Callers
may explicitly supply a finite factor in (0,1e-6]. These rules
distinguish a *numerically refused pivot* from an algebraic exact-rank
theorem. A cancellation-prone 2x2 spectrum, a nonfinite intermediate,
or any other unsupported shape produces an unavailable condition
estimate, not a manufactured finite value.

A small or even zero observed residual is necessary evidence of
reconstruction but does **not** prove low forward error for an
ill-conditioned input. No matrix inverse is formed in the dense solver.
LU `P*A=L*U`, QR `Q*R=A` and orthogonality, and residuals are the
stable independent oracles. Provider-specific vector ordering and
individual QR/SVD column signs must not be used as acceptance oracles.

## Deliberately not admitted

Full public U/S/V decomposition, singular-vector transport, sparse LU
solvers, rank-revealing pivoted QR, pseudoinverses, precision guarantees
on extreme scales, and a general n-dimensional condition-number API
remain outside the admitted bounded M3-W01 package. No symbolic
operation is implemented by this numerical work package.

These operations require separate product acceptance, a new bounded
RED/GREEN and numerical error/conditioning corpus, an exact package
and platform qualification, and any later shared `dw.discovery`
skill/agent metadata through the neutral project owner. The
independent Math.NET SVD smoke is not sufficient for this promotion.

**Review trigger:** a real Math, MCDM or Aura consumer requires
full-SVD factors, a wider dimension budget, robust rank decisions, or
explicit condition estimates beyond admitted 2x2 data.

**Nonclaims:** no public NuGet publication, unspecified RID,
`BrokerReady` or Aura invocation, Decision/MCDM repository edit,
or general all-mathematics coverage follows from RS043.
