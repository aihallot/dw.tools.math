# Resource policy

Adapted from the AURA contract docs/04_execution/resource-cost-policy.md as consulted on 2026-10-07.

Zero sub-agents by default. At most one when independent review provides justified value; bounded mission, minimal context, one pass.
Before any expensive block: state objective, completion condition, agent/test budget, and stop-loss.
Use one targeted RED and one targeted GREEN per change when relevant. After a post-correction validation failure, allow one evidence-based correction; beyond that, request arbitration.
No general suite unless an explicit gate or demonstrated cross-cutting risk requires it. Gates should not repeat every suite for every subtask.
A required review does not create a perfection loop. Fix blockers and defer optional improvements.
Do not hide overruns behind invented estimates. Report commands, results, agents, and limits; never invent billing cost.
Stop at a coherent checkpoint. Chunks are traceability units, not mandatory interruption points.
Provider research has a corpus, questions, and an exit decision; no massive benchmark by default.
