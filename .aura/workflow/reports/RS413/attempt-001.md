# RS413 Attempt 001 Payload Operations

- Operations: 45
- Changed: 40
- No change: 0
- Verified: 5
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS412/attempt-001.json` | run=RS412 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C01-T1-R state=done |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C01-T1-G state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11 state=done |
| 5 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 6 | `files.replace-from-staged` | Changed | `docs/planning/ValidateNativeDwfAdoption.cs` | bytes=14728 |
| 7 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM10MigrationAdmission.cs` | bytes=46466 |
| 8 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs` | bytes=61038 |
| 9 | `files.replace-from-staged` | Changed | `docs/planning/ValidateAssessaM9Admission.cs` | bytes=43404 |
| 10 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9AssessaPublishedHandoff.cs` | bytes=5241 |
| 11 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs` | bytes=3343 |
| 12 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs` | bytes=67823 |
| 13 | `files.replace-from-staged` | Changed | `README.md` | bytes=9956 |
| 14 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m10-production-cutover-green-rs413.md` | bytes=3556 |
| 15 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM10ProductionCutover.cs` | bytes=10752 |
| 16 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 17 | `files.replace-from-staged` | Changed | `Directory.Build.props` | bytes=643 |
| 18 | `files.replace-from-staged` | Changed | `global.json` | bytes=105 |
| 19 | `files.replace-from-staged` | Changed | `src/projects/dw.quantities.expression/packages.lock.json` | bytes=126 |
| 20 | `files.replace-from-staged` | Changed | `src/projects/dw.quantities.standard/packages.lock.json` | bytes=283 |
| 21 | `files.replace-from-staged` | Changed | `src/projects/dw.quantities/packages.lock.json` | bytes=62 |
| 22 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.analysis/packages.lock.json` | bytes=62 |
| 23 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.codecs/packages.lock.json` | bytes=275 |
| 24 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.composition/packages.lock.json` | bytes=275 |
| 25 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.distribution/packages.lock.json` | bytes=959 |
| 26 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.foundation/packages.lock.json` | bytes=62 |
| 27 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.ir/packages.lock.json` | bytes=126 |
| 28 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.numerics/packages.lock.json` | bytes=275 |
| 29 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/packages.lock.json` | bytes=62 |
| 30 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.statistics/packages.lock.json` | bytes=126 |
| 31 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.symbolics/packages.lock.json` | bytes=275 |
| 32 | `files.replace-from-staged` | Changed | `tests/projects/dw.quantities.tests/packages.lock.json` | bytes=4391 |
| 33 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.analysis.tests/packages.lock.json` | bytes=3878 |
| 34 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.codecs.tests/packages.lock.json` | bytes=4174 |
| 35 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.composition.tests/packages.lock.json` | bytes=4229 |
| 36 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.distribution.tests/packages.lock.json` | bytes=5153 |
| 37 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.foundation.tests/packages.lock.json` | bytes=3880 |
| 38 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.ir.tests/packages.lock.json` | bytes=4018 |
| 39 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.numerics.tests/packages.lock.json` | bytes=4176 |
| 40 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/packages.lock.json` | bytes=3880 |
| 41 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.statistics.tests/packages.lock.json` | bytes=4026 |
| 42 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.symbolics.tests/packages.lock.json` | bytes=4177 |
| 43 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj` | bytes=571 |
| 44 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-preflight/Program.cs` | bytes=17112 |
| 45 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M10-W01-C02-T1-G activated=M10-W01-C02-T1-G |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS412/attempt-001.json`
- Summary: run=RS412 attempt=1 lineage=exact
- After SHA-256: `2b612ac6cec15dc2a71349891e1f85385df99270bc43eac4720656b7c79a4315`

## 2. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C01-T1-R state=done
- After SHA-256: `be5fb7889ebec410228a71a28ab2a39299bd82bb9d7b17caa8e0b09cb94adecb`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C01-T1-G state=done
- After SHA-256: `be5fb7889ebec410228a71a28ab2a39299bd82bb9d7b17caa8e0b09cb94adecb`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11 state=done
- After SHA-256: `be5fb7889ebec410228a71a28ab2a39299bd82bb9d7b17caa8e0b09cb94adecb`

## 5. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 6. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: bytes=14728
- Before SHA-256: `f02bd426403fd1e18ebc1503395ec243db6a44eb77b3fdb0048f4d901b9bd49c`
- After SHA-256: `228764985aa022ae06d463f5f1b03fa492a64bfc83268366ee74b94cb4a008a3`

## 7. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM10MigrationAdmission.cs`
- Summary: bytes=46466
- Before SHA-256: `71faee850cdd1ec943ae4f2f10f8b8c8b7ae5293bd99e711d17e3e07c2eb36c1`
- After SHA-256: `fec5e78d7d8d7520bd94b760f2ed68392a5aedbd95dc1e76ac45470b8c03d1fb`

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs`
- Summary: bytes=61038
- Before SHA-256: `e367140558ce4d26f3319d13008bfd138a8b45be8173e0f554c7cfbc4b1152df`
- After SHA-256: `5769a407b51eaf6b87768716f4724ec789e42f9bebc235bb9b41d298e5680679`

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateAssessaM9Admission.cs`
- Summary: bytes=43404
- Before SHA-256: `5ab8fa3e0560052018e0f23242526079a38855b58e7f09b323b767e19fc6eeac`
- After SHA-256: `6d4e7d5ed9b08676c5a8468356c78bfd7bb2c25704997e8fa79e6f8c4239514c`

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9AssessaPublishedHandoff.cs`
- Summary: bytes=5241
- Before SHA-256: `dce3d99cef8020fa608452a6b4512568549d2619d74ad66871ee6ee56e209cbf`
- After SHA-256: `d317845fd84fd292bf22e8231c260dac43486e1b841aeb5283f45bf16f051492`

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs`
- Summary: bytes=3343
- Before SHA-256: `a45ecd778d40df77e3b0f33285672e5170dad086e7bc5ea9d7f846dfb89786d2`
- After SHA-256: `50e82009dde058aa2256007c467640c7e80c219933e0df80149205ad2817586c`

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs`
- Summary: bytes=67823
- Before SHA-256: `8e95356de69e01768fb539c1da8ddda9a67f94876dc4b24d3720ee44c98e5be7`
- After SHA-256: `919ca3c287ca6f4df27053a8379f78c59ad8906afda6532a1b3232fe4c61dd7e`

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `README.md`
- Summary: bytes=9956
- Before SHA-256: `175dc5482590993ef7a61a495f3e16bedf253e64655c95da01749cb0e61cd41e`
- After SHA-256: `bfc3a439b28073f4d11d63480b2f72f7caf7c2a51812ed93434a79030a715f59`

## 14. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m10-production-cutover-green-rs413.md`
- Summary: bytes=3556
- After SHA-256: `a076d958e923a433b86a9d77d3a9e9373cc344eb71067a2c963506a1e5306787`

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ProbeM10ProductionCutover.cs`
- Summary: bytes=10752
- After SHA-256: `03d5b7d25884e2a07446204b6df059a083cf1b76bb6e755c4c6a3a9a5a7d7719`

## 16. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `17d9815f2508b25d855c57992dbe9b4f8073720dfd58276eb3290aa47047b23c`
- After SHA-256: `dc0a2d0c46319db76972544bd48c370ddfec4f990cb19461ee8452c3d0984871`

## 17. `files.replace-from-staged`

- Status: `Changed`
- Path: `Directory.Build.props`
- Summary: bytes=643
- Before SHA-256: `0fb999b91f4bf691cbfb80b4204ca89143adf15a783fc376a31444fd7c350544`
- After SHA-256: `3591aa5392ceaa87778758c2a9a4d7bebfe06f960a11310d33eb7ac599cb0887`

## 18. `files.replace-from-staged`

- Status: `Changed`
- Path: `global.json`
- Summary: bytes=105
- Before SHA-256: `ce61f5ae207d33d00594c77c5999c7f0dc66434e85a85f38090335d588438974`
- After SHA-256: `bfaf40d1a86baeea39cde6300a5aaa9c55917256d4fade475ac7ffa3bd58bb58`

## 19. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.quantities.expression/packages.lock.json`
- Summary: bytes=126
- Before SHA-256: `fdb1918bc37f00fb31224c1a94f36fca2b96815cf0fe3a0a3933f7e2698909a7`
- After SHA-256: `b23a6aefc2afb063effe6318ac6e380f79e6110d1594c24438d0632f42fb56dc`

