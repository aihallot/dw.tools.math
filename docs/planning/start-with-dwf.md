# First prompt after DWF initialization

The owner may give this prompt to ChatGPT in the Math repository context after initializing the selected DWF version. This file does not prepare a run and is not a bootstrap command.

> Work only in aihallot/dw.tools.math. Read AGENTS.md, PROJECT-MANTRA.md, PROJECT-CONSTITUTION.md, then docs/planning/implementation-handoff.md and the active DWF guidance. docs/planning/backlog.json is the product plan; docs/planning/dwf-map.json gives ID mapping, not native plan bytes to copy.
>
> Observe Git state, DWF identity, and any pending run before acting. If the native plan is empty with no pending run, follow DWF's initial product-plan procedure while preserving schemaVersion and project.id/title. Invent no success history or activation state.
>
> Prepare the first bounded M0-W01-C01 result: .NET foundation, runner, reproducible scripts, package, and minimal isolated consumer. Refine exact files and the complete restore/build/test/pack/consumer chain; the proposed test command alone is insufficient. Do not begin AURA extraction before provenance and boundary gates.
>
> Validate planning with dotnet run --file docs/planning/ValidatePlan.cs -- --check. If the backlog changes, regenerate projections with -- --write in an appropriate mutating authoring step or payload and declare every modified file. Never put generation in a preparation-safe validation.
>
> No write access to other repositories. For any external need, prepare a request under docs/coordination/requests/ for owner transmission. Do not call a request sent or an adoption completed without external evidence.
>
> Stop the first slice at the verified foundation. Report evidence, limits, and the next chunk. The operator keeps the normal loop for the installed DWF version, generally dwf run next; do not transfer internal workflow repairs to the operator.

Historical note: RS001 and RS002 have since completed the initial foundation and native DWF adoption. This prompt is retained as the original bootstrap instruction, not as current continuation state.
