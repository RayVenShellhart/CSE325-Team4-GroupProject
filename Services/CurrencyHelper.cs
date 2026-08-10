namespace CSE325_Team4_GroupProject.Services;

/// <summary>
/// Maps countries to currencies and formats prices.
/// Product prices in the database are treated as USD (base currency).
/// Rates are approximate static values suitable for a class project (not live FX).
/// </summary>
public static class CurrencyHelper
{
    public record CurrencyInfo(string Code, string Symbol, string CountryName, decimal RateFromUsd);

    // ISO country code -> currency. RateFromUsd = units of local currency per 1 USD.
    private static readonly Dictionary<string, CurrencyInfo> ByCountry =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Africa
            ["NG"] = new("NGN", "₦", "Nigeria", 1600m),
            ["ZA"] = new("ZAR", "R", "South Africa", 18.5m),
            ["KE"] = new("KES", "KSh", "Kenya", 130m),
            ["GH"] = new("GHS", "GH₵", "Ghana", 15.5m),
            ["TZ"] = new("TZS", "TSh", "Tanzania", 2600m),
            ["UG"] = new("UGX", "USh", "Uganda", 3700m),
            ["RW"] = new("RWF", "FRw", "Rwanda", 1300m),
            ["ET"] = new("ETB", "Br", "Ethiopia", 57m),
            ["EG"] = new("EGP", "E£", "Egypt", 49m),
            ["MA"] = new("MAD", "DH", "Morocco", 10m),
            ["DZ"] = new("DZD", "DA", "Algeria", 134m),
            ["TN"] = new("TND", "DT", "Tunisia", 3.1m),
            ["CM"] = new("XAF", "FCFA", "Cameroon", 600m),
            ["SN"] = new("XOF", "CFA", "Senegal", 600m),
            ["CI"] = new("XOF", "CFA", "Côte d'Ivoire", 600m),
            ["BF"] = new("XOF", "CFA", "Burkina Faso", 600m),
            ["ML"] = new("XOF", "CFA", "Mali", 600m),
            ["NE"] = new("XOF", "CFA", "Niger", 600m),
            ["TG"] = new("XOF", "CFA", "Togo", 600m),
            ["BJ"] = new("XOF", "CFA", "Benin", 600m),
            ["GA"] = new("XAF", "FCFA", "Gabon", 600m),
            ["CG"] = new("XAF", "FCFA", "Congo", 600m),
            ["CD"] = new("CDF", "FC", "DR Congo", 2800m),
            ["AO"] = new("AOA", "Kz", "Angola", 850m),
            ["MZ"] = new("MZN", "MT", "Mozambique", 64m),
            ["ZM"] = new("ZMW", "ZK", "Zambia", 27m),
            ["ZW"] = new("USD", "$", "Zimbabwe", 1m),
            ["BW"] = new("BWP", "P", "Botswana", 13.5m),
            ["NA"] = new("NAD", "N$", "Namibia", 18.5m),
            ["MW"] = new("MWK", "MK", "Malawi", 1730m),
            ["MU"] = new("MUR", "₨", "Mauritius", 46m),
            ["SC"] = new("SCR", "₨", "Seychelles", 14m),
            ["MG"] = new("MGA", "Ar", "Madagascar", 4500m),
            ["SD"] = new("SDG", "ج.س", "Sudan", 600m),
            ["SS"] = new("SSP", "£", "South Sudan", 1300m),
            ["LY"] = new("LYD", "LD", "Libya", 4.8m),
            ["LR"] = new("LRD", "L$", "Liberia", 190m),
            ["SL"] = new("SLE", "Le", "Sierra Leone", 22m),
            ["GM"] = new("GMD", "D", "Gambia", 68m),
            ["GN"] = new("GNF", "FG", "Guinea", 8600m),
            ["GW"] = new("XOF", "CFA", "Guinea-Bissau", 600m),
            ["CV"] = new("CVE", "$", "Cape Verde", 100m),
            ["ST"] = new("STN", "Db", "São Tomé and Príncipe", 22m),
            ["GQ"] = new("XAF", "FCFA", "Equatorial Guinea", 600m),
            ["TD"] = new("XAF", "FCFA", "Chad", 600m),
            ["CF"] = new("XAF", "FCFA", "Central African Republic", 600m),
            ["BI"] = new("BIF", "FBu", "Burundi", 2900m),
            ["DJ"] = new("DJF", "Fdj", "Djibouti", 178m),
            ["SO"] = new("SOS", "Sh", "Somalia", 570m),
            ["ER"] = new("ERN", "Nfk", "Eritrea", 15m),
            ["LS"] = new("LSL", "L", "Lesotho", 18.5m),
            ["SZ"] = new("SZL", "E", "Eswatini", 18.5m),
            ["KM"] = new("KMF", "CF", "Comoros", 450m),

