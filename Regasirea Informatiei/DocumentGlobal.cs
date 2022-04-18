namespace Regasirea_Informatiei;

public class DocumentGlobal
{
    private static readonly char[] DelimitatoriAtribute = {' ', ':'};
    private static readonly char SimbolAtribut = '@';
    private static readonly char SimbolTitlu = '#';
    private readonly List<string> _documenteCaSiStringuri = new List<string>(8000);
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
        using (var cititor = new StreamReader(_numeFisier))
        {
            while (!cititor.EndOfStream)
            {
                var linie = cititor.ReadLine() ?? throw new InvalidOperationException();

                
                    if (linie.StartsWith(SimbolAtribut))
                    {
                        AdaugaAtributInListaLaCitire(linie.Substring(1));
                    }
                    else if (!string.IsNullOrWhiteSpace(linie))
                    {
                        AdaugaDocumentInDictionarLaCitire(linie);
                        AdaugaCuvantInDictionarLaCitire(_documenteCaDictionare, ConvertesteDocumentInDictionar(linie));
                    }
            }
        }
    }

    private void AdaugaDocumentInDictionarLaCitire(string document)
    {
        _documenteCaSiStringuri.Add(document);
    }

    private void AdaugaCuvantInDictionarLaCitire(Dictionary<string, Dictionary<int, int>> dictionar,
        KeyValuePair<string, Dictionary<int, int>> pereche)
    {
        dictionar.Add(pereche.Key, pereche.Value);
    }


    public void ScrieDate()
    {
        if (_esteNevoieDeSuprascriere)
        {
            Console.WriteLine("Da");
            using (var scriitor = new StreamWriter(_numeFisier, false))
            {
                scriitor.AutoFlush = true;
                foreach (var atribut in _listaAtribute) scriitor.WriteLine($"{SimbolAtribut}{atribut}");

                scriitor.WriteLine("@data");

                foreach (var document in _documenteCaSiStringuri) scriitor.WriteLine(document);
            }
        }
    }

    public void AdaugaDocumentInLista(string document)
    {
        string dateDocument = document.Split(SimbolTitlu)[1];
        foreach(var documentNormalizat in _documenteCaSiStringuri)
        {
            if (documentNormalizat.Contains(dateDocument))
            {
                
                return;
            }
            else
            {
                Console.WriteLine(dateDocument+"SEPARATOR\n"+documentNormalizat+"\n\n\n");
            }
        }

        if (_esteNevoieDeSuprascriere == false)
        {
            _esteNevoieDeSuprascriere = true;
        }
        _documenteCaSiStringuri.Add(document);
    }

    private KeyValuePair<string, Dictionary<int, int>> ConvertesteDocumentInDictionar(string document)
    {
        var dateDocumet = ReturneazaDateleDocumentului(document);

        var titluDocument = dateDocumet[0];
        var dateDocument = dateDocumet[1];
        var dateCaString = dateDocument.Split(DelimitatoriAtribute, StringSplitOptions.RemoveEmptyEntries);
        List<int> dateCaNumere = new(5000);
        foreach (var data in dateCaString)
                dateCaNumere.Add(int.Parse(data));

        Dictionary<int, int> dictionarAtribut = new(5000);

        for (var index = 0; index < dateCaNumere.Count - 1; index += 2)
            dictionarAtribut.Add(dateCaNumere[index], dateCaNumere[index + 1]);

        var dictionarCaSiPereche = new KeyValuePair<string, Dictionary<int, int>>(titluDocument, dictionarAtribut);
        return dictionarCaSiPereche;
    }

    private string[] ReturneazaDateleDocumentului(string document)
    {
        return document.Split(SimbolTitlu);
    }
    private void AdaugaAtributInListaLaCitire(string atribut)
    {
       _listaAtribute.Add(atribut);
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
            AdaugaAtributInListaDinArticol(atribut);
        }
    }
}