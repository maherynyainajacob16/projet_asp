using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetAsp.Models;

namespace ProjetAsp.Controllers
{
    public class HomeController : Controller
    {
        DataCommEntities4 db =new DataCommEntities4();
        public ActionResult Index()
        {
            
            return View();
        }


        public ActionResult liste_commande()
        {

            return View(db.Commande.ToList());
        }

        [HttpPost]
        public ActionResult liste_commande(DateTime date1, DateTime date2)
        {

            return View(db.Commande.Where(x => x.DateCommande >= date1 && x.DateCommande <= date2).ToList());
        }


        public ActionResult Bilan()
        {
            using (DataCommEntities4 db = new DataCommEntities4())
            {
                DateTime stD = Convert.ToDateTime("01/01/0009");
                DateTime stF = Convert.ToDateTime("31/12/0009");
                var CHIFFRE = (
                    from com in db.Commande
                    join clie in db.Client on com.NumClient equals clie.NumClient
                    join pro in db.Produit on com.NumProduit equals pro.NumProduit
                    where com.DateCommande >= stD && com.DateCommande <= stF
                    select new ChiffreAffaire
                    {
                        num = clie.NumClient,
                        nom = clie.NomClient,
                        ChiffreAF = (com.Qte * pro.PU)
                    }
                ).ToList();
                return View(CHIFFRE);

            }

        }
        [HttpPost]
        public ActionResult Bilan(int an)
        {
            using (DataCommEntities4 db = new DataCommEntities4())
            {
                DateTime stD = Convert.ToDateTime("01/01/" + an);
                DateTime stF = Convert.ToDateTime("31/12/" + an);
                var CHIFFRE = (
                    from com in db.Commande
                    join clie in db.Client on com.NumClient equals clie.NumClient
                    join pro in db.Produit on com.NumProduit equals pro.NumProduit
                    where com.DateCommande >= stD && com.DateCommande <= stF
                    select new ChiffreAffaire
                    {
                        num = clie.NumClient,
                        nom = clie.NomClient,
                        ChiffreAF = (com.Qte * pro.PU)
                    }
                ).ToList();
                return View(CHIFFRE);

            }
        }


        public ActionResult Facture()
        {
            ViewBag.Total = "";
            var DATA = (
                    from com in db.Commande
                    join clie in db.Client on com.NumClient equals clie.NumClient
                    join pro in db.Produit on com.NumProduit equals pro.NumProduit
                    where clie.NumClient == ""
                    select new facture
                    {
                        numProduit = com.NumProduit,
                        Categorie = pro.Desing,
                        qte = com.Qte,
                        pu = pro.PU,
                        montant = (com.Qte * pro.PU)
                    }
                ).Take(10);

            var label = db.Client.Where(x => x.NumClient == "").ToList();
            foreach (var item in label)
            {
                ViewBag.nom = item.NomClient;
            };
            return View(DATA);
        }

        [HttpPost]
        public ActionResult Facture(string num_client)
        {

            decimal? total = 0;
            var DATA = (
                    from com in db.Commande
                    join clie in db.Client on com.NumClient equals clie.NumClient
                    join pro in db.Produit
                        on com.NumProduit equals pro.NumProduit
                    where clie.NumClient == num_client
                    select new facture
                    {
                        numProduit = pro.NumProduit,
                        Categorie = pro.Desing,
                        qte = com.Qte,
                        pu = pro.PU,
                        montant = (com.Qte * pro.PU),
                    }

                ).Take(10);
            foreach (var item in DATA)
            {
                total = DATA.Sum(m => m.montant);
                ViewBag.lettre = converti(Convert.ToDouble(total));
            };
            var label = db.Client.Where(x => x.NumClient == num_client).ToList();
            foreach (var item in label)
            {
                ViewBag.nom = item.NomClient;
            };
            return View(DATA);
        }


