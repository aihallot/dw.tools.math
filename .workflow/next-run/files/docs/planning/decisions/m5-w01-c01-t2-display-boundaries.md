# M5-W01-C01-T2 — Exact canonical display boundary

## Accepted responsibility

T2 closes the M5-W01-C01 boundary: bound text escaping,
depth, and permitted LaTeX display commands; execute no TeX
and do not claim typographic fidelity or a reversible LaTeX parser.
T1 already admitted the closed exact canonical ASCII grammar
dw-math-canonical-text/1. A distinct public Math-only display
auditor now qualifies that exact T1 contract without broadening its
parser, introducing a CAS, or accepting external LaTeX.

## Audited design

CanonicalDisplayBoundaryAudit.AuditText(string) and AuditIr(IrNode)
delegate to the T1 grammar/rendering and independently verify:

1. Math IR JSON Decode(Encode(node)) is byte-identical and the
   typed source semantic structural SHA-256 survives the roundtrip.
   Canonical text -> IR -> canonical text is also checked through
   the accepted Math codec. Neither is a general algebraic proof.
2. An original-domain assumption must reference a variable
   actually present in its expression, not an unrelated symbol.
   Exact equalities incompatible with not-equalities, a different
   equality or an admitted lower bound are refused without
   an expression, LaTeX or proof marker.
3. Only renderer-generated LaTeX is inspected; an ASCII-only,
   bounded brace scan admits these command tokens:
   frac, left, right, cdot, sqrt, mathrm, ne, ge, with explicitly
   known punctuation-spacing escapes. The whitelist is not a
   generic external TeX sanitizer or executor.
4. There is an observable loss of structural invertibility:
   square(x) and pow(x,2) produce the identical LaTeX
   \left(x\right)^{2}, yet their canonical text and original
   IR structural hashes differ. A successful safe display
   explicitly exposes SafeDisplayWithCollision and
   PresentationMayCollide; LatexInverseQualified always remains
   false, even for displays without a known collision.
5. Original T1 refusal statuses are retained as a typed cause;
   any refused operation carries no expression, canonical text,
   LaTeX, hash or rule ID. An admitted boundary carries all of
   them plus a versioned audit rule, never runnable TeX.

## Independent qualification

One compiled reflection-based missing-public-API RED must be
observed against the previously qualified codecs preview.1 before
the boundary source is installed. Then recover the RED and pass
51 independent GREEN tests, with exact/original-domain/ambiguity,
forbidden commands, depth, units, finite-binary64 and explicit
typed refusals. The native solution must restore locked, build
Release and pass the focused codec suite plus relevant IR and
quantities regressions.

Eight independent NuGet-only client oracles must run against one
pinned codecs preview.2 dependency with transitive Math IR and
quantities and no ProjectReference. Inspect three freshly generated
local product archives with a separate C# package helper for
net10 assembly, manifest IDs/versions, direct dependencies,
actual byte sizes and SHA-256.

## Accepted completion and deferrals

RS067 may finish only M5-W01-C01-T2-R/G/V, T2 and phase C01,
after actual evidence. M5-W01 and M5 stay in-progress; C02 is
not-ready until a separate bounded LaTeX import run is prepared.

No unbounded infix text, arbitrary user TeX parsing, macro execution,
opaque provider AST, general exact algebraic equivalence,
MathML/Office interoperability, inverse display fidelity,
public NuGet, stable 1.0, AURA/Decision/MCDM adoption or
dw.discovery broker permission is admitted.
