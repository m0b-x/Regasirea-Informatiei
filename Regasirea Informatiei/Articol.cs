using System.Text;
using System.Xml;
using SF.Snowball.Ext;

namespace Regasirea_Informatiei;

public class Articol
{
    private static readonly DictionarGlobal DictionarGlobal = new();
    public static DocumentGlobal DocumentGlobal = new(DictionarGlobal);

    public string Titlu { get; private set; }

    private string NumeFisier { get; set; }

    public int FrecventaMaxima = 1;

    public double EntropieTotala;
    public Dictionary<string, int> DictionarCuvinte { get; } = new(5000);
    
    private readonly EnglishStemmer _stemmerCuvinte = new();

    private readonly string _pathFisier;

    private readonly StringBuilder _documentNormalizat = new(5000);

    public Articol(string pathFisier)
    {
        NumeFisier = pathFisier;
        Titlu = string.Empty;
        _pathFisier = pathFisier;
        CitesteDate();
    }

    public Articol(StringBuilder documentNormalizat)
    {
        _pathFisier =  "NONE";
        NumeFisier = "NONE";
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
            AdaugaCuvantDistinctInDictionar(DictionarGlobal.ListaCuvinte[dateCaNumere[index]], dateCaNumere[index + 1],
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

    private void AdaugaCuvantDistinctInDictionar(string cuvant, int frecventa, Dictionary<string, int> dictionar)
    {
        dictionar.Add(cuvant, frecventa);
    }

    private void RealizeazaFormaVectoriala()
    {
        //aici
        _documentNormalizat.Append($"{NumeFisier}# ");

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
        AdaugaCuvinteleDistincteInDictionar(listaCuvinte);
        DocumentGlobal.AdaugaAtributeDinArticol(listaCuvinte);
        DictionarGlobal.AdaugaCuvinteInLista(listaCuvinte);
    }

    private void AdaugaCuvinteleDistincteInDictionar(List<string> listaCuvinte)
    {
        foreach (var cuvant in listaCuvinte)
        {
            AdaugaCuvantDistinctInDictionar(cuvant, DictionarCuvinte);
        }
    }

    private List<string> ReturneazaCuvinteleNormalizate(string continutFisier)
    {
        var cuvinte = continutFisier.InlocuiestePunctuatia().ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

        for (int index = 0; index < cuvinte.Count; index++)
        {
            if (UtilitatiCuvinte.EsteCuvantValid(cuvinte[index]))
            {
                if (DictionarGlobal.DictionarStopWords.ListaStopWords.Contains(cuvinte[index]))
                {
                    cuvinte.RemoveAt(index);
                    index--;
                }
                else
                {
                    cuvinte[index] = ReturneazaRadacinaCuvantului(cuvinte[index]);
                }
            }
            else
            {
                cuvinte.RemoveAt(index);
                index--;
            }
        }

        return cuvinte;
    }

    private string ReturneazaRadacinaCuvantului(string cuvant)
    {
        _stemmerCuvinte.SetCurrent(cuvant);
        _stemmerCuvinte.Stem();
        return _stemmerCuvinte.GetCurrent();
    }

    private void AdaugaCuvantDistinctInDictionar(string cuvant, Dictionary<string, int> dictionar)
    {
        if (dictionar.ContainsKey(cuvant))
        {
            dictionar[cuvant]++;
            if (dictionar[cuvant] > FrecventaMaxima)
                FrecventaMaxima = dictionar[cuvant];
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
}