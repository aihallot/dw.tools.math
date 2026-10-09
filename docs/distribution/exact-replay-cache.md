# Exact replay identity and bounded local cache — M2-W02-C03

`dw.tools.math.composition` provides exact local computation replay with caller-supplied semantic identity. This is not a provider implementation, an AURA permission system, or a cross-platform floating-point reproducibility guarantee.

## T1 — reproducible replay and optional in-memory cache

`ExactReplayRunner.Run` hashes length-framed exact step kinds, canonical rationals, unit ids/scales/dimensions, and explicit assumption, policy, catalog, tolerance and provider labels under `exact-quantity-replay/1-sha256`. The caller is responsible for supplying meaningful, current versions; changing any version or the exact input changes the key. Unknown provider labels are opaque text, never evidence of installation or authorization.

`ExactReplayCache` is optional, caller-owned, volatile, first-in-first-out, and limited to 1–32 successfully computed results. A cache hit reuses the exact result, not a remote computation. Without a cache the pipeline evaluates again.

## T2 — nonfinal outcomes, cache integrity and bounds

`ExactReplayRunner.TryRun` is an optional typed facade over the *same* `Run` path. Its outcome status is one of `Exact`, `Unsupported`, `BudgetExceeded`, or `Cancelled`. `Exact` carries exactly one completed receipt; all other outcomes contain a short reason and **no** receipt, no final numerical value, no fictitious partial computation. Unsupported dimension/step requests, resource bound excess, and cooperative cancellation are explicitly distinguishable. The existing `Run` API retains its exception behavior.

Cancelled or rejected attempts are never inserted into the cache, cannot replace a valid result, and are never promoted to success merely because some intermediate work occurred. The 32-entry quota applies to successful completed entries; eviction is insertion-order FIFO. A caller-owned cache's memory usage is not a fixed byte quota and scheduling cannot forcibly interrupt arbitrary blocking enumerators or uninterruptible arbitrary-precision math.

A controlled RED checks for the previously missing public nonfinal-status facade. Thirteen independent GREEN boundary tests exercise completed versus absent final values, unsuitable dimensions, zero divisor, exact-number and step limits, exponent overflow, cancellation, eviction at 32 entries, failed-attempt cache integrity, version changes, and absence of external authorization. T1 contract tests, composition/IR/foundation regressions, locked build, architecture verification and NuGet manifest checks remain mandatory.

## Qualification limits

Only the exact rational and zero-offset linear pipeline is replayable. No bitwise binary64 portability is promised; approximate outcomes are not cached as exact values. No remote/disk cache, provider invocation, AURA authority, distributed scheduler or external adoption is claimed. A successful RS037 closes `M2-W02-C03` and `M2-W02`, **not** milestone M2; the M2 gate and evidence must be qualified independently.
