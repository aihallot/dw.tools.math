# Search: `BigInteger`

Paths: `src/projects`

```text
src/projects/dw.quantities.expression/ExpressionParser.cs:457:             var numerator = BigInteger.Pow(value.Value.Numerator, power);
src/projects/dw.quantities.expression/ExpressionParser.cs:458:             var denominator = BigInteger.Pow(value.Value.Denominator, power);
src/projects/dw.quantities.expression/ExpressionParser.cs:480:         private static bool TryExactRoot(BigInteger value, int degree, out BigInteger root)
src/projects/dw.quantities.expression/ExpressionParser.cs:489:             var target = BigInteger.Abs(value);
src/projects/dw.quantities.expression/ExpressionParser.cs:490:             var low = BigInteger.Zero;
src/projects/dw.quantities.expression/ExpressionParser.cs:491:             var high = target + BigInteger.One;
src/projects/dw.quantities.expression/ExpressionParser.cs:492:             while (low + BigInteger.One < high)
src/projects/dw.quantities.expression/ExpressionParser.cs:495:                 if (BigInteger.Pow(middle, degree) <= target) low = middle;
src/projects/dw.quantities.expression/ExpressionParser.cs:500:             return BigInteger.Pow(root, degree) == value;
src/projects/dw.quantities.expression/ExpressionParser.cs:532:         private static int Digits(BigInteger value) => BigInteger.Abs(value).ToString().Length;
src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs:114:         yield return InformationUnit("information.byte", "B", "byte", UnitSystem.Si, BigInteger.One);
src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs:127:         var decimalScale = BigInteger.One;
src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs:145:         var binaryScale = BigInteger.One;
src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs:158:         BigInteger scale) =>
src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs:165:             new ExactRational(scale, BigInteger.One),
src/projects/dw.quantities/ExactBinaryNumber.cs:14:         var significand = new BigInteger(exponent == 0 ? fraction : fraction | (1UL << 52));
src/projects/dw.quantities/ExactBinaryNumber.cs:18:             ? new ExactRational(significand << power, BigInteger.One)
src/projects/dw.quantities/ExactBinaryNumber.cs:19:             : new ExactRational(significand, BigInteger.One << -power);
src/projects/dw.quantities/ExactDecimalFormatter.cs:32:         var scale = BigInteger.Pow(10, decimalPlaces);
src/projects/dw.quantities/ExactDecimalFormatter.cs:33:         var quotient = BigInteger.DivRem(
src/projects/dw.quantities/ExactDecimalFormatter.cs:34:             BigInteger.Abs(value.Numerator) * scale,
src/projects/dw.quantities/ExactDecimalFormatter.cs:39:             quotient += BigInteger.One;
src/projects/dw.quantities/ExactDecimalFormatter.cs:65:         BigInteger quotient,
src/projects/dw.quantities/ExactDecimalFormatter.cs:66:         BigInteger remainder,
src/projects/dw.quantities/ExactDecimalFormatter.cs:67:         BigInteger denominator,
src/projects/dw.quantities/ExactRational.cs:8:     public ExactRational(BigInteger numerator, BigInteger denominator)
src/projects/dw.quantities/ExactRational.cs:17:             numerator = BigInteger.Negate(numerator);
src/projects/dw.quantities/ExactRational.cs:18:             denominator = BigInteger.Negate(denominator);
src/projects/dw.quantities/ExactRational.cs:21:         var divisor = BigInteger.GreatestCommonDivisor(BigInteger.Abs(numerator), denominator);
src/projects/dw.quantities/ExactRational.cs:23:         _denominatorMinusOne = denominator / divisor - BigInteger.One;
src/projects/dw.quantities/ExactRational.cs:26:     public BigInteger Numerator { get; }
src/projects/dw.quantities/ExactRational.cs:30:     private readonly BigInteger _denominatorMinusOne;
src/projects/dw.quantities/ExactRational.cs:32:     public BigInteger Denominator => _denominatorMinusOne + BigInteger.One;
src/projects/dw.quantities/ExactRational.cs:34:     public static ExactRational Zero { get; } = new(BigInteger.Zero, BigInteger.One);
src/projects/dw.quantities/ExactRational.cs:36:     public static ExactRational One { get; } = new(BigInteger.One, BigInteger.One);
src/projects/dw.quantities/ExactRational.cs:39:         ? new ExactRational(BigInteger.Abs(Numerator), Denominator)
src/projects/dw.quantities/ExactRational.cs:72:         var numerator = BigInteger.Parse(digits, NumberStyles.None, CultureInfo.InvariantCulture);
src/projects/dw.quantities/ExactRational.cs:75:             numerator = BigInteger.Negate(numerator);
src/projects/dw.quantities/ExactRational.cs:78:         return new ExactRational(numerator, BigInteger.Pow(10, fractional.Length));
src/projects/dw.quantities/ExactRational.cs:104:         var absoluteNumerator = BigInteger.Abs(Numerator);
src/projects/dw.quantities/ExactRational.cs:105:         var whole = BigInteger.DivRem(absoluteNumerator, Denominator, out var remainder);
src/projects/dw.quantities/ExactRational.cs:166:             !BigInteger.TryParse(parts[0], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var numerator) ||
src/projects/dw.quantities/ExactRational.cs:167:             !BigInteger.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var denominator) ||
src/projects/dw.quantities/ExactRational.cs:168:             denominator <= BigInteger.Zero)
src/projects/dw.quantities/ExactRational.cs:178:         if (!BigInteger.TryParse(
src/projects/dw.quantities/ExactRational.cs:194:             BigInteger.Abs(whole) * fraction.Denominator + fraction.Numerator,
src/projects/dw.quantities/ExactRational.cs:197:             ? new ExactRational(BigInteger.Negate(magnitude.Numerator), magnitude.Denominator)
src/projects/dw.quantities/ExactRationalBudget.cs:23:         if (BigInteger.Abs(first).GetBitLength() >= maxBits ||
src/projects/dw.quantities/ExactRationalBudget.cs:24:             BigInteger.Abs(second).GetBitLength() >= maxBits)
src/projects/dw.quantities/ExactRationalBudget.cs:54:     private static void RequireWithin(BigInteger value, int bits)
src/projects/dw.quantities/ExactRationalBudget.cs:56:         if (BigInteger.Abs(value).GetBitLength() > bits)
src/projects/dw.quantities/ExactRationalBudget.cs:60:     private static void RequireProduct(BigInteger left, BigInteger right, int bits)
src/projects/dw.quantities/ExactRationalBudget.cs:62:         var a = BigInteger.Abs(left).GetBitLength();
src/projects/dw.quantities/ExactRationalBudget.cs:63:         var b = BigInteger.Abs(right).GetBitLength();
src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs:200:                     BigInteger.Negate(positive.Numerator),positive.Denominator));
src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs:235:                     new ExactRational(ReadInteger(true), BigInteger.One));
src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs:266:             if (denominator <= BigInteger.Zero)
src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs:268:             var gcd = BigInteger.GreatestCommonDivisor(
src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs:269:                 BigInteger.Abs(numerator), denominator);
src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs:270:             if (denominator.IsOne || numerator.IsZero || gcd != BigInteger.One)
src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs:275:         private BigInteger ReadInteger(bool allowSign)
src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs:291:             if (!BigInteger.TryParse(digits,NumberStyles.None,
src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs:296:             return negative ? BigInteger.Negate(integer) : integer;
src/projects/dw.tools.math.codecs/CanonicalMathNotation.cs:210:             exponent.Value.Denominator == BigInteger.One &&
src/projects/dw.tools.math.codecs/CanonicalMathNotation.cs:211:             exponent.Value.Numerator >= BigInteger.Zero &&
src/projects/dw.tools.math.codecs/CanonicalMathNotation.cs:222:         BigInteger.Abs(value.Numerator).GetBitLength() <= MaximumRationalBits &&
src/projects/dw.tools.math.codecs/CanonicalMathNotation.cs:314:                     BigInteger.Abs(value.Numerator).ToString(CultureInfo.InvariantCulture) +
src/projects/dw.tools.math.codecs/CanonicalMathNotation.cs:420:                         scalar.Value.Denominator != BigInteger.One ||
src/projects/dw.tools.math.codecs/CanonicalMathNotation.cs:421:                         scalar.Value.Numerator < BigInteger.Zero ||
src/projects/dw.tools.math.composition/ExactQuantityPipeline.cs:215:         if (System.Numerics.BigInteger.Abs(value.Numerator)
src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs:141:         if (!BigInteger.TryParse(Text(obj, "numerator"), NumberStyles.AllowLeadingSign,
src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs:143:             !BigInteger.TryParse(Text(obj, "denominator"), NumberStyles.AllowLeadingSign,
src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs:144:                 CultureInfo.InvariantCulture, out var denominator) || denominator <= BigInteger.Zero)
src/projects/dw.tools.math.numerics/RationalBinary64Conversion.cs:45:     private static readonly BigInteger NormalUnit = BigInteger.One << 52;
src/projects/dw.tools.math.numerics/RationalBinary64Conversion.cs:46:     private static readonly BigInteger OverflowedSignificand = BigInteger.One << 53;
src/projects/dw.tools.math.numerics/RationalBinary64Conversion.cs:55:         var absolute = BigInteger.Abs(numerator);
src/projects/dw.tools.math.numerics/RationalBinary64Conversion.cs:120:     private static BigInteger RoundedQuotient(BigInteger numerator, BigInteger denominator)
src/projects/dw.tools.math.numerics/RationalBinary64Conversion.cs:122:         var integral = BigInteger.DivRem(numerator, denominator, out var remainder);
src/projects/dw.tools.math.numerics/RationalBinary64Conversion.cs:125:             ? integral + BigInteger.One
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:20:         BigInteger left, BigInteger right, ExactScientificBudget? budget=null,
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:46:         BigInteger left, BigInteger right, ExactScientificBudget? budget=null,
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:56:         if(left.IsZero||right.IsZero)return Admit(BigInteger.Zero,0,limits);
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:77:         var value=BigInteger.One;
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:105:         var value=BigInteger.One;
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:118:         BigInteger first,BigInteger difference,int count,
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:130:         if(count==0)return Admit(BigInteger.Zero,0,limits);
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:132:         var n=new BigInteger(count);
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:143:     private static bool WithinOperand(BigInteger value,ExactScientificBudget limits) =>
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:144:         BigInteger.Abs(value).GetBitLength()<=limits.MaximumOperandBits;
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:146:     private static bool WithinResult(BigInteger value,ExactScientificBudget limits) =>
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:147:         BigInteger.Abs(value).GetBitLength()<=limits.MaximumResultBits;
src/projects/dw.tools.math.scientific/BoundedExactScientific.cs:150:         BigInteger value,int steps,ExactScientificBudget limits) =>
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:15:     ExactScientificStatus Status, BigInteger? Value, int Steps,
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:24: /// Provider-neutral exact BigInteger boundary facade. T1 operations remain intact.
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:36:         BigInteger left, BigInteger right,
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:47:             normalized?BigInteger.Abs(left):left,
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:48:             normalized?BigInteger.Abs(right):right,budget,cancellationToken);
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:53:         BigInteger left, BigInteger right,
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:64:             normalized?BigInteger.Abs(left):left,
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:65:             normalized?BigInteger.Abs(right):right,budget,cancellationToken);
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:82:         BigInteger first,BigInteger difference,int count,
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:96:            BigInteger.Abs(first).GetBitLength()>limits.MaximumOperandBits ||
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:97:            BigInteger.Abs(difference).GetBitLength()>limits.MaximumOperandBits)
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:102:         var n=new BigInteger(count);
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:103:         var pairs=n*(n-BigInteger.One)/2;
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:108:             BigInteger.Abs(left).GetBitLength(),
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:109:             global::System.Math.Max(BigInteger.Abs(right).GetBitLength(),
src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs:110:                 BigInteger.Abs(total).GetBitLength()));
src/projects/dw.tools.math.scientific/ExactScientificContracts.cs:20: /// <summary>An admitted exact BigInteger or a typed refusal with no partial number.</summary>
src/projects/dw.tools.math.scientific/ExactScientificContracts.cs:22:     ExactScientificStatus Status, BigInteger? Value, int Steps, string RuleId)
src/projects/dw.tools.math.statistics/DescriptiveStatistics64.cs:91:         var mean = sum / new ExactRational(values.Length, BigInteger.One);
src/projects/dw.tools.math.statistics/DescriptiveStatistics64.cs:107:         if (BigInteger.Abs(value.Numerator).GetBitLength() > MaximumExactOperandBits ||
src/projects/dw.tools.math.statistics/QuantileStatistics64.cs:87:             var rank = BigInteger.DivRem(scaled, probability.Denominator, out var rem);
src/projects/dw.tools.math.statistics/QuantileStatistics64.cs:93:         var integer = BigInteger.DivRem(scaledPosition, probability.Denominator,
src/projects/dw.tools.math.statistics/QuantileStatistics64.cs:125:         if (BigInteger.Abs(value.Numerator).GetBitLength() > MaximumExactOperandBits ||
src/projects/dw.tools.math.symbolics/SymbolicDerivativePointAnalysis.cs:177:                    exponent.Value.Denominator!=BigInteger.One ||
src/projects/dw.tools.math.symbolics/SymbolicDerivativePointAnalysis.cs:259:         BigInteger.Abs(value.Numerator).GetBitLength()<=MaximumRationalBits &&
src/projects/dw.tools.math.symbolics/SymbolicExactDifferentiation.cs:140:                    exponent.Value.Denominator!=BigInteger.One)
src/projects/dw.tools.math.symbolics/SymbolicExactPolynomialCalculus.cs:235:                     exponent.Value.Denominator != BigInteger.One ||
src/projects/dw.tools.math.symbolics/SymbolicExactPolynomialCalculus.cs:360:         BigInteger.Abs(x.Numerator).GetBitLength() <= MaximumRationalBits &&
src/projects/dw.tools.math.symbolics/SymbolicExactRealSolving.cs:184:     private static bool PerfectSquare(BigInteger nonnegative,out BigInteger root)
src/projects/dw.tools.math.symbolics/SymbolicExactRealSolving.cs:186:         root = BigInteger.Zero;
src/projects/dw.tools.math.symbolics/SymbolicExactRealSolving.cs:189:         var low = BigInteger.Zero;
src/projects/dw.tools.math.symbolics/SymbolicExactRealSolving.cs:190:         var high = BigInteger.One << (int)((nonnegative.GetBitLength()+1)/2);
src/projects/dw.tools.math.symbolics/SymbolicExactRealSolving.cs:201:                 low = mid+BigInteger.One;
src/projects/dw.tools.math.symbolics/SymbolicExactRealSolving.cs:203:                 high = mid-BigInteger.One;
src/projects/dw.tools.math.symbolics/SymbolicExactRealSolving.cs:221:         BigInteger.Abs(value.Numerator).GetBitLength()<=MaximumRationalBits
src/projects/dw.tools.math.symbolics/SymbolicExactTransformations.cs:243:         BigInteger.Abs(number.Numerator).GetBitLength() <= MaximumCoefficientBits &&
src/projects/dw.tools.math.symbolics/SymbolicRationalCancellationBridge.cs:77:         if (BigInteger.Abs(c.Numerator).GetBitLength() > MaximumConstantOperandBits ||
src/projects/dw.tools.math.symbolics/SymbolicRealCalculusBoundaries.cs:264:         BigInteger.Abs(x.Numerator).GetBitLength()<=MaximumRationalBits &&
src/projects/dw.tools.math.symbolics/SymbolicRootAudit.cs:279:                         if(exponent.Denominator!=BigInteger.One ||
src/projects/dw.tools.math.symbolics/SymbolicRootAudit.cs:335:         BigInteger.Abs(x.Numerator).GetBitLength()<=MaximumRationalBits &&
```
