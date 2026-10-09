# M1 / 0.2.0-preview — exact Math release-gate recipe

The local release gate checks mathematical exactness, package independence and a bounded standalone consumer. It does **not** publish binaries or assert recipient adoption.

## Reproduction

From the Math repository root:

```pwsh
dotnet restore dw.tools.math.slnx --locked-mode
dotnet build dw.tools.math.slnx -c Release --no-restore
dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --no-restore --no-build
pwsh -NoProfile -NonInteractive -File scripts/verify.ps1
dotnet run --file docs/planning/ValidateTransferArchitecture.cs
```

Package-only consumer reproduction, including the isolated local NuGet feed, is documented in `docs/distribution/exact-math-packages.md` and implemented by RS025's validated source recipe. The gate reproduces these three `dw.quantities*` package builds and the independent `tests/consumers/exact-math-smoke/` run, recording new package SHA-256s.

After the DWF payload updates the product backlog and native plan, a read-only gate checker runs:

```pwsh
dotnet run --file docs/planning/ValidateM1Gate.cs -- --check
dotnet run --file docs/planning/ValidatePlan.cs -- --check
```

`ValidatePlan.cs --write` is a **payload-only** generated-document operation. All execution results must be observed, not estimated.

## Authority and consumer contract

- Owner authorization: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`, AURA revision `82a6b435a387a7e116b47a6b2c433ae9e067bf21`.
- Three package IDs/version `0.2.0-preview.1`, no AURA/dw.localization dependencies.
- M1-W03-C01 receipt: `docs/planning/evidence/M1-W03-C01-qualified.json`.
- M1 gate receipt: `docs/planning/evidence/M1-W03-C02-gate.json`.
- Adoption decision: `docs/planning/decisions/m1-w03-c02.md`.
- External request: `docs/coordination/requests/MATH-XR-002-aura-adoption.json` (**draft**, transmission **none**).
- Recipient matrix: `docs/coordination/matrices/m1-aura-adoption-consumers.json` (**not adopted**, AURA tests **not run**).

The three package binaries remain in an ignored local build directory and are never committed to Math or automatically published. Their newly observed SHA-256s belong to that run; package availability does not mean an external feed or recipient is using them. No AURA source deletion or rollback action occurs in Math.

The exact gate is fully local and is distinct from the M2 broader math platform and any future AURA consumer cutover.
