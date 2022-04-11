namespace Regasirea_Informatiei;

public static class UtilitatiCuvinte
{
    public static bool EsteCuvantValid(string? cuvant)
    {
        if (!AreCaractereSpeciale(cuvant))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool EsteAbreviere(string? cuvant)
    {
        if (cuvant.All<char>(Char.IsUpper))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void InlocuiesteApostrof(string paragraf)
    {
        _ = paragraf.Replace("'s", "");
        _ = paragraf.Replace("'d", "");
        _ = paragraf.Replace("' ", " ");
    }

    public static bool AreCaractereSpeciale(string? cuvant)
    {
        return cuvant.Any(caracter => !Char.IsLetterOrDigit(caracter));
    }

    private static bool ContineDoarCaractere(string cuvant)
    {
        foreach (Char caracter in cuvant)
        {
            if (Char.IsDigit(caracter))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ContineDoarLitereMari(string cuvant)
    {
        for (int caracter = 0; caracter < cuvant.Length; caracter++)
        {
            if (Char.IsLetter(cuvant[caracter]) && !Char.IsUpper(cuvant[caracter])) return false;
        }

        return true;
    }
}