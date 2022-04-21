namespace Regasirea_Informatiei;

using Accord.MachineLearning.Text.Stemmers;
public class Interogare
{
    private static readonly EnglishStemmer StemmerCuvinte = new();

    public string StringInterogare { get; }

    public Dictionary<string, int> DictionarCuvinte { get; } = new(Constante.NumarCuvinteEstimatDocument);

    public DictionarGlobal DictionarGlobal { get; }

    public Dictionary<int, int> DictionarNormalizat { get; } = new(Constante.NumarEstimatCuvinteInterogare);

    public int FrecventaMaxima = 1;
    
    public Interogare(string interogare, DictionarGlobal dictionarGlobal)
    {
        DictionarGlobal = dictionarGlobal;
        StringInterogare = interogare;
        CitesteDate();
    }


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
            AdaugaCuvantDistinctInDictionar(cuvant, DictionarCuvinte);
        }
    }

    private List<string> ReturneazaCuvinteleNormalizate(string continutFisier)
    {
        var cuvinte = continutFisier.InlocuiestePunctuatia().ToLowerInvariant().
            Split(Constante.DelimitatorGeneral)
            .Where(cuvant => UtilitatiCuvinte.EsteCuvantValid(cuvant) &&
                             !DictionarGlobal.DictionarStopWords.ListaStopWords.Contains(cuvant))
            .Select(cuvant => ReturneazaRadacinaCuvantului(cuvant));

        return cuvinte.ToList();
    }

    private string ReturneazaRadacinaCuvantului(string cuvant)
    {
        return StemmerCuvinte.Stem(cuvant);
    }

    private void AdaugaCuvantInDictionarNormalizat(int cuvantIndex, Dictionary<int, int> dictionar)
    {
        if (DictionarNormalizat.ContainsKey(cuvantIndex))
            DictionarNormalizat[cuvantIndex] = dictionar[cuvantIndex] + 1;
        else
            DictionarNormalizat.Add(cuvantIndex, 1);
    }

    private void AdaugaCuvantDistinctInDictionar(string cuvant, Dictionary<string, int> dictionar)
    {
        if (DictionarCuvinte.ContainsKey(cuvant))
        {
            DictionarCuvinte[cuvant] = dictionar[cuvant] + 1;
            if (DictionarCuvinte[cuvant] > FrecventaMaxima)
                FrecventaMaxima = DictionarCuvinte[cuvant];
        }
        else
        {
            DictionarCuvinte.Add(cuvant, 1);
        }
        
    }
}