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
    public static DictionarGlobal DictionarGlobal = new();
    public static DocumentGlobal DocumentScriereGlobal = new();

    private static readonly DictionarStopWords _fisierDictionarStopWords = new();
    private static readonly SnowballStemmer _stemmerCuvinte = new();

    private readonly string _numeFisier;
    private readonly XmlTextReader _cititorXml;


    public string Titlu { get; private set; }

    public Dictionary<string, int> DictionarCuvinte { get; } = new(30000);

    public Articol(string numeFisier)
    {
        _numeFisier = numeFisier;
        try
        {
            _cititorXml = new XmlTextReader($@"{NumeFisierDocumente}/{_numeFisier}");
            CitesteDate();
        }
        catch (Exception exceptie)
        {
            Console.WriteLine(@"Exceptie citire fisier: {0}", exceptie);
        }

    }

    public void CitesteDate()
    {
        var continutFisier = new StringBuilder(50000);
        using (_cititorXml)
        {
            while (_cititorXml.Read())
                if (_cititorXml.NodeType == XmlNodeType.Element && _cititorXml.Name == "p")
                    continutFisier.Append(_cititorXml.ReadElementString());
                else if (_cititorXml.NodeType == XmlNodeType.Element && _cititorXml.Name == "title")
                    Titlu = _cititorXml.ReadElementString();
        }

        TransformaArticolInCuvinte(continutFisier);
    }

    private void TransformaArticolInCuvinte(StringBuilder continutFisier)
    {
        var cuvinte = ReturneazaCuvinteleNormalizate(continutFisier.ToString());

        var listaCuvinte = cuvinte.ToList();
        DocumentScriereGlobal.AdaugaAtributeInLista(listaCuvinte);
        DictionarGlobal.AdaugaCuvinteInLista(listaCuvinte);

        foreach (var cuvant in listaCuvinte)
        {
            AdaugaCuvantInDictionar(cuvant, DictionarCuvinte);
            Console.WriteLine(cuvant);
        }
    }

    private IEnumerable<string> ReturneazaCuvinteleNormalizate(string continutFisier)
    {
        var cuvinte = continutFisier.Split().Select(cuvant =>
                ReturneazaRadacinaCuvantului(
                    cuvant.StergePunctuatia().ToLowerInvariant().Trim()))
            .Where(cuvant => !string.IsNullOrEmpty(cuvant) &&
                             UtilitatiCuvinte.EsteCuvantValid(cuvant))
            .Except(_fisierDictionarStopWords.ListaStopWords).Distinct();
        return cuvinte;
    }

    public string ReturneazaRadacinaCuvantului(string cuvant)
    {
        return _stemmerCuvinte.Stem(cuvant);
    }

    public void AdaugaCuvantInDictionar(string cuvant, Dictionary<string, int> dictionar)
    {
        if (DictionarCuvinte.ContainsKey(cuvant))
            DictionarCuvinte[cuvant] = dictionar[cuvant] + 1;
        else
            DictionarCuvinte.Add(cuvant, 1);
    }

    public static void ScrieArticoleInFiserGlobal()
    {
        DictionarGlobal.ScrieCuvinteleInFisier();
        DocumentScriereGlobal.ScrieDate();
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