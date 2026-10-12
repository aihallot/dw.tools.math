# Bounded provider-neutral dense linear algebra — M3-W01-C03-T1

**Status: RS042 candidate; not a delivered capability until DWF succeeds.**

## Contracts

The `dw.tools.math.numerics` 0.4.0-preview.3 candidate adds
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
