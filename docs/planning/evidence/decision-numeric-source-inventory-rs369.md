# RS369 - Decision numeric subsidiarity source inventory

Scope: all Git-tracked C# and project files under src/projects, checked against the production solution roster. Hashes identify LF-normalized text, not binaries.
This is a lexical review aid, not a compiler API inventory or proof of absence. Review all candidate declarations and alternate implementations before admitting new APIs.
The report does not establish performance, package compatibility, publication or adoption.

## Tracked project files (includes all thirteen production projects)
- src/projects/dw.quantities.expression/dw.quantities.expression.csproj
- src/projects/dw.quantities.standard/dw.quantities.standard.csproj
- src/projects/dw.quantities/dw.quantities.csproj
- src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj
- src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj
- src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj
- src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj
- src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj
- src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj
- src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj
- src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj
- src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj
- src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj

## Complete tracked source roster
- src/projects/dw.quantities.expression/ExactSelection.cs | 434d05cfddb720a88efd1b591425e31eb8e933bdfbedfe2b88b395af17551c8a
- src/projects/dw.quantities.expression/ExpressionDiagnostics.cs | 74ad83dfe295f87ec16997713c8c72f627dcaff61f5674ad86ef5372528a3395
- src/projects/dw.quantities.expression/ExpressionParser.cs | 53a4ea633084ecbc4236851405c9f84ba6cca2905671e9efd132acf504eb9e28
- src/projects/dw.quantities.expression/dw.quantities.expression.csproj | c9bdd28d8f1502c6423018c3838c5de7ffd0f7f2b2447986c939fb7a7279d531
- src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs | c64f9b0f9595889190e4d383853ba981b61dd241bd7565673d28e7d31b203b6a
- src/projects/dw.quantities.standard/ExplicitProfileExpressionUnitResolver.cs | 4c33ef90e8a99385849a49a7d639bc9df87169fb8d96f0038d620bd5f58c9d2f
- src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs | 1b5384ce9e56cecd1746d8b1dc9f1bf3000501fdf787896df5dd2f08373c7108
- src/projects/dw.quantities.standard/PureUnitConverter.cs | 21201d67082c731a65269d4751e0829c6c727cbacb373f6490b22a49d2627915
- src/projects/dw.quantities.standard/StandardExpressionUnitResolver.cs | 01f5eff951115f803c3f58ed76fb779864c4a0aa6d8eb3015ec2492e5f7a0895
- src/projects/dw.quantities.standard/StandardUnitCatalog.cs | 3795ffd2f668fba864673b4e88921578e4b7eab9fb4712b266c8dc63453a4a82
- src/projects/dw.quantities.standard/dw.quantities.standard.csproj | d20d71997dae2eaa9e0f1e34eed5f697eda1af72dea94c75f726c2dcf9e951c7
- src/projects/dw.quantities/ApproximateEquivalence.cs | 34c1726fe637ea4ed3da41b793a5c9e9ef3f0088baf21d8e96c42aabd33949dc
- src/projects/dw.quantities/DimensionVector.cs | d558be25410547f6b77b1d0a15370f86475f087614b10dc3fb29eaee948fb5c2
- src/projects/dw.quantities/ExactBinaryNumber.cs | cda9835b8a83f01239d344bd83b4144c7a6e9dcad59d9c2cc385103b76a2225d
- src/projects/dw.quantities/ExactComparisonPolicy.cs | 72ce91ecb236c249a1ef743162cc24731a85231c0d0fe9e956270d24ae914c72
- src/projects/dw.quantities/ExactDecimalFormatter.cs | 20499543d9635a291a54ff91763ece266176fa08bf67421d173caae5f59d45ce
- src/projects/dw.quantities/ExactRational.cs | 9fca6cb35d4254351b4fb82c659a1f8de52ca2e29deebd28a84bfb401647d90b
- src/projects/dw.quantities/ExactRationalBudget.cs | 833b53e74a2f327e39e8c527480c6df2f6a4db711b4ca677beedbca52f6438ed
- src/projects/dw.quantities/Quantity.cs | f9121bedc85e810a958f6d2f833652a59f5f7b66a81a8445e9d0c6cec7104e6f
- src/projects/dw.quantities/UnitDefinition.cs | 8b882bf0103e1568c8c5b5d6afa0ddb7e72876819de6c83f963bbc4c3337d199
- src/projects/dw.quantities/dw.quantities.csproj | 9f36cb3214bd7caa77271ff6421a00f881a88d4c482030b9f2f5fb3daf14eba8
- src/projects/dw.tools.math.analysis/BoundedDifferentialAnalysis64.cs | 5db05839052dea9265438d9210dcfaa9d522298039104f3a915148e28a7ec674
- src/projects/dw.tools.math.analysis/BoundedUnivariateAnalysis64.cs | 73242b1321f7e9e8690ddf346f9fbe516dbf5bac6c572d3b54729f836e1ed229
- src/projects/dw.tools.math.analysis/CurveBoundaryAudit64.cs | 155c80d2bbf56ad72897106718de92755f4fc49f389770213499acef5435b282
- src/projects/dw.tools.math.analysis/CurvePoint64.cs | 51d7b2f2240a5ecd0d0c6da6c1122eb0b789cade5cfb8ba0f18a4372b9ab41a4
- src/projects/dw.tools.math.analysis/DifferentialBoundaryAudit64.cs | c9591c0ebdf62f64bae11b32d6cfa7639e6f2b86683f72855ea589c68649c77a
- src/projects/dw.tools.math.analysis/M6W01C01Contract.cs | cf4e53737e8f674ecd033dc2a869d1415251e3b7fcd192a0cb4aebe1b3042b7c
- src/projects/dw.tools.math.analysis/UnivariateBoundaryAudit64.cs | 6115b35ea2b6a55f26ebe567c21ddbac439b24938c4bfcbd0c602016a6f761b9
- src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj | f92d0bf06704bb4b153fa976e6e19aca04c744fb89d81884dc5451205c6a3da7
- src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs | 8f3a41ea90865acd6065130cb5d9ec14dad2025eaa979c95be7110e7e5ad99c4
- src/projects/dw.tools.math.codecs/CanonicalDisplayBoundaryAudit.cs | f5557d97226b51779e3331824889829987887a42a2ac8f036cdbec91a2ffb095
- src/projects/dw.tools.math.codecs/CanonicalMathNotation.cs | c9e58671207598d931b5c409eb1f44fb38ba1f8aa435ba95b016c4adef80fb8d
- src/projects/dw.tools.math.codecs/ExactMathMlInterop.cs | ea7bb23e56226918251cde00b14a7f004d9c438171c127dcf512b6795516e92f
- src/projects/dw.tools.math.codecs/ExactOfficeOmmlCodec.cs | 931110122cf8844f7cac5229707886fc5f421799fc91aa5e8be2da6d199161dc
- src/projects/dw.tools.math.codecs/LatexImportBoundaryAudit.cs | 124cc5473edb324b32873cc16f245ef7422985c27a223037b9a9ad75b9a69c83
- src/projects/dw.tools.math.codecs/MarkdownUnicodeMathCodec.cs | bc40d6804cf32738e3dcfbf002aa835148d47fc26e7382f2e208a0c4e51f5a2b
- src/projects/dw.tools.math.codecs/MathDisplayBoundaryAudit.cs | 831f8faf711b3c45280bb51da7a0802627388d934f206f44c2c3bceb000fa743
- src/projects/dw.tools.math.codecs/MathMlBoundaryAudit.cs | 6979dbdc4610dc4b094beff395774c9963fbfca0960b990ce08d5bd75c69f19d
- src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj | 96d72f1420ecdce3b5e69e0c5eb25e293e3a703e68c6beadee1a7ccc8c8e483b
- src/projects/dw.tools.math.composition/ExactQuantityPipeline.cs | f581d99c12c5387adbb3d64da8c26d900fe82b65a780a1f9ffb0105f6434b013
- src/projects/dw.tools.math.composition/ExactReplayContracts.cs | 411bcbbdef3c7d0d27880979334fb9881f39400de19cbbac7dbc04daa9291bc5
- src/projects/dw.tools.math.composition/MathCapabilityCatalog.cs | a9ec695a3ef34f0efa996800667d6fec731ca49ab3973553bceb635f8cf4a557
- src/projects/dw.tools.math.composition/OperationContracts.cs | af567a8b2d850685700815e9020336eaa6a0900db7f4d813689af8eec7631893
- src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj | 65a8928fdef6f75db77010bbbe150f7cc55e8e57bc1e3de4ca781de8329cd8ca
- src/projects/dw.tools.math.distribution/ExactMathVerticalRunner.cs | 556bac495038ccbf2a66a0e648f8cc0c07336cfb791fb373499e2d58907e64ea
- src/projects/dw.tools.math.distribution/ExactVerticalBoundaryAudit.cs | 9e5efc559d047ffbf010bf073185df37d009728c161921e4994e7f5f4dbfb6b2
- src/projects/dw.tools.math.distribution/ExactVerticalContracts.cs | 68590d91d4da670b7073f176852a4ad7d802a39198b254f8a5069d48ec0cd239
- src/projects/dw.tools.math.distribution/MathLocalReleaseQualification.cs | 5a05fce35ad08b25301993548e3b4b04902a907e6aa9c5d0facd03b50e4f0cd4
- src/projects/dw.tools.math.distribution/MathPackageRuntimeSurfaceAudit.cs | 8bc2a33db4ac728c496e5570bc27113dcd2e98d7a439fd41d76254e0ea3d794f
- src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj | d820a7e8b0504cb6d08b2d2fd461099d2bf690251662db360e30465e007f69e3
- src/projects/dw.tools.math.foundation/FoundationContract.cs | 3d2ae2b448a543c2d0a3c607b94a69068aa6df928fa2169fe615252a398c8654
- src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj | 372911f791264d41e2b65f89913c06aff8d18baa74dc09d10c7bdd5fbd648c54
- src/projects/dw.tools.math.ir/IrCanonicalHashes.cs | 18bf29cf31748615f49fd2548de4fc1cfda9a15e2190239f1c092741bc905c48
- src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs | e82f19e9631bac5b862a2327c65342d67ae242fc770f3b1655ba2d4764e898f2
- src/projects/dw.tools.math.ir/IrCanonicalJsonGuard.cs | ba9421e1637727ac4dc67d2d9fc3ce54b4584ff60f56326fec8eb8a1894f9643
- src/projects/dw.tools.math.ir/IrGraphLimits.cs | 9296ec953f00d0fb203802c2ddf6cdd447bb092355144ee6cdd050d5fa65479d
- src/projects/dw.tools.math.ir/IrMatrix.cs | 5e391c8d406ce84c0d2599f5d734dcd5e21928dfb2b57587fd02a85d618bb5cf
- src/projects/dw.tools.math.ir/IrModel.cs | 50da7f35bd9d550dbca928d6b88c2c9259c93d6b67a287366718e3fed46b9fca
- src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj | c5cc62555ee4a86eb48db21f0d2fad64ee02799bcf8f2b881d4ac264f119732f
- src/projects/dw.tools.math.numerics/BoundedLinearAlgebra64.cs | 63791defea2af85af32fc731c6dc1bc60043aebb40e55c8e44ef5797a18c4c3e
- src/projects/dw.tools.math.numerics/NumericalMatrices.cs | 2bd0304f3aee8cf7f53bbd7e69a31369a07bd298c8b4c3c78d9e5320f2f60e90
- src/projects/dw.tools.math.numerics/RationalBinary64Conversion.cs | 5a77588a3cf61bab5d4dd5313286d850e4653b4f5a042bdfe515df186614bab4
- src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj | 9fc83068c901d4f1373a04094087473a7646c2bf6aac76e130e87cd9f41758ab
- src/projects/dw.tools.math.scientific/BoundedExactScientific.cs | 8161f07a760066e345db2c2f2297a9f93045443c65a5facb613e0e609f620e45
- src/projects/dw.tools.math.scientific/ExactScientificBoundaryAudit.cs | d3dd0a01a76fe42c9d38a35dca4cdc18134c56c4728762fdbaa2adb41708e9db
- src/projects/dw.tools.math.scientific/ExactScientificContracts.cs | acdf3e354623209771496d55f6c602685f6b68de552b642c860867e7664f8ea6
- src/projects/dw.tools.math.scientific/FiniteGeometry64.cs | c367c5b6a4d1534457aab77a31b00b7fd4f9f1c81f25d27beff5ebac5c0e3b66
- src/projects/dw.tools.math.scientific/FiniteGeometryBoundaryAudit64.cs | 7355c99cb2bdcb72a3cdb2880136e8bd4643645ee578608582fbc76bf68e1fa6
- src/projects/dw.tools.math.scientific/FiniteSignal64.cs | 5afd547024f3b4a0928f8916fc6bac359f129d9524273a4b7a8b5cdeb397caff
- src/projects/dw.tools.math.scientific/FiniteSignalBoundaryAudit64.cs | da135763f464d65fc5f71d1bb09689259ea480ac26bde4b976db5ac6ee450ce3
- src/projects/dw.tools.math.scientific/FiniteUniformSeries64.cs | cde666251758da2e808a035a83a1f9d6b3d4ba072006e511895b71b63ea4f508
- src/projects/dw.tools.math.scientific/FiniteUniformSeriesBoundaryAudit64.cs | 908f18668b23ea788e5f1be531bf8a3bb6108c048a1e663f6c1927bdd9ab5d10
- src/projects/dw.tools.math.scientific/M6W02C03BoundaryAggregate64.cs | 634f5e701d92c9820d910b9a97f898c95c14e6abeb16ac6a3be3347d9d8b00e4
- src/projects/dw.tools.math.scientific/M6W02C03Contract.cs | bb0a22a3bd8689af6811613f388c37af8ffe206312c3fbf075dbf5a69e13cde5
- src/projects/dw.tools.math.scientific/M6W02C04Boundary64.cs | df2ce1460020743df127c19377e99b85b65a4e1fd57104db54bf833ea3357b60
- src/projects/dw.tools.math.scientific/M6W02C04Contract.cs | 31e1c8fff54c129313f6409df7ca0790c507a61f3dba384a0bb8a182e8ee5ec2
- src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs | ecc8004f78e14584df80ac66cec069ce43ab5f04d742e934c6bb7119e2f24e78
- src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs | f9d8067dfcb1112ff997c15529fb3c866f2aba1b70b7c7eff064dc3433ecf467
- src/projects/dw.tools.math.scientific/SpecialFunctionContracts64.cs | 5add6c1e86a47004b759197115a2bc9a0386eb4bc806bcb6ab4a665b6c916582
- src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj | b7876b943c5f45afc76fce3fa1869ec2e28e20529068c15fd6525bb90856d6c1
- src/projects/dw.tools.math.statistics/BernoulliDistribution64.cs | 1e65927a2f7c23622eb70f503df815cd4187a8225944d7f600816bbd9b6dd9f3
- src/projects/dw.tools.math.statistics/ContinuousUniformDistribution64.cs | d65d4dd22ca5266bc6fb41a79af846d0f049ecd8534a33145ace6c8ca3003b5b
- src/projects/dw.tools.math.statistics/CorrelationStatistics64.cs | 0484ab67e23458b2110ba60399b231a1eb79d5d1692b28725b2b883cd3ce4ae3
- src/projects/dw.tools.math.statistics/DescriptiveStatistics64.cs | 9382fbaae529e4499608f9e66a40af3017ce8bff322aa0f556caf8969b7c8923
- src/projects/dw.tools.math.statistics/FrequencyWeightedStatistics64.cs | a965ecfcf26e0ac2b1a507c8c50f94a18ef7806941e401dbaa45d71e3e8cda15
- src/projects/dw.tools.math.statistics/QuantileStatistics64.cs | 52352a818dfb3ce3990cd4c8d0ee67a85765b620b3326de961af2a4460d26814
- src/projects/dw.tools.math.statistics/SplitMix64PositionCodec.cs | d866f37bb2701708b2c8d5128983bf5805599af9ef0a4d6da6dbcf7c711b3ecf
- src/projects/dw.tools.math.statistics/SplitMix64Random.cs | 9c1ca16250090fd58aac645052903e25090f9b5058d0df138161f4fee1c2eba4
- src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj | 988e2f3b3c9046c664aa56f40c6d6304bef1486ecca2debd2aba2abc8511760f
- src/projects/dw.tools.math.symbolics/SymbolicBoundedExecution.cs | ff4ee32c9e69f7bedd810e53e4c57c2a12d32c36766be74b102914dd85e0b0ac
- src/projects/dw.tools.math.symbolics/SymbolicDerivativePointAnalysis.cs | e4e72035f151a0b10d86a643829f6ca432c083a57a28a3fde0131f23caf21fa0
- src/projects/dw.tools.math.symbolics/SymbolicExactDifferentiation.cs | 88ae9346dde6f0aac8c175eeca8a12563af5753a6f1ed2f43ed76c89f3bc2cc8
- src/projects/dw.tools.math.symbolics/SymbolicExactPolynomialCalculus.cs | 6b842de6f77b4c692dcffe2e9248c71822b321216ffadb28a93a729dd148084a
- src/projects/dw.tools.math.symbolics/SymbolicExactRealSolving.cs | 12e295c5b4a95e6fbe972a3be69e77174bc143baceb647d21633b7a95651551a
- src/projects/dw.tools.math.symbolics/SymbolicExactTransformations.cs | 1aaa82fdfa7115ca870c6ce6036655beebe80929416f822e6111b63302e7e848
- src/projects/dw.tools.math.symbolics/SymbolicProviderResultAdmission.cs | 99b2723143ccc53c79f533ef51c14b2a3061a9d26fabcf812f847a43c451ba7e
- src/projects/dw.tools.math.symbolics/SymbolicRationalCancellationBridge.cs | 7aecdb7817065f6a771921330f12115f7c0c14146bdd2368ced1f8c7dea9d341
- src/projects/dw.tools.math.symbolics/SymbolicRealCalculusBoundaries.cs | b760030dec21dbac73a75ea22e0e17a44a39470194bd01b8f1e41e352acdbd23
- src/projects/dw.tools.math.symbolics/SymbolicRootAudit.cs | 556d9372b910d965628350fdb23e1a59c3ae3fabf86400aafcb16b09ae8eea3d
- src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj | 6b5d11b4e42bc2fb3d7aa3384153d95b49a73bda2c700ad132ca08a1414898f2

