using QRCoder;

namespace WireGuardTools.QR;

public static class QrCode
{
    public static void GenerateQrCodePng(string configContent, string filePath)
    {
        try
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                using (var qrCodeData = qrGenerator.CreateQrCode(configContent, QRCodeGenerator.ECCLevel.Q))
                {
                    using (var qrCode = new PngByteQRCode(qrCodeData))
                    {
                        var qrCodeBytes = qrCode.GetGraphic(20);
                        File.WriteAllBytes(filePath, qrCodeBytes);
                    }
                }
            }

            Console.WriteLine($"QR-Code als PNG in '{filePath}' gespeichert.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
        }
    }
}