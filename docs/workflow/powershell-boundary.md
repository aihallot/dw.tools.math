---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.powershell-boundary"
documentVersion: 2
kind: "guidance"
title: "PowerShell Boundary"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV012Draft.cs#PowerShellBoundary"
---
# PowerShell Boundary

PowerShell is a narrow integration boundary, not the default implementation language.

Use PowerShell when it materially simplifies an operator command, Windows or shell integration, invocation of an existing external tool, or a bounded external publication or migration wrapper. Use typed C# for parsing, mutation decisions, contracts, orchestration, rollback logic, and reusable behavior.

A run that adds or changes PowerShell stages complete `.ps1` files. Do not construct scripts from multiline C# strings, concatenate fragments, or patch generated scripts after creation.

Before payload execution, current pending-run PowerShell is parsed through the PowerShell AST parser. Syntax failure occurs before attempt allocation and repository mutation. Parser success proves syntax only; non-trivial scripts still require behavioral, disposable, mutation-boundary, and rollback validation appropriate to their risk.

When punctuation follows a variable, delimit the variable explicitly, for example `${Label}:`.

Do not add another scripting framework solely to replace disciplined complete-source authoring and the built-in parser boundary.
