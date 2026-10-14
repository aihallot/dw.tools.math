# M4 typed symbolic provider result admission — local preview.2

This T2 increment deliberately **refuses opaque external CAS outputs**. A symbolic result proposed as arbitrary text or foreign objects cannot be accepted as a mathematics proof, even when it contains a phrase such as "provided x!=1".

The public Math library references only dw.tools.math.ir (and transitive dw.quantities). The new SymbolicProviderResultAdmission API accepts an IrRestrictedExpression only if canonical math-ir/1 serialization agrees with a *fresh* result that the prior T1 exact structural rule can prove for the original source expression. Its accepted domain remains the same known one free real symbol and rational identity:

  (x²-c²)/(x-c) -> x+c, condition x != c.

An unconditional x+c, an alternative excluded point, a new extra assumption, a dropped prior assumption, a different symbol, a binary64 widening, and a provider-supplied printable "provided" string are all refused. The gate does not call object ToString or execute user-provided scripts. The gate returns the locally proven conditional expression rather than the proposed instance.

Scientific executable qualification:
- One compiled missing-gate RED against an existing preview.1 assembly, then 29 GREEN tests, for 30 targeted passes.
- Existing full symbolics / IR / exact quantities regression suites.
- A separate eight-probe .NET 10 consumer with only dw.tools.math.symbolics 0.5.0-preview.2 PackageReference, restored exclusively from three newly built local Math archives.
- A separate, archive-only C# manifest/SHA-256 verifier: dw.quantities 0.2.0-preview.1 -> dw.tools.math.ir 0.3.0-preview.1 -> dw.tools.math.symbolics 0.5.0-preview.2; product artifacts remain local, ignored and not publicly published.
- Exact consumer marker: dw.tools.math/m4-provider-admission-consumer/0.5 qualified; probes=8.

This package has *no AngouriMath runtime reference*. C01 established a provisional provider candidate and process-boundary limitations; it did **not** qualify a native typed adapter. Provider-specific text extraction, unbounded CAS execution, general equivalence, algebraic proofs for arbitrary expressions, other operating systems, security sandbox guarantees and agent invocation are all deferred.

RS056 can close the accepted C02 only after its own durable tests and evidence. M4 symbolic release gate G-M4 and following C03 transformation/substitution work require later independent qualification.
