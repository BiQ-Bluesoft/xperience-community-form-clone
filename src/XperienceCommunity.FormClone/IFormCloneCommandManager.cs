using System.Text.Json;

using Kentico.Xperience.Admin.Base.Internal;

namespace XperienceCommunity.FormClone;

internal interface IFormCloneCommandManager
{
    public Task<GetContentItemCloneFormItemsCommandResult> GetCloneFormItems(int formId, CancellationToken ct = default);

    public Task Clone(int formId, Dictionary<string, JsonElement> formData, CancellationToken ct = default);
}
