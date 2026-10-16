# M5-W01-C04-T1 — exact Markdown/Unicode and standalone OMML

## Accepted T1

MarkdownUnicodeMathCodec renders a bounded Unicode display of an already-qualified
Math IR expression and exposes explicit Inline or Block math placement. The
first has single dollar delimiters; the second uses a pair of double-dollar
delimiter lines. The Unicode fraction slash is a glyph, not an inverse grammar.
The result retains the independent canonical text, source typed IR replay,
semantic structural SHA-256, and a versioned Math rule.

ExactOfficeOmmlCodec is a separate .NET 10/BCL-only codec for canonical
standalone Office Math OMML. Its admitted root is m:oMath in the Open XML
math namespace, containing only m:r/m:t runs and m:f/m:num/m:den fractions.
Admitted IR is restricted to the previously qualified MathML subset: exact
rational scalars, free real x/y/z, a two-operand ordered sum, and one original
equality restriction. An equality is an original typed restriction, not a
solver result. No precision loss or symbol-domain invention is authorized.

## Original-IR-bound import

Render clones the original typed IR through canonical JSON and exposes its
qualified semantic structural digest. ImportCanonical(xml, originalIr) requires
the caller to pass the retained original IR; it never reconstructs domains,
symbol scope, units, restrictions or semantic equivalence from glyphs.
Import securely parses bounded standalone XML, rejects DTDs/entities and
non-OMML roots, regenerates canonical OMML from the independently provided
original IR, requires structural equality and exact canonical serialization,
and refuses altered math rather than substituting new IR. A standalone,
source-free typography inverse is explicitly unqualified.

## Verification boundary

T1 requires one compiled absent-public-API RED, independent fraction,
equation, Markdown inline/block and typed refusal GREEN tests, focused codec
and IR/quantities regressions, and a separate pinned NuGet-only consumer with
eight independent probes. Three local generated product archives must be
inspected for actual net10 assemblies, nuspec dependencies, bytes and SHA-256.
No Office application, Office SDK, external CAS or agent broker is needed.
The exact local codec version is 1.0.0-preview.7.

C04-T2 remains responsible for wider unsupported input, whole-document
conversion, font/positioning and semantic-loss boundaries. Completing T1
does not close C04, M5-W01 or M5. This is neither a general OMML interpreter,
a portable standard Markdown math parser, nor stable/public NuGet.
