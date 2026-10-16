# M5 bounded math display and Office interoperability

Local prerelease: dw.tools.math.codecs 1.0.0-preview.8 on .NET 10.
Its local transitive dependencies are dw.tools.math.ir 0.3.0-preview.1
and dw.quantities 0.2.0-preview.1. No Office installation or SDK required.

Use MathDisplayBoundaryAudit.AuditOmml(omml, originalMathIr) for the
previously admitted canonical standalone OMML subset. Use
AuditMarkdown(markdown, placement, originalMathIr) for exact Unicode math
wrappers and explicit Inline/Block placement.

A qualified result retains original typed Math IR, canonical text,
semantic structural SHA-256, exact canonical representation and a bounded
source SHA-256 fingerprint. Each refusal returns typed status/loss and
no mathematical IR, canonical expression, semantic proof or rule.
Oversized inputs cannot trigger source-hash allocation.

Do not use these APIs to convert complete Word/Excel/PowerPoint documents,
interpret font runs or positioning, infer symbol scope from glyphs, execute
entities, translate arbitrary TeX/Markdown/OMML, or promise typography
equivalence. Unsupported formatting and all foreign containers are refused.
A bounded source SHA-256, including on refusal, is not a semantic proof.

C04 and M5-W01 close only after the T2 independent verified gate;
M5 and its separate M5-W02 package/distribution qualification stay open.
No public stable release or external repository adoption is asserted.
