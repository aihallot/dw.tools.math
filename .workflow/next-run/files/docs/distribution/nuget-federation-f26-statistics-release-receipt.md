# RS159 — Correct the private Statistics publication receipt field

## Durable baseline and bounded issue

RS158 succeeded first-pass on GitHub `main` at `48407dd5cf4f6df16b2ea5ba15a079f257421219`, attempt 001 with DWF `0.1.66`. F25 records a qualified publisher rehearsal for `dw.tools.math.statistics [20260.20261106.1001]` exclusively in a disposable feed: 18 binary oracles, pinned `dw.quantities [20260.20261102.1001]`, seven internal commands, refusal of collision/digest mismatch/cancellation, and no shared-feed publication.

F24 independently qualified the **unpublished original** Statistics candidate archive: exactly **17,451 bytes**, archive SHA-256 `9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8` and packaged DLL SHA-256 `75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96`. The original Statistics source Version remains `0.4.0-preview.6`, with eight C# files and exactly one historical `ProjectReference` to Quantities.

An owner-facing code audit after RS158 found a **single inherited IR-specific receipt field** in the guarded `--publish` mode of `tools/dw.tools.math.nuget-statistics-release/Program.cs`: `irDllSha256 = CandidateDllSha`. Although its value is the correct Statistics DLL hash, emitting the field `irDllSha256` would misidentify the evidence in the operator's immutable private publication receipt.

## Exact RS159 correction and source lineage

RS159 changes **only one field name** in the complete existing controller:

`irDllSha256 = CandidateDllSha` becomes `statisticsDllSha256 = CandidateDllSha`.

The value, physical source archive guard, SHA-256, package ID/version, dependency graph, publisher implementation, loaded library identity, 18 compiled binary tests, status messages, rights requirement, clean-`main` guard and RS158 success gate are not changed.

The read-only F26 validator examines the immutable historical controller at `.workflow/runs/RS158/staged/tools/dw.tools.math.nuget-statistics-release/Program.cs` and verifies its SHA-256 against F25's source-file receipt. It checks that the **entire corrected controller source** is exactly the historical source with this one substitution, and pins both original and corrected source hashes. The validator keeps F24/F25 evidence immutable; it cannot demand the historical F25 source SHA equal the intentionally corrected RS159 controller SHA.

RS159 installs the corrected controller and observational F26 validator, executes a private-feed-only `dotnet restore` and `dotnet build` of the existing nonpackable tool and captures three actual process receipts (restore, build, SDK). No `--publish`, `--verify` or `--rehearse` mode is invoked.

## Operator gate after success

Only after durable RS159 success can the owner separately decide to invoke `--publish` and then `--verify` on a clean `main` checkout. Both guarded modes still require the successful RS158 report. The original F24 archive alone may be published; never substitute F25 disposable-rehearsal bytes. The private operator receipt will then use `statisticsDllSha256`, and independent physical-feed verification is a distinct future DWF run.

RS159 does not publish or reserve an archive, select a public license, qualify installed rollback or other OS/RIDs, modify the eight original Statistics C# sources, or enable Discovery, Decision or AURA integration. The 541 accepted native Math nodes and seven previously published packages remain unchanged.
