-- =====================================================================
-- SISTEMA DE BONIFICADOS - RECAMIER
-- ESTRUCTURAS DE BASE DE DATOS (VERSION RECOMENDADA v2)
-- =====================================================================
-- Basada en sqlboni.sql (se conservan sus convenciones) + documento
-- funcional "Sistema_Bonificados_Resumen_Tecnico.pptx":
--   * RF-01 Campañas (Activo/Inactivo/Borrador, duplicables)
--   * C-01..C-11 tipos de condición del motor de reglas
--   * RF-03 Eventos con QR (invitados, token, log de escaneos)
--   * V-08/C-05/C-08 control de consumo por cliente/periodo
--   * C-03 escalonamiento con tabla de niveles y multi-bonificado
--   * C-11 stock limitado con inactivación automática
--
-- Convenciones heredadas de la BD actual (eva_database):
--   * mob_r<entidad> catálogos / mob_<entidad> transaccionales
--   * prefijo de columnas por tabla + _cmpy CHAR(2) siempre primero
--   * estado_envio INTEGER DEFAULT 0 en tablas que sincroniza EVA
--   * llave de pedido igual a mob_rordhead/mob_rordline:
--     cmpy, reg, teri, num, entdt, cust, ship
--   * FKs a catálogos existentes: mob_rlinea, mob_rsublineas,
--     mob_rproductos, mob_rcustmain, mob_rinvclass, mob_rcusttype,
--     mob_rlistpric, mob_rusuarios, mob_rordhead
-- =====================================================================


-- =====================================================================
-- BLOQUE A. CATALOGOS Y CONFIGURACION (Módulo Web)
-- =====================================================================

-- A.1 Tipos de condición del motor (catálogo fijo C-01..C-11).
--     CAMBIO vs sqlboni: el catálogo deja de ser genérico (LINE_QTY...)
--     y pasa a ser el catálogo funcional del documento, con fase.
CREATE TABLE mob_rbontipos (
    rbt_cmpy    CHAR(2)   NOT NULL,
    rbt_tipo    CHAR(5)   NOT NULL,        -- 'C-01'..'C-11'
    rbt_descr   CHAR(100) NOT NULL,
    rbt_fase    NUMERIC(1) DEFAULT 1,      -- 1 = Fase I, 2 = Fase II
    rbt_activo  CHAR(1)   DEFAULT 'S',
    PRIMARY KEY (rbt_cmpy, rbt_tipo)
);

-- Semilla del catálogo (por compañía; ejemplo cmpy 'RE')
INSERT INTO mob_rbontipos VALUES ('RE','C-01','Conteo de unidades por agrupacion',1,'S');
INSERT INTO mob_rbontipos VALUES ('RE','C-02','Monto minimo de compra por agrupacion',1,'S');
INSERT INTO mob_rbontipos VALUES ('RE','C-03','Escalonamiento / multiplos por niveles',1,'S');
INSERT INTO mob_rbontipos VALUES ('RE','C-04','Cliente no comprador (rango de dias)',1,'S');
INSERT INTO mob_rbontipos VALUES ('RE','C-05','Oferta 1 vez por mes por cliente',1,'S');
INSERT INTO mob_rbontipos VALUES ('RE','C-06','Categoria de cliente especifica',2,'S');
INSERT INTO mob_rbontipos VALUES ('RE','C-07','Participacion con QR de evento',2,'S');
INSERT INTO mob_rbontipos VALUES ('RE','C-08','Solo primer pedido (cliente nuevo)',2,'S');
INSERT INTO mob_rbontipos VALUES ('RE','C-09','Precio minimo del producto bonificado',2,'S');
INSERT INTO mob_rbontipos VALUES ('RE','C-10','Producto foco obligatorio',2,'S');
INSERT INTO mob_rbontipos VALUES ('RE','C-11','Stock limitado / hasta agotar',2,'S');


