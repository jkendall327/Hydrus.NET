namespace Hydrus.NET;

public record HydrusJobStatusNetworkJob(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("waiting_on_connection_error")] bool WaitingOnConnectionError,
    [property: JsonPropertyName("domain_ok")] bool DomainOk,
    [property: JsonPropertyName("waiting_on_serverside_bandwidth")] bool WaitingOnServerSideBandwidth,
    [property: JsonPropertyName("no_engine_yet")] bool NoEngineYet,
    [property: JsonPropertyName("has_error")] bool HasError,
    [property: JsonPropertyName("total_data_used")] long TotalDataUsed,
    [property: JsonPropertyName("is_done")] bool IsDone,
    [property: JsonPropertyName("status_text")] string StatusText,
    [property: JsonPropertyName("current_speed")] long CurrentSpeed,
    [property: JsonPropertyName("bytes_read")] long BytesRead,
    [property: JsonPropertyName("bytes_to_read")] long BytesToRead);

public record HydrusJobStatusFiles(
    [property: JsonPropertyName("hashes")] string[] Hashes,
    [property: JsonPropertyName("label")] string Label);

public record HydrusJobStatus(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("creation_time")] float CreationTime,
    [property: JsonPropertyName("status_title")] string? StatusTitle = null,
    [property: JsonPropertyName("status_text_1")] string? StatusText1 = null,
    [property: JsonPropertyName("status_text_2")] string? StatusText2 = null,
    [property: JsonPropertyName("had_error")] bool HadError = false,
    [property: JsonPropertyName("traceback")] string? Traceback = null,
    [property: JsonPropertyName("is_cancellable")] bool IsCancellable = false,
    [property: JsonPropertyName("is_cancelled")] bool IsCancelled = false,
    [property: JsonPropertyName("is_deleted")] bool IsDeleted = false,
    [property: JsonPropertyName("is_pausable")] bool IsPausable = false,
    [property: JsonPropertyName("is_paused")] bool IsPaused = false,
    [property: JsonPropertyName("is_working")] bool IsWorking = false,
    [property: JsonPropertyName("nice_string")] string? NiceString = null,
    [property: JsonPropertyName("attached_files_mergable")] bool? AttachedFilesMergable = null,
    [property: JsonPropertyName("popup_gauge_1")] float[]? PopupGauge1 = null,
    [property: JsonPropertyName("popup_gauge_2")] float[]? PopupGauge2 = null,
    [property: JsonPropertyName("api_data")] object? ApiData = null,
    [property: JsonPropertyName("files")] HydrusJobStatusFiles? Files = null,
    [property: JsonPropertyName("user_callable_label")] string? UserCallableLabel = null,
    [property: JsonPropertyName("network_job")] HydrusJobStatusNetworkJob? NetworkJob = null);

public record HydrusPopupsResponse(
    [property: JsonPropertyName("job_statuses")] HydrusJobStatus[] JobStatuses);

public record HydrusPopupAddResponse(
    [property: JsonPropertyName("job_status")] HydrusJobStatus JobStatus);

public record HydrusPopupUpdateResponse(
    [property: JsonPropertyName("job_status")] HydrusJobStatus JobStatus);

public class HydrusPopupManager
{
    private readonly HttpClient _httpClient;

    public HydrusPopupManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Get a list of popups from the client.
    /// </summary>
    /// <param name="onlyInView">Whether to show only the popups currently in view in the client.</param>
    public async Task<HydrusPopupsResponse> GetPopupsAsync(bool? onlyInView = null)
    {
        var url = "manage_popups/get_popups";
        if (onlyInView.HasValue)
        {
            url += $"?only_in_view={onlyInView.Value.ToString().ToLower()}";
        }

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusPopupsResponse>();
    }

    /// <summary>
    /// Add a popup.
    /// </summary>
    public async Task<HydrusPopupAddResponse> AddPopupAsync(
        string? statusTitle = null,
        string? statusText1 = null,
        string? statusText2 = null,
        bool? isCancellable = null,
        bool? isPausable = null,
        bool? attachedFilesMergable = null,
        float[]? popupGauge1 = null,
        float[]? popupGauge2 = null,
        object? apiData = null,
        string? filesLabel = null,
        HydrusFiles? files = null)
    {
        var requestContent = new Dictionary<string, object>();

        if (statusTitle != null)
        {
            requestContent["status_title"] = statusTitle;
        }

        if (statusText1 != null)
        {
            requestContent["status_text_1"] = statusText1;
        }

        if (statusText2 != null)
        {
            requestContent["status_text_2"] = statusText2;
        }

        if (isCancellable.HasValue)
        {
            requestContent["is_cancellable"] = isCancellable.Value;
        }

        if (isPausable.HasValue)
        {
            requestContent["is_pausable"] = isPausable.Value;
        }

        if (attachedFilesMergable.HasValue)
        {
            requestContent["attached_files_mergable"] = attachedFilesMergable.Value;
        }

        if (popupGauge1 != null)
        {
            requestContent["popup_gauge_1"] = popupGauge1;
        }

        if (popupGauge2 != null)
        {
            requestContent["popup_gauge_2"] = popupGauge2;
        }

        if (apiData != null)
        {
            requestContent["api_data"] = apiData;
        }

        if (filesLabel != null)
        {
            requestContent["files_label"] = filesLabel;
        }

        files?.AddToDictionary(requestContent);

        var response = await _httpClient.PostAsJsonAsync("manage_popups/add_popup", requestContent);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusPopupAddResponse>();
    }

