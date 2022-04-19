using System.Text;
using System.Xml;
using SF.Snowball.Ext;

namespace Regasirea_Informatiei;

public class Articol
{
    public static DictionarGlobal DictionarGlobal = new();
    public static DocumentGlobal DocumentGlobal = new(DictionarGlobal);

    public string Titlu { get; private set; }
    public Dictionary<string, int> DictionarCuvinte { get; } = new(30000);
    private readonly EnglishStemmer _stemmerCuvinte = new();

    private readonly string _pathFisier;

    private StringBuilder _documentNormalizat = new(5000);


    public Articol(string pathFisier)
    {
        _pathFisier = pathFisier;
        CitesteDate();
    }

    public Articol(StringBuilder documentNormalizat)
    {
        _pathFisier = "NONE";

        _documentNormalizat = documentNormalizat;
        var dateDocumet = documentNormalizat.ToString().Split('#');

        Titlu = dateDocumet[0];
        var dateDocument = dateDocumet[1];
        var separatori = new[] {':', ' '};
        var dateCaString = dateDocument.Split(separatori, StringSplitOptions.RemoveEmptyEntries);
        List<int> dateCaNumere = new(5000);
        foreach (var data in dateCaString)
        {
            dateCaNumere.Add(int.Parse(data));
        }

        for (var index = 0; index < dateCaNumere.Count - 1; index += 2)
        {
            AdaugaCuvantInDictionar(DictionarGlobal.ListaCuvinte[dateCaNumere[index]], dateCaNumere[index + 1],
                DictionarCuvinte);
        }
    }


    private void CitesteDate()
    {
        var continutFisier = new StringBuilder(50000);
        using (var cititorXml = new XmlTextReader(_pathFisier))
        {
            while (cititorXml.Read())
            {
                if (cititorXml.NodeType == XmlNodeType.Element)
                {
                    switch (cititorXml.Name)
                    {
                        case "p":
                            continutFisier.Append(cititorXml.ReadElementString());
                            break;
                        case "title":
                            Titlu = cititorXml.ReadElementString();
                            break;
                    }
                }
            }
        }

        TransformaArticolInCuvinte(continutFisier);
        RealizeazaFormaVectoriala();
    }

    private void AdaugaCuvantInDictionar(string cuvant, int frecventa, Dictionary<string, int> dictionar)
    {
        dictionar.Add(cuvant, frecventa);
    }

    private void RealizeazaFormaVectoriala()
    {
        _documentNormalizat.Append($"{Titlu}# ");

        foreach (var cuvant in DictionarCuvinte)
        {
            _documentNormalizat.Append($"{DictionarGlobal.ListaCuvinte.IndexOf(cuvant.Key)}:{cuvant.Value} ");
        }

        _documentNormalizat.Remove(_documentNormalizat.Length - 1, 1);
        DocumentGlobal.AdaugaDocumentInLista(_documentNormalizat.ToString());
    }

    private void TransformaArticolInCuvinte(StringBuilder continutFisier)
    {
        var listaCuvinte = ReturneazaCuvinteleNormalizate(continutFisier.ToString());
        AdaugaCuvinteleInDictionar(listaCuvinte);
        DocumentGlobal.AdaugaAtributeDinArticol(listaCuvinte);
        DictionarGlobal.AdaugaCuvinteInLista(listaCuvinte);
    }

    private void AdaugaCuvinteleInDictionar(List<string> listaCuvinte)
    {
        foreach (var cuvant in listaCuvinte)
        {
            AdaugaCuvantInDictionar(cuvant, DictionarCuvinte);
        }
    }

    private List<string> ReturneazaCuvinteleNormalizate(string continutFisier)
    {
        var cuvinte = continutFisier.InlocuiestePunctuatia().ToLowerInvariant().
            Split(' ',StringSplitOptions.TrimEntries)
            .Where(cuvant => UtilitatiCuvinte.EsteCuvantValid(cuvant) &&
                             !DictionarGlobal.DictionarStopWords.ListaStopWords.Contains(cuvant))
            .Select(cuvant => ReturneazaRadacinaCuvantului(cuvant)).ToList();

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
        {
            dictionar[cuvant]++;
        }
        else
        {
            dictionar.Add(cuvant, 1);
        }
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