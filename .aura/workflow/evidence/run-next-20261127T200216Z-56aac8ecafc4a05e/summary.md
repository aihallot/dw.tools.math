# Run-next failure evidence

- Invocation: `run-next-20261127T200216Z-56aac8ecafc4a05e`
- Run: `RS413`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS413 failed before attempt allocation. classification=validation:math-rs413-actual-target. The source repository remains unchanged and no durable attempt was created.
Process status: timedOut=false cancelled=false exitCode=-532462766 workflowExitCode=-532462766 terminationAttempted=false terminationErrorPresent=false timeoutMilliseconds=240000 durationMilliseconds=28052
Standard output:
[dwf-validation] strategy=declared-authority stdoutPresent=false stderrPresent=true nugetDiagnosticObserved=false compilerOrMsbuildDiagnosticObserved=false Process status: timedOut=false cancelled=false exitCode=-532462766 workflowExitCode=-532462766 terminationAttempted=false terminationErrorPresent=false timeoutMilliseconds=240000 durationMilliseconds=28052
Standard error:
Unhandled exception. System.Text.Json.JsonReaderException: 'n' is an invalid start of a property name. Expected a '"'. LineNumber: 3 | BytePositionInLine: 4.
   at System.Text.Json.ThrowHelper.ThrowJsonReaderException(Utf8JsonReader& json, ExceptionResource resource, Byte nextByte, ReadOnlySpan`1 bytes)
   at System.Text.Json.Utf8JsonReader.ReadSingleSegment()
   at System.Text.Json.Utf8JsonReader.Read()
   at System.Text.Json.JsonDocument.Parse(ReadOnlySpan`1 utf8JsonSpan, JsonReaderOptions readerOptions, MetadataDb& database, StackRowStack& stack)
   at System.Text.Json.JsonDocument.ParseUnrented(ReadOnlyMemory`1 utf8Json, JsonReaderOptions readerOptions, JsonTokenType tokenType, Boolean allowDuplicateProperties)
   at System.Text.Json.JsonDocument.ParseValue(ReadOnlySpan`1 json, JsonDocumentOptions options)
   at System.Text.Json.Nodes.JsonNode.Parse(String json, Nullable`1 nodeOptions, JsonDocumentOptions documentOptions)
   at Program.<>c__DisplayClass0_0.<<Main>$>g__Equivalent|11(String path, String a, String b) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-d7969507545e4629b1580b6e05b5f11e\repository\docs\engineering\VerifyM10ProductionCutover.cs:line 84
   at Program.<>c__DisplayClass0_1.<<Main>$>b__16(String p) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-d7969507545e4629b1580b6e05b5f11e\repository\docs\engineering\VerifyM10ProductionCutover.cs:line 89
   at System.Linq.Enumerable.ArrayWhereIterator`1.ToArray(ReadOnlySpan`1 source, Func`2 predicate)
   at System.Linq.Enumerable.ArrayWhereIterator`1.ToArray()
   at System.Linq.Enumerable.OrderedIterator`1.ToArray()
   at Program.<>c__DisplayClass0_0.<<Main>$>g__Mismatches|12(Dictionary`2 actual, Dictionary`2 target) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-d7969507545e4629b1580b6e05b5f11e\repository\docs\engineering\VerifyM10ProductionCutover.cs:line 89
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-d7969507545e4629b1580b6e05b5f11e\repository\docs\engineering\VerifyM10ProductionCutover.cs:line 123
