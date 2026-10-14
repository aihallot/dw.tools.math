using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Numerics;
using Dw.Tools.Math.Statistics;

// M3 NuGet-only Release-gate smoke: no source/project reference, provider,
// AURA, Decision, MCDM, or broker runtime needed.
var passed = 0;
void Probe(string name, Action action)
{
    try { action(); passed++; }
    catch (Exception error)
    {
        throw new InvalidOperationException("M3 isolated gate oracle failed: "+name, error);
    }
}
static void Require(bool valid, string reason)
{
    if (!valid) throw new InvalidOperationException(reason);
}
static void Near(double value, double expected, string reason, double tolerance=1e-11)
{
    Require(double.IsFinite(value) && global::System.Math.Abs(value-expected)<=tolerance,
        reason+" expected "+expected.ToString("R")+" got "+value.ToString("R"));
}
static void Throws<T>(Action action) where T:Exception
{
    try { action(); }
    catch(Exception exception) when(exception is T) { return; }
    throw new InvalidOperationException("Expected "+typeof(T).Name+" was not observed.");
}

Probe("exact fraction arithmetic", () =>
    Require(new ExactRational(1,3)+new ExactRational(1,6)==new ExactRational(1,2),
        "one third plus one sixth must be one half"));

Probe("binary64 half has exact rational representation", () =>
    Require(ExactBinaryNumber.FromDouble(.5d)==new ExactRational(1,2),
        "binary64 .5 must remain exact rational one-half"));

Probe("binary64 decimal-looking third retains rounding loss", () =>
    Require(ExactBinaryNumber.FromDouble(1d/3d)!=new ExactRational(1,3),
        "binary64 one third must not be silently exact"));

Probe("matrix keeps 2 by 2 shape and correct entries", () =>
{
    var matrix=FiniteMatrix64.Create(2,2,[2d,1d,1d,3d]);
    Require(matrix.Rows==2 && matrix.Columns==2 && matrix.At(0,0)==2d &&
        matrix.At(1,1)==3d,"matrix shape/data");
});

Probe("matrix multiplication uses independent hand product", () =>
{
    var matrix=FiniteMatrix64.Create(2,2,[2d,1d,1d,3d]);
    var squared=matrix.Multiply(matrix);
    Require(squared.At(0,0)==5d && squared.At(0,1)==5d &&
        squared.At(1,0)==5d && squared.At(1,1)==10d,"A squared");
});

Probe("matrix addition is elementwise", () =>
{
    var m=FiniteMatrix64.Create(2,2,[2d,1d,1d,3d]);
    var sum=m.Add(m);
    Require(sum.At(0,0)==4d && sum.At(1,1)==6d,"two matrices");
});

Probe("matrix times vector gives hand-checkable right side", () =>
{
    var result=FiniteMatrix64.Create(2,2,[2d,1d,1d,3d])
        .Multiply(FiniteVector64.Create([1d,2d]));
    Require(result.At(0)==4d && result.At(1)==7d,"matrix vector");
});

Probe("bounded solve gives independent 2 by 2 solution", () =>
{
    var matrix=FiniteMatrix64.Create(2,2,[2d,1d,1d,3d]);
    var solved=BoundedLinearAlgebra64.Solve(matrix,FiniteVector64.Create([4d,7d]));
    Require(solved.HasUniqueSolution && solved.Solution is not null,"unique solve");
    Near(solved.Solution!.At(0),1d,"first solution");
    Near(solved.Solution.At(1),2d,"second solution");
    Near(solved.AbsoluteResidualInfinity!.Value,0d,"residual");
});

Probe("rank-one system cannot invent a unique solution", () =>
{
    var rankOne=FiniteMatrix64.Create(2,2,[1d,2d,2d,4d]);
    var solved=BoundedLinearAlgebra64.Solve(rankOne,FiniteVector64.Create([3d,6d]));
    Require(solved.Status==LinearAlgebraStatus64.Singular &&
        solved.Solution is null && !solved.HasUniqueSolution,"singular result");
});

