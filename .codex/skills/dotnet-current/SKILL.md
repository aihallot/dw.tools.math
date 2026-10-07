---
name: dotnet-current
description: Apply current .NET and C# authoring practices, analyzer-aware patterns, SDK discovery, repository layout conventions, and a non-mutating structural checker.
---

# Current .NET/C# authoring skill

Use this skill for non-trivial .NET/C# work when the repository targets a current SDK or enables strict analyzers/warnings-as-errors.

This skill complements `engineering-guidance`. It is intentionally narrower and can evolve independently as SDK, analyzer and language practices change.

## Before coding

Inspect the repository's actual toolchain before choosing APIs or syntax:

1. read `global.json` when present;
2. inspect `Directory.Build.props`, `Directory.Build.targets`, `.editorconfig` and project files;
3. inspect target frameworks, `LangVersion`, analyzer packages, `AnalysisLevel` and `TreatWarningsAsErrors`;
4. inspect installed SDKs with `dotnet --list-sdks` when execution is available;
5. prefer the repository's compatible installed SDK over an unavailable aspirational version;
6. when external documentation is available, verify current APIs and analyzer guidance against official Microsoft/.NET documentation rather than relying on stale examples.

Never silently weaken warnings, analyzers or target frameworks merely to make a build pass.

## Current authoring priorities

- Prefer analyzer-clean code on the first implementation pass.
- Treat analyzer diagnostics as design feedback when the repository promotes them to errors.
- Prefer current APIs and idioms supported by the repository's resolved SDK and target framework.
- Do not assume an API exists merely because it appears in a newer SDK than the repository resolves.
- Do not pin `global.json` to an SDK that is unavailable on the intended execution environment without an explicit installation/deployment plan.
- Keep planning/validation helpers in the product's native stack by default. For a .NET product, prefer .NET for executable planning/checking utilities unless a different runtime has a demonstrated benefit.
- Preserve explicit compatibility constraints instead of force-upgrading unrelated projects.

## Analyzer-aware patterns

Before delivery, pay particular attention to analyzer families that commonly become errors under current recommended analysis levels:

- dictionary lookup patterns such as `TryGetValue` versus repeated `ContainsKey` + indexer;
- unnecessarily abstract return types when a concrete type is known and improves generated code or analysis;
- correct parameter names in argument exceptions;
- allocation, enumeration and LINQ patterns flagged by performance analyzers;
- nullable flow and API annotations;
- disposal and async correctness;
- source-generation/AOT restrictions when trimming or native-AOT-sensitive code is involved.

Do not memorize rule numbers as eternal truth. Read the diagnostic message and apply the repository's current analyzer configuration.

## Repository structure

For new or deliberately touched .NET project structure:

- solution names, project names, `.slnx` / `.sln` filenames, `.csproj` filenames, project directories and command-facing project paths are lowercase by default;
- namespaces and C# type names remain idiomatic PascalCase;
- keep generated and compiled outputs outside `src/` and `tests/`;
- centralize build/intermediate output roots through repository-level MSBuild configuration when practical;
- do not commit `bin/`, `obj/`, publish output or generated build products under source trees;
- do not mass-rename unrelated legacy projects solely to satisfy this rule; apply it to new/touched structure or a dedicated migration.

## Structural check

The skill includes `scripts/check-dotnet-conventions.cs`.

Run it from the repository root when structural qualification is relevant:

`dotnet run --file .codex/skills/dotnet-current/scripts/check-dotnet-conventions.cs -- .`

The checker is read-only. It reports:

- uppercase or mixed-case `.csproj` paths under `src/` or `tests/`;
- missing repository-level build/intermediate output relocation when projects live under `src/` or `tests/`;
- committed or present `bin/` or `obj/` directories beneath those source trees.

A failure is evidence to correct deliberately; the checker never rewrites the repository.

## Handoff

Report the resolved SDK/TFM/language/analyzer baseline, material diagnostics addressed, structural checker result when applicable, and any deliberate deviation from these conventions.
