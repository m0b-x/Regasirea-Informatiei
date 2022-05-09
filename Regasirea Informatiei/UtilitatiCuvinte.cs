using System.Text;
using System.Text.RegularExpressions;

namespace Regasirea_Informatiei;

public static class UtilitatiCuvinte
{
    public static bool EsteCuvantValid(string cuvant)
    {
        if (cuvant.Length < 2)
            return false;
        if (AreCaractereSpeciale(cuvant))
            return false;
        return true;
    }

    public static string InlocuiestePunctuatia(this string sir)
    {
        var sirInlocuit = new StringBuilder(Constante.LungimeMedieCuvant*Constante.NumarEstimatCuvinteInterogare);

        foreach (var caracter in sir)
        {
            if (!char.IsPunctuation(caracter))
            {
                sirInlocuit.Append(caracter);
            }
            else
            {
                sirInlocuit.Append(Constante.InlocuitorPunctuatie);
            }
        }
        return sirInlocuit.ToString();
    }
  
    public static string InlocuiesteCifrele(this string sir)  
    {
        var sirInlocuit = new StringBuilder(Constante.LungimeMedieCuvant);

        foreach (var caracter in sir)
        {
            if (!char.IsDigit(caracter))
            {
                sirInlocuit.Append(caracter);
            }
        }
        return sirInlocuit.ToString();
    }
    private static bool AreCaractereSpeciale(string cuvant)
    {
        return  Regex.IsMatch(cuvant,("[^a-zA-Z0-9_.]"));
    }
}