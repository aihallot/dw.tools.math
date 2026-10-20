# M7 formal interoperability — explicit trusted-base and toolchain deferral

**Research disposition after RS119:** the finite source-owned `Z[x]` polynomial certificate remains reproducibly replayable. **No** Lean, Rocq, SMT provider, proof kernel, theorem-proving runtime or public Math formal API has been admitted.

## Proof/certificate/derivation separation

| Evidence kind or state | Trusted observation | Inference that must be rejected |
|---|---|---|
| Local coefficient certificate | Exact BCL `BigInteger` convolution verifies `[-1,0,1]` for `(x-1)(x+1)=x^2-1` | A general theorem prover or an externally certified proof kernel |
| Algebraic derivation | Explanatory rearrangement or transformation | A validated proof term or accepted trusted-base certificate |
| Bad or truncated complete witness | **invalid-certificate** | Proof success |
| Missing proof, hole, timeout or external failure | **unproved** | Successful theorem or silently assumed lemma |
| Lean/Rocq proof term | No installed checker or witnessed kernel replay | Locally accepted proof |
| SMT solver proof/certificate | No pinned solver, proof format or independently replayed kernel | Solver response or unknown/timeout equals certificate |

The actual local trusted base is the immutable fixture JSON, tiny read-only C# checker and installed .NET `BigInteger` runtime. It is **not** formalized or kernel-certified. There is no external axiomatic system, proof language or arbitrarily large verified theorem corpus.

## Candidate toolchain / legality / platform matrix

| Candidate | Missing trust and license evidence | Current disposition |
|---|---|---|
| Lean | Exact source/commit/version, kernel proof-term replay, license/notices/rights, supported TFM/RID, reproducible install | **Defer** |
| Rocq | Exact source/commit/version, independent proof-kernel check, redistribution rights, offline packaging and target platform | **Defer** |
| SMT with proof certificates | Exact solver and format, independent proof checker trusted base, version/source/license/RID and negative tests | **Defer** |
| Local fixed `Z[x]` BigInteger verifier | Specific source SHA and exact bounded coefficient corpus retained | **Research only** — no public library or new consumer |

The research evidence is intentionally **not** a verified vendor-comparison benchmark, provider procurement recommendation or a permission to download solver binaries.

## Three re-entry triggers — each required

1. **Actual consumer and proof language:** identify a named recipient, algebraic/decision problem, assumptions and exact proof/certificate subset not already covered by qualified Math exact computation.
2. **Kernel trust, safety and budgets:** separate trusted checker/kernel from untrusted generator; formalize error/hole/unproved/timeout, no partial proof, bounded certificate and CPU/memory/deadline/cancellation policy, and independent counterexamples.
3. **Provenance and source-free approval:** pin source/build/version SHA, full license/transitive/native notices, target .NET platform and RID, offline replay and rollback, isolated source-free consumer and approval of Math and the actual owning client.

**If any trigger is missing, remain deferred.** Trigger satisfaction still requires a separate DWF admission decision; neither T1 nor T2 automatically creates a formal product.

## Negative product claims and succession

- **No** downloaded Lean/Rocq/SMT executable, owned prover implementation, public theorem/SMT API, `dw.discovery` skill, package build, source-free consumer, AURA execution integration or public release is qualified.
- A pass for one small integer polynomial identity is a *finite certificate verification*, not a general symbolic decision procedure or proof assistant kernel.
- `M7-W02-C01` closes only as a research spike. `M7-W02` remains in progress; `M7-W02-C02` is a separate owner/completion inventory; M7's release gate stays open.
- Future formal adoption needs explicit owner approval and independent source/rights/runtime checks; no sibling repo is modified by this Math run.
