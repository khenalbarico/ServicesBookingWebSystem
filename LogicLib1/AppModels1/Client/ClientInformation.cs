using System.ComponentModel.DataAnnotations;
using ToolsLib1.FormatAttributes1;

namespace LogicLib1.AppModels1.Client;

public class ClientInformation : IValidatableObject
{
    public string ClientBookingId { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string ContactNumber { get; set; } = "";

    [Required]
    public string FirstName { get; set; } = "";

    [Required]
    public string LastName { get; set; } = "";

    public string ConsumerFirstName { get; set; } = "";
    public string ConsumerLastName { get; set; } = "";
    public string ConsumerContactNumber { get; set; } = "";

    public DateTime BookingDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        bool hasAnyConsumerValue =
            !string.IsNullOrWhiteSpace(ConsumerFirstName) ||
            !string.IsNullOrWhiteSpace(ConsumerLastName) ||
            !string.IsNullOrWhiteSpace(ConsumerContactNumber);

        if (hasAnyConsumerValue)
        {
            if (string.IsNullOrWhiteSpace(ConsumerFirstName))
            {
                yield return new ValidationResult(
                    "Consumer first name is required once any consumer field is provided.",
                    new[] { nameof(ConsumerFirstName) });
            }

            if (string.IsNullOrWhiteSpace(ConsumerLastName))
            {
                yield return new ValidationResult(
                    "Consumer last name is required once any consumer field is provided.",
                    new[] { nameof(ConsumerLastName) });
            }

            if (string.IsNullOrWhiteSpace(ConsumerContactNumber))
            {
                yield return new ValidationResult(
                    "Consumer contact number is required once any consumer field is provided.",
                    new[] { nameof(ConsumerContactNumber) });
            }
        }
    }
}