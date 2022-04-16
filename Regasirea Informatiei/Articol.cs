using System.Text;
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
    private StringBuilder _documentNormalizat = new StringBuilder();
    private readonly XmlTextReader _cititorXml;
    private Dictionary<string, int> _dictionarCuvinte = new Dictionary<string, int>();


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
        CitesteTitlu();
        CitesteCuvinte();
        RealizeazaFormaVectoriala();
    }

    public void CitesteTitlu()
    {
        while (_cititorXml.Read())
        {
            if (_cititorXml.NodeType == XmlNodeType.Element && _cititorXml.Name == "title")
            {
                _titlu = (_cititorXml.ReadElementString());
                break;
            }
        }
    }

    public void CitesteCuvinte()
    {
        while (_cititorXml.Read())
        {
            if (_cititorXml.NodeType == XmlNodeType.Element && _cititorXml.Name == "p")
            {
                String continutFisier = (_cititorXml.ReadElementString());

                var punctuatii = continutFisier.Where(Char.IsPunctuation).Distinct().ToArray();
                IEnumerable<string> cuvinte = continutFisier.Split().Select(punctuatie => punctuatie.Trim(punctuatii));

                cuvinte = cuvinte.Select(cuvant => cuvant.ToLowerInvariant().Replace(" ", "")).ToList()
                    .Where(cuvant => !string.IsNullOrEmpty(cuvant) &&
                                     UtilitatiCuvinte.EsteCuvantValid(cuvant) &&
                                     !UtilitatiCuvinte.EsteAbreviere(cuvant))
                    .Except(_fisierDictionarStopWords.ListaStopWords).Distinct();

                cuvinte = ReturneazaRadacinileCuvintelor(cuvinte);

                DocumentScriereGlobal.AdaugaAtributeInLista(cuvinte);
                DictionarGlobal.AdaugaCuvinteInLista(cuvinte);

                foreach (string cuvant in cuvinte)
                {
                    AdaugaCuvantInDictionar(cuvant, _dictionarCuvinte);
                }
            }
        }
    }

    public IEnumerable<string> ReturneazaRadacinileCuvintelor(IEnumerable<string> cuvinte)
    {
        List<string> cuvinteStemate = new List<string>();

        foreach (var cuvant in cuvinte)
        {
            if (!cuvinteStemate.Contains(cuvant))
                cuvinteStemate.Add(_stemmerCuvinte.Stem(cuvant));
        }

        return cuvinteStemate.AsEnumerable();
    }

    public void RealizeazaFormaVectoriala()
    {
        _documentNormalizat = new StringBuilder();
        _documentNormalizat.Append($"{_titlu}# ");
        foreach (var cuvant in _dictionarCuvinte)
        {
            _documentNormalizat.Append($"{DictionarGlobal.ListaCuvinte.IndexOf(cuvant.Key)}:{cuvant.Value} ");
        }

        DocumentScriereGlobal.AdaugaDocumentInLista(_titlu, _documentNormalizat.ToString());
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