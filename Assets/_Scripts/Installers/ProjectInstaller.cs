using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GrassManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<MoneyManager>().FromComponentInHierarchy().AsSingle();
    }
}