Probe("pivot LU reconstructs permuted original", () =>
{
    var m=FiniteMatrix64.Create(2,2,[0d,1d,2d,3d]);
    var f=BoundedLinearAlgebra64.FactorLu(m);
    Require(f.Factored && f.Lower is not null && f.Upper is not null,"LU admitted");
    var result=f.Lower!.Multiply(f.Upper!);
    for(var row=0;row<2;row++)
        for(var col=0;col<2;col++)
            Near(result.At(row,col),m.At(f.RowPermutation[row],col),"permuted LU");
});

Probe("QR preserves reconstruction, not factor vector signs", () =>
{
    var m=FiniteMatrix64.Create(2,2,[2d,1d,1d,3d]);
    var f=BoundedLinearAlgebra64.FactorQr(m);
    Require(f.Factored && f.Q is not null && f.R is not null,"QR admitted");
    var rebuilt=f.Q!.Multiply(f.R!);
    for(var row=0;row<2;row++)
        for(var col=0;col<2;col++)
            Near(rebuilt.At(row,col),m.At(row,col),"QR reconstruction");
});

Probe("2x2 singular spectrum is available but not a full SVD", () =>
{
    var m=FiniteMatrix64.Create(2,2,[2d,1d,1d,3d]);
    var spectrum=BoundedLinearAlgebra64.SingularValues2x2(m);
    Require(spectrum.Status==LinearAlgebraStatus64.Success &&
        spectrum.LargestSingularValue.HasValue &&
        spectrum.SmallestSingularValue.HasValue &&
        spectrum.LargestSingularValue.Value>=spectrum.SmallestSingularValue.Value,
        "bounded spectrum");
});

Probe("sparse matrix expands only listed nonzero cells", () =>
{
    var s=SparseMatrix64.Create(2,2,[new SparseEntry64(0,0,2d),
        new SparseEntry64(1,1,3d)]);
    var dense=s.ToDense();
    Require(s.NonZeroCount==2 && dense.At(0,0)==2d && dense.At(1,1)==3d &&
        dense.At(0,1)==0d && dense.At(1,0)==0d,"sparse transport");
});

Probe("provider array is a copy, never a product alias", () =>
{
    var matrix=FiniteMatrix64.Create(2,2,[2d,1d,1d,3d]);
    var copy=matrix.ToProviderArrayCopy();
    copy[0,0]=100d;
    Require(matrix.At(0,0)==2d,"copy-only provider transport");
});

Probe("lossy rational-to-binary64 conversion is explicit", () =>
{
    var r=RationalBinary64Conversion.TryConvert(new ExactRational(1,3));
    Require(r.Status==RationalBinary64Status.Rounded &&
        r.Value.HasValue && r.AbsoluteError.HasValue &&
        r.AbsoluteError.Value>ExactRational.Zero,"explicit rounded status");
});

Probe("exact half to binary64 reports no absolute error", () =>
{
    var r=RationalBinary64Conversion.TryConvert(new ExactRational(1,2));
    Require(r.Status==RationalBinary64Status.Exact && r.Value==.5d &&
        r.AbsoluteError==ExactRational.Zero,"exact finite conversion");
});

Probe("approximate matrix transport preserves finite elements", () =>
{
    var m=FiniteMatrix64.Create(2,2,[2d,1d,1d,3d]);
    var restored=NumericalMatrixTransport.FromApproximateIr(
        NumericalMatrixTransport.ToApproximateIr(m));
    Require(restored.At(0,0)==2d && restored.At(1,1)==3d,"finite IR roundtrip");
});

Probe("compensated sum retains one in cancellation corpus", () =>
    Near(DescriptiveStatistics64.Analyze([1e16,1d,-1e16]).Sum,1d,
        "Neumaier cancellation",0d));

Probe("population and sample variance have distinct denominators", () =>
{
    var result=DescriptiveStatistics64.Analyze([1d,2d,3d,4d]);
    Near(result.Mean,2.5d,"mean",0d);
    Near(result.PopulationVariance,1.25d,"population variance",0d);
    Near(result.SampleVariance!.Value,5d/3d,"sample variance");
});

Probe("rational statistics do not silently widen", () =>
{
    var result=DescriptiveStatistics64.AnalyzeExact(
        [new ExactRational(1,3),new ExactRational(1,6)]);
    Require(result.Sum==new ExactRational(1,2) &&
        result.Mean==new ExactRational(1,4),"exact sum and mean");
});

