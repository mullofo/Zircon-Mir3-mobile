
using SharpDX.Mathematics.Interop;

namespace SharpDX
{
    public static class Vector3Ex
    {
        public static RawVector3 ToRawVector3(this Vector3 vector)
        {
            return new RawVector3(vector.X, vector.Y, vector.Z);
        }
    }
}
