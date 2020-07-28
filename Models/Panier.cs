using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shopoo.Models
{
    public class Panier
    {
        [Key]
        public int Id { get; set; }

        public virtual ICollection<Produit> Produits { get; set; }

        public virtual Utilisateur Utilisateur { get; set; }

    }
}