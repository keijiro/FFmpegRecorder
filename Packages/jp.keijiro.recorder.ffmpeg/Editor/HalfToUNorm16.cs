using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;

namespace FFmpegOut.Recorder
{
    static class HalfToUNorm16
    {
        public static NativeArray<ulong> Convert(NativeArray<ulong> source)
        {
            // Temporary memory for the conversion job
            using (var temp = new NativeArray<ulong>
              (source.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
            {
                // Launch and complete the job.
                var job = new ConverterJob { Input = source, Output = temp };
                job.Schedule(source.Length, 64).Complete();

                // The TempJob memory is not accessible from the encoder
                // thread, so we have to copy it to temp memory.
                return new NativeArray<ulong>(temp, Allocator.Temp);
            }
        }

        [Unity.Burst.BurstCompile(CompileSynchronously = true)]
        struct ConverterJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<ulong> Input;
            [WriteOnly] public NativeArray<ulong> Output;

            public void Execute(int i)
            {
                // half4 as ulong
                var ul = Input[i];

                // half4 as ulong -> half4 as uint4
                var ui4 = math.uint4((uint)(ul      ),
                                     (uint)(ul >> 16),
                                     (uint)(ul >> 32),
                                     (uint)(ul >> 48)) & 0xffffu;

                // half4 as int4 -> float4
                var f4 = HalfToSingle(ui4);

                // float4 -> UNorm16x4 as uint4
                ui4 = (uint4)(math.saturate(f4) * 65535);

                // UNorm16x4 as uint4 -> UNorm16x4 as ulong
                ul = ((ulong)ui4.x      ) +
                     ((ulong)ui4.y << 16) +
                     ((ulong)ui4.z << 32) +
                     ((ulong)ui4.w << 48);

                Output[i] = ul;
            }
        }

        // This is not a complete implementation of half-to-single conversion.
        // - No subnormal value support
        // - No infinite value support
        static float4 HalfToSingle(uint4 ui4)
        {
            var frac = ui4 & 0x3ffu;
            var exp = (ui4 >> 10) & 0x1fu;
            var sign = ui4 >> 15;
            return
              math.min(1, exp) *    // Zero cases (real zero and subnormals)
              (1 - sign * 2) *              // Sign flag
              math.pow(2, exp - 15) *       // Exponent part
              (1 + (float4)frac / 1024.0f); // Fraction part
        }
    }
}