Probe("exact Type-7 quantile is five halves", () =>
    Require(QuantileStatistics64.EvaluateExact(
        [ExactRational.Zero,new ExactRational(10,1)],
        new ExactRational(1,4),QuantileConvention64.Type7)==new ExactRational(5,2),
        "exact Type-7"));

Probe("finite Type-7 quantile is two and one half", () =>
    Near(QuantileStatistics64.Evaluate([0d,10d],.25d,QuantileConvention64.Type7),
        2.5d,"finite Type-7",0d));

Probe("nearest rank is not interchangeable with Type-7", () =>
    Near(QuantileStatistics64.Evaluate([0d,10d],.25d,QuantileConvention64.NearestRank),
        0d,"nearest rank",0d));

Probe("increasing affine Pearson is defined and one", () =>
{
    var r=CorrelationStatistics64.AnalyzeWithStatus([1d,2d,3d],[2d,4d,6d]);
    if (!r.IsDefined || r.PearsonCorrelation is not double pearson)
        throw new InvalidOperationException("Expected a defined finite Pearson coefficient.");
    Near(pearson,1d,"positive affine Pearson");
});

Probe("constant Pearson is typed undefined and null", () =>
{
    var r=CorrelationStatistics64.AnalyzeWithStatus([4d,4d,4d],[1d,2d,3d]);
    Require(r.Status==CorrelationDefinitionStatus64.UndefinedZeroVariance &&
        !r.IsDefined && r.PearsonCorrelation is null,"undefined Pearson");
});

Probe("null pair requires explicit skip policy", () =>
{
    Throws<ArgumentException>(()=>CorrelationStatistics64.AnalyzeWithStatus(
        [(double?)1d,null,3d],[(double?)2d,4d,6d]));
    var r=CorrelationStatistics64.AnalyzeWithStatus(
        [(double?)1d,null,3d],[(double?)2d,4d,6d],MissingPairPolicy64.SkipPair);
    Require(r.ProvidedPairs==3 && r.IncludedPairs==2 && r.MissingPairs==1,
        "aligned pair deletion");
});

Probe("degenerate Bernoulli p=1 quantile is in positive-mass support", () =>
    Require(new BernoulliDistribution64(1d).Quantile(0d)==1,
        "Bernoulli q=0 p=1"));

Probe("seed-zero SplitMix64 first word has exact golden bits", () =>
    Require(new SplitMix64Random(0UL).NextUInt64()==0xe220a8397b1dcdafUL,
        "fixed seed-zero vector"));

Probe("portable checkpoint restores next draw from fixed seed", () =>
{
    var rng=new SplitMix64Random(42UL);
    for(var i=0;i<4;i++)rng.NextUInt64();
    var restored=SplitMix64Random.FromSnapshot(
        SplitMix64PositionCodec.Parse(SplitMix64PositionCodec.Format(rng.Snapshot())));
    Require(restored.NextUInt64()==0x09bc585a244823f2UL &&
        rng.Counter==4UL,"golden fifth word at restored position");
});

Probe("uniform seeded samples are finite and half-open", () =>
{
    var law=new ContinuousUniformDistribution64(-2d,6d);
    var rng=new SplitMix64Random(9UL);
    for(var i=0;i<128;i++)
    {
        var value=law.Sample(rng);
        Require(double.IsFinite(value)&&value>=-2d&&value<6d,"uniform sample bound");
    }
    Require(rng.Counter==128UL,"one draw per sample");
});

Probe("NuGet assemblies retain zero AURA Decision MCDM and MathNet deps", () =>
{
    var assemblies=new[]{typeof(ExactRational).Assembly,
        typeof(FiniteMatrix64).Assembly,typeof(DescriptiveStatistics64).Assembly,
        typeof(IrMatrix).Assembly};
    var dependencies=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(n=>n.Name??string.Empty).ToArray();
    Require(!dependencies.Any(n=>n.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        n.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "no provider/broker/decision dependency");
});

if(passed!=31)throw new InvalidOperationException("Gate requires 31 independent package-only oracles.");
Console.WriteLine("dw.tools.math/m3-nuget-gate/0.4 qualified; probes="+passed);
