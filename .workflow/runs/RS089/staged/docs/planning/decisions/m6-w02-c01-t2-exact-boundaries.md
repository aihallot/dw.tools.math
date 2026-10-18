# M6-W02-C01-T2 — explicit signed-domain and intermediate-bit auditing

## Scope and accepted boundary policy

The qualified T1 methods of the independent net10 BCL-only
dw.tools.math.scientific package keep their signatures and behavior.
Local preview 1.1.0-preview.2 adds ExactScientificBoundaryAudit.

SignedOperandPolicy.RejectNegative stays the default for GCD/LCM,
preserving the T1 refusal for any negative operand.
NormalizeMagnitude is an explicit caller policy. It applies
BigInteger.Abs to signed Euclidean inputs before invoking the
qualified T1 implementation; NegativeOperandsNormalized is true
only if normalization was needed and the calculation succeeded.
This is an integer sign convention, not inference or unit conversion.

Factorial and Binomial retain strict nonnegative bounded domains.
SumArithmeticSequence still accepts signed first terms and
differences and now checks MaximumIntermediateBits separately
(default 4096; hard ceiling 16384) for both exact products and
their sum before accepting the bounded T1 result. Final output
bits, operand bits and logical step budgets remain independent.
An exactly zero output can still be refused if its intermediate
products exceed the declared maximum. Example: first=1000,
difference=-2000, count=2 yields products +2000 and -2000,
both eleven bits in magnitude. A cap of eight rejects the
operation with ResultTooLarge even though the final sum is zero.

On invalid mode, invalid intermediate size, negative domain,
operand/output/step budget refusal or cancellation the API returns
a typed nonpartial refusal: null Value, null observed intermediate
bit count, zero successful steps and no normalized-input claim.
The wrapper does not relax T1 hard limits or provide unlimited
BigInteger performance or resource guarantees.

There is no probabilistic primality API or claim. Known algorithms
are exact within their bounded admitted scopes, not universal
integer mathematics, a CAS, or a decision model.

## Independent falsification and distribution

Observe a genuinely compiled missing-public-sidecar RED on the
qualified T1 package before staging new product code. Recover
this RED and pass 87 independent GREEN MSTests and all 70
retained T1 MSTests. Also run the full scientific test project
and four relevant Math regression projects in locked Release.

Pack the actual net10 preview.2 archive to a single isolated
local NuGet feed. Inspect binary, nuspec/dependencies, byte
count and SHA-256. Run two independently restored source-free
eight-probe consumers (new T2 and retained T1) against the
exact same archive bytes and compare SHA/byte receipts.

On durable success, close C01-T2 R/G/V and phase C01 only.
M6-W02 and M6 remain in progress; special functions C02 awaits
its own qualification. No stable/public NuGet, nonobserved OS,
foreign provider or AURA/Decision/MCDM adoption is asserted.
