# M5-W01-C02-T1 — Exact bounded LaTeX import and explicit juxtaposition policy

## Accepted responsibility and scientific boundary

The M5-W01-C02 accepted contract requires a LaTeX importer or
adapter with typed success, invalidity, unsupported shape and
mathematical ambiguity. The first accepted independent result is
exact 1/2 from the LaTeX fraction \frac{1}{2}; apparent function
application versus implicit multiplication requires an explicit,
caller-declared interpretation convention. This is not a universal
TeX compiler, broad text parser, nor an inversion of the T1/T2
display-only renderer.

The pre-existing IR contract math-ir/1 and the qualified
dw-math-canonical-text/1 codec are reused as product authorities.
No algebraic simplification, binary64 conversion, provider AST,
foreign symbolic CAS or interpreted TeX is introduced.

## Closed v1 importer

BoundedLatexImporter.Import(string, LatexJuxtapositionConvention)
is a deterministic, resource-limited, non-evaluating pure .NET
parser. It admits only these exact shapes:

- canonical signed integers and reduced TeX rational fractions
  such as \frac{1}{2} and -\frac{2}{3}, with strictly positive
  denominator, no implicit decimal approximation and a 128-bit
  rational magnitude budget;
- free real x/y/z with original typed Math IR identity;
- unary \sqrt{x} or \sqrt{exact}, and \left|x\right|;
- explicit x\cdot y or exact fraction\cdot symbol;
- bounded + and - expression nesting and passive parentheses,
  including T1 \left(...\right) display grouping.

Juxtaposed x(y) is Ambiguous by default. With the **explicit**
LatexJuxtapositionConvention.Multiplication, x(y) becomes the
typed ordered multiplication mul(x,y), and the chosen convention
is returned with the admitted result. An arbitrary f(x), unknown
symbol or function macro is never inferred. Adjacent xy remains
ambiguous even under this narrow convention.

The distinct T1 IR constructors square(x) and pow(x,2) produce
the same \left(x\right)^{2}. Its import therefore returns
Ambiguous with no source IR, canonical text, structural SHA-256,
proof rule or admitted convention. A format that lost constructor
identity must not quietly reconstruct one. Other exponent
notations require separate qualification.

An accepted result contains exact Math IR, canonical Math text,
original structural semantic SHA-256, a versioned import rule,
and the effective ambiguity convention. A refusal returns
neither IR nor canonical output/proof, and differentiates
Ambiguous, Invalid, Unsupported, NonCanonical and BudgetExceeded.

## Resource, invalid-input and loss boundaries

Maximum incoming LaTeX is 512 ASCII characters and its
structural recursive descent is limited to 12 levels. Only a
closed set of fraction, radical, absolute, grouping and
multiplication tokens is interpreted; unknown control sequences
are data to refuse, never commands to execute. Missing braces,
denominator zero, unknown functions, unsupported Unicode, empty
input, unreduced 2/4, excessive precision and deep formulas are
falsified separately. No unrestricted macros or arbitrary
external TeX are passed through a renderer.

This T1 slice admits only mathematically lossless normalized
exact representations. Any unqualified syntactic/presentation
loss (including unapproved fraction normalization or unknown
operator intent) is a typed refusal rather than silently accepted
conversion. The separately accepted T2 boundaries/integration
task will qualify richer loss metadata, ambiguity migration and
adversarial macro/depth behavior; do not pre-complete it here.

## RS068 qualification evidence

Observe one compiled missing-public-API reflection RED against
the qualified codecs preview.2 library. Recover it and pass
50 independent MSTest GREEN oracles, including exact fraction
math, identity, explicit multiplication, caller-selected
juxtaposition, function-vs-product ambiguity, squared-display
non-injectivity, hostile macros, invalid syntax, noncanonical
fractions and bounded exhaustion. Run the full focused codecs
test pack plus relevant Math IR and quantities regressions.

Three local product packages must be inspected by a separate
C# SHA-256/nuspec/net10 checker:
dw.quantities 0.2.0-preview.1, math.ir 0.3.0-preview.1,
and math.codecs 1.0.0-preview.3. Eight independent local-feed,
package-only .NET consumer oracles demonstrate the new import
contract without any source ProjectReference or agent/CAS runtime.

Only T1 RED/GREEN/verification and T1 may complete.
M5-W01-C02 starts in-progress, as do the already active M5
and M5-W01; T2 stays not-ready, and no M5 gate or public
NuGet publication is implied.
