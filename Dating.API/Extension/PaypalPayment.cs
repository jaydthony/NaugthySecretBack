using Dating.API.Configuration;
using PayPalCheckoutSdk.Core;

namespace Dating.API.Extension
{
    public static class PaypalPayment
    {
        public static void ConfigurePayment(this IServiceCollection service, IConfiguration configuration)
        {
            var paypalConfiguration = configuration.GetSection("PayPal").Get<Paypal>();
            var environment = new SandboxEnvironment(paypalConfiguration.ClientId, paypalConfiguration.Secret);
            service.AddSingleton(new PayPalHttpClient(environment));
        }
    }
}