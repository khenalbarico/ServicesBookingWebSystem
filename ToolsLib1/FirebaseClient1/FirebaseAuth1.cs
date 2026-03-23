using Firebase.Auth;
using System.Text;
using System.Text.Json;

namespace ToolsLib1.FirebaseClient1;

public class FirebaseAuth1(IFirebaseCfg _cfg, HttpClient _httpClient) : IToolAuthEmailProvider
{
    readonly FirebaseAuthClient _authClient = _cfg.CreateAuthClient();

    public string GetCurrentUser()
    {
        if (_authClient.User == null)
            throw new InvalidOperationException("No user is currently signed in.");

        return _authClient.User.Uid;
    }

    public async Task<UserCredential> SignInAsync(
        string email,
        string password)
    {
        var res = await _authClient.SignInWithEmailAndPasswordAsync(email, password);

        if (!res.User.Info.IsEmailVerified)
        {
            _authClient.SignOut();
            throw new Exception("Email's not yet verified. Please verify the email first.");
        }

        return res;
    }

    public async Task<UserCredential> SignUpAsync(
        string email,
        string password)
    {
        var res = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);

        var token = await res.User.GetIdTokenAsync();

        await SendEmailVerificationAsync(token);

        SignOut();

        return res;
    }

    public async Task<bool> HasValidSessionAsync()
    {
        try
        {
            var user = _authClient.User;
            if (user == null)
                return false;

            var token = await user.GetIdTokenAsync();

            return !string.IsNullOrWhiteSpace(token);
        }
        catch
        {
            return false;
        }
    }

    async Task SendEmailVerificationAsync(string idToken)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={_cfg.FirebaseApiKey}";

        var requestBody = new
        {
            requestType = "VERIFY_EMAIL",
            idToken
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await _httpClient.PostAsync(url, content);

        var resTxt = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new InvalidOperationException($"Failed to send verification email: {resTxt}");
    }

    public void SignOut()
        => _authClient.SignOut();
}