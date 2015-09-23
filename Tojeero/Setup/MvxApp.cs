using Cirrious.CrossCore.IoC;
using Tojeero.Core.ViewModels;
using Tojeero.Core.ViewModels;

namespace Tojeero
{
    public class MvxApp : Cirrious.MvvmCross.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
			CreatableTypes()
				.EndingWith("Service")
				.AsInterfaces()
				.RegisterAsLazySingleton();

			CreatableTypes()
				.EndingWith("ViewModel")
				.AsTypes()
				.RegisterAsDynamic();

			RegisterAppStart<LogInViewModel>();
        }
    }
}