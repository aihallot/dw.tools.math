# M5-W01-C01-T1 — Canonical exact Math text to typed IR and LaTeX

## Why a separate codec library

The accepted M5 1.0 interop backlog requires mathematical text, rational
polynomials, functions, quantities and source exclusions to pass through a
typed Math IR before rendering. The canonical Math IR is already
qualified in M2; Math symbolic rules in M4 may not be widened merely
because a presentation string looks like an equivalent formula.

A new .NET 10 provider-neutral product library
src/projects/dw.tools.math.codecs/ owns the notation grammar and
display-only LaTeX renderer. It directly references Math IR, with exact
quantities arriving transitively. Product and test projects are added
to the existing solution with explicit locked NuGet dependency graphs.
No existing symbolic or numerical package is silently modified.

## Accepted closed grammar (T1)

The grammar ID is dw-math-canonical-text/1. Its exact canonical
ASCII text is prefix-structured, not a permissive infix parser:

- exact normalized signed rationals: -2/3, 0, 1/2, 7;
- three free real symbols: x, y, z, with equal display and stable
  identity; no bound-variable interpretation;
- add/mul with 2–8 operands, sub/div with two, square/sqrt/abs with
  one, pow(base,n) with an integer exponent from zero to eight;
- unit(exact,identifier,temperatureKind), whitelisted as
  si.metre/None, imperial.inch/None, si.kelvin/Absolute,
  with exact DimensionVector and UnitSystem metadata;
- where(expression,ne|eq|ge(symbol,rational), ...), with 1–8
  retained original-domain typed assumptions.

A canonical absolute-temperature example is unit(5463/20,si.kelvin,Absolute): its exact value equals 27315/100 K, but the unreduced input unit(27315/100,si.kelvin,Absolute) is NonCanonical. Scientific equality does not waive canonical text normalization.

The parser refuses noncanonical aliases such as 2/4, 01 and -0;
implicit decimals, unit aliases, arbitrary identifiers, general
infix and arbitrary LaTeX commands are not part of this *canonical*
mode. An accepted source is not evaluated or simplified, and a
division by a symbolic denominator does not silently cancel an
excluded original-domain point.

The typed result binds original Math IR, canonical text, display-only
LaTeX and IrCanonicalHashes.SemanticStructuralSha256. The Math IR
JSON transport is the independent semantic roundtrip check;
text -> IR -> LaTeX does not imply LaTeX -> IR. Reconstructing
meaning from rendered typography alone is not claimed.
Unadmitted nodes and inputs have typed refusals with no expression,
text, LaTeX or semantic proof hash.

## Limits and scientific deferrals

Maximum canonical text length 1024 UTF-16 characters with canonical
ASCII input, maximum expression depth 16, 8 operator operands,
8 conditions and 128-bit exact rational magnitude. Output LaTeX
length is limited to 4096 characters. No TeX execution occurs.
All mathematical names/units/commands are whitelisted; neither
arbitrary user labels nor unknown unit conversions enter LaTeX.

T2 remains responsible for input escaping, nesting, forbidden
commands, misleading equivalent presentation and end-to-end
interoperation boundaries under adversarial inputs. M5-W01-C02
independently owns *bounded LaTeX import*, not this renderer.
M5-W01-C03/04 own MathML/Markdown/Office, and M5-W02 owns
cross-vertical 1.0 qualification; no public NuGet publishing or
AURA/Decision/MCDM/dw.discovery integration is admitted here.

## RS066 evidence threshold

Observe one genuinely failing compiled public-API RED against an
empty new codec project before installing source, then recover that
RED with 50 independent GREEN MSTests. Verify relevant M2 IR and
quantities regressions and the locked solution compilation. Eight
isolated NuGet-only consumer probes must run from one pinned codec
package and transitive IR/quantities packages, with all three local
product archives inspected independently for net10 assembly,
nuspec dependency graph, actual bytes and SHA-256.

Only M5-W01-C01-T1 RED/GREEN/verification and T1 complete; milestone
M5, work package W01 and phase C01 start as in-progress but remain
unfinished. T2 and all future M5 phases retain their original IDs
and are not promoted. A durable RS066 report, not this candidate
decision document, determines actual qualification.
