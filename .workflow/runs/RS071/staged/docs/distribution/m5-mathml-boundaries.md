# MathML 3 annotated boundary audit — local codecs preview.6

MathMlBoundaryAudit.Audit(string) independently verifies the
narrow exact MathML 3 Content/Presentation profile admitted by
ExactMathMlInterop. This version adds explicit loss/refusal
classification and the SHA-256 of bounded original input bytes.

An admitted exchange is structurally exact only when its
typed Math IR survives canonical JSON transport, canonical
Math text and regenerated Content and Presentation MathML,
with matching original annotated XML. An admission includes
source hash, structural semantic SHA-256 and a versioned
audit rule. It does not prove algebraic equivalence or
make Presentation MathML reversibly parseable.

| Input | Audit outcome |
| --- | --- |
| Annotated exact half 1/2 | Admitted, no structural loss |
| Annotated original x=1 restriction | Admitted original Math IR |
| Standalone Presentation mfrac | PresentationWithoutAnnotation |
| Changed Content numerator | AlteredSemanticBinding |
| Unreduced source annotation 2/4 | NonCanonicalSerialization |
| Foreign MathML namespace | UnsupportedProfile |
| DTD or external entity | InvalidXml |
| 4097-character input | ResourceLimit, no source hash |

A refused bounded input can retain its source-byte SHA-256
for traceability. It contains NO mathematical source expression,
canonical text, structural IR SHA, presentation or proof rule.
Oversized source is refused before any source-hash allocation.

The implementation reuses a qualified XmlReader configured
with DTD prohibited, resolver null, and explicit character/node/
depth bounds. This is not a generic XML sanitizer, arbitrary
MathML interpreter, TeX executor or stable 1.0 public API.

No public NuGet publishing, external AURA/Decision/MCDM adoption,
algebraic solver or dw.discovery broker exposure is claimed.
Further M5-W01-C04 Markdown/Office and M5-W02 promotion gates
remain separate.
