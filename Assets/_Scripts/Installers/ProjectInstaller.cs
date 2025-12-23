using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GrassManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<MoneyManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<AudioPlayer>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SoundManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<VFXPlayer>().FromComponentInHierarchy().AsSingle();
        Container.Bind<VFXManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GrassCutter>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SalesTable>().FromComponentInHierarchy().AsSingle();
    }
}
