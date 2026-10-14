# M4 symbolic provider boundary experiment

The T2 experiment is a .NET 10 standalone **child-process supervised test**, not a public Math symbolics adapter.

Run from its own separately restored packages cache with a pinned, observed AngouriMath 2.5.0 archive. Source:
- tests/providers/m4-w01-c01-boundaries/m4-provider-boundaries.csproj
- tests/providers/m4-w01-c01-boundaries/Program.cs
- docs/planning/evidence/M4-W01-C01-T2-boundaries-qualified.json (created only on successful DWF execution)

Admitted test-only mathematical inputs are p(x)=x^2-1 and f(x)=(x^2-1)/(x-1), x != 1. Input admission is limited to 128 characters, 8 nested parentheses, 64 non-whitespace symbolic token characters, and **exact fixture-name matching**. Invalid or unknown domain expressions are refused before entering the provider. Over reals sqrt(x^2)=|x| is an example of an unsupported assumption requiring independent Math-IR treatment, not permission to simplify to x.

The bounded host spawns its own built DLL via dotnet, without code-generation, script execution, external CAS or user-chosen command lines. The worker returns only fixed cases:
- derivative: the residual of provider derivative p'(x)-2x simplifies to 0.
- conditional: the provider's reduced quotient has observable condition text (contains "provided" and variable/boundary markers). It is **not** encoded as portable typed Math IR.
- delay: an intentionally noncooperative child sleeps; the parent kills its process tree on a 200 ms host deadline.
- cancellation: a second delay is terminated after a 150 ms parent cancellation request.
- failure: a nonzero child exit is never a successful mathematical result.

The successful executable marker has probes=14, timeout=host, cancellation=host, ir=unsupported. The host-enforced deadline is a refusal boundary, **not** a universal performance promise; the selected provider is not certified cooperative, and no memory sandbox or non-Windows runtime is qualified.

An independent read-only archive verifier observes and rechecks the exact RS053 third-party package bytes:
AngouriMath 2.5.0 SHA-256 8f194bf9ee09cc0514bac5894d413bb8c7e6c6f01071aa8274e24718bf855419 (MIT; Antlr4.Runtime.Standard, GenericTensor, HonkSharp, PeterO.Numbers, System.Memory). Product assemblies remain package/solution-independent of this experimental CAS; no new symbolic public library is installed.

The outcome is documentation, a bounded scientific host experiment and a durable release-scoped evidence receipt. Neither a full IR bridge, native provider cancellation, AURA/BrokerReady invocation nor Decision/MCDM adoption may be inferred. M4-W01-C01-T2 and C01 close only on a successful DWF run; the next accepted C02 deliverable owns exact assumption and exclusion preservation.
