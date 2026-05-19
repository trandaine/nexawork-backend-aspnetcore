using System;

namespace NexaWork.Application.DTOs;

public record FileDTO
(
    Stream Content,
    string FileName,
    string ContentType,
    long Length
);
