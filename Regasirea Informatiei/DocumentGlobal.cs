using System.Text;

namespace Regasirea_Informatiei;

public class DocumentGlobal
{
    public static Dictionary<string,int> ToateTopicurile { get; set; } = new ();
    
    public bool EsteNevoieDeSuprascriere;
    private readonly string _numeFisier = "FisierGlobal.txt";
    private HashSet<Document> _listaDocumenteNormalizate = new HashSet<Document>(Constante.NumarDocumenteEstimat,Document.PathFisierComparer);

    
    public HashSet<Document> ListaDocumenteNormalizate
    {
        get => _listaDocumenteNormalizate;
        set => _listaDocumenteNormalizate = value;
    }

    public DictionarGlobal DictionarGlobal { get; set; }

    public DocumentGlobal(ref DictionarGlobal dictionarGlobal)
    {
        DictionarGlobal = dictionarGlobal;
        if (File.Exists(_numeFisier))
        {
            CitesteDate();
            EsteNevoieDeSuprascriere = false;
        }
        else
        {
            File.Create(_numeFisier);
            EsteNevoieDeSuprascriere = true;
        }


        Console.WriteLine("Fisier Global initializat.");
    }

    private void CitesteDate()
    {
        using var cititor = new StreamReader(_numeFisier);
        while (!cititor.EndOfStream)
        {
            var linie = cititor.ReadLine() ?? throw new InvalidOperationException();


            if (linie.StartsWith(Constante.SimbolAtribut))
            {
                DictionarGlobal.ListaCuvinte.Add(linie.Substring(1));
            }
            else if (!string.IsNullOrWhiteSpace(linie))
            {
                _listaDocumenteNormalizate.Add(new Document(new StringBuilder(linie)));
            }
        }
    }


    public void ScrieDate()
    {
        if (EsteNevoieDeSuprascriere)
        {
            using var scriitor = new StreamWriter(_numeFisier, false);
            scriitor.AutoFlush = true;
            SortedSet<string> listaAtributeSortate = new(DictionarGlobal.ListaCuvinte);
            foreach (var atribut in listaAtributeSortate) scriitor.WriteLine($"{Constante.SimbolAtribut}{atribut}");

            scriitor.WriteLine("@data");

            foreach (var document in _listaDocumenteNormalizate)
            {
                scriitor.WriteLine(document.DocumentNormalizat);
            }
        }
    }

    public void AdaugaDocumentInLista(string document)
    {
        _listaDocumenteNormalizate.Add(new Document(new StringBuilder(document)));
    }

}