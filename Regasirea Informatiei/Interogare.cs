namespace Regasirea_Informatiei;
using SF.Snowball.Ext;

public class Interogare
{
    private static readonly DictionarStopWords FisierDictionarStopWords = new();
    private static readonly EnglishStemmer StemmerCuvinte = new();

    public Interogare(string interogare, DictionarGlobal dictionarGlobal)
    {
        DictionarGlobal = dictionarGlobal;
        StringInterogare = interogare;
        CitesteDate();
    }


    public string StringInterogare { get; }


    public Dictionary<string, int> DictionarCuvinte { get; } = new(30000);

    public DictionarGlobal DictionarGlobal { get; }

    public Dictionary<int, int> DictionarNormalizat { get; } = new(30000);

    private void CitesteDate()
    {
        var continutiterogare = StringInterogare;

        TransformaInterogareInCuvinte(continutiterogare);
    }

    private void TransformaInterogareInCuvinte(string continutiterogare)
    {
        var cuvinte = ReturneazaCuvinteleNormalizate(continutiterogare);

        foreach (var cuvant in cuvinte)
        {
            AdaugaCuvantInDictionarNormalizat(DictionarGlobal.ListaCuvinte.IndexOf(cuvant), DictionarNormalizat);
            AdaugaCuvantInDictionar(cuvant, DictionarCuvinte);
        }
    }

    private IEnumerable<string> ReturneazaCuvinteleNormalizate(string continutFisier)
    {
        var cuvinte = continutFisier.Split().Select(cuvant =>
                ReturneazaRadacinaCuvantului(cuvant.StergePunctuatia().ToLowerInvariant().Trim()))
            .Where(cuvant =>UtilitatiCuvinte.EsteCuvantValid(cuvant))
            .Except(FisierDictionarStopWords.ListaStopWords).Distinct();
        return cuvinte;
    }

    private string ReturneazaRadacinaCuvantului(string cuvant)
    {
        StemmerCuvinte.SetCurrent(cuvant);
        StemmerCuvinte.Stem();
        return StemmerCuvinte.GetCurrent();
    }

    private void AdaugaCuvantInDictionarNormalizat(int cuvantIndex, Dictionary<int, int> dictionar)
    {
        if (DictionarNormalizat.ContainsKey(cuvantIndex))
            DictionarNormalizat[cuvantIndex] = dictionar[cuvantIndex] + 1;
        else
            DictionarNormalizat.Add(cuvantIndex, 1);
    }

    private void AdaugaCuvantInDictionar(string cuvant, Dictionary<string, int> dictionar)
    {
        if (DictionarCuvinte.ContainsKey(cuvant))
            DictionarCuvinte[cuvant] = dictionar[cuvant] + 1;
        else
            DictionarCuvinte.Add(cuvant, 1);
    }

    public int ExtrageFrecventaMaxima()
    {
        int max = 0;
        foreach (var cuvant in DictionarCuvinte)
            if (cuvant.Value > max)
                max = cuvant.Value;

        return max;
    }
}