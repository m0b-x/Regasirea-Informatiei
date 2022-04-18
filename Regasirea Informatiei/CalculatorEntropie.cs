namespace Regasirea_Informatiei;



public static class CalculatorEntropie
{
    public static double CalculeazaValoareaEntropiei(string mesaj)
    {
        double entropia = 0;
        var dictionarLitere = new Dictionary<char, double>(35);

        foreach (var caracter in mesaj)
            if (dictionarLitere.ContainsKey(caracter))
                dictionarLitere[caracter]++;
            else
                dictionarLitere.Add(caracter, 1);
        foreach (var litera in dictionarLitere)
        {
            double frecventa = litera.Value / mesaj.Length;
            entropia += frecventa * (Math.Log(frecventa) / Math.Log(2));
        }

        entropia *= -1;
        return entropia;
    }
}