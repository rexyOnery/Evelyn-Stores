using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvelynStores.Web.Services;

public enum ToastLevel { Info, Success, Warning, Error }

public class ToastMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Message { get; set; } = string.Empty;
    public ToastLevel Level { get; set; } = ToastLevel.Info;
    public int Duration { get; set; } = 4000; // ms
}

public class ToastService
{
    public event Action<ToastMessage>? OnShow;
    public event Action<Guid>? OnHide;

    public void ShowToast(string message, ToastLevel level = ToastLevel.Info, int duration = 4000)
    {
        var toast = new ToastMessage { Message = message, Level = level, Duration = duration };
        OnShow?.Invoke(toast);
    }

    public void ShowSuccess(string message, int duration = 4000) => ShowToast(message, ToastLevel.Success, duration);
    public void ShowError(string message, int duration = 4000) => ShowToast(message, ToastLevel.Error, duration);
    public void ShowInfo(string message, int duration = 4000) => ShowToast(message, ToastLevel.Info, duration);
    public void ShowWarning(string message, int duration = 4000) => ShowToast(message, ToastLevel.Warning, duration);

    public void Hide(Guid id) => OnHide?.Invoke(id);
}
