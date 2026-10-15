# Local Math canonical text and display-only LaTeX (M5 preview)

The Math codecs library 1.0.0-preview.1 provides a bounded, exact
typed interop subset. Parse a canonical ASCII expression with
CanonicalMathNotation.Parse(text); render an admitted IR with
CanonicalMathNotation.Render(node). Both return a typed status,
canonical text, original IR, structural semantic SHA-256, and
display-only LaTeX on successful admission.

Examples:

| Canonical text | Meaning |
| --- | --- |
| 1/2 | exact rational half; LaTeX \frac{1}{2} |
| add(square(x),mul(2,x),1) | x squared plus twice x plus one, original tree |
| sqrt(square(x)) | symbolic radical, without an invented domain simplification |
| unit(5/2,imperial.inch,None) | exact 5/2 with BritishImperial length metadata |
| where(div(sub(square(x),1),sub(x,1)),ne(x,1)) | rational function with original x != 1 hole |

The canonical grammar is *not* arbitrary infix mathematics or general
LaTeX input. Units are a three-entry closed whitelist, labels are
x/y/z, and operation/condition names are closed and case sensitive.
Noncanonical rational aliases and unsupported syntax are refused.
A successful rendering does not prove the rendered LaTeX can be
inversely parsed, nor does it evaluate or simplify the source IR.

No TeX engine, arbitrary text execution, general CAS, complex roots,
public NuGet publishing, stable 1.0, agent permissions or external
adoption is qualified. T2 will independently challenge presentation
escaping, grammar and ambiguity boundaries.
