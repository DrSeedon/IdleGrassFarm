using Zenject;

/// <summary>
/// Инсталлер для регистрации зависимостей в Zenject.
/// Связывает сервисы и компоненты в проекте.
/// </summary>
public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        //Container.Bind<TransmissionService>().FromComponentInHierarchy().AsSingle();
    }
}
