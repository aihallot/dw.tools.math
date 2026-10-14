using System;
using AngouriMath;

// Provider-only smoke; AngouriMath stays outside Math's product projects.
// Scientific domain exclusions are independently required, not inferred from
// a visually shorter result or an external provider description.
static void Require(bool value, string message)
{
    if (!value) throw new InvalidOperationException("AngouriMath 2.5.0 provider: " + message);
}

var polynomial = MathS.FromString("x^2 - 1");
var derivative = polynomial.Differentiate("x").Simplify();
var difference = (derivative - MathS.FromString("2*x")).Simplify();
Require(difference.ToString().Trim() == "0", "polynomial derivative must be exactly 2*x.");

var roots = polynomial.SolveEquation("x");
var rootText = roots.ToString();
Require(rootText.Contains("-1", StringComparison.Ordinal) &&
        rootText.Contains("1", StringComparison.Ordinal),
    "x^2-1 must exhibit both real roots -1 and 1; no universal root solver claim.");

var cancellable = MathS.FromString("(x^2 - 1)/(x - 1)");
var reduced = cancellable.Simplify();
var rendering = reduced.ToString();
Require(rendering.Contains("provided", StringComparison.OrdinalIgnoreCase),
    "Cancellation must carry an explicit condition instead of silently filling x=1.");
Require(rendering.Contains("x", StringComparison.OrdinalIgnoreCase) &&
        rendering.Contains("1", StringComparison.Ordinal),
    "Conditional result must retain the nonzero-denominator symbol and boundary.");

// The smoke observes this one provider/version, not a certified Math IR bridge.
// The x=1 exclusion is explicit even if the reduced algebraic shape is x+1.
Console.WriteLine("m4-symbolics-angouri/2.5.0:qualified; derivative=2*x; roots=-1,1; domain=conditional");
