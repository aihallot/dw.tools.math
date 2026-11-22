# M8-W01 — Portable private-package Discovery skill export

Status: RS281 Windows .NET 11 export qualification succeeded at attempt 001 after three pre-attempt refusals (`firstPassSucceeded=false`). RS282 Linux/WSL qualification succeeded at attempt 001, first-pass true. RS283 defines a candidate unsigned local export integrity receipt; qualification is determined only by the durable RS283 attempt report, not by this page. No external adoption or public publication is claimed. Generated source metadata remains canonical in `docs/discovery/qualification/m8-discovery-manifest.json`.

## Purpose

The existing Math Discovery skill supports four selected AgentCallable operations, but its source-project example link is repository-relative. `ExportM8PortableDiscoverySkill.cs` creates a reproducible, isolated export with rewritten portable references, an embedded copy of the original positive CLR seed, a `net11.0` project with an exact private `PackageReference`, and a pinned SDK. This does not modify the canonical generated skill or its source-only package identities.

## Export and independent qualification

- Authoring check: `dotnet run --file docs/discovery/qualification/ExportM8PortableDiscoverySkill.cs -- --check`. It creates two distinct disposable exports under `build/artifacts/`, checks deterministic source files, validates installed links/authorization boundaries, and exercises the sample through the actual private immutable Composition, IR and Quantities binaries on Windows .NET 11 with locked restore.
- Controlled optional export: `dotnet run --file docs/discovery/qualification/ExportM8PortableDiscoverySkill.cs -- --export <new-absolute-directory>`. The destination must not exist. The exporter includes neither NuGet archive nor private-feed credentials.
- In an exported directory, set the private package source explicitly and follow `PORTABLE-README.md`. The generated example requires SDK `11.0.100`; it is not a runnable source project outside the export.

## Exact package boundaries

| Package | Version | Archive SHA-256 |
| --- | --- | --- |
| `dw.tools.math.composition` | `20260.20261107.1001` | `b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f` |
| `dw.tools.math.ir` | `20260.20261104.1001` | `d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480` |
| `dw.quantities` | `20260.20261102.1001` | `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d` |

Restore is limited to the selected private feed and exact three-package closure. No package is built, republished or overwritten. The original consumer algorithm is reused without new Math semantics; only its result scope label changes for the exported package-only invocation.

## Independent Linux/WSL qualification — RS282 succeeded

The Math-owned probe `docs/discovery/qualification/ProbeM8WslNet11PortableDiscoverySkill.cs` reuses the exact RS281 `--export` path to create a new disposable twelve-file bundle. On the default WSL Linux distribution, it checks SDK `11.0.100`, the three immutable private archive/DLL SHA-256 identities, an isolated locked `net11.0` `PackageReference` restore and the same four selected positive CLR calls. It also verifies that no exported input files or original archives changed. This qualifies the Linux package-only example if and only if RS282 succeeds; it does not install the skill in a remote agent host or confer broker permissions.

## Optional portable integrity receipt — RS283 succeeded

The existing qualified twelve-file export remains unchanged. The Math-owned `AttestM8PortableDiscoveryExport.cs` wrapper invokes that export into a fresh destination, adds its own standalone `verify-export.cs` verifier, and creates an unsigned `portable-attestation.json` with SHA-256 digests for the 13 payload files. A clean export contains 14 files including the receipt. The verifier can be run **from the exported directory without the Math repository or private feed**, before package restore:

`dotnet run --file verify-export.cs -- --verify <ABSOLUTE_EXPORT_DIRECTORY>`

It refuses changed, missing or unexpected files and altered source capability authorization. The RS283 `--check` profile compares two exports byte-for-byte, runs the copied verifier independently and tests three deliberate corruptions without replaying the RS281/RS282 package suites. The wrapper offers `--export <NEW_ABSOLUTE_DIRECTORY>` for an opt-in local export; the destination must not exist. No operator-side export command is needed for DWF run-next.

The integrity receipt is **not digitally signed** and does not authenticate provenance against a malicious party able to replace both the receipt and verifier. External distribution would require a trusted out-of-band digest or signature. It does not install the bundle in an agent host, confer execution permission, promote BrokerReady or authorize publication.

## Strict CLR metadata before attestation — RS288 integration

The RS288 qualified path reuses the existing `ValidateM8StrictBindingMetadata.cs --validate` mode against the **exported** `.agents/skills/dw-tools-math/references/manifest.json` from each newly generated bundle. The attestation wrapper must refuse metadata whose canonical signature disagrees with declared assembly, member, return value or ordered CLR parameters, or whose fingerprint or runtime-only authorization is inconsistent. No additional generated file, new package or secret is inserted; the existing unsigned fourteen-file export identity remains.

The offline `verify-export.cs` contract is **unchanged**: it checks the unsigned receipt, the recorded file hashes and the selected runtime-only authorization boundaries without requiring the Math source repository. This local checksum does not replace a trusted signature, prove actual loaded CLR assemblies or authorize any broker invocation.

## Limits

An isolated directory copy plus a package-only executable is not an installation in a supported agent host, a runtime authorization grant, BrokerReady status, AURA integration or external consumer acceptance. The generated source skill still declares its source package versions. M8 native tasks and RED/GREEN/verify subtasks remain not-ready unless a separate lawful forward-only transition is made.
