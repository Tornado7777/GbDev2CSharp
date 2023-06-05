using CloneHabr.BlazorUI;
using CloneHabr.Dto;
using CloneHabr.Dto.@enum;
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
    public Roles? Role { get; private set; } = Roles.UnregistredUser;
    public string LoginName { get; set; }
    public string Token { get; set; }

    public bool IsLoggedIn { get; set; }
    
    public void LogIn(RegistrationResponse response)
    {
        UserId = response.Session.User.UserId;
        LoginName = response.Session.User.Login;
        Token = response.Session.SessionToken;
        Role = response.Session.User.Role;
        IsLoggedIn = true;
    }

    public void LogIn(AuthenticationResponse response)
    {
        UserId = response.Session.User.UserId;
        LoginName = response.Session.User.Login;
        Token = response.Session.SessionToken;
        Role = response.Session.User.Role;
        IsLoggedIn = true;
    }

    public void LogOut() 
    {
        UserId = 0;
        SessionId = 0;
        LoginName = string.Empty;
        Token = string.Empty;
        Role = Roles.UnregistredUser;
        IsLoggedIn = false;
    }



}
