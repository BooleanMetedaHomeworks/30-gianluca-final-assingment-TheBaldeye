using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PizzaWebApi.Models;
using PizzaWebApi.Services;
using System.Text;

namespace PizzaWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Creazione del builder dell'applicazione - questo è il punto di partenza
            // che ci permette di configurare tutti i servizi e le impostazioni
            var builder = WebApplication.CreateBuilder(args);

            // ===== CONFIGURAZIONE DEI SERVIZI =====

            // Aggiunge il supporto per i controller API - questo permette all'applicazione
            // di gestire le richieste HTTP attraverso i controller che creeremo
            builder.Services.AddControllers();

            // Configurazione del sistema di autenticazione JWT
            // JWT (JSON Web Token) è un sistema che permette agli utenti di autenticarsi
            // e mantenere la loro sessione attraverso un token
            builder.Services.AddAuthentication(x =>
            {
                // Specifica che useremo JWT come schema di autenticazione predefinito
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                // Configurazione delle opzioni per la validazione dei token JWT
                x.RequireHttpsMetadata = false;  // Non richiede HTTPS in sviluppo
                x.SaveToken = true;              // Salva il token per uso futuro
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    // Configura come il token deve essere validato
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(
                                builder.Configuration.GetSection("JwtSettings")
                                                    .Get<JwtSettings>().Key)),
                    ValidateIssuer = false,      // Non validiamo l'emittente
                    ValidateAudience = false     // Non validiamo il destinatario
                };
            });

            // Configurazione di Swagger - uno strumento che genera documentazione
            // automatica per le nostre API e permette di testarle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ===== REGISTRAZIONE DEI SERVIZI =====
            // Qui registriamo tutti i servizi che la nostra applicazione userà
            // AddScoped significa che viene creata una nuova istanza per ogni richiesta HTTP

            // Servizi di logging personalizzato
            builder.Services.AddScoped<ICustomLogger, CustomLogger>();

            // Repository per l'accesso ai dati
            builder.Services.AddScoped<PizzaRepository>();
            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddScoped<IngredientRepository>();

            // Servizi per l'autenticazione e la gestione degli utenti
            builder.Services.AddScoped<IPasswordHasher<UserModel>,
                                         PasswordHasher<UserModel>>();
            builder.Services.AddScoped<JwtAuthenticationService>();
            builder.Services.AddScoped<UserService>();

            // ===== COSTRUZIONE E CONFIGURAZIONE DELL'APPLICAZIONE =====

            // Costruisce l'applicazione con tutte le configurazioni fatte sopra
            var app = builder.Build();

            // ===== CONFIGURAZIONE DEL PIPELINE HTTP =====
            // Definisce come le richieste HTTP vengono processate

            // In ambiente di sviluppo, abilita Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Forza l'uso di HTTPS
            app.UseHttpsRedirection();

            // Aggiunge il middleware di autenticazione e autorizzazione
            // Questi controllano che l'utente sia autenticato e abbia i permessi necessari
            app.UseAuthentication();
            app.UseAuthorization();

            // Configura il routing per i controller
            app.MapControllers();

            // Avvia l'applicazione
            app.Run();
        }
    }
}