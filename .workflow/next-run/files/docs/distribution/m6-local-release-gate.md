# Math G-M6 local scientific release gate — reproducible check

**Product milestone:** M6 / `1.1.0` local acceptance, **not a public stable NuGet release**. Historical observed packages are `dw.tools.math.analysis 1.1.0-preview.6` and `dw.tools.math.scientific 1.1.0-preview.14` only.

## Independent acceptance recipe

1. From the pinned source tree and an available .NET 10.0.4xx SDK under `global.json`, perform locked restore of `dw.tools.math.slnx`.
2. Run the entire admitted .NET 10 solution test suite in Release, failing the gate on any regression. This qualifies current code, not fresh package oracles.
3. Run `dotnet run --file docs/planning/ValidateM6ReleaseGate.cs -- --check` against the pre-transition baseline. It checks all eleven accepted chunks, twenty-two tasks and sixty-six native/product subtasks, status-pinned phase receipts, M5 predecessor, and owner/publication limits.
4. Preserve the historical source-free package evidence: three eight-probe analysis consumers (27,820-byte SHA `facb88dcf029de1445122eef38a147b609b89d1c5a36567506b1c10c2ce423ac`) and seven eight-probe scientific consumers (37,270-byte SHA `2de3e9f536ced58e28d1558ff7c4692f367b72d5efaa1989d56ffa8ec07a31cf`). These are already completed **prior** qualifications; they are not recreated or rerun by G-M6.
5. Record the validator source SHA and decision/recipe SHA, current selected SDK and RID, commands actually executed, exact accepted evidence files, local package history, and explicit unapproved actions in `docs/planning/evidence/M6-gate.json`.
6. Transition the M6 native milestone to done only under the same controlled DWF mutation, mark the corresponding backlog release done, regenerate the accepted projection and re-run all structural, independent gate and diff checks.

## Reproduction commands

`dotnet restore dw.tools.math.slnx --locked-mode`

`dotnet test dw.tools.math.slnx --configuration Release --no-restore`

`dotnet run --file docs/planning/ValidateM6ReleaseGate.cs -- --check`

`dotnet run --file docs/planning/ValidateNativeDwfAdoption.cs`

`dotnet run --file docs/planning/ValidatePlan.cs -- --check`

All are local; no GitHub Actions or paid CI is authorized.

## Qualified limits

The independent historic NuGet receipts originated on .NET 10, `win-x64`, and recorded SDK `10.0.402`. Record actual RS105 host/SDK; an observed second environment does not automatically qualify ABI/OS portability without consumer tests there. Fresh full-solution tests do not replace exact source-free package tests.

No public NuGet publication, stable 1.1.0 package, license grant, remote numerical provider, automatic scientific family replacement, Decision approval, external recipient migration, production support SLA or certified universal numerical error bounds are permitted. A separate owner authorization and recipient acceptance are required before any future package cutover.

**G-M6 completed** means the *local bounded scientific milestone* is accepted. M7 remains an independently admitted future roadmap, not an automatic runtime/publication change.