-- A.2 CAMPAÑAS (NUEVA - RF-01). Agrupa reglas, define vigencia mensual
--     y permite duplicar: clonar campaña = clonar sus reglas hijas.
CREATE TABLE mob_rboncampana (
    cmp_cmpy        CHAR(2)   NOT NULL,
    cmp_campana     CHAR(15)  NOT NULL,       -- código (ej. ABR2026-SALON)
    cmp_descr       CHAR(150) NOT NULL,
    cmp_fecini      DATETIME  NOT NULL,       -- V-01 vigencia
    cmp_fecfin      DATETIME  NOT NULL,
    cmp_estado      CHAR(1)   DEFAULT 'B',    -- A=Activo, I=Inactivo, B=Borrador
    cmp_origen      CHAR(15),                 -- campaña de la que se duplicó (NULL si es original)
    cmp_codusu_crea CHAR(10),
    cmp_fecha_crea  DATETIME  DEFAULT CURRENT_TIMESTAMP,
    cmp_codusu_mod  CHAR(10),
    cmp_fecha_mod   DATETIME,
    estado_envio    INTEGER   DEFAULT 0,
    PRIMARY KEY (cmp_cmpy, cmp_campana)
);


-- A.3 GRUPOS DE PRODUCTOS (NUEVA - C-01 "agrupaciones mixtas").
--     El catálogo real mezcla líneas, sub-líneas y referencias sueltas
--     en un mismo grupo (ej. grupo "tintes" = TechniTone 750301 +
--     Green Forest 730750302 + Gloss 75030). Reutilizable entre reglas
--     y entre campañas; el conteo/monto se suma a nivel de grupo.
CREATE TABLE mob_rbongrupo (
    grp_cmpy    CHAR(2)   NOT NULL,
    grp_grupo   CHAR(15)  NOT NULL,           -- ej. TINTES, DECO500, VEGAN
    grp_descr   CHAR(100) NOT NULL,
    grp_activo  CHAR(1)   DEFAULT 'S',
    estado_envio INTEGER  DEFAULT 0,
    PRIMARY KEY (grp_cmpy, grp_grupo)
);

CREATE TABLE mob_rbongrupodet (
    grd_cmpy     CHAR(2)  NOT NULL,
    grd_grupo    CHAR(15) NOT NULL,
    grd_seq      NUMERIC(3) NOT NULL,
    grd_nivel    CHAR(10) NOT NULL,   -- LINEA, SUBLINEA, REFERENCIA, CATEGORIA
    grd_valor    CHAR(21) NOT NULL,   -- rln_linea / rsb_sublineaid / rpr_id / categoría
    PRIMARY KEY (grd_cmpy, grd_grupo, grd_seq),
    FOREIGN KEY (grd_cmpy, grd_grupo) REFERENCES mob_rbongrupo(grp_cmpy, grp_grupo)
);


-- A.4 REGLAS. CAMBIOS vs sqlboni:
--     * FK a campaña (la vigencia base la da la campaña; fecini/fecfin
--       propios son opcionales para acotar dentro de la campaña)
--     * rbr_tipo ahora es el tipo principal C-01..C-11
--     * Modificadores del documento como columnas explícitas:
--       frecuencia (C-05), rango días no comprador (C-04), categoría de
--       cliente (C-06), requiere QR (C-07), solo primer pedido (C-08),
--       precio mínimo bonificado (C-09), stock limitado (C-11)
CREATE TABLE mob_rbonreglas (
    rbr_cmpy        CHAR(2)   NOT NULL,
    rbr_regla       CHAR(15)  NOT NULL,
    rbr_campana     CHAR(15)  NOT NULL,       -- FK -> mob_rboncampana
    rbr_descr       CHAR(150) NOT NULL,
    rbr_tipo        CHAR(5)   NOT NULL,       -- FK -> mob_rbontipos (C-01..C-11)
    rbr_prioridad   NUMERIC(3) DEFAULT 1,
    rbr_acumulable  CHAR(1)   DEFAULT 'N',    -- S/N combinable con otras reglas
    rbr_activo      CHAR(1)   DEFAULT 'S',
    rbr_fecini      DATETIME,                 -- opcional, acota dentro de la campaña
    rbr_fecfin      DATETIME,
    -- ---- modificadores funcionales ----
    rbr_frecuencia  CHAR(1)   DEFAULT 'N',    -- N=sin límite, M=1 vez/mes (C-05), U=1 vez/cliente (C-08 kits, prepack academias)
    rbr_maxclimes   NUMERIC(5),               -- V-08 tope de aplicaciones por cliente en el periodo (NULL = sin tope)
    rbr_diasmin     NUMERIC(5),               -- C-04 días sin compra desde (0, 90...)
    rbr_diasmax     NUMERIC(5),               -- C-04 días sin compra hasta (90, 180...)
    rbr_categcli    CHAR(20),                 -- C-06 FK lógica -> mob_rcusttype.cu_custype (NULL = todas)
    rbr_reqqr       CHAR(1)   DEFAULT 'N',    -- C-07 requiere escaneo QR de evento (V-09)
    rbr_soloprimped CHAR(1)   DEFAULT 'N',    -- C-08 solo primer pedido / cliente nuevo (V-10)
    rbr_preciominbon NUMERIC(16,2),           -- C-09 precio mínimo del bonificado (V-07)
    rbr_stocklimit  CHAR(1)   DEFAULT 'N',    -- C-11 'S' = se inactiva al agotar stock asignado (V-06)
    -- ---- auditoría de configuración ----
    rbr_codusu_crea CHAR(10),
    rbr_fecha_crea  DATETIME  DEFAULT CURRENT_TIMESTAMP,
    rbr_codusu_mod  CHAR(10),
    rbr_fecha_mod   DATETIME,
    estado_envio    INTEGER   DEFAULT 0,
    PRIMARY KEY (rbr_cmpy, rbr_regla),
    FOREIGN KEY (rbr_cmpy, rbr_campana) REFERENCES mob_rboncampana(cmp_cmpy, cmp_campana),
    FOREIGN KEY (rbr_cmpy, rbr_tipo)    REFERENCES mob_rbontipos(rbt_cmpy, rbt_tipo)
);


