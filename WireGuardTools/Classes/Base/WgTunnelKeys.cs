namespace WireGuardTools.Classes.Base;

public readonly record struct WgTunnelKeys ( WgKeys Server , WgKeys Client , WgBaseKey PresharedKey );