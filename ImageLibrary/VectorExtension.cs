using System.Runtime.CompilerServices;
using System.Windows;

namespace ImageDB
{
    public static class VectorExtension
    {
        public static Vector Switch(this Vector vector) =>
            new Vector(vector.Y, vector.X);
    }
}
