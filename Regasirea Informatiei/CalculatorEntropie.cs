namespace Regasirea_Informatiei;

public static class CalculatorEntropie
{
    public static double CalculeazaValoareaEntropiei(string mesaj)
    {			
        double entropia=0;
        Dictionary<char,double> tabel = new Dictionary<char, double>();
 
 
        foreach (char caracter in mesaj)
        {
            if (tabel.ContainsKey(caracter))
                tabel[caracter]++;
            else
                tabel.Add(caracter,1);
 
        }
        double frecventa;
        foreach (KeyValuePair<char,double> litera in tabel)
        {
            frecventa=litera.Value/mesaj.Length;
            entropia += frecventa * (Math.Log(frecventa) / Math.Log(2));
        }
        entropia*=-1;
        return entropia;
    }
}