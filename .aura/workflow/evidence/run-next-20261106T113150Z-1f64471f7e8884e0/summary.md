# Run-next failure evidence

- Invocation: `run-next-20261106T113150Z-1f64471f7e8884e0`
- Run: `RS157`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS157 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
statistics-candidate exit=1
MATH-STATISTICS-CANDIDATE-REFUSED: f1-inventory-before exit=1
Unhandled exception: An error occurred trying to start process 'E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3cbbd6d9d70a45abaaababa3e366ff09\repository\build\artifacts\bin\dw.tools.math.nuget-preflight\Release\net10.0\dw.tools.math.nuget-preflight.exe' with working directory 'E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3cbbd6d9d70a45abaaababa3e366ff09\repository'. The system cannot find the file specified.
Payload source: payload.cs:line 260
