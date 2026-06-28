--
-- PostgreSQL database dump
--

--
-- Name: historico_conversas; Type: TABLE; Schema: public; Owner: admin
--

CREATE TABLE public.historico_conversas (
    id bigint NOT NULL,
    sessao_id uuid NOT NULL,
    pergunta_usuario text NOT NULL,
    resposta_ia text NOT NULL,
    data_interacao timestamp with time zone DEFAULT CURRENT_TIMESTAMP
);


--ALTER TABLE public.historico_conversas OWNER TO admin;

--
-- Name: TABLE historico_conversas; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON TABLE public.historico_conversas IS 'Registra o histórico completo de interações com a IA Íris para fins de auditoria e melhorias no RAG.';


--
-- Name: COLUMN historico_conversas.id; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.historico_conversas.id IS 'Chave primária autoincremental (BIGINT devido ao volume potencial de logs).';


--
-- Name: COLUMN historico_conversas.sessao_id; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.historico_conversas.sessao_id IS 'Identificador único da sessão do usuário no navegador (usado para agrupar conversas).';


--
-- Name: COLUMN historico_conversas.pergunta_usuario; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.historico_conversas.pergunta_usuario IS 'Prompt ou pergunta exata feita pelo visitante à IA.';


--
-- Name: COLUMN historico_conversas.resposta_ia; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.historico_conversas.resposta_ia IS 'Resposta textual gerada pela LLM (Gemini) baseada no contexto do currículo.';


--
-- Name: COLUMN historico_conversas.data_interacao; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.historico_conversas.data_interacao IS 'Timestamp exato de quando a interação ocorreu.';


--
-- Name: historico_conversas_id_seq; Type: SEQUENCE; Schema: public; Owner: admin
--

CREATE SEQUENCE public.historico_conversas_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--ALTER SEQUENCE public.historico_conversas_id_seq OWNER TO admin;

--
-- Name: historico_conversas_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: admin
--

ALTER SEQUENCE public.historico_conversas_id_seq OWNED BY public.historico_conversas.id;


--
-- Name: mensagens_contato; Type: TABLE; Schema: public; Owner: admin
--

CREATE TABLE public.mensagens_contato (
    id integer NOT NULL,
    nome character varying(100) NOT NULL,
    email character varying(150) NOT NULL,
    assunto character varying(150),
    mensagem text NOT NULL,
    data_envio timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    lida boolean DEFAULT false
);


--ALTER TABLE public.mensagens_contato OWNER TO admin;

--
-- Name: TABLE mensagens_contato; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON TABLE public.mensagens_contato IS 'Registra as mensagens enviadas por visitantes e recrutadores através do formulário Fale Conosco.';


--
-- Name: COLUMN mensagens_contato.id; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.mensagens_contato.id IS 'Chave primária autoincremental da mensagem.';


--
-- Name: COLUMN mensagens_contato.nome; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.mensagens_contato.nome IS 'Nome completo do remetente.';


--
-- Name: COLUMN mensagens_contato.email; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.mensagens_contato.email IS 'E-mail de contato do remetente.';


--
-- Name: COLUMN mensagens_contato.assunto; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.mensagens_contato.assunto IS 'Assunto ou título resumido do contato.';


--
-- Name: COLUMN mensagens_contato.mensagem; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.mensagens_contato.mensagem IS 'Conteúdo textual completo enviado pelo visitante.';


--
-- Name: COLUMN mensagens_contato.data_envio; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.mensagens_contato.data_envio IS 'Data e hora do envio do formulário.';


--
-- Name: COLUMN mensagens_contato.lida; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.mensagens_contato.lida IS 'Controle de visualização para o painel administrativo do Backoffice.';


--
-- Name: mensagens_contato_id_seq; Type: SEQUENCE; Schema: public; Owner: admin
--

CREATE SEQUENCE public.mensagens_contato_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.mensagens_contato_id_seq OWNER TO admin;

--
-- Name: mensagens_contato_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: admin
--

ALTER SEQUENCE public.mensagens_contato_id_seq OWNED BY public.mensagens_contato.id;


--
-- Name: projetos; Type: TABLE; Schema: public; Owner: admin
--

CREATE TABLE public.projetos (
    id integer NOT NULL,
    titulo character varying(150) NOT NULL,
    descricao text NOT NULL,
    tecnologias character varying(50)[] NOT NULL,
    url_imagem character varying(255),
    url_github character varying(255),
    url_demonstracao character varying(255),
    data_criacao timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    ativo boolean DEFAULT true
);


ALTER TABLE public.projetos OWNER TO admin;

