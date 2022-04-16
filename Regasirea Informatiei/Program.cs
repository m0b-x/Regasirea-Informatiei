// See https://aka.ms/new-console-template for more information

using Regasirea_Informatiei;

Articol articol = new Articol("3665NEWS.XML");
Articol articol1 = new Articol("2504NEWS.XML");
Articol articol2 = new Articol("2538NEWS.XML");

Articol.ScrieArticoleInFiserGlobal();
List<Articol> articole = new List<Articol>();
articole.Add(articol);
articole.Add(articol1);
articole.Add(articol2);

Interogator interogator = new Interogator(ref Articol.DocumentScriereGlobal);
interogator.InterogheazaArticole(articole);