# M9 — Owner authorization for private Statistics publication

On 2026-11-21 the owner explicitly authorized a **controlled private publication** of
`dw.tools.math.statistics 20260.20261120.1001` to the already configured `NUGET_LOCAL`
feed, with **no overwrite** and with independent verification.

Scope: this exact version and its exact published `dw.quantities 20260.20261102.1001`
dependency only. No public registry, public license selection, retroactive changes to
existing NuGet archives, change to the Math production SDK, or Assessa repository mutation.

This approval does not imply that a publication happened. DWF RS272 must first qualify
the guarded publisher using a disposable rehearsal feed. To prevent irreversible external
side effects during DWF preselection, actual `NUGET_LOCAL` publication is performed only
by the guarded `--publish` mode **after** durable RS272 success on clean `main`.
The separate independent reader/consumer must then verify physical bytes and net11 binary
behavior before Assessa adoption is claimed. An existing target version is always refused,
even when its bytes match.

The exact publisher result, hashes, and independent readback evidence are to be reported
truthfully by the operator; neither preparation nor rehearsal is publication.
