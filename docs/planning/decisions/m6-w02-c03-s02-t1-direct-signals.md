# M6-W02-C03-S02-T1 — bounded direct DFT and linear convolution

## Scope and sequencing

RS095 independently qualifies only T1 of the existing S02 size-M child
within the original size-L M6-W02-C03 aggregate. Its dependency S01
finite Cartesian geometry is now durably qualified through RS094. The
scientific library remains BCL-only and exposes the new FiniteSignal64
facade in local dw.tools.math.scientific 1.1.0-preview.7, preserving
the previously qualified preview.6 baseline and all existing contracts.

## Admitted operations

- DirectDft operates on a caller-supplied array of complex binary64 samples
  (System.Numerics.Complex) with lengths from 1 to 32. It uses a direct
  quadratic DFT, not FFT or a signal library provider. Forward bins are
  X[k] = sum(n=0..N-1) x[n] * exp(-2*pi*i*k*n/N), with zero-based indexing.
  Inverse bins are x[n] = (1/N)*sum(k=0..N-1) X[k] * exp(+2*pi*i*k*n/N).
  Both produce complex arrays and explicitly record the inverse flag.
  No complex-to-real truncation or windowing is implicit.
- LinearConvolution operates on two real binary64 arrays, each with
  1..32 samples. It returns N+M-1 real samples with zero extension
  and y[k] = sum(i=max(0,k-M+1)..min(k,N-1)) a[i]*b[k-i].
  No modulo index, circular wrap, periodic extension, implicit padding
  to a DFT power of two or time-unit inference is performed.
- Input components must be finite with absolute magnitude at most 1e6.
  Intermediate/output components must be finite and at most 4e13.
  Maximum DFT bins 32; maximum linear convolution output 63.
  These are deliberate finite resource/domain budgets, not universal
  overflow/precision safety theorems.
- CancellationToken is the last public argument and is checked before
  input validation, during bounded loops and before return.
  Typed refusals InvalidLength, InvalidInput, OutsideQualifiedRange,
  NonFiniteResult and Cancelled return NO bins/samples or partial result.
- Success values are approximate binary64 observations; ErrorBoundGuaranteed
  is false, even for mathematically integral example outputs. Finite
  sampling does not establish alias-free reconstruction or physical units.

## Independent falsification

Compile one independent reflection-based RED test against preview.6 BEFORE
introducing the public facade. It must fail at runtime with the exact
missing-public-FiniteSignal64 assertion. A compiler error is not acceptable.
Then recover the RED and run 59 independent GREEN tests including:

- DFT([1,0,0,0]) approximately [1,1,1,1].
- Forward shifted impulse [0,1,0,0] yields approximately [1,-i,-1,+i].
- Inverse of [1,1,1,1] yields approximately [1,0,0,0].
- Convolution [1,2] * [1,1] produces [1,3,2] and output length 3.
- Selected 1/32-sample extremes, input refusals, length limits,
  inverse roundtrip, cancellation and prior-family regressions.

Require all 404 retained scientific tests (53 S01-T2, 25 S01-T1,
88 C02-T2, 80 C02-T1, 88 C01-T2 and 70 C01-T1),
four relevant Math regression projects, plan/native adoption and transfer
architecture validation. Additional scientific tests in the project must
remain green.

Pack a real local scientific 1.1.0-preview.7 NuGet archive, inspect its
net10 assembly/nuspec/dependencies and SHA-256 bytes, and qualify three
separately cached PackageReference-only eight-probe consumers restored from
a cleared local feed: new S02 direct-signal consumer, retained S01-T2
Euclidean boundary consumer and retained C02-T2 special-function consumer.
Require identical real archive byte length and SHA in all three receipts.

## Explicitly deferred

S02-T2 retains stronger DFT sign/inverse/aliasing, near-cancellation,
nonfinite/intermediate, budget and linear-versus-circular boundary audits.
The S02 phase, S03 uniform-series child, original C03 parent with its
original aggregate T1/T2, work package M6-W02 and milestone M6 remain open.
No public NuGet release, arbitrary sample count, certified error bound,
implicit time/sample units, FFT performance claim or AURA/Decision/MCDM
adoption follows from this controlled local task.