## Candidate lines across all production C# sources

### src/projects/dw.quantities.expression/ExpressionParser.cs

```text
5: public enum ExpressionUnitResolutionFailure
13: public sealed record ExpressionUnitResolution(
18: public interface IExpressionUnitResolver
23: public static class ExpressionDefaults
25: public const string Culture = "metric-international";
28: public static class MathExpressionLimits
30: public const int MaximumCharacters = 4096;
31: public const int MaximumTokens = 256;
32: public const int MaximumDepth = 32;
33: public const int MaximumMagnitudeDigits = 256;
34: public const int MaximumPower = 16;
35: public const int MaximumRootDegree = 16;
38: public enum TemperatureSemantics
45: public enum ExpressionFailureKind
59: public readonly record struct EvaluatedQuantity(
60: ExactRational Value,
64: public abstract record ExpressionEvaluationOutcome
68: public sealed record Success(EvaluatedQuantity Quantity) : ExpressionEvaluationOutcome;
70: public sealed record Failure(ExpressionFailureKind Kind, int Position) : ExpressionEvaluationOutcome;
73: public static class ExpressionParser
75: public static ExpressionEvaluationOutcome Evaluate(string expression) =>
78: public static ExpressionEvaluationOutcome Evaluate(
198: public EvaluatedQuantity Parse()
259: return value with { Value = new ExactRational(-value.Value.Numerator, value.Value.Denominator) };
308: ExactRational value;
311: value = ExactRational.ParseInvariantDecimal(number.Text);
459: var rational = exponent < 0
460: ? new ExactRational(denominator, numerator)
461: : new ExactRational(numerator, denominator);
462: return Checked(new(rational, Scale(value.Dimension, exponent), TemperatureSemantics.None), position);
475: new ExactRational(numerator, denominator),
503: private static DimensionVector Scale(DimensionVector value, int factor) => new(
564: public ExpressionFailureKind Kind { get; } = kind;
565: public int Position { get; } = position;
570: public static MissingUnitResolver Instance { get; } = new();
572: public ExpressionUnitResolution Resolve(string token, string culture) =>
```

### src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs

```text
11: public static class ElectrochemicalUnitProfile
13: public const string ProfileId = "math.si-electrochemical";
14: public const string ProfileVersion = "2026-11-electrochemical-exact-v1";
16: public static UnitDefinition Ampere { get; } = Define(
20: public static UnitDefinition Volt { get; } = Define(
24: public static UnitDefinition Ohm { get; } = Define(
28: public static UnitDefinition Watt { get; } = Define(
32: public static UnitDefinition Mole { get; } = Define(
36: public static UnitDefinition MolePerCubicMetre { get; } = Define(
40: public static ImmutableArray<UnitDefinition> All { get; } =
50: public static bool TryGetById(string id, out UnitDefinition? unit)
62: ExactRational.One, ExactRational.Zero, UnitTransformKind.Linear);
```

### src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs

```text
11: public static class InheritedStandardUnitCatalog
13: public const string Version = "0.2.0";
19: public static ImmutableArray<UnitDefinition> All { get; } =
64: new ExactRational(5, 9),
65: new ExactRational(45967, 180),
90: public static bool TryGetById(string id, out UnitDefinition? unit) =>
99: string scale,
107: ExactRational.ParseInvariantDecimal(scale),
108: ExactRational.Zero,
127: var decimalScale = BigInteger.One;
130: decimalScale *= 1000;
131: yield return InformationUnit(unit.Id, unit.Symbol, unit.Name, UnitSystem.Si, decimalScale);
145: var binaryScale = BigInteger.One;
148: binaryScale *= 1024;
149: yield return InformationUnit(unit.Id, unit.Symbol, unit.Name, UnitSystem.IecBinary, binaryScale);
158: BigInteger scale) =>
165: new ExactRational(scale, BigInteger.One),
166: ExactRational.Zero,
175: string scale,
184: ExactRational.ParseInvariantDecimal(scale),
185: ExactRational.ParseInvariantDecimal(offset),
204: new ExactRational(numerator, denominator),
205: ExactRational.Zero,
```

### src/projects/dw.quantities.standard/PureUnitConverter.cs

```text
8: public static class PureUnitConverter
10: public static ExactRational Convert(ExactRational value, UnitDefinition source, UnitDefinition target)
21: public static TemperatureMeasurement Convert(TemperatureMeasurement value,
```

### src/projects/dw.quantities.standard/StandardUnitCatalog.cs

```text
10: public static class StandardUnitCatalog
12: public const string CatalogVersion = "2026-10-exact-v1";
14: public static DimensionVector Volume { get; } = new(3, 0, 0, 0, 0, 0, 0);
16: public static ImmutableArray<UnitDefinition> Units { get; } =
19: ExactRational.One, "meter", "metres", "meters"),
21: new ExactRational(1, 100), "centimeter"),
23: new ExactRational(1000, 1), "kilometer"),
25: new ExactRational(127, 5000), "inches"),
27: ExactRational.One, "sec"),
29: new ExactRational(60, 1), "minutes"),
31: ExactRational.One, "bytes"),
33: new ExactRational(1024, 1), "kibibytes"),
35: new ExactRational(2365882365, 10000000000000L), "cups"),
37: new ExactRational(284130625, 1000000000000L), "cups"),
39: new ExactRational(1, 4000), "cups"),
41: new ExactRational(473176473, 1000000000000L), "pints"),
43: new ExactRational(56826125, 100000000000L), "pints"),
45: new ExactRational(57, 100000), "pints")
48: public static ImmutableDictionary<string, string> PassiveNames { get; } =
60: public static ImmutableArray<UnitDefinition> All => Units;
62: public static bool TryGetById(string id, out UnitDefinition? unit)
73: DimensionVector dimension, ExactRational scale, params string[] aliases) =>
74: new(id, symbol, name, system, dimension, scale, ExactRational.Zero,
```

### src/projects/dw.quantities/ApproximateEquivalence.cs

```text
3: public readonly record struct ApproximationTolerance
5: public ApproximationTolerance(ExactRational absolute, ExactRational relative)
7: if (absolute < ExactRational.Zero)
12: if (relative < ExactRational.Zero)
21: public ExactRational Absolute { get; }
23: public ExactRational Relative { get; }
25: public static ApproximationTolerance Exact { get; } =
26: new(ExactRational.Zero, ExactRational.Zero);
29: public static class ApproximateEquivalence
31: public static bool AreEquivalent(
32: ExactRational left,
33: ExactRational right,
42: public static bool AreEquivalent(
48: private static ExactRational Max(ExactRational left, ExactRational right) =>
```

