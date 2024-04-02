using System.Windows;

namespace ImageDB
{
    public static class VectorExtension
    {
        public static Vector Switch(this Vector vector) =>
            new(vector.Y, vector.X);
    }
}
