using System.Diagnostics;
using System.Text;
using System.Xml;
using SF.Snowball.Ext;

namespace Regasirea_Informatiei;

public class Articol
{
    public static DictionarGlobal DictionarGlobal = new();
    public static DocumentGlobal DocumentGlobal = new();

    private readonly DictionarStopWords _fisierDictionarStopWords = new();
    private readonly EnglishStemmer _stemmerCuvinte = new();

    private readonly string _pathFisier;

    private StringBuilder _documentNormalizat = new(5000);

    public Articol(string pathFisier)
    {
        _pathFisier = pathFisier;
        CitesteDate();
    }

    public string Titlu { get; private set; }

    public Dictionary<string, int> DictionarCuvinte { get; } = new(30000);


    private void CitesteDate()
    {
        var continutFisier = new StringBuilder(50000);
        using (var _cititorXml = new XmlTextReader(_pathFisier))
        {
            while (_cititorXml.Read())
            {
                if (_cititorXml.NodeType == XmlNodeType.Element)
                {
                    switch (_cititorXml.Name)
                    {
                        case "p":
                            continutFisier.Append(_cititorXml.ReadElementString());
                            break;
                        case "title":
                            Titlu = _cititorXml.ReadElementString();
                            break;
                    }
                }
            }
        }

        TransformaArticolInCuvinte(continutFisier);
        RealizeazaFormaVectoriala();
    }

    private void RealizeazaFormaVectoriala()
    {;
        _documentNormalizat.Append($"{Titlu}# ");
        foreach (var cuvant in DictionarCuvinte)
            _documentNormalizat.Append($"{DictionarGlobal.ListaCuvinte.IndexOf(cuvant.Key)}:{cuvant.Value} ");

        DocumentGlobal.AdaugaDocumentInLista(_documentNormalizat.ToString());
    }

    private void TransformaArticolInCuvinte(StringBuilder continutFisier)
    {
        var listaCuvinte = ReturneazaCuvinteleNormalizate(continutFisier.ToString());

        DocumentGlobal.AdaugaAtributeDinArticol(listaCuvinte);
        DictionarGlobal.AdaugaCuvinteInLista(listaCuvinte);

        foreach (var cuvant in listaCuvinte) AdaugaCuvantInDictionar(cuvant, DictionarCuvinte);
    }

    private List<string> ReturneazaCuvinteleNormalizate(string continutFisier)
    {
        var cuvinte = continutFisier.Split().Select(cuvant =>
                ReturneazaRadacinaCuvantului(
                    cuvant.StergePunctuatia().ToLowerInvariant().Trim()))
            .Where(cuvant => UtilitatiCuvinte.EsteCuvantValid(cuvant))
            .Except(_fisierDictionarStopWords.ListaStopWords).Distinct().ToList();
        return cuvinte;
    }

    private string ReturneazaRadacinaCuvantului(string cuvant)
    {
        _stemmerCuvinte.SetCurrent(cuvant);
        _stemmerCuvinte.Stem();
        return _stemmerCuvinte.GetCurrent();
    }

    private void AdaugaCuvantInDictionar(string cuvant, Dictionary<string, int> dictionar)
    {
        if (dictionar.ContainsKey(cuvant))
            dictionar[cuvant] = dictionar[cuvant] + 1;
        else
            dictionar.Add(cuvant, 1);
    }

    public static void ScrieDateInFisiereGlobale()
    {
        DictionarGlobal.ScrieCuvinteleInFisier();
        DocumentGlobal.ScrieDate();
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