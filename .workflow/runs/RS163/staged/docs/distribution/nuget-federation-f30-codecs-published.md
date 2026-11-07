# RS163 — Independent readback of privately published Codecs, IR and Quantities

## Provenance, authorization and separation

RS162 succeeded first-pass on GitHub `main` at `8577c235ff3fe1fa37e39767dac281f114aef56d`, with DWF 0.1.66, attempt 001 and no invocation failures. Its F29 receipt proves a disposable-feed publisher rehearsal with fifteen compiled Codecs checks; **it is not evidence of publication to the shared feed**. The user subsequently authorized **only** the private immutable publication of the original `dw.tools.math.codecs [20260.20261107.1001]` candidate into `NUGET_LOCAL`, followed by independent verification. This permission does not authorize public licensing, remote registries, installed rollback or external repository mutation.

F28/RS161 qualified the original Codecs candidate: 35,096 bytes, SHA-256 `00392fbeddebceb70286b11377aeab120bef0e3ef45f9b12dc2ba878918f8e35`, packaged net10.0 DLL SHA-256 `3e8ba8e03bf6519de28837b1e01d9842e5e76bfee6a6ad9cceee442b47f2fe74`. This **original artifact**, not F29's disposable rehearsal bytes, is the approved immutable private publication target. Its one declared exact direct dependency is `dw.tools.math.ir [20260.20261104.1001]`; IR declares an exact transitive `dw.quantities [20260.20261102.1001]`.

The previously independently qualified IR archive is 21,926 bytes, SHA-256 `d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480`, DLL SHA-256 `5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af`. Quantities is 14,747 bytes, archive SHA-256 `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`, DLL SHA-256 `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4`. All three are required to be physically present in `NUGET_LOCAL` before RS163 can succeed.

## Independent physical readback

The RS163 C# verifier has no publication API or mutation permission for `NUGET_LOCAL`. It independently checks all three physical archives, exact bytes, SHA-256, archive structure, embedded DLLs, and the one-hop Codecs → IR → Quantities package manifest closure. The archive-only package set is copied to a disposable snapshot; a fresh isolated `net10.0` project uses **one Codecs PackageReference** and no Math source `ProjectReference`. It restores exclusively from the snapshot and runs the fifteen original compiled Codecs binary oracles including loaded DLL SHA, canonical notations and refusal/resource boundaries. Feed archives are checked again for byte identity after the consumer run. Four inner and four outer command receipts, SDK/runtime metadata, exact dependencies and evidence limits are retained.

The verifier only acknowledges independently inspected physical evidence. The F30 readback result is written by DWF only after actual success. There is no shared-feed publication, local package reservation, public license, installed rollback, broad feed inventory, cross-OS qualification, external migration, or AURA D3 in RS163. The 541 completed native nodes and accepted Math product plan remain unchanged.

## Operator order and refusal boundaries

Perform the authorized private release only on the exact clean `main` checkout with `NUGET_LOCAL` pointing to the existing private feed. First confirm that the original RS161 archive is physically present at `build/artifacts/rs161-codecs-candidate/candidate/dw.tools.math.codecs.20260.20261107.1001.nupkg` and matches the exact bytes and SHA above. If it is missing, stop: do not substitute freshly packed or F29 rehearsal bytes, and do not assume DWF's disposable workspace artifacts were retained. Use the already-qualified `tools/dw.tools.math.nuget-codecs-release` controller's guarded `--publish` then `--verify` modes. On an ambiguous or partial write, inspect the existing immutable target and receipt before any retry.

Only after the physical package and owner verification have succeeded should the operator invoke `dwf run next` for RS163. Its DWF payload explicitly invokes **only `--verify-published-codecs`** in a fresh disposable workspace, never `--publish`. If the published package is missing or differs, this run refuses to call it qualified.
