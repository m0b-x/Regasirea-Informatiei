namespace Regasirea_Informatiei;

public class DictionarStopWords
{
    private static readonly string[] SeparatorCitire = {"\r\n", "\r", "\n"};
    private readonly string _numeFisier = "StopWords.txt";

    public DictionarStopWords()
    {
        Console.WriteLine("DA");
        if (File.Exists(_numeFisier))
            CitesteDate();
        else
            File.Create(_numeFisier);
    }

    public HashSet<string> ListaStopWords { get; } = new(450);

    private void CitesteDate()
    {
        using (var cititorCuvinte = new StreamReader(_numeFisier))
        {
            var cuvinte = cititorCuvinte.ReadToEnd().Split(SeparatorCitire, StringSplitOptions.RemoveEmptyEntries);
            foreach (var cuvant in cuvinte) ListaStopWords.Add(cuvant);
        }
    }
}