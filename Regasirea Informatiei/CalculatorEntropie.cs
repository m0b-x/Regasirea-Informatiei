namespace Regasirea_Informatiei;

public static class CalculatorEntropie
{
    public static double CalculeazaValoareaEntropiei(string mesaj)
    {
        double entropia = 0;
        var tabel = new Dictionary<char, double>();


        foreach (var caracter in mesaj)
            if (tabel.ContainsKey(caracter))
                tabel[caracter]++;
            else
                tabel.Add(caracter, 1);
        foreach (var litera in tabel)
        {
            var frecventa = litera.Value / mesaj.Length;
            entropia += frecventa * (Math.Log(frecventa) / Math.Log(2));
        }

        entropia *= -1;
        return entropia;
    }
}