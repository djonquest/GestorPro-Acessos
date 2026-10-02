--
-- PostgreSQL database dump
--

\restrict WzCDVXHIZeqC9QdBWbnJ2UZcK1NFucnsltHzLb9xtA6wwbn4sJiNNBBWpax06Kp

-- Dumped from database version 18.6
-- Dumped by pg_dump version 18.6

-- Started on 2026-09-24 21:43:04

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

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 222 (class 1259 OID 16449)
-- Name: auditoria; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.auditoria (
    id integer NOT NULL,
    data_hora timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    id_usuario_responsavel integer,
    tipo_operacao character varying(100) NOT NULL,
    registro_afetado character varying(255) NOT NULL,
    valor_anterior text,
    novo_valor text
);


ALTER TABLE public.auditoria OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16448)
-- Name: auditoria_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.auditoria_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.auditoria_id_seq OWNER TO postgres;

--
-- TOC entry 5047 (class 0 OID 0)
-- Dependencies: 221
-- Name: auditoria_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.auditoria_id_seq OWNED BY public.auditoria.id;


--
-- TOC entry 224 (class 1259 OID 16467)
-- Name: eventos_autenticacao; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.eventos_autenticacao (
    id integer NOT NULL,
    data_hora timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    nome_usuario_tentativa character varying(255) NOT NULL,
    id_usuario_relacionado integer,
    tipo_evento character varying(100) NOT NULL,
    resultado_operacao character varying(255) NOT NULL
);


ALTER TABLE public.eventos_autenticacao OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16466)
-- Name: eventos_autenticacao_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.eventos_autenticacao_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.eventos_autenticacao_id_seq OWNER TO postgres;

--
-- TOC entry 5048 (class 0 OID 0)
-- Dependencies: 223
-- Name: eventos_autenticacao_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.eventos_autenticacao_id_seq OWNED BY public.eventos_autenticacao.id;


--
-- TOC entry 220 (class 1259 OID 16421)
-- Name: usuarios; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.usuarios (
    id integer NOT NULL,
    nome_completo character varying(255) NOT NULL,
    nome_usuario character varying(100) NOT NULL,
    email character varying(255) NOT NULL,
    senha character varying(255) NOT NULL,
    tipo_usuario character varying(50) DEFAULT 'Comum'::character varying NOT NULL,
    status_ativo boolean DEFAULT true NOT NULL,
    imagem_perfil character varying(100) NOT NULL,
    data_criacao timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    data_ultima_alteracao timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    ultimo_login timestamp without time zone,
    tentativas_invalidas integer DEFAULT 0,
    bloqueado boolean DEFAULT false
);


ALTER TABLE public.usuarios OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16420)
-- Name: usuarios_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.usuarios_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.usuarios_id_seq OWNER TO postgres;

--
-- TOC entry 5049 (class 0 OID 0)
-- Dependencies: 219
-- Name: usuarios_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.usuarios_id_seq OWNED BY public.usuarios.id;


--
-- TOC entry 4873 (class 2604 OID 16452)
-- Name: auditoria id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.auditoria ALTER COLUMN id SET DEFAULT nextval('public.auditoria_id_seq'::regclass);


--
-- TOC entry 4875 (class 2604 OID 16470)
-- Name: eventos_autenticacao id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.eventos_autenticacao ALTER COLUMN id SET DEFAULT nextval('public.eventos_autenticacao_id_seq'::regclass);


--
-- TOC entry 4866 (class 2604 OID 16424)
-- Name: usuarios id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios ALTER COLUMN id SET DEFAULT nextval('public.usuarios_id_seq'::regclass);


