using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shopoo.Models
{
    public class Panier
    {
        [Key]
        public int Id { get; set; }

        public virtual ICollection<Produit> Produits { get; set; }

        public virtual int IdUtilisateur { get; set; }

    }
}