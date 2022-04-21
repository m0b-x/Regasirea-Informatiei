using System.Diagnostics;
using Regasirea_Informatiei;

const string numeFolder = "Documente32";
const string patternFisier = "*.xml";
//Etapa 1
var cronometru = new Stopwatch();
cronometru.Start();

HashSet<Document> documente = new HashSet<Document>(Constante.NumarDocumenteEstimat,Document.PathFisierComparer);
foreach (var pathFisier in Directory.EnumerateFiles(numeFolder, patternFisier))
{
    Document document = new Document(pathFisier);
    documente.Add(document);
}

cronometru.Stop();
Console.WriteLine($"Timp citire {cronometru.Elapsed}");
Document.ScrieDateInFisiereGlobale();

//Etapa 2
var interogator = new Interogator(ref Document.DocumentGlobal);
interogator.InterogheazaDocumente();
