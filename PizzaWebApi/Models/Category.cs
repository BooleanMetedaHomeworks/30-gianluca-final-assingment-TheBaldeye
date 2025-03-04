namespace PizzaWebApi.Models
{
    // SEZIONE COMPLETA (50%)
    // ========== CLASSE CATEGORIA ==========
    // Questa classe rappresenta una categoria di pizze nel nostro sistema
    // Per esempio: "Pizze Classiche", "Pizze Piccanti", "Pizze Vegetariane"
    public class Category
    {
        // Identificativo unico della categoria nel database
        // Quando salviamo una categoria nel DB, questo ID viene generato automaticamente
        public int Id { get; set; }

        // Il nome della categoria, ad esempio "Pizze Classiche"
        public string Name { get; set; }
    

    // QUIZ (50%): Come implementeresti i costruttori della classe Category?
    // Obiettivo: Fornire modi flessibili per creare categorie
    // Processo logico:
    // 1. Serve un costruttore vuoto per il framework
    // 2. Serve un costruttore per creare una categoria con tutti i dati

    /* SCEGLI TRA:
    A)
    public Category() { }

    B)
    public Category(string name) 
    {
        Name = name;
    }*/

    //C)
        public Category() { }
        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}