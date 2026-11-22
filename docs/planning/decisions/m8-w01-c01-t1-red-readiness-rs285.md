# RS285 — Admit one Math Discovery protocol RED readiness chain

## Decision

Only the existing native nodes `M8-W01`, `M8-W01-C01`, `M8-W01-C01-T1`, and `M8-W01-C01-T1-R` move from `not-ready` to `ready`, in that order, using the documented idempotent typed DWF transition. The prerequisite root `M8` is `ready` from RS284; `M7` is `done`. This is **scheduling readiness**, not a RED test execution or retrospective work acceptance.

The target contains one effectively ready descendant leaf: `M8-W01-C01-T1-R` (RED for pinned protocol and conformance). The parallel CLR task T2, GREEN and Verification siblings, Discovery generation phase C02, work package M8-W02, and all M9 nodes remain `not-ready`. The workflow active hierarchy remains null until separately activated.

## Product acceptance and existing evidence

The canonical backlog `docs/planning/backlog.json` version `0.1.124` still lists M8 and its chunks as `planned`. `M8-W01-C01` accepts the thirteen pinned portable `dw.discovery/1.0.0-draft.1` fixtures and the selected CLR signatures; it explicitly refuses unselected helpers, BrokerReady promotion, and invented runtime authorization.

RS131 executed the 13-case corpus (2 accepted, 11 rejected); RS209 independently reconciled exact fixture pins and receipt verdicts. RS279–RS283 provide separate package and portable skill delivery evidence. RS281 succeeded attempt 001 after three pre-attempt refusals (`firstPassSucceeded=false`); RS282–RS284 succeeded first-pass. None of those earlier proofs is reassigned to a newly completed RED/GREEN/Verification subtask.

## Next fresh RED test boundary

After this readiness run succeeds, a separate task run may activate the protocol RED leaf and independently falsify a new bounded admission/refusal scenario, without changing the immutable pinned fixture, schema, historical receipts or library outputs. Candidate cases are: altered protocol schema identity or content digest must be refused; unauthorized BrokerReady or metadata-granted permission must be refused. A valid pinned envelope should remain accepted. Use Math-owned C# validators, deterministic fixtures, no sibling project reference and no generic shell-domain logic. The RED step remains incomplete until DWF records new execution and proper later lifecycle transitions.

## Nonclaims and safeguards

No active workflow pointer is set by RS285. No `done` achievement, new package publication, SDK migration, AURA adoption, broker authorization, public license, product backlog completion or external repository mutation is authorized. The existing 591-node native plan is preserved except for exactly four `ready` target states. Two maintained C# projection validators enforce that frontier; avoid creating a redundant validator.