### src/projects/dw.quantities/ExactBinaryNumber.cs

```text
6: public static class ExactBinaryNumber
8: public static ExactRational FromDouble(double value)
11: var bits = BitConverter.DoubleToUInt64Bits(value);
18: ? new ExactRational(significand << power, BigInteger.One)
19: : new ExactRational(significand, BigInteger.One << -power);
```

### src/projects/dw.quantities/ExactComparisonPolicy.cs

```text
6: /// relative tolerance is a non-negative dimensionless exact rational.
8: public readonly record struct QuantityTolerance(Quantity Absolute, ExactRational Relative)
10: public static QuantityTolerance FromQuantities(Quantity absolute, Quantity relative)
14: if (relative.Value < ExactRational.Zero)
20: public static class ExactQuantityComparison
22: public static bool AreEquivalent(Quantity left, Quantity right, QuantityTolerance tolerance)
26: if (tolerance.Relative < ExactRational.Zero)
28: if (tolerance.Absolute.Value < ExactRational.Zero)
33: tolerance.Absolute.Value == ExactRational.Zero))
44: /// Tagged temperatures must already be expressed on a common base scale
48: public static class ExactTemperatureComparison
50: public static bool AreEquivalent(
54: ExactRational relativeTolerance)
64: if (absoluteTolerance.Value < ExactRational.Zero)
66: if (relativeTolerance < ExactRational.Zero)
82: public static bool AreWithin(
83: ExactRational left, ExactRational right,
84: ExactRational absoluteTolerance, ExactRational relativeTolerance)
```

### src/projects/dw.quantities/ExactDecimalFormatter.cs

```text
6: public enum DecimalRoundingMode
15: public static class ExactDecimalFormatter
17: public const int MaximumDecimalPlaces = 100;
19: public static string Format(ExactRational value, int decimalPlaces, DecimalRoundingMode rounding)
32: var scale = BigInteger.Pow(10, decimalPlaces);
34: BigInteger.Abs(value.Numerator) * scale,
```

### src/projects/dw.quantities/ExactRational.cs

```text
6: public readonly record struct ExactRational : IComparable<ExactRational>
8: public ExactRational(BigInteger numerator, BigInteger denominator)
12: throw new DivideByZeroException("An exact rational denominator cannot be zero.");
26: public BigInteger Numerator { get; }
28: // Store denominator minus one so default(ExactRational) is canonical zero (0/1).
29: // This also preserves value equality with ExactRational.Zero.
32: public BigInteger Denominator => _denominatorMinusOne + BigInteger.One;
34: public static ExactRational Zero { get; } = new(BigInteger.Zero, BigInteger.One);
36: public static ExactRational One { get; } = new(BigInteger.One, BigInteger.One);
38: public ExactRational Abs() => Numerator.Sign < 0
39: ? new ExactRational(BigInteger.Abs(Numerator), Denominator)
42: public int CompareTo(ExactRational other) =>
45: public static ExactRational ParseInvariantDecimal(string value)
78: return new ExactRational(numerator, BigInteger.Pow(10, fractional.Length));
81: public static ExactRational ParseInvariant(string value)
96: public string ToFractionString() => Denominator.IsOne
102: public string ToMixedNumberString()
125: public static ExactRational operator +(ExactRational left, ExactRational right) =>
130: public static ExactRational operator -(ExactRational left, ExactRational right) =>
135: public static ExactRational operator *(ExactRational left, ExactRational right) =>
138: public static ExactRational operator /(ExactRational left, ExactRational right)
142: throw new DivideByZeroException("Cannot divide an exact rational by zero.");
145: return new ExactRational(
150: public static bool operator <(ExactRational left, ExactRational right) =>
153: public static bool operator <=(ExactRational left, ExactRational right) =>
156: public static bool operator >(ExactRational left, ExactRational right) =>
159: public static bool operator >=(ExactRational left, ExactRational right) =>
162: private static ExactRational ParseSimpleFraction(string value)
173: return new ExactRational(numerator, denominator);
176: private static ExactRational ParseMixedNumber(string wholeText, string fractionText)
193: var magnitude = new ExactRational(
197: ? new ExactRational(BigInteger.Negate(magnitude.Numerator), magnitude.Denominator)
```

### src/projects/dw.quantities/ExactRationalBudget.cs

```text
6: /// Explicit resource policy for exact operations. Unbudgeted ExactRational arithmetic is exact but unbounded;
9: public static class ExactRationalBudget
11: public static ExactRational Add(ExactRational left, ExactRational right, int maxBits)
29: return new ExactRational(numerator, denominator);
32: public static ExactRational Multiply(ExactRational left, ExactRational right, int maxBits)
39: return new ExactRational(left.Numerator * right.Numerator, left.Denominator * right.Denominator);
48: private static void RequireWithin(ExactRational value, int bits)
57: throw new ArithmeticException("Exact rational exceeds the configured bit budget.");
```

### src/projects/dw.quantities/Quantity.cs

```text
3: public readonly record struct Quantity(ExactRational Value, DimensionVector Dimension)
5: public bool TryAdd(Quantity other, out Quantity result)
17: public Quantity Multiply(Quantity other) =>
20: public Quantity Divide(Quantity other) =>
```

### src/projects/dw.quantities/UnitDefinition.cs

```text
5: public enum UnitSystem
15: public enum UnitTransformKind
21: public sealed record UnitDefinition
23: public UnitDefinition(
29: ExactRational scaleToBase,
30: ExactRational offsetToBase,
37: if (scaleToBase.Numerator.IsZero)
39: throw new ArgumentOutOfRangeException(nameof(scaleToBase), "A unit scale cannot be zero.");
47: ScaleToBase = scaleToBase;
56: public string Id { get; }
57: public string Symbol { get; }
58: public string Name { get; }
59: public UnitSystem System { get; }
60: public DimensionVector Dimension { get; }
61: public ExactRational ScaleToBase { get; }
62: public ExactRational OffsetToBase { get; }
63: public UnitTransformKind TransformKind { get; }
64: public ImmutableArray<string> Aliases { get; }
66: public ExactRational ToBase(ExactRational value) => value * ScaleToBase + OffsetToBase;
68: public ExactRational FromBase(ExactRational value) => (value - OffsetToBase) / ScaleToBase;
75: public TemperatureMeasurement ToBase(TemperatureMeasurement value)
81: TemperatureKind.Interval => TemperatureMeasurement.Interval(value.Value * ScaleToBase),
86: public TemperatureMeasurement FromBase(TemperatureMeasurement value)
92: TemperatureKind.Interval => TemperatureMeasurement.Interval(value.Value / ScaleToBase),
101: if (TransformKind == UnitTransformKind.Linear && OffsetToBase != ExactRational.Zero)
106: public enum TemperatureKind
116: public readonly record struct TemperatureMeasurement(ExactRational Value, TemperatureKind Kind)
118: public static TemperatureMeasurement Absolute(ExactRational value) => new(value, TemperatureKind.Absolute);
119: public static TemperatureMeasurement Interval(ExactRational value) => new(value, TemperatureKind.Interval);
121: public TemperatureMeasurement Add(TemperatureMeasurement other)
133: public TemperatureMeasurement Subtract(TemperatureMeasurement other)
```

### src/projects/dw.tools.math.analysis/CurveBoundaryAudit64.cs

```text
4: public enum CurveExtrapolation64 { Refuse, LinearSegment }
7: public readonly record struct CurveAxisUnits64(string XUnit,string YUnit);
10: public sealed record CurveBoundaryPolicy64(
14: public enum CurveBoundaryStatus64
22: public sealed record CurveBoundaryValue64(
28: public bool Admitted =>
38: public sealed record CurveBoundaryFit64(
43: public bool Admitted =>
59: public static class CurveBoundaryAudit64
61: public const string RuleId="dw-math-curve-boundaries-binary64-1";
62: public const int MaximumUnitSymbolCharacters=24;
65: public static CurveBoundaryValue64 EvaluateLinear(
66: IReadOnlyList<CurvePoint64>? samples,double query,
118: public static CurveBoundaryFit64 FitDescriptive(
119: IReadOnlyList<CurvePoint64>? samples,CurveAxisUnits64 units,
138: IReadOnlyList<CurvePoint64>? input,CurveBoundaryPolicy64 policy,
139: out CurvePoint64[] points,out double relative)
146: var copy=new CurvePoint64[input.Count];
162: var scale=global::System.Math.Max(1d,
166: if(!double.IsFinite(span)||!double.IsFinite(scale))
168: relative=span/scale;
```

### src/projects/dw.tools.math.analysis/CurvePoint64.cs

```text
4: public readonly record struct CurvePoint64(double X,double Y);
```

### src/projects/dw.tools.math.analysis/M6W01C01Contract.cs

```text
4: public enum CurveAnalysisStatus64
16: public sealed record CurveValueResult64(
19: public bool Succeeded => Status==CurveAnalysisStatus64.Success &&
27: public sealed record CurveFitResult64(
31: public bool Succeeded =>
45: public static class BoundedCurveAnalysis64
47: public const string RuleId="dw-math-curve-binary64-1";
48: public const int MaximumSamples=32;
50: public static CurveValueResult64 InterpolateLinear(
51: IReadOnlyList<CurvePoint64>? samples,double x)
58: public static CurveValueResult64 InterpolateNaturalSpline(
59: IReadOnlyList<CurvePoint64>? samples,double x)
114: public static CurveFitResult64 FitLeastSquares(
115: IReadOnlyList<CurvePoint64>? samples)
157: private static CurveValueResult64 Linear(CurvePoint64[] points,double x)
179: private static int FindSegment(CurvePoint64[] points,double x)
188: IReadOnlyList<CurvePoint64>? input,out CurvePoint64[] points)
195: var copy=new CurvePoint64[input.Count];
```

### src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs

```text
9: public enum LatexImportStatus
20: public enum LatexJuxtapositionConvention
31: public sealed record LatexImportResult(
39: public bool IsAdmitted =>
47: /// Non-executing bounded LaTeX importer for selected exact rational,
51: public static class BoundedLatexImporter
53: public const string Rule = "math-bounded-latex-exact-subset/v1";
54: public const int MaximumInputCharacters = 512;
55: public const int MaximumDepth = 12;
56: public const int MaximumExactBits = 128;
58: public static LatexImportResult Import(
199: return new IrExactScalar(new ExactRational(
235: new ExactRational(ReadInteger(true), BigInteger.One));
258: private ExactRational ReadFraction()
272: return new ExactRational(numerator, denominator);
```

### src/projects/dw.tools.math.codecs/CanonicalDisplayBoundaryAudit.cs

```text
10: public enum CanonicalDisplayBoundaryStatus
26: public sealed record CanonicalDisplayBoundaryResult(
36: public bool IsAdmitted =>
43: public bool PresentationMayCollide =>
53: public static class CanonicalDisplayBoundaryAudit
55: public const string Rule = "canonical-safe-math-display-domain-audit/v1";
56: public const int MaximumDisplayCharacters = 4096;
57: public const int MaximumBraceDepth = 64;
59: public static CanonicalDisplayBoundaryResult AuditText(string canonicalText)
66: public static CanonicalDisplayBoundaryResult AuditIr(IrNode original)
196: exponent.Value == new ExactRational(2,1):
```

### src/projects/dw.tools.math.codecs/CanonicalMathNotation.cs

```text
9: public enum CanonicalNotationStatus
26: public sealed record CanonicalNotationResult(
33: public bool IsAdmitted =>
41: /// ASCII-only: exact rationals, free real x/y/z, prefix add/sub/mul/div,
45: public static class CanonicalMathNotation
47: public const string Grammar = "dw-math-canonical-text/1";
48: public const string LatexRule = "dw-math-latex-display-only/1";
49: public const int MaximumInputCharacters = 1024;
50: public const int MaximumLatexCharacters = 4096;
51: public const int MaximumDepth = 16;
52: public const int MaximumRationalBits = 128;
53: public const int MaximumArguments = 8;
54: public const int MaximumConditions = 8;
55: public const int MaximumPower = 8;
61: public static CanonicalNotationResult Parse(string canonicalText)
106: public static CanonicalNotationResult Render(IrNode source)
221: private static bool Bounded(ExactRational value) =>
222: BigInteger.Abs(value.Numerator).GetBitLength() <= MaximumRationalBits &&
223: value.Denominator.GetBitLength() <= MaximumRationalBits;
253: case IrExactScalar rational:
254: return rational.Value.ToFractionString();
307: case IrExactScalar rational:
309: var value = rational.Value;
395: return new IrExactScalar(ReadRational());
516: throw new FormatException("An assumption needs an exact rational.");
528: private ExactRational ReadRational()
534: throw new FormatException("An exact rational requires digits.");
536: throw new FormatException("Missing positive rational denominator.");
537: var value = ExactRational.ParseInvariant(_text[start.._position]);
540: "Exact rational numerator or denominator exceeds the grammar bit budget.");
```

