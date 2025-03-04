// ===== IMPLEMENTAZIONE DEL SERVIZIO DI LOGGING =====

namespace PizzaWebApi.Services
{
    // Questa classe concreta implementa l'interfaccia ICustomLogger
    // Il ":ICustomLogger" indica che questa classe deve fornire tutte le funzionalità
    // richieste dall'interfaccia ICustomLogger
    public class CustomLogger : ICustomLogger
    {
        // Implementazione concreta del metodo WriteLog definito nell'interfaccia
        // Questa implementazione è semplice: scrive sulla console con data e ora
        //
        // In un'applicazione reale, questo metodo potrebbe:
        // - Scrivere su un file
        // - Inviare log a un database
        // - Usare servizi di logging più sofisticati come Serilog o NLog
        // - Filtrare i messaggi per livello di importanza (Debug, Info, Warning, Error)
        //
        // Il vantaggio dell'interfaccia è che possiamo cambiare questa implementazione
        // in qualsiasi momento senza dover modificare il resto dell'applicazione
        public void WriteLog(string message, string caller)
        {
            // Formato del log: [Data e Ora] [Nome del chiamante] Messaggio
            // DateTime.Now fornisce la data e l'ora corrente
            // L'operatore $ permette di inserire variabili direttamente nella stringa
            Console.WriteLine($"{DateTime.Now} [{caller}] {message}");
        }
    }
}