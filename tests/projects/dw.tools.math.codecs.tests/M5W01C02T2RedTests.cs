using System.Reflection;
using Dw.Tools.Math.Codecs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C02T2RedTests
{
    [TestMethod]
    public void IndependentLatexLossAndProvenanceAuditApiMustExist()
    {
        var assembly=typeof(BoundedLatexImporter).Assembly;
        var type=assembly.GetType(
            "Dw.Tools.Math.Codecs.LatexImportBoundaryAudit",false);
        var audit=type?.GetMethod("Audit",BindingFlags.Public|BindingFlags.Static,
            binder:null,
            types:[typeof(string),typeof(LatexJuxtapositionConvention)],
            modifiers:null);
        Assert.IsNotNull(audit,
            "M5-W01-C02-T2 RED: independent bounded LaTeX semantic-loss and provenance audit API is absent.");
    }
}
