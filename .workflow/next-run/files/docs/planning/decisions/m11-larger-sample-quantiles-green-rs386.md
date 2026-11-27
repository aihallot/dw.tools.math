# RS386 - Implement bounded larger-sample quantile GREEN

RS385 succeeded first-pass under DWF 0.1.80, result 92a105fcaaeba0c30ce03d3038e1259d00eb17f6. Reconcile canonical RED completion into backlog 0.1.159 and activate GREEN.

Add four-argument overloads to the existing QuantileStatistics64 engine, with maxObservations in 1..1000000 for finite binary64 and 1..8192 for exact rational. These are explicit resource ceilings, not changed defaults. Three-argument overloads delegate with the historical 4096 observation bound; exact operand/intermediate limits remain 4096 bits. Validate budgets before enumeration, consume no more than budget plus one element to detect excess, and retain historical finite interpolation overflow refusal and exact rational arithmetic. Materialization and sorting use O(n) sample storage and O(n log n) comparisons; no allocation measurement or arbitrary input cost guarantee is asserted.

Run the delivered independent GREEN oracle (including million-observation finite cases and exact 4097 cases), then maintained quantile tests covering both ceilings, bounded enumeration, legacy refusal, ties, endpoints and extreme arithmetic. Do not rerun unrelated project suites. No package publication, independent binary consumer, implicit approximation, parallel engine or M10 cutover is admitted.