-- A.5 Alcance / segmentación (IGUAL que sqlboni; se agrega REGION y
--     TERRITORIO porque V-02 valida canal/región/territorio).
CREATE TABLE mob_rbonalcance (
    rbe_cmpy         CHAR(2)  NOT NULL,
    rbe_regla        CHAR(15) NOT NULL,
    rbe_tipoalcance  CHAR(15) NOT NULL,  -- CANAL, REGION, TERRITORIO, ZONA, TIPOCLIENTE, CLIENTE, LISTAPRECIO
    rbe_valor        CHAR(20) NOT NULL,
    PRIMARY KEY (rbe_cmpy, rbe_regla, rbe_tipoalcance, rbe_valor),
    FOREIGN KEY (rbe_cmpy, rbe_regla) REFERENCES mob_rbonreglas(rbr_cmpy, rbr_regla)
);


-- A.6 Condiciones de compra. CAMBIOS vs sqlboni:
--     * rbc_grupo referencia grupos mixtos (V-03/V-04)
--     * rbc_operador define cómo se combinan condiciones (AND / OR)
--     * rbc_foco marca la condición de producto foco (C-10)
CREATE TABLE mob_rbonregcond (
    rbc_cmpy        CHAR(2)   NOT NULL,
    rbc_regla       CHAR(15)  NOT NULL,
    rbc_seq         NUMERIC(3) NOT NULL,
    rbc_operador    CHAR(3)   DEFAULT 'AND', -- combinación con la condición siguiente
    rbc_grupo       CHAR(15),                -- FK -> mob_rbongrupo (agrupación mixta)
    rbc_linea       CHAR(10),                -- alternativa simple: FK -> mob_rlinea
    rbc_subl        CHAR(10),                -- FK -> mob_rsublineas
    rbc_id          CHAR(21),                -- FK -> mob_rproductos (referencia puntual)
    rbc_cantmin     NUMERIC(9,2),            -- C-01 / V-05 unidades mínimas
    rbc_multiplo    NUMERIC(9,2),            -- C-03 complementaria: "por cada N adicionales"
    rbc_valmin      NUMERIC(16,2),           -- C-02 / V-05 monto mínimo (IVA incl.)
    rbc_foco        CHAR(1)   DEFAULT 'N',   -- C-10 'S' = producto foco obligatorio
    PRIMARY KEY (rbc_cmpy, rbc_regla, rbc_seq),
    FOREIGN KEY (rbc_cmpy, rbc_regla) REFERENCES mob_rbonreglas(rbr_cmpy, rbr_regla),
    FOREIGN KEY (rbc_cmpy, rbc_grupo) REFERENCES mob_rbongrupo(grp_cmpy, grp_grupo)
);


