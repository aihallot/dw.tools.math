# M6-W02-C03-S02-T2 — explicit discrete sampling and linear topology audit

## Accepted scope

RS096 independently qualifies the T2 boundary and integration gate for S02,
following RS095's qualified finite BCL-only direct O(N^2) complex DFT and
real zero-extended linear convolution. The T1 FiniteSignal64 methods remain
callable and unchanged. The separate FiniteSignalBoundaryAudit64 facade
adds explicit caller policy and conservative typed cancellation screening
in local dw.tools.math.scientific 1.1.0-preview.8.

## Explicit interpretation and refusal

- EvaluateDft requires CallerAcceptsDiscreteSamples. Without it, return
  SamplingPolicyRequired. The agreement only affirms discrete finite
  samples, NOT an observed Nyquist bandwidth, antialias filtering,
  sampling interval, unit or alias-free physical signal.
- EvaluateConvolution requires LinearZeroExtended and rejects an
  unspecified topology with ConvolutionTopologyRequired. A caller
  requesting CircularUnsupported receives CircularConvolutionUnsupported.
  An FFT-based or periodic convolution is never substituted.
- Pre-cancelled operations return Cancelled before policy or input checks.
  For accepted policies, previously qualified InvalidLength, InvalidInput,
  OutsideQualifiedRange, NonFiniteResult and Cancelled are mapped to typed
  nonpartial audit outcomes with no DFT bins or convolution samples.
- T1 input length 1..32, complex/real binary64 components <=1e6,
  output components <=4e13 and linear output size N+M-1 remain intact.

## Conservative binary64 signal ratios (not error theorems)

DFT audit takes each accepted backend bin's complex magnitude divided
by the sum of input sample magnitudes; this is the relative signed-sum
signal because twiddle factors have unit magnitude. If the input magnitude
sum is positive and any bin ratio is below 1e-12, return the typed
SpectralCancellationRisk refusal with no bins. An all-zero input is still
valid. An exactly zero mathematical bin may be conservatively refused.
Inverse uses the same explicitly normalized T1 direct transform.

Linear convolution audit takes each backend output magnitude divided by
the sum of absolute products contributing to that zero-extended output.
A strictly positive absolute-term sum with ratio below 1e-12 causes
ConvolutionCancellationRisk. A sample with only zero products is not a
cancellation risk. For example [1,-1] convoluted with [1,1] has a
cancelling middle sample: T1 direct convolution remains valid, while
the conservative T2 audit may refuse it.

These fixed heuristic ratios are NOT certified forward/backward errors,
proved condition numbers, measured round-off failures or evidence that a
sampled signal is alias-free. Exact, fully cancelling signals can be
rejected conservatively. No DFT-to-FFT rewrite or implicit time units.

## Reproducible evidence and gates

Observe one independently compiled reflection RED for the missing
FiniteSignalBoundaryAudit64 public API BEFORE staging its implementation.
A compile error is never an acceptable RED substitute. Recover it with
68 independent GREEN boundary and policy assertions, retain 60 S02-T1,
53 S01-T2, 25 S01-T1 and 326 prior C01/C02 scientific tests (464 retained),
four Math regression projects and structural/transfer validations.

Build/pack one actual local preview.8 scientific NuGet archive. Inspect
compiled net10 assembly, nuspec identity/dependencies, byte count and
SHA-256. Four independently cached package-only clients, each pinned to
that exact archive from a cleared local feed, must execute eight probes:
S02-T2 audit, retained S02-T1 direct signal, retained S01-T2 geometry
boundary and retained C02-T2 special functions. Their actual byte counts
and SHA-256 must agree with the inspector.

On durable acceptance close only S02-T2 RED/GREEN/V, task T2 and the
S02 phase. S03, the original C03 size-L aggregate T1/T2, M6-W02 and
M6 remain open. This does not certify other operating systems, public
NuGet availability, geodesic/physical units, FFT complexity guarantees,
or AURA/Decision/MCDM consumer adoption.
