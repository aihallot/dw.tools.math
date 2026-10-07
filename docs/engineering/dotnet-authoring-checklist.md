# .NET and DWF authoring checklist

Planning uses executable .NET 10 C# and the BCL. The validator requires no Python, NuGet package, or Unix shell.
SDK 10.0.401 was observed locally and is now pinned by the delivered M0-W01-C01 foundation.

- Project paths and command-facing project/solution names are lowercase; new namespaces are PascalCase. Preserve inherited dw.quantities namespaces during initial transfer unless an ADR justifies change.
- Compiled output lives under build/artifacts/, packages under publish/. Do not ignore all build/ because source scripts may live there.
- Central package management, lock files, nullable, and analyzers are configured at bootstrap.
- CancellationToken is the last public argument; check cancellation before computation and at bounded points.
- Verify actual runner/framework signatures, especially relational assertions; do not transpose xUnit/NUnit assumptions into MSTest.
- If MSTest 4 is used: TestMethod + DataRow, dedicated collection assertions, and unambiguous bound helpers.
- Avoid ContainsKey/indexer double lookup, repeated constant-array allocation, and exceptions naming a parameter that does not exist.
- Restore/build/test share configuration and artifact root. Do not run --no-build against output that has not been built.
- DWF: use exact generated paths, dotnet run --file, structured arguments, and stagedFiles from the current run's staged tree.
- Preparation validations are read-only. Projection generation belongs in a mutating payload or deliberate planning-authoring step with all mutations declared.
- Payloads are replayable from declared baseline or exact target and refuse third states rather than masking drift.
- Verify public provider APIs against pinned versions; assume no RID availability that has not been qualified.
