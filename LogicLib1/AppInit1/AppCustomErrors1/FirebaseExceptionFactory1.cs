using Firebase.Auth;

namespace LogicLib1.AppInit1.AppCustomErrors1;

public static class FirebaseExceptionFactory1
{
    public static AppAuthException CreateFirebaseAuthExceptiin(
           this   FirebaseAuthException ex,
           bool                         isSignUp)
    {
        var reason = ex.Reason.ToString();

        return reason switch
        {
            "WrongPassword" => new AppAuthException(
                "wrong-password",
                "Wrong password.",
                ex),

            "UnknownEmailAddress" => new AppAuthException(
                "user-not-found",
                "No account was found with this email.",
                ex),

            "EmailExists" => new AppAuthException(
                "email-already-in-use",
                "This email is already in use.",
                ex),

            "InvalidEmailAddress" => new AppAuthException(
                "invalid-email",
                "The email address is not valid.",
                ex),

            "WeakPassword" => new AppAuthException(
                "weak-password",
                "Password is too weak. Use at least 6 characters and make it stronger.",
                ex),

            "MissingPassword" => new AppAuthException(
                "missing-password",
                "Password is required.",
                ex),

            "MissingEmail" => new AppAuthException(
                "missing-email",
                "Email is required.",
                ex),

            "OperationNotAllowed" => new AppAuthException(
                "operation-not-allowed",
                "Email/password sign-in is not enabled in Firebase.",
                ex),

            "TooManyAttemptsTryLater" => new AppAuthException(
                "too-many-requests",
                "Too many attempts. Please try again later.",
                ex),

            "UserDisabled" => new AppAuthException(
                "user-disabled",
                "This account has been disabled.",
                ex),

            "InvalidApiKey" => new AppAuthException(
                "invalid-api-key",
                "Firebase API key is invalid.",
                ex),

            "AppNotAuthorized" => new AppAuthException(
                "app-not-authorized",
                "This app is not authorized to use Firebase Authentication.",
                ex),

            "NetworkRequestFailed" => new AppAuthException(
                "network-request-failed",
                "Network error. Please check your internet connection and try again.",
                ex),

            "QuotaExceeded" => new AppAuthException(
                "quota-exceeded",
                "Firebase quota exceeded. Please try again later.",
                ex),

            "CaptchaCheckFailed" => new AppAuthException(
                "captcha-check-failed",
                "Security verification failed. Please try again.",
                ex),

            "WebStorageUnsupported" => new AppAuthException(
                "web-storage-unsupported",
                "This device does not support the required local storage.",
                ex),

            "InternalError" => new AppAuthException(
                "internal-error",
                "Firebase returned an internal error. Please try again.",
                ex),

            "InvalidCustomToken" => new AppAuthException(
                "invalid-custom-token",
                "The custom auth token is invalid.",
                ex),

            "CustomTokenMismatch" => new AppAuthException(
                "custom-token-mismatch",
                "The custom auth token does not match this Firebase project.",
                ex),

            "CredentialAlreadyInUse" => new AppAuthException(
                "credential-already-in-use",
                "This credential is already associated with another account.",
                ex),

            "AccountExistsWithDifferentCredential" => new AppAuthException(
                "account-exists-with-different-credential",
                "An account already exists with the same email but different sign-in credentials.",
                ex),

            "InvalidCredential" => new AppAuthException(
                "invalid-credential",
                isSignUp
                    ? "The sign-up credentials are invalid."
                    : "Invalid email or password.",
                ex),

            "ExpiredOobCode" => new AppAuthException(
                "expired-action-code",
                "The action code has expired.",
                ex),

            "InvalidOobCode" => new AppAuthException(
                "invalid-action-code",
                "The action code is invalid.",
                ex),

            "SessionExpired" => new AppAuthException(
                "session-expired",
                "Session expired. Please sign in again.",
                ex),

            _ => new AppAuthException(
                $"firebase-auth/{reason}",
                $"Authentication failed: {reason}",
                ex)
        };
    }
}
