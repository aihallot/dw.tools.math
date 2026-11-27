# RS384 - Larger-sample quantile RED

RS383 closed the streaming moments source phase, first pass under DWF 0.1.80, result 11fa0e1f0cc5f53dcd74e666ffe678b3174e5260. Reconcile its canonical completion and activate quantile RED.

Existing QuantileStatistics64 already implements Type7 and NearestRank separately for finite binary64 and exact rational samples. Both three-argument entry points take at most 4097 items and reject a count above 4096. Exact operand/intermediate limits are 4096 bits. Preserve these existing entry points and their refusal semantics. Reuse the existing algorithms rather than adding a parallel quantile engine.

The compiled oracle checks independent closed-form expectations for descending consecutive integer samples at sizes 1, 4096, 4097 and 1000000, both conventions and endpoints, verifies actual legacy outputs/refusal, and establishes missing additive overloads with an explicit integer observation budget. The GREEN fixture is designed to exercise the same reference against the future overloads; exact rational larger-sample checks are bounded to 4097 small operands. An explicit finite ceiling of at least 1000000 and separate exact observation/operand budgets must be qualified in GREEN; no unbounded allocation or implicit approximation is allowed. Additional ties, extreme interpolation/refusal, and enumeration-budget tests remain GREEN/verification scope.

No production source or packages change in RED. No release, cross-repository consumer or M10 cutover claim.
