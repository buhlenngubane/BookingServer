using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingServer.Services
{
    public interface ITokenManager
    {
        Task<bool> IsCurrentActiveToken();
        Task DeactivateCurrentAsync();
        Task RefreshCurrentAsync();
        Task<bool> IsActiveAsync(string token);
        Task DeactivateAsync(string token);
        Task RefreshAsync(string token);
    }
}
