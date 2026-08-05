-- ============================================
-- Banco de Dados: Salão de Beleza
-- ============================================

CREATE DATABASE IF NOT EXISTS bdsalaodebeleza;
USE bdsalaodebeleza;

-- ============================================
-- TABELA: Profissional
-- ============================================
CREATE TABLE IF NOT EXISTS profissional (
    id_profissional INT PRIMARY KEY AUTO_INCREMENT,
    nome            VARCHAR(100) NOT NULL,
    especialidade   VARCHAR(80),
    cpf             CHAR(11) UNIQUE,
    foto            VARCHAR(255),
    ativo           TINYINT(1) DEFAULT 1
);

-- ============================================
-- TABELA: Servico
-- ============================================
CREATE TABLE IF NOT EXISTS servico (
    id_servico   INT PRIMARY KEY AUTO_INCREMENT,
    nome         VARCHAR(80) NOT NULL,
    preco        DECIMAL(8,2) NOT NULL,
    duracao_min  INT NOT NULL,
    ativo        TINYINT(1) DEFAULT 1
);

-- ============================================
-- TABELA: Cliente
-- ============================================
CREATE TABLE IF NOT EXISTS cliente (
    id_cliente      INT PRIMARY KEY AUTO_INCREMENT,
    nome            VARCHAR(100) NOT NULL,
    telefone        VARCHAR(15),
    data_nascimento DATE,
    ativo           TINYINT(1) DEFAULT 1
);

-- ============================================
-- TABELA: Produto
-- ============================================
CREATE TABLE IF NOT EXISTS produto (
    id_produto INT PRIMARY KEY AUTO_INCREMENT,
    nome       VARCHAR(100) NOT NULL,
    marca      VARCHAR(60),
    preco      DECIMAL(8,2) NOT NULL,
    estoque    INT NOT NULL,
    ativo      TINYINT(1) DEFAULT 1
);

-- ============================================
-- TABELA: Agendamento
-- ============================================
CREATE TABLE IF NOT EXISTS agendamento (
    id_agendamento  INT PRIMARY KEY AUTO_INCREMENT,
    data_hora       DATETIME NOT NULL,
	status          VARCHAR(20) NOT NULL DEFAULT 'agendado',
    id_cliente      INT NOT NULL,
    id_profissional INT NOT NULL,
    id_servico      INT NOT NULL,
    ativo           TINYINT(1) DEFAULT 1,
    CONSTRAINT fkAgendCliente      FOREIGN KEY (id_cliente)      REFERENCES cliente(id_cliente),
    CONSTRAINT fkAgendProfissional FOREIGN KEY (id_profissional) REFERENCES profissional(id_profissional),
    CONSTRAINT fkAgendServico      FOREIGN KEY (id_servico)      REFERENCES servico(id_servico)
);

-- ============================================
-- TABELA: Usuários (sistema de login)
-- ============================================
CREATE TABLE IF NOT EXISTS usuarios (
    id            INT PRIMARY KEY AUTO_INCREMENT,
    username      VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role          VARCHAR(30) NOT NULL,
    ativo         TINYINT(1) DEFAULT 1,
    criado_em     DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- DADOS INICIAIS
-- ============================================

-- Profissionais
INSERT INTO profissional (nome, especialidade, cpf, ativo) VALUES
('Ana Paula Souza',   'Cabeleireira',  '12345678901', 1),
('Bruno Lima',        'Barbeiro',      '98765432100', 1),
('Carla Mendes',      'Manicure',      '11122233344', 1),
('Diego Rocha',       'Colorista',     '55566677788', 1);

-- Serviços
INSERT INTO servico (nome, preco, duracao_min, ativo) VALUES
('Corte Feminino',   55.00,  60, 1),
('Corte Masculino',  35.00,  30, 1),
('Coloração',       150.00, 120, 1),
('Escova',           70.00,  60, 1),
('Manicure',         30.00,  40, 1),
('Pedicure',         35.00,  50, 1);

-- Clientes
INSERT INTO cliente (nome, telefone, data_nascimento, ativo) VALUES
('Juliana Ferreira', '11999990001', '1990-03-15', 1),
('Marcos Alves',     '11999990002', '1985-07-22', 1),
('Patricia Nunes',   '11999990003', '1995-11-08', 1);

-- Produtos
INSERT INTO produto (nome, marca, preco, estoque, ativo) VALUES
('Shampoo Hidratante 300ml', 'L\'Oréal',   25.90, 20, 1),
('Condicionador 300ml',      'L\'Oréal',   23.90, 18, 1),
('Tinta #5.0 Castanho',      'Igora',      42.00, 10, 1),
('Progressiva 1L',           'Keraton',   120.00,  5, 1),
('Esmalte Vermelho',         'Risqué',      8.50, 30, 1);

-- Agendamentos
INSERT INTO agendamento (data_hora, status, id_cliente, id_profissional, id_servico, ativo) VALUES
('2025-06-10 09:00:00', 'agendado',   1, 1, 1, 1),
('2025-06-10 10:00:00', 'agendado',   2, 2, 2, 1),
('2025-06-11 14:00:00', 'concluido',  3, 3, 5, 1),
('2025-06-12 11:00:00', 'cancelado',  1, 4, 3, 1);

-- Usuário Admin padrão (senha: admin123)
-- Hash BCrypt gerado para "admin123"
INSERT INTO usuarios (username, password_hash, role, ativo) VALUES
('admin', '$2a$11$K5KinrRi7eHV/R6D9JGn8.lCpBpQ7CaazGYMN5uqPbMkmOQwByJXK', 'Admin', 1),
('gerente', '$2a$11$K5KinrRi7eHV/R6D9JGn8.lCpBpQ7CaazGYMN5uqPbMkmOQwByJXK', 'Gerente', 1);

INSERT INTO usuarios (username, password_hash, role, ativo) 
VALUES ('admin', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcg7b3XeKeUxWdeS86E36P4/KFm', 'Admin', 1);


select * FROM ALL_TABLES;


-- ============================================================
-- INSERT CORRETO PARA USUÁRIO ADMIN
-- ============================================================

-- PRIMEIRO, LIMPE OS USUÁRIOS ANTIGOS (se necessário):
DELETE FROM usuarios WHERE username IN ('admin', 'gerente');

-- AGORA INSIRA O NOVO USUÁRIO COM A SENHA CORRETA:
-- Senha: admin123 (hash BCrypt)
INSERT INTO usuarios (username, password_hash, role, ativo) 
VALUES ('admin', '$2a$11$Q91fiPYPec73pUA4DKByXeSNOZ6TYn2ZY5jWSWpr57rkfUEyKjWq2', 'Admin', 1);

-- TAMBÉM INSIRA O USUÁRIO GERENTE:
INSERT INTO usuarios (username, password_hash, role, ativo) 
VALUES ('gerente', '$2a$11$Q91fiPYPec73pUA4DKByXeSNOZ6TYn2ZY5jWSWpr57rkfUEyKjWq2', 'Gerente', 1);

-- ============================================================
-- DADOS DE LOGIN:
-- ============================================================
-- Usuário: admin
-- Senha: admin123
-- Perfil: Admin
-- ============================================================

-- VERIFICAR SE FOI INSERIDO COM SUCESSO:
SELECT id, username, role, ativo FROM usuarios;