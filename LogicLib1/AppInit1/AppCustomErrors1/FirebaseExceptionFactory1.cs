using Firebase.Auth;

namespace LogicLib1.AppInit1.AppCustomErrors1;

public static class FirebaseExceptionFactory1
{
    public static AppCustomException CreateFirebaseAuthException(
           this   FirebaseAuthException ex,
           bool                         isSignUp)
    {
        var raw = string.Join(" | ",
            ex.Reason.ToString(),
            ex.Message,
            ex.InnerException?.Message);

        string text = raw.ToUpperInvariant();

        bool Has(params string[] terms) =>
            terms.Any(term => text.Contains(term, StringComparison.InvariantCultureIgnoreCase));

        return true switch
        {
            _ when Has("WRONG_PASSWORD", "INVALID_PASSWORD") => new AppCustomException(
                "wrong-password",
                "Wrong password.",
                ex),

            _ when Has("UNKNOWN_EMAIL_ADDRESS", "EMAIL_NOT_FOUND", "USER_NOT_FOUND") => new AppCustomException(
                "user-not-found",
                "No account was found with this email.",
                ex),

            _ when Has("EMAIL_EXISTS") => new AppCustomException(
                "email-already-in-use",
                "This email is already in use.",
                ex),

            _ when Has("INVALID_EMAIL", "INVALID_EMAIL_ADDRESS") => new AppCustomException(
                "invalid-email",
                "The email address is not valid.",
                ex),

            _ when Has("WEAK_PASSWORD") => new AppCustomException(
                "weak-password",
                "Password is too weak. Use at least 6 characters and make it stronger.",
                ex),

            _ when Has("MISSING_PASSWORD") => new AppCustomException(
                "missing-password",
                "Password is required.",
                ex),

            _ when Has("MISSING_EMAIL") => new AppCustomException(
                "missing-email",
                "Email is required.",
                ex),

            _ when Has("OPERATION_NOT_ALLOWED") => new AppCustomException(
                "operation-not-allowed",
                "Email/password sign-in is not enabled in Firebase.",
                ex),

            _ when Has("TOO_MANY_ATTEMPTS_TRY_LATER", "TOO_MANY_REQUESTS") => new AppCustomException(
                "too-many-requests",
                "Too many attempts. Please try again later.",
                ex),

            _ when Has("USER_DISABLED") => new AppCustomException(
                "user-disabled",
                "This account has been disabled.",
                ex),

            _ when Has("INVALID_API_KEY") => new AppCustomException(
                "invalid-api-key",
                "Firebase API key is invalid.",
                ex),

            _ when Has("APP_NOT_AUTHORIZED") => new AppCustomException(
                "app-not-authorized",
                "This app is not authorized to use Firebase Authentication.",
                ex),

            _ when Has("NETWORK_REQUEST_FAILED") => new AppCustomException(
                "network-request-failed",
                "Network error. Please check your internet connection and try again.",
                ex),

            _ when Has("QUOTA_EXCEEDED") => new AppCustomException(
                "quota-exceeded",
                "Firebase quota exceeded. Please try again later.",
                ex),

            _ when Has("CAPTCHA_CHECK_FAILED") => new AppCustomException(
                "captcha-check-failed",
                "Security verification failed. Please try again.",
                ex),

            _ when Has("WEB_STORAGE_UNSUPPORTED") => new AppCustomException(
                "web-storage-unsupported",
                "This device does not support the required local storage.",
                ex),

            _ when Has("INTERNAL_ERROR") => new AppCustomException(
                "internal-error",
                "Firebase returned an internal error. Please try again.",
                ex),

            _ when Has("INVALID_CUSTOM_TOKEN") => new AppCustomException(
                "invalid-custom-token",
                "The custom auth token is invalid.",
                ex),

            _ when Has("CUSTOM_TOKEN_MISMATCH") => new AppCustomException(
                "custom-token-mismatch",
                "The custom auth token does not match this Firebase project.",
                ex),

            _ when Has("CREDENTIAL_ALREADY_IN_USE") => new AppCustomException(
                "credential-already-in-use",
                "This credential is already associated with another account.",
                ex),

            _ when Has("ACCOUNT_EXISTS_WITH_DIFFERENT_CREDENTIAL") => new AppCustomException(
                "account-exists-with-different-credential",
                "An account already exists with the same email but different sign-in credentials.",
                ex),

            _ when Has("INVALID_CREDENTIAL", "INVALID_LOGIN_CREDENTIALS") => new AppCustomException(
                "invalid-credential",
                isSignUp
                    ? "The sign-up credentials are invalid."
                    : "Invalid email or password.",
                ex),

            _ when Has("EXPIRED_OOB_CODE") => new AppCustomException(
                "expired-action-code",
                "The action code has expired.",
                ex),

            _ when Has("INVALID_OOB_CODE") => new AppCustomException(
                "invalid-action-code",
                "The action code is invalid.",
                ex),

            _ when Has("SESSION_EXPIRED") => new AppCustomException(
                "session-expired",
                "Session expired. Please sign in again.",
                ex),

            _ => new AppCustomException(
                "auth/unknown",
                $"Authentication failed: {ex.Message}",
                ex)
        };
    }
}