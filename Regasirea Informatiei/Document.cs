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
    public static DocumentGlobal DocumentGlobal = new(ref DictionarGlobal);

    private static readonly EnglishStemmer StemmerCuvinte = new();
    public List<string> Topicuri { get; set; } = new List<string>();

    public int FrecventaMaxima { get; private set; } = 1 ;
    
    public Dictionary<string, int> DictionarCuvinte { get; } = new(Constante.NumarCuvinteEstimatDocument);

    public StringBuilder DocumentNormalizat =
        new(Constante.NumarCuvinteEstimatDocument * Constante.LungimeMedieCuvant);
    
    public readonly string PathFisier;
    



    public Document(string pathFisier)
    {
        PathFisier = pathFisier;
        if (DocumentGlobal.ListaDocumenteNormalizate.TryGetValue(this,out var documentGasit))
        {
            FrecventaMaxima = documentGasit.FrecventaMaxima;
            DictionarCuvinte = documentGasit.DictionarCuvinte;
            DocumentNormalizat = documentGasit.DocumentNormalizat;
        }
        else
        {
            if(DocumentGlobal.EsteNevoieDeSuprascriere == false)
                DocumentGlobal.EsteNevoieDeSuprascriere = true;
            CitesteDateDinFisier();
        }
    }

    public Document(StringBuilder documentNormalizat)
    {
        DocumentNormalizat = documentNormalizat;
        var dateDocument = documentNormalizat.ToString().Split(Constante.SimbolTitlu);

        PathFisier = dateDocument[0];
        
        var dateCaString = dateDocument[1].Split(Constante.DelimitatorIndexFrecventa, StringSplitOptions.RemoveEmptyEntries);

        for (int index=0;index<dateCaString.Length-1;index+=2)
        {
            DictionarCuvinte.Add(
                DictionarGlobal.ListaCuvinte[UInt16.Parse(dateCaString[index])],
                UInt16.Parse(dateCaString[index + 1]) );
        }

        var topicuri = dateDocument[^1].Split(Constante.DelimitatorTopicuri,StringSplitOptions.RemoveEmptyEntries);
        foreach (var topic in topicuri)
        {
            AdaugaTopic(topic);
        }
    }



    private void CitesteDateDinFisier()
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
                            continutFisier.Append($"{cititorXml.ReadElementContentAsString()} ");
                            break;
                        case "metadata":
                            
                            var metadata =cititorXml.ReadInnerXml().
                                                        Split(Constante.DelimitatorInceputTopicuri)[1].
                                                        Split(Environment.NewLine);
                            foreach (var data in metadata)
                            {
                                if (data.Contains("code code"))
                                {
                                    string topic = data.Split(Constante.DelimitatorClaseCitire)[1];
                                    AdaugaTopic(topic);
                                }
                            }
                            break;
                    }
                }
            }
        }

        continutFisier.Replace(Constante.InceputParagraf,Constante.InlocuitorParagrf);
        continutFisier.Replace(Constante.SfarsitParagraf,Constante.InlocuitorParagrf);
        RealizeazaNormalizareaDocumentului(continutFisier.ToString());
        RealizeazaFormaVectoriala();
    }

    private void AdaugaTopic(string topic)
    {
        Topicuri.Add(topic);
        if (!DocumentGlobal.ToateTopicurile.ContainsKey(topic))
        {
            DocumentGlobal.ToateTopicurile.Add(topic, 1);
        }
        else
        {
            DocumentGlobal.ToateTopicurile[topic]++;
        }
    }
    private void RealizeazaFormaVectoriala()
    {
        DocumentNormalizat.Append($"{PathFisier}{Constante.SimbolTitlu} ");

        
        foreach (var cuvant in DictionarCuvinte)
        {
            DocumentNormalizat.Append($"{DictionarGlobal.ListaCuvinte.IndexOf(cuvant.Key)}:{cuvant.Value} ");
        }

        
        DocumentNormalizat.Append(Constante.DelimitatorClase);
        foreach (var topic in Topicuri)
        {
            DocumentNormalizat.Append($"{topic}_");
        }
        DocumentGlobal.AdaugaDocumentInLista(DocumentNormalizat.ToString());
    }    
    public void RefaFormaVectoriala()
    {
        DocumentNormalizat = new StringBuilder(Constante.NumarCuvinteEstimatDocument * Constante.LungimeMedieCuvant);
        DocumentNormalizat.Append($"{PathFisier}{Constante.SimbolTitlu} ");

        
        foreach (var cuvant in DictionarCuvinte)
        {
            if(DictionarGlobal.ListaCuvinte.Contains(cuvant.Key))
                DocumentNormalizat.Append($"{DictionarGlobal.ListaCuvinte.IndexOf(cuvant.Key)}:{cuvant.Value} ");
        }

        
        DocumentNormalizat.Append(Constante.DelimitatorClase);
        foreach (var topic in Topicuri)
        {
            DocumentNormalizat.Append($"{topic}_");
        }
        DocumentGlobal.AdaugaDocumentInLista(DocumentNormalizat.ToString());
    }

    public void RefaDictionarulDeCuvinte()
    {
        foreach (var cuvant in DictionarCuvinte)
        {
            if (!DictionarGlobal.ListaCuvinte.Contains(cuvant.Key))
            {
                DictionarCuvinte.Remove(cuvant.Key);
            }
        }
    }
    private void AdaugaCuvantInDictionar(string cuvant)
    { 
            DictionarGlobal.AdaugaCuvantInLista(cuvant);
            
            AdaugaCuvantDistinctInDictionar(cuvant, DictionarCuvinte);
    }
    
    private void RealizeazaNormalizareaDocumentului(string continutFisier)
    {
        var cuvinte = continutFisier.InlocuiestePunctuatia().ToLowerInvariant()
            .Split(Constante.DelimitatorGeneral, StringSplitOptions.RemoveEmptyEntries).ToList();
        foreach (var cuvantNeschimbat in cuvinte)
        {
            var cuvant = cuvantNeschimbat.Trim().InlocuiesteCifrele();
            if (UtilitatiCuvinte.EsteCuvantValid(cuvant))
            {
                if (!DictionarGlobal.DictionarStopWords.ListaStopWords.Contains(cuvant))
                {
                    AdaugaCuvantInDictionar(ReturneazaRadacinaCuvantului(cuvant));
                }
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

    public static void ScrieDateleInFisiere()
    {
        DictionarGlobal.ScrieCuvinteleInFisier();
        DocumentGlobal.ScrieDocumenteleInFisier();
    }
}