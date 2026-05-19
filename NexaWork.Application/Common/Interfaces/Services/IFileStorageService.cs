using System;
using NexaWork.Application.DTOs;

// using NexaWork.Application.DTOs.Post;

namespace NexaWork.Application.Common.Interfaces.Services;

public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file and returns its URL or identifier.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="subDirectory"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> UploadFileAsync(FileDTO file, string subDirectory, CancellationToken cancellationToken);
}