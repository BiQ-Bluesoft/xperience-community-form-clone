using System.Text.Json;

using CMS.DataEngine;
using CMS.EmailMarketing.Internal;
using CMS.FormEngine;
using CMS.OnlineForms;
using CMS.OnlineForms.Internal;

using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Admin.Base.Forms.Internal;
using Kentico.Xperience.Admin.Base.Internal;

namespace XperienceCommunity.FormClone;

internal sealed class FormCloneCommandManager : IFormCloneCommandManager
{
    private readonly IFormItemCollectionProvider formItemCollectionProvider;
    private readonly IFormDataBinder formDataBinder;
    private readonly IAutoresponderProcessHelper autoresponderProcessHelper;

    public FormCloneCommandManager(IFormItemCollectionProvider formItemCollectionProvider, IFormDataBinder formDataBinder, IAutoresponderProcessHelper autoresponderProcessHelper)
    {
        this.formItemCollectionProvider = formItemCollectionProvider;
        this.formDataBinder = formDataBinder;
        this.autoresponderProcessHelper = autoresponderProcessHelper;
    }

    public async Task<GetContentItemCloneFormItemsCommandResult> GetCloneFormItems(int formId, CancellationToken ct = default)
    {
        var form = await BizFormInfo.Provider.GetAsync(formId, ct)
            ?? throw new InvalidOperationException($"Form with ID {formId} not found.");

        var model = new CloneFormModel { FormDisplayName = $"{form.FormDisplayName} (copy)" };
        var formItems = await formItemCollectionProvider.GetFormItems(model, ct);
        var clientProperties = await formItems.OnlyVisible().GetClientProperties();

        return new GetContentItemCloneFormItemsCommandResult { FormItems = clientProperties };
    }

    public async Task Clone(int formId, Dictionary<string, JsonElement> formData, CancellationToken ct = default)
    {
        var originalForm = await BizFormInfo.Provider.GetAsync(formId, ct)
            ?? throw new InvalidOperationException($"Form with ID {formId} not found.");

        var model = new CloneFormModel();
        var formItems = await formItemCollectionProvider.GetFormItems(model, formData, ct);
        var components = formItems.OfType<IFormComponent>().ToList();
        formDataBinder.BindSubmittedData(model, components);

        if (string.IsNullOrWhiteSpace(model.FormDisplayName))
        {
            throw new InvalidOperationException("Form display name cannot be empty.");
        }

        // Create a new form with its own DB table (BizFormHelper handles table name generation and uniqueness)
        var clonedForm = BizFormHelper.Create(model.FormDisplayName, InfoHelper.CODENAME_AUTOMATIC, InfoHelper.CODENAME_AUTOMATIC);

        // Copy field structure from original DataClass to the clone's DataClass
        var originalClass = DataClassInfoProvider.GetDataClassInfo(originalForm.FormClassID);
        var clonedClass = DataClassInfoProvider.GetDataClassInfo(clonedForm.FormClassID);

        var originalFormInfo = new FormInfo(originalClass.ClassFormDefinition);
        var clonedFormInfo = new FormInfo(clonedClass.ClassFormDefinition);

        // Rename the primary key in the original definition to match the clone's primary key
        string originalPkName = originalFormInfo.GetFields<FormFieldInfo>().First(f => f.PrimaryKey).Name;
        string clonedPkName = clonedFormInfo.GetFields<FormFieldInfo>().First(f => f.PrimaryKey).Name;
        var pkField = originalFormInfo.GetFormField(originalPkName);
        pkField.Name = clonedPkName;
        pkField.Caption = clonedPkName;

        // Apply the original form definition (with renamed PK) to the cloned DataClass.
        // SetDataClassInfo will update the DB table to add the new columns.
        clonedClass.ClassFormDefinition = originalFormInfo.GetXmlDefinition();
        clonedClass.ClassContactMapping = originalClass.ClassContactMapping;
        DataClassInfoProvider.SetDataClassInfo(clonedClass);

        // Copy relevant BizFormInfo settings from the original form to the clone
        clonedForm.FormBuilderLayout = originalForm.FormBuilderLayout;
        clonedForm.FormSubmitButtonText = originalForm.FormSubmitButtonText;
        clonedForm.FormSubmitButtonImage = originalForm.FormSubmitButtonImage;
        clonedForm.FormLogActivity = originalForm.FormLogActivity;
        clonedForm.FormAccess = originalForm.FormAccess;
        clonedForm.FormReportFields = originalForm.FormReportFields;
        await BizFormInfo.Provider.SetAsync(clonedForm);

        await CloneAutoresponder(originalForm, clonedForm);
    }

    private async Task CloneAutoresponder(BizFormInfo originalForm, BizFormInfo clonedForm)
    {
        var automationProcess = await autoresponderProcessHelper.GetAutomationProcess(originalForm);
        if (automationProcess is null)
        {
            return;
        }

        var emailSource = await autoresponderProcessHelper.GetEmailSource(automationProcess);
        if (emailSource == AutoresponderEmailSource.None)
        {
            return;
        }

        var selectedEmailGuid = Guid.Empty;
        if (emailSource == AutoresponderEmailSource.Emails)
        {
            var selectedEmail = await autoresponderProcessHelper.GetSelectedEmail(automationProcess);
            selectedEmailGuid = selectedEmail?.EmailConfigurationGUID ?? Guid.Empty;
        }

        await autoresponderProcessHelper.CreateAutomationProcess(clonedForm, selectedEmailGuid);
    }
}
