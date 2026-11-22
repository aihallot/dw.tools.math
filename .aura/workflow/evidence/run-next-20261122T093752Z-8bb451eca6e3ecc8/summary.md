# Run-next failure evidence

- Invocation: `run-next-20261122T093752Z-8bb451eca6e3ecc8`
- Run: `RS281`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS281 failed before attempt allocation. classification=validation:m8-portable-discovery-skill-export-net11-private-package-locked. The source repository remains unchanged and no durable attempt was created.
Standard output:
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-8fe3ad68e1dc4f41bc037b96f64b1590\repository\docs\discovery\qualification\ExportM8PortableDiscoverySkill.cs(150,19): error CS0165: Use of unassigned local variable 'net11'
Standard error:

The build failed. Fix the build errors and run again.
