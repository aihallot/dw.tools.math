# RS393 - Close isolated numeric package GREEN and ready verification

RS392 succeeded on attempt 002 in correction mode from attempt 001, under DWF 0.1.80. Result: a512d93cd6878a88143213d71e6218dcb7d78157. The first pass failed at pack due to implicit CLR versions derived from the calendar package version; correction 52bcfde76e05537995da56dcf9fa4cc6c4eab563 separated AssemblyVersion and FileVersion at 1.0.0.0 without changing package identities. Preserve the failure lineage and the nine preparation checks plus one successful post-attempt binary validation.

The following measured markers are copied from the durable attempt-002 report (not newly observed by this closure):

`MATH-RS392-ARCHIVE|id=dw.quantities|version=20260.20261127.1001|archiveSha256=1795d4c74ac29f20930b4caacb9ca99ee397520ffefe7d16350c869c43a8b30d|dllSha256=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|tfm=net10.0|PASS`

`MATH-RS392-ARCHIVE|id=dw.tools.math.statistics|version=20260.20261127.1002|archiveSha256=cd29b2d8592f65e158f7401f363cc7903fbe69b18b28f787ad3f30d0d0d9bdc8|dllSha256=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|tfm=net10.0|PASS`

`MATH-RS392-CONSUMER|tfm=net10.0|sdk=10.0.404|lockSha256=cb140f47caa53088d16189fe7692adfde477eab56a37b92349fec8ceedb67498|statisticsDll=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|quantitiesDll=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|PASS`

`MATH-RS392-CONSUMER|tfm=net11.0|sdk=11.0.100|lockSha256=944ed2abe6502ae1b33d9b935eb625b97b82d8bdbea1d690bde76c160bf1e66b|statisticsDll=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|quantitiesDll=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|PASS`

`MATH-RS392-CANDIDATES|root=D:\data.dev\github\ai@hallot.net\dw.tools.math\build\artifacts\m11-candidates-1aa0ad9d4f7e|oldArchivesUnchanged=true|publication=false|windowsNet10=PASS|windowsNet11=PASS|PASS`

Both Windows consumers also passed all twelve M9 checks and M11 scale, moments, quantiles and legacy checks. The exact archived bytes remain local in the recorded candidate feed. Do not rebuild them under the same candidate identities. Independent verification must reopen the actual archives and verify their hashes and exact dependencies, restore from the candidate pair and then the old immutable pair in isolated package-only consumers with locked graphs and loaded-DLL identity checks. No publication or production target change; no Linux or all-RID claim.

Reconcile canonical activation into backlog 0.1.166, close only native GREEN and ready independent verification. Reuse the measured evidence with structural validation only. M10 cutover remains gated by C04.
