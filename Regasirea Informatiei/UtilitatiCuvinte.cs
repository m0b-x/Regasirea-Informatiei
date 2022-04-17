using System.Text;
using System.Text.RegularExpressions;

namespace Regasirea_Informatiei;

public static class UtilitatiCuvinte
{
    public static bool EsteCuvantValid(string cuvant)
    {
        if (!AreCaractereSpeciale(cuvant) && !ContineCifre(cuvant) && !string.IsNullOrEmpty(cuvant))
            return true;
        return false;
    }

    public static string StergePunctuatia(this string s)
    {
        var cuvantFaraPunctuatie = new StringBuilder();
        foreach (var c in s)
            if (!char.IsPunctuation(c))
                cuvantFaraPunctuatie.Append(c);
        return cuvantFaraPunctuatie.ToString();
    }

    public static bool EsteAbreviere(string cuvant)
    {
        if (cuvant.All(char.IsUpper))
            return true;
        return false;
    }

    public static void InlocuiesteApostrofuri(string paragraf)
    {
        paragraf = paragraf.Replace("'s", String.Empty);
        paragraf = paragraf.Replace("'d", String.Empty);
        paragraf = paragraf.Replace("' ", " ");
    }

    public static bool AreCaractereSpeciale(string cuvant)
    {
        return cuvant.Any(caracter => !char.IsLetterOrDigit(caracter));
    }

    private static bool ContineCifre(string cuvant)
    {
        return Regex.IsMatch(cuvant, @"\d");
    }

    private static bool ContineDoarCaractere(string cuvant)
    {
        foreach (var caracter in cuvant)
            if (char.IsDigit(caracter))
                return false;

        return true;
    }

    public static string StergeSpatiile(string cuvant)
    {
        return new string(cuvant.ToCharArray()
            .Where(c => !char.IsWhiteSpace(c))
            .ToArray());
    }

    private static bool ContineDoarCifre(string cuvant)
    {
        foreach (var caracter in cuvant)
            if (!char.IsDigit(caracter))
                return false;

        return true;
    }

    private static bool ContineDoarLitereMari(string cuvant)
    {
        foreach (var t in cuvant)
            if (char.IsLetter(t) && !char.IsUpper(t))
                return false;

        return true;
    }
}