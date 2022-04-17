using Regasirea_Informatiei;

var articole = new List<Articol>(7500);


const string numeFolder = "Documente32";
const string patternFisier = "*.xml";
foreach (var pathFisier in Directory.EnumerateFiles(numeFolder, patternFisier))
    articole.Add(new Articol(pathFisier));

Articol.ScrieDateInFisiereGlobale();

var interogator = new Interogator(ref Articol.DocumentGlobal);
interogator.InterogheazaArticole(articole);

