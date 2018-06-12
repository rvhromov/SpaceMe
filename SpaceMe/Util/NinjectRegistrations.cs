using Ninject.Modules;
using SpaceMe.Repository;

namespace SpaceMe.Util
{
    public class NinjectRegistrations : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepository>().To<PostRepository>();
        }
    }
}