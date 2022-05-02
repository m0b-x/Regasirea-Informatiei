namespace Regasirea_Informatiei;

using Accord.MachineLearning.Text.Stemmers;
public class Interogare
{
    private readonly EnglishStemmer _stemmerCuvinte = new();

    private string StringInterogare { get; }

    public Dictionary<string, int> DictionarCuvinte { get; } = new(Constante.NumarCuvinteEstimatDocument);

    private DictionarGlobal DictionarGlobal { get; }

    private Dictionary<int, int> DictionarNormalizat { get; } = new(Constante.NumarEstimatCuvinteInterogare);

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
        var cuvinte = continutiterogare.InlocuiestePunctuatia().ToLowerInvariant()
            .Split(Constante.DelimitatorGeneral, StringSplitOptions.RemoveEmptyEntries).ToList();

        foreach (var cuvantNetransformat in cuvinte)
        {
            if (UtilitatiCuvinte.EsteCuvantValid(cuvantNetransformat))
            {
                if (!DictionarGlobal.DictionarStopWords.ListaStopWords.Contains(cuvantNetransformat))
                {
                    string cuvantTransformat = ReturneazaRadacinaCuvantului(cuvantNetransformat);
                    AdaugaCuvantInDictionarNormalizat(DictionarGlobal.ListaCuvinte.IndexOf(cuvantTransformat), DictionarNormalizat);
                    AdaugaCuvantDistinctInDictionar(cuvantTransformat, DictionarCuvinte);
                }
            }
        }
    }
    private string ReturneazaRadacinaCuvantului(string cuvant)
    {
        return _stemmerCuvinte.Stem(cuvant);
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