### src/projects/dw.tools.math.codecs/ExactMathMlInterop.cs

```text
9: public enum MathMlInteropStatus
25: public sealed record MathMlInteropResult(
35: public bool IsAdmitted =>
49: public static class ExactMathMlInterop
51: public const string Rule = "dw-math-mathml3-annotated-ir/1";
52: public const string Namespace = "http://www.w3.org/1998/Math/MathML";
53: public const string ContentEncoding = "application/mathml-content+xml";
54: public const string TextEncoding = "application/x-dw-math-canonical-text-v1";
55: public const int MaximumXmlCharacters = 4096;
56: public const int MaximumXmlNodes = 32;
57: public const int MaximumXmlDepth = 12;
62: public static MathMlInteropResult Render(IrNode expression)
119: public static MathMlInteropResult ImportAnnotated(string xml)
251: "Only exact rationals, three real symbols, simple sums and a restricted equality are in the T1 MathML subset.");
```

### src/projects/dw.tools.math.composition/ExactQuantityPipeline.cs

```text
7: public enum ExactPipelineStepKind { Start, Convert, Divide, Add }
9: public sealed record ExactPipelineStep
11: private ExactPipelineStep(ExactPipelineStepKind kind, ExactRational amount, UnitDefinition unit)
18: public ExactPipelineStepKind Kind { get; }
19: public ExactRational Amount { get; }
20: public UnitDefinition Unit { get; }
22: public static ExactPipelineStep Start(ExactRational amount, UnitDefinition unit) =>
25: public static ExactPipelineStep Convert(UnitDefinition unit) =>
26: new(ExactPipelineStepKind.Convert, ExactRational.Zero, Require(unit));
28: public static ExactPipelineStep Divide(ExactRational amount, UnitDefinition unit) =>
31: public static ExactPipelineStep Add(ExactRational amount, UnitDefinition unit) =>
39: public sealed record ExactPipelineStepSnapshot
41: internal ExactPipelineStepSnapshot(int index, ExactPipelineStepKind kind,
50: public int Index { get; }
51: public ExactPipelineStepKind Kind { get; }
52: public Quantity BaseQuantity { get; }
53: public string? PresentationUnitId { get; }
57: public sealed record ExactPipelineResult
61: ImmutableArray<ExactPipelineStepSnapshot> detailedSteps)
69: public Quantity BaseQuantity { get; }
70: public UnitDefinition? PresentationUnit { get; }
71: public ExactRational DisplayValue => PresentationUnit is null
73: public ImmutableArray<ExactPipelineStepKind> Steps { get; }
74: public ImmutableArray<ExactPipelineStepSnapshot> DetailedSteps { get; }
81: public sealed class ExactQuantityPipeline
83: public const int MaximumSteps = 16;
84: public const int MaximumNumeralDigits = 256;
89: public static ExactQuantityPipeline From(ExactRational amount, UnitDefinition unit) =>
92: public ExactQuantityPipeline ConvertTo(UnitDefinition unit) => Append(ExactPipelineStep.Convert(unit));
93: public ExactQuantityPipeline DivideBy(ExactRational amount, UnitDefinition unit) =>
95: public ExactQuantityPipeline Add(ExactRational amount, UnitDefinition unit) =>
98: public ExactPipelineResult Evaluate() => Evaluate(_steps, CancellationToken.None);
100: public ExactPipelineResult Evaluate(CancellationToken cancellationToken) =>
103: public static ExactPipelineResult Evaluate(IEnumerable<ExactPipelineStep> steps) =>
110: public static ExactPipelineResult Evaluate(IEnumerable<ExactPipelineStep> steps,
122: var observations = ImmutableArray.CreateBuilder<ExactPipelineStepSnapshot>(finite.Length);
123: observations.Add(new ExactPipelineStepSnapshot(0, ExactPipelineStepKind.Start,
150: observations.Add(new ExactPipelineStepSnapshot(i, step.Kind,
195: if (step.Amount == ExactRational.Zero)
208: unit.OffsetToBase != ExactRational.Zero)
210: CheckMagnitude(unit.ScaleToBase);
213: private static void CheckMagnitude(ExactRational value)
```

### src/projects/dw.tools.math.composition/ExactReplayContracts.cs

```text
10: public sealed record ExactReplayContext
22: public string Assumptions { get; }
23: public string Policy { get; }
24: public string Catalog { get; }
25: public string Tolerance { get; }
26: public string Provider { get; }
28: public static ExactReplayContext Create(string assumptions, string policy, string catalog,
44: public sealed record ExactReplayKey
47: public const string Scheme = "exact-quantity-replay/1-sha256";
48: public string Digest { get; }
80: Rational(writer, unit.ScaleToBase);
81: Rational(writer, unit.OffsetToBase);
82: Rational(writer, step.Amount);
88: private static void Rational(BinaryWriter writer, ExactRational value)
110: public sealed class ExactReplayCache
112: public const int MaximumEntries = 32;
117: public ExactReplayCache(int capacity = 16)
124: public int Capacity { get; }
126: public int Count
148: public void Clear()
159: public sealed record ExactReplayReceipt
168: public ExactReplayKey Key { get; }
169: public ExactPipelineResult Result { get; }
170: public bool CacheHit { get; }
174: public enum ExactReplayStatus { Exact, Unsupported, BudgetExceeded, Cancelled }
177: public sealed record ExactReplayAttempt
186: public ExactReplayStatus Status { get; }
187: public ExactReplayReceipt? Receipt { get; }
188: public string? Reason { get; }
189: public bool HasFinalValue => Status == ExactReplayStatus.Exact && Receipt is not null;
203: public static class ExactReplayRunner
206: public static ExactReplayReceipt Run(IEnumerable<ExactPipelineStep> steps,
231: public static ExactReplayAttempt TryRun(IEnumerable<ExactPipelineStep> steps,
```

### src/projects/dw.tools.math.distribution/ExactMathVerticalRunner.cs

```text
17: public static class ExactMathVerticalRunner
19: public const string Rule = "m5-exact-numeric-symbolic-public-vertical/1";
20: public const string Provider = "dw.tools.math/managed-qualified-engines";
21: public const string PointRule =
29: public static ExactVerticalResult Run(string canonicalSource,
30: ExactRational evaluationPoint,
32: RationalBinary64Policy numericPolicy = RationalBinary64Policy.RequireExact)
106: new IrExactScalar(ExactRational.Zero),variable,candidates.Roots);
114: var binary=RationalBinary64Conversion.TryConvert(value.Value.Value);
117: return Refuse(binary.Status==RationalBinary64Status.BudgetExceeded
119: if (binary.Status==RationalBinary64Status.Rounded &&
120: numericPolicy==RationalBinary64Policy.RequireExact)
158: LossClassification=binary.Status==RationalBinary64Status.Rounded
```

### src/projects/dw.tools.math.distribution/ExactVerticalBoundaryAudit.cs

```text
13: public enum ExactVerticalBoundaryStatus
19: public enum ExactVerticalBoundaryLossKind
30: public sealed record ExactVerticalBoundaryResult(
47: public bool IsAdmitted =>
62: public static class ExactVerticalBoundaryAudit
64: public const string Rule = "m5-exact-numeric-symbolic-vertical-boundary/1";
65: public const int MaximumInputCharacters = CanonicalMathNotation.MaximumInputCharacters;
67: public static ExactVerticalBoundaryResult Audit(
68: string canonicalSource, ExactRational evaluationPoint,
70: RationalBinary64Policy numericPolicy = RationalBinary64Policy.RequireExact)
162: new IrExactScalar(ExactRational.Zero),variable,unrestricted.Roots);
195: var converted=RationalBinary64Conversion.TryConvert(point.Value.Value);
200: BitConverter.DoubleToInt64Bits(approximateValue.Value)!=
201: BitConverter.DoubleToInt64Bits(converted.Value.Value) ||
202: (numericPolicy==RationalBinary64Policy.RequireExact &&
203: converted.Status==RationalBinary64Status.Rounded))
206: var numericLoss=converted.Status==RationalBinary64Status.Rounded
```

### src/projects/dw.tools.math.distribution/ExactVerticalContracts.cs

```text
9: /// <summary>One bounded public exact vertical or a typed non-admission.</summary>
10: public enum ExactVerticalStatus
26: public sealed record ExactVerticalResult
28: public ExactVerticalStatus Status { get; init; }
29: public IrRestrictedExpression? Original { get; init; }
30: public IrNode? FormalBodyDerivative { get; init; }
31: public SymbolicRootAuditResult? OriginalRootAudit { get; init; }
32: public SymbolicRealSolvingResult? UnrestrictedCandidates { get; init; }
33: public ExactRational? ExactValueAtPoint { get; init; }
34: public RationalBinary64Result? Binary64Projection { get; init; }
35: public MathOperationResult? ExactComposition { get; init; }
36: public MathOperationResult? Binary64Composition { get; init; }
37: public string? CanonicalSource { get; init; }
38: public string? CanonicalDerivative { get; init; }
39: public string? SourceLatex { get; init; }
40: public string? SourceCanonicalIrJson { get; init; }
41: public string? OriginalSemanticSha256 { get; init; }
42: public string? FormalDerivativeRule { get; init; }
43: public string? PointEvaluationRule { get; init; }
44: public string? ProviderId { get; init; }
45: public string? OriginalAssumption { get; init; }
46: public string? LossClassification { get; init; }
47: public string? RuleId { get; init; }
49: public bool IsAdmitted =>
```

### src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs

```text
10: public static class IrCanonicalJsonCodec
12: public const string Version = "math-ir/1";
13: public const int MaximumJsonCharacters = 262144;
15: public static string Encode(IrNode root)
30: public static IrNode Decode(string json)
145: throw new FormatException("Invalid exact rational numerator or denominator.");
146: var value = new ExactRational(numerator, denominator);
148: throw new FormatException("Noncanonical exact rational is not admitted.");
161: BitConverter.Int64BitsToDouble(unchecked((long)unsigned)));
```

### src/projects/dw.tools.math.ir/IrCanonicalJsonGuard.cs

```text
8: public const int MaximumIntegerDigits = 1024;
10: public static void Validate(JsonElement root)
19: public static void ValidateRestrictions(IrNode root)
124: throw new FormatException("An exact rational component must be a decimal string.");
127: throw new FormatException("Exact rational numeral exceeds admitted digits.");
```

### src/projects/dw.tools.math.ir/IrModel.cs

