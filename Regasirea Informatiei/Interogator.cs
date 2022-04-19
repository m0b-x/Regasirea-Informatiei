using System.Diagnostics;

namespace Regasirea_Informatiei;

public class Interogator
{
    private static DocumentGlobal _documentGlobal = null!;
    private readonly Dictionary<string, Dictionary<int, double>> _dictionarDocumenteNormalizate = new(7000);
    private Dictionary<int, double> _dictionarInterogareNormalizat = new(150);
    private Interogare? _interogare;
    private readonly List<KeyValuePair<double, string>> _similaritateArticole = new(7000);
    private bool _esteRelevantaInterogarea;

    public Interogator(ref DocumentGlobal documentGlobal)
    {
        _documentGlobal = documentGlobal;
    }

    private string? PropozitieInterogare { get; set; }

    public void InterogheazaArticole()
    {
        Console.Write("Definiti o interogare: ");
        PropozitieInterogare = Console.ReadLine() ?? throw new InvalidOperationException();

        Stopwatch cronometru = new Stopwatch();
        cronometru.Start();

        if (string.IsNullOrEmpty(PropozitieInterogare))
        {
            Console.WriteLine("Eroare:stringul este null!");
        }
        else
        {
            _interogare = new Interogare(PropozitieInterogare, _documentGlobal.DictionarGlobal);
            CalculeazaRelevantaInterogarii();
            
            if (_esteRelevantaInterogarea)
            {
                 RealizeazaVectorulNormalizatAlInterogarii(_interogare);
                 RealizeazaVectoreleNormalizateDeAtribute();
                 
                foreach (var document in _dictionarDocumenteNormalizate)
                {
                    var similaritate = CalculeazaSimilaritate(_dictionarInterogareNormalizat.Values.ToList(),
                        document.Value.Values.ToList());
                    if (similaritate > 0)
                        _similaritateArticole.Add(new KeyValuePair<double, string>(similaritate, document.Key));
                }

                if (_similaritateArticole.Count > 0)
                {
                    _similaritateArticole.Sort((x, y) => y.Key.CompareTo(x.Key));

                    Console.WriteLine("Articole gasite:");

                    foreach (var articoleSortate in _similaritateArticole)
                        Console.WriteLine($"{articoleSortate.Value} - {articoleSortate.Key}");
                }
                else
                {
                    Console.WriteLine("Nu a fost gasit nimic,scuze.");
                }

                cronometru.Stop();
                Console.WriteLine($"\nTimp scurs:{cronometru.Elapsed}");
            }
            else
            {
                Console.WriteLine("Nu a fost gasit nimic,scuze.");
            }
        }
    }

    private void CalculeazaRelevantaInterogarii()
    {
        if (_interogare != null)
        {
            foreach (var cuvant in _interogare.DictionarCuvinte.Keys)
            {
                if (_documentGlobal.DictionarGlobal.ListaCuvinte.Contains(cuvant))
                {
                    _esteRelevantaInterogarea = true;
                    break;
                }
            }
        }
    }


