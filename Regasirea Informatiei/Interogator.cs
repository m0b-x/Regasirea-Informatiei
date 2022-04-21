using System.Diagnostics;

namespace Regasirea_Informatiei;

public class Interogator
{
    private static DocumentGlobal _documentGlobal = null!;
    private static readonly Dictionary<string, double> DictionarIdf = new(Constante.NumarCuvinteUniceEstimat);
    private static readonly Dictionary<string, double> DictionarFrecventaNomnialaAtricole = new(Constante.NumarCuvinteUniceEstimat);
    private static readonly Dictionary<int, int> DictionarAparitiiInDocumente = new(Constante.NumarCuvinteUniceEstimat);

    private readonly Dictionary<string, Dictionary<int, double>> _dictionarDocumenteNormalizate = new(Constante.NumarDocumenteEstimat);
    private Dictionary<int, double> _dictionarInterogareNormalizat = new(Constante.NumarEstimatCuvinteInterogare);
    private Interogare? _interogare;
    private readonly List<KeyValuePair<double, string>> _similaritateDocumente = new(Constante.NumarCuvinteEstimatDocument);

    public Interogator(ref DocumentGlobal documentGlobal)
    {
        _documentGlobal = documentGlobal;
    }

    private string? PropozitieInterogare { get; set; }

    public void InterogheazaDocumente()
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
                CalculeazaSimilaritateaDocumentelor();
                AfiseazaSimilaritatile();

                cronometru.Stop();
                Console.WriteLine($"\nTimp interogare scurs:{cronometru.Elapsed}");
            }
            else
            {
                Console.WriteLine("Nu a fost gasit nimic,scuze.(interogare irelevanta)");
            }
        }
    }

    private void AfiseazaSimilaritatile()
    {
        if (_similaritateDocumente.Count > 0)
        {
            _similaritateDocumente.Sort((x, y) => y.Key.CompareTo(x.Key));

            Console.WriteLine("Documente gasite:");

            foreach (var documenteSortate in _similaritateDocumente)
                Console.WriteLine($"{documenteSortate.Value} - {documenteSortate.Key}");
        }
        else
        {
            Console.WriteLine("Nu a fost gasit nimic,scuze.(0 documente similare)");
        }
    }

    private void CalculeazaSimilaritateaDocumentelor()
    {
        foreach (var document in _dictionarDocumenteNormalizate)
        {
            var similaritate = CalculeazaSimilariteateaCuInterogarea(_dictionarInterogareNormalizat,
                document.Value);
            if (similaritate > 0)
                _similaritateDocumente.Add(new KeyValuePair<double, string>(similaritate, document.Key));
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
        foreach (var document in _documentGlobal.ListaDocumenteNormalizate)
        {
            Dictionary<int, double> frecventeNormalizate = new(Constante.NumarCuvinteEstimatDocument);
        
            foreach (var cuvant in document.DictionarCuvinte)
            {
                var indexCuvant = _documentGlobal.DictionarGlobal.ListaCuvinte.IndexOf(cuvant.Key);
                var frecventaCuvantuluiNormalizata =
                    CalculeazaFrecventaCuvantuluiInDocument(_documentGlobal.DictionarGlobal.ListaCuvinte[indexCuvant],
                        document);
                frecventeNormalizate.Add(indexCuvant, frecventaCuvantuluiNormalizata);
            }
            _dictionarDocumenteNormalizate.Add(document.PathFisier, frecventeNormalizate);
        }
    }

    private void RealizeazaVectorulNormalizatAlInterogarii(Interogare interogare)
    {
        Dictionary<int, double> frecventeNormalizate = new(Constante.NumarCuvinteEstimatDocument);
        foreach (var cuvant in interogare.DictionarCuvinte)
        {
            if (_documentGlobal.DictionarGlobal.ListaCuvinte.Contains(cuvant.Key))
            {
                var frecventaCuvantuluiNormalizata =
                    CalculeazaFrecventaCuvantuluiInInterogare(cuvant.Key,
                        interogare);
                frecventeNormalizate.Add(_documentGlobal.DictionarGlobal.ListaCuvinte.IndexOf(cuvant.Key), 
                    frecventaCuvantuluiNormalizata);
            }
        }

        _dictionarInterogareNormalizat = frecventeNormalizate;
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
            else
            {
                DictionarIdf.Add(atribut, 0);
                return 0;
            }
        }
        else
        {
            return DictionarIdf[atribut];
        }
    }

    private double CalculeazaFrecventaCuvantuluiInDocument(string atribut, Document document)
    {
            return CalculeazaIdf(atribut) * CalculeazaNormalizareaNominala(document, atribut);
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
            var aparitiiDocument = interogare.DictionarCuvinte[atribut];
            return (double) aparitiiDocument / interogare.FrecventaMaxima > 0 ? interogare.FrecventaMaxima : 1;
            
        }
        else
        {
            return 0;
        }
    }

    private double CalculeazaNormalizareaNominala(Document document, string atribut)
    {
        if (!DictionarFrecventaNomnialaAtricole.ContainsKey(atribut))
        {
            if (_documentGlobal.DictionarGlobal.ListaCuvinte.Contains(atribut))
            {
                if (document.DictionarCuvinte.ContainsKey(atribut))
                {
                    var aparitiiDocument = document.DictionarCuvinte[atribut];
                    double normalizareNominala =  aparitiiDocument / document.FrecventaMaxima > 0 ? document.FrecventaMaxima: 1;
                    DictionarFrecventaNomnialaAtricole.Add(atribut,normalizareNominala);
                    return normalizareNominala;
                }
                else
                {
                    DictionarFrecventaNomnialaAtricole.Add(atribut, 0);
                    return 0;
                }
            }
            else
            {
                
                DictionarFrecventaNomnialaAtricole.Add(atribut, 0);
                return 0;
            }
        }
        else
        {
            return DictionarFrecventaNomnialaAtricole[atribut];
        }
    }
}