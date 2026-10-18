# Exact signed integer and finite-sequence audit — local preview.2

Package: dw.tools.math.scientific 1.1.0-preview.2 (net10, BCL-only).

ExactScientificBoundaryAudit.GreatestCommonDivisor and
LeastCommonMultiple reject negative operands by default.
A caller can deliberately select SignedOperandPolicy.NormalizeMagnitude
and observe whether negative input magnitudes were normalized.
The original T1 methods never silently change signs.

Factorial and Binomial remain finite exact wrappers with no
negative-domain normalization. SumArithmeticSequence accepts
signed terms while bounding intermediate product sizes separately
from input/output bit limits and logical step budgets.

Example: gcd(-48,18) refuses by default; explicit normalization
yields 6. Lcm(-6,8) similarly yields 24 only when requested.
For first=1000, difference=-2000, count=2, the exact result is
zero from two eleven-bit intermediate products. An intermediate
limit of eight bits refuses, rather than mistaking cancellation
for small resource use.

All rejected operations return null numeric value/bit observation
and zero reported successful steps. BigInteger does not waive
finite budgets; primality and probabilistic-prime certainty
are outside this API.

Qualification requires one compiled public-API RED recovered,
87 independent GREEN, 70 retained T1 tests, full scientific
project validation, four Math regressions and two separate
eight-probe NuGet consumers verifying one actual SHA-inspected
local archive.

C01 may close upon its durable T2 receipt, but M6-W02/M6
remain open. No public NuGet, untested OS, CAS, agent, Decision
or MCDM integration is certified.
