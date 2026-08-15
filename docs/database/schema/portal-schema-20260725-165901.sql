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

DROP INDEX IF EXISTS portal.idx_projects_active;
DROP INDEX IF EXISTS portal.idx_chat_history_session;
ALTER TABLE IF EXISTS ONLY portal.projects DROP CONSTRAINT IF EXISTS projects_pkey;
ALTER TABLE IF EXISTS ONLY portal.chat_history DROP CONSTRAINT IF EXISTS chat_history_pkey;
ALTER TABLE IF EXISTS portal.projects ALTER COLUMN id DROP DEFAULT;
ALTER TABLE IF EXISTS portal.chat_history ALTER COLUMN id DROP DEFAULT;
DROP SEQUENCE IF EXISTS portal.projects_id_seq;
DROP TABLE IF EXISTS portal.projects;
DROP SEQUENCE IF EXISTS portal.chat_history_id_seq;
DROP TABLE IF EXISTS portal.chat_history;
DROP SCHEMA IF EXISTS portal;
--
-- Name: portal; Type: SCHEMA; Schema: -; Owner: admin
--

CREATE SCHEMA portal;


ALTER SCHEMA portal OWNER TO admin;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: chat_history; Type: TABLE; Schema: portal; Owner: admin
--

CREATE TABLE portal.chat_history (
    id bigint NOT NULL,
    session_id uuid NOT NULL,
    user_question text NOT NULL,
    ai_response text NOT NULL,
    interaction_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE portal.chat_history OWNER TO admin;

--
-- Name: chat_history_id_seq; Type: SEQUENCE; Schema: portal; Owner: admin
--

CREATE SEQUENCE portal.chat_history_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE portal.chat_history_id_seq OWNER TO admin;

--
-- Name: chat_history_id_seq; Type: SEQUENCE OWNED BY; Schema: portal; Owner: admin
--

ALTER SEQUENCE portal.chat_history_id_seq OWNED BY portal.chat_history.id;


--
-- Name: projects; Type: TABLE; Schema: portal; Owner: admin
--

CREATE TABLE portal.projects (
    id integer NOT NULL,
    title character varying(150) NOT NULL,
    description text NOT NULL,
    technologies character varying[] NOT NULL,
    image_url character varying(255),
    github_url character varying(255),
    demo_url character varying(255),
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    is_active boolean DEFAULT true
);


ALTER TABLE portal.projects OWNER TO admin;

--
-- Name: projects_id_seq; Type: SEQUENCE; Schema: portal; Owner: admin
--

CREATE SEQUENCE portal.projects_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE portal.projects_id_seq OWNER TO admin;

--
-- Name: projects_id_seq; Type: SEQUENCE OWNED BY; Schema: portal; Owner: admin
--

ALTER SEQUENCE portal.projects_id_seq OWNED BY portal.projects.id;


--
-- Name: chat_history id; Type: DEFAULT; Schema: portal; Owner: admin
--

ALTER TABLE ONLY portal.chat_history ALTER COLUMN id SET DEFAULT nextval('portal.chat_history_id_seq'::regclass);


--
-- Name: projects id; Type: DEFAULT; Schema: portal; Owner: admin
--

ALTER TABLE ONLY portal.projects ALTER COLUMN id SET DEFAULT nextval('portal.projects_id_seq'::regclass);


--
-- Data for Name: chat_history; Type: TABLE DATA; Schema: portal; Owner: admin
--

COPY portal.chat_history (id, session_id, user_question, ai_response, interaction_date) FROM stdin;
\.


--
-- Data for Name: projects; Type: TABLE DATA; Schema: portal; Owner: admin
--

COPY portal.projects (id, title, description, technologies, image_url, github_url, demo_url, created_at, is_active) FROM stdin;
1	Portal IRibeiro / Íris	Assistente inteligente de IA focado na leitura dinâmica de currículo e trajetórias profissionais, utilizando tecnologia RAG.	{".NET 10","C# 14",Blazor,"Upstash Redis",RAG}	/images/laboratorio/resume-assist.jpeg	https://github.com/iribeirodev/PortalIRibeiro#assistente-inteligente-%C3%ADris		2026-06-27 21:14:51.650179+00	t
\.


--
-- Name: chat_history_id_seq; Type: SEQUENCE SET; Schema: portal; Owner: admin
--

SELECT pg_catalog.setval('portal.chat_history_id_seq', 1, false);


--
-- Name: projects_id_seq; Type: SEQUENCE SET; Schema: portal; Owner: admin
--

SELECT pg_catalog.setval('portal.projects_id_seq', 1, true);


--
-- Name: chat_history chat_history_pkey; Type: CONSTRAINT; Schema: portal; Owner: admin
--

ALTER TABLE ONLY portal.chat_history
    ADD CONSTRAINT chat_history_pkey PRIMARY KEY (id);


--
-- Name: projects projects_pkey; Type: CONSTRAINT; Schema: portal; Owner: admin
--

ALTER TABLE ONLY portal.projects
    ADD CONSTRAINT projects_pkey PRIMARY KEY (id);


--
-- Name: idx_chat_history_session; Type: INDEX; Schema: portal; Owner: admin
--

CREATE INDEX idx_chat_history_session ON portal.chat_history USING btree (session_id);


--
-- Name: idx_projects_active; Type: INDEX; Schema: portal; Owner: admin
--

CREATE INDEX idx_projects_active ON portal.projects USING btree (created_at DESC) WHERE (is_active = true);


--
-- PostgreSQL database dump complete
--

\unrestrict gJfRDLxIGWa4fUWb5mOJEU8z3nAyZu8mHhInB8dlPhY8jStc34a0R2GcZsPXJdi