--
-- Name: TABLE projetos; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON TABLE public.projetos IS 'Armazena os projetos do portfólio que serão renderizados dinamicamente no Blazor WASM.';


--
-- Name: COLUMN projetos.id; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.projetos.id IS 'Chave primária autoincremental do projeto.';


--
-- Name: COLUMN projetos.titulo; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.projetos.titulo IS 'Título comercial do projeto (ex: IRibeiroForHire).';


--
-- Name: COLUMN projetos.descricao; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.projetos.descricao IS 'Descrição detalhada sobre o escopo, arquitetura e objetivos do projeto.';


--
-- Name: COLUMN projetos.tecnologias; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.projetos.tecnologias IS 'Array de strings contendo as tecnologias utilizadas (ex: {"C#", ".NET 10", "Angular 19"}).';


--
-- Name: COLUMN projetos.url_imagem; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.projetos.url_imagem IS 'Link público da imagem ou screenshot que ilustra o projeto no card do frontend.';


--
-- Name: COLUMN projetos.url_github; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.projetos.url_github IS 'Link público para o repositório do código-fonte.';


--
-- Name: COLUMN projetos.url_demonstracao; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.projetos.url_demonstracao IS 'Link público para a aplicação rodando em ambiente de demonstração.';


--
-- Name: COLUMN projetos.data_criacao; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.projetos.data_criacao IS 'Data e hora em que o projeto foi cadastrado no sistema (com fuso horário).';


--
-- Name: COLUMN projetos.ativo; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.projetos.ativo IS 'Flag que controla a visibilidade do projeto no front-end.';


--
-- Name: projetos_id_seq; Type: SEQUENCE; Schema: public; Owner: admin
--

CREATE SEQUENCE public.projetos_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.projetos_id_seq OWNER TO admin;

--
-- Name: projetos_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: admin
--

ALTER SEQUENCE public.projetos_id_seq OWNED BY public.projetos.id;


--
-- Name: rss_feeds; Type: TABLE; Schema: public; Owner: admin
--

CREATE TABLE public.rss_feeds (
    id integer NOT NULL,
    nome_fonte character varying(100) NOT NULL,
    url_feed character varying(255) NOT NULL,
    ativo boolean DEFAULT true,
    ultima_sincronizacao timestamp with time zone
);


ALTER TABLE public.rss_feeds OWNER TO admin;

--
-- Name: TABLE rss_feeds; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON TABLE public.rss_feeds IS 'Cadastro de fontes de feeds RSS que o BackgroundService irá monitorar.';


--
-- Name: COLUMN rss_feeds.id; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.rss_feeds.id IS 'Chave primária autoincremental da fonte do feed.';


--
-- Name: COLUMN rss_feeds.nome_fonte; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.rss_feeds.nome_fonte IS 'Nome amigável da origem (ex: Inoreader Vagas, LinkedIn RSS).';


--
-- Name: COLUMN rss_feeds.url_feed; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.rss_feeds.url_feed IS 'URL absoluta do arquivo XML do feed RSS (campo único para evitar duplicados).';


--
-- Name: COLUMN rss_feeds.ativo; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.rss_feeds.ativo IS 'Determina se o Background Worker deve varrer este feed no loop atual.';


--
-- Name: COLUMN rss_feeds.ultima_sincronizacao; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.rss_feeds.ultima_sincronizacao IS 'Registra a última vez em que o Worker leu este feed com sucesso.';


--
-- Name: rss_feeds_id_seq; Type: SEQUENCE; Schema: public; Owner: admin
--

CREATE SEQUENCE public.rss_feeds_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.rss_feeds_id_seq OWNER TO admin;

--
-- Name: rss_feeds_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: admin
--

ALTER SEQUENCE public.rss_feeds_id_seq OWNED BY public.rss_feeds.id;


--
-- Name: vagas_triadas; Type: TABLE; Schema: public; Owner: admin
--

CREATE TABLE public.vagas_triadas (
    id bigint NOT NULL,
    feed_id integer,
    guid_vaga character varying(255) NOT NULL,
    titulo_vaga character varying(255) NOT NULL,
    link_vaga character varying(255) NOT NULL,
    data_publicacao timestamp with time zone,
    data_captura timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    enviado_por_email boolean DEFAULT false
);


ALTER TABLE public.vagas_triadas OWNER TO admin;

--
-- Name: TABLE vagas_triadas; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON TABLE public.vagas_triadas IS 'Controle e cache de vagas capturadas para triagem. Evita reenvio de e-mails duplicados.';


--
-- Name: COLUMN vagas_triadas.id; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.vagas_triadas.id IS 'Chave primária autoincremental.';