--
-- TOC entry 5039 (class 0 OID 16449)
-- Dependencies: 222
-- Data for Name: auditoria; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.auditoria (id, data_hora, id_usuario_responsavel, tipo_operacao, registro_afetado, valor_anterior, novo_valor) FROM stdin;
4	2026-09-23 20:49:43.752904	2	Cadastro de usuário	Utilizador: admin	\N	Criação do primeiro administrador do sistema.
5	2026-09-23 20:58:47.926928	2	Cadastro de usuário	Utilizador: valdinei	\N	Conta criada
6	2026-09-23 21:00:05.341296	2	Alteração de usuário	Utilizador: lourdes	Dados antigos	Dados atualizados
7	2026-09-24 19:28:31.66841	2	Desbloqueio de usuário	Utilizador: lourdes	Bloqueado	Desbloqueado
8	2026-09-24 19:29:48.992632	2	Cadastro de usuário	Utilizador: mary	\N	Conta criada
9	2026-09-24 19:30:17.942217	2	Cadastro de usuário	Utilizador: carlos	\N	Conta criada
10	2026-09-24 19:30:50.659437	2	Cadastro de usuário	Utilizador: lorena	\N	Conta criada
11	2026-09-24 19:40:33.667082	5	Alteração de usuário	Utilizador: lourdes	Dados antigos	Dados atualizados
12	2026-09-24 19:41:00.108055	5	Redefinição de senha	Utilizador ID: 5	\N	Nova senha atribuída
13	2026-09-24 20:21:03.787784	6	Alteração de usuário	Utilizador: lorena	Dados antigos	Dados atualizados
14	2026-09-24 20:21:28.232059	6	Alteração de usuário	Utilizador: lorena	Dados antigos	Dados atualizados
\.