```text
7: public abstract record IrNode;
8: public abstract record IrScalar : IrNode;
10: public sealed record IrExactScalar(ExactRational Value) : IrScalar;
12: public sealed record IrFiniteBinary64Scalar : IrScalar
15: public long Ieee754Bits { get; }
16: public double Value => BitConverter.Int64BitsToDouble(Ieee754Bits);
18: public static IrFiniteBinary64Scalar FromDouble(double value)
22: return new(BitConverter.DoubleToInt64Bits(value));
26: public enum IrScalarDomain { Real, Integer }
27: public enum IrTemperatureKind { None, Absolute, Interval }
29: public sealed record IrSymbol : IrNode
41: public string Identity { get; }
42: public string DisplayName { get; }
43: public IrScalarDomain Domain { get; }
44: public string? ScopeId { get; }
45: public int Slot { get; }
46: public bool IsBound => ScopeId is not null;
48: public static IrSymbol Free(string identity, string displayName,
57: public static IrSymbol Bound(string scopeId, int slot, string displayName,
82: public sealed record IrQuantityLiteral : IrNode
84: public IrQuantityLiteral(IrScalar value, string canonicalUnitId, UnitSystem system,
97: public IrScalar Value { get; }
98: public string CanonicalUnitId { get; }
99: public UnitSystem System { get; }
100: public DimensionVector Dimension { get; }
101: public IrTemperatureKind Temperature { get; }
104: public enum IrOperation { Add, Subtract, Multiply, Divide, Power, Square, Sqrt, Abs, Min, Max }
106: public sealed record IrApply : IrNode
114: public IrOperation Operation { get; }
115: public ImmutableArray<IrNode> Arguments { get; }
117: public static IrApply Create(IrOperation operation, IEnumerable<IrNode> arguments)
134: public enum IrRelationKind { Equal, NotEqual, GreaterOrEqual }
136: public sealed record IrRelation
138: public IrRelation(IrSymbol symbol, IrRelationKind kind, IrExactScalar right)
147: public IrSymbol Symbol { get; }
148: public IrRelationKind Kind { get; }
149: public IrExactScalar Right { get; }
152: public sealed record IrAssumptionSet
155: public ImmutableArray<IrRelation> Conditions { get; }
156: public static IrAssumptionSet Empty { get; } = new(ImmutableArray<IrRelation>.Empty);
157: public static IrAssumptionSet Create(IEnumerable<IrRelation> conditions)
163: var unique = new HashSet<(string, IrScalarDomain, IrRelationKind, ExactRational)>();
171: public bool DeclaresNonNegative(IrSymbol symbol) =>
175: x.Right.Value == ExactRational.Zero);
178: public sealed record IrRestrictedExpression : IrNode
180: public IrRestrictedExpression(IrNode expression, IrAssumptionSet assumptions)
188: public IrNode Expression { get; }
189: public IrAssumptionSet Assumptions { get; }
193: public static class IrRealDomainRules
195: public static IrNode NormalizeSqrtOfSquare(IrApply squareRoot, IrAssumptionSet assumptions)
```

### src/projects/dw.tools.math.numerics/BoundedLinearAlgebra64.cs

```text
6: public enum LinearAlgebraStatus64
16: public sealed record LuFactorization64(
24: public bool Factored => Status == LinearAlgebraStatus64.Success &&
29: public sealed record LinearSolveResult64(
36: public bool HasUniqueSolution => Status == LinearAlgebraStatus64.Success &&
44: public double? TwoByTwoSpectralConditionEstimate { get; init; }
48: public sealed record QrFactorization64(
54: public bool Factored => Status == LinearAlgebraStatus64.Success &&
59: public sealed record SingularSpectrum2x2Result64(
69: public static class BoundedLinearAlgebra64
71: public const int MaximumDimension = 16;
72: public const double DefaultRelativeThreshold = 1e-12;
74: public static LuFactorization64 FactorLu(
152: public static LinearSolveResult64 Solve(
232: public static QrFactorization64 FactorQr(
261: var scaled = r[i, k] / maxInColumn;
262: sumSquares += scaled * scaled;
282: var scale = beta * dot;
285: r[i, j] -= scale * v[i - k];
296: var scale = beta * dot;
299: q[i, j] -= scale * v[j - k];
320: public static SingularSpectrum2x2Result64 SingularValues2x2(FiniteMatrix64 matrix)
355: var scale = global::System.Math.Max(global::System.Math.Abs(a), global::System.Math.Abs(b));
356: if (scale == 0d) return 0d;
357: var x = a / scale;
358: var y = b / scale;
359: var value = scale * global::System.Math.Sqrt(x * x + y * y);
```

### src/projects/dw.tools.math.numerics/NumericalMatrices.cs

```text
6: /// <summary>Explicitly approximate numerical values. No implicit exact-rational conversion.</summary>
7: public enum NumericalPrecisionKind { FiniteBinary64 }
10: public sealed class FiniteMatrix64
12: public const int MaximumElements = 4096;
13: public const int MaximumScalarProducts = 65536;
23: public int Rows { get; }
24: public int Columns { get; }
25: public NumericalPrecisionKind Precision => NumericalPrecisionKind.FiniteBinary64;
27: public static FiniteMatrix64 Create(int rows, int columns, IEnumerable<double> values)
40: public static FiniteMatrix64 FromProviderArrayCopy(double[,] values)
53: public double[,] ToProviderArrayCopy()
62: public double At(int row, int column)
71: public FiniteMatrix64 Add(FiniteMatrix64 other)
82: public FiniteMatrix64 Multiply(FiniteMatrix64 other)
106: public FiniteVector64 Multiply(FiniteVector64 vector)
137: public sealed class FiniteVector64
142: public int Length => _values.Length;
143: public NumericalPrecisionKind Precision => NumericalPrecisionKind.FiniteBinary64;
145: public static FiniteVector64 Create(IEnumerable<double> values)
156: public double At(int index)
164: public double Dot(FiniteVector64 other)
178: public sealed record SparseEntry64(int Row, int Column, double Value);
180: public sealed class SparseMatrix64
189: public int Rows { get; }
190: public int Columns { get; }
191: public int NonZeroCount => Entries.Length;
192: public ImmutableArray<SparseEntry64> Entries { get; }
194: public static SparseMatrix64 Create(int rows, int columns, IEnumerable<SparseEntry64> entries)
214: public FiniteMatrix64 ToDense()
225: /// a separate explicit rational-to-binary64 policy, never silent widening.
227: public static class NumericalMatrixTransport
229: public static IrMatrix ToApproximateIr(FiniteMatrix64 matrix)
240: public static FiniteMatrix64 FromApproximateIr(IrMatrix matrix)
244: throw new NotSupportedException("No implicit conversion from exact rational IR to binary64.");
```

### src/projects/dw.tools.math.numerics/RationalBinary64Conversion.cs

```text
8: public enum RationalBinary64Policy
15: public enum RationalBinary64Status
28: public sealed record RationalBinary64Result(
29: RationalBinary64Status Status,
31: ExactRational? AbsoluteError)
33: public bool IsFiniteResult => Value.HasValue &&
34: (Status is RationalBinary64Status.Exact or RationalBinary64Status.Rounded);
38: /// Bounded exact rational to correctly-rounded IEEE 754 binary64, ties to even.
41: public static class RationalBinary64Conversion
43: public const int MaximumOperandBits = 4096;
48: public static RationalBinary64Result TryConvert(ExactRational rational)
50: var numerator = rational.Numerator;
51: var denominator = rational.Denominator;
53: return new(RationalBinary64Status.Exact, 0d, ExactRational.Zero);
59: return Rejected(RationalBinary64Status.BudgetExceeded);
73: return Rejected(RationalBinary64Status.Overflow);
90: return Rejected(RationalBinary64Status.Overflow);
100: return Rejected(RationalBinary64Status.Underflow);
110: var rounded = BitConverter.UInt64BitsToDouble(bits);
112: return Rejected(RationalBinary64Status.Overflow);
114: var error = (ExactBinaryNumber.FromDouble(rounded) - rational).Abs();
115: return error == ExactRational.Zero
116: ? new(RationalBinary64Status.Exact, rounded, ExactRational.Zero)
117: : new(RationalBinary64Status.Rounded, rounded, error);
129: private static RationalBinary64Result Rejected(RationalBinary64Status status) =>
137: public sealed record ExactMatrixBinary64Result(
138: RationalBinary64Status Status,
141: ExactRational? MaximumAbsoluteError,
144: public bool Accepted => Matrix is not null;
147: public static class ExactMatrixBinary64Conversion
149: public static ExactMatrixBinary64Result TryConvert(
151: RationalBinary64Policy policy)
158: "Only exact rational IR matrices may use explicit rational conversion.");
162: var largestError = ExactRational.Zero;
167: var result = RationalBinary64Conversion.TryConvert(exact);
169: policy == RationalBinary64Policy.RequireExact &&
170: result.Status == RationalBinary64Status.Rounded)
174: if (result.Status == RationalBinary64Status.Rounded)
182: rounded == 0 ? RationalBinary64Status.Exact : RationalBinary64Status.Rounded,
```

### src/projects/dw.tools.math.scientific/FiniteGeometry64.cs

```text
6: public enum FiniteGeometryStatus64
16: public sealed record FiniteGeometryOutcome64(
20: public bool Succeeded
42: public static class FiniteGeometry64
44: public const string RuleId = "dw-math-finite-euclidean-1";
45: public const double MaxAbsCoordinate = 1e100d;
46: public const double MinNonzeroAxisMagnitude = 1e-100d;
48: public static FiniteGeometryOutcome64 Norm(
58: public static FiniteGeometryOutcome64 Distance(
76: public static FiniteGeometryOutcome64 Project(
86: var scale = 0d;
88: scale = global::System.Math.Max(scale, global::System.Math.Abs(component));
89: if (scale == 0d) return Refuse(FiniteGeometryStatus64.ZeroAxis);
90: if (scale < MinNonzeroAxisMagnitude)
99: normalizedAxis[i] = axis[i] / scale;
133: var scale = 0d;
135: scale = global::System.Math.Max(scale, global::System.Math.Abs(component));
136: if (scale == 0d) return 0d;
140: var normalized = component / scale;
143: return scale * global::System.Math.Sqrt(squares);
```

### src/projects/dw.tools.math.scientific/FiniteGeometryBoundaryAudit64.cs

```text
10: public enum FiniteGeometryAuditStatus64
27: public sealed record FiniteGeometryAuditResult64(
34: public bool Succeeded => Status == FiniteGeometryAuditStatus64.Success &&
44: public static class FiniteGeometryBoundaryAudit64
46: public const string RuleId = "dw-math-finite-euclidean-boundaries-1";
47: public const double MinRelativeDisplacement = 1e-12d;
48: public const double MinProjectionDotSignal = 1e-12d;
50: public static FiniteGeometryAuditResult64 EvaluateNorm(
59: public static FiniteGeometryAuditResult64 EvaluateDistance(
89: public static FiniteGeometryAuditResult64 EvaluateProjection(
102: var scale = 0d;
107: scale = global::System.Math.Max(scale, global::System.Math.Abs(component));
115: var term = vector![i] * (axis[i] / scale);
```

### src/projects/dw.tools.math.scientific/FiniteUniformSeries64.cs

```text
5: /// <summary>Nonpartial, bounded uniformly sampled series operation statuses.</summary>
6: public enum FiniteUniformSeriesStatus64
23: public sealed record FiniteUniformSeriesOutcome64(
24: FiniteUniformSeriesStatus64 Status,
31: public bool Succeeded
35: if (Status != FiniteUniformSeriesStatus64.Success ||
39: Samples.Length > FiniteUniformSeries64.MaxSamplesPerInput)
50: /// BCL-only bounded finite uniformly sampled first differences and sliding
55: public static class FiniteUniformSeries64
57: public const string RuleId = "dw-math-finite-uniform-series-1";
58: public const int MaxSamplesPerInput = 32;
59: public const double MaxAbsInputSample = 1e6d;
60: public const double MaxAbsOutputSample = 2e6d;
66: public static FiniteUniformSeriesOutcome64 FirstDifferences(
71: return Refuse(FiniteUniformSeriesStatus64.Cancelled);
73: if (status != FiniteUniformSeriesStatus64.Success)
80: return Refuse(FiniteUniformSeriesStatus64.Cancelled);
83: return Refuse(FiniteUniformSeriesStatus64.NonFiniteResult);
85: return Refuse(FiniteUniformSeriesStatus64.OutsideQualifiedRange);
89: return Refuse(FiniteUniformSeriesStatus64.Cancelled);
90: return new(FiniteUniformSeriesStatus64.Success, differences,
99: public static FiniteUniformSeriesOutcome64 MovingAverage(
104: return Refuse(FiniteUniformSeriesStatus64.Cancelled);
106: if (status != FiniteUniformSeriesStatus64.Success)
109: return Refuse(FiniteUniformSeriesStatus64.InvalidWindow);
115: return Refuse(FiniteUniformSeriesStatus64.Cancelled);
120: return Refuse(FiniteUniformSeriesStatus64.Cancelled);
124: return Refuse(FiniteUniformSeriesStatus64.NonFiniteResult);
127: return Refuse(FiniteUniformSeriesStatus64.NonFiniteResult);
129: return Refuse(FiniteUniformSeriesStatus64.OutsideQualifiedRange);
133: return Refuse(FiniteUniformSeriesStatus64.Cancelled);
134: return new(FiniteUniformSeriesStatus64.Success, means,
138: private static FiniteUniformSeriesStatus64 Validate(
143: return FiniteUniformSeriesStatus64.InvalidLength;
145: return FiniteUniformSeriesStatus64.InvalidSampleInterval;
149: return FiniteUniformSeriesStatus64.InvalidInput;
151: return FiniteUniformSeriesStatus64.OutsideQualifiedRange;
153: return FiniteUniformSeriesStatus64.Success;
156: private static FiniteUniformSeriesOutcome64 Refuse(
157: FiniteUniformSeriesStatus64 status) =>
```

