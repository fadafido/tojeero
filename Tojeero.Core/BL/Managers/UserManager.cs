using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers
{
    public class UserManager : IUserManager
    {
        #region IUserManager implementation

        public IUser Create()
        {
            return new User();
        }

        #endregion
    }
}