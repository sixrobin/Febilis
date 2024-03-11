namespace Templar.SceneLoadingDatasStorage
{
    public interface ISceneLoadingDatasOwner<T> where T : ISceneLoadingDatas
    {
        T SaveDatasBeforeSceneLoading();
        void LoadDatasAfterSceneLoading(T datas);
    }
}