namespace Regasirea_Informatiei;

public class Interogator
{
    private string _stringInterogare;
    private Interogare _interogare;
    private static DocumentGlobal _documentGlobal;
    private Dictionary<string, Dictionary<int, double>> _dictionarDocumenteNormalizate = new();
    private Dictionary<int, double> _dictionarInterogareNormalizat = new();
    private List<KeyValuePair<double, string>> similaritateArticole = new();

    public string StringInterogare
    {
        get { return _stringInterogare; }
        set { _stringInterogare = value; }
    }

    public Interogator(ref DocumentGlobal documentGlobal)
    {
        _documentGlobal = documentGlobal;
    }

    public void InterogheazaArticole(List<Articol> articole)
    {
        Console.Write("Definiti o interogare: ");
        _stringInterogare = Console.ReadLine() ?? throw new InvalidOperationException();
        if (String.IsNullOrEmpty(_stringInterogare))
        {
            Console.WriteLine("Eroare:stringul este null!");
        }
        else
        {
            foreach (var articol in articole)
            {
                RealizeazaVectorulNormalizatDeAtribute(articol);
            }

            _interogare = new Interogare(_stringInterogare,Articol.DictionarGlobal);
            RealizeazaVectorulNormalizatAlInterogarii(_interogare);

            foreach (var document in _dictionarDocumenteNormalizate)
            {
                double similaritate = CalculeazaSimilaritate(_dictionarInterogareNormalizat.Values.ToList(),
                    document.Value.Values.ToList());
                if (similaritate > 0)
                {
                    similaritateArticole.Add(new KeyValuePair<double, string>(similaritate, document.Key));
                }
            }
        }

        if (similaritateArticole.Count > 0)
        {
            similaritateArticole.Sort((x, y) => y.Key.CompareTo(x.Key));

            Console.WriteLine("Articole gasite:");

            foreach (var articoleSortate in similaritateArticole)
            {
                Console.WriteLine($"{articoleSortate.Value} - {articoleSortate.Key}");
            }
        }
        else
        {
            Console.WriteLine("Nu a fost gasit nimic,scuze.");
        }
    }


    public static double CalculeazaSimilaritate(List<double> vectorA, List<double> vectorB)
    {
        double produsElemente = 0.0;
        double putereVectorA = 0.0;
        double putereVectorB = 0.0;
        for (int index = 0; index < vectorA.Count; index++)
        {
            produsElemente += vectorA[index] * vectorB[index];
            putereVectorA += Math.Pow(vectorA[index], 2);
            putereVectorB += Math.Pow(vectorB[index], 2);
        }

        if (putereVectorA > 0 && putereVectorB > 0)
        {
            return produsElemente / (Math.Sqrt(putereVectorA) * Math.Sqrt(putereVectorB));
        }
        else
        {
            return 0;
        }
    }

    public void RealizeazaVectorulNormalizatAlInterogarii(Interogare interogare)
    {
        Dictionary<int, double> frecventeNormalizate = new(500);
        foreach (var cuvant in Articol.DictionarGlobal.ListaCuvinte)
        {
            int indexCuvant = Articol.DictionarGlobal.ListaCuvinte.IndexOf(cuvant);
            if (interogare.DictionarCuvinte.ContainsKey(cuvant))
            {
                double frecventaCuvantuluiNormalizata =
                    CalculeazaFrecventaCuvantuluiInInterogare(Articol.DictionarGlobal.ListaCuvinte[indexCuvant],
                        interogare);
                frecventeNormalizate.Add(indexCuvant, frecventaCuvantuluiNormalizata);
            }
            else
            {
                frecventeNormalizate.Add(indexCuvant, 0);
            }
        }

        _dictionarInterogareNormalizat = frecventeNormalizate;
    }

