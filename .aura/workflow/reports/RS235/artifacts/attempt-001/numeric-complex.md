# Search: `Complex`

Paths: `src/projects`

```text
src/projects/dw.quantities.expression/ExpressionDiagnostics.cs:33:         ExpressionFailureKind.ComplexityLimit => "math.expression.complexity-limit",
src/projects/dw.quantities.expression/ExpressionParser.cs:55:     ComplexityLimit,
src/projects/dw.quantities.expression/ExpressionParser.cs:89:                     ? ExpressionFailureKind.ComplexityLimit
src/projects/dw.quantities.expression/ExpressionParser.cs:171:                 throw new ExpressionException(ExpressionFailureKind.ComplexityLimit, start);
src/projects/dw.quantities.expression/ExpressionParser.cs:539:                 throw new ExpressionException(ExpressionFailureKind.ComplexityLimit, Current.Position);
src/projects/dw.tools.math.scientific/FiniteSignal64.cs:14:     FiniteSignalStatus64 Status, Complex[]? Bins, bool Inverse,
src/projects/dw.tools.math.scientific/FiniteSignal64.cs:55: /// Complex bins use the negative-exponent forward convention and positive
src/projects/dw.tools.math.scientific/FiniteSignal64.cs:67:         Complex[]? samples, bool inverse = false,
src/projects/dw.tools.math.scientific/FiniteSignal64.cs:72:         var status = CheckComplexSamples(samples);
src/projects/dw.tools.math.scientific/FiniteSignal64.cs:76:         var bins = new Complex[length];
src/projects/dw.tools.math.scientific/FiniteSignal64.cs:105:             bins[k] = new Complex(real, imaginary);
src/projects/dw.tools.math.scientific/FiniteSignal64.cs:148:     private static FiniteSignalStatus64 CheckComplexSamples(Complex[]? samples)
src/projects/dw.tools.math.scientific/FiniteSignalBoundaryAudit64.cs:71:         Complex[]? samples,
src/projects/dw.tools.math.scientific/FiniteSignalBoundaryAudit64.cs:94:             sumAbsoluteInputs += Complex.Abs(sample);
src/projects/dw.tools.math.scientific/FiniteSignalBoundaryAudit64.cs:105:                 var ratio = Complex.Abs(bin) / sumAbsoluteInputs;
src/projects/dw.tools.math.scientific/M6W02C03Contract.cs:16:     Complex[]? DiscreteSamples,
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:15:     private const double MaximumComplexComponent=12d;
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:65:     public SpecialComplexResult64 EvaluateComplex(
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:66:         SpecialFunctionKind64 kind, Complex argument,
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:71:             return RefuseComplex(SpecialFunctionStatus64.Cancelled);
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:74:             return RefuseComplex(SpecialFunctionStatus64.InvalidPolicy);
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:76:             return RefuseComplex(SpecialFunctionStatus64.InvalidInput);
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:77:         if(global::System.Math.Abs(argument.Real)>selected.MaximumComplexComponentMagnitude||
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:78:            global::System.Math.Abs(argument.Imaginary)>selected.MaximumComplexComponentMagnitude)
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:79:             return RefuseComplex(SpecialFunctionStatus64.OutsideQualifiedRange);
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:80:         // Complex Gamma and its poles/branches are explicitly outside T1.
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:82:             return RefuseComplex(SpecialFunctionStatus64.UnsupportedDomain);
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:83:         if(kind==SpecialFunctionKind64.NaturalLogarithm&&argument==Complex.Zero)
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:84:             return RefuseComplex(SpecialFunctionStatus64.UnsupportedDomain);
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:87:             SpecialFunctionKind64.Sine=>Complex.Sin(argument),
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:88:             SpecialFunctionKind64.Cosine=>Complex.Cos(argument),
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:89:             SpecialFunctionKind64.Exponential=>Complex.Exp(argument),
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:90:             SpecialFunctionKind64.NaturalLogarithm=>Complex.Log(argument),
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:91:             _=>new Complex(double.NaN,double.NaN)
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:94:             return RefuseComplex(SpecialFunctionStatus64.NonFiniteResult);
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:102:         double.IsFinite(p.MaximumComplexComponentMagnitude) &&
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:103:         p.MaximumComplexComponentMagnitude>0d &&
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:104:         p.MaximumComplexComponentMagnitude<=MaximumComplexComponent;
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:129:     private SpecialComplexResult64 RefuseComplex(SpecialFunctionStatus64 status) =>
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:146:     public static SpecialComplexResult64 EvaluateComplex(
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:147:         SpecialFunctionKind64 kind,Complex argument,
src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs:149:         ManagedProvider.EvaluateComplex(kind,argument,policy,cancellationToken);
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:10: public enum ComplexLogCutPolicy64 { RejectNegativeRealCut, CallerAcceptsPrincipalCut }
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:16:     ComplexGammaUnsupported, BranchCutRequiresOptIn, AngleUnitRequired,
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:31: public sealed record SpecialComplexBoundaryResult64(
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:32:     SpecialBoundaryStatus64 Status, Complex? Value,
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:33:     ComplexLogCutPolicy64 CutPolicy, string ProviderId, string RuleId,
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:99:     public static SpecialComplexBoundaryResult64 EvaluateComplex(
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:100:         SpecialFunctionKind64 kind,Complex argument,
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:101:         ComplexLogCutPolicy64 cutPolicy = ComplexLogCutPolicy64.RejectNegativeRealCut,
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:106:             return RejectComplex(SpecialBoundaryStatus64.Cancelled,cutPolicy);
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:108:             return RejectComplex(SpecialBoundaryStatus64.InvalidInput,cutPolicy);
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:109:         if(cutPolicy is not (ComplexLogCutPolicy64.RejectNegativeRealCut or
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:110:             ComplexLogCutPolicy64.CallerAcceptsPrincipalCut))
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:111:             return RejectComplex(SpecialBoundaryStatus64.InvalidPolicy,cutPolicy);
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:113:            cutPolicy==ComplexLogCutPolicy64.CallerAcceptsPrincipalCut)
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:114:             return RejectComplex(SpecialBoundaryStatus64.InvalidPolicy,cutPolicy);
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:117:             return RejectComplex(SpecialBoundaryStatus64.ComplexGammaUnsupported,cutPolicy);
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:120:            cutPolicy==ComplexLogCutPolicy64.RejectNegativeRealCut)
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:121:             return RejectComplex(SpecialBoundaryStatus64.BranchCutRequiresOptIn,cutPolicy);
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:125:                 return RejectComplex(SpecialBoundaryStatus64.OverflowRisk,cutPolicy);
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:127:                 return RejectComplex(SpecialBoundaryStatus64.UnderflowRisk,cutPolicy);
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:129:         var result=BoundedSpecialFunctions64.EvaluateComplex(kind,argument,
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:132:             return RejectComplex(Map(result.Status),cutPolicy);
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:154:     private static SpecialComplexBoundaryResult64 RejectComplex(
src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs:155:         SpecialBoundaryStatus64 status,ComplexLogCutPolicy64 policy) =>
src/projects/dw.tools.math.scientific/SpecialFunctionContracts64.cs:24:     double MaximumComplexComponentMagnitude = 12d);
src/projects/dw.tools.math.scientific/SpecialFunctionContracts64.cs:37: public sealed record SpecialComplexResult64(
src/projects/dw.tools.math.scientific/SpecialFunctionContracts64.cs:38:     SpecialFunctionStatus64 Status, Complex? Value,
src/projects/dw.tools.math.scientific/SpecialFunctionContracts64.cs:53:     SpecialComplexResult64 EvaluateComplex(SpecialFunctionKind64 kind, Complex argument,
```
