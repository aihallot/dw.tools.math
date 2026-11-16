# Search: `checked(`

Paths: `src/projects`

```text
src/projects/dw.quantities.expression/ExpressionParser.cs:504:             checked(value.Length * factor), checked(value.Mass * factor), checked(value.Time * factor),
src/projects/dw.quantities.expression/ExpressionParser.cs:505:             checked(value.ElectricCurrent * factor), checked(value.Temperature * factor),
src/projects/dw.quantities.expression/ExpressionParser.cs:506:             checked(value.AmountOfSubstance * factor), checked(value.LuminousIntensity * factor),
src/projects/dw.quantities.expression/ExpressionParser.cs:507:             checked(value.Information * factor));
src/projects/dw.quantities/DimensionVector.cs:27:             checked(left.Length + right.Length),
src/projects/dw.quantities/DimensionVector.cs:28:             checked(left.Mass + right.Mass),
src/projects/dw.quantities/DimensionVector.cs:29:             checked(left.Time + right.Time),
src/projects/dw.quantities/DimensionVector.cs:30:             checked(left.ElectricCurrent + right.ElectricCurrent),
src/projects/dw.quantities/DimensionVector.cs:31:             checked(left.Temperature + right.Temperature),
src/projects/dw.quantities/DimensionVector.cs:32:             checked(left.AmountOfSubstance + right.AmountOfSubstance),
src/projects/dw.quantities/DimensionVector.cs:33:             checked(left.LuminousIntensity + right.LuminousIntensity),
src/projects/dw.quantities/DimensionVector.cs:34:             checked(left.Information + right.Information));
src/projects/dw.quantities/DimensionVector.cs:38:             checked(left.Length - right.Length),
src/projects/dw.quantities/DimensionVector.cs:39:             checked(left.Mass - right.Mass),
src/projects/dw.quantities/DimensionVector.cs:40:             checked(left.Time - right.Time),
src/projects/dw.quantities/DimensionVector.cs:41:             checked(left.ElectricCurrent - right.ElectricCurrent),
src/projects/dw.quantities/DimensionVector.cs:42:             checked(left.Temperature - right.Temperature),
src/projects/dw.quantities/DimensionVector.cs:43:             checked(left.AmountOfSubstance - right.AmountOfSubstance),
src/projects/dw.quantities/DimensionVector.cs:44:             checked(left.LuminousIntensity - right.LuminousIntensity),
src/projects/dw.quantities/DimensionVector.cs:45:             checked(left.Information - right.Information));
src/projects/dw.tools.math.distribution/MathPackageRuntimeSurfaceAudit.cs:93:             var dllBytes=new byte[checked((int)dll.Length)];
src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs:67:             ["bits"] = unchecked((ulong)approximate.Ieee754Bits).ToString("x16", CultureInfo.InvariantCulture)
src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs:161:             BitConverter.Int64BitsToDouble(unchecked((long)unsigned)));
src/projects/dw.tools.math.ir/IrMatrix.cs:28:         var size = checked(rows * columns);
src/projects/dw.tools.math.ir/IrMatrix.cs:46:         return Elements[checked(row * Columns + column)];
src/projects/dw.tools.math.numerics/NumericalMatrices.cs:31:         var count = checked(rows * columns);
src/projects/dw.tools.math.numerics/NumericalMatrices.cs:46:         var copy = new double[checked(rows * columns)];
src/projects/dw.tools.math.numerics/NumericalMatrices.cs:68:         return _values[checked(row * Columns + column)];
src/projects/dw.tools.math.numerics/NumericalMatrices.cs:91:         var result = new double[checked(Rows * other.Columns)];
src/projects/dw.tools.math.numerics/NumericalMatrices.cs:216:         var values = new double[checked(Rows * Columns)];
src/projects/dw.tools.math.numerics/NumericalMatrices.cs:233:         var nodes = new List<IrScalar>(checked(matrix.Rows * matrix.Columns));
src/projects/dw.tools.math.statistics/SplitMix64Random.cs:33:         _state = unchecked(seed + Increment * counter);
src/projects/dw.tools.math.statistics/SplitMix64Random.cs:56:         var z = _state = unchecked(_state + Increment);
src/projects/dw.tools.math.statistics/SplitMix64Random.cs:57:         z = unchecked((z ^ (z >> 30)) * Mix1);
src/projects/dw.tools.math.statistics/SplitMix64Random.cs:58:         z = unchecked((z ^ (z >> 27)) * Mix2);
```
