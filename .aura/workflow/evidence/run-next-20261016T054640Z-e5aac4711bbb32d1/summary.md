# Run-next failure evidence

- Invocation: `run-next-20261016T054640Z-e5aac4711bbb32d1`
- Run: `RS068`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS068 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet build dw.tools.math.slnx -c Release --no-restore exited 1: error CS1503: Argument 1: cannot convert from 'System.Numerics.BigInteger' to 'dw.quantities.ExactRational' [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-0fa3ac183e8c4e28ac31f755bcf27fc2\repository\src\projects\dw.tools.math.codecs\dw.tools.math.codecs.csproj]
dw.tools.math.composition -> E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-0fa3ac183e8c4e28ac31f755bcf27fc2\repository\build\artifacts\bin\dw.tools.math.composition\Release\net10.0\dw.tools.math.composition.dll
dw.tools.math.numerics -> E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-0fa3ac183e8c4e28ac31f755bcf27fc2\repository\build\artifacts\bin\dw.tools.math.numerics\Release\net10.0\dw.tools.math.numerics.dll
dw.tools.math.symbolics -> E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-0fa3ac183e8c4e28ac31f755bcf27fc2\repository\build\artifacts\bin\dw.tools.math.symbolics\Release\net10.0\dw.tools.math.symbolics.dll
Payload source: payload.cs:line 426
