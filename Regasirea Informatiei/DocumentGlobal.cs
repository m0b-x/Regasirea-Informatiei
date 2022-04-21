using System.Text;

namespace Regasirea_Informatiei;

public class DocumentGlobal
{

    private readonly HashSet<string> _documenteCaSiStringuri = new HashSet<string>(Constante.NumarCuvinteEstimatDocument);
    private Dictionary<string, SortedDictionary<int, int>> _documenteCaDictionare = new(Constante.NumarCuvinteEstimatDocument);
    

    private bool _esteNevoieDeSuprascriere;
    private readonly string _numeFisier = "FisierGlobal.txt";
    private readonly HashSet<string> _listaAtribute = new();
    private HashSet<Document> _listaDocumenteNormalizate = new HashSet<Document>(Constante.NumarDocumenteEstimat,Document.PathFisierComparer);

    public Dictionary<string, SortedDictionary<int, int>> DocumenteCaSiDictionare
    {
        get => _documenteCaDictionare;
        set => _documenteCaDictionare = value;
    }

    
    public HashSet<Document> ListaDocumenteNormalizate
    {
        get => _listaDocumenteNormalizate;
        set => _listaDocumenteNormalizate = value;
    }

    public DictionarGlobal DictionarGlobal { get; set; }

    public DocumentGlobal(DictionarGlobal dictionarGlobal)
    {
        DictionarGlobal = dictionarGlobal;
        if (File.Exists(_numeFisier))
        {
            CitesteDate();
            _esteNevoieDeSuprascriere = false;
        }
        else
        {
            File.Create(_numeFisier);
            _esteNevoieDeSuprascriere = true;
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
                AdaugaAtributInListaLaCitire(linie.Substring(1));
            }
            else if (!string.IsNullOrWhiteSpace(linie))
            {
                _listaDocumenteNormalizate.Add(new Document(new StringBuilder(linie)));
                AdaugaCuvantInDictionarLaCitire(_documenteCaDictionare, linie);
            }
        }
    }

    private void AdaugaCuvantInDictionarLaCitire(Dictionary<string, SortedDictionary<int, int>> dictionar, string linie)
    {
        var pereche =
            ConvertesteDocumentInDictionar(linie);

            _documenteCaSiStringuri.Add(linie);
            dictionar.Add(pereche.Key, pereche.Value);
    }

    private void AdaugaCuvantDistinctInDictionar(Dictionary<string, SortedDictionary<int, int>> dictionar, string linie)
    {
        var pereche =
            ConvertesteDocumentInDictionar(linie);
        if (!dictionar.ContainsKey(pereche.Key))
        {
            _documenteCaSiStringuri.Add(linie);
            dictionar.Add(pereche.Key, pereche.Value);
            if (_esteNevoieDeSuprascriere == false)
            {
                _esteNevoieDeSuprascriere = true;
            }
        }
    }


    public void ScrieDate()
    {
        if (_esteNevoieDeSuprascriere)
        {
            using var scriitor = new StreamWriter(_numeFisier, false);
            scriitor.AutoFlush = true;
            SortedSet<string> listaAtributeSortate = new(_listaAtribute);
            foreach (var atribut in listaAtributeSortate) scriitor.WriteLine($"{Constante.SimbolAtribut}{atribut}");

            scriitor.WriteLine("@data");

            foreach (var document in _documenteCaSiStringuri) scriitor.WriteLine(document);
        }
    }

    public void AdaugaDocumentInLista(string document)
    {
        if (!_documenteCaSiStringuri.Contains(document)) 
        {
            _listaDocumenteNormalizate.Add(new Document(new StringBuilder(document)));
            AdaugaCuvantDistinctInDictionar(_documenteCaDictionare, document);
        }
    }

    private KeyValuePair<string, SortedDictionary<int, int>> ConvertesteDocumentInDictionar(string document)
    {
        var dateDocumet = ReturneazaDateleDocumentului(document);

        var titluDocument = dateDocumet[0];
        var dateDocument = dateDocumet[1];
        var dateCaString = dateDocument.Split(Constante.DelimitatorIndexFrecventa, StringSplitOptions.RemoveEmptyEntries);
        List<int> dateCaNumere = new(Constante.NumarCuvinteEstimatDocument);
        foreach (var data in dateCaString)
            dateCaNumere.Add(int.Parse(data));

        SortedDictionary<int, int> dictionarAtribut = new();

        for (var index = 0; index < dateCaNumere.Count - 1; index += 2)
            dictionarAtribut.Add(dateCaNumere[index], dateCaNumere[index + 1]);

        var dictionarCaSiPereche = new KeyValuePair<string, SortedDictionary<int, int>>(titluDocument, dictionarAtribut);
        return dictionarCaSiPereche;
    }

    private string[] ReturneazaDateleDocumentului(string document)
    {
        return document.Split(Constante.SimbolTitlu);
    }

    private void AdaugaAtributInListaLaCitire(string atribut)
    {
        _listaAtribute.Add(atribut);
    }

    public void AdaugaAtributInListaDinDocument(string atribut)
    {
        if (!_listaAtribute.Contains(atribut))
        {
            _listaAtribute.Add(atribut);
            if (_esteNevoieDeSuprascriere == false)
            {
                
                _esteNevoieDeSuprascriere = true;
            }
        }
    }

    public void AdaugaAtributeDinDocument(IEnumerable<string> atribute)
    {
        foreach (var atribut in atribute)
        {
            AdaugaAtributInListaDinDocument(atribut);
        }
    }
}