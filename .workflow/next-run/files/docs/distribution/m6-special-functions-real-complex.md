# Managed bounded special functions — local scientific preview.3

Package: dw.tools.math.scientific 1.1.0-preview.3, net10,
BCL-only. Provider: ManagedBclSpecialFunctionsProvider64,
via BoundedSpecialFunctions64.EvaluateReal/EvaluateComplex.

Real: Sine/Cosine/Exponential/NaturalLogarithm (System.Math),
positive-real Gamma approximated with g=7 Lanczos on [0.5,20].
Real arguments must have magnitude <=20, with caller-selected
smaller range allowed. Log requires x>0; values outside
admitted Gamma or log domains refuse with typed status.

Complex: Sin/Cos/Exp/Log (System.Numerics.Complex),
finite real and imaginary parts each <=12 in magnitude.
Logarithm uses the principal branch. Complex Gamma refuses
in this initial T1 subset. All trigonometric angles are radians;
no degrees conversion or alternate branch is offered yet.

Results expose numeric value (or null refusal), status,
provider identity, operation rule, radian convention and
ErrorBoundGuaranteed=false. Numeric accuracy is observed against
finite independent reference values, not mathematically certified.
Unexpected NaN/Infinity or policy/operation values produce
typed nonpartial refusal; cancellation is checked before work.

Scientific gates: one compiled missing-public-API RED recovered;
79 new GREEN and 158 retained T1/T2 exact-integer tests,
full scientific MSTest suite, four relevant Math regressions
and two independently restored eight-probe source-free
consumers verifying the same actual SHA-inspected local nupkg.

No stable/public NuGet, untested operating system, general
complex Gamma, poles/reflection, branch selection, error theorem
or agent/Decision/MCDM integration is admitted. C02 T2 remains
for numeric range and branch boundary testing; M6-W02 and M6
remain in progress.
