namespace WireGuardTools.Classes.Base;

public record struct WgTunnel ( WgKeys Server , WgKeys Client , WgBaseKey PresharedKey );