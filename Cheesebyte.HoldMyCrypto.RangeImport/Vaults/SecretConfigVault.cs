using Cheesebyte.HoldMyCrypto.Vaults.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Cheesebyte.HoldMyCrypto.Vaults;

/// <summary>
/// Simple implementation of a <see cref="ISecretVault"/> that reads
/// secure data from <see cref="IConfiguration"/> objects.
/// </summary>
public class SecretConfigVault : ISecretVault
{
    private readonly IConfigurationRoot _configRoot;
    
    public SecretConfigVault(IConfigurationRoot configRoot)
    {
        _configRoot = configRoot;
    }
    
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public T? GetSecureKey<T>(string keyName)
    {
        var transformedKeyName = keyName.Replace('/', ':');
        var result = _configRoot.GetValue<T>(transformedKeyName);
        
        return result;
    }
}
