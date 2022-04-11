namespace Regasirea_Informatiei;

public class DocumentGlobal
{
    private readonly string _numeFisier = "FisierGlobal.txt";
    private List<string> _documenteNormalizate = new List<string>();
    private SortedSet<string> _listaAtribute = new SortedSet<string>();
    
    public static string SimbolAtribut = "@";
    
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

                if (linie != null)
                {
                    if (linie.StartsWith(SimbolAtribut))
                    {
                        _listaAtribute.Add(linie);
                    }
                    else
                    {
                        _documenteNormalizate.Add(linie);
                    }
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
                scriitor.Flush();
            }

            foreach (string document in _documenteNormalizate)
            {
                scriitor.WriteLine(document);
                scriitor.Flush();
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

    public void AdaugaAtributInLista(string atribut)
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