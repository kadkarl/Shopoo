using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shopoo.Models
{
    public class Commande
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateCommande { get; set; }

        public bool EstValide { get; set; }

        public virtual int IdUtilisateur { get; set; }

        public ICollection<Produit> Produits { get; set; }
    }
}