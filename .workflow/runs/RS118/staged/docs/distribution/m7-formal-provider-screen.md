# M7-W02 — proof and certificate interoperability, candidate-only screening

**T1 qualification scope:** a bounded local `Z[x]` polynomial certificate replay, source-pinned under `docs/planning/fixtures/m7-polynomial-identity-certificate.json`. Candidate proof systems below are **not** installed, licensed, trusted, consumed or accepted by Math.

## Machine-readable reference and falsifiable outcomes

| Research input | Expected exact observation | Interpretation |
|---|---|---|
| LHS `[-1,1] * [1,1]` | `[-1,0,1]` | Exact integer polynomial coefficient convolution |
| RHS and certificate vector | Both `[-1,0,1]` | Independently verified single `(x-1)(x+1)=x^2-1` identity in `Z[x]` |
| Altered coefficient | `[-1,1,1]` | **invalid-certificate**; never a proof |
| Missing coefficient or missing witness | Incomplete polynomial/certificate | **invalid-certificate** or **unproved**, no permissive coercion |
| Hole / TODO / unfilled proof | No complete certificate | **unproved**, irrespective of example values |
| Timeout or provider failure | No independently verified artifact | **unproved**; no automatic retry as a success |
| Sample evaluations | E.g. test `x=-2,0,2` | Secondary illustrative observations; samples never replace exact certificate check |

The read-only C# `docs/planning/ValidateM7FormalCertificate.cs` computes the coefficient convolution with BCL `BigInteger` and requires equality to the fixture's certificate and RHS. The trusted base is **the small local checker plus the .NET integer implementation**. This is a finite certificate for one identity; **not a theorem-prover kernel**.

## Candidate formal technologies and provenance gate

| Technology class | Evidence if later selected | Current disposition |
|---|---|---|
| Lean proof kernel | Exact version/source, kernel-trusted proof term and reproducible offline verifier | **Deferred**; no executable/version/license qualified |
| Rocq proof kernel | Exact version/source, accepted proof term, kernel and redistribution rights | **Deferred**; no executable/version/license qualified |
| SMT solver with certificates | Named solver and proof format, separately validated certificate checker and declared assumptions | **Deferred**; unknown/timeout/solver-claim alone is not proof |
| Local fixed polynomial coefficient checker | Exact finite algebraic identity with independently replayed coefficient vector | **Research only**, no external kernel, no public API |

## Three admission triggers — all required for a future provider

1. **Actual proof consumer/domain:** a named Math or consuming app use case, exact proof language or certificate subset, stated assumptions/term semantics, and reason existing exact algebra is insufficient.
2. **Trusted base, errors and bounded resources:** pinned proof checker, explicit trusted axioms/operations and certificate format, assumptions, no holes, failure/timeout/unproved statuses, finite CPU/memory/input-output budgets and independently falsifying examples.
3. **Immutable provider/source-free receipt:** exact source/package commit SHA/version, copyright/license/transitive notices/redistribution rights, target .NET platform/RID and offline installation, independent certificate replay and isolated source-free Math consumer, with explicit owner approval.

**If any trigger is missing, remain deferred.** Merely naming a proof assistant or storing a derivation does not admit a formal provider or new NuGet package.

## Separation of ownership and next scope

Math could someday expose a qualified mathematical certificate data contract; it does not own a generic proof engine, agent execution, AURA scheduler, provider process broker, GUI or public theorem-prover integration without a separate approved contract.

`M7-W02-C01-T2` must establish an actual trusted base and provider/version/license/toolchain gate **or explicitly defer** further proof integration with a documented re-entry condition. C02 coverage review and the M7 1.2.0-candidate release gate remain open. No public theorem proof service or formal package is currently delivered.
