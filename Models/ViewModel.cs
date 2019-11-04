using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetAsp.Models
{
    public class ViewModel
    {
        public Client ClientListe { get; set; }
        public Commande CommandeListe { get; set; }
        public Produit ProduitListe { get; set; }
    }
    public class ChiffreAffaire
    {

        public string num { get; set; }
        public string nom { get; set; }
        public decimal? ChiffreAF { get; set; }

        public decimal? percent
        {
            get
            {
                return ChiffreAF * 100;
            }
        }

    }
    public class Etat_Stock
    {

        public string Client { get; set; }
        public int? QTE { get; set; }
        public System.DateTime? date_vente { get; set; }

    }
    public class facture
    {

        public string numProduit { get; set; }
        public string Categorie { get; set; }
        public decimal? pu { get; set; }
        public int? qte { get; set; }
        public decimal? montant { get; set; }

    }
}