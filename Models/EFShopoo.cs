namespace Shopoo.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class EFShopoo : DbContext
    {
        // Votre contexte a été configuré pour utiliser une chaîne de connexion « EFShopoo » du fichier 
        // de configuration de votre application (App.config ou Web.config). Par défaut, cette chaîne de connexion cible 
        // la base de données « Shopoo.Models.EFShopoo » sur votre instance LocalDb. 
        // 
        // Pour cibler une autre base de données et/ou un autre fournisseur de base de données, modifiez 
        // la chaîne de connexion « EFShopoo » dans le fichier de configuration de l'application.
        public EFShopoo()
            : base("name=EFShopoo")
        {
        }

        // Ajoutez un DbSet pour chaque type d'entité à inclure dans votre modèle. Pour plus d'informations 
        // sur la configuration et l'utilisation du modèle Code First, consultez http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Utilisateur> Utilisateurs { get; set; }
        public virtual DbSet<Produit> Produits { get; set; }
        public virtual DbSet<Categorie> Categories { get; set; }
        public virtual DbSet<Panier> Paniers { get; set; }
        public virtual DbSet<Commande> Commandes { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}