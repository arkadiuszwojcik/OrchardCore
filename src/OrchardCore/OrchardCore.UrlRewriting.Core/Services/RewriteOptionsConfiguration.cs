using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.UrlRewriting.Rules;

namespace OrchardCore.UrlRewriting.Services;

public sealed class RewriteOptionsConfiguration : IConfigureOptions<RewriteOptions>
{
    private readonly IRewriteRulesStore _rewriteRulesStore;
    private readonly IEnumerable<IUrlRewriteRuleSource> _sources;
    private readonly AdminOptions _adminOptions;

    public RewriteOptionsConfiguration(
        IRewriteRulesStore rewriteRulesStore,
        IEnumerable<IUrlRewriteRuleSource> sources,
        IOptions<AdminOptions> adminOptions)
    {
        _rewriteRulesStore = rewriteRulesStore;
        _sources = sources;
        _adminOptions = adminOptions.Value;
    }

    public void Configure(RewriteOptions options)
    {
        var rules = _rewriteRulesStore.GetAllAsync()
            .GetAwaiter()
            .GetResult();

        foreach (var rule in rules.OrderBy(r => r.Order).ThenBy(r => r.CreatedUtc))
        {
            var source = _sources.FirstOrDefault(x => x.Name == rule.Source);

            if (source == null)
            {
                continue;
            }

            source.Configure(options, rule);
        }

        if (options.Rules.Count > 0)
        {
            // Exclude URIs prefixed with 'admin' to prevent accidental access restrictions caused by the provided rules.
            options.Rules.Insert(0, new ExcludeUrlPrefixRule(_adminOptions.AdminUrlPrefix));
        }
    }
}