using System.Data.SqlClient;
using PizzaWebApi.Models;

namespace PizzaWebApi
{
    public class CategoryRepository
    {
        // SEZIONE COMPLETA (30%)
        // Questa classe si occupa di tutte le operazioni di database relative alle categorie

        // Repository delle pizze necessario per gestire le relazioni
        private readonly PizzaRepository _pizzaRepo;

        // Costruttore che riceve le dipendenze necessarie
        public CategoryRepository(PizzaRepository pizzaRepo)
        {
            _pizzaRepo = pizzaRepo;
        }

        // Stringa di connessione al database - contiene le informazioni per connettersi al DB
        public const string CONNECTION_STRING = "Data Source=localhost;Initial Catalog=PizzaDB;Integrated Security=True;";

        // Metodo che recupera tutte le categorie dal database
        public async Task<List<Category>> GetAllCategories()
        {
            // Query SQL per selezionare tutte le categorie
            var query = @"SELECT * FROM Categories";

            // Creazione della connessione al database
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            List<Category> Categories = new List<Category>();

            // Esecuzione della query e lettura dei risultati
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Categories.Add(GetCategoryFromData(reader));
                    }
                }
            }

            return Categories;
        }

        // QUIZ 1 (25%): Come implementeresti la ricerca di categorie per nome?
        // Obiettivo: Creare una query che filtra le categorie per nome
        // Processo logico:
        // 1. La query deve usare il parametro name nella clausola WHERE
        // 2. Devi usare parametri SQL per prevenire SQL injection
        // 3. La struttura è simile a GetAllCategories ma con filtro

        /* SCEGLI TRA:
        A)
        public async Task<List<Category>> GetCategoriesByName(string name)
        {
            var query = "SELECT * FROM Categories WHERE Name LIKE '%" + name + "%'";
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            // ... resto del codice
        }*/

        //B)
        public async Task<List<Category>> GetCategoriesByName(string name)
        {
            var query = @"SELECT * FROM Categories WHERE Name=@name";
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            List<Category> Categories = new List<Category>();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@name", name));
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Categories.Add(GetCategoryFromData(reader));
                    }
                }
            }
            return Categories;
        }

        /*C)
        public List<Category> GetCategoriesByName(string name)
        {
            return GetAllCategories().Result.Where(c => c.Name == name).ToList();
        }
        */

        // QUIZ 2 (25%): Implementazione dell'inserimento di una categoria
        // Obiettivo: Creare una query INSERT per aggiungere una nuova categoria
        // Processo logico:
        // 1. Costruire la query INSERT
        // 2. Usare parametri SQL per i valori
        // 3. Eseguire la query e restituire il numero di righe modificate

        /* SCEGLI TRA:
        A)
        public async Task<int> InsertCategory(Category Category)
        {
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            var query = $"INSERT INTO Categories (Name) VALUES ('{Category.Name}')";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                return await cmd.ExecuteNonQueryAsync();
            }
        }*/

        //B)
        public async Task<int> InsertCategory(Category Category)
        {
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            var query = $"INSERT INTO Categories (Name) VALUES (@name)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@name", Category.Name));
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        /*C)
        public async Task<int> InsertCategory(Category Category)
        {
            var categories = await GetAllCategories();
            categories.Add(Category);
            return 1;
        }
        */

        // SEZIONE DA COMPLETARE (20%)
        // Obiettivo: Implementare il metodo GetCategoryFromData che converte i dati dal database in un oggetto Category
        // Tips:
        // 1. Il metodo riceve un SqlDataReader come parametro
        // 2. Devi leggere l'ID e il Nome dalla riga del database
        // 3. Usa il metodo GetOrdinal per trovare l'indice delle colonne
        // 4. Crea e restituisci un nuovo oggetto Category con i dati letti

        // Il tuo codice qui...
        private Category GetCategoryFromData(SqlDataReader reader)
        {
            int id = reader.GetInt32(reader.GetOrdinal("id"));
            string name = reader.GetString(reader.GetOrdinal("name"));
            var Category = new Category(id, name);
            return Category;
        }

        //Task aggiuntivi per risolvere errori nel codice

        //Aggiunto Get per l'Id Categoria
        public async Task<Category> GetCategoryById(int id)
        {
            var query = @"SELECT TOP 1 * FROM Categories WHERE Id = @id";
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", id));
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return GetCategoryFromData(reader);
                    }
                }
            }
            return null;
        }

        //Aggiunta Delete per  Id Categoria
        public async Task<bool> DeleteCategoryById(int id)
        {
            await _pizzaRepo.ClearCategoryRelations(id);
            
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            var query = "DELETE FROM Categories WHERE Id = @id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", id));

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }
    }
}