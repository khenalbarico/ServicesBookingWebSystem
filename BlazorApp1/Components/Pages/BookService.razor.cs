using BlazorApp1.Helpers;
using BlazorApp1.Services;
using LogicLib1.AppEmailer1;
using LogicLib1.AppModels1.Client;
using LogicLib1.AppModels1.Server.Services;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace BlazorApp1.Components.Pages;

public partial class BookService
{
    [Inject]    protected     BookingPageService BookingService { get; set; } = default!;
    [Inject]    protected     IAppEmailer        AppEmailer     { get; set; } = default!;

    [Parameter] public string                    Category       { get; set; } = "";

    protected bool   IsLoading      = true;
    protected bool   IsSubmitting;
    protected bool   ShowClientForm;
    protected string ErrorMessage   = "";
    protected string SuccessMessage = "";

    protected List<BaseSvcStructure> CurrentCategoryServices = [];
    protected ClientInformation      ClientInfo              = new();

    protected readonly Dictionary<string, DateTime?> SelectedDatesByService = [];
    protected readonly Dictionary<string, string>    SelectedTimesByService = [];
    protected readonly List<AppointmentItem>         AppointmentItems       = [];

    protected string CategoryDisplayName => Category.GetDisplayName();

    protected override async Task OnParametersSetAsync()
    {
        IsLoading               = true;
        ErrorMessage            = "";
        SuccessMessage          = "";
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
        {
            SelectedDatesByService[serviceUid] = parsed.Date;
        }
        else
        {
            SelectedDatesByService[serviceUid] = null;
        }
    }

    protected void OnTimeChanged(string serviceUid, string? value)
    => SelectedTimesByService[serviceUid] = value ?? "";
    
    protected void AddToAppointment(BaseSvcStructure service)
    {
        ErrorMessage   = "";
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
            ServiceUid       = service.Uid,
            ServiceName      = service.Details,
            ServiceDetails   = service.Details,
            ServiceCost      = service.Cost,
            SelectedDate     = selectedDate.Value,
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

    protected async Task SubmitAppointmentAsync()
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
            var request = BookingService.BuildClientRequest(ClientInfo, AppointmentItems);

            await AppEmailer.SendEmailAsync(request);

            SuccessMessage = "Your appointment request has been submitted successfully.";
            ResetForm();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to submit appointment: {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    protected void ResetForm()
    {
        ShowClientForm = false;
        AppointmentItems.Clear();
        SelectedDatesByService.Clear();
        SelectedTimesByService.Clear();
        ClientInfo = new ClientInformation();
    }
}
