# Run-next failure evidence

- Invocation: `run-next-20261009T165404Z-21c9ce8dad3c1686`
- Run: `RS035`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS035 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet test tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj -c Release --no-build --no-restore --logger console;verbosity=minimal exited 1: Error Message:
Test method Dw.Tools.Math.Composition.Tests.M2W02C02RedTests.ExactQuantityPipelineMustExposeOneSharedDirectAndFluentEvaluator threw exception:
System.Reflection.AmbiguousMatchException: Ambiguous match found for 'Dw.Tools.Math.Composition.ExactQuantityPipeline Dw.Tools.Math.Composition.ExactPipelineResult Evaluate(System.Collections.Generic.IEnumerable`1[Dw.Tools.Math.Composition.ExactPipelineStep])'.
Stack Trace:
Payload source: payload.cs:line 232
