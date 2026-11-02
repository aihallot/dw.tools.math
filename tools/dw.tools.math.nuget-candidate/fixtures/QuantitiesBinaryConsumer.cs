using System.Security.Cryptography;
using dw.quantities;

if (args.Length != 1 ||
    args[0].Length != 64 ||
    args[0].Any(ch => ch is not (>= '0' and <= '9' or >= 'a' and <= 'f')))
    throw new ArgumentException("One exact packaged DLL SHA-256 is required.");

static void Check(bool condition, string explanation)
{
    if (!condition)
        throw new InvalidOperationException("Math package-only consumer: " + explanation);
}

var library = typeof(ExactRational).Assembly;
Check(library.GetName().Name == "dw.quantities",
    "unexpected loaded assembly name");
Console.WriteLine("MATH-QUANTITIES-CHECK|assembly|PASS");

var actualDllSha = Convert.ToHexStringLower(
    SHA256.HashData(File.ReadAllBytes(library.Location)));
Check(actualDllSha == args[0], "the actual loaded DLL differs from the packed DLL");
Console.WriteLine("MATH-QUANTITIES-CHECK|loaded-dll|PASS");

var exact = new ExactRational(1, 3) + new ExactRational(1, 6);
Check(exact == new ExactRational(1, 2) &&
      exact.ToFractionString() == "1/2", "1/3 + 1/6 must equal exactly 1/2");
Console.WriteLine("MATH-QUANTITIES-CHECK|exact-rational|PASS");

try
{
    _ = new ExactRational(1, 0);
    throw new InvalidOperationException("zero denominator was not rejected");
}
catch (DivideByZeroException)
{
    Console.WriteLine("MATH-QUANTITIES-CHECK|zero-denominator-refusal|PASS");
}

Console.WriteLine("MATH-QUANTITIES-BINARY|PASS|4");
