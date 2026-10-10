# M3-W01-C02 — bounded numerical matrix and vector contracts

M3-W01-C01 qualified Math.NET Numerics 5.0.0 **only as a managed numerical candidate on the tested runtime**. M3-W01-C02-T1 builds a new independent .NET 10 package `dw.tools.math.numerics` containing provider-neutral finite binary64 matrix and vector contracts. It does not place Math.NET in the public or transitive dependency graph of `dw.tools.math.numerics` or the existing IR/composition/quantity packages.

## Admitted operations and independent oracles

`FiniteMatrix64` is an immutable row-major rectangular matrix of finite binary64 values, with 1 to 4096 admitted cells. `Add` and `Multiply` preserve explicit shape semantics and decline non-finite results. Multiplication is limited to **65536 scalar products**, separately from the storage quota.

Two independent hand oracles:

- `[[1,2],[3,4]] + [[4,3],[2,1]] = [[5,5],[5,5]]`
- `[[1,2],[3,4]] × [[2,0],[1,2]] = [[4,4],[10,8]]`

`FiniteVector64` admits 1..4096 finite values, a shape-checked dot product, and matrix-vector multiplication; `[[1,2],[3,4]] × [5,6] = [17,39]`.

`SparseMatrix64` admits a bounded, canonical row-major ordering of unique, non-zero finite coordinate triples and an **explicit** conversion to dense. The initial sparse strategy does not claim arbitrary efficient sparse factorization or a CSR/CSC provider layout.

## Precision and provider boundary

Numerical values are explicitly `FiniteBinary64` and **approximate**, even when a particular input is an integer. `NumericalMatrixTransport` round-trips admitted `IrFiniteBinary64Scalar` matrix bits; an exact-rational IR input is rejected rather than silently widened. No general rational-to-double conversion or tolerance contract is adopted here. Those require separate T2 evidence.

`ToProviderArrayCopy` and `FromProviderArrayCopy` use independent arrays, not views. A separate test-only `MathNetInteropSmoke` project uses the already-qualified managed Math.NET 5.0.0 package to solve a 2×2 system with a residual oracle using **copied** arrays. The smoke's project dependency is test-only; the product NuGet manifest must contain solely `dw.tools.math.ir` as its direct dependency, with `dw.quantities` remaining transitive.

## Acceptance and nonclaims

The first T1 run observes a compiled RED for the missing public numerical matrix contract before installing the GREEN implementation. Seventeen independent tests cover hand-computable operations, vectors, invalid shapes, storage and multiplication budgets, sparse semantics, non-finite arithmetic, exact-versus-approximate IR and copy behavior; prior IR/composition/quantities/foundation tests, locked build, transfer architecture and local NuGet manifest are also requalified.

T1 is complete only after all those observations exist; **M3-W01-C02-T2 remains planned**, including fuller deep-copy/alias and explicit rational-to-double reporting, provider opt-in/load-time policies, precision and sparse algorithm integration. There is no native BLAS, symbolic solver, general-purpose numerical provider routing, AURA adoption, untested RID qualification, or claim of cross-platform bitwise floating reproducibility.
