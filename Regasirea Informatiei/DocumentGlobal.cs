using System.Text;

namespace Regasirea_Informatiei;

public class DocumentGlobal
{
    public static Dictionary<string, int> ToateTopicurile { get; private set; } = new();

    private double _entropieTotala;

    public bool EsteNevoieDeSuprascriere;

    private readonly string _numeFisier = "FisierGlobal.txt";

    private HashSet<Document> _listaDocumenteNormalizate =
        new HashSet<Document>(Constante.NumarDocumenteEstimat, Document.PathFisierComparer);

    private readonly Dictionary<string, double> _dictionarCastigInformational = new(Constante.NumarCuvinteUniceEstimat);

    public Dictionary<string, double> DictionarCastigInformational => _dictionarCastigInformational;

    public HashSet<Document> ListaDocumenteNormalizate
    {
        get => _listaDocumenteNormalizate;
        private set => _listaDocumenteNormalizate = value;
    }

    public DictionarGlobal DictionarGlobal { get; set; }

    public DocumentGlobal(ref DictionarGlobal dictionarGlobal)
    {
        DictionarGlobal = dictionarGlobal;
        if (File.Exists(_numeFisier))
        {
            CitesteDate();
            EsteNevoieDeSuprascriere = false;
        }
        else
        {
            File.Create(_numeFisier);
            EsteNevoieDeSuprascriere = true;
        }


        Console.WriteLine("Fisier Global initializat.");
    }

    private void CitesteDate()
    {
        using var cititor = new StreamReader(_numeFisier);
        while (!cititor.EndOfStream)
        {
            var linie = cititor.ReadLine() ?? throw new InvalidOperationException();

            if (linie.StartsWith(Constante.DelimitatorScurtaAtribut))
            {
                DictionarGlobal.ListaCuvinte.Add(linie.Substring(Constante.DelimitatorAtribut.Length + 1));
            }
            else if (!linie.StartsWith(Constante.DelimitatorAtributPrescurtat))
            {
                _listaDocumenteNormalizate.Add(new Document(new StringBuilder(linie)));
            }
        }
    }


    public void ScrieDocumenteleInFisier()
    {
        if (EsteNevoieDeSuprascriere)
        {
            using var scriitor = new StreamWriter(_numeFisier, false);
            scriitor.AutoFlush = true;
            SortedSet<string> listaAtributeSortate = new(DictionarGlobal.ListaCuvinte);
            foreach (var atribut in listaAtributeSortate)
                scriitor.WriteLine($"{Constante.DelimitatorAtribut} {atribut}");

            scriitor.WriteLine(Constante.SimbolDate);

            foreach (var document in _listaDocumenteNormalizate)
            {
                scriitor.WriteLine(document.DocumentNormalizat);
            }
        }
    }

    public void AdaugaDocumentInLista(string document)
    {
        _listaDocumenteNormalizate.Add(new Document(new StringBuilder(document)));
    }

    private void StergeTopicuriIrelevante(ref int nrTopicuriTotale)
    {
        Dictionary<string, int> toateTopicurileTriate = new();
        Console.WriteLine("Teme si procentul lor");
        foreach (var tema in ToateTopicurile)
        {
            double procent = (double) tema.Value / nrTopicuriTotale;
            Console.WriteLine($"\t{tema.Key}-{procent}");
            if (procent is > Constante.PragInferiorTopicuri and < Constante.PragSuperiorTopicuri)
            {
                nrTopicuriTotale -= tema.Value;
                toateTopicurileTriate.Add(tema.Key, tema.Value);
            }
        }

        ToateTopicurile = toateTopicurileTriate;
    }

    private void TriazaDocumenteleIrelevante()
    {
        HashSet<Document> listaDocumenteNormalizateTriate =
            new HashSet<Document>(Constante.NumarDocumenteEstimat, Document.PathFisierComparer);

        
        foreach (var document in ListaDocumenteNormalizate)
        {
            if (document.Topicuri.Count > 0)
            {
                document.Topicuri.RemoveRange(1, document.Topicuri.Count - 1);
                listaDocumenteNormalizateTriate.Add(document);
            }
        }

        ListaDocumenteNormalizate = listaDocumenteNormalizateTriate;
    }

