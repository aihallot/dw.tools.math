# Display audit for canonical Math notation (local codecs preview.2)

CanonicalDisplayBoundaryAudit.AuditText and AuditIr
independently verify existing exact T1 text/IR transport before
emitting the renderer's passive LaTeX. A successful result includes
typed original IR, canonical text, display-only LaTeX, structural
semantic SHA-256 and a versioned rule.

The audit detects both original-domain errors and display ambiguity:

- where(x,ne(y,1)) is refused: the condition targets an unbound
  expression symbol.
- where(x,eq(x,1),ne(x,1)) is refused: exact hypotheses contradict.
- square(x) and pow(x,2) have different IR structure/hash but
  emit the identical display LaTeX \left(x\right)^{2}.
  Both return SafeDisplayWithCollision, not a reversible
  LaTeX codec guarantee.

All admitted displays are passive. The internal checker recognizes
only the renderer's small ASCII command vocabulary:
frac, left, right, cdot, sqrt, mathrm, ne, ge plus spacing
punctuation and balanced braces. External LaTeX is never parsed,
compiled or executed. Arbitrary macros such as \input are refused
by the upstream canonical Math text grammar.

The local .NET 10 product dw.tools.math.codecs/1.0.0-preview.2
is separately versioned from its IR and quantities dependencies,
with local archive SHA-256 evidence. This does not constitute
public NuGet publication, stable 1.0, arbitrary symbolic algebra,
MathML/Office support, external AURA/Decision adoption or
dw.discovery skill exposure.
