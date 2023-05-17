using CloneHabr.BlazorUI;
using CloneHabr.Dto;
using CloneHabr.Dto.Requests;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace CloneHabr.BlazorUI;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddSingleton<SessionDto>();
        builder.Services.AddSingleton<UserInfo>();
        builder.Services.AddSingleton<CreationArticleRequest>();

        await builder.Build().RunAsync();
    }
}

public class UserInfo
{
    public int UserId { get; set; }
    public int SessionId { get; set; }
    public string LoginName { get; set; }
    public string Token { get; set; }

    public bool IsLoggedIn { get; set; }
    
    public void LogIn(int userId, int sessionId, string loginName, string token)
    {
        UserId = userId;
        SessionId = sessionId;
        LoginName = loginName;
        Token = token;
        IsLoggedIn = true;
    }

    public void LogIn()
    {
        IsLoggedIn = true;
    }

    public void LogOut() 
    {
        UserId = 0;
        SessionId = 0;
        LoginName = string.Empty;
        Token = string.Empty;
        IsLoggedIn = false;
    }



}