        public ActionResult Etat_Stock()
        {
            var DATA = (
                      from com in db.Commande
                      join clie in db.Client on com.NumClient equals clie.NumClient
                      join pro in db.Produit on com.NumProduit equals pro.NumProduit
                      where pro.NumProduit == ""
                      select new Etat_Stock
                      {
                          Client = clie.NomClient,
                          QTE = com.Qte,
                          date_vente = com.DateCommande
                      }
                  ).Take(10);

            var label = db.Produit.Where(x => x.NumProduit == "").ToList();
            foreach (var item in label)
            {
                ViewBag.Categorie = item.Desing;
                ViewBag.etat = item.stock;
            };
            return View(DATA);
        }
        [HttpPost]
        public ActionResult Etat_Stock(string num, DateTime date1, DateTime date2)
        {

            var DATA = (
                    from com in db.Commande
                    join clie in db.Client on com.NumClient equals clie.NumClient
                    join pro in db.Produit
                        on com.NumProduit equals pro.NumProduit
                    where pro.NumProduit == num && com.DateCommande >= date1 && com.DateCommande <= date2
                    select new Etat_Stock
                    {
                        Client = clie.NomClient,
                        QTE = com.Qte,
                        date_vente = com.DateCommande
                    }
                ).Take(10);

            var label = db.Produit.Where(x => x.NumProduit == num).ToList();
            foreach (var item in label)
            {
                ViewBag.Categorie = item.Desing;
                ViewBag.etat = item.stock;
            };
            return View(DATA);
        }