    public void RealizeazaVectorulNormalizatDeAtribute(Articol articol)
    {
        Dictionary<int, double> frecventeNormalizate = new(30000);
        foreach (var cuvant in Articol.DictionarGlobal.ListaCuvinte)
        {
            int indexCuvant = Articol.DictionarGlobal.ListaCuvinte.IndexOf(cuvant);
            if (articol.DictionarCuvinte.Keys.Contains(cuvant))
            {
                double frecventaCuvantuluiNormalizata =
                    CalculeazaFrecventaCuvantuluiInArticol(Articol.DictionarGlobal.ListaCuvinte[indexCuvant],
                        articol);
                frecventeNormalizate.Add(indexCuvant, frecventaCuvantuluiNormalizata);
            }
            else
            {
                frecventeNormalizate.Add(indexCuvant, 0);
            }
        }

        KeyValuePair<string, Dictionary<int, double>> pereche = new(articol.Titlu, frecventeNormalizate);
        AdaugaCuvantInDictionar(_dictionarDocumenteNormalizate, pereche);
    }

    public void AdaugaCuvantInDictionar(Dictionary<string, Dictionary<int, double>> dictionar,
        KeyValuePair<string, Dictionary<int, double>> pereche)
    {
        if (!dictionar.ContainsKey(pereche.Key))
        {
            dictionar.Add(pereche.Key, pereche.Value);
        }
    }


    private double CalculeazaIdf(string atribut)
    {
        int nrTotalDocumente = Articol.DocumentScriereGlobal.DocumenteCaSiDictionare.Count;
        int nrDocumenteContinandAtributul = ReturneazaNrDocumenteCuAtributul(atribut);
        if (nrDocumenteContinandAtributul != 0)
        {
            return Math.Log((double) nrTotalDocumente / nrDocumenteContinandAtributul);
        }
        else
        {
            return 0;
        }
    }

    private double CalculeazaNormalizareaNominala(Articol articol, string atribut)
    {
        int indexulAtributului = Articol.DictionarGlobal.ListaCuvinte.IndexOf(atribut);
        int aparitiiArticol = 0;
        if (indexulAtributului != -1)
        {
            if (articol.DictionarCuvinte.ContainsKey(atribut))
            {
                aparitiiArticol = articol.DictionarCuvinte[atribut];
            }
            else
            {
                aparitiiArticol = 0;
            }
        }

        //problema din cv motiv impartirea returneaza 0
        return (double) aparitiiArticol / articol.ExtrageFrecventaMaxima();
    }

    private double CalculeazaNormalizareaNominala(Interogare interogare, string atribut)
    {
        int indexulAtributului = Articol.DictionarGlobal.ListaCuvinte.IndexOf(atribut);
        int aparitiiArticol = 0;
        if (indexulAtributului != -1)
        {
            aparitiiArticol = interogare.DictionarCuvinte[atribut];
        }

        double valoareReturnata = (double) aparitiiArticol / interogare.ExtrageFrecventaMaxima();
        return valoareReturnata;
    }

    public double CalculeazaFrecventaCuvantuluiInArticol(string atribut, Articol articol)
    {
        double idf = CalculeazaIdf(atribut);
        double similaritateNominala = CalculeazaNormalizareaNominala(articol, atribut);

        return idf * similaritateNominala;
    }

    public double CalculeazaFrecventaCuvantuluiInInterogare(string atribut, Interogare interogare)
    {
        double idf = CalculeazaIdf(atribut);
        double similaritateNominala = CalculeazaNormalizareaNominala(interogare, atribut);

        return idf * similaritateNominala;
    }

    public int ReturneazaNrDocumenteCuAtributul(string atribut)
    {
        int indexCuvant = Articol.DictionarGlobal.ListaCuvinte.IndexOf(atribut);
        int numaratorDocumente = 0;
        foreach (var document in _documentGlobal.DocumenteCaSiDictionare.Values)
        {
            if (document.ContainsKey(indexCuvant))
            {
                numaratorDocumente++;
            }
        }

        return numaratorDocumente;
    }
}