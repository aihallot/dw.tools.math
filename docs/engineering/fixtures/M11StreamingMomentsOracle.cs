using System.Reflection;
using Dw.Tools.Math.Statistics;

if (args.Length != 1 || args[0] is not ("--red" or "--green"))
    throw new ArgumentException("Expected --red or --green.");
void Need(bool condition, string label) {
    if (!condition) throw new InvalidDataException("M11-MOMENTS-ORACLE: " + label);
}
void Near(double actual, double expected, double tolerance, string label) =>
    Need(double.IsFinite(actual) && global::System.Math.Abs(actual - expected) <= tolerance, label);
void Refuses<T>(Action action, string label) where T : Exception {
    try { action(); }
    catch (TargetInvocationException ex) when (ex.InnerException?.GetType() == typeof(T)) { return; }
    catch (Exception ex) when (ex.GetType() == typeof(T)) { return; }
    throw new InvalidDataException("M11-MOMENTS-ORACLE: " + label);
}
const int Million = 1_000_000;
const double Origin = 1_000_000_000_000d;
double[] small = [1d, 2d, 3d, 4d];
var batch = DescriptiveStatistics64.Analyze(small);
Near(batch.Mean, 2.5, 0, "historical batch mean");
Near(batch.PopulationVariance, 1.25, 0, "historical population variance");
Near(batch.SampleVariance!.Value, 5d / 3d, 1e-15, "historical sample variance");
Need(DescriptiveStatistics64.Analyze(Enumerable.Repeat(2d,4096)).Count == 4096, "historical cap");
Refuses<ArgumentOutOfRangeException>(() => DescriptiveStatistics64.Analyze(Enumerable.Repeat(2d,4097)), "historical cap refusal");
var high = DescriptiveStatistics64.Analyze(Enumerable.Range(0,4096).Select(i => Origin + (i % 2 == 0 ? -1d : 1d)));
Near(high.Mean, Origin, 0, "independent offset mean");
Near(high.PopulationVariance, 1d, 0, "independent offset variance");
// Independently accumulate integer deviations, rather than running the proposed recurrence.
long deviationSum = 0, squareSum = 0;
for (var i = 0; i < Million; i++) {
    long deviation = i % 2 == 0 ? -1 : 1;
    deviationSum += deviation;
    squareSum += deviation * deviation;
}
Need(deviationSum == 0 && squareSum == Million, "million closed-form reference");
// Exact closed-form reference for an even alternating pair sequence:
// mean=Origin, sum of squared deviations=Million, sample variance=Million/(Million-1).
var type = typeof(DescriptiveStatistics64).Assembly.GetType("Dw.Tools.Math.Statistics.StreamingMoments64");
if (args[0] == "--red") {
    Need(type is null, "existing public accumulator must be reviewed instead");
    Console.WriteLine("M11-MOMENTS-RED|missingType=true|batchCompatibility=PASS|closedFormReference=PASS|PASS");
    return;
}
var api = type ?? throw new InvalidDataException("Missing streaming moments type");
var add = api.GetMethod("Add", [typeof(double)]) ?? throw new InvalidDataException("Missing Add");
var snapshot = api.GetMethod("Snapshot", Type.EmptyTypes) ?? throw new InvalidDataException("Missing Snapshot");
var result = api.GetMethod("GetResult", Type.EmptyTypes) ?? throw new InvalidDataException("Missing GetResult");
var restore = api.GetMethod("FromSnapshot", BindingFlags.Public | BindingFlags.Static, null, [snapshot.ReturnType], null)
    ?? throw new InvalidDataException("Missing FromSnapshot");
object New(int budget) => Activator.CreateInstance(api, budget) ?? throw new InvalidDataException("Missing constructor");
object Snap(object a) => snapshot.Invoke(a,null)!;
object Result(object a) => result.Invoke(a,null)!;
object? Property(object a,string name) => (a.GetType().GetProperty(name) ?? throw new InvalidDataException(name)).GetValue(a);
void Add(object a,double x) => add.Invoke(a,[x]);
void Expect(object a,int count,double? mean,double? pop,double? sample,double tolerance) {
    var output = Result(a);
    Need((int)Property(output,"Count")! == count,"count");
    foreach (var (name, expected) in new (string,double?)[] { ("Mean",mean),("PopulationVariance",pop),("SampleVariance",sample) }) {
        var actual = (double?)Property(output,name);
        Need(actual.HasValue == expected.HasValue,name+" presence");
        if (expected.HasValue) Near(actual!.Value,expected.Value,tolerance,name);
    }
}
Refuses<ArgumentOutOfRangeException>(() => New(0),"invalid budget");
var smallStream = New(4);
Expect(smallStream,0,null,null,null,0);
Add(smallStream,1);
Expect(smallStream,1,1,0,null,0);
foreach (var x in small.Skip(1)) Add(smallStream,x);
Expect(smallStream,4,2.5,1.25,5d/3d,1e-14);
var full = Snap(smallStream);
Refuses<InvalidOperationException>(() => Add(smallStream,5),"exhausted budget");
Need(full.Equals(Snap(smallStream)),"budget refusal atomicity");
var bad = New(4);
Add(bad,double.MaxValue);
var before = Snap(bad);
Refuses<ArgumentOutOfRangeException>(() => Add(bad,double.NaN),"nonfinite");
Refuses<ArithmeticException>(() => Add(bad,-double.MaxValue),"intermediate overflow");
Need(before.Equals(Snap(bad)),"numeric refusal atomicity");
var stream = New(Million);
object? resumed = null;
for (int i=0;i<Million;i++) {
    double value = Origin + (i % 2 == 0 ? -1d : 1d);
    if(i==123457) resumed=restore.Invoke(null,[Snap(stream)]);
    Add(stream,value);
    if(resumed is not null) Add(resumed,value);
}
Expect(stream,Million,Origin,1d,(double)Million/(Million-1),1e-9);
Need(resumed is not null && Snap(stream).Equals(Snap(resumed)) && Result(stream).Equals(Result(resumed)),"checkpoint replay");
Console.WriteLine("M11-MOMENTS-GREEN|million=PASS|replay=PASS|atomicRefusals=PASS|batchCompatibility=PASS|PASS");
