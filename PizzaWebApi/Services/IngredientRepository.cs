using System.Data.SqlClient;
using PizzaWebApi.Models;

namespace PizzaWebApi
{
    public class IngredientRepository
    {
        // SEZIONE COMPLETA (30%)
        // ========== CONFIGURAZIONE E METODI BASE ==========

        // Stringa di connessione al database SQL Server
        // Questa stringa contiene tutte le informazioni per connettersi al database:
        // - Data Source: indica il server (localhost = questo computer)
        // - Initial Catalog: il nome del database a cui connettersi
        // - Integrated Security: usa l'autenticazione Windows
        public const string CONNECTION_STRING = "Data Source=localhost;Initial Catalog=PizzaDB;Integrated Security=True;";

        // Metodo che recupera tutti gli ingredienti dal database
        // È un metodo asincrono (async) che restituisce una Task<List<Ingredient>>
        public async Task<List<Ingredient>> GetAllIngredients()
        {
            // Query SQL semplice che seleziona tutti i record dalla tabella Ingredients
            var query = @"SELECT * FROM Ingredients";

            // Crea e apre una connessione al database
            // using assicura che la connessione venga chiusa anche in caso di errori
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            // Lista che conterrà tutti gli ingredienti trovati
            List<Ingredient> Ingredients = new List<Ingredient>();

            // Crea e esegue il comando SQL
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                // Legge i risultati riga per riga
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // Per ogni riga, crea un oggetto Ingredient e lo aggiunge alla lista
                        Ingredients.Add(GetIngredientFromData(reader));
                    }
                }
            }

            return Ingredients;
        }

        // QUIZ 1 (25%): Come implementeresti il metodo per recuperare un ingrediente per ID?
        // Obiettivo: Creare una query che cerca un ingrediente specifico
        // Processo logico:
        // 1. La query deve cercare un ingrediente per ID
        // 2. Deve usare parametri SQL per sicurezza
        // 3. Deve gestire il caso in cui l'ingrediente non esiste

        /*SCEGLI TRA:
        A)
        public async Task<Ingredient> GetIngredientById(int id)
        {
            var query = "SELECT * FROM Ingredients WHERE Id = " + id;
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return GetIngredientFromData(reader);
                }
            }
            return null;
        }*/

        //B)
        public async Task<Ingredient> GetIngredientById(int id)
        {
            var query = @"SELECT TOP 1 * FROM Ingredients WHERE Id = @id";
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", id));
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return GetIngredientFromData(reader);
                    }
                }
            }
            return null;
        }

        /*C)
        public async Task<Ingredient> GetIngredientById(int id)
        {
            return (await GetAllIngredients()).FirstOrDefault(i => i.Id == id);
        }
        */

        // QUIZ 2 (25%): Come implementeresti l'aggiornamento di un ingrediente?
        // Obiettivo: Aggiornare i dati di un ingrediente esistente
        // Processo logico:
        // 1. Costruire la query UPDATE corretta
        // 2. Usare parametri SQL per i valori
        // 3. Restituire il numero di righe modificate

        /* SCEGLI TRA:
        A)
        public async Task<int> UpdateIngredient(int id, Ingredient ingredient)
        {
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            var query = "UPDATE Ingredients SET Name = '" + ingredient.Name + "' WHERE Id = " + id;
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        B)
        public async Task<int> UpdateIngredient(int id, Ingredient ingredient)
        {
            ingredient.Id = id;
            var ingredients = await GetAllIngredients();
            var index = ingredients.FindIndex(i => i.Id == id);
            if (index != -1)
                ingredients[index] = ingredient;
            return 1;
        }*/

        //C)
        public async Task<int> UpdateIngredient(int id, Ingredient ingredient)
        {
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            var query = $"UPDATE Ingredients SET Name = @name WHERE Id = @id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", id));
                cmd.Parameters.Add(new SqlParameter("@name", ingredient.Name));
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        // SEZIONE DA COMPLETARE (20%)
        // Obiettivo: Implementare l'eliminazione di un ingrediente e le sue relazioni
        // Tips:
        // 1. Prima di eliminare l'ingrediente, devi eliminare i suoi riferimenti nella tabella PostIngredient
        // 2. Usa il metodo ClearPostIngredients (già implementato) per pulire le relazioni
        // 3. Dopo aver pulito le relazioni, elimina l'ingrediente
        // 4. Usa parametri SQL per sicurezza
        // 5. Ricorda di gestire la connessione con using

        // Il tuo codice qui...
        public async Task<bool> DeleteIngredient(int id)
        {
            var clearedRelations = await ClearPostIngredients(id);

            if (clearedRelations > 0)
            {
                using var conn = new SqlConnection(CONNECTION_STRING);
                await conn.OpenAsync();

                var query = "DELETE FROM Ingredients WHERE Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var rowsAffected = await cmd.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
            else
            {
                return false;
            }
        }

        // Metodo helper che converte una riga del database in un oggetto Ingredient
        private Ingredient GetIngredientFromData(SqlDataReader reader)
        {
            var id = reader.GetInt32(reader.GetOrdinal("id"));
            var name = reader.GetString(reader.GetOrdinal("Name"));
            var ingredient = new Ingredient(id, name);
            return ingredient;
        }

        // Metodo helper che rimuove tutte le relazioni di un ingrediente
        private async Task<int> ClearPostIngredients(int id)
        {
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            var query = $"DELETE FROM PostIngredient WHERE IngredientId = @id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", id));
                return await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}