-- A.7 ESCALONAMIENTO (NUEVAS - C-03). sqlboni no podía expresar la
--     tabla de niveles del catálogo (Nv.1: 6-11 tintes -> 1 tinte +
--     7 Rev 90ml; Nv.2: 12-23 -> 4 tintes + 16 Rev 90 + 1 Rev 900...).
--     Cada nivel entrega VARIOS bonificados distintos.
CREATE TABLE mob_rbonnivel (
    rbn_cmpy    CHAR(2)   NOT NULL,
    rbn_regla   CHAR(15)  NOT NULL,
    rbn_nivel   NUMERIC(3) NOT NULL,          -- 1..n
    rbn_descr   CHAR(100),                    -- "Nv.2 - 1 docena"
    rbn_desde   NUMERIC(9,2) NOT NULL,        -- unidades (o monto) desde
    rbn_hasta   NUMERIC(9,2),                 -- NULL = infinito
    PRIMARY KEY (rbn_cmpy, rbn_regla, rbn_nivel),
    FOREIGN KEY (rbn_cmpy, rbn_regla) REFERENCES mob_rbonreglas(rbr_cmpy, rbr_regla)
);

CREATE TABLE mob_rbonnivelbenef (
    rnb_cmpy    CHAR(2)   NOT NULL,
    rnb_regla   CHAR(15)  NOT NULL,
    rnb_nivel   NUMERIC(3) NOT NULL,
    rnb_seq     NUMERIC(3) NOT NULL,
    rnb_id      CHAR(21),                     -- FK -> mob_rproductos (bonificado del nivel)
    rnb_grupo   CHAR(15),                     -- o grupo (ej. "1 tinte del grupo TINTES")
    rnb_cant    NUMERIC(9,2) NOT NULL,        -- cantidad fija del nivel
    rnb_formula CHAR(15),                     -- NULL = cantidad fija; 'IGUAL_TOTAL' = igual al total comprado (Ttos Sachet = total tintes)
    PRIMARY KEY (rnb_cmpy, rnb_regla, rnb_nivel, rnb_seq),
    FOREIGN KEY (rnb_cmpy, rnb_regla, rnb_nivel) REFERENCES mob_rbonnivel(rbn_cmpy, rbn_regla, rbn_nivel)
);


-- A.8 Beneficios de reglas NO escalonadas (IGUAL que sqlboni, con
--     grupo para "el de menor valor dentro del grupo").
CREATE TABLE mob_rbonregbenef (
    rbb_cmpy       CHAR(2)   NOT NULL,
    rbb_regla      CHAR(15)  NOT NULL,
    rbb_seq        NUMERIC(3) NOT NULL,
    rbb_tipobenef  CHAR(20)  NOT NULL,   -- SAME_PRODUCT, SPECIFIC_PRODUCT, LOWEST_VALUE, MULTIPLE_OPTION
    rbb_cant       NUMERIC(9,2) DEFAULT 1,
    rbb_id         CHAR(21),             -- FK -> mob_rproductos
    rbb_grupo      CHAR(15),             -- FK -> mob_rbongrupo (para LOWEST_VALUE dentro del grupo)
    rbb_multiopc   CHAR(1) DEFAULT 'N',
    rbb_descr      CHAR(200),
    PRIMARY KEY (rbb_cmpy, rbb_regla, rbb_seq),
    FOREIGN KEY (rbb_cmpy, rbb_regla) REFERENCES mob_rbonreglas(rbr_cmpy, rbr_regla),
    FOREIGN KEY (rbb_cmpy, rbb_grupo) REFERENCES mob_rbongrupo(grp_cmpy, grp_grupo)
);

-- Opciones múltiples de un beneficio (IGUAL que sqlboni)
CREATE TABLE mob_rbonbenefopc (
    rbo_cmpy      CHAR(2)   NOT NULL,
    rbo_regla     CHAR(15)  NOT NULL,
    rbo_seqbenef  NUMERIC(3) NOT NULL,
    rbo_orden     NUMERIC(3) NOT NULL,
    rbo_id        CHAR(21)  NOT NULL,
    rbo_cant      NUMERIC(9,2) DEFAULT 1,
    rbo_descr     CHAR(200),
    PRIMARY KEY (rbo_cmpy, rbo_regla, rbo_seqbenef, rbo_orden),
    FOREIGN KEY (rbo_cmpy, rbo_regla, rbo_seqbenef) REFERENCES mob_rbonregbenef(rbb_cmpy, rbb_regla, rbb_seq)
);


