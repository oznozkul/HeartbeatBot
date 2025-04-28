using HeartbeatBot.Job.Context;

public class AppService : IAppService
{
    public List<App> GetAllApps()
    {
        using (var db = new HealtCheckContext())
        {
            return db.Apps.ToList();
        }
    }

    public App GetAppById(int id)
    {
        using (var db = new HealtCheckContext())
        {
            return db.Apps.FirstOrDefault(a => a.Id == id);
        }
    }

    public void AddApp(App newApp)
    {
        using (var db = new HealtCheckContext())
        {
            db.Apps.Add(newApp);
            db.SaveChanges();
        }
    }

    public void UpdateApp(int id, App updatedApp)
    {
        using (var db = new HealtCheckContext())
        {
            var app = db.Apps.FirstOrDefault(a => a.Id == id);
            if (app != null)
            {
                app.ApplicationName = updatedApp.ApplicationName;
                app.Url = updatedApp.Url;
                app.IsLock = updatedApp.IsLock;
                app.IsActive = updatedApp.IsActive;
                db.SaveChanges();
            }
        }
    }

    public void DeleteApp(int id)
    {
        using (var db = new HealtCheckContext())
        {
            var app = db.Apps.FirstOrDefault(a => a.Id == id);
            if (app != null)
            {
                db.Apps.Remove(app);
                db.SaveChanges();
            }
        }
    }
}
