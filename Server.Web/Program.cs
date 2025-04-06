

using Library;
using ServerWeb.Server.Controllers;

var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationManager();
if (File.Exists("appsettings.json"))
{
    config.AddJsonFile("appsettings.json");
}
var environment = builder.Environment.EnvironmentName;
if (File.Exists($"appsettings.{environment}.json"))
{
    config.AddJsonFile($"appsettings.{environment}.json");
}

#region 资源文件
Library.Globals.ResoucePath = string.IsNullOrEmpty(config["resouce"]) ? AppContext.BaseDirectory : config["resouce"];
GameController.AppVersion = config["appVersion"];
GameController.Author = config["author"];
foreach (KeyValuePair<LibraryFile, string> library in Libraries.LibraryList)
{
    var fullpath = Path.Combine(Globals.ResoucePath, library.Value).Replace("\\", "/"); ;
    if (File.Exists(fullpath))
    {
        GameController.LibraryList[library.Value] = new MirLibrary(fullpath);
    }
}
#endregion
// Add services to the container.
builder.Services.AddRazorPages();

builder.WebHost
    .UseUrls(config["urls"]);

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.Run();
