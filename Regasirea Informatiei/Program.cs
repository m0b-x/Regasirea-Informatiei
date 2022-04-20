using System.Diagnostics;
using Regasirea_Informatiei;

const string numeFolder = "Documente32";
const string patternFisier = "*.xml";
//Etapa 1
var cronometru = new Stopwatch();
cronometru.Start();

HashSet<Articol> articole = new HashSet<Articol>(32);
foreach (var pathFisier in Directory.EnumerateFiles(numeFolder, patternFisier))
    articole.Add(new Articol(pathFisier));


cronometru.Stop();
Console.WriteLine($"Timp citire {cronometru.Elapsed}");
Articol.ScrieDateInFisiereGlobale();

//Etapa 2
var interogator = new Interogator(ref Articol.DocumentGlobal);
interogator.RealizeazaVectoriiNormalizatiDeAtribute();
interogator.InterogheazaArticole();
