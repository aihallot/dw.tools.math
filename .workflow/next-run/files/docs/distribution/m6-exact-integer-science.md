# Finite exact scientific integer functions — local preview.1

Package: dw.tools.math.scientific 1.1.0-preview.1, net10, BCL-only.

API: BoundedExactScientific.GreatestCommonDivisor and
LeastCommonMultiple accept nonnegative BigInteger inputs. GCD(0,0)=0,
LCM(0,n)=0; negative input is refused. Factorial(n) admits 0..256;
Binomial(n,k) admits 0..256 and 0<=k<=n; SumArithmeticSequence
(first,difference,count) admits signed integer terms with count
0..4096 and uses the exact closed finite-sum formula.

Each call can take an ExactScientificBudget and a cancellation token.
Default maximum operand/result bits are 128/4096, and maximum steps
4096. Policies and hard operation caps are finite. The result is
ExactScientificResult with either a precise BigInteger or a typed
nonpartial refusal; BigInteger alone does not waive size budgets.

Examples: gcd(48,18)=6, lcm(6,8)=24, 0!=1, C(5,2)=10,
sum(1,3,5,7,9)=25. There is no lossy conversion or probabilistic
primality guarantee.

Independent T1 qualification requires one compiled RED, 69 GREEN
tests, three relevant Math regressions, actual local NuGet SHA
inspection and eight source-free independent consumer probes.
This is not a stable/public release, unlimited mathematics library,
cross-platform certification or agent/Decision/MCDM integration.
C01-T2 and M6-W02/M6 remain open.