        public string converti(double chiffre)
        {
            int centaine, dizaine, unite, reste, y;
            bool dix = false;
            bool soixanteDix = false;
            string lettre = "";
            //strcpy(lettre, "");

            reste = (int)chiffre / 1;

            for (int i = 1000000000; i >= 1; i /= 1000)
            {
                y = reste / i;
                if (y != 0)
                {
                    centaine = y / 100;
                    dizaine = (y - centaine * 100) / 10;
                    unite = y - (centaine * 100) - (dizaine * 10);
                    switch (centaine)
                    {
                        case 0:
                            break;
                        case 1:
                            lettre += "cent ";
                            break;
                        case 2:
                            if ((dizaine == 0) && (unite == 0)) lettre += "deux cents ";
                            else lettre += "deux cent ";
                            break;
                        case 3:
                            if ((dizaine == 0) && (unite == 0)) lettre += "trois cents ";
                            else lettre += "trois cent ";
                            break;
                        case 4:
                            if ((dizaine == 0) && (unite == 0)) lettre += "quatre cents ";
                            else lettre += "quatre cent ";
                            break;
                        case 5:
                            if ((dizaine == 0) && (unite == 0)) lettre += "cinq cents ";
                            else lettre += "cinq cent ";
                            break;
                        case 6:
                            if ((dizaine == 0) && (unite == 0)) lettre += "six cents ";
                            else lettre += "six cent ";
                            break;
                        case 7:
                            if ((dizaine == 0) && (unite == 0)) lettre += "sept cents ";
                            else lettre += "sept cent ";
                            break;
                        case 8:
                            if ((dizaine == 0) && (unite == 0)) lettre += "huit cents ";
                            else lettre += "huit cent ";
                            break;
                        case 9:
                            if ((dizaine == 0) && (unite == 0)) lettre += "neuf cents ";
                            else lettre += "neuf cent ";
                            break;
                    }// endSwitch(centaine)

                    switch (dizaine)
                    {
                        case 0:
                            break;
                        case 1:
                            dix = true;
                            break;
                        case 2:
                            lettre += "vingt ";
                            break;
                        case 3:
                            lettre += "trente ";
                            break;
                        case 4:
                            lettre += "quarante ";
                            break;
                        case 5:
                            lettre += "cinquante ";
                            break;
                        case 6:
                            lettre += "soixante ";
                            break;
                        case 7:
                            dix = true;
                            soixanteDix = true;
                            lettre += "soixante ";
                            break;
                        case 8:
                            lettre += "quatre-vingt ";
                            break;
                        case 9:
                            dix = true;
                            lettre += "quatre-vingt ";
                            break;
                    } // endSwitch(dizaine)

                    switch (unite)
                    {
                        case 0:
                            if (dix) lettre += "dix ";
                            break;
                        case 1:
                            if (soixanteDix) lettre += "et onze ";
                            else
                                if (dix) lettre += "onze ";
                                else if ((dizaine != 1 && dizaine != 0)) lettre += "et un ";
                                else lettre += "un ";
                            break;
                        case 2:
                            if (dix) lettre += "douze ";
                            else lettre += "deux ";
                            break;
                        case 3:
                            if (dix) lettre += "treize ";
                            else lettre += "trois ";
                            break;
                        case 4:
                            if (dix) lettre += "quatorze ";
                            else lettre += "quatre ";
                            break;
                        case 5:
                            if (dix) lettre += "quinze ";
                            else lettre += "cinq ";
                            break;
                        case 6:
                            if (dix) lettre += "seize ";
                            else lettre += "six ";
                            break;
                        case 7:
                            if (dix) lettre += "dix-sept ";
                            else lettre += "sept ";
                            break;
                        case 8:
                            if (dix) lettre += "dix-huit ";
                            else lettre += "huit ";
                            break;
                        case 9:
                            if (dix) lettre += "dix-neuf ";
                            else lettre += "neuf ";
                            break;
                    } // endSwitch(unite)

                    switch (i)
                    {
                        case 1000000000:
                            if (y > 1) lettre += "milliards ";
                            else lettre += "milliard ";
                            break;
                        case 1000000:
                            if (y > 1) lettre += "millions ";
                            else lettre += "million ";
                            break;
                        case 1000:
                            lettre += "mille ";
                            break;
                    }
                } // end if(y!=0)
                reste -= y * i;
                dix = false;
                soixanteDix = false;
            } // end for
            if (lettre.Length == 0) lettre += "zero";

            // pour les chiffres apres la virgule :
            Decimal chiffre3;
            chiffre3 = (Decimal)(chiffre * 100) % 100;
            Console.WriteLine(chiffre3);

            //int chiffre2 = (int)((chiffre - (int)(chiffre/1))*100);
            //Console.WriteLine(chiffre2);
            dizaine = (int)(chiffre3) / 10;
            unite = (int)chiffre3 - (dizaine * 10);

            string lettre2 = "";
            switch (dizaine)
            {
                case 0:
                    break;
                case 1:
                    dix = true;
                    break;
                case 2:
                    lettre2 += "vingt ";
                    break;
                case 3:
                    lettre2 += "trente ";
                    break;
                case 4:
                    lettre2 += "quarante ";
                    break;
                case 5:
                    lettre2 += "cinquante ";
                    break;
                case 6:
                    lettre2 += "soixante ";
                    break;
                case 7:
                    dix = true;
                    soixanteDix = true;
                    lettre2 += "soixante ";
                    break;
                case 8:
                    lettre2 += "quatre-vingt ";
                    break;
                case 9:
                    dix = true;
                    lettre2 += "quatre-vingt ";
                    break;
            } // endSwitch(dizaine)

            switch (unite)
            {
                case 0:
                    if (dix) lettre2 += "dix ";
                    break;
                case 1:
                    if (soixanteDix) lettre2 += "et onze ";
                    else
                        if (dix) lettre2 += "onze ";
                        else if ((dizaine != 1 && dizaine != 0)) lettre2 += "et un ";
                        else lettre2 += "un ";
                    break;
                case 2:
                    if (dix) lettre2 += "douze ";
                    else lettre2 += "deux ";
                    break;
                case 3:
                    if (dix) lettre2 += "treize ";
                    else lettre2 += "trois ";
                    break;
                case 4:
                    if (dix) lettre2 += "quatorze ";
                    else lettre2 += "quatre ";
                    break;
                case 5:
                    if (dix) lettre2 += "quinze ";
                    else lettre2 += "cinq ";
                    break;
                case 6:
                    if (dix) lettre2 += "seize ";
                    else lettre2 += "six ";
                    break;
                case 7:
                    if (dix) lettre2 += "dix-sept ";
                    else lettre2 += "sept ";
                    break;
                case 8:
                    if (dix) lettre2 += "dix-huit ";
                    else lettre2 += "huit ";
                    break;
                case 9:
                    if (dix) lettre2 += "dix-neuf ";
                    else lettre2 += "neuf ";
                    break;
            }

            // on enleve le un devant le mille :
            if (lettre.StartsWith("un mille"))
            {
                //Console.WriteLine("on enleve le un devant le mille");
                lettre = lettre.Remove(0, 3);
            }

            if (lettre2.Equals(""))
                return lettre + "Ariary";
            else if (dizaine.Equals(0) && unite.Equals(1))
                return lettre + "virgule " + lettre2 + "Ariary";
            else
                return lettre + "virgule " + lettre2 + "Ariary";
        }
    }


}