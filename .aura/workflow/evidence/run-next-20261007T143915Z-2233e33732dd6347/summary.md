# Run-next failure evidence

- Invocation: `run-next-20261007T143915Z-2233e33732dd6347`
- Run: `RS002`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS002 failed before attempt allocation. classification=validation:native-dwf-adoption. The source repository remains unchanged and no durable attempt was created.
Standard output:
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-8ab9fae474694cde93e0821299874fe1\repository\docs\planning\ValidateNativeDwfAdoption.cs(197,5): error CS7036: There is no argument given that corresponds to the required parameter 'state' of 'RequireState(IReadOnlyDictionary<string, ActualNode>, string, string)'
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-8ab9fae474694cde93e0821299874fe1\repository\docs\planning\ValidateNativeDwfAdoption.cs(199,13): error CS0128: A local variable or function named 'RequireState' is already defined in this scope
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-8ab9fae474694cde93e0821299874fe1\repository\docs\planning\ValidateNativeDwfAdoption.cs(197,5): error CS8422: A static local function cannot contain a reference to 'this' or 'base'.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-8ab9fae474694cde93e0821299874fe1\repository\docs\planning\ValidateNativeDwfAdoption.cs(199,13): error CS8321: The local function 'RequireState' is declared but never used
Standard error:

The build failed. Fix the build errors and run again.
