# Numerical derivative and scalar ODE (local preview.5)

Package: dw.tools.math.analysis 1.1.0-preview.5, net10, BCL only.

`BoundedDifferentialAnalysis64.Differentiate` returns a finite
four-callback central-difference derivative of f(x) at a caller-defined
point and step policy. The half-step result is admitted only if the
coarse/fine /3 estimate meets the requested heuristic tolerance.
Step representation, callback budgets, cancellation and numeric overflow
produce typed nonpartial refusal. ErrorBoundGuaranteed is always false.

`BoundedDifferentialAnalysis64.IntegrateOde` solves the scalar
forward-time initial-value problem y'=f(t,y), with fixed-step RK4
at N and 2N partitions. The default is N=32 and 384 evaluations;
the maximum N is 256 and the caller must admit 12N callback
evaluations. EstimatedAbsoluteError is |y(2N)-y(N)|/15 and is not
a certified global error. No step-size adaptivity, stiffness control,
event localization, PDE or arbitrary-precision guarantee is provided.

Independent reference examples: derivative of x² at 2 is 4;
integration of y'=y,y(0)=1 to t=1 approximates e with the
qualified observed tolerance. Both methods are approximate and reject
nonfinite or unrepresentable binary64 states.

Prior curve and univariate root/quadrature APIs are unchanged. Package
availability and SHA-256 apply only to the isolated local nupkg and
observed runtime; no stable/public package, downstream adoption or
untested operating system is certified. C03 T2 must qualify additional
step, overflow, and local-versus-global-error limitations.
