# M10 post-cutover regression (RS419)

RS418 closed native RED and readied GREEN first-pass (attempt 001, DWF 0.1.81, eleven structural validations). RS419 reconciles its durable RED completion to backlog 0.1.186 and activates native GREEN.

At this migration trust boundary, the regression recipe derives its focused filters, lock restore, Release build and TRX checks from the qualified RS413 production cutover probe. It first runs the 35 known fragile tests, then unfiltered tests for each of the eleven test projects in the actual 24-project solution. Each project must produce a nonempty, entirely passing TRX. The report records SDK, actual runtime identifier, target assembly hashes, test counts and TRX hashes. No full regression starts if focused validation fails. No package is reserved or published.

The durable validation report, not this decision, determines whether the regression gate passed. All candidate archives, independent consumers, platform-specific consumer execution and archive/rollback gates remain unqualified. The RED ledger is immutable. No product source or historical archive changes are authorized.
