# Run-next failure evidence

- Invocation: `run-next-20261124T144107Z-11fd4faae41beb0b`
- Run: `RS318`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS318 failed before attempt allocation. classification=validation:m8-w02-independent-copied-sdk-delta-verification. The source repository remains unchanged and no durable attempt was created.
Standard output:
<empty>
Standard error:
Unhandled exception. System.IO.InvalidDataException: RS318: Copied original verifier rejected the pristine source graph: 
Unhandled exception: An error occurred trying to start process 'E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-40bb019308e3493d97feff14e106daa6\repository\build\artifacts\m8-rs318-independent-delta\e946929174d04ab0b3ccd02b6554dc0f\build\artifacts\bin\ValidateM8W02OfficialDeltaMatrix.cs\debug\ValidateM8W02OfficialDeltaMatrix.exe' with working directory 'E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-40bb019308e3493d97feff14e106daa6\repository\build\artifacts\m8-rs318-independent-delta\e946929174d04ab0b3ccd02b6554dc0f'. The filename or extension is too long.

   at Program.<<Main>$>g__Need|0_10(Boolean valid, String reason) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-40bb019308e3493d97feff14e106daa6\repository\docs\engineering\ProbeM8W02CopiedOfficialDeltaConsumer.cs:line 154
   at Program.<<Main>$>g__Clean|0_2(<>c__DisplayClass0_0&) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-40bb019308e3493d97feff14e106daa6\repository\docs\engineering\ProbeM8W02CopiedOfficialDeltaConsumer.cs:line 60
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-40bb019308e3493d97feff14e106daa6\repository\docs\engineering\ProbeM8W02CopiedOfficialDeltaConsumer.cs:line 69
