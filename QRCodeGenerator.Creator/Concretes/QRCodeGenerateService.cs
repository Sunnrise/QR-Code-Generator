using QRCodeGenerator.Creator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;

namespace QRCodeGenerator.Creator.Concretes
{
    public class QRCodeGenerateService : IQRCodeGenerateService
    {
        public byte[] GenerateQRCode(string text)
        {
            byte[] qrCodeImage = new byte[0];
            // Generate QR Code
            if (!string.IsNullOrEmpty(text))
            {
                // Generate QR Code
                QRCoder.QRCodeGenerator qRCodeGenerator = new();
                QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(text, QRCoder.QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode bitmap= new(qRCodeData);
                qrCodeImage = bitmap.GetGraphic(20);
                // qrCodeImage = GenerateQRCode(text);
            }
            return qrCodeImage;

        }
    }
}