### src/projects/dw.tools.math.scientific/FiniteUniformSeriesBoundaryAudit64.cs

```text
9: public enum FiniteUniformSeriesAuditStatus64
30: /// A successful result contains one already-qualified uniform series outcome.
34: public sealed record FiniteUniformSeriesAuditOutcome64(
35: FiniteUniformSeriesAuditStatus64 Status,
36: FiniteUniformSeriesOutcome64? Outcome,
41: public bool Succeeded =>
42: Status == FiniteUniformSeriesAuditStatus64.Success &&
49: /// Bounded sidecar over FiniteUniformSeries64, using explicit finite uniform
51: /// resolution/cancellation risks. It never interpolates, rescales samples,
54: public static class FiniteUniformSeriesBoundaryAudit64
56: public const string RuleId = "dw-math-finite-uniform-series-boundaries-1";
57: public const double MaxAbsTimeCoordinate = 1e15d;
58: public const double MaxRelativeIntervalDeviation = 1e-10d;
59: public const double MinRelativeTimeStep = 1e-12d;
60: public const double MinRelativeDifferenceSignal = 1e-12d;
61: public const double MinMeanSignalRatio = 1e-12d;
63: public static FiniteUniformSeriesAuditOutcome64 EvaluateFirstDifferences(
68: return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
69: var outcome = FiniteUniformSeries64.FirstDifferences(
73: if (timing != FiniteUniformSeriesAuditStatus64.Success)
80: return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
81: var scale = global::System.Math.Max(
85: if (delta == 0d || scale == 0d) continue;
86: var signal = global::System.Math.Abs(delta) / scale;
88: return Refuse(FiniteUniformSeriesAuditStatus64.NonFiniteResult);
92: return Refuse(FiniteUniformSeriesAuditStatus64.RelativeDifferenceRisk, signal);
97: public static FiniteUniformSeriesAuditOutcome64 EvaluateMovingAverage(
102: return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
103: var outcome = FiniteUniformSeries64.MovingAverage(
107: if (timing != FiniteUniformSeriesAuditStatus64.Success)
114: return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
119: return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
123: return Refuse(FiniteUniformSeriesAuditStatus64.NonFiniteResult);
128: return Refuse(FiniteUniformSeriesAuditStatus64.NonFiniteResult);
132: return Refuse(FiniteUniformSeriesAuditStatus64.MeanCancellationRisk, signal);
137: private static FiniteUniformSeriesAuditStatus64 ValidateTimes(
141: return FiniteUniformSeriesAuditStatus64.Cancelled;
142: if (times is null) return FiniteUniformSeriesAuditStatus64.SampleTimesRequired;
144: return FiniteUniformSeriesAuditStatus64.InvalidTimeLength;
148: return FiniteUniformSeriesAuditStatus64.Cancelled;
150: return FiniteUniformSeriesAuditStatus64.InvalidTimeValue;
152: return FiniteUniformSeriesAuditStatus64.OutsideQualifiedRange;
157: return FiniteUniformSeriesAuditStatus64.Cancelled;
163: return FiniteUniformSeriesAuditStatus64.TimestampResolutionRisk;
166: return FiniteUniformSeriesAuditStatus64.NonIncreasingTime;
168: return FiniteUniformSeriesAuditStatus64.NonFiniteResult;
172: return FiniteUniformSeriesAuditStatus64.IrregularSampling;
174: return FiniteUniformSeriesAuditStatus64.Success;
177: private static FiniteUniformSeriesAuditOutcome64 Success(
178: FiniteUniformSeriesOutcome64 source, double? ratio, CancellationToken token)
181: return Refuse(FiniteUniformSeriesAuditStatus64.Cancelled);
184: return Refuse(FiniteUniformSeriesAuditStatus64.NonFiniteResult);
185: return new(FiniteUniformSeriesAuditStatus64.Success, source,
189: private static FiniteUniformSeriesAuditStatus64 Map(FiniteUniformSeriesStatus64 status) =>
192: FiniteUniformSeriesStatus64.InvalidLength =>
193: FiniteUniformSeriesAuditStatus64.InvalidLength,
194: FiniteUniformSeriesStatus64.InvalidSampleInterval =>
195: FiniteUniformSeriesAuditStatus64.InvalidSampleInterval,
196: FiniteUniformSeriesStatus64.InvalidWindow =>
197: FiniteUniformSeriesAuditStatus64.InvalidWindow,
198: FiniteUniformSeriesStatus64.InvalidInput =>
199: FiniteUniformSeriesAuditStatus64.InvalidInput,
200: FiniteUniformSeriesStatus64.OutsideQualifiedRange =>
201: FiniteUniformSeriesAuditStatus64.OutsideQualifiedRange,
202: FiniteUniformSeriesStatus64.NonFiniteResult =>
203: FiniteUniformSeriesAuditStatus64.NonFiniteResult,
204: FiniteUniformSeriesStatus64.Cancelled =>
205: FiniteUniformSeriesAuditStatus64.Cancelled,
206: _ => FiniteUniformSeriesAuditStatus64.NonFiniteResult
209: private static FiniteUniformSeriesAuditOutcome64 Refuse(
210: FiniteUniformSeriesAuditStatus64 status, double? signal = null) =>
```

### src/projects/dw.tools.math.scientific/M6W02C03BoundaryAggregate64.cs

```text
7: /// signal interpretation, uniform-series interval and independent value units.
10: public enum M6W02C03UnitPolicy64
20: public sealed record M6W02C03BoundaryRequest64(
31: public enum M6W02C03BoundaryStatus64
38: UniformSeriesRefused,
46: public sealed record M6W02C03BoundaryOutcome64(
53: FiniteUniformSeriesAuditOutcome64? Differences,
54: FiniteUniformSeriesAuditOutcome64? MovingAverage,
57: FiniteUniformSeriesAuditStatus64? UniformSeriesReason,
61: public bool Succeeded =>
65: UniformSeriesReason is null &&
77: /// Euclidean, S02 direct discrete-signal and S03 uniform-grid numerical
81: public static class M6W02C03BoundaryAggregate64
83: public const string RuleId = "dw-math-m6-w02-c03-cross-family-boundaries-1";
85: public static M6W02C03BoundaryOutcome64 Evaluate(
123: var differences = FiniteUniformSeriesBoundaryAudit64.EvaluateFirstDifferences(
124: input.UniformSamples, request.SampleTimes,
127: var average = FiniteUniformSeriesBoundaryAudit64.EvaluateMovingAverage(
128: input.UniformSamples, request.SampleTimes, input.SampleInterval,
152: FiniteUniformSeriesAuditStatus64 reason) =>
153: reason == FiniteUniformSeriesAuditStatus64.Cancelled
155: : Refuse(M6W02C03BoundaryStatus64.UniformSeriesRefused, series: reason);
161: FiniteUniformSeriesAuditStatus64? series = null) =>
```

### src/projects/dw.tools.math.scientific/M6W02C03Contract.cs

```text
12: public sealed record M6W02C03Request64(
19: double[]? UniformSamples,
24: public enum M6W02C03Status64
30: UniformSeriesRefused,
38: public sealed record M6W02C03Outcome64(
45: FiniteUniformSeriesOutcome64? Differences,
46: FiniteUniformSeriesOutcome64? MovingAverage,
50: public bool Succeeded =>
67: public static class M6W02C03Contract
69: public const string RuleId = "dw-math-m6-w02-c03-aggregate-1";
71: public static M6W02C03Outcome64 Evaluate(
100: var differences = FiniteUniformSeries64.FirstDifferences(
101: request.UniformSamples, request.SampleInterval, cancellationToken);
103: var average = FiniteUniformSeries64.MovingAverage(
104: request.UniformSamples, request.SampleInterval,
122: private static M6W02C03Status64 SelectSeries(FiniteUniformSeriesStatus64 status) =>
123: status == FiniteUniformSeriesStatus64.Cancelled
124: ? M6W02C03Status64.Cancelled : M6W02C03Status64.UniformSeriesRefused;
```

### src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs

```text
11: public sealed class ManagedBclSpecialFunctionsProvider64 : ISpecialFunctionsProvider64
13: public const string ProviderIdentity="dw-math-managed-bcl-special-1";
30: public string ProviderId => ProviderIdentity;
32: public SpecialRealResult64 EvaluateReal(
65: public SpecialComplexResult64 EvaluateComplex(
116: var accumulated=GammaCoefficients[0];
118: accumulated+=GammaCoefficients[i]/(shifted+i);
122: global::System.Math.Exp(-t)*accumulated;
135: public static class BoundedSpecialFunctions64
137: public const string RuleId="dw-math-bounded-special-functions-real-complex-1";
141: public static SpecialRealResult64 EvaluateReal(
146: public static SpecialComplexResult64 EvaluateComplex(
```

### src/projects/dw.tools.math.statistics/BernoulliDistribution64.cs

```text
6: /// Bounded finite binary64 Bernoulli mass, integer-support CDF, inverse-CDF
7: /// quantile and deterministic sampling, without an external RNG provider.
9: public sealed class BernoulliDistribution64
11: public BernoulliDistribution64(double probabilityOfOne)
16: "Bernoulli probability must be finite and in [0,1].");
20: public double ProbabilityOfOne { get; }
23: public double ProbabilityMass(int value) => value switch
31: public double CumulativeProbability(int value) => value switch
41: /// so p=1 has quantile 1 rather than a zero-mass value 0.
43: public int Quantile(double cumulativeProbability)
48: "Quantile probability must be finite and in [0,1].");
56: /// Consume exactly one SplitMix64 draw even when p is zero or one,
59: public int Sample(SplitMix64Random generator)
```

### src/projects/dw.tools.math.statistics/ContinuousUniformDistribution64.cs

```text
6: /// A continuous uniform law on finite [minimum, maximum], calculated with
9: public sealed class ContinuousUniformDistribution64
14: public ContinuousUniformDistribution64(double minimum, double maximum)
22: throw new ArithmeticException("Uniform width exceeds finite binary64.");
25: throw new ArithmeticException("Uniform density exceeds finite binary64.");
32: public double Minimum { get; }
33: public double Maximum { get; }
39: public double ProbabilityDensity(double value)
46: public double CumulativeProbability(double value)
56: /// Binary64 rounding may send an interior quantile to an endpoint.
58: public double Quantile(double probability)
62: "Quantile probability must be finite and in [0,1].");
67: throw new ArithmeticException("Uniform quantile overflowed binary64.");
75: public double Sample(SplitMix64Random generator)
78: var value = Quantile(generator.NextUnitInterval());
86: "Uniform evaluation requires finite binary64.");
```

### src/projects/dw.tools.math.statistics/CorrelationStatistics64.cs

```text
8: /// Immutable unweighted paired-sample covariance and Pearson correlation.
9: /// Each reported moment is approximate finite binary64, not an exact rational.
11: public sealed record FiniteBivariateStatistics64(
15: double PopulationCovariance,
16: double SampleCovariance,
19: public DescriptivePrecisionKind Precision => DescriptivePrecisionKind.FiniteBinary64;
23: /// Bounded two-pass centered covariance and Pearson correlation for two
27: public static class CorrelationStatistics64
29: public const int MaximumPairs = DescriptiveStatistics64.MaximumObservations;
31: public static FiniteBivariateStatistics64 Analyze(
51: DescriptiveStatistics64.Accumulate(
53: DescriptiveStatistics64.Accumulate(
71: DescriptiveStatistics64.Accumulate(
73: DescriptiveStatistics64.Accumulate(
75: DescriptiveStatistics64.Accumulate(
84: "Pearson correlation has no admitted nonzero variance; typed undefined results are reserved for T2.");
102: public static CorrelationOutcome64 AnalyzeWithStatus(
161: CorrelationDefinitionStatus64.UndefinedZeroVariance, 0d, 0d, null);
164: // covariance/Pearson implementation with divergent denominators.
168: defined.PopulationCovariance, defined.SampleCovariance,
175: public enum MissingPairPolicy64
182: public enum CorrelationDefinitionStatus64
185: UndefinedZeroVariance,
191: /// For a singleton, population covariance is zero and sample covariance is null.
192: /// With zero variance, both covariances are zero but Pearson is undefined.
194: public sealed record CorrelationOutcome64(
199: double? PopulationCovariance,
200: double? SampleCovariance,
203: public bool IsDefined => Status == CorrelationDefinitionStatus64.Defined;
204: public DescriptivePrecisionKind Precision => DescriptivePrecisionKind.FiniteBinary64;
```

