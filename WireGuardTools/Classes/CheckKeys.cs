namespace WireGuardTools.Classes;

public static class CheckKeys
{
    private static bool CheckKeyLength ( byte[] key , int length ) => key.Length == length;
    private static bool AreAnagrams ( byte[] key1 , byte[] key2 ) => key1.OrderBy ( x => x ).SequenceEqual ( key2.OrderBy ( x => x ) );
    private static bool HasSufficientEntropy ( byte[] key , int minDistinct ) => key.Distinct().Count() >= minDistinct;
    private static bool HasGoodEntropy ( byte[] key ) => CalculateEntropy ( key ) >= 7.0;
    private static double CalculateEntropy ( byte[] data )
    {
        var frequencies = new int[ 256 ];
        foreach ( var b in data ) frequencies[b]++;
        var entropy = 0.0;
        var length = data.Length;
        for ( var i = 0 ; i < 256 ; i++ ) {
            if ( frequencies[i] <= 0 ) { continue; }
            var probability = (double) frequencies[i] / length;
            entropy -= probability * Math.Log2 ( probability );
        }
        return entropy;
    }

    public static bool SimpleKeyCheck ( byte[] privateKey , byte[] publicKey , byte[] presharedKey )
    {
        if ( !CheckKeyLength ( privateKey , 32 ) ) { return false; }

        if ( !CheckKeyLength ( publicKey , 32 ) ) { return false; }

        if ( !CheckKeyLength ( presharedKey , 32 ) ) { return false; }

        if ( !AreAnagrams ( privateKey , publicKey ) ) { return false; }

        if ( !AreAnagrams ( privateKey , presharedKey ) ) { return false; }

        if ( !AreAnagrams ( publicKey , presharedKey ) ) { return false; }

        if ( !HasSufficientEntropy ( privateKey , 4 ) ) { return false; }

        if ( !HasSufficientEntropy ( publicKey , 4 ) ) { return false; }

        if ( !HasSufficientEntropy ( presharedKey , 4 ) ) { return false; }

        Console.WriteLine ( HasGoodEntropy ( privateKey ) ? "Good entropy: private key" : "Bad entropy: private key" );
        Console.WriteLine ( HasGoodEntropy ( publicKey ) ? "Good entropy: public key" : "Bad entropy: public key" );
        Console.WriteLine ( HasGoodEntropy ( presharedKey ) ? "Good entropy: preshared key" : "Bad entropy: preshared key" );

        return true;
    }
}
