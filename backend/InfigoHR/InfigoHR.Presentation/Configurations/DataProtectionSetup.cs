using System.Security.Cryptography.X509Certificates;
using InfigoHR.Persistence;
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
        var dataProtection = services.AddDataProtection()
            .SetApplicationName("InfigoHR");

        if (environment.IsDevelopment())
        {
            var keyDirectory = configuration["DataProtection:KeyDirectory"]
                               ?? Path.Combine(
                                   Environment.GetFolderPath(
                                       Environment.SpecialFolder.LocalApplicationData),
                                   "InfigoHR",
                                   "data-protection-keys");

            dataProtection.PersistKeysToFileSystem(new DirectoryInfo(keyDirectory));
        }
        else
        {
            dataProtection.PersistKeysToDbContext<AppDbContext>();
        }

        var certificateBase64 = configuration["DataProtection:CertificateBase64"];
        var certificatePath = configuration["DataProtection:CertificatePath"];
        var certificatePassword = configuration["DataProtection:CertificatePassword"];
        X509Certificate2? certificate = null;

        if (!string.IsNullOrWhiteSpace(certificateBase64))
        {
            certificate = X509CertificateLoader.LoadPkcs12(
                Convert.FromBase64String(certificateBase64),
                certificatePassword);
        }
        else if (!string.IsNullOrWhiteSpace(certificatePath))
        {
            certificate = X509CertificateLoader.LoadPkcs12FromFile(
                certificatePath,
                certificatePassword);
        }

        if (certificate is not null)
        {
            if (!certificate.HasPrivateKey)
            {
                throw new InvalidOperationException(
                    "The Data Protection certificate must contain its private key.");
            }

            dataProtection.ProtectKeysWithCertificate(certificate);
        }
        else if (!environment.IsDevelopment())
        {
            throw new InvalidOperationException(
                "Configure DataProtection:CertificateBase64 or " +
                "DataProtection:CertificatePath to protect the key ring.");
        }

        return services;
    }
}
