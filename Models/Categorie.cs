using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shopoo.Models
{
    public class Categorie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le Libellé de la catégorie est requis !")]
        public string Libelle { get; set; }

    }
}