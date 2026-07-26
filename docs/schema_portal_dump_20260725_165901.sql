--
-- PostgreSQL database dump
--

\restrict gJfRDLxIGWa4fUWb5mOJEU8z3nAyZu8mHhInB8dlPhY8jStc34a0R2GcZsPXJdi

-- Dumped from database version 17.10 (Debian 17.10-1.pgdg12+1)
-- Dumped by pg_dump version 17.10 (Debian 17.10-1.pgdg12+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP INDEX IF EXISTS portal.idx_projetos_ativo_data;
DROP INDEX IF EXISTS portal.idx_conversas_sessao;
ALTER TABLE IF EXISTS ONLY portal.projetos DROP CONSTRAINT IF EXISTS projetos_pkey;
ALTER TABLE IF EXISTS ONLY portal.historico_conversas DROP CONSTRAINT IF EXISTS historico_conversas_pkey;
ALTER TABLE IF EXISTS portal.projetos ALTER COLUMN id DROP DEFAULT;
ALTER TABLE IF EXISTS portal.historico_conversas ALTER COLUMN id DROP DEFAULT;
DROP SEQUENCE IF EXISTS portal.projetos_id_seq;
DROP TABLE IF EXISTS portal.projetos;
DROP SEQUENCE IF EXISTS portal.historico_conversas_id_seq;
DROP TABLE IF EXISTS portal.historico_conversas;
DROP SCHEMA IF EXISTS portal;
--
-- Name: portal; Type: SCHEMA; Schema: -; Owner: admin
--

CREATE SCHEMA portal;


ALTER SCHEMA portal OWNER TO admin;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: historico_conversas; Type: TABLE; Schema: portal; Owner: admin
--

CREATE TABLE portal.historico_conversas (
    id bigint NOT NULL,
    sessao_id uuid NOT NULL,
    pergunta_usuario text NOT NULL,
    resposta_ia text NOT NULL,
    data_interacao timestamp with time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE portal.historico_conversas OWNER TO admin;

--
-- Name: historico_conversas_id_seq; Type: SEQUENCE; Schema: portal; Owner: admin
--

CREATE SEQUENCE portal.historico_conversas_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE portal.historico_conversas_id_seq OWNER TO admin;

--
-- Name: historico_conversas_id_seq; Type: SEQUENCE OWNED BY; Schema: portal; Owner: admin
--

ALTER SEQUENCE portal.historico_conversas_id_seq OWNED BY portal.historico_conversas.id;


--
-- Name: projetos; Type: TABLE; Schema: portal; Owner: admin
--

CREATE TABLE portal.projetos (
    id integer NOT NULL,
    titulo character varying(150) NOT NULL,
    descricao text NOT NULL,
    tecnologias character varying[] NOT NULL,
    url_imagem character varying(255),
    url_github character varying(255),
    url_demonstracao character varying(255),
    data_criacao timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    ativo boolean DEFAULT true
);


ALTER TABLE portal.projetos OWNER TO admin;

--
-- Name: projetos_id_seq; Type: SEQUENCE; Schema: portal; Owner: admin
--

CREATE SEQUENCE portal.projetos_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE portal.projetos_id_seq OWNER TO admin;

--
-- Name: projetos_id_seq; Type: SEQUENCE OWNED BY; Schema: portal; Owner: admin
--

ALTER SEQUENCE portal.projetos_id_seq OWNED BY portal.projetos.id;


--
-- Name: historico_conversas id; Type: DEFAULT; Schema: portal; Owner: admin
--

ALTER TABLE ONLY portal.historico_conversas ALTER COLUMN id SET DEFAULT nextval('portal.historico_conversas_id_seq'::regclass);


--
-- Name: projetos id; Type: DEFAULT; Schema: portal; Owner: admin
--

ALTER TABLE ONLY portal.projetos ALTER COLUMN id SET DEFAULT nextval('portal.projetos_id_seq'::regclass);


--
-- Data for Name: historico_conversas; Type: TABLE DATA; Schema: portal; Owner: admin
--

COPY portal.historico_conversas (id, sessao_id, pergunta_usuario, resposta_ia, data_interacao) FROM stdin;
\.


--
-- Data for Name: projetos; Type: TABLE DATA; Schema: portal; Owner: admin
--

COPY portal.projetos (id, titulo, descricao, tecnologias, url_imagem, url_github, url_demonstracao, data_criacao, ativo) FROM stdin;
1	Portal IRibeiro / Íris	Assistente inteligente de IA focado na leitura dinâmica de currículo e trajetórias profissionais, utilizando tecnologia RAG.	{".NET 10","C# 14",Blazor,"Upstash Redis",RAG}	/images/laboratorio/resume-assist.jpeg	https://github.com/iribeirodev/PortalIRibeiro#assistente-inteligente-%C3%ADris		2026-06-27 21:14:51.650179+00	t
\.


--
-- Name: historico_conversas_id_seq; Type: SEQUENCE SET; Schema: portal; Owner: admin
--

SELECT pg_catalog.setval('portal.historico_conversas_id_seq', 1, false);


--
-- Name: projetos_id_seq; Type: SEQUENCE SET; Schema: portal; Owner: admin
--

SELECT pg_catalog.setval('portal.projetos_id_seq', 1, true);


--
-- Name: historico_conversas historico_conversas_pkey; Type: CONSTRAINT; Schema: portal; Owner: admin
--

ALTER TABLE ONLY portal.historico_conversas
    ADD CONSTRAINT historico_conversas_pkey PRIMARY KEY (id);


--
-- Name: projetos projetos_pkey; Type: CONSTRAINT; Schema: portal; Owner: admin
--

ALTER TABLE ONLY portal.projetos
    ADD CONSTRAINT projetos_pkey PRIMARY KEY (id);


--
-- Name: idx_conversas_sessao; Type: INDEX; Schema: portal; Owner: admin
--

CREATE INDEX idx_conversas_sessao ON portal.historico_conversas USING btree (sessao_id);


--
-- Name: idx_projetos_ativo_data; Type: INDEX; Schema: portal; Owner: admin
--

CREATE INDEX idx_projetos_ativo_data ON portal.projetos USING btree (ativo, data_criacao DESC);


--
-- PostgreSQL database dump complete
--

\unrestrict gJfRDLxIGWa4fUWb5mOJEU8z3nAyZu8mHhInB8dlPhY8jStc34a0R2GcZsPXJdi

