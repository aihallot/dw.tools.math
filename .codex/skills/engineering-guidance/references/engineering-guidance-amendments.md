# Engineering guidance amendments

This file contains accepted guidance amendments that are immediately applicable but not yet integrated into the canonical engineering guidance version.

Canonical baseline: `engineering-principles-dotnet-conventions.md` v2.7

Next planned integration: v2.8

## Purpose

The canonical engineering guidance should remain stable enough to use without constant micro-version churn.

When a useful rule is discovered after a stable version has been accepted, record it here instead of immediately reopening the full guidance set.

Accepted amendments apply immediately. They are integrated into the canonical document during a planned integration pass.

## Status vocabulary

- `Proposed`: idea captured, not yet accepted.
- `Accepted`: applies immediately, not yet integrated into the canonical document.
- `Integrated`: moved into the canonical document.
- `Rejected`: kept for traceability but not applied.
- `Superseded`: replaced by a better amendment.

## How to use amendments

Use this file when:

- a task touches an area covered by an accepted amendment;
- the user mentions a recent rule that has not yet been folded into the canonical guidance;
- build, test, publish, repository layout, workflow scripts or task runners are involved;
- preparing the next planned integration pass.

Accepted amendments clarify or supplement the canonical guidance for their targeted area.

Do not use this file as a dumping ground for vague ideas. Only accepted, actionable corrections belong in the accepted amendments section.

## Integration policy

Do not create a new canonical version for every typo, clarification, example or isolated micro-rule.

Create the next canonical version when one of these is true:

- several accepted amendments have accumulated;
- an amendment changes agent behavior materially;
- a new section or major concept is needed;
- the skill entrypoint must change;
- a grouped neutral review is needed.

When integrating amendments:

1. Move accepted amendments into the canonical document.
2. Update the canonical version number.
3. Mark integrated amendments as `Integrated` here or move them to an integration history section.
4. Update the abstract or mantra only if their operational summary changes.
5. Recopy accepted references into the skill.

## Accepted amendments

### A001 — Root-level build/test/publish helper scripts

Status: Accepted\
Target version: v2.8\
Applies to: .NET projects, command-facing repositories, build/test/publish workflows\
Target section: .NET implementation profile / Build, test, publish workflow

#### Guidance

Repositories should provide root-level helper scripts, Makefile targets, CMake presets, or equivalent task-runner commands for common `dotnet restore`, `dotnet build`, `dotnet test`, and `dotnet publish` workflows when this improves repeatability and discoverability.

These helpers must respect the repository layout rule that durable build and publish outputs do not live under `src/`.

#### Recommended behavior

- Keep helper scripts at repository root or in a dedicated `scripts/` folder.
- Keep durable `build/` and `publish/` outputs at repository root level, not under `src/`.
- Make common commands discoverable from the repository root.
- Prefer repeatable commands over undocumented manual command sequences.
- Avoid destructive cleanup unless paths are explicit, narrow and guarded.
- Prefer simple PowerShell or shell scripts when they are enough.
- Use Make, CMake presets or task runners only when they simplify orchestration rather than add ceremony.
- Keep output paths explicit in scripts and documented commands.
- Ensure agent-facing commands do not accidentally mix generated artifacts with source folders.

#### Rationale

The layout convention is easier to respect when the intended commands are encoded in scripts or task targets.

This reduces accidental build or publish output under `src/`, improves reproducibility, and makes future AI-agent work safer.

#### Integration note

Fold this into the next canonical version under build/test/publish workflow guidance and mirror it into the engineering-guidance skill references.

### A002 — Current .NET baseline and lowercase project/solution names

Status: Accepted\
Target version: v2.8\
Applies to: new .NET projects, new .NET solutions, new project files, new solution files, touched .NET naming decisions\
Target sections: .NET implementation profile / Platform and language; Naming and repository layout

#### Guidance

For new .NET development, use **.NET 10** and **C# 14** as the default baseline.

Use an older target framework or language version only when an explicit compatibility, deployment, dependency, support, or repository constraint requires it. Record that deviation rather than silently defaulting to an older baseline.

Solution and project names must be lowercase by default. This applies to:

- solution names and `.slnx` / `.sln` filenames;
- project names and `.csproj` filenames;
- project directories;
- command-facing paths containing those names.

Avoid PascalCase or mixed-case solution/project filesystem names such as `Dw.Tools.App` or `Dw.Tools.App.csproj`. Prefer forms such as `dw.tools.app` and `dw.tools.app.csproj`.

C# identifiers follow normal C# conventions independently: namespaces and types may remain PascalCase, for example `Dw.Tools.App`.

#### Recommended behavior

- New projects should target `net10.0` unless a documented constraint requires another target.
- New projects should use C# 14; set `LangVersion` explicitly when doing so improves auditability or prevents ambiguity.
- Do not choose an older .NET/C# baseline merely because an older repository example or template used it.
- Inspect `global.json`, `Directory.Build.props`, CI, deployment targets, package compatibility, and repository-specific rules before changing an existing project's target framework or language version.
- Do not mass-rename existing projects solely to satisfy lowercase naming during unrelated work; align touched/new project and solution names incrementally unless a dedicated migration is planned.
- Keep namespaces and type names idiomatic even when filesystem/project names are lowercase.

#### Rationale

The lowercase naming rule already exists in the canonical v2.7 .NET naming guidance, but it must be surfaced more prominently because project generators and agents can otherwise fall back to common PascalCase .NET naming habits.

.NET 10 is the current active LTS baseline and C# 14 is the corresponding current stable language generation. Making the default explicit avoids stale template choices while preserving documented compatibility exceptions.

#### Integration note

Integrate this into v2.8 sections 24 and 25, and keep the operational rule visible in both the engineering abstract and the skill entrypoint.

## Proposed amendments

None.

## Integration history

No amendments have been integrated yet.
