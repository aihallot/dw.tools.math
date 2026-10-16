# Exact numerical/symbolic console sample (M5-W02-C01-T1)

Local .NET 10 prerelease: dw.tools.math.distribution 1.0.0-preview.1.
Restore the packages only from a pinned, verified local feed; source and
distribution are not stable/public NuGet packages.

Run the standalone sample:

  dotnet restore samples/m5-exact-vertical/m5-exact-vertical.csproj --source <local-feed>
  dotnet run --project samples/m5-exact-vertical/m5-exact-vertical.csproj -c Release

The sample consumes a single NuGet package (no ProjectReference):
dw.tools.math.distribution, bringing qualified math.codecs, math.symbolics,
math.numerics, math.composition, math.ir and dw.quantities transitively.

For canonical where(sub(square(x),1),ne(x,1)) at x=2 the typed result
contains formal body derivative mul(2,x), exact point value 3, a proved
unrestricted candidate pair -1/1, original-domain audited root -1, and
excluded point 1. It retains original canonical Math IR JSON, structural
SHA-256, assumptions, exact-vs-binary64 result types, explicit loss and
provider metadata. The original root audit remains a verified subset,
not an unjustified complete solution certificate.

The parser does not accept ordinary infix or arbitrary math. At x=1 the
original expression is excluded, even if its polynomial body has a
finite value. Rounding requires explicit consent. Passing a derivative
as the original equation is an incompatible pipeline refusal. No AURA,
Office, CAS or sibling repository is needed.

The consumer prints JSON plus a qualification marker after eight probes.
These are one T1 acceptance vertical, not the multi-platform 1.0 gate.
