using Microsoft.AspNetCore.Identity;
using PizzaWebApi.Models;
using System.Data.SqlClient;

namespace PizzaWebApi.Services
{
    public class UserService
    {
        // SEZIONE COMPLETA (30%)
        // ========== CONFIGURAZIONE DEL SERVIZIO ==========

        // Questa è la stringa che contiene tutte le informazioni per connettersi al database
        // Vediamo nel dettaglio cosa significa:
        // - Data Source=localhost -> il database è sul nostro computer
        // - Initial Catalog=PizzaDB -> il nome del database a cui ci connettiamo
        // - Integrated Security=True -> usiamo l'autenticazione di Windows
        public const string CONNECTION_STRING = "Data Source=localhost;Initial Catalog=PizzaDB;Integrated Security=True;";

        // Questo servizio si occupa di gestire le password in modo sicuro
        // PasswordHasher è una classe di ASP.NET che:
        // 1. Prende una password in chiaro
        // 2. La trasforma in una stringa cifrata (hash)
        // 3. Aggiunge un elemento di randomizzazione (salt)
        // In questo modo, anche se qualcuno accede al database, non può vedere le password originali
        private readonly IPasswordHasher<UserModel> _pswHasher;

        // Il costruttore riceve il servizio di hashing attraverso dependency injection
        public UserService(IPasswordHasher<UserModel> pswHasher)
        {
            _pswHasher = pswHasher;
        }

        // ========== REGISTRAZIONE UTENTE ==========
        // Questo metodo gestisce la registrazione di un nuovo utente
        public async Task<bool> RegisterAsync(UserModel user)
        {
            // Primo passo: creiamo l'hash della password
            // Questo trasforma la password "123456" in qualcosa come "AQAAAAEAACcQAAAAEEy7Xw6X9SPR..."
            var passwordHash = _pswHasher.HashPassword(user, user.Password);

            // Secondo passo: ci connettiamo al database
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            // Terzo passo: prepariamo la query SQL per inserire il nuovo utente
            // Nota: salviamo email e hash della password, MAI la password originale
            var query = "INSERT INTO Users (Email, PasswordHash) VALUES (@Email, @PasswordHash)";

            // Quarto passo: eseguiamo la query usando parametri per prevenire SQL injection
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                try
                {
                    cmd.Parameters.Add(new SqlParameter("@Email", user.Email));
                    cmd.Parameters.Add(new SqlParameter("@PasswordHash", passwordHash));
                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception ex) {
                    return false;
                }

                
            }
        }

        // QUIZ 1 (25%): Implementazione dell'autenticazione utente
        // Obiettivo: Verificare le credenziali dell'utente durante il login
        // Processo logico:
        // 1. Cerca l'utente nel database usando l'email
        // 2. Verifica che la password fornita corrisponda all'hash salvato
        // 3. Restituisci i dati dell'utente se l'autenticazione è riuscita

        /* SCEGLI TRA:
        A)
        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var query = "SELECT * FROM Users WHERE Email = @Email AND PasswordHash = @Password";
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@Email", email));
            cmd.Parameters.Add(new SqlParameter("@Password", password));
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return new User { Id = reader.GetInt32(0), Email = email };
            return null;
        }*/

        //B)
        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var query = "SELECT * FROM Users WHERE Email = @Email";
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@Email", email));
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var id = reader.GetInt32(reader.GetOrdinal("Id"));
                var passwordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                var model = new UserModel() { Email = email, Password = password };
                if (_pswHasher.VerifyHashedPassword(model, passwordHash, password)
                        != PasswordVerificationResult.Success)
                { return null; }
                return new User() { Id = id, Email = email };
            }
            return null;
        }

        /*C)
        public Task<User> AuthenticateAsync(string email, string password)
        {
            if (email == "admin@admin.com" && password == "admin")
                return Task.FromResult(new User { Id = 1, Email = email });
            return Task.FromResult<User>(null);
        }
        */

        // QUIZ 2 (25%): Recupero dei ruoli dell'utente
        // Obiettivo: Ottenere tutti i ruoli assegnati a un utente
        // Processo logico:
        // 1. Query per selezionare i ruoli dalla tabella di join
        // 2. Usa INNER JOIN per collegare le tabelle
        // 3. Restituisci la lista dei nomi dei ruoli

        /* SCEGLI TRA:
        A)
        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            return new[] { "USER" };
        }

        B)
        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var roles = new List<string>();
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "SELECT Name FROM Roles WHERE Id IN (SELECT RoleId FROM UserRoles WHERE UserId = @UserId)",
                    connection);
                command.Parameters.AddWithValue("@UserId", userId);
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    roles.Add(reader.GetString(0));
                }
            }
            return roles;
        }*/

        //C)
        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var roles = new List<string>();
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "SELECT r.Name FROM Roles r INNER JOIN UserRoles ur ON r.Id = ur.RoleId WHERE ur.UserId = @UserId",
                    connection);
                command.Parameters.AddWithValue("@UserId", userId);
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    roles.Add(reader.GetString(0));
                }
            }
            return roles;
        }


        // SEZIONE DA COMPLETARE (20%)
        // Obiettivo: Implementare un metodo per verificare se un'email è già registrata
        // Tips:
        // 1. Il metodo deve essere async e accettare una stringa email come parametro
        // 2. Usa una query SELECT COUNT(*) per contare gli utenti con quella email
        // 3. Usa parametri SQL per prevenire SQL injection
        // 4. Restituisci true se l'email esiste già, false altrimenti

        // Il tuo codice qui...
        public async Task<bool> EmailExistsAsync(string email)
        {
            using var conn = new SqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();
            var query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Email", email));
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
        }
    }
}