-- A.9 STOCK ASIGNADO POR REGLA (NUEVA - C-11 / V-06 / R6).
--     Cupo de unidades bonificables por regla-producto. El motor
--     descuenta al aplicar; cuando consumido >= asignado la regla se
--     inactiva automáticamente ("hasta agotar stock"). Complementa la
--     validación de inventario real (mob existente) exigida por V-06.
CREATE TABLE mob_rbonstock (
    rbs_cmpy      CHAR(2)   NOT NULL,
    rbs_regla     CHAR(15)  NOT NULL,
    rbs_id        CHAR(21)  NOT NULL,     -- producto bonificado
    rbs_asignado  NUMERIC(9) NOT NULL,
    rbs_consumido NUMERIC(9) DEFAULT 0,
    rbs_fecha_mod DATETIME,
    estado_envio  INTEGER   DEFAULT 0,
    PRIMARY KEY (rbs_cmpy, rbs_regla, rbs_id),
    FOREIGN KEY (rbs_cmpy, rbs_regla) REFERENCES mob_rbonreglas(rbr_cmpy, rbr_regla)
);


-- =====================================================================
-- BLOQUE B. EVENTOS CON QR (NUEVAS - RF-03 / C-07 / V-09)
-- =====================================================================

CREATE TABLE mob_rbonevento (
    evt_cmpy      CHAR(2)   NOT NULL,
    evt_evento    CHAR(15)  NOT NULL,
    evt_descr     CHAR(150) NOT NULL,
    evt_regla     CHAR(15)  NOT NULL,     -- regla que habilita (rbr_reqqr='S')
    evt_fecini    DATETIME  NOT NULL,     -- ventana fecha/hora del evento
    evt_fecfin    DATETIME  NOT NULL,
    evt_token     CHAR(64)  NOT NULL,     -- token de seguridad embebido en el QR
    evt_estado    CHAR(1)   DEFAULT 'A',  -- A=Activo, C=Cerrado, X=Cancelado
    evt_codusu_crea CHAR(10),
    evt_fecha_crea DATETIME DEFAULT CURRENT_TIMESTAMP,
    estado_envio  INTEGER   DEFAULT 0,
    PRIMARY KEY (evt_cmpy, evt_evento),
    FOREIGN KEY (evt_cmpy, evt_regla) REFERENCES mob_rbonreglas(rbr_cmpy, rbr_regla)
);

-- Invitados (segmento o lista manual de clientes)
CREATE TABLE mob_rboneventoinv (
    evi_cmpy    CHAR(2)  NOT NULL,
    evi_evento  CHAR(15) NOT NULL,
    evi_cust    CHAR(15) NOT NULL,        -- FK lógica -> mob_rcustmain.rctt_cust
    evi_ship    CHAR(15),
    PRIMARY KEY (evi_cmpy, evi_evento, evi_cust),
    FOREIGN KEY (evi_cmpy, evi_evento) REFERENCES mob_rbonevento(evt_cmpy, evt_evento)
);

-- Log de escaneos QR (consultable por supervisor; habilita C-07 y
-- soporta el rechazo de QR vencido o de otro evento)
CREATE TABLE mob_rboneventoscan (
    evs_cmpy     CHAR(2)   NOT NULL,
    evs_evento   CHAR(15)  NOT NULL,
    evs_cust     CHAR(15)  NOT NULL,
    evs_codusu   CHAR(10)  NOT NULL,      -- vendedor que escaneó
    evs_fecha    DATETIME  DEFAULT CURRENT_TIMESTAMP,
    evs_resultado CHAR(1)  DEFAULT 'V',   -- V=Válido, X=Vencido, E=Evento distinto, N=No invitado
    evs_num      NUMERIC(12),             -- pedido asociado, si aplica
    estado_envio INTEGER   DEFAULT 0,
    PRIMARY KEY (evs_cmpy, evs_evento, evs_cust, evs_fecha),
    FOREIGN KEY (evs_cmpy, evs_evento) REFERENCES mob_rbonevento(evt_cmpy, evt_evento)
);


-- =====================================================================
-- BLOQUE C. EJECUCION / APLICACION EN PEDIDO (API + App EVA)
-- =====================================================================

