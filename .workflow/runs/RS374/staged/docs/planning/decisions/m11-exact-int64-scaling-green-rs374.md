# RS374 - Implement exact Int64 scaling GREEN

RS373 succeeded first-pass, attempt 001, result e392ebca72cf817e6786ae30f7fb9324569b3c23. Its durable completion closes the RED leaf and readies GREEN. Reconcile that evidence in canonical backlog 0.1.147 and activate the GREEN continuation only.

Add the proposed public ExactRationalBudget.TryScaleToInt64 member and immutable result/status types in the existing source file. Reuse the existing budget guards, normalize through ExactRational, cancel the denominator before multiplication, and check the exact result budget before the signed Int64 range. Invalid bit budgets throw; other refusals carry a null value. Existing Add, Multiply, and rational semantics are unchanged. No production SDK or package version changes, publication, Decision adoption, or migration.

Use the unchanged RS372 independent oracle in --green mode (25 fixed cases, 2160 generated comparisons, invalid-budget exception and historical arithmetic), then focused maintained Quantities tests and the relevant locked project regression. Only local execution can establish GREEN qualification. Closure follows durable successful evidence; no completion achievement is claimed by this activation run.
