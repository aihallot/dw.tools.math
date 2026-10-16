# M5 exact Markdown/Unicode and Office OMML (T1)

Install the local prerelease package dw.tools.math.codecs 1.0.0-preview.7
from a verified local feed. It transitively requires dw.tools.math.ir
0.3.0-preview.1 and dw.quantities 0.2.0-preview.1. No Office installation
or Open XML SDK is required.

- MarkdownUnicodeMathCodec.Render(originalIr, MarkdownMathPlacement.Inline)
  returns bounded Unicode text and a single-dollar inline math wrapper.
- MarkdownUnicodeMathCodec.Render(originalIr, MarkdownMathPlacement.Block)
  returns bounded Unicode text and a double-dollar multiline block wrapper.
- ExactOfficeOmmlCodec.Render(originalIr) returns canonical standalone OMML
  in the standard Open XML Office math namespace.
- ExactOfficeOmmlCodec.ImportCanonical(omml, originalIr) verifies the exact
  supplied original IR against canonical OMML and refuses ambiguous import.

The original mathematical IR, canonical text, and semantic structural SHA-256
are retained on success. Unicode and OMML display alone cannot establish
the original mathematical meaning. Typed refusals contain no IR, hash or rule.
Only exact rationals, x/y/z free real symbols, a qualified ordered sum and
an original simple equality restriction are admitted. Arbitrary Office
documents, typography, layout/fidelity, fonts, inverse Unicode parsing,
unsupported domains and arbitrary OMML are not qualified.

Independent acceptance is a compiled RED, focused GREEN, eight isolated
NuGet-only consumer probes, archive and dependency verification. T1 has no
authority to close C04, W01 or M5 or to publish stable/remote packages.