## 20. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.quantities.standard/packages.lock.json`
- Summary: bytes=283
- Before SHA-256: `1adc3569108a2a7c99c14ec142a5f6e30bacf017eba6faf51a24bee6896328d3`
- After SHA-256: `d88efbac810dcb79920225d5442804e1a81dbb4dbdc9c79ad20bbb73970e6e78`

## 21. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.quantities/packages.lock.json`
- Summary: bytes=62
- Before SHA-256: `1624731bf151ec2ac6993023046a4abbac7c6ddc71ed2e242e8285bbc69d561e`
- After SHA-256: `43db74eec805a02a5184ad281fc41e169e3f40c0551ee457fbd7f9a851d6f1c2`

## 22. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.analysis/packages.lock.json`
- Summary: bytes=62
- Before SHA-256: `1624731bf151ec2ac6993023046a4abbac7c6ddc71ed2e242e8285bbc69d561e`
- After SHA-256: `43db74eec805a02a5184ad281fc41e169e3f40c0551ee457fbd7f9a851d6f1c2`

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.codecs/packages.lock.json`
- Summary: bytes=275
- Before SHA-256: `1879b7faa989957c08471f6341657409ec3bbe249fb9d00713bdf338a085ef16`
- After SHA-256: `4e459436ce7e5fbf69ac19b796800b2b4bf9ecf843c23599f63113e7a5c35ad5`

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.composition/packages.lock.json`
- Summary: bytes=275
- Before SHA-256: `1879b7faa989957c08471f6341657409ec3bbe249fb9d00713bdf338a085ef16`
- After SHA-256: `4e459436ce7e5fbf69ac19b796800b2b4bf9ecf843c23599f63113e7a5c35ad5`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.distribution/packages.lock.json`
- Summary: bytes=959
- Before SHA-256: `a2cb96aba0fbeb87f86576b16d3b24a01511e6691a77f2e2ab0ed491d3c9da1b`
- After SHA-256: `4329ecfb4445cea8709fbc5cb84e7907ab5ce76a32ac1b4b63335dc36a8b0d03`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.foundation/packages.lock.json`
- Summary: bytes=62
- Before SHA-256: `2241481b7e74dcdcd600303146b730ecc12cbd8e49aa55755ef95ee499fe229a`
- After SHA-256: `43db74eec805a02a5184ad281fc41e169e3f40c0551ee457fbd7f9a851d6f1c2`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.ir/packages.lock.json`
- Summary: bytes=126
- Before SHA-256: `fdb1918bc37f00fb31224c1a94f36fca2b96815cf0fe3a0a3933f7e2698909a7`
- After SHA-256: `b23a6aefc2afb063effe6318ac6e380f79e6110d1594c24438d0632f42fb56dc`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.numerics/packages.lock.json`
- Summary: bytes=275
- Before SHA-256: `1879b7faa989957c08471f6341657409ec3bbe249fb9d00713bdf338a085ef16`
- After SHA-256: `4e459436ce7e5fbf69ac19b796800b2b4bf9ecf843c23599f63113e7a5c35ad5`

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/packages.lock.json`
- Summary: bytes=62
- Before SHA-256: `1624731bf151ec2ac6993023046a4abbac7c6ddc71ed2e242e8285bbc69d561e`
- After SHA-256: `43db74eec805a02a5184ad281fc41e169e3f40c0551ee457fbd7f9a851d6f1c2`

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.statistics/packages.lock.json`
- Summary: bytes=126
- Before SHA-256: `fdb1918bc37f00fb31224c1a94f36fca2b96815cf0fe3a0a3933f7e2698909a7`
- After SHA-256: `b23a6aefc2afb063effe6318ac6e380f79e6110d1594c24438d0632f42fb56dc`

