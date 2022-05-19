namespace Regasirea_Informatiei;

public static class Constante
{

    public const int PragSuperior = 95;
    public const int PragInferior = 5;
    
    public const int NumarEstimatCuvinteInterogare = 150;
    public const int NumarCuvinteEstimatDocument = 750;
    public const int NumarCuvinteUniceEstimat = 5000;
    public const int NumarDocumenteEstimat = 50;
    public const int NumarEstimatStopWords = 450;
    public const int LungimeMedieCuvant = 5;
    public const int NumarCuvinteEstimatDictionar = 5000;
    
    public const double PragGain = 0.25;
    public const double PragSuperiorTopicuri = 0.95;
    public const double PragInferiorTopicuri = 0.2;
    
    public const string DelimitatorInceputTopicuri = "<codes class=\"bip:topics:1.0\">";
    public const string SimbolDate = "@data";
    public const string DelimitatorAtribut = "@attribute";
    public const string DelimitatorScurtaAtribut = "@a";
    public const string DelimitatorClase = "#_";
    public const string InlocuitorPunctuatie = " ";
    public const string InceputParagraf = "<p>";
    public const string SfarsitParagraf = "</p>";
    public const string InlocuitorParagrf = " ";
    public const char DelimitatorAtributPrescurtat = '@';
    public const char SimbolTitlu = '#';
    public const char DelimitatorGeneral = ' ';
    public const char DelimitatorTopicuri = '_';
    public const char DelimitatorClaseCitire = '"';
    public static readonly char[] DelimitatorIndexFrecventa = new []{' ', ':'};
}