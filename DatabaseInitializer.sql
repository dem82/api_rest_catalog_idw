-- Script di esempio per inizializzare il database SQLite
-- Esegui questo script per creare tabelle e dati di esempio

-- Crea tabella workflow
CREATE TABLE IF NOT EXISTS workflow (
    id_workflow TEXT PRIMARY KEY,
    descrizione TEXT,
    input_system TEXT,
    input_format TEXT
);

-- Crea tabella service
CREATE TABLE IF NOT EXISTS service (
    service_code TEXT PRIMARY KEY,
    service_desc TEXT
);

-- Crea tabella workflow_service
CREATE TABLE IF NOT EXISTS workflow_service (
    id_workflow TEXT,
    service_code TEXT,
    PRIMARY KEY (id_workflow, service_code),
    FOREIGN KEY (id_workflow) REFERENCES workflow(id_workflow),
    FOREIGN KEY (service_code) REFERENCES service(service_code)
);

-- Crea tabella systems
CREATE TABLE IF NOT EXISTS systems (
    system_code TEXT PRIMARY KEY,
    system_desc TEXT
);

-- Crea tabella workflow_systems
CREATE TABLE IF NOT EXISTS workflow_systems (
    id_workflow TEXT,
    system_code TEXT,
    system_ord INTEGER,
    PRIMARY KEY (id_workflow, system_code),
    FOREIGN KEY (id_workflow) REFERENCES workflow(id_workflow),
    FOREIGN KEY (system_code) REFERENCES systems(system_code)
);

-- Inserisci dati di esempio

-- Services
INSERT OR IGNORE INTO service (service_code, service_desc) VALUES 
('SRV_001', 'Service 1 - Data Validation'),
('SRV_002', 'Service 2 - Data Transformation'),
('SRV_003', 'Service 3 - Data Enrichment');

-- Systems
INSERT OR IGNORE INTO systems (system_code, system_desc) VALUES 
('SYS_SAP', 'SAP System'),
('SYS_CRM', 'CRM System'),
('SYS_API', 'API Gateway');

-- Workflows
INSERT OR IGNORE INTO workflow (id_workflow, descrizione, input_system, input_format) VALUES 
('WF_001', 'Workflow per dati JSON da SAP', 'SAP', 'JSON'),
('WF_002', 'Workflow per dati XML da CRM', 'CRM', 'XML'),
('WF_003', 'Workflow per dati CSV da SAP', 'SAP', 'CSV');

-- Workflow Services (associazioni)
INSERT OR IGNORE INTO workflow_service (id_workflow, service_code) VALUES 
('WF_001', 'SRV_001'),
('WF_001', 'SRV_002'),
('WF_002', 'SRV_002'),
('WF_002', 'SRV_003'),
('WF_003', 'SRV_001');

-- Workflow Systems (associazioni con ordine)
INSERT OR IGNORE INTO workflow_systems (id_workflow, system_code, system_ord) VALUES 
('WF_001', 'SYS_SAP', 1),
('WF_001', 'SYS_API', 2),
('WF_002', 'SYS_CRM', 1),
('WF_002', 'SYS_API', 2),
('WF_003', 'SYS_SAP', 1);
