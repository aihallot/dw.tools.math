# Canonical Math IR JSON — admitted v1 transport

**Status:** functional subset for `M2-W01-C03-T1`; adversarial decoding and
separate semantic/presentation hashing remain pending in `T2`.

The `dw.tools.math.ir.IrCanonicalJsonCodec` provides
`Encode(IrNode)` and `Decode(string)`, with version `math-ir/1`.
It constructs JSON in deterministic property order with stable enum tokens
and does not use machine-local culture or binary floating-point for exact
rational values. Re-encoding the same admitted structural graph gives the
same bytes. This is **structural identity**, not an assertion that two
different representations are mathematically equivalent.

## Admitted node variants

| Kind | Stored meaning | Admitted shape |
|---|---|---|
| `exact` | canonical `BigInteger` numerator and positive denominator, both JSON strings | exact rational |
| `binary64` | exact 16-digit lowercase IEEE-754 bit hex string | finite values including negative zero |
| `symbol` | stable free/bound identity, passive display label, domain, scope and slot | symbol binding metadata |
| `quantity` | nested scalar, unit ID, explicit `UnitSystem`, all eight dimensions, temperature tag | exact/finite typed quantity |
| `apply` | closed operation enum and ordered typed operands | bounded expression |
| `matrix` | rows, columns, row-major homogeneous scalar cells | immutable rectangular matrix |
| `restricted` | expression and ordered explicit symbol-scoped relations with exact right-hand sides | exclusions/domain conditions |

Root object shape: `{"version":"math-ir/1","root":{...}}`. Rational
`1/3` is represented as `"numerator":"1","denominator":"3"`,
not `0.3333333333333333`; binary64 `-0.0` retains its sign bit.
A scope-sensitive `x != 1` condition is stored as an explicit relation,
not discarded when an expression is serialized.

This initial codec is a plain Math BCL-dependent component of
`dw.tools.math.ir`, not a general symbolic solver, MathML/OpenMath
implementation or JSON-to-C# execution engine. A finite character budget
of 262,144 and the already-qualified in-memory IR graph limits apply.
Unsupported node kinds and unknown schema versions are refused, with
additional malformed-input and unbound-symbol corpus work reserved for T2.

## Qualification and remaining boundaries

The T1 evidence contains two independently observed missing-public-API REDs,
ten direct exact/round-trip oracles and full IR/quantity/foundation regression.
T2 must independently harden parsing limits, unknown fields, duplicate
keys, malformed numeric inputs, unbound/scope-invalid identifiers, and
canonical semantic versus presentation hashing.

A semantic structural hash, if provided later, must be versioned,
independent of any presentation-only label policy, and must **not** be
marketed as proof of full algebraic equivalence. No external AURA package
adoption or publication is claimed by this codec.
