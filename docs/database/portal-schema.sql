-- =====================================================================
-- Portal IRibeiro - Database schema (English naming)
--
-- Drop + create to be applied in local and production environments.
-- Consolidates migrations 001 (visits) and 002 (rename to English).
-- =====================================================================

CREATE SCHEMA IF NOT EXISTS portal;

DROP TABLE IF EXISTS portal.visits;
DROP TABLE IF EXISTS portal.visit_cache;
DROP TABLE IF EXISTS portal.projects;
DROP TABLE IF EXISTS portal.chat_history;
DROP TABLE IF EXISTS portal.parameters;

-- ---------------------------------------------------------------------
-- Projects
-- ---------------------------------------------------------------------
CREATE TABLE portal.projects (
    id serial4 NOT NULL,
    title varchar(150) NOT NULL,
    description text NOT NULL,
    technologies varchar[] NOT NULL,
    image_url varchar(255) NULL,
    github_url varchar(255) NULL,
    demo_url varchar(255) NULL,
    created_at timestamptz DEFAULT CURRENT_TIMESTAMP NULL,
    is_active bool DEFAULT true NULL,
    CONSTRAINT projects_pkey PRIMARY KEY (id)
);
CREATE INDEX idx_projects_active ON portal.projects USING btree (created_at DESC) WHERE (is_active = true);

-- Seed data
INSERT INTO portal.projects (title, description, technologies, image_url, github_url, demo_url, created_at, is_active) VALUES
	 ('Portal IRibeiro / Íris','Assistente inteligente de IA focado na leitura dinâmica de currículo e trajetórias profissionais, utilizando tecnologia RAG.','{".NET 10","C# 14","Blazor","Upstash Redis","RAG"}','/images/laboratorio/resume-assist.jpeg','https://github.com/iribeirodev/PortalIRibeiro#assistente-inteligente-%C3%ADris','','2026-06-27 18:14:51.650',true);

-- ---------------------------------------------------------------------
-- Parameters (key-value store for long-lived configuration/content)
-- ---------------------------------------------------------------------
CREATE TABLE portal.parameters (
    param_key    varchar(64) NOT NULL,
    param_value  text        NOT NULL,
    updated_at   timestamptz DEFAULT CURRENT_TIMESTAMP NULL,
    CONSTRAINT parameters_pkey PRIMARY KEY (param_key)
);

