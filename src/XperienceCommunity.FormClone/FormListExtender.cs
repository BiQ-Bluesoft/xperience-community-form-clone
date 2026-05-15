using System.Threading;
using System.Threading.Tasks;

using CMS.Membership;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Internal;
using Kentico.Xperience.Admin.DigitalMarketing.UIPages;

using Microsoft.Extensions.Options;

[assembly: PageExtender(typeof(XperienceCommunity.FormClone.FormListExtender))]

namespace XperienceCommunity.FormClone;

internal sealed class FormListExtender : PageExtender<FormList>
{
    private readonly IFormCloneCommandManager formCloneCommandManager;
    private readonly IUIPermissionEvaluator permissionEvaluator;
    private readonly FormCloneOptions options;

    public FormListExtender(
        IFormCloneCommandManager formCloneCommandManager,
        IUIPermissionEvaluator permissionEvaluator,
        IOptions<FormCloneOptions> options)
    {
        this.formCloneCommandManager = formCloneCommandManager;
        this.permissionEvaluator = permissionEvaluator;
        this.options = options.Value;
    }

    public override Task ConfigurePage()
    {
        if (!options.Enabled)
        {
            return base.ConfigurePage();
        }

        return ConfigurePageInternal();
    }

    private async Task ConfigurePageInternal()
    {
        bool hasCreatePermission = (await permissionEvaluator.Evaluate(SystemPermissions.CREATE)).Succeeded;

        Page.PageConfiguration.TableActions.AddActionWithCustomComponent(
            new AddActionWithCustomComponentParameters(
                "Clone form",
                new ContentItemCloneComponent
                {
                    Properties = new ContentItemCloneProperties
                    {
                        CloneCommandName = nameof(Clone),
                        GetCloneFormItemsCommandName = nameof(GetFormItemsForClone),
                    }
                })
            {
                Icon = Icons.DocCopy,
                Disabled = !hasCreatePermission
            });

        await base.ConfigurePage();
    }

    [PageCommand(Permission = SystemPermissions.CREATE)]
    public async Task<ICommandResponse<RowActionResult>> Clone(ContentItemCloneCommandArguments args, CancellationToken ct)
    {
        await formCloneCommandManager.Clone(args.SourceContentItemId, args.Data, ct);

        return ResponseFrom(new RowActionResult(true)).AddSuccessMessage("Form cloned successfully.");
    }

    [PageCommand(Permission = SystemPermissions.CREATE)]
    public async Task<ICommandResponse<GetContentItemCloneFormItemsCommandResult>> GetFormItemsForClone(int contentItemId, CancellationToken ct)
    {
        var result = await formCloneCommandManager.GetCloneFormItems(contentItemId, ct);

        return ResponseFrom(result);
    }
}
