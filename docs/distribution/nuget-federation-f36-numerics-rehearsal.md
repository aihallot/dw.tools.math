# RS169 — Numerics immutable publisher rehearsal on a disposable NuGet feed

## Source authority and exact predecessors

RS168 succeeded in attempt 003 at GitHub `main` commit `494a3ec2e2dc36dfa52f6299c68dfd814cd76008`, after two allocated attempt failures and two pre-attempt refusals. F35 qualified the unpublished original Numerics candidate `dw.tools.math.numerics [20260.20261109.1001]` with an archive SHA-256 of `f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d` and DLL SHA-256 `77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4`. All fifteen source-free binary probes passed. The source project remains at `0.4.0-preview.4` with one direct IR project dependency and transitive Quantities.

RS169 preserves that original evidence. It does **not** assume a newly rebuilt archive has the original F35 bytes: its archive SHA-256 is recorded separately and `reconstructionIsOriginalArchiveProof` is a truthful observed comparison, not a publication authorization.

## Rehearsal and independent consumer

The Math-owned `dw.tools.math.nuget-numerics-rehearsal` controller independently links the same three F35-authorized source files, verifies each exact original source SHA-256 and source project/lockfile fingerprints, and packs a fresh temporary `net10.0` candidate with one exact IR PackageReference. It explicitly does **not** relaunch the F35 candidate controller: the earlier F35 cache may retain a package with the same calendar identity but different rebuilt DLL bytes. Each RS169 invocation uses a fresh, empty NuGet package cache, build tree, candidate directory and disposable feed. A real ZIP/nuspec readback checks package ID/version, its sole exact IR dependency and the built DLL SHA-256. The previously accepted eleven F35 internal command receipts remain immutable predecessor evidence, not commands falsely claimed to have run again.

Using the pinned `Dw.Tools.NuGet [20260.20261026.1010]` reader, RS169 prepares, publishes and physically reads back the exact newly built archive in a **new disposable feed**, never the shared `NUGET_LOCAL`. It independently rejects same-version collision, falsified archive digest and cancellation. A new source-free consumer restores by exact package identity from the disposable feed and loads the Numerics assembly to run the existing fifteen compiled probes, including loaded DLL SHA, LU/solve, and IR transport.

Seven actual controller command receipts (producer restore/build/pack, consumer restore/build/run, SDK) and four DWF payload receipts are retained. The F35 internal eleven-command evidence is independently read from the prior successful receipt, not replayed. Physical IR `20260.20261104.1001` and transitive Quantities `20260.20261102.1001` package hashes are checked before and after the consumer.

## Explicit limits

Shared-feed publication, original F35 candidate replacement, public registry delivery, source-version changes, external adoption, installed rollback, public license selection and AURA D3 remain unauthorized. The disposable rehearsal does not prove the original candidate archive is available for owner-controlled publication or that rebuilt bytes are identical. A later distinct owner-authorized immutable publication gate must inspect the **original** F35 candidate and refuse replacement by a rehearsal archive. All 541 completed native nodes remain unchanged.
