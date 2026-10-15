# M4-W02-C01-T1 — qualified exact real formal polynomial differentiation

**Status:** proposed for RS059 controlled RED/GREEN qualification. No capability is declared durable before a successful DWF report and its source-backed scientific evidence.

## Question and accepted contract

M4-W01 delivered bounded, provider-neutral exact IR cancellation, transformations, restrictions and admission/refusal with no AngouriMath or MathNet.Symbolics product dependency. M4-W02 begins **symbolic calculus**. The first falsifiable requirement is d(x³)/dx=3x², d(x³)/dy=0 for an absent independent free symbol y, and explicit refusal rather than a fictitious numerical derivative for unit-bearing quantities.

RS059 introduces dw.tools.math.symbolics **0.5.0-preview.5**, a new typed SymbolicRealFunction and the SymbolicExactDifferentiation.TryDifferentiate entrypoint.

- SymbolicRealFunction explicitly carries a free real Parameter, typed IR Body and Real Domain/Codomain. Unsupported integer/bound parameters, integer codomain or non-real domain are refused at construction rather than coerced.
- TryDifferentiate(IrNode,IrSymbol) admits only exact scalar constants, independent free real symbols, square(symbol), positive integer power(symbol,n) where 1<=n<=8, Add and Subtract of the admitted subset, and constant times a single admitted term. This is a **formal exact real polynomial** derivative; other free symbols are independent algebraic parameters.
- D(x³,x) returns Multiply(3,Power(x,2)), D(y³,x) returns the **exact** rational zero, D(x,x)=1 and D(exactConstant,x)=0. No implicit binary64 or a text-based calculation is used.
- Each admitted result has the fixed proof rule formal-polynomial-derivative/exact/v1, a canonical IR structural SHA-256 of the **source**, and the exact identity of the variable differentiated with respect to. This identifies the qualified operation, not an unrestricted equivalence theorem.
- The existing math-ir/1 codec validates and copies the derivative. Exact rational coefficients, free vs bound identities and real scalar domain remain explicit; no provider AST is admitted.

## Explicit refusals and nonclaims

- Power exponent 9 and higher returns BudgetExceeded; zero/negative/fractional exponent forms are Unsupported until the domain/singularity/branch contract is qualified.
- Multiply of two nonconstant symbolic operands, quotient rules, arbitrary chain rules, absolute value at its non-differentiable point, real sqrt branches, matrices, approximate IEEE binary64 and foreign expressions are Unsupported, not falsely differentiated.
- An IrQuantityLiteral, including a unit literal inside a product, returns **UnsupportedUnits**; dimensional derivative and derived units are not silently inferred.
- An IrRestrictedExpression returns **UnsupportedAssumptions**; the mathematical result never drops an existing x!=a, x>=a or other typed restriction. Integer/bound symbols and non-real differentiation variables return **UnsupportedDomain**.
- The IR graph remains subject to its existing maximum 1024 nodes and 32 levels, and this closed derivative recursion has maximum depth 16. This is a local rule, not a general-purpose time/CPU scheduler or provider cancellation promise. More detailed non-differentiable points, unit derivative conversion, conditional forms, precision and execution budgets belong to C01-T2.

## Falsification, provenance, release

1. With existing preview.4 built from locked .NET 10 solution, compile and execute an independent reflection RED that detects the **absent** public TryDifferentiate(IrNode,IrSymbol). A build or NuGet restore error is **not** valid RED evidence.
2. Install the preview.5 product source and 38 independent mathematical GREEN tests. Require exactly 39 targeted passing tests including the recovered RED. Mathematical oracles include x³, y³, x², coefficient products, negative/fractional constants and explicit refusals, plus canonical round-trip, domain/codomain and proof-variable provenance.
3. Run complete relevant symbolics, typed IR and exact quantities regression suites, and ensure the solution builds in locked mode. Keep broad release certification for the actual G-M4 boundary.
4. Pack three local .NET 10 products dw.quantities 0.2.0-preview.1, dw.tools.math.ir 0.3.0-preview.1 and dw.tools.math.symbolics 0.5.0-preview.5. Recheck actual nuspec identities, transitive package graph, compiled DLL entries, byte lengths and fresh product-only SHA-256s using a separate C# verifier; do not calculate workflow or project-plan hashes.
5. Run eight independent oracles from a NuGet-only consumer with one direct pinned symbolics PackageReference and an isolated cache/local feed, no ProjectReference or external sources. Verify d(x³)/dx, absent variable zero, formal real domain and codomain, typed unit/non-differentiability refusal, polynomial degree bound, provenance and no external CAS/broker dependencies.

Only M4-W02-C01-T1-R/G/V and T1 become done on durable RS059 success. The M4-W02 work package and C01 phase become in-progress; C01-T2, later W02 phases and milestone M4 remain unfinished. No public NuGet feed, neutral dw.discovery skill, Decision/MCDM or AURA broker permission is implied.

**Review triggers:** higher polynomial orders, differentiating arbitrary functions or products, unit-derived results, domain restrictions, complex branches, provider AST integration or hard execution deadlines.
