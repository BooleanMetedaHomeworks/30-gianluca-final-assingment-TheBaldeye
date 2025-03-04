using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PizzaWebApi.Services
{
    // SEZIONE COMPLETA (30%)
    // ========== CONFIGURAZIONE JWT ==========
    // Questa classe contiene le impostazioni per i token JWT
    public class JwtSettings
    {
        // La chiave segreta usata per firmare i token JWT
        // Questa chiave deve essere mantenuta sicura e non condivisa
        // La lunghezza e complessità della chiave influenzano la sicurezza del sistema
        public string Key { get; set; }

        // Durata di validità del token in minuti
        // Dopo questo periodo, l'utente dovrà effettuare nuovamente il login
        // Un valore comune è 60 minuti (1 ora)
        public int DurationInMinutes { get; set; }
    }

    // ========== SERVIZIO DI AUTENTICAZIONE ==========
    // Questa classe si occupa di:
    // - Leggere le impostazioni JWT dalla configurazione
    // - Autenticare gli utenti verificando le loro credenziali
    // - Generare token JWT per gli utenti autenticati
    public class JwtAuthenticationService
    {
        // _config contiene tutte le configurazioni dell'applicazione
        private readonly IConfiguration _config;

        // Settings contiene le impostazioni specifiche per JWT
        // È public readonly perché altre parti dell'applicazione
        // potrebbero aver bisogno di accedere alla durata del token
        public readonly JwtSettings Settings;

        // _userService gestisce le operazioni relative agli utenti
        // come la verifica delle credenziali e il recupero dei ruoli
        private readonly UserService _userService;

        // Il costruttore inizializza il servizio leggendo le configurazioni
        // e preparando tutti i servizi necessari per l'autenticazione
        public JwtAuthenticationService(IConfiguration config, UserService userService)
        {
            this._config = config;
            // Carica le impostazioni JWT dalla sezione "JwtSettings" della configurazione
            this.Settings = config.GetSection("JwtSettings")
                                .Get<JwtSettings>();
            _userService = userService;
        }

        // QUIZ (50%): Come implementeresti il metodo di autenticazione?
        // Obiettivo: Verificare le credenziali e generare un token JWT valido
        // Processo logico:
        // 1. Verificare email e password usando UserService
        // 2. Se le credenziali sono valide, ottenere i ruoli dell'utente
        // 3. Creare i claims (informazioni sull'utente da includere nel token)
        // 4. Generare e firmare il token JWT
        // 5. Restituire null se l'autenticazione fallisce

        /*SCEGLI TRA:
        A)
        public async Task<string> Authenticate(string email, string password)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(Settings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }),
                Expires = DateTime.UtcNow.AddMinutes(Settings.DurationInMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }*/

        //B)
        public async Task<string> Authenticate(string email, string password)
        {
            var user = await _userService.AuthenticateAsync(email, password);
            if (user == null) return null;

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, email) };
            var roles = await _userService.GetUserRolesAsync(user.Id);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(Settings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Settings.DurationInMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /*C)
        public async Task<string> Authenticate(string email, string password)
        {
            if (email == "admin" && password == "admin")
                return "token123";
            return null;
        }
        */

        // SEZIONE DA COMPLETARE (20%)
        // Obiettivo: Aggiungere logging per tracciare i tentativi di autenticazione
        // Tips:
        // 1. Usa ICustomLogger per registrare i tentativi di login
        // 2. Logga sia i tentativi riusciti che quelli falliti
        // 3. Non loggare mai le password!
        // 4. Includi informazioni utili come email e timestamp

        // Il tuo codice qui...

    }
}