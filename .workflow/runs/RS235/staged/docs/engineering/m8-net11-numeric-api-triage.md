# M8-W02-C01 — numeric API usage triage evidence

Status: *lexical scan only; semantic compatibility unqualified*. This checkpoint follows RS234, which installed a read-only numerical API occurrence scanner for the thirteen packable Math source trees.

## Evidence captured by RS235

The controlled RS235 payload publishes five bounded Markdown search artifacts from the current `src/projects` source tree, preserving file names, line references and exact matches for the following signal families:

| Artifact | Literal pattern | Follow-up |
| --- | --- | --- |
| `numeric-math-round.md` | `Math.Round(` | Check overload binding, midpoint modes, tolerances and .NET 11 behavior |
| `numeric-biginteger.md` | `BigInteger` | Separate exact integer semantics from floating-point conversion and rounding boundaries |
| `numeric-complex.md` | `Complex` | Establish whether Complex arithmetic is used; check NaN/infinite behavior only if applicable |
| `numeric-convert.md` | `Convert.To` | Inspect integer narrowing and conversion policy, distinguishing irrelevant overloads |
| `numeric-checked.md` | `checked(` | Inspect overflow handling where the literal syntax exists |

Search results are lexical and can contain comments, unrelated symbols, or misses due to aliases, whitespace and indirect calls. Empty results do not prove absence of API usage. Each artifact has a maximum of 2,000 matches; review truncation metadata before concluding coverage. The earlier `docs/engineering/InspectM8Net11NumericApiSignals.cs` remains the broader lexical pattern source.

## Qualification boundary

No SDK pin, target framework, Math code, published NuGet archive or sibling repository is changed. This evidence does not qualify package binary compatibility, semantics or migration readiness. After inspection, choose a small set of actual Math call sites and formulate independent net10/net11 regression fixtures before proposing a migration or marking M8-W02-C01 ready.
