using System.Xml;

namespace Regasirea_Informatiei;

public class Articol : IDisposable
{
    public void Dispose()
    {
        _cititorXml.Dispose();
    }

    private static DictionarGlobal DictionarGlobal = new DictionarGlobal();
    private static DocumentGlobal DocumentScriereGlobal = new DocumentGlobal();
    private static StopWords FisierStopWords = new StopWords();
    
    private readonly string _numeFisier;
    private readonly XmlTextReader _cititorXml;
    private Dictionary<string, int> _dictionarCuvinte = new Dictionary<string, int>();

    public Articol(String numeFisier)
    {
        try
        {
            _numeFisier = numeFisier;
            _cititorXml = new XmlTextReader(@_numeFisier);
        }
        catch (Exception exceptie)
        {
            Console.WriteLine(@"Exceptie citire fisier: {0}", exceptie);
        }
    }
    
    public void CitesteCuvinte()
    {
        while (_cititorXml.Read())
        {
            if (_cititorXml.NodeType == XmlNodeType.Element && _cititorXml.Name == "p")
            {
                String continutFisier = (_cititorXml.ReadElementString());
                UtilitatiCuvinte.InlocuiesteApostrof(continutFisier);

                var punctuatii = continutFisier.Where(Char.IsPunctuation).Distinct().ToArray();
                IEnumerable<string> cuvinte = continutFisier.Split().Select(punctuatie => punctuatie.Trim(punctuatii));

                cuvinte = cuvinte.Select(cuvant => cuvant.ToLowerInvariant()).ToArray()
                    .Where(cuvant => !string.IsNullOrEmpty(cuvant)).Except(FisierStopWords.ListaStopWords);

                foreach (string cuvant in cuvinte)
                {
                    if (UtilitatiCuvinte.EsteCuvantValid(cuvant))
                    {
                        if (UtilitatiCuvinte.EsteAbreviere(cuvant) == false)
                        {
                            AdaugaCuvantInDictionar(cuvant, _dictionarCuvinte);
                            DictionarGlobal.AdaugaCuvantInLista(cuvant);
                            DocumentScriereGlobal.AdaugaAtributInLista(cuvant);
                        }
                    }
                }
            }
        }
        
    }

    public void AdaugaCuvantInDictionar(string cuvant, Dictionary<string, int> dictionar)
    {
        if (_dictionarCuvinte.ContainsKey(cuvant))
        {
                _dictionarCuvinte[cuvant] = dictionar[cuvant] + 1;
        }
        else
        {
            _dictionarCuvinte.Add(cuvant,1);
        }
    }

    public static void ScrieArticoleInFiserGlobal()
    {
        DictionarGlobal.ScrieCuvinteleInFisier();
        DocumentScriereGlobal.ScrieDate();
    }
    

}