using System.Text;
using System.Text.RegularExpressions;

namespace Regasirea_Informatiei;

public static class UtilitatiCuvinte
{
    public static bool EsteCuvantValid(string cuvant)
    {
        if (string.IsNullOrEmpty(cuvant))
            return false;
        if (ContineCifre(cuvant))
            return false;
        if (AreCaractereSpeciale(cuvant))
            return false;
        return true;
    }

    public static string InlocuiestePunctuatia(this string document)
    {
        return Regex.Replace(document, @"[^\w\s]", Constante.InlocuitorPunctuatie);
    }

    public static bool AreCaractereSpeciale(string cuvant)
    {
        return  Regex.IsMatch(cuvant,("[^a-zA-Z0-9_.]"));
    }

    private static bool ContineCifre(string cuvant)
    {
        return Regex.IsMatch(cuvant, @"\d");
    }
}