
public interface IAppService
{
    void AddApp(App newApp);
    void DeleteApp(int id);
    List<App> GetAllApps();
    App GetAppById(int id);
    void UpdateApp(int id, App updatedApp);
}