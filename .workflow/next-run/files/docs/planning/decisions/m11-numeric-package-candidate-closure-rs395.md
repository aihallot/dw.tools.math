# RS395 - Close numeric candidate qualification

RS394 succeeded at first pass, allocated attempt 001, under stable DWF 0.1.80, result 67bb25b0f6b1bc0de5fccd58b870a165407c70ca. Nine preparation checks and the post-attempt independent binary qualification passed. Preserve the RS392 attempt-001 failure and corrected attempt-002 lineage.

Measured independent readback and consumer markers from RS394:

`MATH-RS394-READBACK|id=dw.quantities|version=20260.20261127.1001|archiveSha256=1795d4c74ac29f20930b4caacb9ca99ee397520ffefe7d16350c869c43a8b30d|dllSha256=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|PASS`

`MATH-RS394-READBACK|id=dw.tools.math.statistics|version=20260.20261127.1002|archiveSha256=cd29b2d8592f65e158f7401f363cc7903fbe69b18b28f787ad3f30d0d0d9bdc8|dllSha256=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|PASS`

`MATH-RS394-READBACK|id=dw.quantities|version=20260.20261102.1001|archiveSha256=e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d|dllSha256=c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4|PASS`

`MATH-RS394-READBACK|id=dw.tools.math.statistics|version=20260.20261120.1001|archiveSha256=feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1|dllSha256=684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea|PASS`

`MATH-RS394-CONSUMER|mode=candidate|tfm=net10.0|sdk=10.0.404|lockSha256=cb140f47caa53088d16189fe7692adfde477eab56a37b92349fec8ceedb67498|statisticsDll=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|quantitiesDll=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|PASS`

`MATH-RS394-CONSUMER|mode=candidate|tfm=net11.0|sdk=11.0.100|lockSha256=944ed2abe6502ae1b33d9b935eb625b97b82d8bdbea1d690bde76c160bf1e66b|statisticsDll=134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3|quantitiesDll=e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed|PASS`

`MATH-RS394-CONSUMER|mode=rollback|tfm=net10.0|sdk=10.0.404|lockSha256=d6a5a88982d1996a2f7861591427079cfef156beea30b3419a89f977d3099379|statisticsDll=684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea|quantitiesDll=c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4|PASS`

`MATH-RS394-CONSUMER|mode=rollback|tfm=net11.0|sdk=11.0.100|lockSha256=4748bfc203a394cb0b1af60439576646504f9ac5eead4c18b30cb92abb2ed921|statisticsDll=684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea|quantitiesDll=c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4|PASS`

`MATH-RS394-VERIFY|archives=4|consumers=4|candidate=PASS|rollbackRecipe=PASS|rebuild=false|publication=false|PASS`

The four package-only consumers passed the twelve historical M9 checks. The candidate pair passed the M11 checks and the historical pair passed the rollback surface and rational golden checks. This qualifies the exact local candidate archives and a fresh-consumer rollback recipe on Windows net10/net11 only, not a production rollback, publication, downstream adoption or other OS/RID.

Reconcile canonical verification activation into backlog 0.1.168, close native C04 verification, task and phase; reconcile canonical completion after the durable RS395 result. Close remaining work package/milestone and refresh the M10 cutover baseline in subsequent bounded work. Keep the historical and candidate archives intact, no reconstruction or publication, no production framework change. Structural corroboration only; no repetition of product or binary tests.
