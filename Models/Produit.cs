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
    
    public partial class Produit
    {
        public string NumProduit { get; set; }
        public string Desing { get; set; }
        public Nullable<decimal> PU { get; set; }
        public Nullable<int> stock { get; set; }
        
        public virtual ICollection<Commande> Commande { get; set; }
    }
}