### src/projects/dw.tools.math.statistics/DescriptiveStatistics64.cs

```text
6: public enum DescriptivePrecisionKind { FiniteBinary64 }
8: /// <summary>Finite approximate aggregates. Sample variance is absent for a singleton.</summary>
9: public sealed record FiniteDescriptiveStatistics64(
10: int Count, double Sum, double Mean, double PopulationVariance,
11: double? SampleVariance, double Minimum, double Maximum)
13: public DescriptivePrecisionKind Precision => DescriptivePrecisionKind.FiniteBinary64;
16: /// <summary>Rational aggregates never undergo silent binary64 conversion.</summary>
17: public sealed record ExactDescriptiveStatistics(
18: int Count, ExactRational Sum, ExactRational Mean,
19: ExactRational Minimum, ExactRational Maximum);
25: public static class DescriptiveStatistics64
27: public const int MaximumObservations = 4096;
28: public const int MaximumExactOperandBits = 4096;
30: /// <summary>Compensated sum with centered two-pass population/sample variance.</summary>
31: public static FiniteDescriptiveStatistics64 Analyze(IEnumerable<double> observations)
42: Accumulate(value, ref sum, ref correction);
49: // Subtract a real input, avoiding cancellation in the variance oracle.
53: Accumulate(RequireFinite(value - reference),
61: Accumulate(RequireFinite(offset * offset),
72: /// <summary>Bounded exact sum, mean and extrema over normalized rationals.</summary>
73: public static ExactDescriptiveStatistics AnalyzeExact(
74: IEnumerable<ExactRational> observations)
80: var sum = ExactRational.Zero;
91: var mean = sum / new ExactRational(values.Length, BigInteger.One);
105: private static void RequireExactBudget(ExactRational value)
121: internal static void Accumulate(double value, ref double sum, ref double correction)
```

### src/projects/dw.tools.math.statistics/FrequencyWeightedStatistics64.cs

```text
10: public enum MissingObservationPolicy64
20: public readonly record struct FrequencyWeightedObservation64(double? Value, int Frequency);
26: public sealed record FrequencyWeightedDescriptiveResult64(
33: double PopulationVariance,
34: double? SampleVariance,
42: public static class FrequencyWeightedStatistics64
44: public const int MaximumRows = DescriptiveStatistics64.MaximumObservations;
45: public const int MaximumEffectiveCount = DescriptiveStatistics64.MaximumObservations;
47: public static FrequencyWeightedDescriptiveResult64 Analyze(
92: DescriptiveStatistics64.Accumulate(weightedValue, ref sum, ref correction);
110: DescriptiveStatistics64.Accumulate(weightedDelta,
125: DescriptiveStatistics64.Accumulate(weightedSquare,
```

### src/projects/dw.tools.math.statistics/QuantileStatistics64.cs

```text
9: /// <summary>Two deliberately distinct sample quantile definitions.</summary>
10: public enum QuantileConvention64
17: /// Bounded sample quantiles. Exact rationals and finite binary64 have separate
20: public static class QuantileStatistics64
22: public const int MaximumObservations = DescriptiveStatistics64.MaximumObservations;
23: public const int MaximumExactOperandBits = DescriptiveStatistics64.MaximumExactOperandBits;
29: public static double Evaluate(
30: IEnumerable<double> observations, double probability, QuantileConvention64 convention)
42: if (convention == QuantileConvention64.NearestRank)
64: /// Exact rational Type-7 interpolation and nearest-rank with a rational
67: public static ExactRational EvaluateExact(
68: IEnumerable<ExactRational> observations, ExactRational probability,
69: QuantileConvention64 convention)
73: if (probability < ExactRational.Zero || probability > ExactRational.One)
82: if (convention == QuantileConvention64.NearestRank)
84: if (probability == ExactRational.Zero)
86: var scaled = probability.Numerator * sorted.Length;
87: var rank = BigInteger.DivRem(scaled, probability.Denominator, out var rem);
92: var scaledPosition = probability.Numerator * (sorted.Length - 1);
93: var integer = BigInteger.DivRem(scaledPosition, probability.Denominator,
98: var fraction = new ExactRational(remainder, probability.Denominator);
101: var scaledDelta = fraction * delta;
102: RequireExactBudget(scaledDelta);
103: var result = sorted[lower] + scaledDelta;
108: private static void RequireConvention(QuantileConvention64 convention)
120: "Quantiles admit at most 4096 observations.");
123: private static void RequireExactBudget(ExactRational value)
128: "An exact quantile operand or intermediate exceeds 4096 bits.");
```

### src/projects/dw.tools.math.statistics/SplitMix64PositionCodec.cs

```text
7: /// Canonical versioned checkpoint: splitmix64/v1 followed by exactly two
10: public static class SplitMix64PositionCodec
12: public static string Format(SplitMix64Position position)
14: if (!string.Equals(position.Algorithm, SplitMix64Random.AlgorithmId,
16: throw new ArgumentException("Unsupported versioned checkpoint.", nameof(position));
17: return string.Concat(SplitMix64Random.AlgorithmId, "|",
22: public static SplitMix64Position Parse(string value)
27: !string.Equals(parts[0], SplitMix64Random.AlgorithmId, StringComparison.Ordinal) ||
33: throw new FormatException("Checkpoint must contain splitmix64/v1 and two lowercase 16-digit hex words.");
34: return new SplitMix64Position(SplitMix64Random.AlgorithmId, seed, counter);
```

### src/projects/dw.tools.math.statistics/SplitMix64Random.cs

```text
6: /// Portable checkpoint for the exact versioned generator and draw index.
7: /// A checkpoint identifies the next draw, not the last emitted value.
9: public readonly record struct SplitMix64Position(string Algorithm, ulong Seed, ulong Counter);
12: /// Deterministic SplitMix64 with explicit seed and unsigned draw counter.
15: public sealed class SplitMix64Random
17: public const string AlgorithmId = "splitmix64/v1";
29: public SplitMix64Random(ulong seed, ulong counter = 0UL)
36: public ulong Seed { get; }
39: public ulong Counter { get; private set; }
41: public SplitMix64Position Snapshot() => new(AlgorithmId, Seed, Counter);
43: public static SplitMix64Random FromSnapshot(SplitMix64Position snapshot)
45: if (!string.Equals(snapshot.Algorithm, AlgorithmId, StringComparison.Ordinal))
46: throw new ArgumentException("Unsupported deterministic generator algorithm.", nameof(snapshot));
47: return new SplitMix64Random(snapshot.Seed, snapshot.Counter);
51: public ulong NextUInt64()
53: if (Counter == ulong.MaxValue)
65: /// from splitmix64/v1 and does not change its raw words or checkpoints.
67: public const string BoundedTransformId = "splitmix64-bounded-rejection/v1";
70: /// Uniformly maps the next accepted raw 64-bit word into [0, exclusiveUpperBound)
71: /// under the assumption that raw 64-bit words are uniformly distributed.
72: /// Rejected words count against maxRawDraws and advance the checkpoint.
76: public ulong NextBoundedUInt64(ulong exclusiveUpperBound, int maxRawDraws)
88: var raw = NextUInt64();
97: /// the same rejection transformation and raw-word budget. The full Int64
100: public long NextInt64Inclusive(long lowerInclusive, long upperInclusive, int maxRawDraws)
108: var lo = unchecked((ulong)(lowerInclusive ^ long.MinValue));
109: var hi = unchecked((ulong)(upperInclusive ^ long.MinValue));
113: var offset = width == 0UL ? NextUInt64() : NextBoundedUInt64(width, maxRawDraws);
121: public double NextUnitInterval() => (NextUInt64() >> 11) * Unit53;
```

### src/projects/dw.tools.math.symbolics/SymbolicDerivativePointAnalysis.cs

```text
8: public enum SymbolicDerivativePointStatus
24: public sealed record SymbolicDerivativePointResult(
26: ExactRational? Value,
31: public bool HasExactValue => Status == SymbolicDerivativePointStatus.ExactValue &&
41: public static class SymbolicDerivativePointAnalysis
43: public const string PolynomialRule = "pointwise-exact-polynomial-derivative/v1";
44: public const string AbsoluteRule = "pointwise-abs-away-from-zero/v1";
45: public const string AbsoluteSquareRootRule = "pointwise-sqrt-square-real-away-from-zero/v1";
46: public const int MaximumEvaluationDepth = 16;
47: public const int MaximumRationalBits = 128;
49: public static SymbolicDerivativePointResult Analyze(
50: SymbolicRealFunction function, ExactRational point)
56: public static SymbolicDerivativePointResult Analyze(
57: IrNode source,IrSymbol withRespectTo,ExactRational point)
87: return point < ExactRational.Zero
89: : point == ExactRational.Zero
99: var cancellation=SymbolicRationalCancellationBridge.TryCancel(source);
101: SymbolicRationalCancellationBridge.EvaluateExactAt(
125: evaluated.Value is not ExactRational value)
141: IrNode source,IrSymbol variable,ExactRational point,string rule) =>
142: point==ExactRational.Zero
145: point>ExactRational.Zero?ExactRational.One:
146: new ExactRational(-1,1),rule);
148: private static (SymbolicDerivativePointStatus Status,ExactRational? Value)
149: EvaluateExact(IrNode node,IrSymbol variable,ExactRational point,int depth)
167: operand.Value is not ExactRational r)
184: operand.Value is not ExactRational factor)
186: var total=ExactRational.One;
197: var sum=ExactRational.Zero;
202: operand.Value is not ExactRational value)
214: left.Value is not ExactRational a)
218: right.Value is not ExactRational b)
227: var product=ExactRational.One;
232: operand.Value is not ExactRational value)
246: IrNode source,IrSymbol variable,ExactRational value,string rule) =>
258: private static bool Bounded(ExactRational value) =>
259: BigInteger.Abs(value.Numerator).GetBitLength()<=MaximumRationalBits &&
260: value.Denominator.GetBitLength()<=MaximumRationalBits;
```

### src/projects/dw.tools.math.symbolics/SymbolicExactDifferentiation.cs

```text
8: public enum SymbolicDifferentiationStatus
22: public sealed class SymbolicRealFunction
24: public SymbolicRealFunction(
42: public IrSymbol Parameter { get; }
43: public IrNode Body { get; }
44: public IrScalarDomain Domain { get; }
45: public IrScalarDomain Codomain { get; }
49: public sealed record SymbolicDerivativeResult(
56: public bool IsAdmitted =>
65: /// IR symbols, canonical rationals and elementary sums/constant products.
68: public static class SymbolicExactDifferentiation
70: public const string Rule = "formal-polynomial-derivative/exact/v1";
71: public const int MaximumPower = 8;
72: public const int MaximumRecursionDepth = 16;
74: private static readonly IrExactScalar Zero = new(ExactRational.Zero);
75: private static readonly IrExactScalar One = new(ExactRational.One);
77: public static SymbolicDerivativeResult TryDifferentiate(SymbolicRealFunction function)
83: public static SymbolicDerivativeResult TryDifferentiate(
207: if(factor.Value==ExactRational.Zero || IsZero(innerValue))
209: if(factor.Value==ExactRational.One)
234: var coefficient=new IrExactScalar(new ExactRational(exponent,1));
238: symbol,new IrExactScalar(new ExactRational(exponent-1,1))]);
250: value is IrExactScalar scalar && scalar.Value==ExactRational.Zero;
```

### src/projects/dw.tools.math.symbolics/SymbolicExactPolynomialCalculus.cs

