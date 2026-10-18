# M6-W02-C02-T1 — bounded real/complex special functions

## Qualified provider and scientific choice

An independent managed BCL-only implementation is selected for
dw.tools.math.scientific 1.1.0-preview.3, with explicit interface
ISpecialFunctionsProvider64 and managed provider class
ManagedBclSpecialFunctionsProvider64. This is a new *local* provider
qualification, not a claim that the previously selected MathNet 5.0
managed backend has been imported into this package. The prior
EXT-NUMERIC provider gate remains satisfied without causing a
silent fallback or an unobserved transitive NuGet dependency.

Real operations: sine, cosine, exponential and natural logarithm
use System.Math; all trigonometry is in radians, real input magnitude
is caller-bounded at most 20, and log requires strictly positive
argument. A disclosed g=7 Lanczos approximation admits Gamma(x)
only for 0.5 <= x <= 20; this includes Gamma(5)=24 and
Gamma(0.5)=sqrt(pi) to independently checked numerical tolerances.
Gamma is not an exact factorial conversion or certified proof.

Complex operations use System.Numerics.Complex Sin/Cos/Exp/Log
on finite real and imaginary components independently bounded
to at most 12. Log uses the .NET principal complex logarithm;
log(0) refuses. Complex Gamma is explicitly unsupported in T1.
The complex negative-real branch conventions and poles are
documented for separate T2 falsification; no degree/radian
conversion, custom-branch selection or implicit provider fallback.

Inputs, policies and outputs are finite; statuses distinguish
invalid input, invalid policy, unsupported domain, outside
qualified magnitude and nonfinite results. Cancellation occurs
before evaluation; no error-bound theorem, arbitrary precision,
analytic continuation or cross-platform bitwise reproducibility
is claimed. Provider id, rule id and angle unit are reported
on every real or complex result, including refusal.

## Evidence and release limits

Observe one genuinely compiled missing-public-provider RED before
staging the new implementation; recover it with 79 independently
asserted GREEN tests including sin(0)=0, log(1)=0, Gamma(5)=24,
Gamma(0.5)=sqrt(pi), complex exp(i*pi)=-1 and log(-1)=i*pi.
Preserve all 70 C01-T1 and 88 C01-T2 scientific tests, run
the full scientific test project and four relevant Math regressions
with locked Release build. Package real net10 preview.3 locally,
inspect nuspec/dll/SHA, then run a new eight-probe source-free
scientific-function consumer and a retained eight-probe exact
integer consumer against the same local archive with matching SHA.

Only C02-T1 (R/G/V) is completed. C02-T2 will qualify complex
branches, Gamma poles, domain and overflow/underflow, and angle
conventions. M6-W02/M6 remain open. No public/stable NuGet,
nonobserved OS, arbitrary Gamma continuation, external CAS,
AURA/Decision/MCDM integration or provider auto-selection is certified.
