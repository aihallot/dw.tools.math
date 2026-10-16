# M5-W01-C03-T2 — MathML source provenance and semantic boundary

## Accepted product boundary

M5-W01-C03-T1 already qualifies a *closed* MathML 3 profile:
Content MathML integer rationals, free real x/y/z, one ordered
addition and one original equality restriction, with a parallel
passive Presentation MathML branch and one Math-owned exact
canonical-text annotation. T2 must independently qualify the
adversarial XML / namespace / depth / semantic loss boundary;
it must not expand the grammar or reconstruct meaning from
unannotated typography.

## Design

MathMlBoundaryAudit.Audit(string) is a separate public .NET 10
Math-only result contract layered over the existing
ExactMathMlInterop.ImportAnnotated reader. It does NOT implement
another XML parser, execute DTDs or TeX, interpret arbitrary
Content MathML, or invoke a symbolic CAS.

A bounded source has a lowercase SHA-256 of the ORIGINAL UTF-8
input bytes. The digest can be retained on refusal to correlate
which input was inspected, but is NEVER a mathematical proof.
Sources longer than 4096 characters are rejected without
allocating source-hash bytes.

On a successful imported annotated document, the audit:

1. Re-encodes and decodes its original typed Math IR using the
   qualified canonical JSON IR codec and compares exact bytes.
2. Independently renders the decoded IR with the qualified
   canonical text and MathML profiles.
3. Requires exact equality of the canonical text, semantic
   structural IR SHA-256, Content branch, Presentation branch,
   and annotated XML input bytes before certifying fidelity.
4. Retains original IR, all three output representations,
   a versioned audit rule, and source hash, while ALWAYS setting
   PresentationInverseQualified=false. Structural fidelity is
   not algebraic equivalence and not a typography inverse.

On refusal, the exact underlying MathMlInteropStatus maps to a
separate typed MathMlBoundaryLossKind:
PresentationWithoutAnnotation, InvalidXml, UnsupportedProfile,
NonCanonicalSerialization, AlteredSemanticBinding,
ResourceLimit or UnverifiedStructuralIdentity.
No refused result contains Math IR, a structural semantic hash,
an annotated display or a mathematical proof rule.

## Security and loss falsification

The previous T1 reader already prohibits DTD processing,
uses a null XmlResolver, and bounds to 4096 characters,
32 nodes and 12 element levels. This independent T2 gate
observes its actual outcomes with adversarial input: external
DTDs, internal entities, undefined entities, malformed XML,
unrecognized namespaces / encoding labels, missing annotations,
tampered integer Content MathML, tampered presentation, altered
canonical text, unreduced fractions, extra XML lexical material,
over-length input, excessive nodes and excessive nesting.

No network fetch, external-file entity, XML input grammar widening
or generalized MathML equivalence is authorized. In particular,
an unannotated mfrac or ci/mi is NOT an exact source certificate.

## RS071 acceptance

Observe one genuinely compiled missing-public-API RED against
the qualified codecs 1.0.0-preview.5 BEFORE introducing the audit.
Recover the RED and pass 42 independent GREEN MSTests, then all
focused codecs tests and relevant IR/quantities regressions under
locked .NET 10 Release build.

Repack exactly three independently versioned local Math NuGet
products: dw.quantities 0.2.0-preview.1, math.ir
0.3.0-preview.1, and math.codecs 1.0.0-preview.6.
Inspect actual local net10 assemblies, nuspec dependencies,
archive lengths and SHA-256 using a separate C# helper.
Eight NuGet-only consumer probes must run from one pinned
codecs preview.6 PackageReference with an isolated local feed
and no source ProjectReference or external CAS/agent dependency.

Upon durable success complete only T2 R/G/V, T2 and C03.
M5-W01 and M5 remain in-progress; C04 Markdown/Office
integration remains not-ready. No public NuGet, stable 1.0,
AURA/Decision/MCDM adoption or dw.discovery skill is claimed.