--
-- Name: COLUMN vagas_triadas.feed_id; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.vagas_triadas.feed_id IS 'Chave estrangeira apontando para o feed de origem.';


--
-- Name: COLUMN vagas_triadas.guid_vaga; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.vagas_triadas.guid_vaga IS 'Identificador global único fornecido pelo XML do RSS (usado para checar duplicidade).';


--
-- Name: COLUMN vagas_triadas.titulo_vaga; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.vagas_triadas.titulo_vaga IS 'Título da vaga capturada.';


--
-- Name: COLUMN vagas_triadas.link_vaga; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.vagas_triadas.link_vaga IS 'URL de redirecionamento para a candidatura ou detalhes da vaga.';


--
-- Name: COLUMN vagas_triadas.data_publicacao; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.vagas_triadas.data_publicacao IS 'Data em que a vaga foi originalmente publicada na fonte RSS.';


--
-- Name: COLUMN vagas_triadas.data_captura; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.vagas_triadas.data_captura IS 'Data e hora em que o nosso Background Worker processou o item.';


--
-- Name: COLUMN vagas_triadas.enviado_por_email; Type: COMMENT; Schema: public; Owner: admin
--

COMMENT ON COLUMN public.vagas_triadas.enviado_por_email IS 'Flag indicando se a vaga passou no filtro e já foi disparada para o e-mail destino.';


--
-- Name: vagas_triadas_id_seq; Type: SEQUENCE; Schema: public; Owner: admin
--

CREATE SEQUENCE public.vagas_triadas_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.vagas_triadas_id_seq OWNER TO admin;

--
-- Name: vagas_triadas_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: admin
--

ALTER SEQUENCE public.vagas_triadas_id_seq OWNED BY public.vagas_triadas.id;


--
-- Name: historico_conversas id; Type: DEFAULT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.historico_conversas ALTER COLUMN id SET DEFAULT nextval('public.historico_conversas_id_seq'::regclass);


--
-- Name: mensagens_contato id; Type: DEFAULT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.mensagens_contato ALTER COLUMN id SET DEFAULT nextval('public.mensagens_contato_id_seq'::regclass);


--
-- Name: projetos id; Type: DEFAULT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.projetos ALTER COLUMN id SET DEFAULT nextval('public.projetos_id_seq'::regclass);


--
-- Name: rss_feeds id; Type: DEFAULT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.rss_feeds ALTER COLUMN id SET DEFAULT nextval('public.rss_feeds_id_seq'::regclass);


--
-- Name: vagas_triadas id; Type: DEFAULT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.vagas_triadas ALTER COLUMN id SET DEFAULT nextval('public.vagas_triadas_id_seq'::regclass);


--
-- Name: historico_conversas historico_conversas_pkey; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.historico_conversas
    ADD CONSTRAINT historico_conversas_pkey PRIMARY KEY (id);


--
-- Name: mensagens_contato mensagens_contato_pkey; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.mensagens_contato
    ADD CONSTRAINT mensagens_contato_pkey PRIMARY KEY (id);


--
-- Name: projetos projetos_pkey; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.projetos
    ADD CONSTRAINT projetos_pkey PRIMARY KEY (id);


--
-- Name: rss_feeds rss_feeds_pkey; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.rss_feeds
    ADD CONSTRAINT rss_feeds_pkey PRIMARY KEY (id);


--
-- Name: rss_feeds rss_feeds_url_feed_key; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.rss_feeds
    ADD CONSTRAINT rss_feeds_url_feed_key UNIQUE (url_feed);


--
-- Name: vagas_triadas vagas_triadas_guid_vaga_key; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.vagas_triadas
    ADD CONSTRAINT vagas_triadas_guid_vaga_key UNIQUE (guid_vaga);


--
-- Name: vagas_triadas vagas_triadas_pkey; Type: CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.vagas_triadas
    ADD CONSTRAINT vagas_triadas_pkey PRIMARY KEY (id);


--
-- Name: idx_conversas_sessao; Type: INDEX; Schema: public; Owner: admin
--

CREATE INDEX idx_conversas_sessao ON public.historico_conversas USING btree (sessao_id);


--
-- Name: idx_vagas_guid; Type: INDEX; Schema: public; Owner: admin
--

CREATE INDEX idx_vagas_guid ON public.vagas_triadas USING btree (guid_vaga);


--
-- Name: vagas_triadas vagas_triadas_feed_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: admin
--

ALTER TABLE ONLY public.vagas_triadas
    ADD CONSTRAINT vagas_triadas_feed_id_fkey FOREIGN KEY (feed_id) REFERENCES public.rss_feeds(id) ON DELETE CASCADE;


