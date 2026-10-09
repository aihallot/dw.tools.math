# Exact replay identity and bounded local cache — M2-W02-C03-T1

The `dw.tools.math.composition` library builds an explicit deterministic identity for bounded exact local quantity pipelines. This contract does not create a provider, grant AURA permission, deploy an external cache, or promise numerical cross-platform floating-point reproducibility.

## Versioned key and replay

`ExactReplayRunner.Run(steps, context, optionalCache, cancellationToken)` accepts only the previously qualified bounded `ExactPipelineStep` vocabulary. The versioned `exact-quantity-replay/1-sha256` key hashes a domain-separated, length-framed binary description of the exact sequence, step kinds, canonical rational values, unit identity, scales and dimensions, and five explicitly supplied context labels: assumptions, calculation policy, catalog, tolerance policy, and provider version.

Changing any one of these changes the identity. The provider version is an *opaque identity value*, not proof that the provider is installed, trusted, authorized, or used. The context is supplied by the caller, so equality promises apply only when callers accurately identify their own external policy changes. Unit presentation labels are not semantic input; unit ids/scales/dimensions are.

A matching replay recomputes the same exact arithmetic through the already qualified `ExactQuantityPipeline`. For 5 km / 2 minutes, the result is always 125/3 m/s or exactly 150 km/h under the fixed unit and policy contract.

## Optional cache

`ExactReplayCache` is a caller-owned, volatile, in-process, deterministic insertion-order cache. Capacity is bounded from 1 to 32 entries (default 16); it stores only successfully completed exact results and has an explicit `Clear()`. `CacheHit` is an observation about local reuse, not execution provenance. Without a cache, every call evaluates again. Invalid input and cancellation do not write entries, and cancellation is checked before returning a hit.

## Qualification and remaining work

RS036 records one controlled RED for the missing exact replay identity public contract and executes 14 independent GREEN tests: identical exact replay, identity invalidation by input, assumptions, policy, catalog, tolerance and provider label, changed step order/unit scaling, optional cache hit/no-cache parity, FIFO eviction, invalid results not cached, cancellation, context and step quotas, oversized numerals, and provider independence. Existing composition, IR, foundation, architecture and local NuGet package regressions must also pass.

Only `M2-W02-C03-T1` may close. `T2` remains planned for rigorous partial-result and cache-quota boundaries, collisions, context normalization policies, cross-platform binary64 nonclaims and integration. No disk persistence, remote cache, execution authority, external adoption or floating-point bitwise portability is delivered.
