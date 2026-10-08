using System.Reflection;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C02RedTests
{
    [TestMethod]
    public void StandaloneExactGrammarAssemblyMustExistBeforeQualification()
    {
        Assembly? grammar;
        try
        {
            grammar = Assembly.Load("dw.quantities.expression");
        }
        catch (FileNotFoundException)
        {
            grammar = null;
        }
        Assert.IsNotNull(grammar,
            "M1-W02-C02-T1 RED: standalone dw.quantities.expression assembly is absent from Math.");
    }

    [TestMethod]
    public void StandaloneParserPublicSurfaceMustExistBeforeQualification()
    {
        Assembly? grammar;
        try
        {
            grammar = Assembly.Load("dw.quantities.expression");
        }
        catch (FileNotFoundException)
        {
            grammar = null;
        }

        var parserTypes = grammar?.GetExportedTypes()
            .Where(type => type.Name.Contains("Expression", StringComparison.Ordinal)
                || type.Name.Contains("Parser", StringComparison.Ordinal))
            .ToArray();
        Assert.IsTrue(parserTypes is { Length: > 0 },
            "M1-W02-C02-T1 RED: no exported parser or expression contract is materialized.");
    }
}
