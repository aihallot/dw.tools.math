# M9 — Statistics bounded integer private publication readback

Status: **awaiting independent RS273 DWF qualification**. The owner reported successful execution of the guarded `--publish` controller after RS272, with the following local output:

`MATH-M9-STATISTICS-PUBLISHED|id=dw.tools.math.statistics|version=20260.20261120.1001|sha256=feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1|dllSha256=684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea|feed=NUGET_LOCAL|independentNet11=PASS`

This is operator-reported evidence, **not yet a durable independent workflow qualification**. RS273 checks the actual physical shared-feed archive, the published embedded DLL hash, the exact `dw.quantities 20260.20261102.1001` dependency, old immutable Statistics archive, locked .NET 11 package-only restore, and the independent twelve-check consumer.

No publication operation is included in RS273. The original controller must not be run with `--publish` again: its immutable target-version guard forbids republishing. A successful RS273 report is the release evidence for the Assessa owning agent; Math does not modify Assessa or claim its `GENERATE-001 / EG-A-01 / T002` qualification.

## Methodology improvement

The owner requested reducing exceptional manual publication commands. DWF guidance permits post-attempt validation profiles distinct from preparation-safe checks, but a safe external release step additionally needs an exactly-once/idempotent receipt, recovery after partial publication, authorization and durable proof. This boundary should be designed deliberately in `dw.tools.workflow` by its owning agent; it is not achieved by inserting an irreversible `--publish` into a disposable/preselection validation. No sibling-repository changes are made by RS273.
