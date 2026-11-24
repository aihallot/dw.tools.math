# RS314 — Activate M8-W02 official SDK baseline audit RED

## Proven entry and existing source authorities

RS313 succeeded at attempt 001, first-pass, with no prior failures. It completed Discovery W01 and left M8-W02 ready, with M8-W02-C01 and its children not-ready. The Math-owned SDK source inventory InspectM8Net11SourceBaseline.cs already checks the inherited 10.0.401/latestPatch, net10.0/C#14 production baseline and exactly thirteen packable libraries; its --check-sdk11 mode also observes the independent stable SDK 11.0.100. InspectM8Net11PackageGraph.cs already checks the sixteen direct reference edges, acyclicity and absence of sibling repository project references. Do not duplicate these audits or rerun the historical thirteen-project full compilation campaign merely to activate a phase.

## Bounded new RED evidence

Activate only M8-W02/C01/T1/RED and confirm the existing two source inventories still pass. Add one narrow read-only test-coverage probe that maps eleven existing MSTest csproj projects in the root solution to all thirteen packable source projects. The test requires exactly twenty-three direct test source references and rejects a deliberately incomplete in-memory projection that omits a unique direct test owner. This establishes an observable baseline and a real detector for missing test coverage, not .NET 11 semantic compatibility or an approved migration.

## Official primary sources and unresolved responsibilities

Use Microsoft Learn's .NET 11 overview, compatibility changes, C# 15 documentation and global.json SDK selection as current primary-source delta inputs. Preserve earlier Math source/package consumers and the historical RS252 audit as bounded evidence with their original limitations. The next GREEN candidate must assign official delta applicability to each of the thirteen libraries and their tests; evaluate runtime CPU/RID, locked restore, precision and serializer behavior without assuming a syntax upgrade is required.

M8 remains in-progress, W01 done, W02/C01/T1/RED in-progress and all other W02 phases and tasks not-ready. There is no RED completion, SDK/TFM switch, package rewrite, public release, external host installation, or broker invocation authority.
