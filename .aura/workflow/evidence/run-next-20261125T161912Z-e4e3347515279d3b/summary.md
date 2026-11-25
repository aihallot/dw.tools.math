# Run-next failure evidence

- Invocation: `run-next-20261125T161912Z-e4e3347515279d3b`
- Run: `RS340`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS340 failed before attempt allocation. classification=validation:m8-rs340-new-net11-two-package-locked-consumer. The source repository remains unchanged and no durable attempt was created.
Process status: timedOut=false cancelled=false exitCode=-532462766 workflowExitCode=-532462766 terminationAttempted=false terminationErrorPresent=false timeoutMilliseconds=1000000 durationMilliseconds=8338
Standard output:
[dwf-validation] strategy=declared-authority stdoutPresent=false stderrPresent=true nugetDiagnosticObserved=false compilerOrMsbuildDiagnosticObserved=true Process status: timedOut=false cancelled=false exitCode=-532462766 workflowExitCode=-532462766 terminationAttempted=false terminationErrorPresent=false timeoutMilliseconds=1000000 durationMilliseconds=8338
Standard error:
Unhandled exception. System.IO.InvalidDataException: RS340: dotnet pack E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-28855fd7715245909ba9c240b3652cf3\repository\build\artifacts\f340-317200c3\producer\dw.quantities\dw.quantities.csproj -c Release --no-restore -o E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-28855fd7715245909ba9c240b3652cf3\repository\build\artifacts\f340-317200c3\feed failed: E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-28855fd7715245909ba9c240b3652cf3\repository\build\artifacts\f340-317200c3\out\obj\dw.quantities\Release\net11.0\dw.quantities.AssemblyInfo.cs(17,59): error CS7035: The specified version string '20260.20261125.1001.0' does not conform to the recommended format - major.minor.build.revision [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-28855fd7715245909ba9c240b3652cf3\repository\build\artifacts\f340-317200c3\producer\dw.quantities\dw.quantities.csproj]
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-28855fd7715245909ba9c240b3652cf3\repository\build\artifacts\f340-317200c3\out\obj\dw.quantities\Release\net11.0\dw.quantities.AssemblyInfo.cs(21,55): error CS7034: The specified version string '20260.20261125.1001.0' does not conform to the required format - major[.minor[.build[.revision]]] [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-28855fd7715245909ba9c240b3652cf3\repository\build\artifacts\f340-317200c3\producer\dw.quantities\dw.quantities.csproj]
   at Program.<<Main>$>g__Need|0_10(Boolean ok, String reason) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-28855fd7715245909ba9c240b3652cf3\repository\docs\engineering\ProbeM8W02Net11StatisticsCandidate.cs:line 196
   at Program.<<Main>$>g__Pass|0_11(String cwd, String[] command) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-28855fd7715245909ba9c240b3652cf3\repository\docs\engineering\ProbeM8W02Net11StatisticsCandidate.cs:line 200
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-28855fd7715245909ba9c240b3652cf3\repository\docs\engineering\ProbeM8W02Net11StatisticsCandidate.cs:line 100
