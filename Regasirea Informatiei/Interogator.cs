using System.Collections;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;

namespace Regasirea_Informatiei;

public class Interogator
{
    private string _stringInterogare;
    private Interogare _interogare;
    private DocumentGlobal _documentGlobal;
    private Dictionary<string, Dictionary<int, double>> _dictionarDocumenteNormalizate = new();
    private Dictionary<int, double> _dictionarNormalizatInterogare = new();
    private SortedList<double, string> _articoleSortateDupaSimilaritate = new();

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
        _stringInterogare = Console.ReadLine();
        if (String.IsNullOrEmpty(_stringInterogare))
        {
            Console.WriteLine("Eroare:stringul este null!");
        }
        else
        {
            _interogare = new Interogare(_stringInterogare);
            List<double> vectorInterogare = _dictionarNormalizatInterogare.Values.ToList();
            RealizeazaVectorulNormalizatAlInterogarii(_interogare);
            foreach (var articol in articole)
            {
                RealizeazaVectorulNormalizatDeAtribute(articol);
            }

            foreach (var document in _dictionarDocumenteNormalizate)
            {
                List<double> vectorDocument = new();
                foreach (var valoareVector in document.Value)
                {
                    vectorDocument.Add(valoareVector.Value);
                }

                _articoleSortateDupaSimilaritate.Add(CalculeazaSimilaritatea(vectorInterogare, vectorDocument),
                    document.Key);
            }
        }

        Console.WriteLine("Articole Rezultate");
        foreach (var articoleSortate in _articoleSortateDupaSimilaritate)
        {
            Console.WriteLine($"{articoleSortate.Value}");
        }
    }

    public static double CalculeazaSimilaritatea(List<double> vectorA, List<double> vectorB)
    {
        int numarulMaiMareDeElemente = 0;
        numarulMaiMareDeElemente = ((vectorB.Count < vectorA.Count) ? vectorB.Count : vectorA.Count);
        double sumaVectoriala = 0.0d;
        double putereaPrimuluiVector = 0.0d;
        double putereaAlDoileaVector = 0.0d;
        for (int n = 0; n < numarulMaiMareDeElemente; n++)
        {
            sumaVectoriala += vectorA[n] * vectorB[n];
            putereaPrimuluiVector += Math.Pow(vectorA[n], 2);
            putereaAlDoileaVector += Math.Pow(vectorB[n], 2);
        }

        return sumaVectoriala / (Math.Sqrt(putereaPrimuluiVector) * Math.Sqrt(putereaAlDoileaVector));
    }

    public void RealizeazaVectorulNormalizatAlInterogarii(Interogare interogare)
    {
        Dictionary<int, double> frecventeNormalizate = new();
        foreach (var aparitieCuvant in interogare.DictionarNormalizat)
        {
            double frecventaCuvantuluiNormalizata =
                ReturneazaFrecventaCuvantuluiInInterogare(Articol.DictionarGlobal.ListaCuvinte[aparitieCuvant.Key],
                    interogare);
            frecventeNormalizate.Add(aparitieCuvant.Key, frecventaCuvantuluiNormalizata);
        }

        _dictionarNormalizatInterogare = frecventeNormalizate;
    }

    public void RealizeazaVectorulNormalizatDeAtribute(Articol articol)
    {
        foreach (var documentCaSiDictionar in _documentGlobal.DocumenteCaSiDictionare)
        {
            Dictionary<int, double> frecventeNormalizate = new();
            foreach (var aparitieCuvant in documentCaSiDictionar.Value)
            {
                double frecventaCuvantuluiNormalizata =
                    ReturneazaFrecventaCuvantuluiInArticol(Articol.DictionarGlobal.ListaCuvinte[aparitieCuvant.Key],
                        articol);
                frecventeNormalizate.Add(aparitieCuvant.Key, frecventaCuvantuluiNormalizata);
            }

            _dictionarDocumenteNormalizate.Add(documentCaSiDictionar.Key, frecventeNormalizate);
        }
    }

    private double CalculeazaIDF(string atribut)
    {
        int nrTotalDocumente = Articol.DocumentScriereGlobal.DocumenteCaSiStringuri.Count;
        int nrDocumenteContinandAtributul = ReturneazaNrDocumenteCuAtributul(atribut);
        return Math.Log(nrTotalDocumente / nrDocumenteContinandAtributul);
    }

    private double CalculeazaNormalizareaNominala(Articol articol, string atribut)
    {
        int indexulAtributului = Articol.DictionarGlobal.ListaCuvinte.IndexOf(atribut);
        int numarAparitii = 0;
        if (indexulAtributului != -1)
        {
            numarAparitii = articol.DictionarCuvinte[atribut];
        }

        return numarAparitii / articol.ExtrageFrecventaMaxima();
    }

    private double CalculeazaNormalizareaNominala(Interogare interogare, string atribut)
    {
        int indexulAtributului = Articol.DictionarGlobal.ListaCuvinte.IndexOf(atribut);
        int numarAparitii = 0;
        if (indexulAtributului != -1)
        {
            numarAparitii = interogare.DictionarCuvinte[atribut];
        }

        return numarAparitii / interogare.ExtrageFrecventaMaxima();
    }

    public double ReturneazaFrecventaCuvantuluiInArticol(string atribut, Articol articol)
    {
        return CalculeazaIDF(atribut) * CalculeazaNormalizareaNominala(articol, atribut);
    }

    public double ReturneazaFrecventaCuvantuluiInInterogare(string atribut, Interogare interogare)
    {
        return CalculeazaIDF(atribut) * CalculeazaNormalizareaNominala(interogare, atribut);
    }

    public bool ContineDocumentulAtributul(string document, string atribut)
    {
        int numaratorDocumente = 0;

        if (document.Split('#')[1].Contains(atribut))
        {
            return true;
        }

        return false;
    }

    public int ReturneazaNrDocumenteCuAtributul(string atribut)
    {
        int numaratorDocumente = 1;
        foreach (var document in _documentGlobal.DocumenteCaSiStringuri)
        {
            if (document.Contains(atribut))
            {
                numaratorDocumente++;
            }
        }

        return numaratorDocumente;
    }
}