# Math electrochemical SI unit profile — source qualification

## Scope and ownership

Gate: `MATH-ELECTROCHEM-UNIT-PROFILE`; consumer need: `A2-W02-C01-U-N`.
The request was relayed by the owner on 2026-11-09, with AURA checkpoint
`c818346f`. AURA-side local-only documents were not accessible from the
GitHub-only operator session; this work uses the owner's provided requirements
and the authoritative Math source and backlog. No AURA code or tests were copied.

The original Math-owned, portable `dw.quantities.standard.ElectrochemicalUnitProfile`
is **explicitly opt-in**. `ProfileId = "math.si-electrochemical"`;
`ProfileVersion = "2026-11-electrochemical-exact-v1"`.
Its typed CLR API exposes `Ampere`, `Volt`, `Ohm`, `Watt`, `Mole`,
`MolePerCubicMetre`, `All` and `TryGetById`. It reuses existing
`dw.quantities.DimensionVector`, `UnitDefinition`, `ExactRational`,
`Quantity`, and `PureUnitConverter`; no new arithmetic or AURA host policy.

Eight exponents are ordered L, M, T, I, Theta, N, J, Information.
All definitions have `System = Si`, `TransformKind = Linear`,
`ScaleToBase = 1/1` and `OffsetToBase = 0/1`.

| Typed API | Unit ID | Symbol | Name | Exact dimension vector |
| --- | --- | --- | --- | --- |
| `Ampere` | `si.ampere` | A | ampere | 0,0,0,1,0,0,0,0 |
| `Volt` | `si.volt` | V | volt | 2,1,-3,-1,0,0,0,0 |
| `Ohm` | `si.ohm` | Ω | ohm | 2,1,-3,-2,0,0,0,0 |
| `Watt` | `si.watt` | W | watt | 2,1,-3,0,0,0,0,0 |
| `Mole` | `si.mole` | mol | mole | 0,0,0,0,0,1,0,0 |
| `MolePerCubicMetre` | `si.mole-per-cubic-metre` | mol/m³ | mole per cubic metre | -3,0,0,0,0,1,0,0 |

## Preservation and release plan

`InheritedStandardUnitCatalog` remains version `0.2.0` with 58 entries,
and `StandardUnitCatalog` remains `2026-10-exact-v1` with 14 entries.
Neither catalog is extended, renamed or implicitly overridden; all existing
NuGet package versions remain immutable. This run preserves the source
`dw.quantities.standard.csproj` identity (`0.2.0-preview.1`) and its
two original project dependencies.

The target package family is the existing `dw.quantities.standard`, but the
new electrochemical profile requires a **new immutable calendar NuGet version**,
not an edit to an already published archive. The final candidate version
must be selected from the actual private feed and creation-date convention;
NuGet calendar version and CLR profile/assembly identity are distinct.
Exact published dependencies will remain `dw.quantities`
`20260.20261102.1001` and `dw.quantities.expression`
`20260.20261102.1001`.

RS173 qualifies *source-compiled Math tests only*: six individually selectable
unit-definition rows and six independent aggregate cases (catalog identities,
two electrical equations, molar conversion and inverse, both-present
kilogram/mole refusal, incompatible dimensions/transform refusal).
A succeeding RS173 is **not** yet a package-only consumer qualification.

Follow-on bounded DWF runs must create the exact calendar candidate,
qualify a fresh independent binary consumer using PackageReference only,
use a fresh private NuGet cache and explicit local sources, record loaded DLL
hashes and effective local feeds, publish only a qualifying immutable archive
to `NUGET_LOCAL` without overwrite, and independently reread its physical
archive/dependency closure before an AURA handoff. No consumer may rebuild
these six definitions.

## Rights and limitations

Inherited AURA source-reuse rights remain **not established**. This change is
new original Math code using existing Math contracts; it grants no new AURA
redistribution permission, does not change historical rights attestations,
and introduces no public SPDX or license declaration. No public registry,
global NuGet source edit, sibling-repository mutation, AURA host/worker/CLI
integration, formula delivery, C01 closure or release authorization is claimed.