--
-- TOC entry 5041 (class 0 OID 16467)
-- Dependencies: 224
-- Data for Name: eventos_autenticacao; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.eventos_autenticacao (id, data_hora, nome_usuario_tentativa, id_usuario_relacionado, tipo_evento, resultado_operacao) FROM stdin;
3	2026-09-23 20:49:59.686678	admin	2	Login realizado com sucesso	Acesso permitido
4	2026-09-23 20:52:01.932787	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
5	2026-09-23 20:52:10.745099	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
6	2026-09-23 20:52:13.20129	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
7	2026-09-23 20:52:25.044138	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
8	2026-09-23 20:52:26.080904	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
9	2026-09-23 20:52:30.93318	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
10	2026-09-23 20:52:37.336485	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
11	2026-09-23 20:52:43.313536	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
12	2026-09-23 20:52:44.866306	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
13	2026-09-23 20:52:45.268604	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
14	2026-09-23 20:52:46.90413	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
15	2026-09-23 20:52:47.12912	valdinei	\N	Tentativa de login inválida	Utilizador inexistente
16	2026-09-23 20:53:46.740472	admin	2	Login realizado com sucesso	Acesso permitido
17	2026-09-23 20:58:16.570494	admin	2	Login realizado com sucesso	Acesso permitido
18	2026-09-23 21:02:07.85952	lourdes	5	Tentativa de login inválida	Palavra-passe incorreta
19	2026-09-23 21:02:11.783006	lourdes	5	Tentativa de login inválida	Palavra-passe incorreta
20	2026-09-23 21:02:17.348811	lourdes	5	Bloqueio temporário de conta	Múltiplas falhas - Conta bloqueada
21	2026-09-23 21:02:21.305558	lourdes	5	Tentativa de login inválida	Conta bloqueada temporariamente
22	2026-09-23 21:04:58.551888	lourdes	5	Tentativa de login inválida	Conta bloqueada temporariamente
23	2026-09-23 21:05:09.376981	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
24	2026-09-23 21:05:12.157771	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
25	2026-09-23 21:05:18.026335	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
26	2026-09-23 21:05:21.685825	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
27	2026-09-23 21:05:23.043229	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
28	2026-09-23 21:05:25.588644	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
29	2026-09-23 21:05:25.769848	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
30	2026-09-23 21:05:26.049339	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
31	2026-09-23 21:05:27.626939	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
32	2026-09-23 21:05:27.825386	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
33	2026-09-23 21:05:28.039631	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
34	2026-09-23 21:05:29.856111	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
35	2026-09-23 21:05:31.982784	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
36	2026-09-23 21:15:37.969653	lourdes	5	Tentativa de login inválida	Conta bloqueada temporariamente
37	2026-09-23 21:15:43.169686	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
38	2026-09-23 21:15:55.688005	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
39	2026-09-23 21:15:58.369505	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
40	2026-09-23 21:15:58.716092	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
41	2026-09-23 21:15:59.033421	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
42	2026-09-23 21:15:59.600744	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
43	2026-09-23 21:15:59.815276	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
44	2026-09-23 21:16:00.566245	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
45	2026-09-23 21:16:45.501585	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
46	2026-09-23 21:16:50.462384	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
47	2026-09-23 21:16:55.238966	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
48	2026-09-23 21:16:55.595828	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
49	2026-09-23 21:16:55.76724	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
50	2026-09-23 21:16:55.934306	admin	2	Tentativa de login inválida	Conta bloqueada temporariamente
51	2026-09-23 21:20:49.726671	lourdes	5	Login realizado com sucesso	Acesso permitido
52	2026-09-23 21:21:07.47585	lourdes	5	Tentativa de login inválida	Palavra-passe incorreta
53	2026-09-23 21:21:12.767223	lourdes	5	Tentativa de login inválida	Palavra-passe incorreta
54	2026-09-23 21:21:19.622034	lourdes	5	Bloqueio temporário de conta	Múltiplas falhas - Conta bloqueada
55	2026-09-23 21:21:31.572663	admin	2	Login realizado com sucesso	Acesso permitido
56	2026-09-23 21:28:37.520515	admin	2	Login realizado com sucesso	Acesso permitido
57	2026-09-24 19:19:38.729667	admin	2	Login realizado com sucesso	Acesso permitido
58	2026-09-24 19:21:34.754826	admin	2	Login realizado com sucesso	Acesso permitido
59	2026-09-24 19:28:09.887882	admin	2	Login realizado com sucesso	Acesso permitido
60	2026-09-24 19:32:16.88128	lourdes	5	Login realizado com sucesso	Acesso permitido
61	2026-09-24 19:40:06.097501	lourdes	5	Login realizado com sucesso	Acesso permitido
62	2026-09-24 19:41:21.958135	carlos	7	Login realizado com sucesso	Acesso permitido
63	2026-09-24 19:41:41.188865	lourdes	5	Login realizado com sucesso	Acesso permitido
64	2026-09-24 19:41:58.990883	mary	6	Login realizado com sucesso	Acesso permitido
65	2026-09-24 20:05:04.053288	carlos	7	Login realizado com sucesso	Acesso permitido
66	2026-09-24 20:05:17.935891	mary	6	Login realizado com sucesso	Acesso permitido
67	2026-09-24 20:09:22.387037	mary	6	Login realizado com sucesso	Acesso permitido
68	2026-09-24 20:10:53.49156	mary	6	Login realizado com sucesso	Acesso permitido
69	2026-09-24 20:13:44.236071	mary	6	Login realizado com sucesso	Acesso permitido
70	2026-09-24 20:15:05.068738	mary	6	Login realizado com sucesso	Acesso permitido
71	2026-09-24 20:20:45.037549	mary	6	Login realizado com sucesso	Acesso permitido
72	2026-09-24 23:45:51.759079	admin	2	Login realizado com sucesso	Acesso permitido
73	2026-09-25 00:14:29.828621	mary	6	Login realizado com sucesso	Acesso permitido
74	2026-09-25 00:28:19.686134	admin	2	Login realizado com sucesso	Acesso permitido
\.


