# M5-W01-C04-T2 — Office and Markdown boundary qualification

## Scope and trust boundary

M5-W01-C04-T1 already qualifies a closed exact profile: standalone canonical
Office Math OMML m:oMath with runs, exact m:f fractions, free real symbols,
one original equality restriction and a two-operand ordered sum; separately
rendered Unicode math with explicit inline/block Markdown placement. Neither
visual representation supplies a source-free mathematical identity.

T2 introduces MathDisplayBoundaryAudit as a typed, independent security and
loss-boundary auditor. It delegates OMML XML parsing entirely to the existing
ExactOfficeOmmlCodec.ImportCanonical. There is no second XML interpreter,
arbitrary Word/PowerPoint/Excel document processing, Unicode parser or CAS.

- AuditOmml(xml, originalIr) requires the caller-retained exact original IR.
  It checks the admitted canonical source bytes, reports typed underlying
  invalid XML, unsupported document/profile, lexical non-canonical markup,
  altered/content/layout markup and resource-limit refusals. Only a source
  reproduced from exact original IR can qualify.
- AuditMarkdown(markdown, placement, originalIr) refuses any non-identical
  wrapper and display. A byte-for-byte match against the *opposite*
  qualified placement is classified as PlacementMismatch, not converted.
  No Unicode, dollar-delimiter or LaTeX inverse is attempted.
- Both auditors independently replay canonical Math IR JSON and cross-check
  original canonical text and semantic structural SHA-256. Input fingerprints
  are lowercase SHA-256 of bounded original UTF-8 bytes, provenance only.
  Over-budget input is refused before source fingerprint allocation.
- Every refusal carries no expression, canonical text, representation,
  semantic digest or certification rule. Bounded source provenance on a
  refusal is never a mathematical proof.

Even an admitted standalone expression does not qualify any font, typography
inverse, positioning, whole Office document conversion, graphics or exact
visual layout. The output explicitly retains false fidelity flags, and all
style, positioning and extra OMML markup are refused. There is no Office
application, Office SDK, external CAS or agent broker dependency.

## Falsification and release boundary

T2 proves actual behavior against altered integer numerator/denominator and
equation operators, mismatched source IR, unrecognized XML namespaces,
m:oMathPara, a Word document root, m:rPr and m:fPr styling, positioning tags,
DTD/entities, malformed or non-canonical XML, and input budgets.

Markdown/Unicode tests falsify inline/block confusion, missing dollar wrappers,
ASCII fractions, Unicode minus substitution, TeX input, fenced code, links,
incorrect original symbol identity, unsupported expressions, invalid placement,
and over-budget input. These are explicit unsupported results: not repairs.

The acceptance gate is one genuinely compiled missing-public-API RED against
qualified codecs preview.7, all independent new GREEN oracles, locked .NET 10
Release build, focused codecs/IR/quantities regressions, eight independent
package-only NuGet consumer probes, and three independently inspected local
net10 archives with real bytes, SHA-256 and dependency graph.

If durable evidence passes, complete only C04-T2 R/G/V and T2, C04 and M5-W01.
M5 remains in progress and M5-W02 awaits its own independent work.
Codecs preview.8 is a local prerelease, not stable/public NuGet.
