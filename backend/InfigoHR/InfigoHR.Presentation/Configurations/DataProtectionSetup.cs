using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InfigoHR.Presentation.Configurations;

public static class DataProtectionSetup
{
    public static IServiceCollection AddInfigoDataProtection(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var keyDirectory = configuration["DataProtection:KeyDirectory"]
                           ?? Path.Combine(
                               Environment.GetFolderPath(
                                   Environment.SpecialFolder.LocalApplicationData),
                               "InfigoHR",
                               "data-protection-keys");

        var dataProtection = services.AddDataProtection()
            .SetApplicationName("InfigoHR")
            .PersistKeysToFileSystem(new DirectoryInfo(keyDirectory));

        var certificatePath =
            configuration["DataProtection:CertificatePath"];

        if (!string.IsNullOrWhiteSpace(certificatePath))
        {
            var certificate = X509CertificateLoader.LoadPkcs12FromFile(
                certificatePath,
                configuration["DataProtection:CertificatePassword"]);

            dataProtection.ProtectKeysWithCertificate(certificate);
        }
        else if (!environment.IsDevelopment())
        {
            throw new InvalidOperationException(
                "Configure a certificate to protect the persistent " +
                "Data Protection key ring.");
        }

        return services;
    }
}