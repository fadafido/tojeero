using System.Threading;
using System.Threading.Tasks;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Services.Contracts
{
    public interface IAuthenticationService
    {
        IUser CurrentUser { get; }
        SessionState State { get; }

        Task LogOut();
        Task RestoreSavedSession();
        Task<IUser> LogInWithFacebook();
        Task UpdateUserDetails(IUser user, CancellationToken token);
    }
}