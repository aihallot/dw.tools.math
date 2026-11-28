# Implementation plan — dw.tools.math

**Objective:** extract the AURA mathematical core, then build an independent, typed, qualified platform.

Product plan 0.1.198. States come from the [canonical backlog](backlog.json). No documentation status proves a product capability.

Hierarchy: **release -> work package -> chunk -> task -> subtask**. DWF maps these levels to milestone/workPackage/phase/task/subtask.

| Milestone | Target version | Outcome | Status |
|---|---|---|---|
| [M0](versions/M0.md) | 0.1.0-preview | Establish a reproducible .NET toolchain and an authorized transfer path without modifying AURA. | done |
| [M1](versions/M1.md) | 0.2.0-preview | Deliver standalone exact-math packages and their AURA adoption dossier. | done |
| [M2](versions/M2.md) | 0.3.0-preview | Define and verify a minimal IR compatible with quantities and future providers. | done |
| [M3](versions/M3.md) | 0.4.0-preview | Deliver the first numerical families behind clean provider-neutral contracts. | done |
| [M4](versions/M4.md) | 0.5.0-preview | Preserve domains and assumptions across bounded symbolic operations. | done |
| [M5](versions/M5.md) | 1.0.0 | Qualify the first coherent numerical/symbolic platform usable without AURA. | done |
| [M6](versions/M6.md) | 1.1.0 | Extend useful families through qualified contracts and providers without absorbing decision domains. | done |
| [M7](versions/M7.md) | 1.2.0-candidate | Qualify advanced needs and deliver only those admitted by ADR; make deferrals explicit. | done |
| [M8](versions/M8.md) | 1.3.0-preview | Qualify opt-in Math Discovery and independently assess safe .NET 11 / C# 15 modernization without consumer ownership transfer. | done |
| [M9](versions/M9.md) | 1.4.0-preview | Deliver an independently qualified generic noncryptographic bounded integer draw contract for external Math consumers. | done |
| [M10](versions/M10.md) | 1.5.0-preview | Migrate the actual Math production source and maintained build graph to SDK 11.0.100, net11.0 and C# 15 with evidence-backed compatibility and rollback boundaries. | done |
| [M11](versions/M11.md) | 1.6.0-preview | Provide admitted generic exact Int64 scaling, resumable finite moments and larger-budget quantiles with compatibility-preserving candidate qualification before the actual M10 cutover. | done |
| [M12](versions/M12.md) | 1.7.0-preview | Establish a measured net11/C# 15 baseline and deliver evidence-driven additive modernization without semantic regressions. | ready |
| [M13](versions/M13.md) | 1.8.0-preview | Generate a complete admitted, compact, deterministic hierarchical Math skill from safe introspection, explicit semantics and resources; qualify invocation surfaces independently. | planned |
| [M14](versions/M14.md) | 1.9.0-preview | Deliver provider-neutral explicit temporal semantics, representations, zone-aware calendars and configurable academic/fiscal/business policies. | planned |
| [M15](versions/M15.md) | 1.10.0-preview | Add semantic quantity kinds and compiler-checked physical quantities over the existing exact/dynamic core. | planned |
| [M16](versions/M16.md) | 1.11.0-preview | Deliver bounded, discoverable typed scientific laws with assumptions, units, domains, measurement policies and independent scientific oracles. | planned |
| [M17](versions/M17.md) | 1.12.0-preview | Extend reproducible statistics, finite stochastic primitives and calendar-aware signal/time-series operations without importing decision policy. | planned |
| [M18](versions/M18.md) | 1.13.0-preview | Qualify scalable numeric representations and bounded advanced linear algebra, tensors and interval/precision engines. | planned |
| [M19](versions/M19.md) | 1.14.0-preview | Deliver bounded pure compute graphs, differentiable operations, mathematical explanation and visualization models, keeping orchestration and education policy external. | planned |
| [M20](versions/M20.md) | 2.0.0 | Qualify the exact supported 2.0 product graph, reproducible packages and skill, compatibility and rollback; separate any publication or downstream adoption. | planned |

M1 delivers the exact core early; 1.0 follows the M5 numerical/symbolic vertical. M7 spikes may conclude with deferral: their decisions are not delivered functions. Target versions are indicative, not dates or authorization to execute the whole program.

- [Architecture](architecture.md) and [inventory](existing-code-inventory.md)
- [Coverage](coverage.md), [sources](sources.md), and [qualification](testing-and-quality.md)
- [Coordination](cross-repository-coordination.md) and [DWF handoff](implementation-handoff.md)
- [Resource policy](resource-cost-policy.md)

- [Math 2.0 audit and roadmap](math-2.0/roadmap.md), [original audit](math-2.0/audit-original.fr.md), and [hierarchical Discovery specification](math-2.0/discovery-generation.md)

Scope: 21 releases, 37 work packages, 129 chunks, 192 tasks, 576 subtasks, 182 requirements.

Verify: dotnet run --file docs/planning/ValidatePlan.cs -- --check. Regenerate after editing with -- --write, exclusively outside preparation-safe validation. Self-tests: -- --self-test.

Functional dependencies, no agent fan-out. Refine files/commands before ready; split L blocks; do not create empty product packages in advance.
