using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Regasirea_Informatiei;

public class DictionarGlobal
{
    private readonly string _numeFisier = "Dictionar.txt";
    private List<string> _listaCuvinte = new List<string>(30000);
    private int _marimeDictionar;

    public string NumeFisier
    {
        get { return _numeFisier; }
    }
    public List<string> ListaCuvinte
    {
        get{return _listaCuvinte;}
    }

    public int MarimeDictionar
    {
        get { return _marimeDictionar; }
    }
    
    public DictionarGlobal()
    {
        _marimeDictionar = 0;
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

    private void CitesteCuvinte()
    {
        using (StreamReader cititorCuvinte = new StreamReader(_numeFisier))
        {
            string[] cuvinte = cititorCuvinte.ReadToEnd().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string cuvant in cuvinte)
            {
                AdaugaCuvantInLista(cuvant);
            }
        } 
        _listaCuvinte.Sort();
    }

    public void ScrieCuvinteleInFisier()
    {
        using (StreamWriter scriitorCuvinte = new StreamWriter(_numeFisier,false))
        {
            scriitorCuvinte.AutoFlush = true;
            foreach (var cuvant in _listaCuvinte)
            {
                scriitorCuvinte.Write($"{cuvant} ");
            }
        }
    }

    
    public void AdaugaCuvinteInLista(IEnumerable<string> cuvinte)
    {
        foreach(string cuvant in cuvinte)
        {
            AdaugaCuvantInLista(cuvant);
        }
        _listaCuvinte.Sort();
        ScrieCuvinteleInFisier();
    }

    private void AdaugaCuvantInLista(string cuvant)
    {
        if (!_listaCuvinte.Contains(cuvant))
        {
            _listaCuvinte.Add(cuvant);
            _marimeDictionar++;
        }
    }
}