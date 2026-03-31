using BlazorApp1.Helpers;
using BlazorApp1.Services;
using LogicLib1.AppModels1.Client;
using LogicLib1.AppModels1.Server.Booking;
using LogicLib1.AppModels1.Server.Services;
using LogicLib1.AppPayment1;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace BlazorApp1.Components.Pages;

public partial class BookService : IDisposable
{
    [Inject] protected BookingPageService BookingService { get; set; } = default!;
    [Inject] protected PaymongoQrph1 PaymongoQrph { get; set; } = default!;
    [Inject] protected IBookingPaymentStore BookingPaymentStore { get; set; } = default!;
    [Inject] protected BookingNotificationService BookingNotificationService { get; set; } = default!;

    [Parameter] public string Category { get; set; } = "";

    protected bool IsLoading = true;
    protected bool IsSubmitting;
    protected bool ShowClientForm;
    protected bool ShowPaymentModal;
    protected bool ShowSuccessModal;

    protected string ErrorMessage = "";
    protected string SuccessMessage = "";

    protected List<BaseSvcStructure> CurrentCategoryServices = [];
    protected ClientInformation ClientInfo = new();

    protected readonly Dictionary<string, DateTime?> SelectedDatesByService = [];
    protected readonly Dictionary<string, string> SelectedTimesByService = [];
    protected readonly List<AppointmentItem> AppointmentItems = [];

    protected PaymongoQrphChargeResult? PaymentResult;
    protected ClientRequest? PendingRequest;
    protected string CurrentBookingId = "";

    private IDisposable? _bookingSubscription;

    protected string CategoryDisplayName => Category.GetDisplayName();

    protected override async Task OnParametersSetAsync()
    {
        IsLoading = true;
        ErrorMessage = "";
        SuccessMessage = "";
        CurrentCategoryServices = [];

        try
        {
            CurrentCategoryServices = await BookingService.GetServicesByCategoryAsync(Category);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load services: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected DateTime? GetSelectedDate(string serviceUid) =>
        SelectedDatesByService.TryGetValue(serviceUid, out var value) ? value : null;

    protected string GetSelectedDateString(string serviceUid) =>
        GetSelectedDate(serviceUid)?.ToString("yyyy-MM-dd") ?? "";

    protected string GetSelectedTime(string serviceUid) =>
        SelectedTimesByService.TryGetValue(serviceUid, out var value) ? value : "";

    protected void OnDateChanged(string serviceUid, string? value)
    {
        if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            SelectedDatesByService[serviceUid] = parsed.Date;
        else
            SelectedDatesByService[serviceUid] = null;
    }

    protected void OnTimeChanged(string serviceUid, string? value) =>
        SelectedTimesByService[serviceUid] = value ?? "";

    protected void AddToAppointment(BaseSvcStructure service)
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (AppointmentItems.Any(x => x.ServiceUid == service.Uid))
        {
            ErrorMessage = "This service is already in the appointment list.";
            return;
        }

        var selectedDate = GetSelectedDate(service.Uid);
        var selectedTime = GetSelectedTime(service.Uid);

        if (!selectedDate.HasValue)
        {
            ErrorMessage = $"Please select a date for {service.Details}.";
            return;
        }

        if (string.IsNullOrWhiteSpace(selectedTime))
        {
            ErrorMessage = $"Please select a time slot for {service.Details}.";
            return;
        }

        AppointmentItems.Add(new AppointmentItem
        {
            ServiceUid = service.Uid,
            ServiceName = service.Details,
            ServiceDetails = service.Details,
            ServiceCost = service.Cost,
            SelectedDate = selectedDate.Value,
            SelectedTimeSlot = selectedTime
        });
    }

    protected void RemoveFromAppointment(string serviceUid)
    {
        var item = AppointmentItems.FirstOrDefault(x => x.ServiceUid == serviceUid);
        if (item is not null)
            AppointmentItems.Remove(item);
    }

    protected void OpenClientForm()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (AppointmentItems.Count == 0)
        {
            ErrorMessage = "Please add at least one service first.";
            return;
        }

        ShowClientForm = true;
    }

    protected void CloseClientForm()
    {
        ShowClientForm = false;
    }

    protected async Task StartPaymentAsync()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        if (AppointmentItems.Count == 0)
        {
            ErrorMessage = "There are no services in the appointment.";
            return;
        }

        try
        {
            IsSubmitting = true;

            BookingService.ApplyConsumerFallbacks(ClientInfo);

            PendingRequest = BookingService.BuildClientRequest(ClientInfo, AppointmentItems);
            CurrentBookingId = PendingRequest.ClientInformation.ClientBookingId ?? "";

            PaymentResult = await PaymongoQrph.CreateQrphChargeAsync(PendingRequest);

            var record = new BookingPaymentRecord
            {
                BookingId = CurrentBookingId,
                Category = Category,
                PaymentIntentId = PaymentResult.PaymentIntentId ?? "",
                PaymentStatus = "Pending",
                EmailSent = false,
                CreatedAt = DateTime.UtcNow,
                Request = PendingRequest
            };

            await BookingPaymentStore.SaveAsync(record);

            _bookingSubscription?.Dispose();
            _bookingSubscription = BookingNotificationService.Subscribe(CurrentBookingId, OnBookingStatusChangedAsync);

            ShowClientForm = false;
            ShowPaymentModal = true;
            ErrorMessage = "";
            SuccessMessage = "";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to start payment: {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private async Task OnBookingStatusChangedAsync(string status)
    {
        if (string.Equals(status, "Paid", StringComparison.OrdinalIgnoreCase))
        {
            ShowPaymentModal = false;
            ShowSuccessModal = true;
            ErrorMessage = "";
            SuccessMessage = "";
        }
        else if (string.Equals(status, "Failed", StringComparison.OrdinalIgnoreCase))
        {
            ShowPaymentModal = false;
            ErrorMessage = "Payment failed. Please try again.";
        }
        else if (string.Equals(status, "Expired", StringComparison.OrdinalIgnoreCase))
        {
            ShowPaymentModal = false;
            ErrorMessage = "QR payment expired. Please generate a new QR code.";
        }

        await InvokeAsync(StateHasChanged);
    }

    protected void ClosePaymentModal()
    {
        ShowPaymentModal = false;
    }

    protected void CloseSuccessModal()
    {
        ShowSuccessModal = false;
    }

    protected void FinishSuccessfulBooking()
    {
        ShowSuccessModal = false;
        SuccessMessage = "Your appointment has been confirmed successfully.";
        ResetForm();
    }

    protected void ResetForm()
    {
        _bookingSubscription?.Dispose();
        _bookingSubscription = null;

        ShowClientForm = false;
        ShowPaymentModal = false;
        PaymentResult = null;
        PendingRequest = null;
        CurrentBookingId = "";
        ClientInfo = new ClientInformation();
        AppointmentItems.Clear();
        SelectedDatesByService.Clear();
        SelectedTimesByService.Clear();
    }

    public void Dispose()
    {
        _bookingSubscription?.Dispose();
    }
}