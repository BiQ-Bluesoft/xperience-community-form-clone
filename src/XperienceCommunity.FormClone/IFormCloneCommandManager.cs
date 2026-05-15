using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Kentico.Xperience.Admin.Base.Internal;

namespace XperienceCommunity.FormClone;

internal interface IFormCloneCommandManager
{
    Task<GetContentItemCloneFormItemsCommandResult> GetCloneFormItems(int formId, CancellationToken ct = default);

    Task Clone(int formId, Dictionary<string, JsonElement> formData, CancellationToken ct = default);
}
