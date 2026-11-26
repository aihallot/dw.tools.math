# RS371 - Ready M11 exact rational scaling RED

RS370 admitted M11 at attempt 001, first-pass success, result 1f6e7426d1ed02a86950c8fac720b8abf59792aa. Nine preparation validations passed. RS371 only readies the five-node C01/T1/RED chain; other M11 nodes remain not-ready, M10 readiness is preserved, and canonical status remains planned pending durable readiness reconciliation. No product source change or qualification of a new API.

## Reuse and proposed public contract

Extend the existing public static class dw.quantities.ExactRationalBudget in src/projects/dw.quantities/ExactRationalBudget.cs, alongside its existing Add and Multiply methods and resource-policy helpers. ExactRational already guarantees a reduced numerator and strictly positive denominator, including default zero. ExactDecimalFormatter has rounding semantics and RationalBinary64Conversion targets binary64; neither is a lossless integer conversion. The M3-W02-C04 consumer-local TryScaleExactly is historical evidence, not a public product member. Its positive-only scale contract remains unchanged.

Proposed additive member for the RED contract: ExactInt64ScaleResult TryScaleToInt64(ExactRational value, BigInteger scale, int maxBits). Public result has ExactInt64ScaleStatus Status and long? Value. Statuses: Success, NonIntegral, Overflow, ResourceLimit (use a non-success default if a value type is selected). Value is present exactly for Success. Invalid maxBits below two throws ArgumentOutOfRangeException, consistent with existing budget entry points. No changes to existing Add/Multiply or rational operators. New member and result types are not yet implemented, tested or published.

The generic scale is any integer, including zero and negative. Decision's positive decimal scale and 0-18 places remain consumer policy. Budget applies to absolute normalized numerator, denominator, scale and exact integral result; validate input budgets before any zero shortcut. On valid operands, divisibility is equivalent to denominator dividing scale because the rational is reduced. Cancel the denominator before multiplication; reuse existing budget checks where appropriate, bound any uncertain product to at most maxBits+1 bits before final exact bit check, and avoid constructing arbitrarily large unbounded intermediates. Define refusal precedence: invalid budget argument, input ResourceLimit, NonIntegral, result ResourceLimit, then Int64 Overflow. Success may represent long.MinValue, long.MaxValue and zero; no floating point, rounding or truncation. Exact result uses absolute bit length (long.MinValue needs 64 bits).

## RED oracle cases

Use independent BigInteger arithmetic or fixed mathematical expectations, not the production algorithm as oracle. For a sufficient explicit budget such as 4096:
- 3/4 times 100 = 75; -3/4 times 100 = -75; 3/4 times -100 = -75; -3/4 times -100 = 75.
- 1/3 times 100 and 1/1000 times 100 => NonIntegral; 1/3 times 0 => Success 0; default rational times a signed integer => Success 0.
- Both Int64 boundaries => Success; one beyond either endpoint => Overflow; Int64 minimum times -1 => Overflow.
- (1 / 2^2000) times 2^2000 => Success 1; do not reject due to avoidable uncancelled intermediate growth.
- At maxBits=8, 1/3 times 256 => ResourceLimit before NonIntegral, 128 times 2 => ResourceLimit, 127 times 2 => Success 254, 1/3 times 100 => NonIntegral.
- At maxBits=64, long.MaxValue times 2 => Overflow; long.MinValue times -1 => Overflow; at maxBits=63, long.MinValue times 1 => ResourceLimit.
- Invalid budget 1 => ArgumentOutOfRangeException; non-success carries no numeric value. Include fixed independent cases and deterministic bounded generated comparisons, with stable historical rational regression evidence.

Target test project: tests/projects/dw.quantities.tests/dw.quantities.tests.csproj. RED must establish an executable missing-member/contract falsification without breaking the normal build; GREEN shall make the same oracle pass. Keep tests focused before wider qualification. Final implementation may refine signature only through explicit documented RED review; no silent behavior change. Historical packages remain immutable, no new release authorization, no Decision changes and no production framework cutover.
