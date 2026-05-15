using Microsoft.Extensions.DependencyInjection;

namespace XperienceCommunity.FormClone;

public static class FormCloneServiceCollectionExtensions
{
    /// <summary>
    /// Registers XperienceCommunity.FormClone services and optionally configures <see cref="FormCloneOptions"/>.
    /// </summary>
    public static IServiceCollection AddXperienceCommunityFormClone(
        this IServiceCollection services)
    {
        services.AddSingleton<IFormCloneCommandManager, FormCloneCommandManager>();

        services.Configure<FormCloneOptions>(o => o.Enabled = true);

        return services;
    }
}
