# Run-next failure evidence

- Invocation: `run-next-20261110T123420Z-a2e3d3ca6aeeec08`
- Run: `RS174`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS174 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
independent-package-candidate exit=1
MATH-ELECTROCHEM-CANDIDATE-REFUSED: consumer-release-build exit=1
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-6cd1a35420904fc8b3d8f583d6d921f8\repository\build\artifacts\rs174-electrochemical-candidate\c24af06b56104c2185051101c9ee70c5\consumer\Program.cs(123,16): error CS0165: Use of unassigned local variable 'm' [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-6cd1a35420904fc8b3d8f583d6d921f8\repository\build\artifacts\rs174-electrochemical-candidate\c24af06b56104c2185051101c9ee70c5\consumer\electrochemical-consumer.csproj]
Build FAILED.
Payload source: payload.cs:line 209
