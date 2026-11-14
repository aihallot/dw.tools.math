# Math Discovery — navigation

Selected Math capability metadata from `docs/discovery/qualification/m8-discovery-manifest.json`.

- [Capability signatures, parameters and numerical limits](selected-capabilities.md)
- [Separated protocol, generator, bundle and package versions](skill-version.json)
- [Runnable source consumer examples and limitations](examples.md)

The entries below are intentionally selected `AgentCallable` surfaces, not broker authorization.

## quantities
- `math.quantity.exact-pipeline.evaluate` — Exact bounded quantity-pipeline outcome.

## replay
- `math.replay.try-evaluate` — Typed replay attempt with explicit incomplete outcomes.

## ir
- `math.ir.canonical.encode` — Canonical math-ir/1 JSON for admitted bounded IR nodes.
- `math.ir.hash.semantic` — Structural identity, not algebraic equivalence.

Inspect without invoking: `dotnet run --file docs/discovery/qualification/DescribeM8Discovery.cs -- --list` or `--explain <selected-capability-id>`.

Runtime permission and broker routing are external; none of these capabilities is `BrokerReady`.
