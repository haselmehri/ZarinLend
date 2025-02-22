using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IDastineService
    {
        Task<string> PDFDigestForMultiSign(string pdfData, string signerCertificate, string signatureFieldName, Guid creatorId, CancellationToken cancellationToken);
        Task<string> PutPDFSignatureForMultiSign(string pdfData, string signerCertificate, string signature, string signatureFieldName, Guid creatorId, CancellationToken cancellationToken);
    }
}