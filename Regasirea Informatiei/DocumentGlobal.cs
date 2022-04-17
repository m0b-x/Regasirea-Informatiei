namespace Regasirea_Informatiei;

public class DocumentGlobal
{
    private static readonly char[] DelimitatoriAtribute = {' ', ':'};
    private static readonly string _simbolAtribut = "@";
    private static readonly List<string> _documenteCaSiStringuri = new(8000);
    private static readonly Dictionary<string, Dictionary<int, int>> _documenteCaSiDictionare = new(8000);

    private readonly string _numeFisier = "FisierGlobal.txt";
    private readonly SortedSet<string> _listaAtribute = new();

    public DocumentGlobal()
    {
        var fisierulExista = File.Exists(_numeFisier);
        if (fisierulExista)
            CitesteDate();
        else
            File.Create(_numeFisier);


        Console.WriteLine("Fisier Global initializat.");
    }

    public List<string> DocumenteCaSiStringuri => _documenteCaSiStringuri;

    public Dictionary<string, Dictionary<int, int>> DocumenteCaSiDictionare => _documenteCaSiDictionare;

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
                        AdaugaCuvantInDictionar(_documenteCaSiDictionare, pereche);
                    }
                }
            }
        }
    }

    public void AdaugaCuvantInDictionar(Dictionary<string, Dictionary<int, int>> dictionar,
        KeyValuePair<string, Dictionary<int, int>> pereche)
    {
        if (!dictionar.ContainsKey(pereche.Key.Split('#')[0])) dictionar.Add(pereche.Key, pereche.Value);
    }

    public void ScrieDate()
    {
        using (var scriitor = new StreamWriter(_numeFisier, false))
        {
            scriitor.AutoFlush = true;
            foreach (var atribut in _listaAtribute) scriitor.WriteLine($"{atribut}");

            scriitor.WriteLine("\n@data\n");

            foreach (var document in _documenteCaSiStringuri) scriitor.WriteLine(document);
        }
    }

    public bool EsteDocumentInLista(string numeDocument)
    {
        var numeDocumentNormalizat = $"{numeDocument}#";
        foreach (var document in _documenteCaSiStringuri)
            if (document.Contains(numeDocumentNormalizat))
                return true;

        return false;
    }

    public void AdaugaDocumentInLista(string titlu, string document)
    {
        if (!_documenteCaSiStringuri.Contains(titlu)) _documenteCaSiStringuri.Add(document);
    }

    public KeyValuePair<string, Dictionary<int, int>> ConvertesteDocumentInDictionar(string document)
    {
        KeyValuePair<string, Dictionary<int, int>> dictionarCaSiPereche;
        var titluDocument = ReturneazaTitlulDocumentului(document);

        var dateDocument = document.Split('#')[1];
        var dateCaString = dateDocument.Split(DelimitatoriAtribute, StringSplitOptions.TrimEntries).ToList();
        List<int> dateCaNumere = new();
        foreach (var data in dateCaString)
            if (!string.IsNullOrWhiteSpace(data))
                dateCaNumere.Add(int.Parse(data));

        Dictionary<int, int> dictionarAtribut = new();

        for (var index = 0; index < dateCaNumere.Count - 1; index += 2)
            dictionarAtribut.Add(dateCaNumere[index], dateCaNumere[index + 1]);

        dictionarCaSiPereche = new KeyValuePair<string, Dictionary<int, int>>(titluDocument, dictionarAtribut);
        return dictionarCaSiPereche;
    }

    public string ReturneazaTitlulDocumentului(string document)
    {
        var titlu = document.Split('#')[0];

        return titlu;
    }

    public void AdaugaAtributInLista(string atribut)
    {
        if (!_listaAtribute.Contains(atribut)) _listaAtribute.Add(atribut);
    }

    public static int ReturneazaNrDocumenteCuAtributul(string atribut)
    {
        var numaratorDocumente = 0;
        var indexCuvant = Articol.DictionarGlobal.ListaCuvinte.IndexOf(atribut);
        var atributNormalizat = $"{indexCuvant}:";
        foreach (var document in _documenteCaSiStringuri)
            if (document.Split('#')[1].Contains(atribut))
                numaratorDocumente++;
        return numaratorDocumente;
    }


    public void AdaugaAtributeInLista(IEnumerable<string> atribute)
    {
        foreach (var atribut in atribute) AdaugaAtributInLista($"@{atribut}");
    }

    public bool EsteAtributulInLista(string atribut)
    {
        if (_listaAtribute.Contains(atribut))
            return true;
        return false;
    }
}