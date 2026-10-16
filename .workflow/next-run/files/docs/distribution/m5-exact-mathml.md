# MathML exact annotated interchange — local codecs preview.5

ExactMathMlInterop.Render and ImportAnnotated provide a narrow
MathML 3 Content + Presentation exchange bound to exact typed
Math IR, without inferring mathematical meaning from display
typography. The profile uses the standard MathML namespace.

An annotated exchange has presentation in a semantics element,
with a versioned Math canonical-text annotation and independent
Content MathML annotation-xml. The importer requires ALL
three views to match the same original Math IR.

Accepted exact examples:
- 1/2: Content apply/divide with integer-typed cn values,
  Presentation mfrac, original 1/2 Math canonical text.
- x/y/z: Content ci type=real, Presentation mi, original
  free real identities (not arbitrary LaTeX identifiers).
- add(x,1): Content apply/plus, Presentation mrow plus.
- where(x,eq(x,1)): a typed original equality restriction
  shown as Content apply/eq, not a solved equation.

Standalone Presentation mfrac cannot certify exact rational
content and is refused as PresentationAmbiguous. Content
MathML without this profile's annotations is also unqualified.
A tampered Content branch, presentation, source annotation,
DTD or external entity never yields admitted IR or proof.

Only a canonical serialized MathML subprofile is admitted,
not every syntactically valid or semantically equivalent MathML
document. The renderer validates existing structural Math IR
hashes. Input XML budgets are 4096 characters, 32 elements
and 12 element levels; the next T2 gate independently tests
and refines security and loss-reporting boundaries.

The versioned local package dw.tools.math.codecs preview.5
is not a public NuGet/stable 1.0 release. No MathML-to-TeX
execution, arbitrary XML adapter, algebraic CAS, agent
broker or AURA/Decision/MCDM adoption is claimed.