    private void CalculeazaTopicuriTotale(out int nrTopicuriTotale)
    {
        nrTopicuriTotale = 0;
        foreach (var topic in DocumentGlobal.ToateTopicurile)
        {
            nrTopicuriTotale += topic.Value;
        }
    }

    public void SelecteazaTrasaturileRelevante()
    {
        CalculeazaTopicuriTotale(out var nrTopicuriTotale);

        StergeTopicuriIrelevante(ref nrTopicuriTotale);
        Console.WriteLine();
        TriazaDocumenteleIrelevante();

        foreach (var topicuri in DocumentGlobal.ToateTopicurile)
        {
            double impartire = (double) topicuri.Value / nrTopicuriTotale;
            _entropieTotala -= impartire *
                               Math.Log2(impartire);
        }
        Console.WriteLine("Entropie Totala");
        Console.WriteLine($"\t{_entropieTotala}");
        Console.WriteLine("Atribute Relevante:");
        
        foreach (var cuvant in DictionarGlobal.ListaCuvinte)
        {
            if (!_dictionarCastigInformational.ContainsKey(cuvant))
            {
                double gainCuvant = _entropieTotala;

                double entropieCuAtribute = 0;
                int topicuriCuAtribut = 0;

                double entropieFaraAtribute = 0;
                int topicuriFaraAtribut = 0;

                Dictionary<string, int> dictionarTopicuriCuAtribut =
                    new Dictionary<string, int>(DocumentGlobal.ToateTopicurile.Count);

                Dictionary<string, int> dictionarTopicuriFaraAtribut =
                    new Dictionary<string, int>(DocumentGlobal.ToateTopicurile.Count);

                foreach (var document in ListaDocumenteNormalizate)
                {
                    if (document.DictionarCuvinte.ContainsKey(cuvant))
                    {
                        topicuriCuAtribut += document.Topicuri.Count;
                        foreach (var topic in document.Topicuri)
                        {
                            if (dictionarTopicuriCuAtribut.ContainsKey(topic))
                            {
                                dictionarTopicuriCuAtribut[topic]++;
                            }
                            else
                            {
                                dictionarTopicuriCuAtribut.Add(topic, 1);
                            }
                        }
                    }
                    else
                    {
                        topicuriFaraAtribut += document.Topicuri.Count;
                        foreach (var topic in document.Topicuri)
                        {
                            if (dictionarTopicuriFaraAtribut.ContainsKey(topic))
                            {
                                dictionarTopicuriFaraAtribut[topic]++;
                            }
                            else
                            {
                                dictionarTopicuriFaraAtribut.Add(topic, 1);
                            }
                        }
                    }
                }

                foreach (var topic in dictionarTopicuriCuAtribut)
                {
                    double impartire = (double) topic.Value / nrTopicuriTotale;
                    entropieCuAtribute -=
                        impartire * Math.Log2(impartire);
                }
            
                foreach (var topic in dictionarTopicuriFaraAtribut)
                {
                    double impartire = (double) topic.Value / nrTopicuriTotale;
                    entropieFaraAtribute -=
                        impartire * Math.Log2(impartire);
                }
                double impartireCuAtribute = (double) topicuriCuAtribut / nrTopicuriTotale;
                double impartireFaraAtribute = (double) topicuriFaraAtribut / nrTopicuriTotale;
        
                gainCuvant = gainCuvant - impartireCuAtribute * entropieCuAtribute - impartireFaraAtribute * entropieFaraAtribute;
                if (gainCuvant > Constante.PragGain)
                {
                    Console.WriteLine($"\t{cuvant}->{gainCuvant}");
                    _dictionarCastigInformational.Add(cuvant, gainCuvant);
                }
                else
                {
                    if (EsteNevoieDeSuprascriere == false)
                        EsteNevoieDeSuprascriere = true;
                }
            }
        }

        //Refacere Documente
        if (EsteNevoieDeSuprascriere)
        {
            DictionarGlobal.EsteNevoieDeSupraScriere = true;
            DictionarGlobal.ListaCuvinte = new List<string>(_dictionarCastigInformational.Keys);
            foreach (var document in _listaDocumenteNormalizate)
            {
                document.RefaDictionarulDeCuvinte();
                document.RefaFormaVectoriala();
            }
        }
    }
}