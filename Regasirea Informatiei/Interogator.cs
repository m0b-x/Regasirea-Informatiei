using System.Diagnostics;

namespace Regasirea_Informatiei;

public class Interogator
{
    private readonly DocumentGlobal _documentGlobal;
    private readonly Dictionary<string, double> _dictionarIdf = new(Constante.NumarCuvinteUniceEstimat);
    private readonly Dictionary<int, int> _dictionarAparitiiInDocumente = new(Constante.NumarCuvinteUniceEstimat);

    private readonly Dictionary<string, Dictionary<int, double>> _dictionarDocumenteNormalizate =
        new(Constante.NumarDocumenteEstimat);

    private Dictionary<int, double> _dictionarInterogareNormalizat = new(Constante.NumarEstimatCuvinteInterogare);
    private Interogare? _interogare;

    private readonly List<KeyValuePair<double, string>> _similaritateDocumente =
        new(Constante.NumarCuvinteEstimatDocument);

    private double _entropieDocumente;

    public Interogator(ref DocumentGlobal documentGlobal)
    {
        _documentGlobal = documentGlobal;
    }

    private string? PropozitieInterogare { get; set; }

    public void InterogheazaDocumente()
    {
        Console.Write("Definiti o interogare: ");
        PropozitieInterogare = Console.ReadLine() ?? throw new InvalidOperationException();

        var cronometru = new Stopwatch();
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
                CalculeazaEntropiaDocumentelor();
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

    private void CalculeazaEntropiaDocumentelor()
    {
        int nrTopicuriTotale = 0;

        foreach (var tema in DocumentGlobal.ToateTopicurile)
        {
            nrTopicuriTotale += tema.Value;
        }

        foreach (var tema in DocumentGlobal.ToateTopicurile)
        {
            _entropieDocumente -= (double) tema.Value / nrTopicuriTotale *
                                  Math.Log2((double) tema.Value / nrTopicuriTotale);
        }

        Console.WriteLine(_entropieDocumente);
    }

    public double CalculeazaCastigulInformatieiPentruAtributul(string atribut)
    {
        int aparitiileAtributuluiInDocumente = ReturneazaNrDocumenteCuAtributul(atribut);
        int neaparitiileAtributului =
            _documentGlobal.ListaDocumenteNormalizate.Count - aparitiileAtributuluiInDocumente;

        double castig = _entropieDocumente -
                        (double) aparitiileAtributuluiInDocumente / _documentGlobal.ListaDocumenteNormalizate.Count -
                        (double) neaparitiileAtributului / _documentGlobal.ListaDocumenteNormalizate.Count;

        return castig;
    }

    private void AfiseazaSimilaritatile()
    {
        if (_similaritateDocumente.Count > 0)
        {
            _similaritateDocumente.Sort((cuvantA, cuvantB) => cuvantB.Key.CompareTo(cuvantA.Key));

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
            var similaritate = CalculeazaSimilariteateaCuInterogarea(_dictionarInterogareNormalizat, document.Value);
            if (similaritate > 0)
                _similaritateDocumente.Add(new KeyValuePair<double, string>(similaritate, document.Key));
        }
    }

    private bool EsteRelevantaInterogarea()
    {
        if (_interogare != null)
            foreach (var cuvant in _interogare.DictionarCuvinte.Keys)
                if (_documentGlobal.DictionarGlobal.ListaCuvinte.Contains(cuvant))
                    return true;

        return false;
    }

    private double CalculeazaSimilariteateaCuInterogarea(Dictionary<int, double> dictionarInterogare,
        Dictionary<int, double> dictionarDocument)
    {
        var produsElemente = 0.0;
        var produsInterogare = 0.0;
        var produsDocument = 0.0;
        foreach (var cuvant in dictionarInterogare)
        {
            produsInterogare += Math.Pow(cuvant.Value, 2);
            if (dictionarDocument.ContainsKey(cuvant.Key))
                produsElemente += cuvant.Value * dictionarDocument[cuvant.Key];
        }

        foreach (var cuvant in dictionarDocument) produsDocument += Math.Pow(cuvant.Value, 2);

        return produsElemente / (Math.Sqrt(produsInterogare) * Math.Sqrt(produsDocument));
    }

    private void RealizeazaVectoriiNormalizatiDeAtribute()
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
                var frecventaCuvantuluiNormalizata = CalculeazaFrecventaCuvantuluiInInterogare(cuvant.Key, interogare);
                frecventeNormalizate.Add(_documentGlobal.DictionarGlobal.ListaCuvinte.IndexOf(cuvant.Key),
                    frecventaCuvantuluiNormalizata);
            }
            else
            {
                Console.WriteLine("#"+cuvant);
            }
        }

        _dictionarInterogareNormalizat = frecventeNormalizate;
    }

    private double CalculeazaIdf(string atribut)
    {
        if (!_dictionarIdf.ContainsKey(atribut))
        {
            var nrDocumenteContinandAtributul = ReturneazaNrDocumenteCuAtributul(atribut);
            var nrTotalDocumente = _documentGlobal.ListaDocumenteNormalizate.Count;
            var idf = Math.Log((double) nrTotalDocumente / nrDocumenteContinandAtributul);

            _dictionarIdf.Add(atribut, idf);
            return idf;
        }
        else
        {
            return _dictionarIdf[atribut];
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
        if (!_dictionarAparitiiInDocumente.ContainsKey(indexCuvant))
        {
            var numaratorDocumente = 0;
            foreach (var document in _documentGlobal.ListaDocumenteNormalizate)
                if (document.DictionarCuvinte.ContainsKey(atribut))
                    numaratorDocumente++;

            _dictionarAparitiiInDocumente.Add(indexCuvant, numaratorDocumente);
            return numaratorDocumente;
        }
        else
        {
            return _dictionarAparitiiInDocumente[indexCuvant];
        }
    }

    private double CalculeazaNormalizareaNominala(Interogare interogare, string atribut)
    {
        if (_documentGlobal.DictionarGlobal.ListaCuvinte.Contains(atribut))
        {
            var aparitiiDocument = interogare.DictionarCuvinte[atribut];

            return (double) aparitiiDocument / interogare.FrecventaMaxima;
        }
        else
        {
            return 0;
        }
    }

    private double CalculeazaNormalizareaNominala(Document document, string atribut)
    {
        if (document.DictionarCuvinte.ContainsKey(atribut))
        {
            var aparitiiDocument = document.DictionarCuvinte[atribut];
            var normalizareNominala = (double) aparitiiDocument / document.FrecventaMaxima;
            return normalizareNominala;
        }
        else
        {
            return 0;
        }
    }
}