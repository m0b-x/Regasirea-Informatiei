namespace Regasirea_Informatiei;

public class FisierGlobal
{
    private readonly string _numeFisier = "FisierGlobal.txt";
    private List<string> _documenteNormalizate = new List<string>();
    private SortedSet<string> _listaAtribute = new SortedSet<string>();
    
    private static string _simbolAtribut = "@";
    
    public FisierGlobal()
    {
        using (var scriitor = new StreamWriter(_numeFisier, true))
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
        }

        Console.WriteLine("Fisier Global initializat.");
        
    }

    private void CitesteDate()
    {
        using (var cititor = new System.IO.StreamReader(@"C:\file.txt"))
        {
            while (!cititor.EndOfStream)
            {
                string linie = cititor.ReadLine();

                if (linie.StartsWith(_simbolAtribut))
                {
                    _listaAtribute.Add(linie);
                }
                else
                {
                    _documenteNormalizate.Add(linie);
                }
            }

            cititor.Close();
        }        
    }

    public void ScrieDate()
    {
        using (var scriitor = new StreamWriter(_numeFisier, true))
        {
            foreach (string atribut in _listaAtribute)
            {
                scriitor.WriteLine(atribut);
            }

            foreach (string document in _documenteNormalizate)
            {
                scriitor.WriteLine(document);
            }
        }
    }

    public bool EsteDocumentInLista(string numeDocument)
    {
        string numeDocumentNormalizat = $"{numeDocument}#";
        foreach (var document in _documenteNormalizate)
        {
            if (document.Contains(numeDocumentNormalizat))
            {
                return true;
            }
        }

        return false;
    }
    private void AdaugaDocumentInLista(string document)
    {
        if (!_documenteNormalizate.Contains(document))
        {
            _documenteNormalizate.Add(document);
        }
    }   

    private void AdaugaAtributInLista(string atribut)
    {
        if (!_listaAtribute.Contains(atribut))
        {
            _listaAtribute.Add(atribut);
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