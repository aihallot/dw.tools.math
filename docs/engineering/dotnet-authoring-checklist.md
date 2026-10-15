# .NET and DWF authoring checklist

Planning uses executable .NET 10 C# and the BCL. The validator requires no Python, NuGet package, or Unix shell.
SDK 10.0.401 is the observed M0-W01-C01 baseline. `global.json` allows `latestPatch` only within the 10.0.4xx SDK feature band, selecting an installed stable patch at or above 10.0.401 (for example, 10.0.402). Record the actual selected SDK per run; locked package restore remains mandatory.

- Project paths and command-facing project/solution names are lowercase; new namespaces are PascalCase. Preserve inherited dw.quantities namespaces during initial transfer unless an ADR justifies change.
- Compiled output lives under build/artifacts/, packages under publish/. Do not ignore all build/ because source scripts may live there.
- Central package management, lock files, nullable, and analyzers are configured at bootstrap.
- CancellationToken is the last public argument; check cancellation before computation and at bounded points.
- Verify actual runner/framework signatures, especially relational assertions; do not transpose xUnit/NUnit assumptions into MSTest.
- If MSTest 4 is used: TestMethod + DataRow, dedicated collection assertions, and unambiguous bound helpers.
- Avoid ContainsKey/indexer double lookup, repeated constant-array allocation, and exceptions naming a parameter that does not exist.
- Restore/build/test share configuration and artifact root. Do not run --no-build against output that has not been built.
- DWF: use exact generated paths, dotnet run --file, structured arguments, and stagedFiles from the current run's staged tree.
- Before publishing the final DWF request commit, audit every contribution field against the **active executable authoring contract** (not just JSON syntax): summary <= 500 characters, value <= 1000, nextAction <= 500; no CR/LF or blank text; <= 20 unique debtChanges with <= 300 characters each, and <= 20 achievements with unique project nodes and <= 1000 characters each. Count characters programmatically, reserve margin, and check all fields before publication. A pre-attempt refusal is a failed first invocation even if no attempt is allocated.
- Preparation validations are read-only. Projection generation belongs in a mutating payload or deliberate planning-authoring step with all mutations declared.
- Payloads are replayable from declared baseline or exact target and refuse third states rather than masking drift.
- Verify public provider APIs against pinned versions; assume no RID availability that has not been qualified.
