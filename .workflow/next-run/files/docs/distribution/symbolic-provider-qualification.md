# M4 symbolic provider experiment — reproducible mathematical corpus

This is a provider-isolated spike, not a public Math symbolics library or automatic CAS adoption.

The common hand-verifiable corpus is:
- p(x)=x^2-1, derivative p'(x)=2x.
- p(-1)=p(1)=0; an engine that merely evaluates candidate points has not been shown to solve polynomial equations.
- f(x)=(x^2-1)/(x-1), domain x != 1. Any cancellation to x+1 requires that exclusion. AngouriMath 2.5.0 is required to retain an explicit provided clause. MathNet.Symbolics 0.25.0 verifies only the value at x=2 and deliberately leaves conditional-domain preservation unqualified.

Both direct C# consumers run in separate .NET 10 projects and separate restore caches. Their package DLLs must never become required dependencies of dw.quantities, dw.tools.math.ir, dw.tools.math.numerics or dw.tools.math.statistics.

Only DWF execution may assert the following expected markers:
  m4-symbolics-angouri/2.5.0:qualified; derivative=2*x; roots=-1,1; domain=conditional
  m4-symbolics-mathnet/0.25.0:qualified; derivative=6@3; root-check=-1,1; exclusion=not-qualified

The archive-only helper examines actually downloaded NuGet packages and returns each version, MIT metadata, direct dependency names, SHA-256 and size. It does not hash the workflow/project authorities or assert portability of a rebuild. The run also executes relevant existing exact rational/IR regressions, a locked .NET 10 solution build, and canonical DWF/planning/transfer validators.

Accepted conclusion after success: a qualified Math-side *provider comparison decision*, not a public symbolic result, release, numerical proof of all functions, arbitrary root solver, full integration, engine cancellation guarantee, thread-safety claim, or accessible AURA/dw.discovery skill.
