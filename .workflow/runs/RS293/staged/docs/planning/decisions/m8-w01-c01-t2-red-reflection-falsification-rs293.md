# RS293 — Select CLR binding RED: coherent manifest forgeries versus real published methods

## Evidence and exact progress

RS292 succeeded at attempt 001, first-pass, and completed both the T1 Verification leaf and parent protocol task. It admitted M8-W01-C01-T2 and its RED leaf to ready. RS293 uses the typed target-reentrant ProjectPlan.ActivateReadyContinuation operation for M8-W01-C01-T2-R; the active prefix M8 → W01 → C01 stays in-progress and the only two new states are T2/T2-R ready → in-progress. No completion achievements are declared.

## New falsifiable RED test

The Math-owned ProbeM8SelectedClrReflectionRed.cs checks three immutable NUGET_LOCAL packages: dw.tools.math.composition 20260.20261107.1001, dw.tools.math.ir 20260.20261104.1001 and dw.quantities 20260.20261102.1001, with their existing archive and embedded DLL SHA-256 pins. It extracts the DLLs to a fresh ignored build/artifacts directory, loads the actual compiled assemblies into an isolated AssemblyLoadContext and verifies all four approved public static method signatures via System.Reflection, including exact CLR parameters, return type and declaring assembly version.

Three test-only forged Math manifests use a nonexistent member, a wrong first parameter type, and a wrong return type. Each forgery recomputes its complete canonical signature and fingerprint. The metadata-only RS287 gate must continue accepting these internally consistent descriptions, while reflection against the real published DLL must refuse them all. A separate unauthorized execution-grant mutation must still fail the original RS287 gate. No actual method invocation, new restore, package overwrite or release publication occurs.

The observed distinction is intentional: RS287 guarantees metadata self-consistency and never claimed to prove the presence of a real CLR method. RS293 establishes a new selected-CLR runtime admission boundary for a subsequent GREEN implementation. Historical source-only binding flags remain historical truth; RS279/282 consumer results are not reassigned to this RED subtask.

## Limits and continuation

T2-GREEN and T2-Verification, unrelated M8 nodes and M9 remain not-ready; T1 stays done and M8/W01/C01 stay in-progress. Canonical product backlog 0.1.124 remains planned. No broker permission, AURA integration, public NuGet release, signed provenance or general Math coverage is implied. A future GREEN run must qualify an independent reflection-backed gate that accepts the four actual selected methods and refuses coherent forged metadata.
