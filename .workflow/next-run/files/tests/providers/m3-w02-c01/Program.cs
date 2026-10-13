using Dw.Tools.Math.Statistics;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;

Control.UseManaged();
var provider = MathNet.Numerics.Providers.LinearAlgebra.LinearAlgebraControl.Provider.GetType().Name;
if (!provider.Contains("Managed",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Only the pinned managed Math.NET provider is admitted.");

static void Near(double actual,double expected,string label)
{
    if (!double.IsFinite(actual)||global::System.Math.Abs(actual-expected)>1e-12)
        throw new InvalidOperationException(label+": numerical oracle mismatch.");
}

var input=new[]{1d,2d,3d,4d};
var ours=DescriptiveStatistics64.Analyze(input);
Near(ours.Mean,Statistics.Mean(input),"mean");
Near(ours.PopulationVariance,Statistics.PopulationVariance(input),"population variance");
Near(ours.SampleVariance!.Value,Statistics.Variance(input),"sample variance");
Near(ours.Minimum,1d,"minimum");
Near(ours.Maximum,4d,"maximum");
if (ours.Count!=input.Length)
    throw new InvalidOperationException("Statistics sample count changed.");

var independent=DescriptiveStatistics64.Analyze([1e16,1d,-1e16]);
Near(independent.Sum,1d,"compensated cancellation");
// This smoke is corroboration only, not provider ownership of Math contracts.
if(typeof(Control).Assembly.GetName().Version?.Major!=5)
    throw new InvalidOperationException("Pinned Math.NET major assembly version drifted.");
Console.WriteLine("M3-W02-C01:managed-mathnet-5.0.0-descriptive-oracles:qualified");