            // Americas
            ["US"] = new("USD", "$", "United States", 1m),
            ["CA"] = new("CAD", "C$", "Canada", 1.37m),
            ["MX"] = new("MXN", "MX$", "Mexico", 17.2m),
            ["BR"] = new("BRL", "R$", "Brazil", 5.1m),
            ["AR"] = new("ARS", "$", "Argentina", 900m),
            ["CL"] = new("CLP", "$", "Chile", 950m),
            ["CO"] = new("COP", "$", "Colombia", 4000m),
            ["PE"] = new("PEN", "S/", "Peru", 3.7m),
            ["VE"] = new("VES", "Bs", "Venezuela", 36m),
            ["EC"] = new("USD", "$", "Ecuador", 1m),
            ["BO"] = new("BOB", "Bs", "Bolivia", 6.9m),
            ["PY"] = new("PYG", "₲", "Paraguay", 7500m),
            ["UY"] = new("UYU", "$U", "Uruguay", 39m),
            ["CR"] = new("CRC", "₡", "Costa Rica", 520m),
            ["PA"] = new("USD", "$", "Panama", 1m),
            ["GT"] = new("GTQ", "Q", "Guatemala", 7.8m),
            ["HN"] = new("HNL", "L", "Honduras", 24.7m),
            ["SV"] = new("USD", "$", "El Salvador", 1m),
            ["NI"] = new("NIO", "C$", "Nicaragua", 36.7m),
            ["DO"] = new("DOP", "RD$", "Dominican Republic", 59m),
            ["CU"] = new("CUP", "$", "Cuba", 24m),
            ["JM"] = new("JMD", "J$", "Jamaica", 156m),
            ["TT"] = new("TTD", "TT$", "Trinidad and Tobago", 6.8m),
            ["BB"] = new("BBD", "Bds$", "Barbados", 2m),
            ["BS"] = new("BSD", "B$", "Bahamas", 1m),
            ["HT"] = new("HTG", "G", "Haiti", 132m),
            ["GY"] = new("GYD", "$", "Guyana", 209m),
            ["SR"] = new("SRD", "$", "Suriname", 35m),
            ["BZ"] = new("BZD", "BZ$", "Belize", 2m),

