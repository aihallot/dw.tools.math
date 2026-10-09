# Inspectable composition results — math-composition/1

M2-W02-C01 delivers a standalone .NET 10 package `dw.tools.math.composition`, depending only on the qualified mathematical IR. It defines inspectable contracts, not a computational engine.

## Result and capability contract (T1)

Results have explicit `Exact`, `Approximate`, `Unsupported`, or `BudgetExceeded` status. Only success with a supplied matching typed IR numerical value carries a final value. Failures carry a bounded non-empty reason and **no** final value. No unsupported outcome is silently converted into success.

`MathValueDescriptor.Inspect` recognizes exact and finite-binary64 scalars and homogeneous scalar matrices without widening. `MathOperationCapability.MatrixInspection` advertises a matrix-only, 4096-element contract supporting exact and approximate matrix inspection. `Accepts` validates described shape; it does not invoke an operation.

## Integration and authority boundary (T2)

`MathCapabilityCatalog.Discover()` provides a stable immutable listing of public operation descriptions. `Find(id)` returns only known descriptions; unknown and null identifiers return no capability. Neither method accepts a provider, user identity, permission token, session, AURA handle, or execution environment. Discovery is **not** provider availability, authorization, scheduling, execution, or admission.

The public assembly boundary is checked for inadvertent provider, permission, or execution surface. Result and provenance types have no public constructors able to fabricate successful or authorized operations. The typed result explicitly distinguishes missing final values from supplied successful values; provenance retains only bounded operation id and contract version, not an execution trace.

Qualification requires a controlled targeted RED for absent discovery, followed by the independent T2 GREEN tests, the seven T1 contract regressions, IR tests, foundation verification, locked build, transfer architecture validation, and a locally packed NuGet manifest/assembly check. No external package publication or consumer/AURA adoption is claimed.

M2-W02-C01 is complete only after its boundary proof is executed and recorded. Subsequent M2-W02-C02 work will separately qualify compatible typed pipelines and direct/fluent evaluation; no pipeline or provider execution is delivered by C01.
