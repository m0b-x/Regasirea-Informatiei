using System.Xml;

namespace Regasirea_Informatiei;

public class Articol : IDisposable
{
    public void Dispose()
    {
        _cititorXml.Dispose();
    }

    public static string NumeFisierDocumente = "Documente";
    public static DictionarGlobal DictionarGlobal = new DictionarGlobal();
    public static DocumentGlobal DocumentScriereGlobal = new DocumentGlobal();

    private static DictionarStopWords _fisierDictionarStopWords = new DictionarStopWords();
    private static SnowballStemmer _stemmerCuvinte = new();

    private readonly string _numeFisier;
    private string _titlu;
    private readonly XmlTextReader _cititorXml;
    private Dictionary<string, int> _dictionarCuvinte = new Dictionary<string, int>(30000);


    public String Titlu
    {
        get { return _titlu; }
    }

    public Dictionary<string, int> DictionarCuvinte
    {
        get { return _dictionarCuvinte; }
    }

    public Articol(String numeFisier)
    {
        _numeFisier = numeFisier;
        try
        {
            _cititorXml = new XmlTextReader($@"{NumeFisierDocumente}/{_numeFisier}");
        }
        catch (Exception exceptie)
        {
            Console.WriteLine(@"Exceptie citire fisier: {0}", exceptie);
        }

        CitesteDate();
    }

    public void CitesteDate()
    {
        while (_cititorXml.Read())
        {
            if (_cititorXml.NodeType == XmlNodeType.Element && _cititorXml.Name == "p")
            {
                String continutFisier = (_cititorXml.ReadElementString());

                var cuvinte = ReturneazaCuvinteleNormalizate(continutFisier);

                var listaCuvinte = cuvinte.ToList();
                DocumentScriereGlobal.AdaugaAtributeInLista(listaCuvinte);
                DictionarGlobal.AdaugaCuvinteInLista(listaCuvinte);

                foreach (string cuvant in listaCuvinte)
                {
                    AdaugaCuvantInDictionar(cuvant, _dictionarCuvinte);
                }
            }
            else if (_cititorXml.NodeType == XmlNodeType.Element && _cititorXml.Name == "title")
            {
                _titlu = (_cititorXml.ReadElementString());
            }
        }
    }

    private IEnumerable<string> ReturneazaCuvinteleNormalizate(string continutFisier)
    {
        IEnumerable<string> cuvinte = continutFisier.Split().Select(cuvant =>
                ReturneazaRadacinaCuvantului(
                    UtilitatiCuvinte.StergePunctuatia(cuvant).ToLowerInvariant().Trim()))
            .Where(cuvant => !string.IsNullOrEmpty(cuvant) &&
                             UtilitatiCuvinte.EsteCuvantValid(cuvant) &&
                             !UtilitatiCuvinte.EsteAbreviere(cuvant))
            .Except(_fisierDictionarStopWords.ListaStopWords).Distinct();
        return cuvinte;
    }

    public string ReturneazaRadacinaCuvantului(string cuvant)
    {
        return _stemmerCuvinte.Stem(cuvant);
    }

    public void AdaugaCuvantInDictionar(string cuvant, Dictionary<string, int> dictionar)
    {
        if (_dictionarCuvinte.ContainsKey(cuvant))
        {
            _dictionarCuvinte[cuvant] = dictionar[cuvant] + 1;
        }
        else
        {
            _dictionarCuvinte.Add(cuvant, 1);
        }
    }

    public static void ScrieArticoleInFiserGlobal()
    {
        DictionarGlobal.ScrieCuvinteleInFisier();
        DocumentScriereGlobal.ScrieDate();
    }

    public int ExtrageFrecventaMaxima()
    {
        int max = -1;
        foreach (var cuvant in _dictionarCuvinte)
        {
            if (cuvant.Value > max)
            {
                max = cuvant.Value;
            }
        }

        return max;
    }
}