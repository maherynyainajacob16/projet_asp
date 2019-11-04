using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjetAsp.Models;

namespace ProjetAsp.Controllers
{
    public class CommandesController : Controller
    {
        private DataCommEntities4 db = new DataCommEntities4();

        // GET: Commandes
        public ActionResult Index()
        {
            return View( db.Commande.ToList());
        }

        // GET: Commandes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commande commande = await db.Commande.FindAsync(id);
            if (commande == null)
            {
                return HttpNotFound();
            }
            return View(commande);
        }


        // GET: Commandes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commande commande = await db.Commande.FindAsync(id);
            if (commande == null)
            {
                return HttpNotFound();
            }
            return View(commande);
        }

        // POST: Commandes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "NumClient,NumProduit,Qte,DateCommande,IdCommande")] Commande commande)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commande).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(commande);
        }

        // GET: Commandes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Commandes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Commande commande)
        {
            if (!(IsFindClient(commande.NumClient) && IsFindProd(commande.NumProduit))) {
                return View();
            }

            else { 
                int? QTE; int? QT;
                int? qte = commande.Qte;
                var label = db.Produit.Where(x => x.NumProduit ==commande.NumProduit).ToList();
                foreach (var item in label)
                {
                    QTE = item.stock;
                    QT = QTE - qte;
                    UpdateQTE(QT,commande.NumProduit);
                };

                try
                {
                    // TODO: Add insert logic here
                    db.Commande.Add(commande);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }
        }

        public bool IsFindClient(string num)
        {
            var numC = db.Client.ToList();
            foreach (var i in numC)
            {
                if (i.NumClient.Equals(num)) { return true; };
            };

            ViewBag.notFindID = "Le numéro du Client que vous avez entré n'existe pas";
            return false;
        }

        public bool IsFindProd(string num)
        {
            var numP = db.Produit.ToList();
            foreach (var i in numP)
            {
                if (i.NumProduit.Equals(num)) { return true; };
            };
            ViewBag.notFindID = "Le numéro du produit que vous avez entré n'existe pas";
            return false;
        }

        public void UpdateQTEVente(int Id, int? qte)
        {
            int? QTE; int? QT; int? QTvente; string num_V;
            var VenteQT = db.Commande.Where(x => x.IdCommande == Id).ToList();
            foreach (var ite in VenteQT)
            {
                QTvente = ite.Qte;
                num_V = ite.NumProduit;
                var label = db.Produit.Where(x => x.NumProduit == num_V).ToList();
                foreach (var item in label)
                {
                    QTE = item.stock;
                    QT = QTE + QTvente;
                    QT = QT - qte;
                    UpdateQTE(QT, num_V);
                };
            };
        }

        public void RestoreQTE(int Id)
        {
            int? QTE; int? QT; int? QTCom; string num_pro;
            var comQT = db.Commande.Where(x => x.IdCommande == Id).ToList();
            foreach (var ite in comQT)
            {
                QTCom = ite.Qte;
                num_pro = ite.NumProduit;
                var label = db.Produit.Where(x => x.NumProduit == num_pro).ToList();
                foreach (var item in label)
                {
                    QTE = item.stock;
                    QT = QTE + QTCom;
                    UpdateQTE(QT, num_pro);
                };
            };
        }

        public void UpdateQTE(int? q, string num)
        {
            var produit = new Produit() { NumProduit = num, stock = q };
            using (var db = new DataCommEntities4())
            {
                db.Produit.Attach(produit);
                db.Entry(produit).Property(x => x.stock).IsModified = true;
                db.SaveChanges();
            }
        }



        [HttpGet]
        public ActionResult Delete(int Id)
        {
            return View(db.Commande.Where(x => x.IdCommande == Id).FirstOrDefault());

        }

        [HttpPost]
        public ActionResult Delete(int Id, Commande commande)
        {

            RestoreQTE(Id);
            try
            {
                // TODO: Add delete logic here
                Commande Cdelete = db.Commande.Where(x => x.IdCommande == Id).FirstOrDefault();
                db.Commande.Remove(Cdelete);
                db.SaveChanges();


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        
    }
}
