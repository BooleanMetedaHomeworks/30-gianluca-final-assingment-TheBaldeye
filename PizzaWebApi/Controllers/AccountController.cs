using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Models;
using PizzaWebApi.Services;

namespace PizzaWebApi.Controllers
{
    // SEZIONE COMPLETA (30%)
    // Questo controller gestisce l'autenticazione e la registrazione degli utenti
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        // Servizi necessari per l'autenticazione e la gestione utenti
        private readonly JwtAuthenticationService _jwt;
        private readonly UserService _userService;

        // Il costruttore riceve i servizi attraverso dependency injection
        public AccountController(JwtAuthenticationService jwt,
            UserService userService)
        {
            this._jwt = jwt;
            _userService = userService;
        }

        // Endpoint che gestisce la registrazione di un nuovo utente
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserModel user)
        {
            var result = await _userService.RegisterAsync(user);
            if (!result)
            {
                return BadRequest(new { Message = "Registrazione fallita!" });
            }

            return Ok(new { Message = "Registrazione avvenuta con successo!" });
        }

        // QUIZ 1 (25%): Come implementeresti l'endpoint di login?
        // Obiettivo: Creare un endpoint che autentica l'utente e restituisce un token JWT
        // Processo logico:
        // 1. Ricevi email e password
        // 2. Verifica le credenziali e genera un token
        // 3. Restituisci il token con la sua scadenza

        /*SCEGLI TRA:
        A)
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserModel user)
        {
            var token = await _jwt.Authenticate(user.Email, user.Password);
            return Ok(new { Token = token });
        }*/

        //B)
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserModel user)
        {
            var token = await _jwt.Authenticate(user.Email, user.Password);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(new
            {
                Token = token,
                ExpirationUtc = DateTime.UtcNow.AddMinutes(_jwt.Settings.DurationInMinutes)
            });
        }

        /*C)
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserModel user)
        {
            if (user.Email == "admin" && user.Password == "admin")
                return Ok(new { Token = "token123" });
            return Unauthorized();
        }
        

        // QUIZ 2 (25%): Come gestiresti la verifica del token?
        // Obiettivo: Creare un endpoint che verifica se il token è valido
        // Processo logico:
        // 1. L'endpoint deve essere protetto (solo utenti autenticati)
        // 2. Deve verificare l'identità dell'utente
        // 3. Deve restituire informazioni sull'utente corrente*/

        /* SCEGLI TRA:
        A)
        [HttpGet("Verify")]
        public IActionResult Verify()
        {
            return Ok("Token valido");
        }*/

        //B)
        [HttpGet("Verify")]
        [Authorize]
        public IActionResult Verify()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return Ok(new { 
                IsAuthenticated = identity.IsAuthenticated,
                Username = identity.Name
            });
        }

        /*C)
        [HttpGet("Verify")]
        public async Task<IActionResult> Verify([FromHeader] string token)
        {
            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            return Ok();
        }*/


        // SEZIONE DA COMPLETARE (20%)
        // Obiettivo: Implementare l'endpoint di logout
        // Tips:
        // 1. Il metodo deve essere decorato con [HttpPost("Logout")]
        // 2. L'utente deve essere autenticato [Authorize]
        // 3. Non è necessario invalidare il token (scadrà automaticamente)
        // 4. Restituisci un messaggio di successo

        // Il tuo codice qui...
        [HttpPost("Logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout effettuato con successo!" });
        }  
    }
}