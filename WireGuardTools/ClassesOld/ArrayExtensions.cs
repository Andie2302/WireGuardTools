namespace WireGuardTools.ClassesOld;

public static class ArrayExtensions
{
    public static T[] Reverse < T > ( this T[] array )
    {
        Array.Reverse ( array );

        return array;
    }
}