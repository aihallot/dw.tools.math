# Run-next failure evidence

- Invocation: `run-next-20261009T090550Z-c75f314af1450d9d`
- Run: `RS029`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS029 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet test tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj -c Release --no-build --no-restore --filter FullyQualifiedName~M2W01C02BoundaryRedTests.PublicFactoriesMustBoundExpandedNodeBudget --logger console;verbosity=normal exited 1: Error Message:
Assertion failed. Expected exception of exact type ArgumentOutOfRangeException but no exception was thrown.
M2-W01-C02-T2 RED: shared DAG exceeds the 1024-node expanded budget.
Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => IrApply.Create(IrOperation.Add, [node, node]))
Payload source: payload.cs:line 278