    /// <summary>
    /// Call the user callable function of a popup.
    /// </summary>
    /// <param name="jobStatusKey">The job status key to call the user callable of.</param>
    public async Task CallUserCallableAsync(string jobStatusKey)
    {
        var requestContent = new Dictionary<string, string>
        {
            ["job_status_key"] = jobStatusKey
        };

        var response = await _httpClient.PostAsJsonAsync("manage_popups/call_user_callable", requestContent);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Try to cancel a popup.
    /// </summary>
    /// <param name="jobStatusKey">The job status key to cancel.</param>
    public async Task CancelPopupAsync(string jobStatusKey)
    {
        var requestContent = new Dictionary<string, string>
        {
            ["job_status_key"] = jobStatusKey
        };

        var response = await _httpClient.PostAsJsonAsync("manage_popups/cancel_popup", requestContent);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Try to dismiss a popup.
    /// </summary>
    /// <param name="jobStatusKey">The job status key to dismiss.</param>
    public async Task DismissPopupAsync(string jobStatusKey)
    {
        var requestContent = new Dictionary<string, string>
        {
            ["job_status_key"] = jobStatusKey
        };

        var response = await _httpClient.PostAsJsonAsync("manage_popups/dismiss_popup", requestContent);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Mark a popup as done.
    /// </summary>
    /// <param name="jobStatusKey">The job status key to finish.</param>
    public async Task FinishPopupAsync(string jobStatusKey)
    {
        var requestContent = new Dictionary<string, string>
        {
            ["job_status_key"] = jobStatusKey
        };

        var response = await _httpClient.PostAsJsonAsync("manage_popups/finish_popup", requestContent);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Finish and dismiss a popup.
    /// </summary>
    /// <param name="jobStatusKey">The job status key to finish and dismiss.</param>
    /// <param name="seconds">Optional number of seconds to wait before dismissing.</param>
    public async Task FinishAndDismissPopupAsync(string jobStatusKey, int? seconds = null)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["job_status_key"] = jobStatusKey
        };

        if (seconds.HasValue)
        {
            requestContent["seconds"] = seconds.Value;
        }

        var response = await _httpClient.PostAsJsonAsync("manage_popups/finish_and_dismiss_popup", requestContent);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Update a popup.
    /// </summary>
    public async Task<HydrusPopupUpdateResponse> UpdatePopupAsync(
        string jobStatusKey,
        string? statusTitle = null,
        string? statusText1 = null,
        string? statusText2 = null,
        float[]? popupGauge1 = null,
        float[]? popupGauge2 = null,
        object? apiData = null,
        string? filesLabel = null,
        HydrusFiles? files = null)
    {
        var requestContent = new Dictionary<string, object>
        {
            ["job_status_key"] = jobStatusKey
        };

        if (statusTitle != null)
        {
            requestContent["status_title"] = statusTitle;
        }

        if (statusText1 != null)
        {
            requestContent["status_text_1"] = statusText1;
        }

        if (statusText2 != null)
        {
            requestContent["status_text_2"] = statusText2;
        }

        if (popupGauge1 != null)
        {
            requestContent["popup_gauge_1"] = popupGauge1;
        }

        if (popupGauge2 != null)
        {
            requestContent["popup_gauge_2"] = popupGauge2;
        }

        if (apiData != null)
        {
            requestContent["api_data"] = apiData;
        }

        if (filesLabel != null)
        {
            requestContent["files_label"] = filesLabel;
        }

        files?.AddToDictionary(requestContent);

        var response = await _httpClient.PostAsJsonAsync("manage_popups/update_popup", requestContent);
        response.EnsureSuccessStatusCode();
        return await response.ReadFromHydrusJsonAsync<HydrusPopupUpdateResponse>();
    }
}