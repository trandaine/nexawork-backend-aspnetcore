using System;

namespace NexaWork.Application.DTOs.Authentication;

public class ResponseDTO
{
    public bool Success { get; set; }
    public string? AccessToken { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
}
