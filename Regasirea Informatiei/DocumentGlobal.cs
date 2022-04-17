namespace Regasirea_Informatiei;

public class DocumentGlobal
{
    private static readonly char[] DelimitatoriAtribute = {' ', ':'};
    private static readonly string _simbolAtribut = "@";
    private readonly List<string> _documenteCaSiStringuri = new(8000);
    private Dictionary<string, Dictionary<int, int>> _documenteCaDictionare = new(8000);

    private bool _esteNevoieDeSuprascriere;
    private readonly string _numeFisier = "FisierGlobal.txt";
    private readonly SortedSet<string> _listaAtribute = new();

    public Dictionary<string, Dictionary<int, int>> DocumenteCaSiDictionare
    {
        get { return _documenteCaDictionare; }
        set => _documenteCaDictionare = value;
    }

    public DocumentGlobal()
    {
        var fisierulExista = File.Exists(_numeFisier);
        if (fisierulExista)
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
        using (var cititor = new StreamReader(_numeFisier))
        {
            while (!cititor.EndOfStream)
            {
                var linie = cititor.ReadLine() ?? throw new InvalidOperationException();

                if (!string.IsNullOrWhiteSpace(linie))
                {
                    if (linie.StartsWith(_simbolAtribut))
                    {
                        AdaugaAtributInLista(linie.Split()[0].Trim());
                    }
                    else
                    {
                        _documenteCaSiStringuri.Add(linie);

                        var pereche = ConvertesteDocumentInDictionar(linie);
                        AdaugaCuvantInDictionar(_documenteCaDictionare, pereche);
                    }
                }
            }
        }
    }

    private void AdaugaCuvantInDictionar(Dictionary<string, Dictionary<int, int>> dictionar,
        KeyValuePair<string, Dictionary<int, int>> pereche)
    {
        if (!dictionar.ContainsKey(pereche.Key.Split('#')[0])) dictionar.Add(pereche.Key, pereche.Value);
    }

    public void ScrieDate()
    {
        if (_esteNevoieDeSuprascriere)
        {
            using (var scriitor = new StreamWriter(_numeFisier, false))
            {
                scriitor.AutoFlush = true;
                foreach (var atribut in _listaAtribute) scriitor.WriteLine($"{atribut}");

                scriitor.WriteLine("\n@data\n");

                foreach (var document in _documenteCaSiStringuri) scriitor.WriteLine(document);
            }
        }
    }
    public void AdaugaDocumentInLista(string titlu, string document)
    {
        if (!_documenteCaSiStringuri.Contains(titlu)) _documenteCaSiStringuri.Add(document);
    }

    private KeyValuePair<string, Dictionary<int, int>> ConvertesteDocumentInDictionar(string document)
    {
        var titluDocument = ReturneazaTitlulDocumentului(document);

        var dateDocument = document.Split('#')[1];
        var dateCaString = dateDocument.Split(DelimitatoriAtribute, StringSplitOptions.TrimEntries).ToList();
        List<int> dateCaNumere = new(30000);
        foreach (var data in dateCaString)
            if (!string.IsNullOrWhiteSpace(data))
                dateCaNumere.Add(int.Parse(data));

        Dictionary<int, int> dictionarAtribut = new(30000);

        for (var index = 0; index < dateCaNumere.Count - 1; index += 2)
            dictionarAtribut.Add(dateCaNumere[index], dateCaNumere[index + 1]);

        var dictionarCaSiPereche = new KeyValuePair<string, Dictionary<int, int>>(titluDocument, dictionarAtribut);
        return dictionarCaSiPereche;
    }

    private string ReturneazaTitlulDocumentului(string document)
    {
        var titlu = document.Split('#')[0];

        return titlu;
    }

    private void AdaugaAtributInLista(string atribut)
    {
        if (!_listaAtribute.Contains(atribut)) _listaAtribute.Add(atribut);
    }
    private void AdaugaAtributInListaDinArticol(string atribut)
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

    public void AdaugaAtributeDinArticol(IEnumerable<string> atribute)
    {
        foreach (var atribut in atribute)
        {
            AdaugaAtributInListaDinArticol($"@{atribut}");
        }
        
    }
}