    private static double CalculeazaSimilaritate(List<double> vectorA, List<double> vectorB)
    {
        var produsElemente = 0.0;
        var putereVectorA = 0.0;
        var putereVectorB = 0.0;
        for (var index = 0; index < vectorA.Count; index++)
        {
                produsElemente += vectorA[index] * vectorB[index];

            if (vectorA[index] > 0)
                putereVectorA += Math.Pow(vectorA[index], 2);
            if (vectorB[index] > 0)
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

    private void RealizeazaVectorulNormalizatAlInterogarii(Interogare interogare)
    {
        Dictionary<int, double> frecventeNormalizate = new(500);
        foreach (var cuvant in _documentGlobal.DictionarGlobal.ListaCuvinte)
        {
            if (interogare.DictionarCuvinte.ContainsKey(cuvant))
            {
                var indexCuvant = _documentGlobal.DictionarGlobal.ListaCuvinte.IndexOf(cuvant);
                var frecventaCuvantuluiNormalizata =
                    CalculeazaFrecventaCuvantuluiInInterogare(_documentGlobal.DictionarGlobal.ListaCuvinte[indexCuvant],
                        interogare);
                frecventeNormalizate.Add(indexCuvant, frecventaCuvantuluiNormalizata);
            }
            else
            {
                frecventeNormalizate.Add(_documentGlobal.DictionarGlobal.ListaCuvinte.IndexOf(cuvant), 0);
            }
        }

        _dictionarInterogareNormalizat = frecventeNormalizate;
    }

    public void RealizeazaVectoreleNormalizateDeAtribute()
    {
        foreach (var articol in _documentGlobal.ListaArticoleNormalizate) RealizeazaVectorulNormalizatDeAtribute(articol);
    }

    private void RealizeazaVectorulNormalizatDeAtribute(Articol articol)
    {
        Dictionary<int, double> frecventeNormalizate = new(30000);
        foreach (var cuvant in _documentGlobal.DictionarGlobal.ListaCuvinte)
        {
            if (articol.DictionarCuvinte.Keys.Contains(cuvant))
            {
                var indexCuvant = _documentGlobal.DictionarGlobal.ListaCuvinte.IndexOf(cuvant);
                var frecventaCuvantuluiNormalizata =
                    CalculeazaFrecventaCuvantuluiInArticol(_documentGlobal.DictionarGlobal.ListaCuvinte[indexCuvant],
                        articol);
                frecventeNormalizate.Add(indexCuvant, frecventaCuvantuluiNormalizata);
            }
            else
            {
                frecventeNormalizate.Add(_documentGlobal.DictionarGlobal.ListaCuvinte.IndexOf(cuvant), 0);
            }
        }
        
        AdaugaCuvantInDictionar(_dictionarDocumenteNormalizate, new(articol.Titlu, frecventeNormalizate));
    }

    private void AdaugaCuvantInDictionar(Dictionary<string, Dictionary<int, double>> dictionar,
        KeyValuePair<string, Dictionary<int, double>> pereche)
    {
        if (!dictionar.ContainsKey(pereche.Key)) dictionar.Add(pereche.Key, pereche.Value);
    }


    private double CalculeazaIdf(string atribut)
    {
        var nrTotalDocumente = _documentGlobal.DocumenteCaSiDictionare.Count;
        var nrDocumenteContinandAtributul = ReturneazaNrDocumenteCuAtributul(atribut);
        if (nrDocumenteContinandAtributul > 0)
            return Math.Log((double) nrTotalDocumente / nrDocumenteContinandAtributul);
        return 0;
    }

    private double CalculeazaNormalizareaNominala(Articol articol, string atribut)
    { 
        var aparitiiArticol = 0;
        if (_documentGlobal.DictionarGlobal.ListaCuvinte.Contains(atribut))
        {
            if (articol.DictionarCuvinte.ContainsKey(atribut))
                aparitiiArticol = articol.DictionarCuvinte[atribut];
            else
                return 0;
        }

        double frecventaMaxima = articol.ExtrageFrecventaMaxima();
        if (frecventaMaxima > 0)
        {
            return aparitiiArticol / frecventaMaxima;
        }
        else
        {
            return 0;
        }
    }

    private double CalculeazaNormalizareaNominala(Interogare interogare, string atribut)
    {
        if (_documentGlobal.DictionarGlobal.ListaCuvinte.Contains(atribut))
        {
            var aparitiiArticol = interogare.DictionarCuvinte[atribut];
            return (double) aparitiiArticol / interogare.ExtrageFrecventaMaxima();
        }
        else
        {
            return 0;
        }

    }

    private double CalculeazaFrecventaCuvantuluiInArticol(string atribut, Articol articol)
    {
        return CalculeazaIdf(atribut) *  CalculeazaNormalizareaNominala(articol, atribut);
    }

    private double CalculeazaFrecventaCuvantuluiInInterogare(string atribut, Interogare interogare)
    {
        return CalculeazaIdf(atribut) * CalculeazaNormalizareaNominala(interogare, atribut);
    }

    private int ReturneazaNrDocumenteCuAtributul(string atribut)
    {
        var indexCuvant = _documentGlobal.DictionarGlobal.ListaCuvinte.IndexOf(atribut);
        var numaratorDocumente = 0;
        foreach (var document in _documentGlobal.DocumenteCaSiDictionare.Values)
            if (document.ContainsKey(indexCuvant))
                numaratorDocumente++;

        return numaratorDocumente;
    }
}