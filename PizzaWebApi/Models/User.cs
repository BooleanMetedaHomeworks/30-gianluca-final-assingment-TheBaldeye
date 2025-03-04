namespace PizzaWebApi.Models
{
    // SEZIONE COMPLETA (50%)
    // ========== MODELLI UTENTE ==========
    // In questo file abbiamo due classi correlate:
    // 1. UserModel: usato per le operazioni di login/registrazione
    // 2. User: rappresentazione dell'utente nel sistema dopo l'autenticazione

    // ===== CLASSE UserModel =====
    // Questa classe viene usata quando un utente fa login o si registra
    // Contiene solo le informazioni necessarie per l'autenticazione
    public class UserModel
    {
        // Email dell'utente - sarà usata come username per il login
        public string Email { get; set; }

        // Password dell'utente - viene usata solo durante login/registrazione
        // Non viene mai salvata direttamente nel database (viene salvato solo l'hash)
        public string Password { get; set; }
    }

    // QUIZ (50%): Come implementeresti la classe User?
    // Obiettivo: Creare la classe che rappresenta un utente autenticato
    // Processo logico:
    // 1. Servono le proprietà per Id ed Email
    // 2. Serve memorizzare l'hash della password, non la password in chiaro

    /* SCEGLI TRA:
    A)
    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }*/

    //B)
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }

    /*C)
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    */
}