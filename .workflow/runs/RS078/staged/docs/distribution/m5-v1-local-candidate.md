# Math local 1.0 evidence candidate — NOT a stable published release

The executable product remains `dw.tools.math.distribution`
**1.0.0-preview.4** in a local net10 NuGet graph. The label
`1.0.0-local-rc-evidence-only` is a documentary evidence bundle only.

Reproducible qualification:

1. Read the ten exact source criteria in `docs/planning/coverage.md`.
2. Read `docs/planning/releases/m5-v1-local-candidate-map.json` for each
   admitted proof and its explicit mathematical limitation.
3. Run `dotnet run --file docs/planning/ValidateM5ReleaseCandidate.cs -- --check`
   on a .NET 10 installation; this is **read only**.
4. Check prior `M1-W03-C02-gate.json`, `M2-gate.json`, `M3-gate.json`,
   `M4-gate.json` and source-owner authorization in M0 evidence.
5. Reuse existing source-free NuGet examples and the restricted x²−1,
   x≠1 exact derivative/value/root recipe. Treat every unsupported operation
   and approximate conversion as a typed boundary, not a silent success.

The manifest maps ten V1 criteria and previous gates; it does NOT establish
the final G-M5 gate. No arbitrary CAS or TeX/Office source inversion, no
unqualified OS RID, no guaranteed cross-version API/ABI or latency SLA, no
signed/stable NuGet publication, no open-source license grant and no delivered
AURA/Decision/MCDM integration are claimed.

Recipient drafts are intentionally unsent. All publication decisions,
recipient tests and owner-controlled rollout/rollback belong to C03-T2 and
the final independent release gate.
