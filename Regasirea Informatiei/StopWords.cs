namespace Regasirea_Informatiei;

public class StopWords
{ 
    private readonly string _numeFisier = "StopWords.txt";
    private List<string> _listaStopWords = new List<string>();

    public List<string> ListaStopWords
    {
        get { return _listaStopWords; }
    }

    public StopWords()
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

        Console.WriteLine("Fisier StopWords initializat.");
    }

    public void CitesteDate()
    {
        using (StreamReader cititorCuvinte = new StreamReader(_numeFisier))
        {
            string[] cuvinte = cititorCuvinte.ReadToEnd().Split(' ');
            foreach (string cuvant in cuvinte)
            {
                _listaStopWords.Add(cuvant);
            }
        }
    }

    public bool EsteStopWord(string cuvant)
    {
        if (_listaStopWords.Contains(cuvant))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}