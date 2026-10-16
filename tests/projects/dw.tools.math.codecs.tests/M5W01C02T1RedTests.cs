using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C02T1RedTests
{
    [TestMethod]
    public void CompiledBoundedLatexImportApiMustBePresent()
    {
        var assembly=Assembly.Load("dw.tools.math.codecs");
        var type=assembly.GetType("Dw.Tools.Math.Codecs.BoundedLatexImporter",
            throwOnError:false);
        var convention=assembly.GetType(
            "Dw.Tools.Math.Codecs.LatexJuxtapositionConvention",throwOnError:false);
        var import=convention is null?null:type?.GetMethod("Import",
            BindingFlags.Public|BindingFlags.Static,
            binder:null,types:[typeof(string),convention],modifiers:null);
        Assert.IsTrue(import is not null,
            "M5-W01-C02-T1 RED: bounded LaTeX importer and explicit ambiguity convention public API are absent.");
    }
}
