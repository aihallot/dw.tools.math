# Math post-M7 roadmap — owner-approved direction, proposed topology

Status: **owner-approved direction, not yet admitted to canonical backlog or native DWF plan**. Date: 2026-11-12.

## Preservation boundary

The accepted backlog `docs/planning/backlog.json` stays at `0.1.122`, `done`, for M0–M7. The 541 existing native DWF nodes and all existing run evidence remain intact. No package version or immutable feed artifact is changed in RS179. Owner approval authorizes the strategic sequence below, not an unverified claim that a future Math feature is implemented.

## Approved order

1. **M8 — Foundation and Discovery.** Two independent bounded streams: (a) opt-in Math Discovery, exact CLR bindings, pinned 13-case schema-2 conformance and deterministic generated skills; (b) explicit .NET 11 / C# 15 SDK/API compatibility audit, targeting and selected migration. Start with Discovery's exact authority/topology assessment, then modernization qualification.
2. **M9 — Scientific Quantities.** Versioned physical/scientific constants and independently selected opt-in unit profiles in electrical, thermal, mechanical, chemical and photometric contexts. Preserve the 58/14 inherited catalogues.
3. **M10 — Advanced Numerics.** Precision and interval policies, uncertainty, finite tensors and qualified numerical algorithms. Require bounded budgets and independent numerical oracles.
4. **M11 — Computation and Interoperability.** Local finite DAG, deterministic typed composition and bounded formal exchange; never promise a theorem prover or distributed scheduler.
5. **M12 — Qualification and Distribution.** Windows/Linux/macOS evidence as actually available, per-capability documentation, package compatibility matrices and owner-gated release decisions.

The structured proposal, scoped work streams and acceptance criteria are in `docs/planning/proposals/math-next-roadmap-m8-m12.json`. This document is its human-readable introduction, not an independently maintained second backlog.

## Prior Discovery proposal and DWF topology

Keep `docs/planning/proposals/math-agent-discovery-work-package.json` as the starting candidate, not a silently accepted `M3-W03` inside the already-completed M3. Review whether Discovery should be represented under the new M8 rather than retroactively altering M3.

DWF issue #139 is **open**, but current `dw.tools.workflow` source contains typed `ProjectPlan.EnsureMilestoneSubtree` and `ProjectPlan.EnsureWorkPackageSubtree`. Their availability in the operator's installed **DWF 0.1.66** has *not* been verified. The next topology run must inspect active compiled SDK signatures and use only an admitted typed operation or an approved rebaseline; never directly edit canonical native plan JSON.

## .NET 11 / C# 15 transition

Use Microsoft's current official documentation as the API source: <https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/overview> and <https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15>. The first modernization slice verifies the actual stable installed SDK and assesses new numeric APIs, generic complex/decimal families, C# union/closed hierarchy opportunities and compatibility. No conversion solely for syntax fashion; benchmark or test every chosen replacement. The current Math repository and historic packages remain on their evidenced .NET 10 source/toolchain boundary until a separate qualified migration.

## Exit criteria for the first implementation sequence

- Confirm the exact DWF tool version and whether typed milestone insertion is callable; preserve all historical plan identities.
- Reconcile a genuinely new accepted M8 scope into the canonical backlog, native plan, validators and projections in the same coherent DWF preparation/execution sequence; no shadow plan.
- Independently validate chosen Math CLR bindings against actual compiled package graph, with reproducible examples and no automatic BrokerReady/authorization escalation.
- Migrate net11-specific functionality and publish new packages only after bounded compatibility, tests and approval gates. Never overwrite existing NuGet archives.

RS179 itself performs *documentation-only* preparation and validation; it does **not** make M8 active or claim any new task completed.
