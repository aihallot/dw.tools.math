# M7 C01 precision/interval adoption gate — deferred, not delivered

**Outcome:** retain existing Math exact/binary64 contracts; defer new variable-precision/interval provider and public API. This is a planning and owner-gated decision, **not** an implementation or public release.

## Historical exact witnesses and accepted baseline

- `docs/planning/evidence/M7-W01-C01-T1-qualified.json`: RS106 proves finite exact-integer cancellation reference `1` against binary64 `0`, and a one-constant rational enclosure `BitDecrement(0.1) < 1/10 < 0.1`.
- `docs/planning/ValidateM7PrecisionAdmission.cs` supplies two independent local BCL checks. The same script is observed again before the T2 product transition; its execution is not an interval library qualification.
- `dw.quantities.ExactRational` and `ExactBinaryNumber` already preserve bounded exact rational and IEEE binary-value identity. M6's `dw.tools.math.analysis` / `dw.tools.math.scientific` remain qualified within their own finite limits, with historical **local-only** NuGet evidence.
- No provider packages were installed, no rights transfer or licenses selected, and no unrelated repository was mutated by this documentary gate.

## Not-admitted candidate matrix

| Option | Disposition | Why not admitted as a new provider now |
|---|---|---|
| Existing BCL `BigInteger` / Math exact rationals | **Retain existing** | Exact references already available; do not invent variable-precision transcendental or directed-rounded interval semantics |
| Fixed .NET `decimal` | **Do not promote** | Fixed exponent and precision are not arbitrary precision; overflow and rounding persist |
| Third-party managed high-precision real | **Defer** | No named required consumer, provider identity/license, independent oracles or qualified net10.0 consumer |
| Third-party managed interval evaluator | **Defer** | No operation-by-operation outward-rounding proof, target runtime, refused-domain corpus or rights/notices review |
| Native MPFR/Arb-class backend | **Defer** | No version/commit, RID/native asset/FFI boundary, transitive redistribution proof, runtime installer or rollback |

These are research *categories*, not current vendor rankings or verified package claims.

## Three review triggers — all required before admission

1. **Consumer/operation need:** a documented Math owner and distinct caller specify a finite domain, operation set, precision objective and failure mode not handled by current exact or bounded binary64 APIs.
2. **Provider provenance and cost:** exact source/nuget version or commit, SHA-256, license, transitive/native notices, accepted rights, TFM/RID, offline restore/install and rollback, maintenance and resource budget.
3. **Independent mathematical acceptance:** adversarial exact oracles and interval inclusion tests per operation/rounding mode, nonfinite inputs and domain refusal, cancellation, time/bit/iteration budgets, no-partial-result statuses and an isolated source-free consumer.

If any trigger is missing: **remain deferred**. Owner acceptance is a separate explicit act, never inferred from the review trigger alone. Keep all status flags for `external_provider_adopted`, `public_precision_api_delivered`, `externally_published` and `automatic_cutover` false.

## Explicit nonclaims

A larger precision setting does not certify floating-point error or measured physical uncertainty. One included rational constant does not establish a reusable interval enclosure theorem. T2 document completion closes the research *spike*, not the M7 milestone, not the M7-W01 work package, not an advanced project/package, and not an external provider or license qualification.

**Next roadmap unit:** `M7-W01-C02-T1` tensors and advanced structures, subject to its own scope/oracles and no implicit precision-provider dependency.
