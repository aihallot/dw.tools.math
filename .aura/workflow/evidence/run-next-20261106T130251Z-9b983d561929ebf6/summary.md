# Run-next failure evidence

- Invocation: `run-next-20261106T130251Z-9b983d561929ebf6`
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
MATH-STATISTICS-CANDIDATE-REFUSED: consumer-build exit=1
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-4c0802988f52473aa3fb9002c2902760\repository\build\artifacts\rs157-statistics-candidate\consumer\Program.cs(56,10): error CS8629: Nullable value type may be null. [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-4c0802988f52473aa3fb9002c2902760\repository\build\artifacts\rs157-statistics-candidate\consumer\statistics-binary-consumer.csproj]
Build FAILED.
Payload source: payload.cs:line 260
