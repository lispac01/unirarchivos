ALTER PROCEDURE [dbo].[p_lf_devengados_excedente]
 @nro_trans INT,
 @fr_cabnum INT,
 @fecha_mod DATETIME,
 @formulario CHAR(16),
 @operacion_mod CHAR(10),
 @terminal_mod CHAR(4),
 @usuario_mod CHAR(10)
AS
BEGIN
 SET NOCOUNT ON;

 CREATE TABLE #tFull
 (
  fr_cabnum INT,
  fr_codfin CHAR(15),
  fr_cedpro CHAR(12),
  fr_protipo CHAR(2),
  fr_proesp CHAR(2),
  fr_codemba CHAR(15),
  fr_cfotca CHAR(2),
  nrocajas INT,
  fr_cpcpto CHAR(4),
  fr_taval1 DECIMAL(14, 6),
  nro_docum INT,
  fr_cpctadb CHAR(15),
  cod_emp CHAR(15),
  cod_dpto CHAR(6),
  fr_cabtipcam DECIMAL(14, 7),
  fr_cabfectasa DATETIME,
  cod_aux CHAR(15),
  cod_moneda CHAR(4),
  lf_valorDolar DECIMAL(20, 2),
  tc_mov DECIMAL(12, 6),
  lf_valorPeso DECIMAL(20, 2),
  fr_cpvarie CHAR(2),
  fr_codmer CHAR(2),
  ref1_mov VARCHAR(500),
  es_experimento CHAR(1),
  se_exporta VARCHAR(100),
  es_excedente CHAR(1)
 );

 CREATE TABLE #tExcedente
 (
  row_id BIGINT IDENTITY(1,1) NOT NULL,
  fr_cabnum INT,
  fr_codfin CHAR(15),
  fr_cedpro CHAR(12),
  fr_protipo CHAR(2),
  fr_proesp CHAR(2),
  fr_codemba CHAR(15),
  fr_cfotca CHAR(2),
  nrocajas INT,
  fr_cpcpto CHAR(4),
  fr_taval1 DECIMAL(14, 6),
  nro_docum INT,
  fr_cpctadb CHAR(15),
  cod_emp CHAR(15),
  cod_dpto CHAR(6),
  fr_cabtipcam DECIMAL(14, 7),
  fr_cabfectasa DATETIME,
  cod_aux CHAR(15),
  cod_moneda CHAR(4),
  lf_valorDolar DECIMAL(20, 2),
  tc_mov DECIMAL(12, 6),
  lf_valorPeso DECIMAL(20, 2),
  fr_cpvarie CHAR(2),
  fr_codmer CHAR(2),
  ref1_mov VARCHAR(500),
  es_experimento CHAR(1),
  se_exporta VARCHAR(100),
  es_excedente CHAR(1)
 );

 CREATE TABLE #Datos
 (
  fr_codfin CHAR(15),
  fr_cedpro CHAR(12),
  fr_protipo CHAR(2),
  fr_proesp CHAR(2),
  fr_codemba CHAR(15),
  fr_cfotca CHAR(2),
  nrocajas INT,
  nrocajas_real INT,
  fr_cpcpto CHAR(4),
  fr_taval1 DECIMAL(24, 8),
  nro_docum INT,
  fr_cpctadb CHAR(15),
  cod_emp CHAR(15),
  cod_dpto CHAR(6),
  fr_cabtipcam DECIMAL(14, 7),
  fr_cabfectasa DATETIME,
  cod_aux CHAR(15),
  cod_moneda CHAR(4),
  lf_valorDolar DECIMAL(20, 2),
  tc_mov DECIMAL(12, 6),
  lf_valorPeso DECIMAL(20, 2),
  fr_cpvarie CHAR(2),
  fr_codmer CHAR(2),
  ref1_mov VARCHAR(500),
  es_experimento CHAR(1),
  se_exporta VARCHAR(100),
  tope_excedente INT,
  cajas_anteriores INT,
  tar_excedente DECIMAL(24, 8),
  tipo_concepto CHAR(10),
  factor_conver DECIMAL(15, 8),
  liq_excedente CHAR(1),
  cajas_actual INT,
  concepto_liq_excedente CHAR(1),
  tipo_concepto_calcula_excedente CHAR(1),
  orden_prioridad INT,
  orden_venta INT
 );

 CREATE CLUSTERED INDEX IX_Datos_Orden
  ON #Datos (fr_codfin, orden_prioridad, factor_conver, fr_codemba, fr_cfotca, orden_venta);

 CREATE INDEX IX_tFull_Busqueda_1
  ON #tFull (fr_codfin, fr_protipo, fr_proesp, fr_codemba, fr_cfotca, se_exporta, es_experimento, es_excedente, fr_cpcpto)
  INCLUDE (nrocajas);

 CREATE INDEX IX_tFull_Busqueda_2
  ON #tFull (fr_codfin, fr_codemba, fr_cfotca, se_exporta, es_experimento, es_excedente, fr_cpcpto)
  INCLUDE (nrocajas);

 CREATE INDEX IX_tExcedente_Busqueda_1
  ON #tExcedente (fr_codfin, fr_protipo, fr_proesp, fr_codemba, fr_cfotca, se_exporta, es_experimento, fr_cpcpto)
  INCLUDE (nrocajas);

 CREATE INDEX IX_tExcedente_Busqueda_2
  ON #tExcedente (fr_codfin, fr_codemba, fr_cfotca, se_exporta, es_experimento, fr_cpcpto)
  INCLUDE (nrocajas);

 DECLARE @v_fr_codfin CHAR(15);
 DECLARE @v_fr_cedpro CHAR(12);
 DECLARE @v_fr_protipo CHAR(2);
 DECLARE @v_fr_proesp CHAR(2);
 DECLARE @v_fr_codemba CHAR(15);
 DECLARE @v_fr_codemba_tmp CHAR(15);
 DECLARE @v_fr_cfotca CHAR(2);
 DECLARE @v_nrocajas INT;
 DECLARE @v_nrocajas_real INT;
 DECLARE @v_fr_cpcpto CHAR(4);
 DECLARE @v_fr_taval1 DECIMAL(24, 8);
 DECLARE @v_nro_docum INT;
 DECLARE @v_fr_cpctadb CHAR(15);
 DECLARE @v_cod_emp CHAR(15);
 DECLARE @v_cod_dpto CHAR(6);
 DECLARE @v_fr_cabtipcam DECIMAL(14, 7);
 DECLARE @v_fr_cabfectasa DATETIME;
 DECLARE @v_cod_aux CHAR(15);
 DECLARE @v_cod_moneda CHAR(4);
 DECLARE @v_lf_valorDolar DECIMAL(20, 2);
 DECLARE @v_tc_mov DECIMAL(12, 6);
 DECLARE @v_lf_valorPeso DECIMAL(20, 2);
 DECLARE @v_fr_cpvarie CHAR(2);
 DECLARE @v_fr_codmer CHAR(2);
 DECLARE @v_ref1_mov VARCHAR(500);
 DECLARE @v_es_experimento CHAR(1);
 DECLARE @v_se_exporta VARCHAR(100);
 DECLARE @v_factor_conver DECIMAL(15, 8);
 DECLARE @v_tipo_concepto CHAR(10);
 DECLARE @v_liq_excedente CHAR(1);
 DECLARE @v_cajas_actual INT;
 DECLARE @v_concepto_liq_excedente CHAR(1);
 DECLARE @v_tipo_concepto_calcula_excedente CHAR(1);

 DECLARE @v_fr_codfin_tmp CHAR(15);
 DECLARE @v_fr_cfotca_tmp CHAR(2);
 DECLARE @v_fecha_embarque DATE;
 DECLARE @v_semana_embarque INT;
 DECLARE @v_anio_embarque INT;
 DECLARE @v_tope INT;
 DECLARE @v_cajas_acum_ateriores INT;

 DECLARE @v_cajas_acum_embalaje_no_exportadas INT;
 DECLARE @v_cajas_acum_embalaje_experimento INT;
 DECLARE @v_cajas_acum_embalaje_se_exporta INT;
 DECLARE @v_a_distribuir INT;
 DECLARE @v_a_insertar INT;

 DECLARE @v_cpto_excedente CHAR(4);
 DECLARE @v_tar_excedente DECIMAL(24, 8);
 DECLARE @v_excedente INT;

 SELECT @v_fecha_embarque = fr_cabfecha
 FROM cp_frucabemb
 WHERE fr_cabnum = @fr_cabnum;

 SELECT @v_semana_embarque = [week],
        @v_anio_embarque = [year]
 FROM weekOfYearBY2(@v_fecha_embarque);

 SELECT @v_cpto_excedente = fr_cpcpto_exce
 FROM ct_paramfrut;

 INSERT INTO #Datos
 (
  fr_codfin,
  fr_cedpro,
  fr_protipo,
  fr_proesp,
  fr_codemba,
  fr_cfotca,
  nrocajas,
  nrocajas_real,
  fr_cpcpto,
  fr_taval1,
  nro_docum,
  fr_cpctadb,
  cod_emp,
  cod_dpto,
  fr_cabtipcam,
  fr_cabfectasa,
  cod_aux,
  cod_moneda,
  lf_valorDolar,
  tc_mov,
  lf_valorPeso,
  fr_cpvarie,
  fr_codmer,
  ref1_mov,
  es_experimento,
  se_exporta,
  tope_excedente,
  cajas_anteriores,
  tar_excedente,
  tipo_concepto,
  factor_conver,
  liq_excedente,
  cajas_actual,
  concepto_liq_excedente,
  tipo_concepto_calcula_excedente,
  orden_prioridad,
  orden_venta
 )
 SELECT dev.fr_codfin,
        dev.fr_cedpro,
        dev.fr_protipo,
        dev.fr_proesp,
        dev.fr_codemba,
        dev.fr_cfotca,
        FLOOR(dev.nrocajas * ISNULL(emba.factor_conver, 1)) AS nrocajas,
        dev.nrocajas AS nrocajas_real,
        dev.fr_cpcpto,
        dev.fr_taval1,
        dev.nro_docum,
        dev.fr_cpctadb,
        dev.cod_emp,
        dev.cod_dpto,
        dev.fr_cabtipcam,
        dev.fr_cabfectasa,
        dev.cod_aux,
        dev.cod_moneda,
        dev.lf_valorDolar,
        dev.tc_mov,
        dev.lf_valorPeso,
        dev.fr_cpvarie,
        dev.fr_codmer,
        dev.ref1_mov,
        dev.es_experimento,
        concep.se_exporta,
        ISNULL(fin.tope_excedente, 0) AS tope_excedente,
        ISNULL(acum.cajas, 0) AS cajas_anteriores,
        texce.fr_taval1 AS tar_excedente,
        concep.cod_tipo AS tipo_concepto,
        ISNULL(emba.factor_conver, 1) AS factor_conver,
        clasi.liq_excedente,
        cajas_actual.cajas - ISNULL(acum.cajas, 0) AS cajas_actual,
        ISNULL(concep.aplica, 'S') AS concepto_liq_excedente,
        CASE WHEN EXISTS (SELECT 1 FROM ct_tconcexced tce WHERE tce.cod_tipo = concep.cod_tipo) THEN 'S' END AS tipo_concepto_calcula_excedente,
        ISNULL(clasi.prioridad, 1000) AS orden_prioridad,
        CASE WHEN concep.cod_tipo = 'VENTA' THEN 0 ELSE 1 END AS orden_venta
 FROM lf_devengados_base(@fr_cabnum, 'ANTERIOR') dev
 LEFT JOIN dbo.f_acumulados_excedente_fruta(@v_semana_embarque, @v_anio_embarque, @fr_cabnum, 'N') acum
  ON acum.fr_codfin = dev.fr_codfin
 LEFT JOIN dbo.f_acumulados_excedente_fruta(@v_semana_embarque, @v_anio_embarque, @fr_cabnum, 'S') cajas_actual
  ON cajas_actual.fr_codfin = dev.fr_codfin
 LEFT JOIN ct_tarexcedente texce
  ON texce.fr_codfin = dev.fr_codfin
 AND texce.anio_embarque = @v_anio_embarque
 AND texce.sem_embarque = @v_semana_embarque
 LEFT JOIN cp_frucptos concep
  ON concep.fr_cpcpto = dev.fr_cpcpto
 AND concep.fr_cpvarie = dev.fr_cpvarie
 LEFT JOIN cp_fruemba emba
  ON emba.fr_codemba = dev.fr_codemba
 LEFT JOIN ct_clasicajas clasi
  ON clasi.cod_clasi_caja = emba.cod_clasi_caja
 LEFT JOIN cp_fincas fin
  ON fin.fr_codfin = dev.fr_codfin;

 DECLARE cDatos CURSOR LOCAL FAST_FORWARD FOR
 SELECT fr_codfin,
        fr_cedpro,
        fr_protipo,
        fr_proesp,
        fr_codemba,
        fr_cfotca,
        nrocajas,
        nrocajas_real,
        fr_cpcpto,
        fr_taval1,
        nro_docum,
        fr_cpctadb,
        cod_emp,
        cod_dpto,
        fr_cabtipcam,
        fr_cabfectasa,
        cod_aux,
        cod_moneda,
        lf_valorDolar,
        tc_mov,
        lf_valorPeso,
        fr_cpvarie,
        fr_codmer,
        ref1_mov,
        es_experimento,
        se_exporta,
        tope_excedente,
        cajas_anteriores,
        tar_excedente,
        tipo_concepto,
        factor_conver,
        liq_excedente,
        cajas_actual,
        concepto_liq_excedente,
        tipo_concepto_calcula_excedente
 FROM #Datos
 ORDER BY fr_codfin, orden_prioridad, factor_conver, fr_codemba, fr_cfotca, orden_venta;

 OPEN cDatos;

 FETCH NEXT FROM cDatos
 INTO @v_fr_codfin, @v_fr_cedpro, @v_fr_protipo, @v_fr_proesp, @v_fr_codemba, @v_fr_cfotca, @v_nrocajas, @v_nrocajas_real, @v_fr_cpcpto, @v_fr_taval1,
      @v_nro_docum, @v_fr_cpctadb, @v_cod_emp, @v_cod_dpto, @v_fr_cabtipcam, @v_fr_cabfectasa, @v_cod_aux, @v_cod_moneda, @v_lf_valorDolar,
      @v_tc_mov, @v_lf_valorPeso, @v_fr_cpvarie, @v_fr_codmer, @v_ref1_mov, @v_es_experimento, @v_se_exporta, @v_tope, @v_cajas_acum_ateriores,
      @v_tar_excedente, @v_tipo_concepto, @v_factor_conver, @v_liq_excedente, @v_cajas_actual, @v_concepto_liq_excedente, @v_tipo_concepto_calcula_excedente;

 SET @v_fr_codfin_tmp = '';
 SET @v_fr_codemba_tmp = '';
 SET @v_fr_cfotca_tmp = '';
 SET @v_excedente = 0;

 WHILE @@FETCH_STATUS = 0
 BEGIN
  IF @v_fr_codfin_tmp <> @v_fr_codfin
  BEGIN
   SET @v_fr_codfin_tmp = @v_fr_codfin;
   SET @v_cajas_acum_embalaje_no_exportadas = 0;
   SET @v_cajas_acum_embalaje_experimento = 0;
   SET @v_cajas_acum_embalaje_se_exporta = 0;

   IF @v_cajas_acum_ateriores >= @v_tope
   BEGIN
    SET @v_excedente = @v_cajas_acum_ateriores + @v_cajas_actual;
   END
   ELSE IF @v_cajas_acum_ateriores + @v_cajas_actual <= @v_tope
   BEGIN
    SET @v_excedente = 0;
   END
   ELSE
   BEGIN
    SET @v_excedente = @v_cajas_acum_ateriores + @v_cajas_actual - @v_tope;
   END
  END

  IF @v_fr_codemba_tmp <> @v_fr_codemba
  BEGIN
   SET @v_cajas_acum_embalaje_no_exportadas = 0;
   SET @v_cajas_acum_embalaje_experimento = 0;
   SET @v_cajas_acum_embalaje_se_exporta = 0;
   SET @v_fr_codemba_tmp = @v_fr_codemba;
  END

  IF @v_fr_cfotca_tmp <> @v_fr_cfotca
  BEGIN
   SET @v_cajas_acum_embalaje_no_exportadas = 0;
   SET @v_cajas_acum_embalaje_experimento = 0;
   SET @v_cajas_acum_embalaje_se_exporta = 0;
   SET @v_fr_cfotca_tmp = @v_fr_cfotca;
  END

  SET @v_a_distribuir = @v_nrocajas_real;

  IF @v_excedente > 0 AND @v_a_distribuir > 0 AND @v_tipo_concepto = 'VENTA' AND @v_liq_excedente = 'S'
  BEGIN
   IF @v_tar_excedente IS NULL
    SET @v_tar_excedente = @v_fr_taval1;
   ELSE
    SET @v_tar_excedente = @v_tar_excedente * @v_factor_conver;

   IF @v_a_distribuir <= @v_excedente
    SET @v_a_insertar = @v_nrocajas_real;
   ELSE
    SET @v_a_insertar = @v_excedente;

   INSERT INTO #tExcedente
   (
    fr_cabnum,
    fr_codfin,
    fr_cedpro,
    fr_protipo,
    fr_proesp,
    fr_codemba,
    fr_cfotca,
    nrocajas,
    fr_cpcpto,
    fr_taval1,
    nro_docum,
    fr_cpctadb,
    cod_emp,
    cod_dpto,
    fr_cabtipcam,
    fr_cabfectasa,
    cod_aux,
    cod_moneda,
    lf_valorDolar,
    tc_mov,
    lf_valorPeso,
    fr_cpvarie,
    fr_codmer,
    ref1_mov,
    es_experimento,
    se_exporta,
    es_excedente
   )
   VALUES
   (
    @fr_cabnum,
    @v_fr_codfin,
    @v_fr_cedpro,
    @v_fr_protipo,
    @v_fr_proesp,
    @v_fr_codemba,
    @v_fr_cfotca,
    @v_a_insertar,
    @v_fr_cpcpto,
    @v_tar_excedente,
    @v_nro_docum,
    @v_fr_cpctadb,
    @v_cod_emp,
    @v_cod_dpto,
    @v_fr_cabtipcam,
    @v_fr_cabfectasa,
    @v_cod_aux,
    @v_cod_moneda,
    @v_a_insertar * @v_tar_excedente,
    @v_tc_mov,
    (@v_a_insertar * @v_tar_excedente) * @v_fr_cabtipcam,
    @v_fr_cpvarie,
    @v_fr_codmer,
    @v_ref1_mov,
    @v_es_experimento,
    @v_se_exporta,
    'S'
   );

   SET @v_excedente = @v_excedente - @v_a_insertar;
   SET @v_a_distribuir = @v_a_distribuir - @v_a_insertar;
  END

  IF @v_tipo_concepto <> 'VENTA'
  BEGIN
   IF @v_tipo_concepto_calcula_excedente = 'S' AND @v_liq_excedente = 'S'
   BEGIN
    SET @v_tar_excedente = @v_fr_taval1;

    SELECT TOP (1) @v_a_insertar = nrocajas
    FROM #tExcedente
    WHERE fr_codfin = @v_fr_codfin
      AND fr_protipo = @v_fr_protipo
      AND fr_proesp = @v_fr_proesp
      AND fr_codemba = @v_fr_codemba
      AND fr_cfotca = @v_fr_cfotca
      AND fr_cpcpto < '1000'
      AND es_experimento = @v_es_experimento
      AND se_exporta = @v_se_exporta
    ORDER BY row_id DESC;

    IF @v_a_insertar IS NULL
    BEGIN
     SELECT TOP (1) @v_a_insertar = nrocajas
     FROM #tExcedente
     WHERE fr_codfin = @v_fr_codfin
       AND fr_codemba = @v_fr_codemba
       AND fr_cfotca = @v_fr_cfotca
       AND fr_cpcpto < '1000'
       AND es_experimento = @v_es_experimento
       AND se_exporta = @v_se_exporta
     ORDER BY row_id DESC;
    END

    IF @v_concepto_liq_excedente = 'S'
    BEGIN
     IF @v_a_insertar > 0
     BEGIN
      INSERT INTO #tExcedente
      (
       fr_cabnum,
       fr_codfin,
       fr_cedpro,
       fr_protipo,
       fr_proesp,
       fr_codemba,
       fr_cfotca,
       nrocajas,
       fr_cpcpto,
       fr_taval1,
       nro_docum,
       fr_cpctadb,
       cod_emp,
       cod_dpto,
       fr_cabtipcam,
       fr_cabfectasa,
       cod_aux,
       cod_moneda,
       lf_valorDolar,
       tc_mov,
       lf_valorPeso,
       fr_cpvarie,
       fr_codmer,
       ref1_mov,
       es_experimento,
       se_exporta,
       es_excedente
      )
      VALUES
      (
       @fr_cabnum,
       @v_fr_codfin,
       @v_fr_cedpro,
       @v_fr_protipo,
       @v_fr_proesp,
       @v_fr_codemba,
       @v_fr_cfotca,
       @v_a_insertar,
       @v_fr_cpcpto,
       @v_tar_excedente,
       @v_nro_docum,
       @v_fr_cpctadb,
       @v_cod_emp,
       @v_cod_dpto,
       @v_fr_cabtipcam,
       @v_fr_cabfectasa,
       @v_cod_aux,
       @v_cod_moneda,
       @v_a_insertar * @v_tar_excedente,
       @v_tc_mov,
       (@v_a_insertar * @v_tar_excedente) * @v_fr_cabtipcam,
       @v_fr_cpvarie,
       @v_fr_codmer,
       @v_ref1_mov,
       @v_es_experimento,
       @v_se_exporta,
       'S'
      );
     END
    END
    SELECT @v_a_insertar = SUM(nrocajas)
    FROM #tFull
    WHERE fr_codfin = @v_fr_codfin
      AND fr_protipo = @v_fr_protipo
      AND fr_proesp = @v_fr_proesp
      AND fr_codemba = @v_fr_codemba
      AND fr_cfotca = @v_fr_cfotca
      AND fr_cpcpto < '1000'
      AND es_experimento = @v_es_experimento
      AND se_exporta = @v_se_exporta;

    IF @v_a_insertar IS NULL
    BEGIN
     SELECT @v_a_insertar = SUM(nrocajas)
     FROM #tFull
     WHERE fr_codfin = @v_fr_codfin
       AND fr_codemba = @v_fr_codemba
       AND fr_cfotca = @v_fr_cfotca
       AND fr_cpcpto < '1000'
       AND es_experimento = @v_es_experimento
       AND se_exporta = @v_se_exporta;
    END

    IF @v_tipo_concepto NOT IN ('PALLET', 'FESTIVOS')
    BEGIN
     IF @v_se_exporta = 'SI'
     BEGIN
      SELECT @v_a_insertar = SUM(nrocajas)
      FROM #tFull
      WHERE fr_codfin = @v_fr_codfin
        AND fr_protipo = @v_fr_protipo
        AND fr_proesp = @v_fr_proesp
        AND fr_codemba = @v_fr_codemba
        AND fr_cfotca = @v_fr_cfotca
        AND se_exporta = 'SI'
        AND es_excedente = 'N'
        AND fr_cpcpto < '1000';

      IF @v_a_insertar IS NULL
      BEGIN
       SELECT @v_a_insertar = SUM(nrocajas)
       FROM #tFull
       WHERE fr_codfin = @v_fr_codfin
         AND fr_codemba = @v_fr_codemba
         AND fr_cfotca = @v_fr_cfotca
         AND se_exporta = 'SI'
         AND es_excedente = 'N'
         AND fr_cpcpto < '1000';
      END
     END
     ELSE IF @v_se_exporta = 'NO-OTROS'
     BEGIN
      SELECT @v_a_insertar = SUM(nrocajas)
      FROM #tFull
      WHERE fr_codfin = @v_fr_codfin
        AND fr_protipo = @v_fr_protipo
        AND fr_proesp = @v_fr_proesp
        AND fr_codemba = @v_fr_codemba
        AND fr_cfotca = @v_fr_cfotca
        AND se_exporta = 'NO-OTROS'
        AND es_experimento = 'N'
        AND es_excedente = 'N'
        AND fr_cpcpto < '1000';

      IF @v_a_insertar IS NULL
      BEGIN
       SELECT @v_a_insertar = SUM(nrocajas)
       FROM #tFull
       WHERE fr_codfin = @v_fr_codfin
         AND fr_codemba = @v_fr_codemba
         AND fr_cfotca = @v_fr_cfotca
         AND se_exporta = 'NO-OTROS'
         AND es_experimento = 'N'
         AND es_excedente = 'N'
         AND fr_cpcpto < '1000';
      END
     END
     ELSE IF @v_se_exporta = 'NO-EXPERIMENTO'
     BEGIN
      SELECT @v_a_insertar = SUM(nrocajas)
      FROM #tFull
      WHERE fr_codfin = @v_fr_codfin
        AND fr_protipo = @v_fr_protipo
        AND fr_proesp = @v_fr_proesp
        AND fr_codemba = @v_fr_codemba
        AND fr_cfotca = @v_fr_cfotca
        AND es_experimento = 'S'
        AND es_excedente = 'N'
        AND fr_cpcpto < '1000';

      IF @v_a_insertar IS NULL
      BEGIN
       SELECT @v_a_insertar = SUM(nrocajas)
       FROM #tFull
       WHERE fr_codfin = @v_fr_codfin
         AND fr_codemba = @v_fr_codemba
         AND fr_cfotca = @v_fr_cfotca
         AND es_experimento = 'S'
         AND es_excedente = 'N'
         AND fr_cpcpto < '1000';
      END
     END
    END
   END
   ELSE
   BEGIN
    SELECT @v_a_insertar = SUM(nrocajas)
    FROM
    (
     SELECT nrocajas
     FROM #tFull
     WHERE fr_codfin = @v_fr_codfin
       AND fr_protipo = @v_fr_protipo
       AND fr_proesp = @v_fr_proesp
       AND fr_codemba = @v_fr_codemba
       AND fr_cfotca = @v_fr_cfotca
       AND fr_cpcpto < '1000'

     UNION ALL

     SELECT nrocajas
     FROM #tExcedente
     WHERE fr_codfin = @v_fr_codfin
       AND fr_protipo = @v_fr_protipo
       AND fr_proesp = @v_fr_proesp
       AND fr_codemba = @v_fr_codemba
       AND fr_cfotca = @v_fr_cfotca
       AND fr_cpcpto < '1000'
    ) AS a;

    IF @v_a_insertar IS NULL
    BEGIN
     SELECT @v_a_insertar = SUM(nrocajas)
     FROM
     (
      SELECT nrocajas
      FROM #tFull
      WHERE fr_codfin = @v_fr_codfin
        AND fr_codemba = @v_fr_codemba
        AND fr_cfotca = @v_fr_cfotca
        AND fr_cpcpto < '1000'

      UNION ALL

      SELECT nrocajas
      FROM #tExcedente
      WHERE fr_codfin = @v_fr_codfin
        AND fr_codemba = @v_fr_codemba
        AND fr_cfotca = @v_fr_cfotca
        AND fr_cpcpto < '1000'
     ) AS a;
    END

    IF @v_tipo_concepto NOT IN ('PALLET', 'FESTIVOS')
    BEGIN
     IF @v_se_exporta = 'SI'
     BEGIN
      SELECT @v_a_insertar = SUM(nrocajas)
      FROM
      (
       SELECT nrocajas
       FROM #tFull
       WHERE fr_codfin = @v_fr_codfin
         AND fr_protipo = @v_fr_protipo
         AND fr_proesp = @v_fr_proesp
         AND fr_codemba = @v_fr_codemba
         AND fr_cfotca = @v_fr_cfotca
         AND se_exporta = 'SI'
         AND fr_cpcpto < '1000'

       UNION ALL

       SELECT nrocajas
       FROM #tExcedente
       WHERE fr_codfin = @v_fr_codfin
         AND fr_protipo = @v_fr_protipo
         AND fr_proesp = @v_fr_proesp
         AND fr_codemba = @v_fr_codemba
         AND fr_cfotca = @v_fr_cfotca
         AND se_exporta = 'SI'
         AND fr_cpcpto < '1000'
      ) AS a;

      IF @v_a_insertar IS NULL
      BEGIN
       SELECT @v_a_insertar = SUM(nrocajas)
       FROM
       (
        SELECT nrocajas
        FROM #tFull
        WHERE fr_codfin = @v_fr_codfin
          AND fr_codemba = @v_fr_codemba
          AND fr_cfotca = @v_fr_cfotca
          AND se_exporta = 'SI'
          AND fr_cpcpto < '1000'

        UNION ALL

        SELECT nrocajas
        FROM #tExcedente
        WHERE fr_codfin = @v_fr_codfin
          AND fr_codemba = @v_fr_codemba
          AND fr_cfotca = @v_fr_cfotca
          AND se_exporta = 'SI'
          AND fr_cpcpto < '1000'
       ) AS a;
      END
     END
     ELSE IF @v_se_exporta = 'NO-OTROS'
     BEGIN
      SELECT @v_a_insertar = SUM(nrocajas)
      FROM
      (
       SELECT nrocajas
       FROM #tFull
       WHERE fr_codfin = @v_fr_codfin
         AND fr_protipo = @v_fr_protipo
         AND fr_proesp = @v_fr_proesp
         AND fr_codemba = @v_fr_codemba
         AND fr_cfotca = @v_fr_cfotca
         AND se_exporta = 'NO-OTROS'
         AND es_experimento = 'N'
         AND fr_cpcpto < '1000'

       UNION ALL

       SELECT nrocajas
       FROM #tExcedente
       WHERE fr_codfin = @v_fr_codfin
         AND fr_protipo = @v_fr_protipo
         AND fr_proesp = @v_fr_proesp
         AND fr_codemba = @v_fr_codemba
         AND fr_cfotca = @v_fr_cfotca
         AND se_exporta = 'NO-OTROS'
         AND es_experimento = 'N'
         AND fr_cpcpto < '1000'
      ) AS a;

      IF @v_a_insertar IS NULL
      BEGIN
       SELECT @v_a_insertar = SUM(nrocajas)
       FROM
       (
        SELECT nrocajas
        FROM #tFull
        WHERE fr_codfin = @v_fr_codfin
          AND fr_codemba = @v_fr_codemba
          AND fr_cfotca = @v_fr_cfotca
          AND se_exporta = 'NO-OTROS'
          AND es_experimento = 'N'
          AND fr_cpcpto < '1000'

        UNION ALL

        SELECT nrocajas
        FROM #tExcedente
        WHERE fr_codfin = @v_fr_codfin
          AND fr_codemba = @v_fr_codemba
          AND fr_cfotca = @v_fr_cfotca
          AND se_exporta = 'NO-OTROS'
          AND es_experimento = 'N'
          AND fr_cpcpto < '1000'
       ) AS a;
      END
     END
     ELSE IF @v_se_exporta = 'NO-EXPERIMENTO'
     BEGIN
      SELECT @v_a_insertar = SUM(nrocajas)
      FROM
      (
       SELECT nrocajas
       FROM #tFull
       WHERE fr_codfin = @v_fr_codfin
         AND fr_protipo = @v_fr_protipo
         AND fr_proesp = @v_fr_proesp
         AND fr_codemba = @v_fr_codemba
         AND fr_cfotca = @v_fr_cfotca
         AND es_experimento = 'S'
         AND fr_cpcpto < '1000'

       UNION ALL

       SELECT nrocajas
       FROM #tExcedente
       WHERE fr_codfin = @v_fr_codfin
         AND fr_protipo = @v_fr_protipo
         AND fr_proesp = @v_fr_proesp
         AND fr_codemba = @v_fr_codemba
         AND fr_cfotca = @v_fr_cfotca
         AND es_experimento = 'S'
         AND fr_cpcpto < '1000'
      ) AS a;

      IF @v_a_insertar IS NULL
      BEGIN
       SELECT @v_a_insertar = SUM(nrocajas)
       FROM
       (
        SELECT nrocajas
        FROM #tFull
        WHERE fr_codfin = @v_fr_codfin
          AND fr_codemba = @v_fr_codemba
          AND fr_cfotca = @v_fr_cfotca
          AND es_experimento = 'S'
          AND fr_cpcpto < '1000'

        UNION ALL

        SELECT nrocajas
        FROM #tExcedente
        WHERE fr_codfin = @v_fr_codfin
          AND fr_codemba = @v_fr_codemba
          AND fr_cfotca = @v_fr_cfotca
          AND es_experimento = 'S'
          AND fr_cpcpto < '1000'
       ) AS a;
      END
     END
    END
   END

   IF @v_a_insertar > 0
   BEGIN
    INSERT INTO #tFull
    (
     fr_cabnum,
     fr_codfin,
     fr_cedpro,
     fr_protipo,
     fr_proesp,
     fr_codemba,
     fr_cfotca,
     nrocajas,
     fr_cpcpto,
     fr_taval1,
     nro_docum,
     fr_cpctadb,
     cod_emp,
     cod_dpto,
     fr_cabtipcam,
     fr_cabfectasa,
     cod_aux,
     cod_moneda,
     lf_valorDolar,
     tc_mov,
     lf_valorPeso,
     fr_cpvarie,
     fr_codmer,
     ref1_mov,
     es_experimento,
     se_exporta,
     es_excedente
    )
    VALUES
    (
     @fr_cabnum,
     @v_fr_codfin,
     @v_fr_cedpro,
     @v_fr_protipo,
     @v_fr_proesp,
     @v_fr_codemba,
     @v_fr_cfotca,
     @v_a_insertar,
     @v_fr_cpcpto,
     @v_fr_taval1,
     @v_nro_docum,
     @v_fr_cpctadb,
     @v_cod_emp,
     @v_cod_dpto,
     @v_fr_cabtipcam,
     @v_fr_cabfectasa,
     @v_cod_aux,
     @v_cod_moneda,
     @v_a_insertar * @v_fr_taval1,
     @v_tc_mov,
     (@v_a_insertar * @v_fr_taval1) * @v_fr_cabtipcam,
     @v_fr_cpvarie,
     @v_fr_codmer,
     @v_ref1_mov,
     @v_es_experimento,
     @v_se_exporta,
     'N'
    );
   END
  END

  IF @v_tipo_concepto = 'VENTA' AND @v_a_distribuir > 0
  BEGIN
   SET @v_a_insertar = @v_a_distribuir;
   SET @v_a_distribuir = @v_a_distribuir - @v_a_insertar;

   IF @v_a_insertar > 0
   BEGIN
    INSERT INTO #tFull
    (
     fr_cabnum,
     fr_codfin,
     fr_cedpro,
     fr_protipo,
     fr_proesp,
     fr_codemba,
     fr_cfotca,
     nrocajas,
     fr_cpcpto,
     fr_taval1,
     nro_docum,
     fr_cpctadb,
     cod_emp,
     cod_dpto,
     fr_cabtipcam,
     fr_cabfectasa,
     cod_aux,
     cod_moneda,
     lf_valorDolar,
     tc_mov,
     lf_valorPeso,
     fr_cpvarie,
     fr_codmer,
     ref1_mov,
     es_experimento,
     se_exporta,
     es_excedente
    )
    VALUES
    (
     @fr_cabnum,
     @v_fr_codfin,
     @v_fr_cedpro,
     @v_fr_protipo,
     @v_fr_proesp,
     @v_fr_codemba,
     @v_fr_cfotca,
     @v_a_insertar,
     @v_fr_cpcpto,
     @v_fr_taval1,
     @v_nro_docum,
     @v_fr_cpctadb,
     @v_cod_emp,
     @v_cod_dpto,
     @v_fr_cabtipcam,
     @v_fr_cabfectasa,
     @v_cod_aux,
     @v_cod_moneda,
     @v_a_insertar * @v_fr_taval1,
     @v_tc_mov,
     (@v_a_insertar * @v_fr_taval1) * @v_fr_cabtipcam,
     @v_fr_cpvarie,
     @v_fr_codmer,
     @v_ref1_mov,
     @v_es_experimento,
     @v_se_exporta,
     'N'
    );
   END
  END

  FETCH NEXT FROM cDatos
  INTO @v_fr_codfin, @v_fr_cedpro, @v_fr_protipo, @v_fr_proesp, @v_fr_codemba, @v_fr_cfotca, @v_nrocajas, @v_nrocajas_real, @v_fr_cpcpto, @v_fr_taval1,
       @v_nro_docum, @v_fr_cpctadb, @v_cod_emp, @v_cod_dpto, @v_fr_cabtipcam, @v_fr_cabfectasa, @v_cod_aux, @v_cod_moneda, @v_lf_valorDolar,
       @v_tc_mov, @v_lf_valorPeso, @v_fr_cpvarie, @v_fr_codmer, @v_ref1_mov, @v_es_experimento, @v_se_exporta, @v_tope, @v_cajas_acum_ateriores,
       @v_tar_excedente, @v_tipo_concepto, @v_factor_conver, @v_liq_excedente, @v_cajas_actual, @v_concepto_liq_excedente, @v_tipo_concepto_calcula_excedente;
 END

 CLOSE cDatos;
 DEALLOCATE cDatos;

 DELETE FROM cpp_excedbase
 WHERE nro_trans = @nro_trans;

 INSERT INTO cpp_excedbase
 (
  nro_trans,
  fr_codfin,
  fr_cedpro,
  fr_protipo,
  fr_proesp,
  fr_codemba,
  fr_cfotca,
  fr_cajasReales,
  fr_cpcpto,
  fr_taval1,
  nro_docum,
  fr_cpctadb,
  cod_emp,
  cod_dpto,
  fr_cabtipcam,
  fr_cabfectasa,
  cod_aux,
  cod_moneda,
  lf_valorDolar,
  tc_mov,
  lf_valorPeso,
  fr_cpvarie,
  fr_codmer,
  ref1_mov,
  es_experimento,
  se_exporta,
  es_excedente,
  bloque,
  estado_registro,
  fecha_mod,
  formulario,
  linea,
  operacion_mod,
  seccion,
  terminal_mod,
  usuario_mod,
  fr_cabnum
 )
 SELECT @nro_trans,
        fr_codfin,
        fr_cedpro,
        fr_protipo,
        fr_proesp,
        fr_codemba,
        fr_cfotca,
        nrocajas,
        fr_cpcpto,
        fr_taval1,
        nro_docum,
        fr_cpctadb,
        cod_emp,
        cod_dpto,
        fr_cabtipcam,
        fr_cabfectasa,
        cod_aux,
        cod_moneda,
        lf_valorDolar,
        tc_mov,
        lf_valorPeso,
        fr_cpvarie,
        fr_codmer,
        ref1_mov,
        es_experimento,
        se_exporta,
        es_excedente,
        'excedente',
        'A',
        @fecha_mod,
        @formulario,
        ROW_NUMBER() OVER (ORDER BY fr_codfin),
        @operacion_mod,
        'excedente',
        @terminal_mod,
        @usuario_mod,
        fr_cabnum
 FROM #tFull

 UNION ALL

 SELECT @nro_trans,
        fr_codfin,
        fr_cedpro,
        fr_protipo,
        fr_proesp,
        fr_codemba,
        fr_cfotca,
        nrocajas,
        fr_cpcpto,
        fr_taval1,
        nro_docum,
        fr_cpctadb,
        cod_emp,
        cod_dpto,
        fr_cabtipcam,
        fr_cabfectasa,
        cod_aux,
        cod_moneda,
        lf_valorDolar,
        tc_mov,
        lf_valorPeso,
        fr_cpvarie,
        fr_codmer,
        ref1_mov,
        es_experimento,
        se_exporta,
        es_excedente,
        'excedente',
        'A',
        @fecha_mod,
        @formulario,
        ROW_NUMBER() OVER (ORDER BY fr_codfin),
        @operacion_mod,
        'excedente',
        @terminal_mod,
        @usuario_mod,
        fr_cabnum
 FROM #tExcedente;

 SELECT fr_cabnum,
        fr_codfin,
        fr_cedpro,
        fr_protipo,
        fr_proesp,
        fr_codemba,
        fr_cfotca,
        nrocajas,
        fr_cpcpto,
        fr_taval1,
        nro_docum,
        fr_cpctadb,
        cod_emp,
        cod_dpto,
        fr_cabtipcam,
        fr_cabfectasa,
        cod_aux,
        cod_moneda,
        lf_valorDolar,
        tc_mov,
        lf_valorPeso,
        fr_cpvarie,
        fr_codmer,
        ref1_mov,
        es_experimento,
        se_exporta,
        es_excedente
 FROM #tFull
 ORDER BY nro_docum, fr_codfin, fr_codemba, fr_cpcpto;
END
