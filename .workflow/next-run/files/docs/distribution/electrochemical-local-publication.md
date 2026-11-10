# Electrochemical Standard private publication (RS176)

RS174 qualified the original 14,269-byte `dw.quantities.standard 20260.20261110.1001`
archive. RS175 qualified the shared publisher on those bytes in a disposable feed.
The RS175 result is `947988c69bbacda5ddf8fd97c0eee65377385a7f`, attempt 001,
first-pass success under DWF 0.1.66. All seven controls passed.

RS176 uses the existing authorization for `MATH-ELECTROCHEM-UNIT-PROFILE`
(`A2-W02-C01-U-N`). Its structural inspection is read-only with respect to the
operator archive and feed. Only the post-attempt publication validation can
write the original archive to the flat `NUGET_LOCAL` feed at `I:\nuget.local`.
It uses the publisher DLL qualified in RS175, checks the candidate, both exact
dependency archives and the earlier Standard archive, and binds the destination
to the qualified package ID, version, length and SHA-256. No build or pack of the
candidate is involved. The controller build uses the exact inherited NuGet package pin.

The publication validation records the observed result in the DWF attempt report.
Its `RS176|mode=publication` output identifies the feed, ID, version, byte count and
archive SHA-256. A fresh write reports `created`. A process interrupted after the
write may observe `existing-exact-after-intent` on re-entry. That observation requires
both the exact qualified target and the matching local intent written before the move.
An existing target without that intent is a collision. Changed bytes refuse at every
entry. The intent alone is never publication evidence.

Independent readback of the physically published archives and all three loaded
DLLs requires its own run after the durable RS176 verdict. M1 remains done and
there is no native lifecycle transition. AURA historical reuse rights remain
unestablished. This private gate authorizes no remote distribution.
