# M0-W02-C01 — source provenance and redistribution rights

## Status

**Blocked pending owner/AURA rights attestation.**

RS003 can qualify the bounded provenance inventory and the transfer exclusions, but it cannot manufacture a legal or ownership decision that is absent from repository evidence.

## Established facts

- `docs/planning/source-baseline.json` records 54 selected working-tree files with SHA-256 values and observed HEADs.
- The baseline explicitly says it is not a clean-tree claim and that no source was imported.
- AURA contributes 43 selected files, Decision 6, MCDM 4, and brainstorming 1.
- The current Math repository contains no written decision authorizing redistribution of inherited AURA source.
- Any source whose bytes differ from the pinned snapshot must be reconciled before transfer.

## Decision boundary

Until written evidence is supplied:

- no distributable AURA source is copied into Math;
- no license is inferred from repository visibility, prior use, or package identity;
- no binary, secret, private data, host policy, or worker policy is imported;
- no public package publication or source removal is authorized.

The product dependency `EXT-SOURCE-RIGHTS` therefore remains blocked.

## Required owner/AURA evidence

A short written record is sufficient if it identifies:

1. the origin/ownership basis for the selected AURA source;
2. whether Math may copy, modify, package, and redistribute it;
3. the license/notices that must accompany redistribution;
4. any excluded files or constraints;
5. the source revision to which the decision applies.

Once that evidence exists, M0-W02-C01-T1-C can resume and the chunk may be completed if the pinned corpus still matches.

## Non-goals

This decision does not start AURA extraction, select future provider licenses, change package identities, or transmit the draft AURA request.
