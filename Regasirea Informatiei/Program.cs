using Regasirea_Informatiei;

const string numeFolder = "Documente32";
const string patternFisier = "*.xml";

//Etapa 1

foreach (var pathFisier in Directory.EnumerateFiles(numeFolder, patternFisier))
{
        _ = new Document(pathFisier);
}

//Etapa 3 + scriere in fisier

Document.DocumentGlobal.SelecteazaTrasaturileRelevante();
//Document.ScrieDateleInFisiere();

//Etapa 2

var interogator = new Interogator(ref Document.DocumentGlobal);
interogator.InterogheazaDocumente();
