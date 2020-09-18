using System.Runtime.CompilerServices;

namespace System
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(this ref T lhs, ref T rhs)
            where T: struct 
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
