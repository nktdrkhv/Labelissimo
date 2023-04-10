using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Labelissimo.Services;

public class TextService
{
    private CultureInfo _currentCultureInfo = null!;
    private IStringLocalizer _localizer;

    public TextService(IStringLocalizer stringLocalizer) => _localizer = stringLocalizer;

    public IStringLocalizer Localizer
    {
        get
        {
            CultureInfo.CurrentCulture = _currentCultureInfo;
            CultureInfo.CurrentUICulture = _currentCultureInfo;
            return _localizer;
        }
    }
}