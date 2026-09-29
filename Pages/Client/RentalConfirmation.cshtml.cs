using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;

namespace _24LockyLockers.Pages.Client;

/// <summary>
/// Page model for rental confirmation.
/// </summary>
[Authorize(Roles = "Client")]
public class RentalConfirmationModel : PageModel
{
    public string? QrCodeImage { get; set; }

    public void OnGet()
    {
        var accessCode = TempData["AccessCode"]?.ToString();
        if (!string.IsNullOrEmpty(accessCode))
        {
            QrCodeImage = GenerateQrCode(accessCode);
        }
    }

    private string GenerateQrCode(string data)
    {
        var generator = new QRCodeGenerator();
        var qrCodeData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

        // Usa PngByteQRCode para gerar bytes PNG
        var qrCode = new PngByteQRCode(qrCodeData);
        var pngBytes = qrCode.GetGraphic(10); // 10 = pixels por módulo

        // Converte para Base64
        var base64 = Convert.ToBase64String(pngBytes);
        return $"data:image/png;base64,{base64}";
    }
}