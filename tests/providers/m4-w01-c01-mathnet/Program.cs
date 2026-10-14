using System;
using System.Collections.Generic;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

// Math.NET Symbolics is tested in a separate process and package cache:
// MathNet.Numerics 6-series requirements must never affect the M3 product
// provider MathNet.Numerics 5.0.0 or become a Math runtime reference.
static void Require(bool valid, string message)
{
    if (!valid) throw new InvalidOperationException("MathNet.Symbolics 0.25.0 provider: " + message);
}
static double Value(Expr expression, double x) =>
    expression.Evaluate(new Dictionary<string, FloatingPoint>
        { ["x"] = x }).RealValue;
static void Near(double observed, double expected, string name)
{
    if (!double.IsFinite(observed) || global::System.Math.Abs(observed-expected)>1e-10)
        throw new InvalidOperationException(name + " differs from its hand-computable oracle.");
}

var symbol = Expr.Variable("x");
var polynomial = symbol*symbol - 1;
var derivative = polynomial.Differentiate(symbol);
Near(Value(derivative, 3d), 6d, "derivative at x=3 must be six");
Near(Value(polynomial, -1d), 0d, "candidate real root -1");
Near(Value(polynomial, 1d), 0d, "candidate real root +1");

var rational = (polynomial/(symbol - 1)).RationalSimplify("x");
Near(Value(rational, 2d), 3d, "rational simplification for x=2");
// This does NOT prove the exclusion x != 1 survives the simplification.
// Math's own conditional result contract must preserve the original domain,
// or reject a result that cannot carry that condition.
Require(typeof(Expr).Assembly.GetName().Name == "MathNet.Symbolics",
    "resolved mathnet package assembly identity");

Console.WriteLine("m4-symbolics-mathnet/0.25.0:qualified; derivative=6@3; root-check=-1,1; exclusion=not-qualified");
