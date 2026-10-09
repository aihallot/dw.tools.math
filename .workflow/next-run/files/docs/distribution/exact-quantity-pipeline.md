# Exact typed composition pipeline — M2-W02-C02-T1

This phase builds on the qualified `math-composition/1` result and capability contracts, without introducing AURA, a provider registry, symbolic solvers, or an unrestricted expression runtime.

## One engine: direct and fluent

The public `ExactQuantityPipeline.Evaluate(IEnumerable<ExactPipelineStep>)` is the single evaluator. The immutable fluent `ExactQuantityPipeline.From(value, unit).ConvertTo(unit).DivideBy(value, unit).ConvertTo(unit).Evaluate()` constructs exactly the same steps and invokes that evaluator.

The closed step vocabulary is `Start`, `Convert`, `Divide`, and `Add`. Every source/unit is explicitly injected as `dw.quantities.UnitDefinition`; no implicit culture, global unit catalog or provider lookup is performed. Quantities are stored in their canonical base units and represented as exact `ExactRational` values; display conversion is applied only to the explicitly requested final compatible unit.

Example: `5 km` converted to metres and divided by `2 min` yields `125/3 m/s`, or exactly `150 km/h`. Direct steps and fluent operations must agree on dimension, rational quantity, final presentation and step history. An attempted mass + time addition is rejected in the full pipeline pre-validation pass before any arithmetic.

## Explicit limits

A chain has at most 16 steps. Each step's dimensions and supported linear/non-affine units are validated before any quantity math. Zero divisors, incompatible dimensions, bad step order, affine temperatures and oversized exact numeral components are refused. Magnitude is checked again after each exact operation; the implementation does not silently switch to binary floating point. The recorded step-kind history is bounded, immutable and not an external execution trace.

Controlled RED verifies the absence of the public shared evaluator before installing the source. Independent GREEN tests establish direct/fluent parity, the 5 km / 2 min -> 150 km/h acceptance, dimensional rejection, division by zero, immutable fluent appends, conversion precision, affine refusal, order and resource limits. Existing composition/IR/foundation regressions and locked NuGet packaging are separately re-run.

This delivers `M2-W02-C02-T1` only. `T2` remains planned for stronger operation/resource/cancellation/step provenance boundaries and integration. Neither the work package nor phase closes in this run.
