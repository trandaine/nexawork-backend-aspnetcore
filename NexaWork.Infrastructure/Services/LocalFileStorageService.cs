using System;
using Microsoft.Extensions.Configuration;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Application.DTOs;
// using NexaWork.Application.DTOs.Post;

namespace NexaWork.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _storagePath;

    public LocalFileStorageService(IConfiguration configuration)
    {
        // Read the path from appsettings, default to a local "SharedStorage" if missing
        _storagePath = configuration.GetValue<string>("Storage:SharedFolderPath") ?? "../SharedStorage";
    }

    // public async Task<string> UploadFileAsync(FileDTO file, CancellationToken cancellationToken)
    // {
    //     // 1. Resolve the full physical path on the hard drive
    //     var fullStorageDirectory = Path.GetFullPath(_storagePath);
    //
    //     // 2. Ensure the directory exists
    //     if (!Directory.Exists(fullStorageDirectory))
    //     {
    //         Directory.CreateDirectory(fullStorageDirectory);
    //     }
    //
    //     // 3. Generate secure filename
    //     var extension = Path.GetExtension(file.FileName);
    //
    //
    //     var uniqueFileName = $"{Guid.NewGuid()}{extension}";
    //     var fullFilePath = Path.Combine(fullStorageDirectory, uniqueFileName);
    //
    //     // 4. Save the file
    //     using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
    //     {
    //         await file.Content.CopyToAsync(fileStream, cancellationToken);
    //     }
    //
    //     // 5. Return the URL path that the API will use to serve the file
    //     // We will configure the API in Step 3 to map "/uploads" to this physical folder
    //     return $"/uploads/{uniqueFileName}";
    // }

    /// <summary>
    /// Method to create folders to storage user uploaded media
    /// </summary>
    /// <param name="file"></param>
    /// <param name="subDirectory"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> UploadFileAsync(FileDTO file, string subDirectory, CancellationToken cancellationToken)
    {
        var fullStorageDirectory = Path.Combine(Path.GetFullPath(_storagePath), subDirectory);

        // 2. Ensure this specific subfolder exists
        if (!Directory.Exists(fullStorageDirectory))
        {
            Directory.CreateDirectory(fullStorageDirectory);
        }

        // 3. Generate secure filename
        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var fullFilePath = Path.Combine(fullStorageDirectory, uniqueFileName);

        // 4. Save the file
        using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
        {
            await file.Content.CopyToAsync(fileStream, cancellationToken);
        }

        // 5. Return the mapped URL path including the subfolder
        // ⚡ WARNING: Always use forward slashes (/) for web URLs, even if the server is Windows!
        return $"/uploads/{subDirectory}/{uniqueFileName}";
    }
}
