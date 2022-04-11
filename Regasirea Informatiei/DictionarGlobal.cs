namespace Regasirea_Informatiei;

public class DictionarGlobal
{
    private readonly string _numeFisier = "Dictionar.txt";
    private List<string> _dictionarCuvinte = new List<string>();

    public List<string> DictionarCuvinte
    {
        get{return _dictionarCuvinte;}
    }
    public DictionarGlobal()
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

        Console.WriteLine("Dictionar initializat.");
    }

    private void CitesteCuvinte()
    {
        using (StreamReader cititorCuvinte = new StreamReader(_numeFisier))
        {
            string[] cuvinte = cititorCuvinte.ReadToEnd().Split(' ');
            foreach (string cuvant in cuvinte)
            {
                _dictionarCuvinte.Add(cuvant);
            }
        } 
        _dictionarCuvinte.Sort();
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

    public void AdaugaCuvinteInLista(IEnumerable<string> cuvinte)
    {
        foreach(string cuvant in cuvinte)
        {
            if (!_dictionarCuvinte.Contains(cuvant))
            {
                _dictionarCuvinte.Add(cuvant);
            }
        }
        _dictionarCuvinte.Sort();
        ScrieCuvinteleInFisier();
    }
}