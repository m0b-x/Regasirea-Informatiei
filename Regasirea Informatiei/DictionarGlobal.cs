namespace Regasirea_Informatiei;

public class DictionarGlobal
{
    public readonly DictionarStopWords DictionarStopWords = new ();
    private string NumeFisier { get; } = "Dictionar.txt";

    public List<string> ListaCuvinte { get; set; } = new(Constante.NumarCuvinteEstimatDictionar);

    public int MarimeDictionar => ListaCuvinte.Count;
    
    public bool EsteNevoieDeSupraScriere { get; set; }
    
    public DictionarGlobal()
    {
        if (File.Exists(NumeFisier))
        {
            CitesteCuvinte();
        }
        else
        {
            File.Create(NumeFisier);
            EsteNevoieDeSupraScriere = true;
        }
    }

    private void CitesteCuvinte()
    {
        using var cititorCuvinte = new StreamReader(NumeFisier);
        var cuvinte = cititorCuvinte.ReadToEnd().
            Split(Constante.DelimitatorGeneral, StringSplitOptions.RemoveEmptyEntries);

        foreach (var cuvant in cuvinte)
        {
            if(!ListaCuvinte.Contains(cuvant))
                ListaCuvinte.Add(cuvant);
        }
    }

    public void ScrieCuvinteleInFisier()
    {
        if (EsteNevoieDeSupraScriere)
        {
            using var scriitorCuvinte = new StreamWriter(NumeFisier, false);
            scriitorCuvinte.AutoFlush = true;
            foreach (var cuvant in ListaCuvinte) scriitorCuvinte.Write($"{cuvant} ");
        }
    }
    
    public void AdaugaCuvantInLista(string cuvant)
    {
        if (!ListaCuvinte.Contains(cuvant))
        {
            if (EsteNevoieDeSupraScriere == false)
            {
                EsteNevoieDeSupraScriere = true;
            }
            ListaCuvinte.Add(cuvant);
        }
    }
}