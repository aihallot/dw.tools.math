# RS402 - Activate M10 readiness GREEN and record project dispositions

RS401 succeeded first-pass at attempt 001 under DWF 0.1.81, result a62933f1f666961600ed2527229429489d0f0423. Its nine preparation validations and native RED completion/GREEN readiness are reconciled in canonical backlog 0.1.174. Activate GREEN only.

The project disposition ledger is generated from actual repository project XML, retaining declared framework/language overrides, project and package references, lock presence and restore policy. Select the 24 solution projects and two maintained tools (discovery bindings and reusable NuGet preflight) for migration qualification. Treat the other 38 NuGet tools as versioned reproduction recipes and preserve the remaining historical consumer/provider/document/sample fixtures using historical checkouts, not silently inheriting future central defaults. This is a conservative proposed operational classification, not a compatibility certification or removal of any tool. Future current consumers and publication tooling require separate qualification.

GREEN remains in progress. Next qualify an isolated actual solution graph, maintained-tool dependencies, targeted fragile tests and actual rollback. No production retarget, package publication or sibling changes.

## Retry 0 preselection correction

The preparation-safe disposition validator refused the generated ledger before allocation (no source mutation), evidence: https://github.com/aihallot/dw.tools.math/commit/c9a55b7d2cd4da8d6433a559056c09249b50ad8f. The original comparison normalized only the file operand's CRLF while comparing it against serializer output; textual line-ending differences are a plausible cause, not established by the available report. The corrected writer normalizes line endings and the reader compares parsed JSON trees in full, retaining exact property values, array membership and order. A true semantic mismatch still fails with the first differing project and field names, or an envelope/roster diagnostic. Preserve all 123 dispositions, generation inputs, scope and validation identities.