INSERT INTO portal.parameters (param_key, param_value) VALUES
    ('curriculo:itamar', $$ITAMAR DA SILVA RIBEIRO JUNIOR
Senior Full Stack Developer / Software Engineer

=========================================
RESUMO PROFISSIONAL E MÉTRICAS DE EXPERIÊNCIA
=========================================
• Tempo Total na Área de TI: Cerca de 30 anos de trajetória sólida.
• Experiência com C# e Ecossistema .NET: Mais de 15 anos de atuação forte, desde versões legadas do .NET Framework até o moderno .NET 10.
• Experiência com VB.NET: Mais de 5 anos de experiência prática em sistemas corporativos e bancários críticos.
• Outras Tecnologias: Experiência prática e desenvolvimento multiplataforma utilizando Node.js e Python.
• Especialidades em Arquitetura: Clean Architecture, CQRS, Injeção de Dependência, Padrões de Projeto e sustentação de sistemas críticos de alta disponibilidade.
• Transição e Foco Atual: Engenharia de Dados (construção de pipelines ELT, Arquitetura Medallion, dbt e DuckDB).
• Infraestrutura e Containers: Domínio em Docker e Portainer CE para orquestração e gerenciamento de ambientes de microsserviços.

=========================================
PROJETOS RECENTES & LABORATÓRIO
=========================================
• Portal IRibeiro / ResumeAssist: Assistente inteligente de IA construído nativamente em .NET 10 e Angular 21. Utiliza arquitetura RAG para respostas contextualizadas, combinada com Upstash Redis para otimização de cache e baixa latência.

=========================================
EXPERIÊNCIA CORPORATIVA DETALHADA
=========================================
Stefanini (alocado na ABB) – Senior Systems Developer – Desde Setembro/2025
Atuação remota em sustentação técnica avançada, refatoração e evolução de sistemas críticos. Garantia de estabilidade operacional, aplicação de melhores práticas de engenharia de software e resolução de incidentes complexos via fila de chamados para os módulos da companhia.

Randstad (alocado na ABB) – Software Developer – De Janeiro/2023 a Setembro/2025
Desenvolvimento de novas funcionalidades e regras de negócio para módulos internos de RH, sistemas de treinamentos e atas de projetos utilizando .NET 5/7 com SQL Server. Criação de interfaces SPA utilizando Angular com Angular Material e Bootstrap. Elaboração e condução de treinamentos práticos para a equipe de engenharia sobre o uso de IA Generativa no tratamento e estruturação de dados desestruturados.

PagBem – Software Developer – De Julho/2021 a Janeiro/2023
Atuação na sustentação e evolução de plataforma core de pagamentos eletrônicos voltada ao setor logístico, focada em alta disponibilidade e segurança transacional. Manutenção corretiva de ecossistema legado e implementação de novas APIs no Back-end utilizando C# (.NET Framework e .NET Core), Redis e SQL Server. Desenvolvimento de interfaces responsivas web com HTML5, AngularJS e Bootstrap integradas a endpoints RESTful.

7Comm (alocado na Ágora Corretora) – Software Developer – De Agosto/2020 a Julho/2021
Manutenção corretiva, evolutiva e adaptativa em sistemas de missão crítica baseados em produtos de Fundos e COE. Liderança técnica na migração tecnológica de módulos para .NET Core 3.1 e implementação de novas funcionalidades no legado de previdência privada (Web API e MVC). Desenvolvimento de Front-End com HTML5 e jQuery via chamadas assíncronas (AJAX). Desenvolvimento de regras de negócio complexas no Back-end com banco de dados Oracle 12 através de Packages, Stored Procedures e Triggers em PL-SQL, além de integração com APIs IBM (DIL).

Sinqia (alocado no Banco BMB) – Software Developer – De Novembro/2019 a Março/2020
Suporte especializado à produção para resolução de incidentes críticos de negócio e manutenção corretiva de sistemas bancários legados. Aplicação de Data Changes controlados em SQL Server e Program Changes em VB.NET para estabilização de sistemas. Gerenciamento e automação de deploys através de esteiras de CI/CD via Visual Studio Team Services (TFS).

Sonda IT (alocado na SMT/SP) – Software Developer – De Janeiro/2019 a Novembro/2019
Manutenção de sistemas transacionais de indicação de multas e liberação de alvarás da Secretaria Municipal de Transportes de São Paulo. Desenvolvimento de Back-end utilizando .NET Core, C# (Web API) e microsserviços em Node.js com persistência em SQL Server via Entity Framework. Criação de interfaces administrativas em AngularJS e Bootstrap, além de desenvolvimento de um sistema desktop utilitário em C# com Windows Forms e MariaDB para o acompanhamento de eleições do conselho municipal. Cobertura de código com testes unitários automatizados utilizando NUnit.

TechFor IT (alocado na Amadeus) – Software Developer – De Fevereiro/2018 a Agosto/2018
Desenvolvimento de robôs de automação de processos e Web Scraping utilizando Node.js e a biblioteca NightmareJS. Implementação de sistema corporativo de comunicação e telefonia VOIP utilizando Java MVC integrado à API do Twilio. Criação de dashboards de gestão em Angular e páginas JSP. Configuração, tunagem e deploy em servidores de aplicação WebLogic 12c com automação via Maven, gerenciando versões do JSE (1.7/1.8) na IDE Eclipse Mars.

TCS Brazil IT Services – Software Developer – De Maio/2013 a Novembro/2017
• Cliente SwissRe: Engenharia e manutenção de módulos complexos de emissão de apólices internacionais para as linhas de negócio Marine, Surety e P&C (Property & Casualty), além de sustentação do core de resseguros e sinistros em sistemas utilizando C#, VB.NET, ASP.NET Web Forms e ASP. Criação de arquitetura baseada em Windows Services e filas de mensageria para assinatura digital de documentos usando ASP.NET, componentes Telerik e SQL Server 2012 (Stored Procedures, Functions, Views e Performance Tuning de Jobs). Atuação em Squad de migração técnica de plataformas legadas para arquitetura integrada Web baseada em ASP.NET MVC, Web API, AngularJS e Bootstrap, conduzindo levantamento de requisitos e alinhamento técnico direto com equipes Onshore e Offshore.
• Cliente Siemens: Sustentação de módulos de engenharia para o gerenciamento de propostas de projetos utilizando ASP Clássico, C# e modelagem em SQL Server 2008.

Systemplan/IBM (alocado na Shell Brazil) – Development Consultant – De Outubro/2011 a Setembro/2012
Consultoria técnica especializada para o desenvolvimento e implantação do ecossistema de Nota Fiscal Eletrônica (NF-e), integrando e mapeando dados fiscais entre a camada .NET e o ERP SAP. Desenvolvimento de rotinas robustas para envio em lote e processamento síncrono/assíncrono de arquivos XML via SQL Server, garantindo integridade regulatória.

Elumini IT – Software Developer – De Agosto/2010 a Outubro/2011
• Cliente SulAmerica/ING: Sustentação do sistema nacional de cotações de seguros de frotas e automóveis. Desenvolvimento e correção em VB.NET com barramento de comunicação baseado em Web Services (SOAP/WSDL).
• Cliente BTG Pactual: Desenvolvimento de software para o core bancário corporativo e módulos confidenciais de cálculo de Participação nos Lucros e Resultados (PLR) em VB.NET. Criação de WCF Services (Windows Communication Foundation) seguros para consumo de dados cadastrais e manutenções críticas em legados construídos em VB.NET 1.1 e planilhas automatizadas com Excel/VBA.
• Cliente Leader Magazine: Suporte de terceiro nível e manutenção ágil na intranet e extranet comercial da varejista utilizando ASP Clássico e PL/SQL em banco de dados Oracle.

DTS Consulting (alocado na Bradesco Seguros) – Software Developer – De Novembro/2009 a Agosto/2010
Sustentação técnica evolutiva do sistema core de vendas e emissão de apólices massificadas. Garantia de continuidade de negócio e correção de bugs prioritários em ambiente composto por ASP Clássico, VB6 e banco de dados Microsoft Access.

Stefanini IT (alocado na SulAmerica/ING) – Software Developer – De Julho/2007 a Setembro/2009
Engenharia de software para o sistema de cadastro de contratos complexos de resseguros e conta corrente utilizando VB6 com persistência em mainframe DB2. Implementação de robôs eficientes para leitura, processamento em lote (Batch) e disparo automático de faturas e prêmios utilizando C#. Condução da migração e reengenharia de software do sistema de emissão de apólices do setor de Aviação (Aviation) de Clipper para VB6.

MGN (alocado na Light) – Software Developer – De Dezembro/2006 a Julho/2007
Desenvolvimento e manutenção em sistemas comerciais de distribuição de energia utilizando VB.NET e banco de dados Oracle. Criação de automações de banco de dados e rotinas de carga via PL/SQL integradas a scripts de infraestrutura em Bash para ambientes Unix/Linux.$$) ON CONFLICT (param_key) DO UPDATE SET param_value = EXCLUDED.param_value, updated_at = CURRENT_TIMESTAMP;

