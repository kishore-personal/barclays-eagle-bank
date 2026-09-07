using System.Text.RegularExpressions;
using EagleBank.Infrastructure.Identity;

namespace EagleBank.Tests.Unit;

public class AccountNumberFactoryTests
{
    private static readonly Regex Pattern = new(@"^01\d{6}$", RegexOptions.Compiled);

    [Fact]
    public void Next_matches_the_contract_pattern()
    {
        var factory = new AccountNumberFactory();

        for (var i = 0; i < 20; i++)
        {
            Assert.Matches(Pattern, factory.Next());
        }
    }
}
