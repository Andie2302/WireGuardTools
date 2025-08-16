
/*

    private WgTunnelKeys GenerateTunnelKey ( WgKeys serverKeys ) => new WgTunnelKeys ( serverKeys , wgKeyPairGenerator.GenerateKeyPair() , wgPresharedKeyGenerator.GeneratePresharedKey ( randomGenerator ) );

    public List< WgTunnelKeys > GenerateTunnelKeys ( int count )
    {
        var tunnels = new List< WgTunnelKeys >();
        var serverKey = wgKeyPairGenerator.GenerateKeyPair();

        for ( var i = 0 ; i < count ; i++ ) { tunnels.Add ( GenerateTunnelKey ( serverKey ) ); }

        return tunnels;
    }

//*/