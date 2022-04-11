namespace Regasirea_Informatiei;

public class DictionarGlobal
{
    private readonly string _numeFisier = "Dictionar.txt";
    private SortedSet<string?> _dictionarCuvinte = new SortedSet<string?>();

    public DictionarGlobal()
    {
        using (var scriitor = new StreamWriter(_numeFisier, true))
        {
            bool fisierulExista = File.Exists(_numeFisier);
            if (fisierulExista)
            {
                CitesteCuvinte();
            }
            else
            {
                File.Create(_numeFisier);
            }
        }

        Console.WriteLine("Dictionar initializat.");
    }

    private void CitesteCuvinte()
    {
        using (StreamReader cititorCuvinte = new StreamReader(_numeFisier))
        {
            string?[] cuvinte = cititorCuvinte.ReadToEnd().Split(' ');
            foreach (string? cuvant in cuvinte)
            {
                _dictionarCuvinte.Add(cuvant);
            }
        }
    }

    public void ScrieCuvinteleInFisier()
    {
        using (StreamWriter scriitorCuvinte = new StreamWriter(_numeFisier))
        {
            foreach (var cuvant in _dictionarCuvinte)
            {
                scriitorCuvinte.Write($"{cuvant} ");
                scriitorCuvinte.Flush();
            }
        }
    }

    public void AdaugaCuvantInLista(string? cuvant)
    {
        if (!_dictionarCuvinte.Contains(cuvant))
        {
            _dictionarCuvinte.Add(cuvant);
            ScrieCuvinteleInFisier();
        }
    }
}