            // Europe
            ["GB"] = new("GBP", "£", "United Kingdom", 0.79m),
            ["IE"] = new("EUR", "€", "Ireland", 0.92m),
            ["FR"] = new("EUR", "€", "France", 0.92m),
            ["DE"] = new("EUR", "€", "Germany", 0.92m),
            ["IT"] = new("EUR", "€", "Italy", 0.92m),
            ["ES"] = new("EUR", "€", "Spain", 0.92m),
            ["PT"] = new("EUR", "€", "Portugal", 0.92m),
            ["NL"] = new("EUR", "€", "Netherlands", 0.92m),
            ["BE"] = new("EUR", "€", "Belgium", 0.92m),
            ["AT"] = new("EUR", "€", "Austria", 0.92m),
            ["FI"] = new("EUR", "€", "Finland", 0.92m),
            ["GR"] = new("EUR", "€", "Greece", 0.92m),
            ["LU"] = new("EUR", "€", "Luxembourg", 0.92m),
            ["MT"] = new("EUR", "€", "Malta", 0.92m),
            ["CY"] = new("EUR", "€", "Cyprus", 0.92m),
            ["SK"] = new("EUR", "€", "Slovakia", 0.92m),
            ["SI"] = new("EUR", "€", "Slovenia", 0.92m),
            ["EE"] = new("EUR", "€", "Estonia", 0.92m),
            ["LV"] = new("EUR", "€", "Latvia", 0.92m),
            ["LT"] = new("EUR", "€", "Lithuania", 0.92m),
            ["HR"] = new("EUR", "€", "Croatia", 0.92m),
            ["CH"] = new("CHF", "CHF", "Switzerland", 0.88m),
            ["SE"] = new("SEK", "kr", "Sweden", 10.5m),
            ["NO"] = new("NOK", "kr", "Norway", 10.7m),
            ["DK"] = new("DKK", "kr", "Denmark", 6.9m),
            ["IS"] = new("ISK", "kr", "Iceland", 138m),
            ["PL"] = new("PLN", "zł", "Poland", 4.0m),
            ["CZ"] = new("CZK", "Kč", "Czech Republic", 23m),
            ["HU"] = new("HUF", "Ft", "Hungary", 360m),
            ["RO"] = new("RON", "lei", "Romania", 4.6m),
            ["BG"] = new("BGN", "лв", "Bulgaria", 1.8m),
            ["RS"] = new("RSD", "дин", "Serbia", 108m),
            ["BA"] = new("BAM", "KM", "Bosnia and Herzegovina", 1.8m),
            ["MK"] = new("MKD", "ден", "North Macedonia", 57m),
            ["AL"] = new("ALL", "L", "Albania", 95m),
            ["UA"] = new("UAH", "₴", "Ukraine", 41m),
            ["MD"] = new("MDL", "L", "Moldova", 17.8m),
            ["BY"] = new("BYN", "Br", "Belarus", 3.3m),
            ["RU"] = new("RUB", "₽", "Russia", 92m),
            ["TR"] = new("TRY", "₺", "Türkiye", 32m),
            ["GE"] = new("GEL", "₾", "Georgia", 2.7m),
            ["AM"] = new("AMD", "֏", "Armenia", 400m),
            ["AZ"] = new("AZN", "₼", "Azerbaijan", 1.7m),

            // Asia / Middle East
            ["CN"] = new("CNY", "¥", "China", 7.2m),
            ["JP"] = new("JPY", "¥", "Japan", 150m),
            ["KR"] = new("KRW", "₩", "South Korea", 1350m),
            ["KP"] = new("KPW", "₩", "North Korea", 900m),
            ["IN"] = new("INR", "₹", "India", 83m),
            ["PK"] = new("PKR", "₨", "Pakistan", 278m),
            ["BD"] = new("BDT", "৳", "Bangladesh", 110m),
            ["LK"] = new("LKR", "Rs", "Sri Lanka", 300m),
            ["NP"] = new("NPR", "Rs", "Nepal", 133m),
            ["BT"] = new("BTN", "Nu", "Bhutan", 83m),
            ["MV"] = new("MVR", "Rf", "Maldives", 15.4m),
            ["AF"] = new("AFN", "؋", "Afghanistan", 70m),
            ["IR"] = new("IRR", "﷼", "Iran", 42000m),
            ["IQ"] = new("IQD", "ع.د", "Iraq", 1310m),
            ["SA"] = new("SAR", "﷼", "Saudi Arabia", 3.75m),
            ["AE"] = new("AED", "د.إ", "United Arab Emirates", 3.67m),
            ["QA"] = new("QAR", "ر.ق", "Qatar", 3.64m),
            ["KW"] = new("KWD", "د.ك", "Kuwait", 0.31m),
            ["BH"] = new("BHD", "BD", "Bahrain", 0.38m),
            ["OM"] = new("OMR", "ر.ع.", "Oman", 0.38m),
            ["YE"] = new("YER", "﷼", "Yemen", 250m),
            ["JO"] = new("JOD", "JD", "Jordan", 0.71m),
            ["LB"] = new("LBP", "ل.ل", "Lebanon", 89500m),
            ["SY"] = new("SYP", "£S", "Syria", 13000m),
            ["IL"] = new("ILS", "₪", "Israel", 3.7m),
            ["PS"] = new("ILS", "₪", "Palestine", 3.7m),
            ["TH"] = new("THB", "฿", "Thailand", 35m),
            ["VN"] = new("VND", "₫", "Vietnam", 24500m),
            ["ID"] = new("IDR", "Rp", "Indonesia", 15800m),
            ["MY"] = new("MYR", "RM", "Malaysia", 4.7m),
            ["SG"] = new("SGD", "S$", "Singapore", 1.34m),
            ["PH"] = new("PHP", "₱", "Philippines", 58m),
            ["MM"] = new("MMK", "K", "Myanmar", 2100m),
            ["KH"] = new("KHR", "៛", "Cambodia", 4100m),
            ["LA"] = new("LAK", "₭", "Laos", 21000m),
            ["BN"] = new("BND", "B$", "Brunei", 1.34m),
            ["TL"] = new("USD", "$", "Timor-Leste", 1m),
            ["MN"] = new("MNT", "₮", "Mongolia", 3400m),
            ["KZ"] = new("KZT", "₸", "Kazakhstan", 450m),
            ["UZ"] = new("UZS", "so'm", "Uzbekistan", 12600m),
            ["TM"] = new("TMT", "m", "Turkmenistan", 3.5m),
            ["KG"] = new("KGS", "с", "Kyrgyzstan", 89m),
            ["TJ"] = new("TJS", "ЅМ", "Tajikistan", 10.9m),
            ["HK"] = new("HKD", "HK$", "Hong Kong", 7.8m),
            ["MO"] = new("MOP", "MOP$", "Macau", 8.0m),
            ["TW"] = new("TWD", "NT$", "Taiwan", 32m),

