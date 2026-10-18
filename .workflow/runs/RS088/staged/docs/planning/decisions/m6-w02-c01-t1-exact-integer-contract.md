# M6-W02-C01-T1 — exact finite integer, combinatorial and sequence contract

## Qualified candidate and scope

Introduce a new independent net10, BCL-only package
`dw.tools.math.scientific 1.1.0-preview.1` to hold finite exact
BigInteger operations. Exact outputs remain integers; there is no
implicit binary64 approximation, model ranking, stochastic primality
or external CAS/provider.

`BoundedExactScientific` offers five operations:
GreatestCommonDivisor, LeastCommonMultiple, Factorial, Binomial and
SumArithmeticSequence. GCD and LCM require nonnegative BigInteger
operands; gcd(0,0)=0 and lcm(0,n)=0 are explicit conventions.
Factorial is defined for integers from zero through 256; binomial
requires n in [0,256] and 0 <= k <= n. The arithmetic sequence sum
is exact for signed first/difference and 0 <= count <= 4096, using
the closed form n*a + n*(n-1)*d/2; no array enumeration is needed.

An ExactScientificBudget limits operand bit count (default 128),
result bit count (default 4096) and operations/logical sequence steps
(default 4096). Policies also have fixed finite ceiling values. On
invalid input, invalid policy, input/iteration budget, output growth or
cancellation the method returns a typed nonpartial refusal, with null
BigInteger value and zero successful steps. Multiplication/intermediate
BigInteger arithmetic is exact, not proof of unbounded resources.
Negative operands for GCD/LCM are refused pending T2 decisions.

## Independent acceptance and distribution

Observe a genuinely compiled RED against the skeleton package before
staging the new operation API; recover the RED and pass 69 separately
named GREEN oracles, including gcd(48,18)=6, lcm(6,8)=24, 0!=1,
C(5,2)=10 and 1+3+5+7+9=25. Verify negative-domain refusal,
oversized outputs, iteration budgets, signed sequence sums, policy
validation and cancellation.

Perform a locked Release solution build, the complete new scientific
MSTest project, three relevant existing Math regressions and the
architecture validator. Pack the actual preview.1 locally; inspect
nuspec, net10 assembly and SHA-256 with an independent C# helper, then
restore and execute a source-free consumer with eight independently
calculated BigInteger oracles against the exact archive.

Only C01-T1 and its RED/GREEN/verify children complete. M6-W02 and
M6-W02-C01 stay in progress; T2 must refine negative input/output-size
budgets and clarify number-theoretic claims. Public/stable NuGet,
cross-platform performance, primality, combinatorial probability and
agent/Decision/MCDM adoption are not qualified.
