# Bounded provider-neutral dense linear algebra — M3-W01-C03-T1

**Status: LU/QR and analytical 2x2 singular spectrum qualified by RS042. The RS043 scientific boundary extension is staged, not yet executed.**

## Contracts

The `dw.tools.math.numerics` 0.4.0-preview.4 candidate adds
`BoundedLinearAlgebra64` on existing immutable `FiniteMatrix64` and
`FiniteVector64`. All results are approximate finite IEEE 754 binary64, not
exact rational. There is no runtime product dependency on Math.NET or Aura.

- `FactorLu`: partial-pivoting `P*A = L*U`, owned triangular factors,
  source-row permutation, and explicit `Singular` / `BudgetExceeded` /
  `NonFiniteIntermediate` statuses.
- `Solve`: bounded LU forward/back substitution without computing
  `Inverse(A)`. `Success` returns a solution and an observed absolute
  infinity-norm residual plus optional normalized residual. Failure returns
  no solution; a small residual is **not** a general condition-number proof.
- `FactorQr`: full-Q Householder QR for `m >= n`, validating
  `Q*R ~= A` and `Q^T*Q ~= I`. Individual vector signs are not oracles.
  Unpivoted dependent columns report `RankDeficient`, not fake full rank.
- `SingularValues2x2`: the **two 2x2 singular values only**, with optional
  finite ratio. This is **not a full SVD**: no U/V vectors, pseudoinverse,
  arbitrary-size SVD, or universal rank/conditioning guarantee.

Admitted dimensions are at most 16 for LU and QR, with the existing
4096-cell parent matrix construction limit unchanged. Default relative
pivot/rank threshold is 1e-12 times the largest absolute matrix entry;
explicit thresholds are finite and in (0, 1e-6]. These are policies, not
absolute mathematical rank theorems. Nonfinite intermediates are refused
without partial results. All buffers are independent owned copies.

## Reproducible qualification gates

The controlled missing-contract RED must precede source installation.
Independent MSTest 4 cases cover the hand-checked
`[[2,1],[1,3]]*[1,2]=[4,7]` solve, 3x3 triangular solve, LU
permutation/reconstruction, QR full/rectangular reconstruction and
orthogonality, dependent columns, singular and inconsistent RHS,
threshold sensitivity, dimension caps, nonfinite intermediates,
and 2x2 spectrum analytical invariants. The optional managed
Math.NET 5.0.0 smoke corroborates LU/QR/2x2 full SVD **outside**
the public product assembly, pinned to the previously qualified provider.

`M3-W01-C03-T2` remains open after T1: strengthen rank/pivot policies,
ill-conditioned and provider boundaries, and determine any qualified
full-SVD adoption. Do not imply untested platforms, native BLAS,
`BrokerReady`, AURA invocation, or a `dw.discovery` implementation.

Discovery metadata and skill generation remain owned by the neutral
`dw.discovery` project, not implemented inside this numerical run.

## M3-W01-C03-T2 — condition availability and final admission boundary

**Targeted by RS043.** Before C03 and M3-W01 can close, the numerical
boundary run must show a controlled missing-contract RED, 16 targeted
GREEN cases, all prior numerics tests, provider-free packaging and the
existing independent managed Math.NET smoke. The final receipt determines
admission; this document alone cannot claim a successful test.

A successful finite 2x2 solve may now report
`LinearSolveResult64.TwoByTwoSpectralConditionEstimate`. It is a nullable,
approximate 2-norm condition ratio computed from the already admitted
`SingularValues2x2` operation, not an exact number or a universal bound.
The value is **null** when the solve is unsuccessful, the matrix is not
2x2, or the available finite spectrum cannot provide a finite ratio.
A successful solve with a small residual can therefore legitimately
report an *unknown* condition ratio.

The default LU pivot and unpivoted QR rank policies use a **relative
threshold of 1e-12 against the largest absolute input entry**, with
an explicit caller override bounded to (0, 1e-6]. Refusal under the
chosen threshold reports `Singular` for LU and `RankDeficient` for QR;
these are numerical **policy outcomes** for the finite data, not an
universal algebraic proof of exact rank. Exact singularity and numerical
ill-conditioning require separate reasoning. A successful triangular
solve uses no explicit inverse, and factor products, matrix-vector
residuals, and orthogonality are the independent oracles: individual
QR/SVD column signs, singular-vector direction, or provider ordering
are not acceptance criteria.

The 16x16 dense budget is independent of the larger matrix storage
budget. No provider assembly is required to load or invoke the public
Math numerical assembly: Math.NET 5.0.0 is an optional **separate
managed test consumer** pinned by prior evidence. No other native BLAS
or RID is inferred. The 2x2 spectrum remains **singular values only**;
full SVD factors, rank-revealing pivoted QR, pseudoinverses and high-
accuracy condition estimates for extreme scales are explicitly outside
this admitted Math surface and require a new independent qualification.

No `BrokerReady` capability, shared `dw.discovery` package, generated
skill, or real Aura execution is implied by the numerical work-package
closure. Interop and skill generation remain a separately owned,
qualified migration.
