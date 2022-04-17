namespace Regasirea_Informatiei;

public class Interogator
{
    private static DocumentGlobal _documentGlobal;
    private readonly Dictionary<string, Dictionary<int, double>> _dictionarDocumenteNormalizate = new();
    private Dictionary<int, double> _dictionarInterogareNormalizat = new();
    private Interogare _interogare;
    private readonly List<KeyValuePair<double, string>> similaritateArticole = new();

    public Interogator(ref DocumentGlobal documentGlobal)
    {
        _documentGlobal = documentGlobal;
    }

    public string StringInterogare { get; set; }

    public void InterogheazaArticole(List<Articol> articole)
    {
        Console.Write("Definiti o interogare: ");
        StringInterogare = Console.ReadLine() ?? throw new InvalidOperationException();
        if (string.IsNullOrEmpty(StringInterogare))
        {
            Console.WriteLine("Eroare:stringul este null!");
        }
        else
        {
            foreach (var articol in articole) RealizeazaVectorulNormalizatDeAtribute(articol);

            _interogare = new Interogare(StringInterogare, Articol.DictionarGlobal);
            RealizeazaVectorulNormalizatAlInterogarii(_interogare);

            foreach (var document in _dictionarDocumenteNormalizate)
            {
                var similaritate = CalculeazaSimilaritate(_dictionarInterogareNormalizat.Values.ToList(),
                    document.Value.Values.ToList());
                if (similaritate > 0)
                    similaritateArticole.Add(new KeyValuePair<double, string>(similaritate, document.Key));
            }
        }

        if (similaritateArticole.Count > 0)
        {
            similaritateArticole.Sort((x, y) => y.Key.CompareTo(x.Key));

            Console.WriteLine("Articole gasite:");

            foreach (var articoleSortate in similaritateArticole)
                Console.WriteLine($"{articoleSortate.Value} - {articoleSortate.Key}");
        }
        else
        {
            Console.WriteLine("Nu a fost gasit nimic,scuze.");
        }
    }


    public static double CalculeazaSimilaritate(List<double> vectorA, List<double> vectorB)
    {
        var produsElemente = 0.0;
        var putereVectorA = 0.0;
        var putereVectorB = 0.0;
        for (var index = 0; index < vectorA.Count; index++)
        {
            produsElemente += vectorA[index] * vectorB[index];
            putereVectorA += Math.Pow(vectorA[index], 2);
            putereVectorB += Math.Pow(vectorB[index], 2);
        }

        if (putereVectorA > 0 && putereVectorB > 0)
            return produsElemente / (Math.Sqrt(putereVectorA) * Math.Sqrt(putereVectorB));
        return 0;
    }

    public void RealizeazaVectorulNormalizatAlInterogarii(Interogare interogare)
    {
        Dictionary<int, double> frecventeNormalizate = new(500);
        foreach (var cuvant in Articol.DictionarGlobal.ListaCuvinte)
        {
            var indexCuvant = Articol.DictionarGlobal.ListaCuvinte.IndexOf(cuvant);
            if (interogare.DictionarCuvinte.ContainsKey(cuvant))
            {
                var frecventaCuvantuluiNormalizata =
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
            var indexCuvant = Articol.DictionarGlobal.ListaCuvinte.IndexOf(cuvant);
            if (articol.DictionarCuvinte.Keys.Contains(cuvant))
            {
                var frecventaCuvantuluiNormalizata =
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
        if (!dictionar.ContainsKey(pereche.Key)) dictionar.Add(pereche.Key, pereche.Value);
    }


    private double CalculeazaIdf(string atribut)
    {
        var nrTotalDocumente = Articol.DocumentScriereGlobal.DocumenteCaSiDictionare.Count;
        var nrDocumenteContinandAtributul = ReturneazaNrDocumenteCuAtributul(atribut);
        Console.WriteLine(Math.Log((double) nrTotalDocumente / nrDocumenteContinandAtributul));
        if (nrDocumenteContinandAtributul != 0)
            return Math.Log((double) nrTotalDocumente / nrDocumenteContinandAtributul);
        return 0;
    }

    private double CalculeazaNormalizareaNominala(Articol articol, string atribut)
    {
        var indexulAtributului = Articol.DictionarGlobal.ListaCuvinte.IndexOf(atribut);
        var aparitiiArticol = 0;
        if (indexulAtributului != -1)
        {
            if (articol.DictionarCuvinte.ContainsKey(atribut))
                aparitiiArticol = articol.DictionarCuvinte[atribut];
            else
                aparitiiArticol = 0;
        }

        //problema din cv motiv impartirea returneaza 0
        return (double) aparitiiArticol / articol.ExtrageFrecventaMaxima();
    }

    private double CalculeazaNormalizareaNominala(Interogare interogare, string atribut)
    {
        var indexulAtributului = Articol.DictionarGlobal.ListaCuvinte.IndexOf(atribut);
        var aparitiiArticol = 0;
        if (indexulAtributului != -1) aparitiiArticol = interogare.DictionarCuvinte[atribut];

        var valoareReturnata = (double) aparitiiArticol / interogare.ExtrageFrecventaMaxima();
        return valoareReturnata;
    }

    public double CalculeazaFrecventaCuvantuluiInArticol(string atribut, Articol articol)
    {
        var idf = CalculeazaIdf(atribut);
        var similaritateNominala = CalculeazaNormalizareaNominala(articol, atribut);

        return idf * similaritateNominala;
    }

    public double CalculeazaFrecventaCuvantuluiInInterogare(string atribut, Interogare interogare)
    {
        var idf = CalculeazaIdf(atribut);
        var similaritateNominala = CalculeazaNormalizareaNominala(interogare, atribut);

        return idf * similaritateNominala;
    }

    public int ReturneazaNrDocumenteCuAtributul(string atribut)
    {
        var indexCuvant = Articol.DictionarGlobal.ListaCuvinte.IndexOf(atribut);
        var numaratorDocumente = 0;
        foreach (var document in _documentGlobal.DocumenteCaSiDictionare.Values)
            if (document.ContainsKey(indexCuvant))
                numaratorDocumente++;

        return numaratorDocumente;
    }
}