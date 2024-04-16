using ShopGiay.Models;

namespace ShopGiay.Services
{
    public interface IVnPay
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}