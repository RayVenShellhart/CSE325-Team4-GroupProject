using CSE325_Team4_GroupProject.Models;
using Microsoft.JSInterop;

namespace CSE325_Team4_GroupProject.Services;

/// <summary>
/// Holds the currently signed-in user for the browser circuit.
/// Registered as Scoped so each user session has independent login state.
/// Raises OnChange so UI (e.g. header) re-renders when the user signs in/out.
/// </summary>
public class AuthStateService
{
    private const string UserIdStorageKey = "shophub_user_id";

    private readonly IJSRuntime _jsRuntime;
    private User? _currentUser;
    private event Action? _onChange;

    public User? CurrentUser => _currentUser;

    public event Action? OnChange
    {
        add => _onChange += value;
        remove => _onChange -= value;
    }

    public AuthStateService(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
    }

    // Stores user. reme,ber me stores users id in local storage
    public async Task SetUserAsync(User? user, bool rememberMe = false)
    {
        _currentUser = user;

        if (rememberMe && user != null)
        {
            await _jsRuntime.InvokeVoidAsync(
                "localStorage.setItem",
                UserIdStorageKey,
                user.Id.ToString()
            );
        }
        else
        {
            await _jsRuntime.InvokeVoidAsync(
                "localStorage.removeItem",
                UserIdStorageKey
            );
        }

        NotifyStateChanged();
    }

    //Restores a previous user
    public async Task RestoreUserAsync(UserService userService)
    {
        try
        {
            var storedUserId = await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                UserIdStorageKey
            );

            if (int.TryParse(storedUserId, out int userId))
            {
                var user = await userService.GetUserByIdAsync(userId);

                if (user != null && user.IsActive)
                {
                    _currentUser = user;
                    NotifyStateChanged();
                    return;
                }
            }
        }
        catch (JSException)
        {

        }

        _currentUser = null;
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;

        await _jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem",
            UserIdStorageKey
        );

        NotifyStateChanged();
    }

    public void SetUser(User? user)
    {
        _currentUser = user;
        NotifyStateChanged();
    }

    // notifies that authentication state has changed 
    private void NotifyStateChanged()
    {
        _onChange?.Invoke();
    }
}