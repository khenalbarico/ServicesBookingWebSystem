using Firebase.Auth;
using LogicLib1.AppInit1.AppCustomErrors1;
using ToolsLib1.FirebaseClient1;

namespace LogicLib1.AppAuth1;

public class AppAuthentication1(IToolAuthEmailProvider _authClient) : IAppAuthentication
{
    public string GetCurrentUser()
        => _authClient.GetCurrentUser();

    public async Task<UserCredential> SignInAsync(string email, string pass)
    {
        try
        {
            return await _authClient.SignInAsync(email, pass);
        }
        catch (FirebaseAuthException ex)
        {
            throw ex.CreateFirebaseAuthException(isSignUp: false);
        }
        catch (HttpRequestException ex)
        {
            throw new AppCustomException(
                "network-request-failed",
                "Network error. Please check your internet connection and try again.",
                ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new AppCustomException(
                "request-timeout",
                "The request timed out. Please try again.",
                ex);
        }
        catch (Exception ex)
        {
            throw new AppCustomException(
                "unknown-error",
                "Something went wrong while signing in.",
                ex);
        }
    }

    public void SignOut()
        => _authClient.SignOut();

    public async Task SignUpAsync(string email, string pass)
    {
        try
        {
            await _authClient.SignUpAsync(email, pass);
        }
        catch (FirebaseAuthException ex)
        {
            throw ex.CreateFirebaseAuthException(isSignUp: true);
        }
        catch (HttpRequestException ex)
        {
            throw new AppCustomException(
                "network-request-failed",
                "Network error. Please check your internet connection and try again.",
                ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new AppCustomException(
                "request-timeout",
                "The request timed out. Please try again.",
                ex);
        }
        catch (Exception ex)
        {
            throw new AppCustomException(
                "unknown-error",
                "Something went wrong while signing up.",
                ex);
        }
    }
}

