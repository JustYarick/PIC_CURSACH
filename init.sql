-- ==========================================
-- МИНИМАЛЬНЫЙ НАБОР ТАБЛИЦ (10 штук)
-- ==========================================

-- 1. Клиенты
CREATE TABLE clients (
    client_id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    passport VARCHAR(20) UNIQUE NOT NULL,
    phone VARCHAR(20),
    email VARCHAR(100)
);

-- 2. Сотрудники  
CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    position VARCHAR(100) NOT NULL
);

-- 3. Филиалы
CREATE TABLE branches (
    branch_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    address VARCHAR(200)
);

-- 4. Роли пользователей
CREATE TABLE user_roles (
    role_id SERIAL PRIMARY KEY,
    role_name VARCHAR(50) UNIQUE NOT NULL
);

-- 5. Пользователи системы
CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    employee_id INTEGER REFERENCES employees(employee_id),
    role_id INTEGER REFERENCES user_roles(role_id),
    username VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL
);

-- 6. Типы депозитов
CREATE TABLE deposit_types (
    type_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    interest_rate DECIMAL(5,2) NOT NULL,
    min_amount DECIMAL(10,2) NOT NULL,
    term_days INTEGER NOT NULL
);

-- 7. Депозитные договоры
CREATE TABLE deposit_contracts (
    contract_id SERIAL PRIMARY KEY,
    client_id INTEGER REFERENCES clients(client_id),
    type_id INTEGER REFERENCES deposit_types(type_id),
    employee_id INTEGER REFERENCES employees(employee_id),
    branch_id INTEGER REFERENCES branches(branch_id),
    amount DECIMAL(10,2) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    status VARCHAR(20) DEFAULT 'ACTIVE'
);

