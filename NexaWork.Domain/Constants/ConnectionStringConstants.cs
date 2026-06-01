using System;

namespace NexaWork.Domain.Constants;

public static class ConnectionStringConstants
{
    public const string IdentityConnectionString = "Server= localhost, 1433; Database=NexaWorkIdentityDatabase; User Id=sa; password=Dai@2018; TrustServerCertificate=True; Trusted_Connection=False; MultipleActiveResultSets=true;";
    public const string ConnectionString = "Server= localhost, 1433; Database=NexaWorkDatabase; User Id=sa; password=Dai@2018; TrustServerCertificate=True; Trusted_Connection=False; MultipleActiveResultSets=true;";
    // public const string ConnectionString = "Server= 100.125.57.47, 1433; Database=NexaWorkDatabase; User Id=sa; password=lohosum619@@; TrustServerCertificate=True; Trusted_Connection=False; MultipleActiveResultSets=true;";
    public const string IPAddress = "localhost";
    public const string Port = "1433";
}
