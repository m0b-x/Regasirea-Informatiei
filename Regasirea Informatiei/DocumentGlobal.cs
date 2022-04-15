namespace Regasirea_Informatiei;

public class DocumentGlobal
{
    
    private static readonly  char[] _delimitatoriAtribute = {' ', ':'};
    private static readonly string _simbolAtribut = "@";
    
    private readonly string _numeFisier = "FisierGlobal.txt";
    private List<string> _documenteCaSiStringuri = new List<string>(8000);
    private Dictionary<string, Dictionary<int, int>> _documenteCaSiDictionare = new(8000);
    private SortedSet<string> _listaAtribute = new SortedSet<string>();

    public List<string> DocumenteCaSiStringuri
    {
        get { return _documenteCaSiStringuri; }
    }

    public Dictionary<string, Dictionary<int, int>> DocumenteCaSiDictionare
    {
        get { return _documenteCaSiDictionare; }
    }
    public DocumentGlobal()
    {
        bool fisierulExista = File.Exists(_numeFisier);
        if (fisierulExista)
        {
            CitesteDate();
        }
        else
        {
            File.Create(_numeFisier);
        }


        Console.WriteLine("Fisier Global initializat.");
        
    }

    private void CitesteDate()
    {
        using (var cititor = new StreamReader(_numeFisier))
        {
            while (!cititor.EndOfStream)
            {
                string linie = cititor.ReadLine();

                if (!String.IsNullOrWhiteSpace(linie))
                {
                    if (linie.StartsWith(_simbolAtribut))
                    {
                        AdaugaAtributInLista(linie.Split()[0].Trim());
                    }
                    else
                    {
                        _documenteCaSiStringuri.Add(linie);
                        KeyValuePair<string, Dictionary<int, int>> pereche = ConvertesteDocumentInDictionar(linie);
                        _documenteCaSiDictionare.Add(pereche.Key,pereche.Value);
                        Console.Write(pereche.Value.ToString());
                    }
                }
            }
        }        
    }

    public void ScrieDate()
    {
        using (var scriitor = new StreamWriter(_numeFisier,false))
        {
            scriitor.AutoFlush = true;
            foreach (string atribut in _listaAtribute)
            {
                scriitor.WriteLine($"{atribut}");
                Console.WriteLine($"#{atribut}#");
            }
            
            scriitor.WriteLine($"\n@data\n");
            
            foreach (string document in _documenteCaSiStringuri)
            {
                scriitor.WriteLine(document);
            }
            
        }
    }

    public bool EsteDocumentInLista(string numeDocument)
    {
        string numeDocumentNormalizat = $"{numeDocument}#";
        foreach (var document in _documenteCaSiStringuri)
        {
            if (document.Contains(numeDocumentNormalizat))
            {
                return true;
            }
        }

        return false;
    }
    public void AdaugaDocumentInLista(string titlu, string document)
    {
        if (!EsteDocumentInLista(titlu))
        {
            _documenteCaSiStringuri.Add(document);
        }
    }

    public KeyValuePair<string, Dictionary<int, int>> ConvertesteDocumentInDictionar(string document)
    {
        
        KeyValuePair<string, Dictionary<int, int>> dictionarCaSiPereche = new();
        string titluDocument = ReturneazaTitlulDocumentului(document);
        
        string dateDocument = document.Split('#')[1];
        List<string> dateCaString = dateDocument.Split(_delimitatoriAtribute, StringSplitOptions.TrimEntries).ToList();
        List<int> dateCaNumere = new();
        foreach (var data in dateCaString)
        {
            if (!String.IsNullOrWhiteSpace(data))
            {
                dateCaNumere.Add(Int32.Parse(data));
            }
        }

        Dictionary<int, int> dictionarAtribut = new();
        
        for(int index = 0; index<dateCaNumere.Count-1;index+=2)
        {
            dictionarAtribut.Add(dateCaNumere[index],dateCaNumere[index+1]);
        }

        dictionarCaSiPereche = new KeyValuePair<string, Dictionary<int, int>>(titluDocument, dictionarAtribut);
        return dictionarCaSiPereche;
    }
    public static string StergeSpatiile(string cuvant)
    {
        return new string(cuvant.ToCharArray()
            .Where(c => !Char.IsWhiteSpace(c))
            .ToArray());
    }
    public string ReturneazaTitlulDocumentului(string document)
    {
        string titlu = document.Split('#')[0];

        return titlu;
    }

    public void AdaugaAtributInLista(string atribut)
    {
        if (!_listaAtribute.Contains(atribut))
        {
            _listaAtribute.Add(atribut);
        }
    }
    
    public void AdaugaAtributeInLista(IEnumerable<string> atribute)
    {
        foreach (var atribut in atribute)
        {
            AdaugaAtributInLista($"@{atribut}");
        }
    }
    
    public bool EsteAtributulInLista(string atribut)
    {
        if (_listaAtribute.Contains(atribut))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}