# M5-W01-C03-T1 — Exact annotated Content and Presentation MathML

## Accepted scope and standards

C03 is the accepted Content/Presentation MathML work. T1 must
preserve exact fraction, symbol and equation meaning across
typed Math IR and a narrow, annotated MathML profile.
Presentation MathML by itself cannot prove an exact original
Math IR interpretation. Markdown and Office import are not
part of this chunk; those are separate accepted C04 work.

The profile pins MathML 3 namespace
http://www.w3.org/1998/Math/MathML and annotation-xml's
application/mathml-content+xml encoding. The profile's
additional canonical-source annotation encoding
application/x-dw-math-canonical-text-v1 is a Math-owned
extension, not a W3C-standard serialization of our IR.
See https://www.w3.org/TR/MathML3/mathml.html (annotation
framework) and https://www.w3.org/TR/mathml-core/ (semantics
and content annotation example).

## Qualified T1 grammar

The new .NET 10 ExactMathMlInterop.Render(IrNode) admits only
the existing canonical Math notation subset plus a SECOND
closed rendering constraint:

- exact signed irreducible rational numbers, with typed
  integer Content MathML numerator/denominator and passive
  Presentation mfrac/mn nodes;
- free real x, y and z, with Content ci type=real and passive mi;
- ordered simple add(symbol, exact) as Content apply/plus,
  with a Presentation mrow operator;
- one original restricted equality where(symbol,eq(symbol,exact))
  as Content apply/eq and Presentation mrow. This is a
  restriction on the original expression, NOT a solved
  equation, new relation type or inferred root.

No arbitrary symbols, functions, exponents, unit conversions,
complex branches or generic Content MathML library is introduced.

An admitted renderer returns original Math IR, canonical text,
Content MathML, passive Presentation MathML, one annotated MathML
document, the existing Math IR structural semantic SHA-256 and
a versioned rule. MathML's semantics element places presentation
first, followed by both an original canonical-text annotation
and explicit Content MathML annotation-xml.

## Import and trust

ExactMathMlInterop.ImportAnnotated(string) accepts only that
canonical, self-consistent annotated document. It:

1. Reads bounded XML through an XmlReader with prohibited DTD
   processing and no XML resolver. Input is capped at 4096
   characters, 32 XML elements and 12 element nesting levels.
2. Requires exact MathML namespace and known annotation
   encodings. No semantic annotation means no certified source
   IR: standalone Presentation MathML returns
   PresentationAmbiguous (and standalone Content MathML remains
   outside this annotated import profile).
3. Independently parses the canonical-text annotation using
   the qualified T1 codec, regenerates BOTH content and
   presentation from resulting Math IR, and checks element-tree
   equality and exact canonical XML lexical serialization.
   A forged annotation or tampered XML refuses with no
   expression or semantic hash.
4. Independently checks Math IR JSON transport and semantic
   structural identity before emitting any admitted result.

This is exact STRUCTURAL interchange, not a universal
typography-to-mathematics inference algorithm, TeX engine,
MathML DOM sanitizer or proof of algebraic equivalence.
The importer does not attempt to decode arbitrary standalone
Content or Presentation MathML without our specific annotation
profile. T2 owns stronger XML adversarial and semantic-loss
boundary integration, including deeper namespace/entity cases.

## Independent evidence and gates

RS070 observes one genuinely compiled public-API RED against
the already-qualified codecs preview.4 BEFORE installing the
MathML implementation. After recovery, 41 independent GREEN
MSTests cover exact rationals, real symbols, original equality
restrictions, simple ordered sums, MathML namespace/annotation
identity, IR JSON replay, tampering, ambiguous presentation,
DTD/entity rejection, XML nodes/depth/budgets and typed refusals.

Run the locked .NET 10 solution build, focused full codec
test pack and relevant Math IR and quantities regressions.
Independently inspect three local product packages with
net10 assembly, nuspec identities, direct dependencies,
bytes and SHA-256; an eight-oracle isolated NuGet-only
consumer uses one pinned codecs preview.5 PackageReference,
without source ProjectReference, foreign CAS or agent runtime.

Success completes only M5-W01-C03-T1 RED/GREEN/verification
and task T1; M5-W01-C03 begins in-progress. M5-W01 and M5
remain in-progress, and T2 remains not-ready. No public
NuGet publishing, stable 1.0, AURA/Decision/MCDM adoption
or dw.discovery broker integration is admitted.
