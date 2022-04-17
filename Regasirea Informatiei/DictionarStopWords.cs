namespace Regasirea_Informatiei;

public class DictionarStopWords
{
    private static readonly string[] SeparatorCitire = {"\r\n", "\r", "\n"};
    private readonly string _numeFisier = "StopWords.txt";

    public DictionarStopWords()
    {
        var fisierulExista = File.Exists(_numeFisier);
        if (fisierulExista)
            CitesteDate();
        else
            File.Create(_numeFisier);
    }

    public List<string> ListaStopWords { get; } = new(450);

    public void CitesteDate()
    {
        using (var cititorCuvinte = new StreamReader(_numeFisier))
        {
            var cuvinte = cititorCuvinte.ReadToEnd().Split(SeparatorCitire, StringSplitOptions.None);
            foreach (var cuvant in cuvinte) ListaStopWords.Add(cuvant);
        }
    }

    public bool EsteStopWord(string cuvant)
    {
        if (ListaStopWords.Contains(cuvant))
            return true;
        return false;
    }
}