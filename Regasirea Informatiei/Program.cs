using System.Diagnostics;
using Regasirea_Informatiei;

var articole = new List<Articol>(100);


const string numeFolder = "Documente32";
const string patternFisier = "*.xml";

var timer = new Stopwatch();
timer.Start();

foreach (var pathFisier in Directory.EnumerateFiles(numeFolder, patternFisier))
    articole.Add(new Articol(pathFisier));

Console.WriteLine($"Timp citire {timer.Elapsed}");
Articol.ScrieDateInFisiereGlobale();

timer.Stop();
var interogator = new Interogator(ref Articol.DocumentGlobal, ref articole);
interogator.RealizeazaVectoreleNormalizateDeAtribute();
interogator.InterogheazaArticole();