-- C.1 Bonificaciones aplicadas por pedido (BASE sqlboni + vendedor y
--     nivel alcanzado, exigidos por la trazabilidad del documento:
--     "fecha, cliente, campaña, regla, vendedor, cantidad, valor")
CREATE TABLE mob_rordbonif (
    orb_cmpy      CHAR(2)   NOT NULL,
    orb_reg       CHAR(5)   NOT NULL,
    orb_teri      CHAR(5)   NOT NULL,
    orb_num       NUMERIC(12) NOT NULL,
    orb_entdt     DATETIME  NOT NULL,
    orb_cust      CHAR(15)  NOT NULL,
    orb_ship      CHAR(15)  NOT NULL,
    orb_regla     CHAR(15)  NOT NULL,
    orb_campana   CHAR(15),               -- redundante a propósito: reporte directo por campaña
    orb_codusu    CHAR(10),               -- NUEVO: vendedor que aplicó
    orb_nivel     NUMERIC(3),             -- NUEVO: nivel de escalonamiento alcanzado (C-03)
    orb_descr     CHAR(200),
    orb_totitems  NUMERIC(9)  DEFAULT 0,
    orb_totvalor  NUMERIC(16,2) DEFAULT 0,
    orb_fecha     DATETIME  DEFAULT CURRENT_TIMESTAMP,
    estado_envio  INTEGER   DEFAULT 0,
    PRIMARY KEY (orb_cmpy, orb_reg, orb_teri, orb_num, orb_entdt, orb_cust, orb_ship, orb_regla),
    FOREIGN KEY (orb_cmpy, orb_reg, orb_teri, orb_num, orb_entdt, orb_cust, orb_ship)
        REFERENCES mob_rordhead(orh_cmpy, orh_reg, orh_teri, orh_num, orh_entdt, orh_cust, orh_ship),
    FOREIGN KEY (orb_cmpy, orb_regla) REFERENCES mob_rbonreglas(rbr_cmpy, rbr_regla)
);

-- C.2 Detalle de ítems bonificados (IGUAL que sqlboni)
CREATE TABLE mob_rordbonline (
    orbl_cmpy       CHAR(2)   NOT NULL,
    orbl_reg        CHAR(5)   NOT NULL,
    orbl_teri       CHAR(5)   NOT NULL,
    orbl_num        NUMERIC(12) NOT NULL,
    orbl_entdt      DATETIME  NOT NULL,
    orbl_cust       CHAR(15)  NOT NULL,
    orbl_ship       CHAR(15)  NOT NULL,
    orbl_regla      CHAR(15)  NOT NULL,
    orbl_part       CHAR(21)  NOT NULL,
    orbl_partorigen CHAR(21),
    orbl_cant       NUMERIC(16,4) NOT NULL,
    orbl_preciolist NUMERIC(16,2),
    orbl_valorbonif NUMERIC(16,2),
    orbl_estado     CHAR(1)   DEFAULT 'P',  -- P=Pendiente, C=Confirmada, D=Entregada, R=Rechazada
    estado_envio    INTEGER   DEFAULT 0,
    PRIMARY KEY (orbl_cmpy, orbl_reg, orbl_teri, orbl_num, orbl_entdt, orbl_cust, orbl_ship, orbl_regla, orbl_part),
    FOREIGN KEY (orbl_cmpy, orbl_reg, orbl_teri, orbl_num, orbl_entdt, orbl_cust, orbl_ship, orbl_regla)
        REFERENCES mob_rordbonif(orb_cmpy, orb_reg, orb_teri, orb_num, orb_entdt, orb_cust, orb_ship, orb_regla)
);

-- C.3 Selección de opción múltiple (IGUAL que sqlboni)
CREATE TABLE mob_rbonopcsel (
    obs_cmpy      CHAR(2)   NOT NULL,
    obs_reg       CHAR(5)   NOT NULL,
    obs_teri      CHAR(5)   NOT NULL,
    obs_num       NUMERIC(12) NOT NULL,
    obs_entdt     DATETIME  NOT NULL,
    obs_cust      CHAR(15)  NOT NULL,
    obs_ship      CHAR(15)  NOT NULL,
    obs_regla     CHAR(15)  NOT NULL,
    obs_seqbenef  NUMERIC(3) NOT NULL,
    obs_orden     NUMERIC(3) NOT NULL,
    obs_codusu    CHAR(10),
    obs_fecha     DATETIME  DEFAULT CURRENT_TIMESTAMP,
    estado_envio  INTEGER   DEFAULT 0,
    PRIMARY KEY (obs_cmpy, obs_reg, obs_teri, obs_num, obs_entdt, obs_cust, obs_ship, obs_regla, obs_seqbenef),
    FOREIGN KEY (obs_cmpy, obs_regla, obs_seqbenef, obs_orden)
        REFERENCES mob_rbonbenefopc(rbo_cmpy, rbo_regla, rbo_seqbenef, rbo_orden)
);

