# M6 scientific families — bounded source and oracle adoption matrix

**Authority:** delivered DWF evidence through RS102, not a published
NuGet product catalogue. Provider is the local, BCL-only
`dw.tools.math.scientific`; latest observed scientific archive is
`1.1.0-preview.14` (37,270 bytes; SHA-256
`2de3e9f536ced58e28d1558ff7c4692f367b72d5efaa1989d56ffa8ec07a31cf`).
The local archive is neither a public release nor evidence of other OS
qualification. Old preview versions below are historical family
**admission pins**, not newly reissued packages.

| Family | Qualified evidence and admitted preview | Pinned source SHA-256 | Independent oracle and domain limits |
|---|---|---|---|
| Exact scientific integers | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` / `1.1.0-preview.2` | `d3dd0a01a76fe42c9d38a35dca4cdc18134c56c4728762fdbaa2adb41708e9db` | `Binomial(5,2)=10`; bounded exact/signed integers and intermediate bit-size refusal, no arbitrary unbounded result guarantee |
| Managed special functions | `docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json` / `1.1.0-preview.4` | `f9d8067dfcb1112ff997c15529fb3c866f2aba1b70b7c7eff064dc3433ecf467` | `Gamma(5)=24`; finite angles/Gamma/complex-branch domain with typed refusals, no certified error theorem |
| Finite Euclidean geometry | `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json` / `1.1.0-preview.6` | `7355c99cb2bdcb72a3cdb2880136e8bd4643645ee578608582fbc76bf68e1fa6` | `Norm(3,4)=5`, project `(3,4)` on x = `(3,0)`; 2D/3D finite Cartesian caller units, no geodesy or implicit frame |
| Direct discrete signals | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` / `1.1.0-preview.8` | `da135763f464d65fc5f71d1bb09689259ea480ac26bde4b976db5ac6ee450ce3` | `DFT([1,0,0,0])=[1,1,1,1]`, `[1,2]*[1,1]=[1,3,2]`; length 1–32, negative forward sign, linear noncircular convolution, no FFT/alias-free proof |
| Uniform discrete series | `docs/planning/evidence/M6-W02-C03-S03-T2-boundaries-qualified.json` / `1.1.0-preview.10` | `908f18668b23ea788e5f1be531bf8a3bb6108c048a1e663f6c1927bdd9ab5d10` | `difference([1,3,6])=[2,3]`, moving mean 2 = `[2,4.5]`; explicit positive interval and ordered finite times, no forecast/interpolation/resampling |
| Cross-family scientific aggregation | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` / `1.1.0-preview.12` | `634f5e701d92c9820d910b9a97f898c95c14e6abeb16ac6a3be3347d9d8b00e4` | Seven independently admitted geometry/signal/series outcomes all-or-none; explicit caller units, discrete sampling, linear topology, no physical compatibility certification |
| Pure financial arithmetic | `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json` / `1.1.0-preview.14` | `df2ce1460020743df127c19377e99b85b65a4e1fd57104db54bf833ea3357b60` | Present value `100/(1+0.1)=1000/11`, IRR `[-100,110]=0.1`; effective rate/bracket, ACT/365 Fixed or ACT/360 actual days, no market input/advice |

## Source and consumer pin interpretation

Each line's **source SHA-256 belongs to the named historical
qualification evidence**, and the source path is the evidence
`source` field. Check the actual file bytes before any reuse or
export; do not silently treat a Git commit blob SHA as the file
SHA-256. DWF RS103 verifies the admitted evidence pin fields and
source-file availability, without claiming a new scientific test.

The latest local preview.14 qualification contains **seven** matching
isolated consumer receipts and proves those exact consumer tests
against that archive (dated finance, pure finance, C03 boundary,
C03 direct aggregate, S03 uniform audit, S02 direct signal audit,
C02 special functions). Earlier distinct geometry/exact contract
admissions are retained through scientific tests; do not claim that
an omitted preview.14 consumer suite was executed separately.

A downstream adopter must pin exact source revision, rules and
version; verify package SHA/length and independent oracles for its
selected families; retain binary64 input budgets and typed nonpartial
refusals; and independently qualify its actual target OS/runtime.
Provider migration requires owner approval, semver and notices review,
not merely documentation status.

## Explicit exclusions

No FFT, numerical error theorem, geographic distance, implicit unit
conversion, market prediction, recommendation, irregular-time forecast,
network-access provider, public NuGet publication or automatic Decision,
AURA, MCDM or `dw.discovery` adoption is certified here.
