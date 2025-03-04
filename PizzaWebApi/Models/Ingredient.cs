namespace PizzaWebApi.Models
{
    // SEZIONE COMPLETA (50%)
    // ========== CLASSE INGREDIENTE ==========
    // Questa classe rappresenta un ingrediente che può essere utilizzato nelle pizze
    // Per esempio: "Mozzarella", "Pomodoro", "Basilico", "Prosciutto"
    public class Ingredient
    {
        // Identificativo unico dell'ingrediente nel database
        // Viene assegnato automaticamente quando creiamo un nuovo ingrediente
        public int Id { get; set; }

        // Il nome dell'ingrediente, ad esempio "Mozzarella"
        public string Name { get; set; }
    }

    // QUIZ (50%): Quali costruttori sono necessari per la classe Ingredient?
    // Obiettivo: Implementare i costruttori necessari per creare ingredienti
    // Processo logico:
    // 1. Considera quando serve creare un ingrediente vuoto
    // 2. Considera quando serve creare un ingrediente con tutti i dati

    /* SCEGLI TRA:
    A)
    public Ingredient(string name) 
    {
        Name = name;
    }

    B)
    public Ingredient() { }
    public Ingredient(int id, string name)
    {
        Id = id;
        Name = name;
    }

    C)
    public Ingredient(int id)
    {
        Id = id;
    }
    */
}