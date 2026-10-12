# Run-next failure evidence

- Invocation: `run-next-20261012T221425Z-41a6411a4b3052b8`
- Run: `RS042`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS042 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet build dw.tools.math.slnx -c Release --no-restore exited 1: error CS0234: The type or namespace name 'Abs' does not exist in the namespace 'Dw.Tools.Math' (are you missing an assembly reference?) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-a430ef1194e34564b090e93c7ecb6d57\repository\src\projects\dw.tools.math.numerics\dw.tools.math.numerics.csproj]
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-a430ef1194e34564b090e93c7ecb6d57\repository\src\projects\dw.tools.math.numerics\BoundedLinearAlgebra64.cs(92,33): error CS0234: The type or namespace name 'Abs' does not exist in the namespace 'Dw.Tools.Math' (are you missing an assembly reference?) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-a430ef1194e34564b090e93c7ecb6d57\repository\src\projects\dw.tools.math.numerics\dw.tools.math.numerics.csproj]
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-a430ef1194e34564b090e93c7ecb6d57\repository\src\projects\dw.tools.math.numerics\BoundedLinearAlgebra64.cs(112,28): error CS0234: The type or namespace name 'Min' does not exist in the namespace 'Dw.Tools.Math' (are you missing an assembly reference?) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-a430ef1194e34564b090e93c7ecb6d57\repository\src\projects\dw.tools.math.numerics\dw.tools.math.numerics.csproj]
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-a430ef1194e34564b090e93c7ecb6d57\repository\src\projects\dw.tools.math.numerics\BoundedLinearAlgebra64.cs(112,51): error CS0234: The type or namespace name 'Abs' does not exist in the namespace 'Dw.Tools.Math' (are you missing an assembly reference?) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-a430ef1194e34564b090e93c7ecb6d57\repository\src\projects\dw.tools.math.numerics\dw.tools.math.numerics.csproj]
Payload source: payload.cs:line 334
