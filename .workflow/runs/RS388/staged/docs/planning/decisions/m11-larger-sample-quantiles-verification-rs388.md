# RS388 - Independently verify bounded quantile contracts

RS387 succeeded at first pass, allocated attempt 001, under DWF 0.1.80. Result: c4e9071e2d42619362025d99101d78c1b109fc94. Reconcile canonical GREEN closure into backlog 0.1.161 and activate only independent verification.

Complement the delivered million-observation oracle and targeted regression with four maintained-project tests: hand-derived quartiles for interleaved nonuniform repeated values above the historical bound; rational probabilities and exact interpolation including a 5000-observation sample; typed refusal of an oversized exact interpolation delta while rank and endpoints remain valid; and enumerator disposal and consumption on success, excess and invalid probability. Preserve the existing production source and tests.

The finite path remains binary64 and preserves its historical overflow refusal. The exact path preserves the existing 4096-bit operand and checked intermediate limits; it does not promise bounded cost for arbitrary user-supplied enumeration or a measured allocation ceiling. No new package or consumer qualification, approximation, M10 cutover or production changes are admitted. Only the new focused verification tests and structural evidence checks are required.