-- ---------------------------------------------------------------------
-- Chat history
-- ---------------------------------------------------------------------
CREATE TABLE portal.chat_history (
    id bigserial NOT NULL,
    session_id uuid NOT NULL,
    user_question text NOT NULL,
    ai_response text NOT NULL,
    interaction_date timestamptz DEFAULT CURRENT_TIMESTAMP NULL,
    CONSTRAINT chat_history_pkey PRIMARY KEY (id)
);
CREATE INDEX idx_chat_history_session ON portal.chat_history USING btree (session_id);

-- ---------------------------------------------------------------------
-- Visits
-- ---------------------------------------------------------------------
CREATE TABLE portal.visits (
    id int4 GENERATED BY DEFAULT AS IDENTITY NOT NULL, -- Unique identifier of the visit record.
    ip_address varchar(45) NULL, -- IP address of the visitor, supporting IPv4 and IPv6.
    country varchar(100) NULL, -- Country estimated from the visitor IP address.
    city varchar(100) NULL, -- City estimated from the visitor IP address.
    region varchar(100) NULL, -- Region or state estimated from the visitor IP address.
    page varchar(200) NULL, -- Portal page accessed by the visitor.
    user_agent text NULL, -- HTTP User-Agent reported by the visitor client.
    accessed_at timestamptz DEFAULT CURRENT_TIMESTAMP NULL, -- UTC timestamp when the visit was recorded.
    referer text NULL, -- HTTP Referer indicating the page or website that originated the navigation.
    visit_type varchar(30) NULL, -- Classification of the visit, such as human, crawler, social_crawler, bot, or unknown.
    bot_name varchar(100) NULL, -- Identified name of the crawler or bot, when applicable.
    CONSTRAINT visits_pkey PRIMARY KEY (id)
);
CREATE INDEX idx_visits_accessed_at ON portal.visits USING btree (accessed_at DESC);
CREATE INDEX idx_visits_ip_address_accessed_at ON portal.visits USING btree (ip_address, accessed_at DESC);
CREATE INDEX idx_visits_visit_type_accessed_at ON portal.visits USING btree (visit_type, accessed_at DESC);

COMMENT ON TABLE portal.visits IS 'Stores visitor telemetry collected by the portal.';

-- ---------------------------------------------------------------------
-- Visit deduplication cache
-- ---------------------------------------------------------------------
-- Short-lived cache that keeps the pair (ip_address + page) for a sliding
-- window. Used to avoid counting repeated visits (and GeoIP lookups) when
-- the same IP reloads the same page within the window. Rows older than the
-- window are opportunistically deleted; nothing here is analytical data.
CREATE TABLE portal.visit_cache (
    ip_address    varchar(45)  NOT NULL, -- Visitor IP (IPv4 or IPv6).
    page          varchar(200) NOT NULL, -- Portal page accessed by the visitor.
    registered_at timestamptz  NOT NULL DEFAULT CURRENT_TIMESTAMP, -- UTC timestamp when the pair was cached.
    CONSTRAINT visit_cache_pkey PRIMARY KEY (ip_address, page)
);
CREATE INDEX idx_visit_cache_registered_at ON portal.visit_cache USING btree (registered_at);

COMMENT ON TABLE portal.visit_cache IS 'Deduplication cache for visit telemetry (single IP + page per window).';
