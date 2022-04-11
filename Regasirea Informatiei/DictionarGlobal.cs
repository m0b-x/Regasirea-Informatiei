namespace Regasirea_Informatiei;

public class DictionarGlobal
{
    private readonly string numeFisier = "Dictionar.txt";
    private SortedSet<string> dictionarCuvinte = new SortedSet<string>();

    private DictionarGlobal()
    {
        using (var tw = new StreamWriter(numeFisier, true))
        {
            bool fisierulExista = File.Exists(numeFisier);
            if (fisierulExista)
            {
                CitesteCuvinte();
            }
            else
            {
                File.Create(numeFisier);
            }
        }

        Console.WriteLine("Dictionar initializat.");
    }

    private void CitesteCuvinte()
    {
        using (StreamReader cititorCuvinte = new StreamReader(numeFisier))
        {
            string[] cuvinte = cititorCuvinte.ReadToEnd().Split(' ');
            foreach (string cuvant in cuvinte)
            {
                dictionarCuvinte.Add(cuvant);
            }
        }
    }

    public void ScrieCuvinteleInFisier()
    {
        using (StreamWriter scriitorCuvinte = new StreamWriter(numeFisier))
        {
            foreach (var cuvant in dictionarCuvinte)
            {
                scriitorCuvinte.Write($"{cuvant} ");
            }
        }
    }

    public void AdaugaCuvantInLista(string cuvant)
    {
        if (!dictionarCuvinte.Contains(cuvant))
        {
            dictionarCuvinte.Add(cuvant);
        }
    }
}