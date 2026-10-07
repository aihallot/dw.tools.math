# dw.tools.math

Independent mathematical computation and representation foundation for applications, scientific libraries, and agents, without an AURA dependency.

**Status on 2026-10-07: initial planning; no mathematical engine has been implemented in this repository yet.** Candidate extraction code exists in AURA; its presence there is not a Math delivery.

The first priority is to recover and qualify the exact rationals, quantities, units, and expressions that are already reusable in AURA. Next come a common mathematical representation, numerical and symbolic providers, composition, and exchange formats. Decision algorithms remain in Decision; multicriteria methods remain in MCDM.

- [Complete plan and versions](docs/planning/README.md)
- [Architecture and contracts](docs/planning/architecture.md)
- [Existing-code inventory](docs/planning/existing-code-inventory.md)
- [Cross-repository migration and coordination](docs/planning/cross-repository-coordination.md)
- [ChatGPT and DWF handoff](docs/planning/implementation-handoff.md)
- [First prompt after DWF initialization](docs/planning/start-with-dwf.md)
- [Canonical backlog](docs/planning/backlog.json)
- [Project constitution](PROJECT-CONSTITUTION.md)

## Documentation validation

From the repository root, with .NET 10 SDK (10.0.401 observed during initialization):

```powershell
dotnet run --file docs/planning/ValidatePlan.cs
```

This command validates without rewriting documents. After an intentional backlog change, regenerate in the payload or during authoring, then validate:

```powershell
dotnet run --file docs/planning/ValidatePlan.cs -- --write
dotnet run --file docs/planning/ValidatePlan.cs
```

Planning requires no Python. This preparation does not install a domain SDK, provider, product CI, or DWF. Future build/test/publish scripts are M0 deliverables, not commands already available today.

DWF works only in this repository. Requests to AURA, Decision, and MCDM are prepared as Markdown and transmitted by the owner to the responsible agents. No write access to other repositories is implied.

Package names, distribution licenses, and exact provider versions must be confirmed at their planned gates; no NuGet publication is claimed.
