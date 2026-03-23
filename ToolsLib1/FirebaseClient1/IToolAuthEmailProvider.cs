using Firebase.Auth;

namespace ToolsLib1.FirebaseClient1;

public interface IToolAuthEmailProvider
{
    string GetCurrentUser();
    Task<UserCredential> SignInAsync(string email, string password);
    Task<UserCredential> SignUpAsync(string email, string password);
    void SignOut();
}
