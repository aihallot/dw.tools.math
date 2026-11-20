# Run-next failure evidence

- Invocation: `run-next-20261120T190124Z-e64d80afa6c793cd`
- Run: `RS269`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS269 failed before attempt allocation. classification=validation:m9-unpublished-statistics-candidate-exact-package. The source repository remains unchanged and no durable attempt was created.
Standard output:
<empty>
Standard error:
Unhandled exception. System.InvalidOperationException: RS269 dotnet pack candidate.csproj -c Release --no-restore --output E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-61a5dd4847db45eb9465d723c8273003\repository\build\artifacts\m9-rs269-bounded-statistics-candidate\candidate failed:
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-61a5dd4847db45eb9465d723c8273003\repository\build\artifacts\m9-rs269-bounded-statistics-candidate\out\obj\Release\net10.0\candidate.AssemblyInfo.cs(15,59): error CS7035: The specified version string '20260.20261120.1001.0' does not conform to the recommended format - major.minor.build.revision [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-61a5dd4847db45eb9465d723c8273003\repository\build\artifacts\m9-rs269-bounded-statistics-candidate\candidate.csproj]
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-61a5dd4847db45eb9465d723c8273003\repository\build\artifacts\m9-rs269-bounded-statistics-candidate\out\obj\Release\net10.0\candidate.AssemblyInfo.cs(19,55): error CS7034: The specified version string '20260.20261120.1001.0' does not conform to the required format - major[.minor[.build[.revision]]] [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-61a5dd4847db45eb9465d723c8273003\repository\build\artifacts\m9-rs269-bounded-statistics-candidate\candidate.csproj]


   at Program.<<Main>$>g__Run|0_2(String[] arguments, <>c__DisplayClass0_0&) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-61a5dd4847db45eb9465d723c8273003\repository\docs\engineering\ProbeM9StatisticsBoundedNugetCandidate.cs:line 95
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-61a5dd4847db45eb9465d723c8273003\repository\docs\engineering\ProbeM9StatisticsBoundedNugetCandidate.cs:line 103
