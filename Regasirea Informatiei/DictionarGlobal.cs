namespace Regasirea_Informatiei;

public class DictionarGlobal
{
    private bool _esteNevoieDeSupraScriere;
    
    public string NumeFisier { get; } = "Dictionar.txt";

    public List<string> ListaCuvinte { get; } = new(30000);

    public int MarimeDictionar => ListaCuvinte.Count;
    
    public DictionarGlobal()
    {
        if (File.Exists(NumeFisier))
        {
            CitesteCuvinte();
        }
        else
        {
            File.Create(NumeFisier);
            _esteNevoieDeSupraScriere = true;
        }
    }

    private void CitesteCuvinte()
    {
        using (var cititorCuvinte = new StreamReader(NumeFisier))
        {
            var cuvinte = cititorCuvinte.ReadToEnd().
                Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct().Except(ListaCuvinte);

            foreach (var cuvant in cuvinte) ListaCuvinte.Add(cuvant);
        }
    }

    public void ScrieCuvinteleInFisier()
    {
        if (_esteNevoieDeSupraScriere)
        {
            using (var scriitorCuvinte = new StreamWriter(NumeFisier, false))
            {
                scriitorCuvinte.AutoFlush = true;
                foreach (var cuvant in ListaCuvinte) scriitorCuvinte.Write($"{cuvant} ");
            }
        }
    }


    public void AdaugaCuvinteInLista(IEnumerable<string> cuvinte)
    {
        foreach (var cuvant in cuvinte) AdaugaCuvantInLista(cuvant);
        ListaCuvinte.Sort();
    }

    private void AdaugaCuvantInLista(string cuvant)
    {
        if (!ListaCuvinte.Contains(cuvant))
        {
            if (_esteNevoieDeSupraScriere == false)
            {
                _esteNevoieDeSupraScriere = true;
            }
            ListaCuvinte.Add(cuvant);
        }
    }
}