using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace XperienceCommunity.FormClone;

internal sealed class CloneFormModel
{
    [TextInputComponent(Label = "Form name", Order = 1)]
    [RequiredValidationRule]
    public string FormDisplayName { get; set; } = string.Empty;
}
