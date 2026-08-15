-- =====================================================================
-- 002 - Rename Portuguese table/column objects to English
--
-- Aligns the database with the English code contracts. All renames are
-- applied in-place, so existing data is preserved.
--
--   portal.projetos            -> portal.projects
--   portal.historico_conversas -> portal.chat_history
-- =====================================================================

-- ---------------------------------------------------------------------
-- portal.projetos -> portal.projects
-- ---------------------------------------------------------------------
ALTER TABLE portal.projetos RENAME TO projects;

ALTER TABLE portal.projects RENAME COLUMN titulo TO title;
ALTER TABLE portal.projects RENAME COLUMN descricao TO description;
ALTER TABLE portal.projects RENAME COLUMN tecnologias TO technologies;
ALTER TABLE portal.projects RENAME COLUMN url_imagem TO image_url;
ALTER TABLE portal.projects RENAME COLUMN url_github TO github_url;
ALTER TABLE portal.projects RENAME COLUMN url_demonstracao TO demo_url;
ALTER TABLE portal.projects RENAME COLUMN data_criacao TO created_at;
ALTER TABLE portal.projects RENAME COLUMN ativo TO is_active;

ALTER TABLE portal.projects RENAME CONSTRAINT projetos_pkey TO projects_pkey;
ALTER INDEX portal.idx_projetos_ativos RENAME TO idx_projects_active;
ALTER SEQUENCE portal.projetos_id_seq RENAME TO projects_id_seq;

-- ---------------------------------------------------------------------
-- portal.historico_conversas -> portal.chat_history
-- ---------------------------------------------------------------------
ALTER TABLE portal.historico_conversas RENAME TO chat_history;

ALTER TABLE portal.chat_history RENAME COLUMN sessao_id TO session_id;
ALTER TABLE portal.chat_history RENAME COLUMN pergunta_usuario TO user_question;
ALTER TABLE portal.chat_history RENAME COLUMN resposta_ia TO ai_response;
ALTER TABLE portal.chat_history RENAME COLUMN data_interacao TO interaction_date;

ALTER TABLE portal.chat_history RENAME CONSTRAINT historico_conversas_pkey TO chat_history_pkey;
ALTER INDEX portal.idx_conversas_sessao RENAME TO idx_chat_history_session;
ALTER SEQUENCE portal.historico_conversas_id_seq RENAME TO chat_history_id_seq;
