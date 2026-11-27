# M10 post-cutover archive preservation and rollback (RS423)

RS422 qualified both independent consumers on Ubuntu 24.04 WSL, SDK 11.0.100 and linux-x64 with all thirteen original candidate DLL identities. RS423 reconciles that evidence in backlog 0.1.190.

The rollback probe reuses RS407's exact 30 input digests and smoke selection and RS415's Git text recovery method. Starting from a copy of current production inputs, recover the original bytes from the pinned pre-cutover commit in an ignored isolated workspace. Perform actual locked restore, Release build of all 24 projects on the original .NET 10 SDK policy and nine fragile smoke tests. Compare production inputs, candidate archive and DLL identities, twelve shared Math archives before/after, and known historical archive hashes; preserve the shared tool package too. No live rollback is permitted.

The exact candidate and current production artifacts are never rebuilt or overwritten. Archive preservation is bounded to the inspected feed and known durable identities; no ABI or release claim. Independent verification and native GREEN closure remain subsequent controlled steps.