            // Oceania
            ["AU"] = new("AUD", "A$", "Australia", 1.52m),
            ["NZ"] = new("NZD", "NZ$", "New Zealand", 1.66m),
            ["FJ"] = new("FJD", "FJ$", "Fiji", 2.25m),
            ["PG"] = new("PGK", "K", "Papua New Guinea", 3.8m),
            ["SB"] = new("SBD", "SI$", "Solomon Islands", 8.4m),
            ["VU"] = new("VUV", "VT", "Vanuatu", 120m),
            ["WS"] = new("WST", "WS$", "Samoa", 2.7m),
            ["TO"] = new("TOP", "T$", "Tonga", 2.35m),
            ["TV"] = new("AUD", "A$", "Tuvalu", 1.52m),
            ["KI"] = new("AUD", "A$", "Kiribati", 1.52m),
            ["NR"] = new("AUD", "A$", "Nauru", 1.52m),
            ["PW"] = new("USD", "$", "Palau", 1m),
            ["FM"] = new("USD", "$", "Micronesia", 1m),
            ["MH"] = new("USD", "$", "Marshall Islands", 1m),
        };

    public static IReadOnlyList<(string Code, string Name)> Countries { get; } =
        ByCountry
            .Select(kv => (Code: kv.Key, Name: kv.Value.CountryName))
            .OrderBy(x => x.Name)
            .ToList();

    public static CurrencyInfo GetCurrency(string? countryCode)
    {
        if (!string.IsNullOrWhiteSpace(countryCode) &&
            ByCountry.TryGetValue(countryCode.Trim(), out var info))
        {
            return info;
        }

        return ByCountry["US"];
    }

    /// <summary>
    /// Convert an amount stored in USD to the user's local currency.
    /// </summary>
    public static decimal ConvertFromUsd(decimal amountUsd, string? countryCode)
    {
        var c = GetCurrency(countryCode);
        return Math.Round(amountUsd * c.RateFromUsd, c.Code is "JPY" or "KRW" or "VND" or "IDR" or "UGX" or "TZS" or "RWF" or "CLP" or "PYG" or "ISK" or "HUF" ? 0 : 2);
    }

    public static string Format(decimal amountUsd, string? countryCode)
    {
        var c = GetCurrency(countryCode);
        var local = ConvertFromUsd(amountUsd, countryCode);
        var decimals = c.Code is "JPY" or "KRW" or "VND" or "IDR" or "UGX" or "TZS" or "RWF" or "CLP" or "PYG" or "ISK" or "HUF"
            ? "N0"
            : "N2";
        return $"{c.Symbol}{local.ToString(decimals)}";
    }

    public static string Format(double amountUsd, string? countryCode) =>
        Format((decimal)amountUsd, countryCode);

    public static string CurrencyLabel(string? countryCode)
    {
        var c = GetCurrency(countryCode);
        return $"{c.Code} ({c.Symbol})";
    }
}