-- 8. Операции по депозитам
CREATE TABLE deposit_operations (
    operation_id SERIAL PRIMARY KEY,
    contract_id INTEGER REFERENCES deposit_contracts(contract_id),
    operation_type VARCHAR(20) NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    operation_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 9. Начисление процентов
CREATE TABLE interest_accruals (
    accrual_id SERIAL PRIMARY KEY,
    contract_id INTEGER REFERENCES deposit_contracts(contract_id),
    amount DECIMAL(10,2) NOT NULL,
    accrual_date DATE NOT NULL
);

-- 10. Документы
CREATE TABLE documents (
    document_id SERIAL PRIMARY KEY,
    contract_id INTEGER REFERENCES deposit_contracts(contract_id),
    document_type VARCHAR(50) NOT NULL,
    file_path VARCHAR(500)
);

-- ==========================================
-- МИНИМАЛЬНЫЕ РОЛИ (4 штуки)
-- ==========================================

CREATE ROLE admin_role;
CREATE ROLE manager_role;  
CREATE ROLE operator_role;
CREATE ROLE viewer_role;

-- ==========================================
-- МИНИМАЛЬНЫЕ ПОЛЬЗОВАТЕЛИ (7 штук)
-- ==========================================

INSERT INTO user_roles (role_name) VALUES 
('admin'), ('manager'), ('operator'), ('viewer');

INSERT INTO employees (first_name, last_name, position) VALUES
('Иван', 'Иванов', 'Администратор'),
('Петр', 'Петров', 'Менеджер'),
('Сидор', 'Сидоров', 'Операционист'),
('Анна', 'Козлова', 'Менеджер'),
('Мария', 'Попова', 'Операционист'),
('Елена', 'Волкова', 'Консультант'),
('Олег', 'Смирнов', 'Операционист');

INSERT INTO users (employee_id, role_id, username, password_hash) VALUES
(1, 1, 'admin', 'hash1'),
(2, 2, 'manager1', 'hash2'),
(3, 3, 'operator1', 'hash3'),
(4, 2, 'manager2', 'hash4'),
(5, 3, 'operator2', 'hash5'),
(6, 4, 'viewer1', 'hash6'),
(7, 3, 'operator3', 'hash7');

-- ==========================================
-- МИНИМАЛЬНЫЕ ПРОЦЕДУРЫ (5 штук)
-- ==========================================

-- 1. Открытие депозита
CREATE OR REPLACE FUNCTION open_deposit(
    p_client_id INTEGER,
    p_type_id INTEGER,
    p_amount DECIMAL
) RETURNS INTEGER AS $$
DECLARE
    new_contract_id INTEGER;
BEGIN
    INSERT INTO deposit_contracts (client_id, type_id, amount, start_date, end_date, employee_id, branch_id)
    VALUES (p_client_id, p_type_id, p_amount, CURRENT_DATE, CURRENT_DATE + INTERVAL '1 year', 1, 1)
    RETURNING contract_id INTO new_contract_id;
    
    INSERT INTO deposit_operations (contract_id, operation_type, amount)
    VALUES (new_contract_id, 'OPEN', p_amount);
    
    RETURN new_contract_id;
END;
$$ LANGUAGE plpgsql;

-- 2. Начисление процентов
CREATE OR REPLACE FUNCTION calculate_interest(p_contract_id INTEGER) 
RETURNS DECIMAL AS $$
DECLARE
    contract_amount DECIMAL;
    interest_rate DECIMAL;
    interest_amount DECIMAL;
BEGIN
    SELECT dc.amount, dt.interest_rate 
    INTO contract_amount, interest_rate
    FROM deposit_contracts dc
    JOIN deposit_types dt ON dc.type_id = dt.type_id
    WHERE dc.contract_id = p_contract_id;
    
    interest_amount := contract_amount * interest_rate / 100 / 365;
    
    INSERT INTO interest_accruals (contract_id, amount, accrual_date)
    VALUES (p_contract_id, interest_amount, CURRENT_DATE);
    
    RETURN interest_amount;
END;
$$ LANGUAGE plpgsql;

-- 3. Закрытие депозита
CREATE OR REPLACE FUNCTION close_deposit(p_contract_id INTEGER)
RETURNS BOOLEAN AS $$
BEGIN
    UPDATE deposit_contracts 
    SET status = 'CLOSED' 
    WHERE contract_id = p_contract_id;
    
    INSERT INTO deposit_operations (contract_id, operation_type, amount)
    SELECT contract_id, 'CLOSE', amount 
    FROM deposit_contracts 
    WHERE contract_id = p_contract_id;
    
    RETURN TRUE;
END;
$$ LANGUAGE plpgsql;

-- 4. Получение баланса
CREATE OR REPLACE FUNCTION get_balance(p_contract_id INTEGER)
RETURNS DECIMAL AS $$
DECLARE
    total_balance DECIMAL := 0;
BEGIN
    SELECT COALESCE(SUM(
        CASE 
            WHEN operation_type IN ('OPEN', 'DEPOSIT') THEN amount
            WHEN operation_type = 'WITHDRAWAL' THEN -amount
            ELSE 0
        END
    ), 0) INTO total_balance
    FROM deposit_operations
    WHERE contract_id = p_contract_id;
    
    RETURN total_balance;
END;
$$ LANGUAGE plpgsql;

-- 5. Поиск клиента
CREATE OR REPLACE FUNCTION find_client(p_passport VARCHAR)
RETURNS TABLE(client_id INTEGER, full_name VARCHAR) AS $$
BEGIN
    RETURN QUERY
    SELECT c.client_id, c.first_name || ' ' || c.last_name
    FROM clients c
    WHERE c.passport = p_passport;
END;
$$ LANGUAGE plpgsql;

-- Таблица аудита
CREATE TABLE audit_log (
    log_id SERIAL PRIMARY KEY,
    table_name VARCHAR(50),
    operation VARCHAR(10),
    old_values TEXT,
    new_values TEXT,
    changed_by VARCHAR(50),
    changed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Функция логирования
CREATE OR REPLACE FUNCTION audit_trigger()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO audit_log (table_name, operation, old_values, new_values, changed_by)
    VALUES (TG_TABLE_NAME, TG_OP, OLD::TEXT, NEW::TEXT, USER);
    
    IF TG_OP = 'DELETE' THEN
        RETURN OLD;
    ELSE
        RETURN NEW;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Триггеры на основные таблицы
CREATE TRIGGER clients_audit AFTER INSERT OR UPDATE OR DELETE ON clients
    FOR EACH ROW EXECUTE FUNCTION audit_trigger();

CREATE TRIGGER contracts_audit AFTER INSERT OR UPDATE OR DELETE ON deposit_contracts
    FOR EACH ROW EXECUTE FUNCTION audit_trigger();

CREATE TRIGGER operations_audit AFTER INSERT OR UPDATE OR DELETE ON deposit_operations
    FOR EACH ROW EXECUTE FUNCTION audit_trigger();
