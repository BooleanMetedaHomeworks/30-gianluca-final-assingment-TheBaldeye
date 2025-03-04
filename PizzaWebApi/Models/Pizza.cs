using System.ComponentModel.DataAnnotations;

namespace PizzaWebApi.Models
{
    // Questa classe rappresenta una Pizza nel nostro sistema
    // È un esempio di "Model", ovvero una classe che rappresenta i dati con cui lavoriamo
    public class Pizza
    {
        // SEZIONE COMPLETA (20%) - PROPRIETÀ BASE
        // Queste sono le proprietà fondamentali di una pizza

        // Identificativo unico della pizza nel database
        public int Id { get; set; }

        // Il Required attribute indica che questo campo è obbligatorio
        // StringLength imposta una lunghezza massima di 20 caratteri
        [Required(ErrorMessage = "Il campo è obbligatorio")]
        [StringLength(20, ErrorMessage = "Il titolo non può avere più di 20 caratteri")]
        public string Name { get; set; }

        // Descrizione della pizza
        public string Description { get; set; }

        // QUIZ 1 (25%): Quale delle seguenti è la corretta definizione della proprietà Price?
        // Obiettivo: Definire un prezzo valido per la pizza (deve essere positivo e non eccessivo)
        // Processo logico: 
        // 1. Il prezzo deve essere un numero decimale (decimal)
        // 2. Deve avere un range valido
        // 3. Deve usare l'attributo Range per la validazione

        /* SCEGLI TRA:
        A)
        public int Price { get; set; }
        
        B)
        [Range(0.1, 10000)]
        public decimal Price { get; set; }
        
        C)
        [Required]
        public double Price { get; set; }
        */

        // QUIZ 2 (25%): Come definiresti la relazione con la categoria?
        // Obiettivo: Permettere a una pizza di appartenere a una categoria (opzionale)
        // Processo logico:
        // 1. Serve un ID per la categoria (può essere null)
        // 2. Serve una proprietà per accedere all'oggetto categoria

        /* SCEGLI TRA:
        A)
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        
        B)
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        
        C)
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public Category Category { get; set; }
        */

        // SEZIONE DA COMPLETARE (30%)
        // Obiettivo: Implementare la relazione con gli ingredienti
        // Tips:
        // 1. Una pizza può avere più ingredienti
        // 2. Servono due liste: una per gli ID degli ingredienti e una per gli oggetti Ingredient
        // 3. Le liste devono essere inizializzate nel costruttore

        // Il tuo codice qui...

        // SEZIONE COMPLETA - COSTRUTTORI
        // Questi costruttori mostrano diversi modi di creare una pizza
        public Pizza()
        {
            // Inizializza le liste vuote quando viene creata una nuova pizza
            IngredientIds = new List<int>();
            Ingredients = new List<Ingredient>();
        }

        public Pizza(int id, string name, string description, decimal price)
            : this(name, description, price)  // Chiama l'altro costruttore
        {
            Id = id;
        }

        public Pizza(string name, string description, decimal price) : this()  // Chiama il costruttore base
        {
            Name = name;
            Description = description;
            Price = price;
        }
    }
}