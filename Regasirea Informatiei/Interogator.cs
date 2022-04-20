using System.Diagnostics;

namespace Regasirea_Informatiei;

public class Interogator
{
    private static DocumentGlobal _documentGlobal = null!;
    private static readonly Dictionary<string, double> DictionarIdf = new(5000);
    private static readonly Dictionary<int, int> DictionarAparitiiInDocumente = new(5000);

    private readonly Dictionary<string, Dictionary<int, double>> _dictionarDocumenteNormalizate = new(7000);
    private Dictionary<int, double> _dictionarInterogareNormalizat = new(150);
    private Interogare? _interogare;
    private readonly List<KeyValuePair<double, string>> _similaritateArticole = new(7000);

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

            if (EsteRelevantaInterogarea())
            {
                RealizeazaVectorulNormalizatAlInterogarii(_interogare);
                RealizeazaVectoriiNormalizatiDeAtribute();
                CalculeazaSimilaritateaArticolelor();
                AfiseazaSimilaritatile();

                cronometru.Stop();
                Console.WriteLine($"\nTimp scurs:{cronometru.Elapsed}");
            }
            else
            {
                Console.WriteLine("Nu a fost gasit nimic,scuze.");
            }
        }
    }

    private void AfiseazaSimilaritatile()
    {
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
    }

    private void CalculeazaSimilaritateaArticolelor()
    {
        foreach (var document in _dictionarDocumenteNormalizate)
        {
            var similaritate = CalculeazaSimilariteateaCuInterogarea(_dictionarInterogareNormalizat,
                document.Value);
            if (similaritate > 0)
                _similaritateArticole.Add(new KeyValuePair<double, string>(similaritate, document.Key));
        }
    }

    private bool EsteRelevantaInterogarea()
    {
        if (_interogare != null)
        {
            foreach (var cuvant in _interogare.DictionarCuvinte.Keys)
            {
                if (_documentGlobal.DictionarGlobal.ListaCuvinte.Contains(cuvant))
                {
                    return true;
                }
            }
        }

        return false;
    }


    private static double CalculeazaSimilariteateaCuInterogarea(Dictionary<int, double> dictionarInterogare,
        Dictionary<int, double> dictionarDocument)
    {
        var produsElemente = 0.0;
        var produsInterogare = 0.0;
        var produsDocument = 0.0;
        foreach (var cuvant in dictionarInterogare)
        {
            produsInterogare += Math.Pow(cuvant.Value, 2);
            if(dictionarDocument.ContainsKey(cuvant.Key))
                produsElemente += cuvant.Value * dictionarDocument[cuvant.Key];
        }

        foreach (var cuvant in dictionarDocument)
        {
            produsDocument += Math.Pow(cuvant.Value, 2);
        }

        if (produsInterogare > 0 && produsDocument > 0)
        {
            return produsElemente / (Math.Sqrt(produsInterogare) * Math.Sqrt(produsDocument));
        }
        else
        {
            return 0;
        }
    }


    public void RealizeazaVectoriiNormalizatiDeAtribute()
    {
        foreach (var articol in _documentGlobal.ListaArticoleNormalizate)
            RealizeazaVectorulNormalizatDeAtribute(articol);
    }

    private void RealizeazaVectorulNormalizatDeAtribute(Articol articol)
    {
        Dictionary<int, double> frecventeNormalizate = new(5000);
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
        }

        AdaugaCuvantDistinctInDictionar(_dictionarDocumenteNormalizate, new(articol.Titlu, frecventeNormalizate));
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
        }

        _dictionarInterogareNormalizat = frecventeNormalizate;
    }

    private void AdaugaCuvantDistinctInDictionar(Dictionary<string, Dictionary<int, double>> dictionar,
        KeyValuePair<string, Dictionary<int, double>> pereche)
    {
        if (!dictionar.ContainsKey(pereche.Key)) dictionar.Add(pereche.Key, pereche.Value);
    }

    private double CalculeazaIdf(string atribut)
    {
        if (!DictionarIdf.ContainsKey(atribut))
        {
            var nrDocumenteContinandAtributul = ReturneazaNrDocumenteCuAtributul(atribut);
            if (nrDocumenteContinandAtributul > 0)
            {
                var nrTotalDocumente = _documentGlobal.DocumenteCaSiDictionare.Count;
                var idf = Math.Log((double) nrTotalDocumente / nrDocumenteContinandAtributul);
                DictionarIdf.Add(atribut, idf);
                return idf;
            }

            DictionarIdf.Add(atribut, 0);
            return 0;
        }

        return DictionarIdf[atribut];
    }

    private double CalculeazaFrecventaCuvantuluiInArticol(string atribut, Articol articol)
    {
        return CalculeazaIdf(atribut) * CalculeazaNormalizareaNominala(articol, atribut);
    }

    private double CalculeazaFrecventaCuvantuluiInInterogare(string atribut, Interogare interogare)
    {
        return CalculeazaIdf(atribut) * CalculeazaNormalizareaNominala(interogare, atribut);
    }

    private int ReturneazaNrDocumenteCuAtributul(string atribut)
    {
        var indexCuvant = _documentGlobal.DictionarGlobal.ListaCuvinte.IndexOf(atribut);
        if (!DictionarAparitiiInDocumente.ContainsKey(indexCuvant))
        {
            var numaratorDocumente = 0;
            foreach (var document in _documentGlobal.DocumenteCaSiDictionare.Values)
                if (document.ContainsKey(indexCuvant))
                    numaratorDocumente++;

            DictionarAparitiiInDocumente.Add(indexCuvant, numaratorDocumente);
            return numaratorDocumente;
        }
        else
        {
            return DictionarAparitiiInDocumente[indexCuvant];
        }
    }
    private double CalculeazaNormalizareaNominala(Interogare interogare, string atribut)
    {
        if (_documentGlobal.DictionarGlobal.ListaCuvinte.Contains(atribut))
        {
            var aparitiiArticol = interogare.DictionarCuvinte[atribut];
            return (double) aparitiiArticol / interogare.FrecventaMaxima > 0 ? interogare.FrecventaMaxima : 1;
        }

        return 0;
    }

    private double CalculeazaNormalizareaNominala(Articol articol, string atribut)
    {
        if (_documentGlobal.DictionarGlobal.ListaCuvinte.Contains(atribut))
        {
            if (articol.DictionarCuvinte.ContainsKey(atribut))
            {
                var aparitiiArticol = articol.DictionarCuvinte[atribut];
                return (double) aparitiiArticol / articol.FrecventaMaxima > 0 ? articol.FrecventaMaxima : 1;
            }

            return 0;
        }

        return 0;
    }
}