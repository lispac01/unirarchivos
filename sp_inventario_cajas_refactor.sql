CREATE OR ALTER PROCEDURE [dbo].[sp_inventario_cajas]
    @p_fr_cabnum INT,
    @p_fec_doc DATE,
    @p_fec_aux DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @fin_mes_aux DATE = EOMONTH(@p_fec_aux);

    WITH estados_disponibles AS (
        SELECT cod_estado
        FROM ct_calcdispEST
        WHERE tipo_calculo = 'disstk'
          AND suma_resta IN ('S', 's')
    ),
    stock AS (
        SELECT
            cod_articulo,
            cod_tit,
            SUM(cantidad * signo) AS cantidad
        FROM cpf_stockaux
        WHERE fec_doc <= @p_fec_doc
          AND fec_doc <= @fin_mes_aux
          AND cod_estado IN (SELECT cod_estado FROM estados_disponibles)
        GROUP BY cod_articulo, cod_tit
    ),
    max_liquida AS (
        SELECT
            ISNULL(MAX(nro_doca), 0) AS max_nro_doca,
            ISNULL(MAX(nro_docum), 0) AS max_nro_docum
        FROM cpp_lfLiquida
    ),
    nro_q2 AS (
        SELECT
            l.fr_cedpro,
            ml.max_nro_doca,
            ml.max_nro_docum + ROW_NUMBER() OVER (ORDER BY l.fr_cedpro) AS nro_docum
        FROM (SELECT fr_cedpro FROM lf_movimientopallets GROUP BY fr_cedpro) l
        CROSS JOIN max_liquida ml
    ),
    q1 AS (
        SELECT
            a.fr_cabnum,
            a.fr_codfin,
            g.fr_nomfin,
            ISNULL(td.cod_tit, 0) AS bodega,
            a.fr_cedpro,
            a.fr_codemba,
            src.cod_articulo,
            SUM(a.fr_cajasreales) AS fr_cajasreales,
            src.cant_compcaja,
            SUM(a.fr_cajasreales) * src.cant_compcaja AS nrocajas,
            ROUND(SUM(a.fr_cajasreales) * src.cant_compcaja * ISNULL(fe.factor, 1), 2) AS nrocajas2,
            a.nro_docum,
            a.cod_dpto,
            src.cod_cta_cos,
            ar.cod_cta_inve,
            fe.factor,
            fe.cod_tipo,
            t.nom_tit,
            d.nom_tit AS dep,
            ar.nom_articulo,
            d.cod_dpto AS cod_dpto_ori,
            a.fr_cpcpto,
            ISNULL(td.cod_tit, 0) AS cod_tit,
            ar.es_loteable,
            ar.tiene_ubic
        FROM cpp_lfliquida a
        LEFT JOIN cpp_titulardepo td
            ON td.fr_codfin = a.fr_codfin
           AND td.fr_cedpro = a.fr_cedpro
           AND td.activa = 'S'
        LEFT JOIN ct_depositos d
            ON d.cod_tit = td.cod_tit
        INNER JOIN ct_titulares t
            ON t.cod_tit = a.fr_cedpro
        INNER JOIN cp_fruemba fe
            ON fe.fr_codemba = a.fr_codemba
        INNER JOIN cp_fincas g
            ON g.fr_codfin = a.fr_codfin
        LEFT JOIN cpp_embprodfinca bodxEmb
            ON bodxEmb.fr_codemba = a.fr_codemba
           AND bodxEmb.cod_tit = td.cod_tit
        LEFT JOIN cpp_embaunidprod em
            ON em.fr_codemba = a.fr_codemba
           AND bodxEmb.fr_codemba IS NULL
        LEFT JOIN (
            SELECT fr_codemba, nro_trans, cod_articulo, cant_compcaja, cod_cta_cos
            FROM cpp_embprodxfinc
        ) artxEmb
            ON artxEmb.fr_codemba = a.fr_codemba
           AND artxEmb.nro_trans = bodxEmb.nro_trans
        CROSS APPLY (
            SELECT
                CASE
                    WHEN bodxEmb.fr_codemba IS NULL THEN em.cod_articulo
                    ELSE artxEmb.cod_articulo
                END AS cod_articulo,
                CASE
                    WHEN bodxEmb.fr_codemba IS NULL THEN em.cant_compcaja
                    ELSE artxEmb.cant_compcaja
                END AS cant_compcaja,
                CASE
                    WHEN bodxEmb.fr_codemba IS NULL THEN em.cod_cta_cos
                    ELSE artxEmb.cod_cta_cos
                END AS cod_cta_cos
        ) src
        LEFT JOIN ct_articulos ar
            ON ar.cod_articulo = src.cod_articulo
        WHERE a.fr_cabnum = @p_fr_cabnum
          AND a.fr_cpcpto >= 1
          AND a.fr_cpcpto <= 999
        GROUP BY
            a.fr_cabnum,
            a.fr_codfin,
            g.fr_nomfin,
            ISNULL(td.cod_tit, 0),
            a.fr_cedpro,
            a.fr_codemba,
            src.cod_articulo,
            src.cant_compcaja,
            a.nro_docum,
            a.cod_dpto,
            src.cod_cta_cos,
            ar.cod_cta_inve,
            fe.factor,
            fe.cod_tipo,
            t.nom_tit,
            d.nom_tit,
            ar.nom_articulo,
            d.cod_dpto,
            a.fr_cpcpto,
            ar.es_loteable,
            ar.tiene_ubic
    ),
    q2 AS (
        SELECT DISTINCT
            l.fr_cabnum,
            l.fr_codfin,
            g.fr_nomfin,
            ISNULL(td.cod_tit, 0) AS bodega,
            l.fr_cedpro,
            l.fr_codemba,
            src.cod_articulo,
            l.nrocajas AS fr_cajasreales,
            src.cant_compcaja,
            l.nrocajas * src.cant_compcaja AS nrocajas,
            ROUND((l.nrocajas * src.cant_compcaja) * ISNULL(h.factor, 1), 2) AS nrocajas2,
            nro.nro_docum,
            (SELECT cod_dpto FROM ct_paramliqfru WHERE cod_emp = 101) AS cod_dpto,
            src.cod_cta_cos,
            j.cod_cta_inve,
            h.factor,
            h.cod_tipo,
            t.nom_tit,
            d.nom_tit AS dep,
            j.nom_articulo,
            d.cod_dpto AS cod_dpto_ori,
            0 AS fr_cpcpto,
            ISNULL(td.cod_tit, 0) AS cod_tit,
            j.es_loteable,
            j.tiene_ubic
        FROM lf_movimientopallets l
        LEFT JOIN cpp_titulardepo td
            ON td.fr_codfin = l.fr_codfin
           AND td.fr_cedpro = l.fr_cedpro
           AND td.activa = 'S'
        LEFT JOIN ct_depositos d
            ON d.cod_tit = td.cod_tit
        INNER JOIN cp_fruemba h
            ON h.fr_codemba = l.fr_codemba
        INNER JOIN cp_fincas g
            ON g.fr_codfin = l.fr_codfin
        INNER JOIN ct_titulares t
            ON t.cod_tit = l.fr_cedpro
        LEFT JOIN cpp_embprodfinca bodxEmb
            ON bodxEmb.fr_codemba = l.fr_codemba
           AND bodxEmb.cod_tit = td.cod_tit
        LEFT JOIN cpp_embaunidprod a
            ON a.fr_codemba = l.fr_codemba
           AND bodxEmb.fr_codemba IS NULL
        LEFT JOIN (
            SELECT fr_codemba, nro_trans, cod_articulo, cant_compcaja, cod_cta_cos
            FROM cpp_embprodxfinc
        ) artxEmb
            ON artxEmb.fr_codemba = l.fr_codemba
           AND artxEmb.nro_trans = bodxEmb.nro_trans
        CROSS APPLY (
            SELECT
                CASE
                    WHEN bodxEmb.fr_codemba IS NULL THEN a.cod_articulo
                    ELSE artxEmb.cod_articulo
                END AS cod_articulo,
                CASE
                    WHEN bodxEmb.fr_codemba IS NULL THEN a.cant_compcaja
                    ELSE artxEmb.cant_compcaja
                END AS cant_compcaja,
                CASE
                    WHEN bodxEmb.fr_codemba IS NULL THEN a.cod_cta_cos
                    ELSE artxEmb.cod_cta_cos
                END AS cod_cta_cos
        ) src
        LEFT JOIN ct_articulos j
            ON j.cod_articulo = src.cod_articulo
        INNER JOIN nro_q2 nro
            ON nro.fr_cedpro = l.fr_cedpro
        WHERE l.fr_cabnum = @p_fr_cabnum
    ),
    q AS (
        SELECT * FROM q1
        UNION ALL
        SELECT * FROM q2
    )
    SELECT
        q.fr_cabnum,
        q.fr_codfin,
        q.fr_nomfin,
        q.fr_cedpro,
        q.fr_cpcpto,
        q.fr_codemba,
        q.cod_articulo,
        q.fr_cajasreales,
        q.cant_compcaja,
        ISNULL(q.factor, 1) AS factor,
        CASE
            WHEN q.cod_tipo = 'carton' THEN q.nrocajas2
            ELSE q.nrocajas
        END AS nrocajas,
        q.bodega,
        q.nro_docum,
        q.cod_dpto,
        q.cod_cta_cos,
        ISNULL(CASE WHEN q.es_loteable = 'N' AND q.tiene_ubic = 'N' THEN s.cantidad END, 0) - q.nrocajas AS diferencia,
        CASE
            WHEN ISNULL(CASE WHEN q.es_loteable = 'N' AND q.tiene_ubic = 'N' THEN s.cantidad END, 0) <= 0 THEN 0
            ELSE ISNULL(CASE WHEN q.es_loteable = 'N' AND q.tiene_ubic = 'N' THEN s.cantidad END, 0)
        END AS cantStock,
        q.cod_cta_inve,
        q.nom_tit,
        q.dep,
        q.nom_articulo,
        q.cod_dpto_ori
    FROM q
    LEFT JOIN stock s
        ON s.cod_articulo = q.cod_articulo
       AND s.cod_tit = q.cod_tit;
END
