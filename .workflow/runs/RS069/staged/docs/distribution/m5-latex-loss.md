# Exact LaTeX import loss audit — local codecs preview.4

LatexImportBoundaryAudit.Audit wraps the qualified
BoundedLatexImporter without changing its grammar. It adds
a typed LatexImportLossKind and a source UTF-8 SHA-256
fingerprint to support independent mathematical consumers.

An admitted import includes exact Math IR, canonical text,
structural semantic SHA-256, an audit rule and the effective
juxtaposition convention. The original input fingerprint
binds source bytes, NOT mathematical meaning.

Examples:

| Input | Interpretation |
| --- | --- |
| x | exact free-real x, no representation loss |
| \frac{1}{2} | exact 1/2, passive notation change |
| x(y), default | Ambiguous, no mathematical result |
| x(y), explicit Multiplication | exact mul(x,y), policy-bound |
| \left(x\right)^{2} | Ambiguous, square versus power |
| \input{private} | Unsupported; never executed |
| \frac{2}{4} | NonCanonical; no silent rewriting |
| \frac{1}{0} | Invalid; undefined denominator |

The source fingerprint can remain on a bounded refusal
without implying semantic admission. Over the input
budget, the audit refuses before allocating source-hash
bytes. Lossless here means exact structural Math meaning
under the stated convention, NOT lossless TeX typography
or reversible LaTeX formatting.

Only local packages are qualified. No public NuGet
publication, stable 1.0, broad TeX support, foreign
CAS runtime, AURA/Decision/MCDM adoption or neutral
dw.discovery skill is included. M5-W01-C03 and the
M5 gate remain separate future qualifications.
