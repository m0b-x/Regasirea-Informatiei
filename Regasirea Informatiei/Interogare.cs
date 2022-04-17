namespace Regasirea_Informatiei;

public class Interogare
{
    private static readonly DictionarStopWords FisierDictionarStopWords = new();
    private static readonly SnowballStemmer StemmerCuvinte = new();

    public Interogare(string interogare, DictionarGlobal dictionarGlobal)
    {
        DictionarGlobal = dictionarGlobal;
        StringInterogare = interogare;
        CitesteDate();
    }


    public string StringInterogare { get; }


    public Dictionary<string, int> DictionarCuvinte { get; } = new();

    public DictionarGlobal DictionarGlobal { get; }

    public Dictionary<int, int> DictionarNormalizat { get; } = new();

    public void CitesteDate()
    {
        var continutiterogare = StringInterogare;

        TransformaInterogareInCuvinte(continutiterogare);
    }

    private void TransformaInterogareInCuvinte(string continutiterogare)
    {
        var cuvinte = ReturneazaCuvinteleNormalizate(continutiterogare);

        var listaCuvinte = cuvinte.ToList();

        foreach (var cuvant in listaCuvinte)
        {
            AdaugaCuvantInDictionarNormalizat(DictionarGlobal.ListaCuvinte.IndexOf(cuvant), DictionarNormalizat);
            AdaugaCuvantInDictionar(cuvant, DictionarCuvinte);
        }
    }

    private IEnumerable<string> ReturneazaCuvinteleNormalizate(string continutFisier)
    {
        var cuvinte = continutFisier.Split().Select(cuvant =>
                ReturneazaRadacinaCuvantului(
                    cuvant.StergePunctuatia().ToLowerInvariant().Trim()))
            .Where(cuvant => !string.IsNullOrEmpty(cuvant) &&
                             UtilitatiCuvinte.EsteCuvantValid(cuvant))
            .Except(FisierDictionarStopWords.ListaStopWords).Distinct();
        return cuvinte;
    }

    public string ReturneazaRadacinaCuvantului(string cuvant)
    {
        return StemmerCuvinte.Stem(cuvant);
    }

    public void AdaugaCuvantInDictionarNormalizat(int cuvantIndex, Dictionary<int, int> dictionar)
    {
        if (DictionarNormalizat.ContainsKey(cuvantIndex))
            DictionarNormalizat[cuvantIndex] = dictionar[cuvantIndex] + 1;
        else
            DictionarNormalizat.Add(cuvantIndex, 1);
    }

    public void AdaugaCuvantInDictionar(string cuvant, Dictionary<string, int> dictionar)
    {
        if (DictionarCuvinte.ContainsKey(cuvant))
            DictionarCuvinte[cuvant] = dictionar[cuvant] + 1;
        else
            DictionarCuvinte.Add(cuvant, 1);
    }

    public int ExtrageFrecventaMaxima()
    {
        var max = -1;
        foreach (var cuvant in DictionarCuvinte)
            if (cuvant.Value > max)
                max = cuvant.Value;

        return max;
    }
}