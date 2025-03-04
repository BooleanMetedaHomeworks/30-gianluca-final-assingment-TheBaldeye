// ===== DEFINIZIONE DELL'INTERFACCIA DI LOGGING =====

namespace PizzaWebApi.Services
{
    // Un'interfaccia è un contratto che definisce quali metodi una classe deve implementare
    // Le interfacce sono fondamentali nei framework moderni perché permettono di:
    //  - Separare "cosa" deve fare un componente da "come" lo fa
    //  - Implementare l'iniezione delle dipendenze, un pattern che rende il codice più modulare
    //  - Facilitare i test automatici creando versioni "finte" (mock) dei servizi
    //
    // In questo caso, definiamo un servizio di logging personalizzato che qualsiasi
    // parte dell'applicazione potrà utilizzare senza preoccuparsi di come viene
    // effettivamente implementato il logging
    public interface ICustomLogger
    {
        // Questo metodo deve essere implementato da qualsiasi classe che implementa ICustomLogger
        // - message: il messaggio da registrare nel log
        // - caller: il nome del componente o metodo che sta effettuando il log
        //
        // Notare come l'interfaccia definisce SOLO la firma del metodo (parametri e tipo di ritorno)
        // ma non specifica come il metodo dovrà funzionare
        void WriteLog(string message, string caller);
    }
}