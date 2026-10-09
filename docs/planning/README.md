# Implementation plan — dw.tools.math

**Objective:** extract the AURA mathematical core, then build an independent, typed, qualified platform.

Product plan 0.1.26. States come from the [canonical backlog](backlog.json). No documentation status proves a product capability.

Hierarchy: **release -> work package -> chunk -> task -> subtask**. DWF maps these levels to milestone/workPackage/phase/task/subtask.

| Milestone | Target version | Outcome | Status |
|---|---|---|---|
| [M0](versions/M0.md) | 0.1.0-preview | Establish a reproducible .NET toolchain and an authorized transfer path without modifying AURA. | done |
| [M1](versions/M1.md) | 0.2.0-preview | Deliver standalone exact-math packages and their AURA adoption dossier. | done |
| [M2](versions/M2.md) | 0.3.0-preview | Define and verify a minimal IR compatible with quantities and future providers. | in_progress |
| [M3](versions/M3.md) | 0.4.0-preview | Deliver the first numerical families behind clean provider-neutral contracts. | planned |
| [M4](versions/M4.md) | 0.5.0-preview | Preserve domains and assumptions across bounded symbolic operations. | planned |
| [M5](versions/M5.md) | 1.0.0 | Qualify the first coherent numerical/symbolic platform usable without AURA. | planned |
| [M6](versions/M6.md) | 1.1.0 | Extend useful families through qualified contracts and providers without absorbing decision domains. | planned |
| [M7](versions/M7.md) | 1.2.0-candidate | Qualify advanced needs and deliver only those admitted by ADR; make deferrals explicit. | planned |

M1 delivers the exact core early; 1.0 follows the M5 numerical/symbolic vertical. M7 spikes may conclude with deferral: their decisions are not delivered functions. Target versions are indicative, not dates or authorization to execute the whole program.

- [Architecture](architecture.md) and [inventory](existing-code-inventory.md)
- [Coverage](coverage.md), [sources](sources.md), and [qualification](testing-and-quality.md)
- [Coordination](cross-repository-coordination.md) and [DWF handoff](implementation-handoff.md)
- [Resource policy](resource-cost-policy.md)

Scope: 8 releases, 17 work packages, 53 chunks, 106 tasks, 318 subtasks, 106 requirements.

Verify: dotnet run --file docs/planning/ValidatePlan.cs -- --check. Regenerate after editing with -- --write, exclusively outside preparation-safe validation. Self-tests: -- --self-test.

Functional dependencies, no agent fan-out. Refine files/commands before ready; split L blocks; do not create empty product packages in advance.
