using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Shell;
using OrchardCore.Mvc.ModelBinding;
using OrchardCore.UrlRewriting.Models;
using OrchardCore.UrlRewriting.ViewModels;

namespace OrchardCore.UrlRewriting.Drivers;

public sealed class RewriteRulesDisplayDriver : DisplayDriver<RewriteRule>
{
    internal readonly IStringLocalizer S;
    private readonly IShellReleaseManager _shellReleaseManager;

    public RewriteRulesDisplayDriver(
        IStringLocalizer<RewriteRulesDisplayDriver> stringLocalizer,
        IShellReleaseManager shellReleaseManager)
    {
        S = stringLocalizer;
        _shellReleaseManager = shellReleaseManager;
    }

    public override Task<IDisplayResult> DisplayAsync(RewriteRule rule, BuildDisplayContext context)
    {
        return CombineAsync(
            View("RewriteRule_Fields_SummaryAdmin", rule).Location("Content:1"),
            View("RewriteRule_Buttons_SummaryAdmin", rule).Location("Actions:5"),
            View("RewriteRule_DefaultTags_SummaryAdmin", rule).Location("Tags:5"),
            View("RewriteRule_DefaultMeta_SummaryAdmin", rule).Location("Meta:5")
        );
    }

    public override IDisplayResult Edit(RewriteRule rule, BuildEditorContext context)
    {
        context.AddTenantReloadWarningWrapper();

        return Initialize<EditRewriteRuleViewModel>("RewriteRule_Fields_Edit", model =>
        {
            model.Name = rule.Name;
            model.Source = rule.Source;
            model.Order = rule.Order;
        }).Location("Content:1");
    }

    public override async Task<IDisplayResult> UpdateAsync(RewriteRule rule, UpdateEditorContext context)
    {
        await context.Updater.TryUpdateModelAsync(rule, Prefix,
            m => m.Name,
            m => m.Source,
            m => m.Order);

        if (string.IsNullOrEmpty(rule.Name))
        {
            context.Updater.ModelState.AddModelError(Prefix, nameof(rule.Name), S["Name is required"]);
        }

        _shellReleaseManager.RequestRelease();

        return Edit(rule, context);
    }
}