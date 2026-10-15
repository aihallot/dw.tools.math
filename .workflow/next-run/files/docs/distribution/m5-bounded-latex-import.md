# Math bounded LaTeX import — codecs local preview.3

BoundedLatexImporter.Import is a small .NET 10, non-executing
mathematical interchange adapter. Its default policy refuses
unqualified interpretation of juxtaposition.

Qualified examples:

| Input LaTeX | Result |
| --- | --- |
| \frac{1}{2} | exact 1/2 Math IR and canonical text |
| -\frac{2}{3} | exact signed -2/3 |
| \sqrt{x} | unevaluated sqrt(x) |
| x\cdot y | typed mul(x,y) |
| x+1 | typed ordered add(x,1) |
| x(y) | Ambiguous unless the caller selects Multiplication |
| \left(x\right)^{2} | Ambiguous: square(x) vs pow(x,2) |
| \frac{2}{4} | NonCanonical, no implicit lossy rewrite |
| \input{file} | Unsupported; never interpreted or executed |

The importer is intentionally NOT the inverse of arbitrary
LaTeX produced by Math. It does not accept unrestricted TeX
macros, broad function vocabularies, general exponents,
complex branches, arbitrary user variable names, units,
assumptions or universally invertible typography.

Results distinguish Admitted, Ambiguous, Invalid,
Unsupported, NonCanonical and BudgetExceeded. Admitted
results carry IR, a canonical text representation,
source structural SHA-256, versioned rule ID and the
effective juxtaposition convention; refusals carry none
of those mathematical artifacts.

This local Math codec preview does not claim a published
NuGet package, stable 1.0 API, AURA/Decision/MCDM adoption
or dw.discovery skill exposure. M5-W01-C02-T2 owns further
semantic-loss metadata and adversarial import boundaries.
