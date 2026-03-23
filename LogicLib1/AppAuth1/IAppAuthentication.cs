using Firebase.Auth;

namespace LogicLib1.AppAuth1;

public interface IAppAuthentication
{
    string GetCurrentUser();
    Task<UserCredential> SignInAsync(
        string email,
        string pass);
    Task<UserCredential> SignUpAsync(
        string email,
        string pass);
    void SignOut();
}
