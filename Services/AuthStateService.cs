using CSE325_Team4_GroupProject.Models;

namespace CSE325_Team4_GroupProject.Services;

/// <summary>
/// Holds the currently signed-in user for the browser circuit.
/// Registered as Scoped so each user session has independent login state.
/// Raises OnChange so UI (e.g. header) re-renders when the user signs in/out.
/// </summary>
public class AuthStateService
{
    private User? _currentUser;
    private event Action? _onChange;

    public User? CurrentUser => _currentUser;

    public event Action? OnChange
    {
        add => _onChange += value;
        remove => _onChange -= value;
    }

    public void SetUser(User? user)
    {
        _currentUser = user;
        NotifyStateChanged();
    }

    public void Logout()
    {
        _currentUser = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        _onChange?.Invoke();
    }
}