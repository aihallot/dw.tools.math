# G-M6 — independent local scientific milestone acceptance

## Question

Can the accepted Math 1.1 scientific expansion be marked **locally complete** using independent evidence for all eleven admitted chunks, source-free NuGet consumption and fresh .NET regression tests, without pretending that stable 1.1.0 is published or that another repository has adopted these APIs?

## Evidence and scientific boundary

- The accepted G-M6 scope comprises eleven completed phases, twenty-two tasks and sixty-six subtasks across the completed `M6-W01` and `M6-W02` work packages. Each phase has durable accepted evidence, a bounded independent oracle and a terminal refusal contract.
- M6-W01 numerical-analysis historical receipt: `docs/planning/evidence/M6-W01-local-gate-qualified.json`. One actual local `dw.tools.math.analysis 1.1.0-preview.6` archive: 27,820 bytes, SHA-256 `facb88dcf029de1445122eef38a147b609b89d1c5a36567506b1c10c2ce423ac`. Three historical source-free eight-probe consumers and 328 targeted tests were retained.
- M6-W02 scientific historical receipt: `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json`. One actual local `dw.tools.math.scientific 1.1.0-preview.14` archive: 37,270 bytes, SHA-256 `2de3e9f536ced58e28d1558ff7c4692f367b72d5efaa1989d56ffa8ec07a31cf`. Seven historical source-free eight-probe consumers and the accepted scientific test corpus were retained.
- `docs/planning/evidence/M6-W02-C05-T1-qualified.json` admits seven historically versioned scientific contracts with independent source SHA and oracle limits; `docs/planning/evidence/M6-W02-C05-T2-qualified.json` closes the owner/semver/notices boundary with an explicit **retain-all-seven** disposition, without granting migration consent.
- The standalone BCL-only `docs/planning/ValidateM6ReleaseGate.cs` checks all product/native lifecycle nodes, status-pinned phase evidence, eleven retained evidence SHA identities, the historical package bytes and ten independent consumer receipts. Its result must be recorded before and after the milestone transition.
- This gate also requires a fresh locked .NET restore and a successful Release run of the full admitted solution test suite. These tests confirm the current Math source but do **not** reconstruct or reissue the historical package archives, and must not be described as new consumer requalification.

## Decision

Only when the independent validator and controlled local regression tests pass, accept M6 as a **locally qualified scientific functionality milestone**. The release label `1.1.0` is the product-roadmap milestone, **not** a packed, signed, released or public stable 1.1.0 binary. The two accepted packages remain `1.1.0-preview.6` and `1.1.0-preview.14`, local and unpublished.

No public NuGet or stable 1.1.0 publication, new license, external provider cutover, Decision/MCDM/AURA adoption, `dw.discovery` integration, untested operating system, cross-version ABI or universally certified numerical error bounds are implied. Owner approval, rights/notices, semver compatibility, target runtime validation, recipient opt-in and rollback remain separate future gates.

The independently preserved finite-domain limits exclude general/high-precision intervals, advanced tensors, FFT, geodesy, stiff ODE/PDE, forecasting, market recommendations and implicit physical-unit compatibility. Accepted evidence is not a theorem of broad universal mathematical coverage.

## Alternatives, acceptance and next decision

Reject inferring release/publication permission from a successful local solution test, retroactively relabeling historical NuGet receipts, and modifying sibling repositories. The only lifecycle transition permitted by this gate is `M6: in-progress -> done` with one matching DWF achievement; generated planning projections must be reconciled.

After verified G-M6 success, the product roadmap proposes M7 advanced extensions. Their scope, providers and independent oracles still require a separate planning/admission decision. Do not prepare a public-release or external-adoption run as an automatic successor.
