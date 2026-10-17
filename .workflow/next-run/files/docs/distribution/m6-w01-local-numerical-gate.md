# M6-W01 numerical-analysis local work-package gate

Observed package: dw.tools.math.analysis 1.1.0-preview.6 (net10, BCL-only).

Accepted finite binary64 families: line/spline interpolation and
unweighted descriptive OLS; bounded bracketed roots and composite Simpson
integration; four-callback central differences and fixed-step scalar RK4.
Sidecars provide explicit caller unit metadata, residual-based root
admission, and regularity/nonstiff/event-free assumptions without
mathematically certifying these assumptions.

Run qualification: all 328 targeted C01/C02/C03 tests, complete analysis
project, four relevant Math regressions, one SHA-inspected real local
nupkg and 3 separate eight-probe NuGet-only consumers against identical
archive bytes from one isolated feed. No direct sibling repository reads.

Limits: maximum finite data and iteration/evaluation budgets remain
operation-specific. Numeric error estimates are heuristics, not proofs.
No stiffness/event/PDE support, global root enclosure, confidence
interval, automatic unit conversion, broad cross-platform certification
or stable/public NuGet release is represented.

This gate is a work-package certification event. M6-W01 closes only
on durable success. M6 and all M6-W02 phases remain open for future runs.
