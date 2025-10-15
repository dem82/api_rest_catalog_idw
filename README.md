# Workflow API - .NET 8 with SQLite and Redis

API REST in .NET 8 che gestisce workflow con caching Redis e database SQLite.

## Caratteristiche

- **POST API** per trovare workflow basati su criteri di input
- **Caching Redis** per ottimizzare le chiamate ripetute
- **Database SQLite** per la persistenza dei dati
- **Swagger/OpenAPI** per la documentazione dell'API

## Prerequisiti

- .NET 8 SDK
- Docker (opzionale - per eseguire Redis)
- SQLite3 (opzionale - per inizializzare il database manualmente)
- Redis (opzionale - l'API funziona anche senza Redis)

## Struttura del Database

Il database SQLite contiene le seguenti tabelle:

- **workflow**: id_workflow (PK), descrizione, input_system, input_format
- **service**: service_code (PK), service_desc
- **workflow_service**: id_workflow, service_code
- **workflow_systems**: id_workflow, system_code, system_ord
- **systems**: system_code (PK), system_desc

## Setup

1. **Installare le dipendenze:**
   ```bash
   dotnet restore
   ```

2. **Configurare Redis (opzionale):**
   - **Opzione 1 - Docker (consigliato):**
     ```bash
     docker-compose up -d
     ```
   - **Opzione 2 - Redis locale:**
     Installa e avvia Redis sulla tua macchina
   - Modifica `appsettings.json` per configurare la connessione Redis
   - Default: `localhost:6379`
   - **Nota:** L'API funziona anche se Redis non è disponibile (fallback al database)

3. **Configurare SQLite:**
   - Il file del database verrà creato automaticamente come `workflow.db`
   - Puoi modificare il path in `appsettings.json`

4. **Creare il database:**
   
   **Opzione 1 - Usando SQLite direttamente:**
   ```bash
   sqlite3 workflow.db < DatabaseInitializer.sql
   ```

   **Opzione 2 - Usando Entity Framework Migrations:**
   ```bash
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
   
   Poi popola il database con i dati di esempio eseguendo lo script SQL fornito.

## Avvio dell'applicazione

```bash
dotnet run
```

L'API sarà disponibile su:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## Utilizzo dell'API

### Endpoint: POST /api/workflow/find

**Request Body:**
```json
{
  "inputFormat": "input_format",
  "inputSystem": "input_system",
  "requiredServices": ["service_code_1", "service_code_2"],
  "outputType": 1
}
```

**Response:**
```json
{
  "workflowCode": "WF_001",
  "success": true
}
```

### Esempio con cURL:

```bash
curl -X POST "https://localhost:5001/api/workflow/find" \
  -H "Content-Type: application/json" \
  -d @example-request.json
```

Oppure:

```bash
curl -X POST "https://localhost:5001/api/workflow/find" \
  -H "Content-Type: application/json" \
  -d '{
    "inputFormat": "JSON",
    "inputSystem": "SAP",
    "requiredServices": ["SRV_001", "SRV_002"],
    "outputType": 1
  }'
```

**Response attesa (con i dati di esempio):**
```json
{
  "workflowCode": "WF_001",
  "success": true
}
```

## Funzionamento del Caching

1. **Prima chiamata**: L'API interroga il database SQLite e salva il risultato in Redis (TTL: 5 minuti)
2. **Chiamate successive**: L'API recupera il risultato dalla cache Redis
3. **Cache miss**: Se il dato non è in cache, viene richiesto al database e poi cachato

La cache key è generata da: `workflow:{inputFormat}:{inputSystem}:{services}:{outputType}`

## Struttura del Progetto

```
/workspace/
├── Controllers/
│   └── WorkflowController.cs      # Controller REST API
├── Data/
│   └── WorkflowDbContext.cs       # Context Entity Framework
├── DTOs/
│   ├── WorkflowRequest.cs         # DTO per la richiesta
│   └── WorkflowResponse.cs        # DTO per la risposta
├── Models/
│   ├── Workflow.cs                # Model workflow
│   ├── Service.cs                 # Model service
│   ├── WorkflowService.cs         # Model relazione workflow-service
│   ├── WorkflowSystem.cs          # Model relazione workflow-system
│   └── SystemEntity.cs            # Model systems
├── Services/
│   ├── IWorkflowService.cs        # Interfaccia service
│   └── WorkflowService.cs         # Implementazione con Redis caching
├── Program.cs                      # Entry point e configurazione
├── appsettings.json               # Configurazione
└── WorkflowApi.csproj             # File di progetto
```

## Note

- L'API è resiliente: se Redis non è disponibile, continua a funzionare usando solo il database
- I log sono configurati per tracciare cache hit/miss e operazioni sul database
- La validazione del body della richiesta è implementata nel controller
