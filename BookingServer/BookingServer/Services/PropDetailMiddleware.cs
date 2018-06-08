using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BookingServer.Services
{
    public class PropDetailMiddleware : IMiddleware
    {
        private readonly IPropDetail propDetail;

        public PropDetailMiddleware(IPropDetail detail)
        {
            propDetail = detail;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if(!propDetail.Stored)
            {
                await next(context);
                return;
            }
        }
    }
}