-- C.4 CONSUMO POR CLIENTE / PERIODO (NUEVA - C-05, C-08, V-08).
--     Sin esta tabla no se puede validar "1 vez/mes", "1 vez por
--     cliente" ni el tope mensual. Se actualiza al confirmar el pedido
--     y se sincroniza a EVA para validar offline con la última sync.
CREATE TABLE mob_rbonconsumo (
    cns_cmpy     CHAR(2)   NOT NULL,
    cns_regla    CHAR(15)  NOT NULL,
    cns_cust     CHAR(15)  NOT NULL,
    cns_periodo  CHAR(6)   NOT NULL,     -- AAAAMM ('000000' para frecuencia U = única vez histórica)
    cns_veces    NUMERIC(5) DEFAULT 0,
    cns_ultfecha DATETIME,
    cns_ultnum   NUMERIC(12),            -- último pedido que consumió
    estado_envio INTEGER   DEFAULT 0,
    PRIMARY KEY (cns_cmpy, cns_regla, cns_cust, cns_periodo),
    FOREIGN KEY (cns_cmpy, cns_regla) REFERENCES mob_rbonreglas(rbr_cmpy, rbr_regla)
);


-- =====================================================================
-- BLOQUE D. AUDITORIA (BASE sqlboni + resultado de validación)
-- =====================================================================

CREATE TABLE mob_rbonaudit (
    rba_cmpy          CHAR(2)   NOT NULL,
    rba_consecutivo   NUMERIC(12) NOT NULL,  -- mob_rconsecutivos, rcons_coddoc = 'BA'
    rba_regla         CHAR(15),
    rba_campana       CHAR(15),
    rba_num           NUMERIC(12),
    rba_cust          CHAR(15),
    rba_accion        CHAR(20)  NOT NULL,  -- CREADA, MODIFICADA, DESACTIVADA, DUPLICADA, APLICADA, REMOVIDA, RECHAZADA, AGOTADA
    rba_validacion    CHAR(5),             -- NUEVO: V-01..V-10 que rechazó (si acción = RECHAZADA)
    rba_valoranterior TEXT,
    rba_valornuevo    TEXT,
    rba_codusu        CHAR(10),
    rba_fecha         DATETIME DEFAULT CURRENT_TIMESTAMP,
    rba_obs           TEXT,
    PRIMARY KEY (rba_cmpy, rba_consecutivo)
);


-- =====================================================================
-- BLOQUE E. INDICES DE SOPORTE
-- =====================================================================

CREATE INDEX IX_rboncampana_estado  ON mob_rboncampana(cmp_cmpy, cmp_estado, cmp_fecini, cmp_fecfin);
CREATE INDEX IX_rbonreglas_campana  ON mob_rbonreglas(rbr_cmpy, rbr_campana, rbr_activo);
CREATE INDEX IX_rbonreglas_tipo     ON mob_rbonreglas(rbr_cmpy, rbr_tipo);
CREATE INDEX IX_rbonregcond_regla   ON mob_rbonregcond(rbc_cmpy, rbc_regla);
CREATE INDEX IX_rbonregcond_grupo   ON mob_rbonregcond(rbc_cmpy, rbc_grupo);
CREATE INDEX IX_rbongrupodet_valor  ON mob_rbongrupodet(grd_cmpy, grd_nivel, grd_valor);
CREATE INDEX IX_rbonalcance_valor   ON mob_rbonalcance(rbe_cmpy, rbe_tipoalcance, rbe_valor);
CREATE INDEX IX_rbonnivel_regla     ON mob_rbonnivel(rbn_cmpy, rbn_regla, rbn_desde);
CREATE INDEX IX_rbonstock_regla     ON mob_rbonstock(rbs_cmpy, rbs_regla);
CREATE INDEX IX_rbonevento_regla    ON mob_rbonevento(evt_cmpy, evt_regla, evt_fecini, evt_fecfin);
CREATE INDEX IX_rboneventoscan_cust ON mob_rboneventoscan(evs_cmpy, evs_cust, evs_fecha);
CREATE INDEX IX_rordbonif_pedido    ON mob_rordbonif(orb_cmpy, orb_reg, orb_teri, orb_num, orb_entdt);
CREATE INDEX IX_rordbonif_campana   ON mob_rordbonif(orb_cmpy, orb_campana, orb_fecha);
CREATE INDEX IX_rordbonline_estado  ON mob_rordbonline(orbl_cmpy, orbl_estado);
CREATE INDEX IX_rbonconsumo_cliente ON mob_rbonconsumo(cns_cmpy, cns_cust, cns_periodo);
CREATE INDEX IX_rbonaudit_regla     ON mob_rbonaudit(rba_cmpy, rba_regla);
CREATE INDEX IX_rbonaudit_pedido    ON mob_rbonaudit(rba_cmpy, rba_num);


