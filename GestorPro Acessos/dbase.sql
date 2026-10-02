CREATE TABLE usuarios (
    id SERIAL PRIMARY KEY,
    nome_completo VARCHAR(255) NOT NULL,
    nome_usuario VARCHAR(100) NOT NULL UNIQUE, -- Não poderá estar duplicado
    email VARCHAR(255) NOT NULL UNIQUE, -- Não poderá estar duplicado
    senha VARCHAR(255) NOT NULL, -- Armazenará o hash da palavra-passe
    tipo_usuario VARCHAR(50) NOT NULL DEFAULT 'Comum', -- 'Administrador' ou 'Comum'
    status_ativo BOOLEAN NOT NULL DEFAULT TRUE, -- Representa o status do utilizador (ativo/inativo)
    imagem_perfil VARCHAR(100) NOT NULL, -- Guardará a referência, ex: 'Avatar 01'
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_ultima_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ultimo_login TIMESTAMP,
    tentativas_invalidas INT DEFAULT 0, -- Controlador para bloqueio temporário
    bloqueado BOOLEAN DEFAULT FALSE -- Status de bloqueio por tentativas inválidas
);

CREATE TABLE auditoria (
    id SERIAL PRIMARY KEY,
    data_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    id_usuario_responsavel INT REFERENCES usuarios(id),
    tipo_operacao VARCHAR(100) NOT NULL, -- Ex: 'Alteração de perfil', 'Exclusão'
    registro_afetado VARCHAR(255) NOT NULL, -- Ex: 'Utilizador: joao'
    valor_anterior TEXT,
    novo_valor TEXT
);

CREATE TABLE eventos_autenticacao (
    id SERIAL PRIMARY KEY,
    data_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    nome_usuario_tentativa VARCHAR(255) NOT NULL, -- Guardado mesmo se o utilizador não existir
    id_usuario_relacionado INT REFERENCES usuarios(id), -- Pode ser NULL se o utilizador for inválido
    tipo_evento VARCHAR(100) NOT NULL, -- 'Login sucesso', 'Tentativa inválida', 'Bloqueio temporário'
    resultado_operacao VARCHAR(255) NOT NULL
);

DELETE FROM eventos_autenticacao 
WHERE id_usuario_relacionado = (SELECT id FROM usuarios WHERE nome_usuario = 'admin');

-- 2. Remove os registos de auditoria onde o utilizador foi o responsável
DELETE FROM auditoria 
WHERE id_usuario_responsavel = (SELECT id FROM usuarios WHERE nome_usuario = 'admin');

-- 3. Por fim, exclui o utilizador da tabela principal
DELETE FROM usuarios 
WHERE nome_usuario = 'admin';

UPDATE usuarios 
SET bloqueado = false, tentativas_invalidas = 0;

select now()

select clock_timestamp()

ALTER DATABASE postgres SET timezone TO 'UTC';

SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = 'postgres'
  AND pid <> pg_backend_pid();