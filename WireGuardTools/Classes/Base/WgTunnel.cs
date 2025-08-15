namespace WireGuardTools.Classes.Base;

public readonly record struct WgTunnel ( WgKeys Server , WgKeys Client , WgBaseKey PresharedKey );