--
-- TOC entry 5037 (class 0 OID 16421)
-- Dependencies: 220
-- Data for Name: usuarios; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.usuarios (id, nome_completo, nome_usuario, email, senha, tipo_usuario, status_ativo, imagem_perfil, data_criacao, data_ultima_alteracao, ultimo_login, tentativas_invalidas, bloqueado) FROM stdin;
5	lourdes	lourdes	lourdes@lourdes.com	$2a$11$FamU4UZqNzKhLvfk9eMbTeDgrAYey0vSrJbQ040srqfJdkvceclMq	Comum	t	avatar1.png	2026-09-23 20:58:47.926928	2026-09-23 20:58:47.926928	2026-09-24 19:41:41.185709	0	f
7	carlos	carlos	carlos@carlos.com	$2a$11$mXfdF3nkMhNbqY3jk9xU5eVD5VOtLpRi8PZStoJluztzarKa0XQrG	Comum	t	avatar3.png	2026-09-24 19:30:17.942217	2026-09-24 19:30:17.942217	2026-09-24 20:05:04.025232	0	f
8	lorena	lorena	lorena@lorena.com	$2a$11$myMTKSH9a3ZYF.4Y9Hyr1uXlQRjYpfXnGsX7dGtjEXOGrRFaTByzC	Comum	t	avatar4.png	2026-09-24 19:30:50.659437	2026-09-24 19:30:50.659437	\N	0	f
6	mary	mary	mary@mary.com	$2a$11$IbyDnFOfKvW1Qa.r9cJoM.oZoOt17/dV5wprDyLe5.ww0M3Eyu7/i	Administrador	t	avatar2.png	2026-09-24 19:29:48.992632	2026-09-24 19:29:48.992632	2026-09-25 00:14:29.812543	0	f
2	valdinei	admin	valdinei@valdinei.com.br	$2a$11$AARbwD3E7SIxmauAqdwFGOBMht5pD.bQdl5dY4Z0NXryCgJQmlRkO	Administrador	t	avatar5.png	2026-09-23 20:49:43.752904	2026-09-23 20:49:43.752904	2026-09-25 00:28:19.673137	0	f
\.


--
-- TOC entry 5050 (class 0 OID 0)
-- Dependencies: 221
-- Name: auditoria_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.auditoria_id_seq', 14, true);


--
-- TOC entry 5051 (class 0 OID 0)
-- Dependencies: 223
-- Name: eventos_autenticacao_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.eventos_autenticacao_id_seq', 74, true);


--
-- TOC entry 5052 (class 0 OID 0)
-- Dependencies: 219
-- Name: usuarios_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.usuarios_id_seq', 8, true);


--
-- TOC entry 4884 (class 2606 OID 16460)
-- Name: auditoria auditoria_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.auditoria
    ADD CONSTRAINT auditoria_pkey PRIMARY KEY (id);


--
-- TOC entry 4886 (class 2606 OID 16479)
-- Name: eventos_autenticacao eventos_autenticacao_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.eventos_autenticacao
    ADD CONSTRAINT eventos_autenticacao_pkey PRIMARY KEY (id);


--
-- TOC entry 4878 (class 2606 OID 16446)
-- Name: usuarios usuarios_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_email_key UNIQUE (email);


--
-- TOC entry 4880 (class 2606 OID 16444)
-- Name: usuarios usuarios_nome_usuario_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_nome_usuario_key UNIQUE (nome_usuario);


--
-- TOC entry 4882 (class 2606 OID 16442)
-- Name: usuarios usuarios_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_pkey PRIMARY KEY (id);


--
-- TOC entry 4887 (class 2606 OID 16461)
-- Name: auditoria auditoria_id_usuario_responsavel_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.auditoria
    ADD CONSTRAINT auditoria_id_usuario_responsavel_fkey FOREIGN KEY (id_usuario_responsavel) REFERENCES public.usuarios(id);


--
-- TOC entry 4888 (class 2606 OID 16480)
-- Name: eventos_autenticacao eventos_autenticacao_id_usuario_relacionado_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.eventos_autenticacao
    ADD CONSTRAINT eventos_autenticacao_id_usuario_relacionado_fkey FOREIGN KEY (id_usuario_relacionado) REFERENCES public.usuarios(id);


-- Completed on 2026-09-24 21:43:05

--
-- PostgreSQL database dump complete
--

\unrestrict WzCDVXHIZeqC9QdBWbnJ2UZcK1NFucnsltHzLb9xtA6wwbn4sJiNNBBWpax06Kp