## 31. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.symbolics/packages.lock.json`
- Summary: bytes=275
- Before SHA-256: `1879b7faa989957c08471f6341657409ec3bbe249fb9d00713bdf338a085ef16`
- After SHA-256: `4e459436ce7e5fbf69ac19b796800b2b4bf9ecf843c23599f63113e7a5c35ad5`

## 32. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.quantities.tests/packages.lock.json`
- Summary: bytes=4391
- Before SHA-256: `2c1844a8dadf17a566815ff93e02c75c7d194dd8d9a45b4b4c001d38189db08a`
- After SHA-256: `98f33956a1e225e3ebb03da1f325531287000c9544639421820481a68c3294c7`

## 33. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.analysis.tests/packages.lock.json`
- Summary: bytes=3878
- Before SHA-256: `50c3bc42260c2d38244fea52aa4bcbb9bf903ac62985158b9a9c44307c701fb0`
- After SHA-256: `60afe83a63ff9d5d633c8aad37b3744ca1ab6cf9d06c586f6fbc494143256557`

## 34. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.codecs.tests/packages.lock.json`
- Summary: bytes=4174
- Before SHA-256: `d012a1459dc7078e0aaaca4968eef38281a2f9553712f1a49656907f75f66fba`
- After SHA-256: `2080fc113804f313bd489274abab57ce03e7575346343a194e2dcc8124fbc363`

