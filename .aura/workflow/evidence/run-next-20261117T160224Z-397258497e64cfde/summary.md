# Run-next failure evidence

- Invocation: `run-next-20261117T160224Z-397258497e64cfde`
- Run: `RS242`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS242 failed before attempt allocation. classification=validation:m8-wsl-linux-net11-temperature-source-consumer. The source repository remains unchanged and no durable attempt was created.
Standard output:
<empty>
Standard error:
Unhandled exception. System.InvalidOperationException: RS242 WSL failed: --exec bash -lc cd "$1" && shift && dotnet "$@" bash /mnt/e/data.temp/fhallot.temp/dw-tools-workflow-disposable-run-86365a143dc24717b646a05fbe59d91a/repository/build/artifacts/m8-rs242-wsl-temperature-source build consumer.csproj -c Release --no-restore
/mnt/e/data.temp/fhallot.temp/dw-tools-workflow-disposable-run-86365a143dc24717b646a05fbe59d91a/repository/build/artifacts/m8-rs242-wsl-temperature-source/Program.cs(19,67): error CS7036: There is no argument given that corresponds to the required parameter 'denominator' of 'ExactRational.ExactRational(BigInteger, BigInteger)' [/mnt/e/data.temp/fhallot.temp/dw-tools-workflow-disposable-run-86365a143dc24717b646a05fbe59d91a/repository/build/artifacts/m8-rs242-wsl-temperature-source/consumer.csproj]

Build FAILED.

/mnt/e/data.temp/fhallot.temp/dw-tools-workflow-disposable-run-86365a143dc24717b646a05fbe59d91a/repository/build/artifacts/m8-rs242-wsl-temperature-source/Program.cs(19,67): error CS7036: There is no argument given that corresponds to the required parameter 'denominator' of 'ExactRational.ExactRational(BigInteger, BigInteger)' [/mnt/e/data.temp/fhallot.temp/dw-tools-workflow-disposable-run-86365a143dc24717b646a05fbe59d91a/repository/build/artifacts/m8-rs242-wsl-temperature-source/consumer.csproj]
    0 Warning(s)
    1 Error(s)

Time Elapsed 00:00:03.27


   at Program.<<Main>$>g__Wsl|0_2(String[] arguments) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-86365a143dc24717b646a05fbe59d91a\repository\docs\engineering\ProbeM8WslNet11TemperatureConsumer.cs:line 87
   at Program.<<Main>$>g__RunLinux|0_3(String[] arguments, <>c__DisplayClass0_0&) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-86365a143dc24717b646a05fbe59d91a\repository\docs\engineering\ProbeM8WslNet11TemperatureConsumer.cs:line 101
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-86365a143dc24717b646a05fbe59d91a\repository\docs\engineering\ProbeM8WslNet11TemperatureConsumer.cs:line 105
