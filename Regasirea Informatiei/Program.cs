// See https://aka.ms/new-console-template for more information

using Regasirea_Informatiei;

var articole = new List<Articol>();


const string numeFolder = "Documente";
const string patternFisier = "*.xml";
foreach (var fisier in Directory.EnumerateFiles(numeFolder, patternFisier))
    articole.Add(new Articol(fisier.Split('/')[1]));

Articol.ScrieArticoleInFiserGlobal();
var interogator = new Interogator(ref Articol.DocumentScriereGlobal);
interogator.InterogheazaArticole(articole);