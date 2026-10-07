# Sources and provider qualification

Initial consultation: 2026-10-07. The sources below inform spikes; they do not announce qualified dependencies.

| Primary source | Use | Current decision |
|---|---|---|
| [Math.NET Numerics](https://numerics.mathdotnet.com/) | Linear algebra, probability, interpolation, integration, and other computation; project advertises MIT and managed/native implementations | Priority numerical candidate; version, RID, and transitives to qualify in M3 |
| [AngouriMath](https://github.com/asc-community/AngouriMath) | C#/F# symbolic library, repository advertises MIT | CAS candidate; test assumptions, solutions, cancellation, and conversions |
| [Math.NET Symbolics](https://symbolics.mathdotnet.com/) | Another symbolic candidate in the .NET ecosystem | Compare useful coverage; do not assume CAS interchangeability |
| [SymPy — gotchas](https://docs.sympy.org/latest/tutorials/intro-tutorial/gotchas.html) | Expression semantics, equality, and conversion pitfalls | Optional reference/oracle; no Python in the mandatory DWF path |
| [OpenMath](https://openmath.org/standard/) | Semantic objects and content dictionaries | IR comparison before custom model |
| [MathML](https://www.w3.org/TR/mathml4/) | Presentation/content distinction and semantic annotations | Pin subset and standard version; never confuse display with meaning |
| [UnitsNet](https://github.com/angularsen/UnitsNet) | Quantity catalog and calculations to examine | Evaluate coverage and numeric representation; no automatic replacement of exact rational behavior |
| [.NET file-based apps](https://learn.microsoft.com/en-us/dotnet/core/sdk/file-based-apps) | C# execution through the .NET SDK | Supports the documentation validator without NuGet dependency |

Local sources: source-baseline.json (hashes of read files), existing-code-inventory.md, brainstorming framing, planning, and active Decision guidance observed at initialization.
DWF guidance IDs/versions are not eternal contracts: the executable used at adoption decides compatibility.

## Required admission dossier

For each serious candidate: immutable version, source URL/commit, exact license and transitive/native notices, verified redistribution rights, maintainer/activity observation, TFM/RID, offline behavior, installation, interfaces, errors, cancellation, budgets, exactness, precision, thread-safety, and reproducibility.
An uncertain legal-license choice is an owner question, not an automatic conclusion from a README.
Compare at least two plausible options per family when available; otherwise document why a second option is not relevant.
An unsuitable option ends in reasoned rejection or proposed scope reduction; it does not justify reimplementing a CAS.

## Sequence

M0: transfer provenance and rights; M2: IR/standards; M3: numerical; M4: symbolic; M5: codecs; M6/M7: new families.
No exact provider version or transitive capability is frozen without a smoke test on the real package.
