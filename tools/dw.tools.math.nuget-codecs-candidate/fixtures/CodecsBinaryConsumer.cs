using System.Security.Cryptography;
using Dw.Tools.Math.Codecs;

if (args.Length != 1 || args[0].Length != 64)
    throw new InvalidOperationException("An exact packaged Codecs DLL SHA-256 is required.");
var passed = 0;
void Probe(string id, Action check)
{
    try
    {
        check();
        passed++;
        Console.WriteLine("MATH-CODECS-CHECK|" + id + "|PASS");
    }
    catch (Exception e)
    {
        throw new InvalidOperationException("Codecs binary consumer failed " + id, e);
    }
}
static void Need(bool valid, string message)
{
    if (!valid) throw new InvalidOperationException(message);
}
var scalar = CanonicalMathNotation.Parse("1");
Probe("loaded-codecs-dll", () =>
{
    var assembly = typeof(CanonicalMathNotation).Assembly;
    Need(assembly.GetName().Name == "dw.tools.math.codecs", "Wrong Codecs assembly.");
    Need(Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(assembly.Location))) == args[0],
        "Actually loaded Codecs DLL has wrong package hash.");
});
Probe("canonical-exact-scalar", () =>
    Need(scalar.IsAdmitted && scalar.CanonicalText == "1" &&
         scalar.Expression is not null &&
         scalar.SemanticStructuralSha256?.Length == 64,
         "Canonical exact scalar roundtrip missing."));
Probe("canonical-exact-add", () =>
{
    var parsed = CanonicalMathNotation.Parse("add(1,2)");
    Need(parsed.IsAdmitted && parsed.CanonicalText == "add(1,2)" &&
         parsed.Expression is not null, "Canonical addition was not admitted.");
});
Probe("canonical-empty-refusal", () =>
    Need(CanonicalMathNotation.Parse("").Status == CanonicalNotationStatus.InvalidInput,
         "Empty canonical syntax must refuse."));
Probe("canonical-whitespace-refusal", () =>
    Need(CanonicalMathNotation.Parse(" 1").Status == CanonicalNotationStatus.InvalidInput,
         "Whitespace in canonical ASCII notation must refuse."));
Probe("canonical-budget-refusal", () =>
    Need(CanonicalMathNotation.Parse(new string('1',
         CanonicalMathNotation.MaximumInputCharacters + 1)).Status ==
         CanonicalNotationStatus.BudgetExceeded, "Unbounded canonical input must refuse."));
Probe("latex-empty-refusal", () =>
    Need(BoundedLatexImporter.Import("").Status == LatexImportStatus.Invalid,
         "Empty LaTeX import must refuse."));
Probe("latex-budget-refusal", () =>
    Need(BoundedLatexImporter.Import(new string('x',
         BoundedLatexImporter.MaximumInputCharacters + 1)).Status ==
         LatexImportStatus.BudgetExceeded, "Oversized LaTeX import must refuse."));
Probe("latex-audit-resource-bound", () =>
    Need(LatexImportBoundaryAudit.Audit(new string('x',
         LatexImportBoundaryAudit.MaximumInputCharacters + 1)).LossKind ==
         LatexImportLossKind.ResourceLimit, "Unbounded LaTeX audit must refuse."));
Probe("mathml-invalid-input", () =>
    Need(ExactMathMlInterop.ImportAnnotated("").Status == MathMlInteropStatus.InvalidXml,
         "Empty MathML must refuse."));
Probe("mathml-budget-refusal", () =>
    Need(ExactMathMlInterop.ImportAnnotated(new string('x',
         ExactMathMlInterop.MaximumXmlCharacters + 1)).Status ==
         MathMlInteropStatus.BudgetExceeded, "Oversized MathML must refuse."));
Probe("office-invalid-input", () =>
{
    var expression = scalar.Expression ??
        throw new InvalidOperationException("Admitted scalar IR absent.");
    Need(ExactOfficeOmmlCodec.ImportCanonical("", expression).Status ==
         OfficeOmmlStatus.InvalidXml, "Empty Office OMML must refuse.");
});
Probe("unicode-invalid-placement", () =>
{
    var expression = scalar.Expression ??
        throw new InvalidOperationException("Admitted scalar IR absent.");
    Need(MarkdownUnicodeMathCodec.Render(expression, (MarkdownMathPlacement)77).Status ==
         MarkdownUnicodeMathStatus.Unsupported, "Invalid Markdown placement must refuse.");
});
Probe("display-audit-invalid-canonical", () =>
    Need(CanonicalDisplayBoundaryAudit.AuditText("").Status ==
         CanonicalDisplayBoundaryStatus.RefusedInput, "Invalid canonical display must refuse."));
Probe("mathml-boundary-budget", () =>
    Need(MathMlBoundaryAudit.Audit(new string('x',
         MathMlBoundaryAudit.MaximumInputCharacters + 1)).LossKind ==
         MathMlBoundaryLossKind.ResourceLimit, "MathML source budget must be enforced."));
Need(passed == 15, "Exactly fifteen independent Codecs compiled checks required.");
Console.WriteLine("MATH-CODECS-BINARY|PASS|15");