-- =====================================================================
-- BLOQUE F. VISTAS DE APOYO
-- =====================================================================

-- Reglas activas y vigentes (ahora la vigencia sale de la CAMPAÑA
-- activa; la regla puede acotar con sus propias fechas)
CREATE VIEW vw_rbonreglas_activas AS
SELECT
    rbr.rbr_cmpy, rbr.rbr_regla, rbr.rbr_campana, cmp.cmp_descr AS campana_descr,
    rbr.rbr_descr, rbr.rbr_tipo, rbt.rbt_descr AS tipo_descr, rbt.rbt_fase,
    rbr.rbr_prioridad, rbr.rbr_acumulable, rbr.rbr_frecuencia, rbr.rbr_maxclimes,
    rbr.rbr_reqqr, rbr.rbr_soloprimped, rbr.rbr_preciominbon, rbr.rbr_stocklimit,
    cmp.cmp_fecini, cmp.cmp_fecfin
FROM mob_rbonreglas rbr
INNER JOIN mob_rboncampana cmp
    ON rbr.rbr_cmpy = cmp.cmp_cmpy AND rbr.rbr_campana = cmp.cmp_campana
INNER JOIN mob_rbontipos rbt
    ON rbr.rbr_cmpy = rbt.rbt_cmpy AND rbr.rbr_tipo = rbt.rbt_tipo
WHERE cmp.cmp_estado = 'A'
  AND rbr.rbr_activo = 'S'
  AND cmp.cmp_fecini <= CURRENT_TIMESTAMP
  AND cmp.cmp_fecfin >= CURRENT_TIMESTAMP
  AND (rbr.rbr_fecini IS NULL OR rbr.rbr_fecini <= CURRENT_TIMESTAMP)
  AND (rbr.rbr_fecfin IS NULL OR rbr.rbr_fecfin >= CURRENT_TIMESTAMP)
  AND (rbr.rbr_stocklimit = 'N' OR EXISTS (
        SELECT 1 FROM mob_rbonstock rbs
        WHERE rbs.rbs_cmpy = rbr.rbr_cmpy AND rbs.rbs_regla = rbr.rbr_regla
          AND rbs.rbs_consumido < rbs.rbs_asignado));

-- Bonificaciones por pedido con cliente y campaña (reportería / IBES)
CREATE VIEW vw_rordbonif_resumen AS
SELECT
    orb.orb_cmpy, orb.orb_num, orb.orb_entdt, orb.orb_cust,
    rctt.rctt_nomcli, orb.orb_campana, orb.orb_regla, orb.orb_codusu,
    orb.orb_nivel, orb.orb_descr, orb.orb_totitems, orb.orb_totvalor
FROM mob_rordbonif orb
INNER JOIN mob_rcustmain rctt
    ON orb.orb_cmpy = rctt.rctt_cmpy AND orb.orb_cust = rctt.rctt_cust;

-- Consumo del periodo actual por cliente (para V-08 en sync móvil)
CREATE VIEW vw_rbonconsumo_actual AS
SELECT cns_cmpy, cns_regla, cns_cust, cns_periodo, cns_veces, cns_ultfecha
FROM mob_rbonconsumo
WHERE cns_periodo IN ('000000', '202600');  -- reemplazar por función de periodo actual según motor de BD