```text
8: public enum SymbolicPolynomialCalculusStatus
24: public sealed record SymbolicPolynomialIntegralResult(
27: ExactRational? DefiniteValue,
34: public bool IsExact => Status == SymbolicPolynomialCalculusStatus.Exact
42: public sealed record SymbolicPolynomialLimitResult(
44: ExactRational? Value,
49: public bool IsExact => Status == SymbolicPolynomialCalculusStatus.Exact
55: /// Closed rational polynomial integration and finite-point limits over one
60: public static class SymbolicExactPolynomialCalculus
62: public const string IntegralRule = "real-polynomial-antiderivative-definite-integral/exact/v1";
63: public const string LimitRule = "real-polynomial-finite-limit/exact/v1";
64: public const string AdditiveConstantConvention = "arbitrary-additive-constant";
65: public const int MaximumDegree = 7;
66: public const int MaximumRationalBits = 128;
67: public const int MaximumParseDepth = 12;
70: public static SymbolicPolynomialIntegralResult Integrate(
72: ExactRational lowerBound, ExactRational upperBound)
89: polynomial.Coefficients is not ExactRational[] input)
92: var primitiveCoefficients = new ExactRational[MaximumDegree + 2];
96: input[degree] / new ExactRational(degree + 1, 1);
113: check.Coefficients is not ExactRational[] recovered ||
146: public static SymbolicPolynomialLimitResult FiniteLimit(
147: IrNode expression, IrSymbol variable, ExactRational point)
163: polynomial.Coefficients is not ExactRational[] values)
211: ExactRational[]? Coefficients) Parse(
216: var c = new ExactRational[CoefficientCount];
225: c[1] = ExactRational.One;
230: c[2] = ExactRational.One;
240: c[(int)exponent.Value.Numerator] = ExactRational.One;
246: if (term.Coefficients is not ExactRational[] parsed)
259: if (left.Coefficients is not ExactRational[] a)
262: if (right.Coefficients is not ExactRational[] b)
277: if (left.Coefficients is not ExactRational[] a)
280: if (right.Coefficients is not ExactRational[] b)
302: ExactRational[] coefficients, IrSymbol variable)
308: if (coefficient == ExactRational.Zero)
315: [variable, new IrExactScalar(new ExactRational(degree, 1))])
317: terms.Add(coefficient == ExactRational.One
324: 0 => new IrExactScalar(ExactRational.Zero),
330: private static (SymbolicPolynomialCalculusStatus Status, ExactRational Value)
331: Evaluate(ExactRational[] coefficients, ExactRational point)
333: var sum = ExactRational.Zero;
339: ExactRational.Zero);
344: private static bool Constant(ExactRational[] coefficients)
347: if (coefficients[i] != ExactRational.Zero)
359: private static bool Bounded(ExactRational x) =>
360: BigInteger.Abs(x.Numerator).GetBitLength() <= MaximumRationalBits &&
361: x.Denominator.GetBitLength() <= MaximumRationalBits;
364: ExactRational[]? Coefficients) ValidPolynomial(ExactRational[] coefficients) =>
368: ExactRational[]? Coefficients) RefusedPolynomial(SymbolicPolynomialCalculusStatus status) =>
```

### src/projects/dw.tools.math.symbolics/SymbolicExactRealSolving.cs

```text
9: public enum SymbolicRealSolvingStatus
28: public sealed record SymbolicRealSolvingResult(
30: ImmutableArray<ExactRational> Roots,
31: ExactRational? ClosedBound,
37: public bool IsQualified => Status is
45: public bool IsComplete => IsQualified;
52: /// An irrational real root is UNKNOWN, never an empty real solution set.
54: public static class SymbolicExactRealSolving
56: public const string Rule = "real-elementary-solution-sets/exact/v1";
57: public const int MaximumRationalBits = 128;
59: public static SymbolicRealSolvingResult Solve(
141: return bound.Value <= ExactRational.Zero
146: return limit.Value < ExactRational.Zero
149: : limit.Value == ExactRational.Zero
152: roots: [ExactRational.Zero])
169: IrNode left, IrNode right, IrSymbol variable, ExactRational c)
171: if (c < ExactRational.Zero)
173: if (c == ExactRational.Zero)
175: roots: [ExactRational.Zero]);
179: var positive = new ExactRational(n,d);
181: roots: [new ExactRational(-n,d),positive]);
214: && exponent.Value == new ExactRational(2,1);
220: private static bool Bounded(ExactRational value) =>
221: BigInteger.Abs(value.Numerator).GetBitLength()<=MaximumRationalBits
222: && value.Denominator.GetBitLength()<=MaximumRationalBits;
225: ExactRational left,IrRelationKind relation,ExactRational right) =>
230: IrSymbol variable,ExactRational? bound=null,
231: ExactRational[]? roots=null) =>
237: new(status,ImmutableArray<ExactRational>.Empty,null,null,null,null,null);
```

### src/projects/dw.tools.math.symbolics/SymbolicExactTransformations.cs

```text
8: public enum SymbolicTransformStatus
17: public sealed record SymbolicTransformResult(
20: public bool IsAdmitted => Status == SymbolicTransformStatus.Admitted &&
28: public static class SymbolicExactTransformations
30: public const int MaximumCoefficientBits = 128;
31: public const string ExpandRule = "binomial-square/exact/v1";
32: public const string FactorRule = "binomial-factor/exact/v1";
33: public const string SubstitutionRule = "free-symbol-substitute/closed/v1";
34: public const string SqrtRule = "sqrt-real-square/domain/v1";
37: public static SymbolicTransformResult ExpandSquare(IrNode source)
62: new IrExactScalar(new ExactRational(2, 1) * a), x]),
68: public static SymbolicTransformResult FactorSquare(IrNode source)
85: var a = twice.Value / new ExactRational(2, 1);
103: public static SymbolicTransformResult SubstituteFree(
127: public static SymbolicTransformResult NormalizeRealSqrtSquare(
242: private static bool Small(ExactRational number) =>
```

### src/projects/dw.tools.math.symbolics/SymbolicProviderResultAdmission.cs

```text
6: public enum SymbolicProviderAdmissionStatus
20: public sealed record SymbolicProviderAdmissionResult(
24: public bool IsAdmitted =>
30: /// proven, bounded rational-cancellation identity AND its original exclusions.
33: public static class SymbolicProviderResultAdmission
35: public static SymbolicProviderAdmissionResult TryAdmit(
49: proof = SymbolicRationalCancellationBridge.TryCancel(source);
```

### src/projects/dw.tools.math.symbolics/SymbolicRationalCancellationBridge.cs

```text
7: /// <summary>Outcome of one closed, exact IR rational-cancellation contract.</summary>
8: public enum SymbolicCancellationStatus
17: public sealed class SymbolicCancellationResult
26: public SymbolicCancellationStatus Status { get; }
27: public IrRestrictedExpression? Conditional { get; }
28: public bool IsAdmitted => Status == SymbolicCancellationStatus.Admitted &&
32: public enum SymbolicPointStatus
40: public sealed record SymbolicPointResult(SymbolicPointStatus Status, ExactRational? Value)
42: public bool HasExactValue => Status == SymbolicPointStatus.ExactValue &&
51: public static class SymbolicRationalCancellationBridge
53: public const int MaximumConstantOperandBits = 128;
56: /// Admit only a single free real symbol and an exact rational c,
60: public static SymbolicCancellationResult TryCancel(IrNode original)
111: // Force the previously qualified public IR codec's restriction checks
120: public static SymbolicPointResult EvaluateExactAt(
121: SymbolicCancellationResult result, IrSymbol variable, ExactRational point)
164: exponent.Value == new ExactRational(2, 1))
```

### src/projects/dw.tools.math.symbolics/SymbolicRealCalculusBoundaries.cs

```text
8: public enum SymbolicCalculusBoundaryStatus
26: public sealed record SymbolicCalculusBoundaryResult(
28: ExactRational? ExactValue,
29: ExactRational? ExceptionalPoint,
35: public bool HasExactValue =>
41: public bool HasQualifiedBoundary =>
55: public static class SymbolicRealCalculusBoundaries
57: public const string SimplePoleRule = "real-simple-rational-pole/original-domain/v1";
58: public const string HoleRule = "real-original-removable-hole/v1";
59: public const string QuotientLimitRule = "real-simple-rational-finite-limit/exact/v1";
60: public const string RemovableLimitRule = "real-qualified-removable-limit/exact/v1";
61: public const string HoleFreeIntegralRule = "real-hole-free-rational-polynomial-integral/exact/v1";
62: public const string ZeroQuotientRule = "real-hole-free-zero-quotient-integral/exact/v1";
63: public const int MaximumRationalBits = 128;
65: public static SymbolicCalculusBoundaryResult AnalyzeIntegral(
67: ExactRational lowerBound, ExactRational upperBound)
86: if (prior.IsExact && prior.DefiniteValue is ExactRational value)
102: numerator.Value == ExactRational.Zero
105: numerator.Value == ExactRational.Zero ? HoleRule : SimplePoleRule);
109: if (numerator.Value == ExactRational.Zero)
110: return Exact(integrand,variable,ExactRational.Zero,
118: var cancellation=SymbolicRationalCancellationBridge.TryCancel(integrand);
127: return delegated.IsExact && delegated.DefiniteValue is ExactRational integral
143: public static SymbolicCalculusBoundaryResult AnalyzeLimit(
144: IrNode expression, IrSymbol variable, ExactRational point)
160: if (prior.IsExact && prior.Value is ExactRational polynomialValue)
173: if(k.Value!=ExactRational.Zero)
179: return Exact(expression,variable,ExactRational.Zero,
191: var cancellation=SymbolicRationalCancellationBridge.TryCancel(expression);
196: if(!limit.IsExact || limit.Value is not ExactRational value)
241: IrNode expression, IrSymbol variable, out ExactRational c)
243: c=ExactRational.Zero;
256: ExactRational point,ExactRational a,ExactRational b) =>
263: private static bool Bounded(ExactRational x) =>
264: BigInteger.Abs(x.Numerator).GetBitLength()<=MaximumRationalBits &&
265: x.Denominator.GetBitLength()<=MaximumRationalBits;
284: IrNode original,IrSymbol variable,ExactRational value,
285: string rule,string? constantConvention=null,ExactRational? exceptionalPoint=null) =>
293: IrNode original,IrSymbol variable,ExactRational point,
```

### src/projects/dw.tools.math.symbolics/SymbolicRootAudit.cs

```text
9: public enum SymbolicRootAuditStatus
26: public sealed record SymbolicAuditedRoot(ExactRational Value, int? Multiplicity);
29: /// Verified roots satisfy the ORIGINAL relation at their exact rational points.
34: public sealed record SymbolicRootAuditResult(
37: ImmutableArray<ExactRational> ExcludedFromDomain,
38: ImmutableArray<ExactRational> RejectedCandidates,
39: ImmutableArray<ExactRational> UnresolvedCandidates,
46: public bool IsComplete => Status == SymbolicRootAuditStatus.Complete;
47: public bool HasVerifiedRoots => VerifiedRoots.Length > 0 &&
53: /// Does not treat candidates, a simplified quotient or missing rational roots
56: public static class SymbolicRootAudit
58: public const string Rule = "real-original-equation-candidate-audit/exact/v1";
59: public const int MaximumCandidates = 8;
60: public const int MaximumEvaluationDepth = 16;
61: public const int MaximumRationalBits = 128;
62: public const int MaximumPower = 8;
64: public static SymbolicRootAuditResult Verify(
66: IrSymbol variable, IEnumerable<ExactRational> proposedCandidates)
110: var excluded=ImmutableArray.CreateBuilder<ExactRational>();
111: var rejected=ImmutableArray.CreateBuilder<ExactRational>();
112: var unresolved=ImmutableArray.CreateBuilder<ExactRational>();
190: EvaluationStatus Status,ExactRational? Value);
193: IrNode expression,IrSymbol target,ExactRational point,int depth)
232: var values=new ExactRational[app.Arguments.Length];
237: child.Value is not ExactRational value)
241: ExactRational result;
245: result=ExactRational.Zero;
257: result=ExactRational.One;
266: if(values[1]==ExactRational.Zero)
282: result=ExactRational.One;
304: IrNode left,IrNode right,IrSymbol variable,ExactRational root,
318: return root==ExactRational.Zero?2:1;
328: && exponent.Value == new ExactRational(2,1);
334: private static bool Bounded(ExactRational x) =>
335: BigInteger.Abs(x.Numerator).GetBitLength()<=MaximumRationalBits &&
336: x.Denominator.GetBitLength()<=MaximumRationalBits;
340: ImmutableArray<ExactRational>.Empty,
341: ImmutableArray<ExactRational>.Empty,
342: ImmutableArray<ExactRational>.Empty,false,null,null,null,null);
```
