# Run-next failure evidence

- Invocation: `run-next-20261112T143421Z-a18c74be22a299dc`
- Run: `RS185`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS185 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Binding generation failed: E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-02ed506aff0b415291ee93358418a301\repository\tools\dw.tools.math.discovery-bindings\Program.cs(54,26): error CS8602: Dereference of a possibly null reference. [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-02ed506aff0b415291ee93358418a301\repository\tools\dw.tools.math.discovery-bindings\dw.tools.math.discovery-bindings.csproj]
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-02ed506aff0b415291ee93358418a301\repository\tools\dw.tools.math.discovery-bindings\Program.cs(57,19): error CS8602: Dereference of a possibly null reference. [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-02ed506aff0b415291ee93358418a301\repository\tools\dw.tools.math.discovery-bindings\dw.tools.math.discovery-bindings.csproj]
The build failed. Fix the build errors and run again.
Payload source: payload.cs:line 50
