using System.Text;

namespace Regasirea_Informatiei;

public class Interogare
{

    public static DictionarGlobal DictionarGlobal = new DictionarGlobal();

    private static DictionarStopWords _fisierDictionarStopWords = new DictionarStopWords();
    private static SnowballStemmer _stemmerCuvinte = new();

    private readonly string _interogare;
    private StringBuilder _documentNormalizat = new StringBuilder();
    private Dictionary<string, int> _dictionarCuvinte = new Dictionary<string, int>();
    private Dictionary<int, int> _dictionarNormalizat = new Dictionary<int, int>();


    public String StringInterogare
    {
        get { return _interogare; }
    }

    public Dictionary<string, int> DictionarCuvinte
    {
        get { return _dictionarCuvinte; }
    }
    public Dictionary<int, int> DictionarNormalizat
    {
        get { return (_dictionarNormalizat); }
    }

    public Interogare(String interogare)
    {
        _interogare = interogare;
        CitesteCuvinte();
        RealizeazaFormaVectoriala();
    }

    public void CitesteCuvinte()
    {
        String continutInterogare = _interogare;

        var punctuatii = continutInterogare.Where(Char.IsPunctuation).Distinct().ToArray();
        IEnumerable<string> cuvinte = continutInterogare.Split().Select(punctuatie => punctuatie.Trim(punctuatii));

        cuvinte = cuvinte.Select(cuvant => cuvant.ToLowerInvariant().Replace(" ", "")).ToList()
            .Where(cuvant => !string.IsNullOrEmpty(cuvant) &&
                             UtilitatiCuvinte.EsteCuvantValid(cuvant) &&
                             !UtilitatiCuvinte.EsteAbreviere(cuvant)).Except(_fisierDictionarStopWords.ListaStopWords)
            .Distinct();

        cuvinte = ReturneazaRadacinileCuvintelor(cuvinte);

        foreach (string cuvant in cuvinte)
        {
            AdaugaCuvantInDictionar(cuvant, _dictionarCuvinte);
            AdaugaCuvantInDictionar(DictionarGlobal.ListaCuvinte.IndexOf(cuvant), _dictionarNormalizat);
            
        }
    }

    public IEnumerable<string> ReturneazaRadacinileCuvintelor(IEnumerable<string> cuvinte)
    {
        List<string> cuvinteStemate = new List<string>();

        foreach (var cuvant in cuvinte)
        {
            if (!cuvinteStemate.Contains(cuvant))
                cuvinteStemate.Add(_stemmerCuvinte.Stem(cuvant));
        }

        return cuvinteStemate.AsEnumerable();
    }

    public void RealizeazaFormaVectoriala()
    {
        _documentNormalizat = new StringBuilder();
        _documentNormalizat.Append($"{_interogare}# ");
        foreach (var cuvant in _dictionarCuvinte)
        {
            _documentNormalizat.Append($"{DictionarGlobal.ListaCuvinte.IndexOf(cuvant.Key)}:{cuvant.Value} ");
        }
    }
    public void AdaugaCuvantInDictionar(int cuvant, Dictionary<int, int> dictionar)
    {
        if (_dictionarNormalizat.ContainsKey(cuvant))
        {
            _dictionarNormalizat[cuvant] = dictionar[cuvant] + 1;
        }
        else
        {
            _dictionarNormalizat.Add(cuvant, 1);
        }
    }
    public void AdaugaCuvantInDictionar(string cuvant, Dictionary<string, int> dictionar)
    {
        if (_dictionarCuvinte.ContainsKey(cuvant))
        {
            _dictionarCuvinte[cuvant] = dictionar[cuvant] + 1;
        }
        else
        {
            _dictionarCuvinte.Add(cuvant, 1);
        }
    }
    
    public int ExtrageFrecventaMaxima()
    {
        int max = -1;
        foreach (var cuvant in _dictionarCuvinte)
        {
            if (cuvant.Value > max)
            {
                max = cuvant.Value;
            }
        }

        return max;

    }
}