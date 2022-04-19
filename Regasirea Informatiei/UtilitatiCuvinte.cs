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

    public static string InlocuiestePunctuatia(this string s)
    {
        var cuvantFaraPunctuatie = new StringBuilder();
        foreach (var c in s)
            if (!char.IsPunctuation(c))
                cuvantFaraPunctuatie.Append(c);
            else
                cuvantFaraPunctuatie.Append(' ');
        return cuvantFaraPunctuatie.ToString();
    }

    public static bool EsteAbreviere(string cuvant)
    {
        if (cuvant.All(char.IsUpper))
            return true;
        return false;
    }

    public static bool AreCaractereSpeciale(string cuvant)
    {
        return  Regex.IsMatch(cuvant,("[^a-zA-Z0-9_.]"));
    }

    private static bool ContineCifre(string cuvant)
    {
        return Regex.IsMatch(cuvant, @"\d");
    }
    public static string StergeSpatiile(string cuvant)
    {
        return new string(cuvant.ToCharArray()
            .Where(c => !char.IsWhiteSpace(c))
            .ToArray());
    }
}