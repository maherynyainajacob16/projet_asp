//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjetAsp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Commande
    {
        public string NumClient { get; set; }
        public string NumProduit { get; set; }
        public Nullable<int> Qte { get; set; }
        public Nullable<System.DateTime> DateCommande { get; set; }
        public int IdCommande { get; set; }

        public virtual Client Client { get; set; }
        public virtual Produit Produit { get; set; }
    }
}
