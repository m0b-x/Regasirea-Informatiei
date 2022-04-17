namespace Regasirea_Informatiei;

public class DictionarStopWords
{
    private static readonly string[] SeparatorCitire = new[] {"\r\n", "\r", "\n"};
    private readonly string _numeFisier = "StopWords.txt";
    private List<string> _listaStopWords = new List<string>(450);

    public List<string> ListaStopWords
    {
        get { return _listaStopWords; }
    }

    public DictionarStopWords()
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

    public void CitesteDate()
    {
        using (StreamReader cititorCuvinte = new StreamReader(_numeFisier))
        {
            string[] cuvinte = cititorCuvinte.ReadToEnd().Split(SeparatorCitire,StringSplitOptions.None);
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