## 35. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.composition.tests/packages.lock.json`
- Summary: bytes=4229
- Before SHA-256: `48ace020a1dc7f7ba5343bc14f7752739ad0ec131715380f4da9d8d183412caf`
- After SHA-256: `677fda71f89589faf7a94ed3832e28a55cc7625f7e7bc684e92832cb06d9f291`

## 36. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.distribution.tests/packages.lock.json`
- Summary: bytes=5153
- Before SHA-256: `3dcb6ce1951ab25deb689fc678f917ff7220f82eb13c87f319a66552d36ce0f0`
- After SHA-256: `29db75501307f066b1f054fa8e5267451983540b8afbe1ae1afa2090055d9d48`

## 37. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.foundation.tests/packages.lock.json`
- Summary: bytes=3880
- Before SHA-256: `449dbde623fbdab65cf2f82637e217dbb431348d8e107bb214e7ca7efb4b3da7`
- After SHA-256: `d7893b930d7c93a59caed4c4193d1b755283dda0ad32866d046417d50411ff09`

## 38. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.ir.tests/packages.lock.json`
- Summary: bytes=4018
- Before SHA-256: `5dd40479c989bc563640b932b0c116b483cd78109d9cbd3ca91fb42d93caf635`
- After SHA-256: `c8c2154cd1c3d44c74bdaea4e4c3b20314dafc9a6f2dbdc5b3ec78efa1dea5f6`

## 39. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.numerics.tests/packages.lock.json`
- Summary: bytes=4176
- Before SHA-256: `58cc13878ee7038e9920e6cbb5cf2c8b55fb81907b1be824bf0ad3a8fb989117`
- After SHA-256: `888a191e526e8d9a18478723f955e763279fef5ee9d63c72f5d92fc477f0cb5f`

## 40. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/packages.lock.json`
- Summary: bytes=3880
- Before SHA-256: `ce5e7d4a11549b8fb39562773ddec282c218b2766661e7301b18eebcb3216ff6`
- After SHA-256: `666d84a2613528f0ebe09d3d0304ae2961d475f6848844e8ddd624064c85c197`

## 41. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.statistics.tests/packages.lock.json`
- Summary: bytes=4026
- Before SHA-256: `ad54c49730a24d590482fc44ccfda28dac329e5434b58957b83288d9b5349ff3`
- After SHA-256: `b0e2b6a09034512c521433d01f72dd09baafe5648e8977192965f3bbef7ded95`

## 42. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.symbolics.tests/packages.lock.json`
- Summary: bytes=4177
- Before SHA-256: `21bcc32234d170972d09f9bb7e5c38a58e1f9ec5a405105cb7476a47a6c9bffd`
- After SHA-256: `e806ac8643259f67cf29745bf450083ba6b09385f487c1c8c428334a1e32ed1f`

## 43. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj`
- Summary: bytes=571
- Before SHA-256: `5dda57552166031a89b13c95236c9a7bc761f8425865b6ddba5918b000c4f7a2`
- After SHA-256: `d3c782f48f6341cc00c35f9236f6c797f78f9e42f8269558159ca8c98be547fe`

## 44. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-preflight/Program.cs`
- Summary: bytes=17112
- Before SHA-256: `5a49800bc40f232cd3b347cba0968bcf1d57f2b6d79fff01765dc405e5e03c92`
- After SHA-256: `f79b6917884841673d5523296a64f9298e0c0a8e0ee06ac7003730e8dd8cee6e`

## 45. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M10-W01-C02-T1-G activated=M10-W01-C02-T1-G
- Before SHA-256: `be5fb7889ebec410228a71a28ab2a39299bd82bb9d7b17caa8e0b09cb94adecb`
- After SHA-256: `3192cd549dceea95bcddd4fcf013e34597edfc2b9e2e17d0cb4e6f50df8845ea`

