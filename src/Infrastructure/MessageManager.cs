using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Labelissimo.Infrastructure;

public class MessageManager
{
    private readonly CultureInfo _currentCultureInfo = null!;
    private IStringLocalizer _localizer = null!;

    public IStringLocalizer Localizer
    {
        get
        {
            CultureInfo.CurrentCulture = _currentCultureInfo;
            CultureInfo.CurrentUICulture = _currentCultureInfo;
            return _localizer;
        }
    }

    public MessageManager(string culture)
    {
        _currentCultureInfo = new CultureInfo(culture);
    }
}