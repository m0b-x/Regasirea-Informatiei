using System.Text;
using System.Xml;
using Accord.MachineLearning.Text.Stemmers;

namespace Regasirea_Informatiei;

public class Document
{
    private sealed class PathFisierEqualityComparer : IEqualityComparer<Document>
    {
        public bool Equals(Document? x, Document? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.PathFisier == y.PathFisier;
        }

        public int GetHashCode(Document obj)
        {
            return obj.PathFisier.GetHashCode();
        }
    }

    public static IEqualityComparer<Document> PathFisierComparer { get; } = new PathFisierEqualityComparer();

    private static readonly DictionarGlobal DictionarGlobal = new();
    public static DocumentGlobal DocumentGlobal = new(DictionarGlobal);

    private static readonly EnglishStemmer StemmerCuvinte = new();
    public string Titlu { get; private set; }

    public List<string> Topicuri { get; } = new List<string>();

    public int FrecventaMaxima { get; private set; } = 1 ;

    public double EntropieTotala;
    public Dictionary<string, int> DictionarCuvinte { get; } = new(Constante.NumarCuvinteEstimatDocument);

    private readonly StringBuilder _documentNormalizat = new(Constante.NumarCuvinteEstimatDocument);
    
    public readonly string PathFisier;
    



    public Document(string pathFisier)
    {
        PathFisier = pathFisier;
        Titlu = string.Empty;
        if (DocumentGlobal.ListaDocumenteNormalizate.Contains(this))
        {
            DocumentGlobal.ListaDocumenteNormalizate.TryGetValue(this,out var documentGasit);
            if (documentGasit != null)
            {
                FrecventaMaxima = documentGasit.FrecventaMaxima;
                DictionarCuvinte = documentGasit.DictionarCuvinte;
                _documentNormalizat = documentGasit._documentNormalizat;
            }
        }
        else
        {
            CitesteDate();
        }
    }

    public Document(StringBuilder documentNormalizat)
    {
        Titlu =  string.Empty;
        _documentNormalizat = documentNormalizat;
        var dateDocument = documentNormalizat.ToString().Split(Constante.SimbolTitlu);

        PathFisier = dateDocument[0];
        
        var dateCaString = dateDocument[1].Split(Constante.DelimitatorIndexFrecventa, StringSplitOptions.RemoveEmptyEntries);
        List<int> dateCaNumere = new(Constante.NumarCuvinteEstimatDocument);

        for (int index=0;index<dateCaString.Length;index++)
        {
            dateCaNumere.Add(int.Parse(dateCaString[index]));
        }
        var topicuri = dateDocument[^1].Split(Constante.DelimitatorTopicuri);
        for (int index = 0; index < dateDocument.Length-1; index++)
        {
            Topicuri.Add(topicuri[index]);
        }
        
        for (var index = 0; index < dateCaNumere.Count - 1; index += 2)
        {
            AdaugaCuvantDistinctInDictionar(DictionarGlobal.ListaCuvinte[dateCaNumere[index]], dateCaNumere[index + 1],
                DictionarCuvinte);
        }
    }



    private void CitesteDate()
    {
        var continutFisier = new StringBuilder(Constante.NumarCuvinteEstimatDocument*Constante.LungimeMedieCuvant);
        using (var cititorXml = new XmlTextReader(PathFisier))
        {
            while (cititorXml.Read())
            {
                if (cititorXml.NodeType == XmlNodeType.Element)
                {
                    switch (cititorXml.Name)
                    {
                        case "p":
                            continutFisier.Append(cititorXml.ReadElementContentAsString());
                            break;
                        case "title":
                            Titlu = cititorXml.ReadElementContentAsString();
                            break;
                        case "metadata":
                            var metadata =cititorXml.ReadInnerXml().Split(Environment.NewLine);
                            foreach (var data in metadata)
                            {
                                if (data.Contains("bip:"))
                                {
                                    string clasa = data.Split(Constante.DelimitatorClaseCitire)[1];
                                    Topicuri.Add(clasa);
                                }
                            }
                            break;
                    }
                }
            }
        }

        continutFisier.Replace(Constante.inceputParagraf,Constante.inlocuitorParagrf);
        continutFisier.Replace(Constante.sfarsitParagraf,Constante.inlocuitorParagrf);
        RealizeazaNormalizareaDocumentului(continutFisier.ToString());
        RealizeazaFormaVectoriala();
    }

    private void AdaugaCuvantDistinctInDictionar(string cuvant, int frecventa, Dictionary<string, int> dictionar)
    {
        dictionar.Add(cuvant, frecventa);
    }
    private void RealizeazaFormaVectoriala()
    {
        _documentNormalizat.Append($"{PathFisier}{Constante.SimbolTitlu} ");

        
        foreach (var cuvant in DictionarCuvinte)
        {
            _documentNormalizat.Append($"{DictionarGlobal.ListaCuvinte.IndexOf(cuvant.Key)}:{cuvant.Value} ");
        }

        
        _documentNormalizat.Append("#_");
        foreach (var topic in Topicuri)
        {
            _documentNormalizat.Append($"{topic}_");
        }
        DocumentGlobal.AdaugaDocumentInLista(_documentNormalizat.ToString());
    }

    private void AdaugaCuvantInDictionar(string cuvant)
    { 
            DictionarGlobal.AdaugaCuvantInLista(cuvant);
            DocumentGlobal.AdaugaAtributInListaDinDocument(cuvant);
            AdaugaCuvantDistinctInDictionar(cuvant, DictionarCuvinte);
    }
    private void RealizeazaNormalizareaDocumentului(string continutFisier)
    {
        var cuvinte = continutFisier.InlocuiestePunctuatia().ToLowerInvariant()
            .Split(Constante.DelimitatorGeneral, StringSplitOptions.RemoveEmptyEntries).ToList();

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
                    AdaugaCuvantInDictionar(cuvinte[index]);
                }
            }
            else
            {
                cuvinte.RemoveAt(index);
                index--;
            }
        }
    }

    private string ReturneazaRadacinaCuvantului(string cuvant)
    {
        return  StemmerCuvinte.Stem(cuvant);
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