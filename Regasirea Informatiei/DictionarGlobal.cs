namespace Regasirea_Informatiei;

public class DictionarGlobal
{
    public DictionarGlobal()
    {
        MarimeDictionar = 0;
        var fisierulExista = File.Exists(NumeFisier);
        if (fisierulExista)
            CitesteCuvinte();
        else
            File.Create(NumeFisier);
    }

    public string NumeFisier { get; } = "Dictionar.txt";

    public List<string> ListaCuvinte { get; } = new(30000);

    public int MarimeDictionar { get; private set; }

    private void CitesteCuvinte()
    {
        using (var cititorCuvinte = new StreamReader(NumeFisier))
        {
            var cuvinte = cititorCuvinte.ReadToEnd().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var cuvant in cuvinte) AdaugaCuvantInLista(cuvant);
        }

        ListaCuvinte.Sort();
    }

    public void ScrieCuvinteleInFisier()
    {
        using (var scriitorCuvinte = new StreamWriter(NumeFisier, false))
        {
            scriitorCuvinte.AutoFlush = true;
            foreach (var cuvant in ListaCuvinte) scriitorCuvinte.Write($"{cuvant} ");
        }
    }


    public void AdaugaCuvinteInLista(IEnumerable<string> cuvinte)
    {
        foreach (var cuvant in cuvinte) AdaugaCuvantInLista(cuvant);
        ListaCuvinte.Sort();
        ScrieCuvinteleInFisier();
    }

    private void AdaugaCuvantInLista(string cuvant)
    {
        if (!ListaCuvinte.Contains(cuvant))
        {
            ListaCuvinte.Add(cuvant);
            MarimeDictionar++;
        }
    }
}