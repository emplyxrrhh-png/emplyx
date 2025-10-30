using Google.OrTools.ConstraintSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTORCore
{
    public class RoboticsOT
    {
        private Solver m_s = null;
        private IntVar[][] m_trabajador_por_dia_y_puesto;
        private IntVar[][] m_puesto_por_dia_y_trabajador;
        private IntVar[][] m_puesto_por_trabajador_y_dia;
        private IntVar[][] m_ocupacion_por_trabajador_y_dia;
        private IntVar[][] m_ocupacion_fin_de_semana_por_trabajador_y_dia;
        private IntVar[][] m_fin_de_semana_completo_por_trabajador_y_dia;
        private IntVar[][] m_entrada_por_trabajador_y_dia;
        private IntVar[][] m_salida_por_trabajador_y_dia;
        private IntVar[][] m_entrada_salida_total_por_puesto;
        private IntVar[][] m_minutos_de_trabajo_por_trabajador_y_dia;
        private int[][][] m_disponibilidad_dia_y_trabajador_y_puesto;
        private int[][] m_coste_por_trabajador_y_dia;
        private IntVar[][] m_coste_total_por_trabajador_y_dia;
        private IntVar[] m_ocupacion_por_trabajador_en_toda_la_solucion;

        private int[][] m_necesidad_por_dia_y_puesto;
        private IntVar[] m_horario_por_puesto;
        private IntVar[][] m_turno_por_trabajador_y_dia;

        private IntVar[] m_flat;
        private IntVar[] m_flat_coste_total;
        private string[] m_nombre_trabajadores;
        private string[] m_nombre_puestos;
        private IntVar m_coste;
        private IntVar m_trabajadores_trabajando;

        private DecisionBuilder m_db;
        private OptimizeVar m_objetivo_coste;
        private OptimizeVar m_objetivo_num_trabajadores;
        private SearchLimit m_lim;
        private long m_init_search_time;

        private int m_i_dias;
        private int m_i_puestos;
        private int m_i_trabajadores;
        private int m_i_libres;
        private int m_i_ocupados;

        private RoboticsTrabajadorList m_trabajadores;
        private RoboticsCalendarioDiasList m_calendario;
        private RoboticsTurnosList m_turnos;
        private RoboticsCalendarioPuestosList m_todos_los_puestos;

        private RoboticsSoluciones m_ultima_solucion;

        private int m_diferencia_coste_entre_soluciones = 1;
        private string m_log_constraint_prefix = "";
        private bool m_no_hay_solucion;
        private int m_modo_solucion = 0;
        private bool m_presentes_siempre_crea_puestos;

        private StringBuilder m_log;
        private List<string> m_errores;
        private List<RoboticsReglaTipo> m_desactivar_reglas;

        public RoboticsOT()
        {
            m_trabajadores = new RoboticsTrabajadorList();
            m_calendario = new RoboticsCalendarioDiasList();
            m_turnos = new RoboticsTurnosList();
            m_log = new StringBuilder();
            m_todos_los_puestos = new RoboticsCalendarioPuestosList();
            m_errores = new List<string>();
            m_desactivar_reglas = new List<RoboticsReglaTipo>();
            m_presentes_siempre_crea_puestos = true;
        }

        public RoboticsTrabajadorList Trabajadores { get { return m_trabajadores; } }
        public RoboticsCalendarioDiasList Calendario { get { return m_calendario; } }
        public RoboticsTurnosList Turnos { get { return m_turnos; } }
        public RoboticsCalendarioPuestosList Puestos { get { return m_todos_los_puestos; } }

        public void Init()
        {
            Init(0);
        }

        public void Init(int modo)
        {
            m_no_hay_solucion = false;
            m_modo_solucion = modo;
            if (m_s != null)
            {
                m_s.Dispose();
                m_s = new Solver(Environment.TickCount.ToString());
                CargarObjetos();
                Solucionar();
                return;
            }

            ConstraintSolverParameters pp = Solver.DefaultSolverParameters();

            m_s = new Solver("");
        }

        private void CrearMatrices(int dias, int puestos, int trabajadores)
        {
            m_i_dias = dias;
            m_i_puestos = trabajadores;
            m_i_trabajadores = trabajadores;
            m_i_libres = trabajadores - puestos;
            m_i_ocupados = puestos;

            m_ultima_solucion = new RoboticsSoluciones(m_i_dias, m_i_puestos, m_i_trabajadores);

            // CREAMOS LAS MATRICES
            m_nombre_trabajadores = new string[m_i_trabajadores];
            m_nombre_puestos = new string[m_i_puestos];
            m_horario_por_puesto = new IntVar[m_i_puestos];

            m_trabajador_por_dia_y_puesto = new IntVar[m_i_dias][];
            for (int i = 0; i < m_i_dias; i++)
                m_trabajador_por_dia_y_puesto[i] = new IntVar[m_i_puestos];
            for (int i = 0; i < m_i_dias; i++)
                for (int j = 0; j < m_i_puestos; j++)
                    m_trabajador_por_dia_y_puesto[i][j] = m_s.MakeIntVar(0, m_i_trabajadores - 1, "trabajador");

            // TIENE EN CUENTA LOS PUESTOS LIBRES Y HACE PRUEBAS CON ELLOS
            // PROCESA PRIMERO TODOS LOS PUESTOS DE UN DIA ANTES DE PASAR AL SIGUIENTE (EN PRINCIPIO ES MEJOR)
            m_flat = new IntVar[m_i_dias * m_i_puestos];
            int n = 0;
            for (int i = 0; i < m_i_dias; i++)
            {
                for (int j = 0; j < m_i_puestos; j++)
                {
                    m_flat[n] = m_trabajador_por_dia_y_puesto[i][j];
                    n++;
                }
            }

            /*
            // TIENE EN CUENTA LOS PUESTOS LIBRES Y HACE PRUEBAS CON ELLOS
            // PROCESA PRIMERO TODOS LOS DIAS DE UN TRABAJADOR ANTES DE PASAR AL SIGUIENTE (EN PRINCIPIO ES PEOR)
            m_flat = new IntVar[m_i_dias * m_i_puestos];
            int n = 0;
            for (int i = 0; i < m_i_puestos; i++)
            {
                for (int j = 0; j < m_i_dias; j++)
                {
                    m_flat[n] = m_trabajador_por_dia_y_puesto[j][i];
                    n++;
                }
            }
            */

            /*
            // NO TIENE EN CUENTA LOS PUESTOS LIBRES A LA HORA DE HACER PRUEBAS CON ELLOS
            // PRIMERO PROCESA TODOS LOS PUESTOS DE UN DIA
            // ESTE PROCESO, SUPONE QUE TODOS LOS DIAS ESTÁN LOS MISMOS PUESTOS OCUPADOS Y LIBRES
            // AQUELLOS DIAS QUE TIENEN MENOS PUESTOS, YA ESTÁN FIJADOS PARA QUE SOLO LO PUEDA TENER LOS DUMMY (EN PRINCIPIO ES MUCHO PEOR)
            m_flat = new IntVar[m_i_dias * m_i_ocupados];
            int n = 0;
            for (int i = 0; i < m_i_dias; i++)
            {
                for (int j = 0; j < m_i_ocupados; j++)
                {
                    m_flat[n] = m_trabajador_por_dia_y_puesto[i][j];
                    n++;
                }
            }
            */

            /*
            // NO TIENE EN CUENTA LOS PUESTOS LIBRES A LA HORA DE HACER PRUEBAS CON ELLOS
            // PRIMERO PROCESA TODOS LOS DIAS DE UN PUESTO
            // ESTE PROCESO, SUPONE QUE TODOS LOS DIAS ESTÁN LOS MISMOS PUESTOS OCUPADOS Y LIBRES
            // AQUELLOS DIAS QUE TIENEN MENOS PUESTOS, YA ESTÁN FIJADOS PARA QUE SOLO LO PUEDA TENER LOS DUMMY (EN PRINCIPIO ES MUCHO PEOR)
            m_flat = new IntVar[m_i_dias * m_i_ocupados];
            int n = 0;
            for (int i = 0; i < m_i_ocupados; i++)
            {
                for (int j = 0; j < m_i_dias; j++)
                {
                    m_flat[n] = m_trabajador_por_dia_y_puesto[j][i];
                    n++;
                }
            }
            */

            m_puesto_por_dia_y_trabajador = new IntVar[m_i_dias][];
            for (int i = 0; i < m_i_dias; i++)
                m_puesto_por_dia_y_trabajador[i] = new IntVar[m_i_trabajadores];
            for (int i = 0; i < m_i_dias; i++)
                for (int j = 0; j < m_i_trabajadores; j++)
                    m_puesto_por_dia_y_trabajador[i][j] = m_s.MakeIntVar(0, m_i_puestos - 1, "puesto");

            m_puesto_por_trabajador_y_dia = new IntVar[m_i_trabajadores][];
            for (int i = 0; i < m_i_trabajadores; i++)
                m_puesto_por_trabajador_y_dia[i] = new IntVar[m_i_dias];
            for (int i = 0; i < m_i_trabajadores; i++)
                for (int j = 0; j < m_i_dias; j++)
                    m_puesto_por_trabajador_y_dia[i][j] = m_s.MakeIntVar(0, m_i_puestos - 1, "puesto");

            // INDICA EL TURNO QUE REALIZA EL TRABAJADOR ESE DIA
            m_turno_por_trabajador_y_dia = new IntVar[m_i_trabajadores][];
            for (int i = 0; i < m_i_trabajadores; i++)
                m_turno_por_trabajador_y_dia[i] = new IntVar[m_i_dias];
            for (int i = 0; i < m_i_trabajadores; i++)
                for (int j = 0; j < m_i_dias; j++)
                    m_turno_por_trabajador_y_dia[i][j] = m_s.MakeIntVar(0, 1000, "turno");

            // INDICA SI EL TRABAJADOR TRABAJA ESE DIA O ESTÁ EN UN PUESTO LIBRE
            m_ocupacion_por_trabajador_y_dia = new IntVar[m_i_trabajadores][];
            for (int i = 0; i < m_i_trabajadores; i++)
                m_ocupacion_por_trabajador_y_dia[i] = new IntVar[m_i_dias];
            for (int i = 0; i < m_i_trabajadores; i++)
                for (int j = 0; j < m_i_dias; j++)
                    m_ocupacion_por_trabajador_y_dia[i][j] = m_s.MakeBoolVar("trabaja");

            // INDICA SI UN TRABAJADOR TRABAJA EN TODA LA SOLUCIÓN
            m_ocupacion_por_trabajador_en_toda_la_solucion = new IntVar[m_i_trabajadores - m_i_ocupados];
            for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
                m_ocupacion_por_trabajador_en_toda_la_solucion[i] = m_s.MakeBoolVar("trabaja");

            m_coste_por_trabajador_y_dia = new int[m_i_trabajadores][];
            for (int i = 0; i < m_i_trabajadores; i++)
                m_coste_por_trabajador_y_dia[i] = new int[m_i_dias];
            for (int i = 0; i < m_i_trabajadores; i++)
                for (int j = 0; j < m_i_dias; j++)
                    m_coste_por_trabajador_y_dia[i][j] = 1;

            m_coste_total_por_trabajador_y_dia = new IntVar[m_i_trabajadores][];
            for (int i = 0; i < m_i_trabajadores; i++)
                m_coste_total_por_trabajador_y_dia[i] = new IntVar[m_i_dias];
            for (int i = 0; i < m_i_trabajadores; i++)
                for (int j = 0; j < m_i_dias; j++)
                    m_coste_total_por_trabajador_y_dia[i][j] = m_s.MakeIntVar(0, 10000, "coste total");

            m_flat_coste_total = new IntVar[m_i_dias * (m_i_trabajadores - m_i_ocupados)];
            n = 0;
            for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
            {
                for (int j = 0; j < m_i_dias; j++)
                {
                    m_flat_coste_total[n] = m_coste_total_por_trabajador_y_dia[i][j];
                    n++;
                }
            }

            m_disponibilidad_dia_y_trabajador_y_puesto = new int[m_i_dias][][];
            for (int i = 0; i < m_i_dias; i++)
            {
                m_disponibilidad_dia_y_trabajador_y_puesto[i] = new int[m_i_trabajadores][];
                for (int j = 0; j < m_i_trabajadores; j++)
                    m_disponibilidad_dia_y_trabajador_y_puesto[i][j] = new int[m_i_puestos];
            }
            for (int i = 0; i < m_i_dias; i++)
                for (int j = 0; j < m_i_trabajadores; j++)
                    for (int m = 0; m < m_i_puestos; m++)
                        m_disponibilidad_dia_y_trabajador_y_puesto[i][j][m] = 1;

            m_necesidad_por_dia_y_puesto = new int[m_i_dias][];
            for (int i = 0; i < m_i_dias; i++)
                m_necesidad_por_dia_y_puesto[i] = new int[m_i_puestos];
            for (int i = 0; i < m_i_dias; i++)
                for (int j = 0; j < m_i_puestos; j++)
                    m_necesidad_por_dia_y_puesto[i][j] = 1;

            m_entrada_salida_total_por_puesto = new IntVar[3][];
            for (int i = 0; i < 3; i++)
                m_entrada_salida_total_por_puesto[i] = new IntVar[m_i_puestos];

            m_entrada_por_trabajador_y_dia = new IntVar[m_i_trabajadores][];
            for (int i = 0; i < m_i_trabajadores; i++)
                m_entrada_por_trabajador_y_dia[i] = new IntVar[m_i_dias];
            for (int i = 0; i < m_i_trabajadores; i++)
                for (int j = 0; j < m_i_dias; j++)
                    m_entrada_por_trabajador_y_dia[i][j] = m_s.MakeIntVar(0, 48 * 60, "entrada");

            m_salida_por_trabajador_y_dia = new IntVar[m_i_trabajadores][];
            for (int i = 0; i < m_i_trabajadores; i++)
                m_salida_por_trabajador_y_dia[i] = new IntVar[m_i_dias];
            for (int i = 0; i < m_i_trabajadores; i++)
                for (int j = 0; j < m_i_dias; j++)
                    m_salida_por_trabajador_y_dia[i][j] = m_s.MakeIntVar(0, 48 * 60, "salida");

            m_minutos_de_trabajo_por_trabajador_y_dia = new IntVar[m_i_trabajadores][];
            for (int i = 0; i < m_i_trabajadores; i++)
                m_minutos_de_trabajo_por_trabajador_y_dia[i] = new IntVar[m_i_dias];
            for (int i = 0; i < m_i_trabajadores; i++)
                for (int j = 0; j < m_i_dias; j++)
                    m_minutos_de_trabajo_por_trabajador_y_dia[i][j] = m_s.MakeIntVar(0, 4000 * 60, "horas");

            // INDICA SI EL TRABAJADOR DESCANSA UN DIA CONSIDERADO FIN DE SEMANA O NO
            m_ocupacion_fin_de_semana_por_trabajador_y_dia = new IntVar[m_i_trabajadores][];
            for (int i = 0; i < m_i_trabajadores; i++)
                m_ocupacion_fin_de_semana_por_trabajador_y_dia[i] = new IntVar[m_i_dias];
            for (int i = 0; i < m_i_trabajadores; i++)
                for (int j = 0; j < m_i_dias; j++)
                    m_ocupacion_fin_de_semana_por_trabajador_y_dia[i][j] = m_s.MakeBoolVar("descansa");

            // INDICA SI EL TRABAJADOR DESCANSA TODOS LOS DIAS DEL FIN DE SEMANA O NO
            m_fin_de_semana_completo_por_trabajador_y_dia = new IntVar[m_i_trabajadores][];
            for (int i = 0; i < m_i_trabajadores; i++)
                m_fin_de_semana_completo_por_trabajador_y_dia[i] = new IntVar[m_i_dias];
            for (int i = 0; i < m_i_trabajadores; i++)
                for (int j = 0; j < m_i_dias; j++)
                    m_fin_de_semana_completo_por_trabajador_y_dia[i][j] = m_s.MakeBoolVar("descansa");
        }

        private void SincronizarMatrices()
        {
            // SINCRONIZAR LA MATRIZ 1 A 2
            for (int i = 0; i < m_i_dias; i++)
            {
                IntVar[] trabajadores_asignados_en_un_dia = m_trabajador_por_dia_y_puesto[i];
                for (int j = 0; j < m_i_trabajadores; j++)
                {
                    IntVar puesto_asignado_al_trabajador_j_el_dia_i = m_puesto_por_dia_y_trabajador[i][j];
                    Constraint sincronizar = m_s.MakeIndexOfConstraint(trabajadores_asignados_en_un_dia, puesto_asignado_al_trabajador_j_el_dia_i, j);
                    m_s.Add(sincronizar);
                }
            }
            // SINCRONIZAR LA MATRIZ 3
            for (int i = 0; i < m_i_trabajadores; i++)
            {
                for (int j = 0; j < m_i_dias; j++)
                {
                    IntVar puesto_del_trabajador_i_dia_j = m_puesto_por_trabajador_y_dia[i][j];
                    IntVar puesto_del_dia_j_trabajador_i = m_puesto_por_dia_y_trabajador[j][i];
                    Constraint sincronizar = m_s.MakeEquality(puesto_del_trabajador_i_dia_j, puesto_del_dia_j_trabajador_i);
                    m_s.Add(sincronizar);
                }
            }

            // SINCRONIZAMOS LA MATRIZ DE TURNO_POR_TRABAJADOR_Y_DIA
            for (int i = 0; i < m_i_trabajadores; i++)
            {
                IntVar[] turnos_asignados_al_trabajador_i = m_turno_por_trabajador_y_dia[i];
                IntVar[] puestos_asignados_al_trabajador_i = m_puesto_por_trabajador_y_dia[i];
                for (int j = 0; j < m_i_dias; j++)
                {
                    IntVar turno_asignados_al_trabajador_i_el_dia_j = turnos_asignados_al_trabajador_i[j];
                    IntVar puestos_asignados_al_trabajador_i_el_dia_j = puestos_asignados_al_trabajador_i[j];

                    IntExpr exp = m_s.MakeElement(m_horario_por_puesto, puestos_asignados_al_trabajador_i_el_dia_j);
                    Constraint sincronizar = m_s.MakeEquality(turno_asignados_al_trabajador_i_el_dia_j, exp);
                    m_s.Add(sincronizar);
                }
            }

            //SINCRONIZAR LA MATRIZ QUE INDICA SI UN TRABAJADOR TRABAJA UN DIA O NO
            for (int i = 0; i < m_i_trabajadores; i++)
            {
                for (int j = 0; j < m_i_dias; j++)
                {
                    IntVar trabaja_el_trabajador_i_el_dia_j = m_ocupacion_por_trabajador_y_dia[i][j];
                    IntVar puesto_asignado_el_dia_j_al_trabajador_i = m_puesto_por_dia_y_trabajador[j][i];
                    Constraint sincronizar = m_s.MakeEquality(trabaja_el_trabajador_i_el_dia_j, m_s.MakeLess(puesto_asignado_el_dia_j_al_trabajador_i, m_i_ocupados));
                    m_s.Add(sincronizar);
                }
            }

            //SINCRONIZAR LA MATRIZ DE COSTES TOTALES
            for (int i = 0; i < m_i_trabajadores; i++)
            {
                for (int j = 0; j < m_i_dias; j++)
                {
                    IntVar ocupacion_trabajador_i_dia_j = m_ocupacion_por_trabajador_y_dia[i][j];
                    IntVar coste_total_trabajador_i_dia_j = m_coste_total_por_trabajador_y_dia[i][j];
                    int coste_trabajador_i_dia_j = m_coste_por_trabajador_y_dia[i][j];

                    Constraint sincronizar = m_s.MakeEquality(coste_total_trabajador_i_dia_j, m_s.MakeProd(ocupacion_trabajador_i_dia_j, coste_trabajador_i_dia_j));
                    m_s.Add(sincronizar);
                }
            }

            // SINCRONIZAMOS LAS MATRICES DE HORARIOS DE ENTRADA Y SALIDA
            for (int i = 0; i < m_i_trabajadores; i++)
            {
                IntVar[] puestos_asignados_al_trabajador_i = m_puesto_por_trabajador_y_dia[i];
                IntVar[] entradas_del_trabajador_i = m_entrada_por_trabajador_y_dia[i];
                IntVar[] salidas_del_trabajador_i = m_salida_por_trabajador_y_dia[i];
                for (int j = 0; j < m_i_dias; j++)
                {
                    IntVar entrada_del_trabajador_i_el_dia_j = entradas_del_trabajador_i[j];
                    IntVar salida_del_trabajador_i_el_dia_j = salidas_del_trabajador_i[j];
                    IntVar horas_trabajadas_el_trabajador_i_el_dia_j = m_minutos_de_trabajo_por_trabajador_y_dia[i][j];
                    IntVar puesto_asignado_al_trabajador_i_el_dia_j = puestos_asignados_al_trabajador_i[j];

                    IntExpr exp = m_s.MakeElement(m_entrada_salida_total_por_puesto[0], puesto_asignado_al_trabajador_i_el_dia_j);
                    Constraint sincronizar = m_s.MakeEquality(entrada_del_trabajador_i_el_dia_j, exp);
                    m_s.Add(sincronizar);

                    exp = m_s.MakeElement(m_entrada_salida_total_por_puesto[1], puesto_asignado_al_trabajador_i_el_dia_j);
                    sincronizar = m_s.MakeEquality(salida_del_trabajador_i_el_dia_j, exp);
                    m_s.Add(sincronizar);

                    exp = m_s.MakeElement(m_entrada_salida_total_por_puesto[2], puesto_asignado_al_trabajador_i_el_dia_j);
                    sincronizar = m_s.MakeEquality(horas_trabajadas_el_trabajador_i_el_dia_j, exp);
                    m_s.Add(sincronizar);
                }
            }

            /*
             * LAS HORAS DE TRABAJO ASIGNADAS A UN TURNO, YA NO SE CALCULAN DINÁMICAMENTE, SE ESPECIFICAN EN EL PROPIO TURNO
             *
            // SINCRONIZAMOS LA MATRIZ DE HORAS TOTALES DE TRABAJO
            for (int i = 0; i < m_i_trabajadores; i++)
            {
                IntVar[] entradas_del_trabajador_i = m_entrada_por_trabajador_y_dia[i];
                IntVar[] salidas_del_trabajador_i = m_salida_por_trabajador_y_dia[i];
                for (int j = 0; j < m_i_dias; j++)
                {
                    IntVar entrada_del_trabajador_i_el_dia_j = entradas_del_trabajador_i[j];
                    IntVar salida_del_trabajador_i_el_dia_j = salidas_del_trabajador_i[j];
                    IntVar horas_trabajadas_el_trabajador_i_el_dia_j = m_minutos_de_trabajo_por_trabajador_y_dia[i][j];
                    IntExpr exp = m_s.MakeDifference(salida_del_trabajador_i_el_dia_j, entrada_del_trabajador_i_el_dia_j);
                    Constraint sincronizar = m_s.MakeEquality(horas_trabajadas_el_trabajador_i_el_dia_j, exp);
                    m_s.Add(sincronizar);
                }
            }
            */

            // SINCRONIZAR LA MATRIZ QUE INDICA SI UN TRABAJADOR DESCANSA UN DIA DE FIN DE SEMANA O NO
            // LA MATRIZ ALMACENA UN 1 SI EL TRABAJADOR NO TRABAJA UN DIA MARCADO COMO FIN DE SEMANA
            // TAMBIÉN SINCRONIZA LA MATRIZ QUE ALMACENA SI DESCANSA TODOS LOS DIAS DEL FIN DE SEMANA
            for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
            {
                RoboticsReglasList rs = m_trabajadores[i].Reglas;
                for (int m = 0; m < rs.Count; m++)
                {
                    RoboticsRegla r = rs[m];
                    if (r.Tipo == RoboticsReglaTipo.establecer_fin_de_semana)
                    {
                        // ALMACENAMOS UN 1 SI EL TRABAJADOR DESCANSA UN DIA DE FIN DE SEMANA
                        for (int j = 0; j < m_i_dias; j++)
                        {
                            DateTime fecha = m_calendario.FechaInicial.AddDays(j);
                            if (fecha >= r.FechaInicio && fecha <= r.FechaFinal)
                            {
                                bool fin_de_semana = false;
                                for (int k = r.Valor; k < r.Valor + r.Valor2; k++)
                                {
                                    if (fecha.DayOfWeek == (DayOfWeek)(k % 7))
                                    {
                                        fin_de_semana = true;
                                        break;
                                    }
                                }
                                IntVar trabaja_el_trabajador_i_el_dia_j = m_ocupacion_fin_de_semana_por_trabajador_y_dia[i][j];
                                if (fin_de_semana)
                                {
                                    IntVar puesto_asignado_el_dia_j_al_trabajador_i = m_puesto_por_dia_y_trabajador[j][i];
                                    Constraint sincronizar = m_s.MakeEquality(trabaja_el_trabajador_i_el_dia_j, m_s.MakeGreaterOrEqual(puesto_asignado_el_dia_j_al_trabajador_i, m_i_ocupados));
                                    m_s.Add(sincronizar);
                                }
                                else
                                    trabaja_el_trabajador_i_el_dia_j.SetRange(0, 0);
                            }
                        }

                        // ALMACENAMOS UN 1 SI EL TRABAJADOR DESCANSA TODOS LOS DIAS DEL FIN DE SEMANA
                        for (int j = 0; j < m_i_dias; j++)
                        {
                            DateTime fecha = m_calendario.FechaInicial.AddDays(j);
                            if (fecha >= r.FechaInicio && fecha <= r.FechaFinal)
                            {
                                // TENEMOS EN CUENTA QUE EL FIN DE SEMANA ESTÉ COMPLETO EN LA LISTA DE DIAS
                                if (fecha.DayOfWeek == (DayOfWeek)r.Valor &&
                                    j + r.Valor2 < m_i_dias)
                                {
                                    IntExpr tro = null;
                                    for (int k = j; k < j + r.Valor2; k++)
                                    {
                                        IntVar descansa_el_trabajador_i_el_dia_k = m_ocupacion_fin_de_semana_por_trabajador_y_dia[i][k];
                                        if (tro == null)
                                            tro = m_s.MakeProd(descansa_el_trabajador_i_el_dia_k, 1);
                                        else
                                            tro = m_s.MakeProd(tro, descansa_el_trabajador_i_el_dia_k);
                                    }
                                    IntVar descansa_el_trabajador_i_el_fin_de_semana_j = m_fin_de_semana_completo_por_trabajador_y_dia[i][j];
                                    descansa_el_trabajador_i_el_fin_de_semana_j.SetName("descansa " + r.Valor2.ToString() + " dias");
                                    Constraint sincronizar = m_s.MakeEquality(descansa_el_trabajador_i_el_fin_de_semana_j, tro);
                                    m_s.Add(sincronizar);
                                }
                                else
                                    m_fin_de_semana_completo_por_trabajador_y_dia[i][j] = m_s.MakeIntConst(0);
                            }
                        }
                    }
                }
            }

            // INDICAMOS CON UN 1 SI EL TRABAJADOR TRABAJA EN TODA LA SOLUCIÓN
            // LA IDEA ES UTILIZAR ESTE VECTOR COMO OBJETIVO, PARA MINIMIZAR
            // SU VALOR Y FORZAR QUE SE BUSQUE UNA SOLUCIÓN CON EL MÍNIMO
            // NÚMERO DE TRABAJADORES POSIBLES
            for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
            {
                IntExpr tro = m_s.MakeSum(m_ocupacion_por_trabajador_y_dia[i]);
                tro = m_s.MakeGreater(tro, 0);
                m_s.Add(m_s.MakeEquality(m_ocupacion_por_trabajador_en_toda_la_solucion[i], tro));
            }

            // DEFINIMOS EL OBJETIVO
            m_coste = m_s.MakeIntVar(0, 1000000, "coste de la solucion");
            m_trabajadores_trabajando = m_s.MakeIntVar(0, 1000, "trabajadores trabajando");
            m_s.Add(m_s.MakeEquality(m_coste, m_s.MakeSum(m_flat_coste_total)));
            m_s.Add(m_s.MakeEquality(m_trabajadores_trabajando, m_s.MakeSum(m_ocupacion_por_trabajador_en_toda_la_solucion)));
        }

        private void CrearCondiciones()
        {
            // CONDICIONES
            DateTime fecha_inicio = m_calendario.FechaInicial;
            DateTime fecha_inicio_anterior = m_calendario.FechaInicial.AddDays((-1) * m_trabajadores[0].DiasAnteriores.Count);
            DateTime fecha_inicio_posterior = m_calendario.FechaInicial.AddDays(m_calendario.Count);

            // CADA PUESTO SOLO CON TRABAJADORES HABILITADOS
            // PERO EN EL CASO DE PUESTOS QUE NO SE OCUPAN UN DIA, SOLO SE PUEDE ASIGNAR
            // EL TRABAJADOR DUMMY. COMO UN TRABAJADOR SOLO PUEDE HACER UN PUESTO
            // HAY UN TRABAJADOR DUMMY DIFERENTE POR CADA POSIBLE PUESTO
            // ESTA CONDICIÓN NO TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("CADA PUESTO SOLO CON TRABAJADORES HABILITADOS", 0);
            for (int i = 0; i < m_i_dias; i++)
            {
                LogTitulo("dia " + i.ToString() + ":", 1);
                IntVar[] trabajadores_del_dia_i = m_trabajador_por_dia_y_puesto[i];
                for (int j = 0; j < m_i_puestos; j++)
                {
                    SetLogConstraintPrefix(m_nombre_puestos[j] + ":");
                    int disponibilidad = 0;
                    for (int m = 0; m < m_i_trabajadores; m++)
                        if (m_disponibilidad_dia_y_trabajador_y_puesto[i][m][j] > 0)
                            disponibilidad++;
                    int[] disponibilidad_del_puesto_j = new int[disponibilidad];
                    disponibilidad = 0;
                    for (int m = 0; m < m_i_trabajadores; m++)
                    {
                        if (m_disponibilidad_dia_y_trabajador_y_puesto[i][m][j] > 0)
                        {
                            disponibilidad_del_puesto_j[disponibilidad] = m;
                            disponibilidad++;
                        }
                    }

                    IntVar trabajador_del_dia_i_y_puesto_j = trabajadores_del_dia_i[j];
                    if (m_necesidad_por_dia_y_puesto[i][j] == 1)
                    {
                        // PUESTO J NECESARIO EL DIA I
                        //IntExpr tro = null;
                        if (disponibilidad_del_puesto_j.Length > 1)
                        {
                            for (int m = 0; m < m_i_trabajadores; m++)
                            {
                                if (!disponibilidad_del_puesto_j.Contains(m))
                                    trabajador_del_dia_i_y_puesto_j.RemoveValue(m);
                            }

                            /*
                            for (int m = 0; m < disponibilidad_del_puesto_j.Length; m++)
                            {
                                Constraint c = m_s.MakeEquality(trabajador_del_dia_i_y_puesto_j, disponibilidad_del_puesto_j[m]);
                                if (tro == null)
                                    tro = c;
                                else
                                    tro = m_s.MakeMax(c, tro);
                            }
                            m_s.Add(LogConstraint(m_s.MakeEquality(tro, 1), "err=1000;dia=" + i.ToString() + ";puesto=" + j.ToString() + ";nombre=" + m_nombre_puestos[j]));
                            */

                            /*
                            // LA MATRIZ SOLO PUEDE TENER VALORES COMPRENDIDOS ENTRE LOS DISPONIBLES PARA ESE PUESTO
                            trabajador_del_dia_i_y_puesto_j.SetMin(disponibilidad_del_puesto_j[0]);
                            trabajador_del_dia_i_y_puesto_j.SetMax(disponibilidad_del_puesto_j[disponibilidad_del_puesto_j.Length - 1]);
                            */
                        }
                        else if (disponibilidad_del_puesto_j.Length == 1)
                        {
                            //trabajador_del_dia_i_y_puesto_j = m_s.MakeIntConst(disponibilidad_del_puesto_j[0]);
                            m_trabajador_por_dia_y_puesto[i][j] = m_s.MakeIntConst(disponibilidad_del_puesto_j[0], "trabajador");
                            //m_s.Add(LogConstraint(m_s.MakeEquality(m_trabajador_por_dia_y_puesto[i][j], disponibilidad_del_puesto_j[0]), "err=1001;dia=" + i.ToString() + ";puesto=" + j.ToString() + ";nombre=" + m_nombre_puestos[j]));
                        }
                        else
                        {
                            // NO HAY SOLUCIÓN
                            m_no_hay_solucion = true;
                        }
                    }
                    else
                    {
                        // EL PUESTO J NO ES NECESARIO EL DIA I
                        // SOLO PUEDE OCUPARLO EL TRABAJADOR DUMMY PARA EL PUESTO J
                        int dummy_j = m_i_trabajadores - m_i_ocupados + j;

                        // EL VALOR DE LA MATRIZ SOLO PUEDE SER EL VALOR DEL TRABAJADOR DUMMY
                        //trabajador_del_dia_i_y_puesto_j = m_s.MakeIntConst(dummy_j);
                        m_trabajador_por_dia_y_puesto[i][j] = m_s.MakeIntConst(dummy_j, "dummy" + dummy_j.ToString());
                        //m_s.Add(LogConstraint(m_s.MakeEquality(m_trabajador_por_dia_y_puesto[i][j], dummy_j), "err=1001;dia=" + i.ToString() + ";puesto=" + j.ToString() + ";nombre=" + m_nombre_puestos[j]));

                        // QUITAMOS DE LOS PUESTOS LIBRES LA POSIBILIDAD DE QUE APAREZCA EL USUARIO DUMMY ASIGNADO A ESTE DIA
                        for (int m = m_i_ocupados; m < m_i_trabajadores; m++)
                            trabajadores_del_dia_i[m].RemoveValue(dummy_j);
                    }
                }
            }

            for (int m = 0; m < m_i_dias; m++)
            {
                for (int i = m_i_ocupados; i < m_i_puestos; i++)
                {
                    if (m_trabajador_por_dia_y_puesto[m][i].Size() <= 1)
                    {
                        Errores.Add("ERROR DE ASIGNACION");
                        return;
                    }
                }
            }

            // CADA PUESTO SOLO UN TRABAJADOR
            // ESTA CONDICIÓN NO TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("CADA PUESTO SOLO UN TRABAJADOR Y SOLO TRABAJADORES HABILITADOS", 0);
            for (int i = 0; i < m_i_dias; i++)
            {
                SetLogConstraintPrefix("dia " + i.ToString() + ":");
                m_s.Add(LogConstraint(m_s.MakeAllDifferent(m_trabajador_por_dia_y_puesto[i]), "err=1002;dia=" + i.ToString()));
            }

            // EVITAMOS QUE SE HAGAN PERMUTACIONES CON LOS MISMOS PUESTOS
            // P.E. SI SE NECESITAN 2 OPERARIOS, ES LOS MISMO QUE SEA EL 1 Y 2, QUE EL 2 Y 1
            // LO MISMO OCURRE AL CONTRARIO, SI LA RELACION 1 Y 2 NO FUNCIONA, TAMPOCO LO HARÁ LA 2 Y 1
            // PARA ELLO, HACEMOS UN TRUCO MUY SIMPLE, Y ES QUE, DENTRO DEL MISMO GRUPO DE PUESTOS A CUBRIR, EL ID DE TRABAJADOR
            // DE LA "FILA" INFERIOR, TIENE QUE SER SUPERIOR AL ID DEL TRABAJADOR DE LA "FILA" SUPERIOR
            LogTitulo("ELIMINAR LAS PERMUTACIONES EN LOS PUESTOS OCUPADOS", 0);
            for (int m = 0; m < m_i_dias; m++)
            {
                LogTitulo("Dia: " + m.ToString(), 1);
                for (int i = 0; i < m_i_ocupados; i++)
                    m_todos_los_puestos[i].Procesado = false;
                for (int i = 0; i < m_i_ocupados; i++)
                {
                    RoboticsCalendarioPuesto p1 = m_todos_los_puestos[i];
                    int p1_index = i;
                    if (!p1.Procesado && m_trabajador_por_dia_y_puesto[m][p1_index].Size() > 1)
                    {
                        p1.Procesado = true;
                        for (int j = 0; j < m_i_ocupados; j++)
                        {
                            RoboticsCalendarioPuesto p2 = m_todos_los_puestos[j];
                            int p2_index = j;
                            if (!p2.Procesado && m_trabajador_por_dia_y_puesto[m][p2_index].Size() > 1)
                            {
                                if (p2.Puesto == p1.Puesto && p2.Turno == p1.Turno)
                                {
                                    p2.Procesado = true;
                                    SetLogConstraintPrefix(p1.Puesto + " " + p1.Turno + "-" + p1_index.ToString() + "-" + p2_index.ToString() + ":");
                                    m_s.Add(LogConstraint(m_s.MakeLess(m_trabajador_por_dia_y_puesto[m][p1_index], m_trabajador_por_dia_y_puesto[m][p2_index]), "err=10022;puesto=" + p1.Puesto + " " + p1.Turno + ";dia=" + m.ToString()));
                                    p1 = p2;
                                    p1_index = p2_index;
                                }
                            }
                        }
                    }
                }
            }

            // EVITAMOS LAS PERMUTACIONES CON LOS PUESTOS LIBRES
            // IMPORTANTE, EMPEZAMOS DESDE UNA POSICIÓN MÁS ALTA PARA HACER REFERENCIA A LA POSICIÓN ANTERIOR
            LogTitulo("ELIMINAR LAS PERMUTACIONES EN LOS PUESTOS LIBRES", 0);
            for (int m = 0; m < m_i_dias; m++)
            {
                LogTitulo("Dia: " + m.ToString(), 1);
                for (int i = m_i_ocupados + 1; i < m_i_puestos; i++)
                {
                    SetLogConstraintPrefix("Libre-" + (i - 1).ToString() + "-" + i.ToString() + ":");
                    m_s.Add(LogConstraint(m_s.MakeLess(m_trabajador_por_dia_y_puesto[m][i - 1], m_trabajador_por_dia_y_puesto[m][i]), "err=100222;puesto=libre;dia=" + m.ToString()));
                }
            }

            /*
            LogTitulo("ELIMINAR LAS PERMUTACIONES EN LOS PUESTOS LIBRES - MODO CUSTOM", 0);
            for (int m = 0; m < m_i_dias; m++)
            {
                LogTitulo("Dia: " + m.ToString(), 1);
                IntVar[] v = new IntVar[m_i_puestos - m_i_ocupados];
                int index = 0;
                for (int i = m_i_ocupados; i < m_i_puestos; i++)
                {
                    v[index] = m_trabajador_por_dia_y_puesto[m][i];
                    index++;
                }
                m_s.Add(new MyCT(m_s, v));
                break;
            }

            */

            // TRABAJADORES QUE NO TRABAJAN UN DIA DETERMINADO OBLIGATORIAMENTE
            // ESTA CONDICIÓN NO TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("TRABAJADORES QUE NO TRABAJAN UN DIA DETERMINADO OBLIGATORIAMENTE", 0);
            for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
            {
                for (int j = 0; j < m_i_dias; j++)
                {
                    if (m_trabajadores[i].Dias[j].Modo == RoboticsTrabajadorDiaModo.ausente)
                    {
                        LogTitulo(m_nombre_trabajadores[i], 1);
                        IntVar trabajador = m_s.MakeIntVar(i, i, "t");
                        trabajador.SetValue(i);
                        IntVar[] puestos_del_dia_j = m_trabajador_por_dia_y_puesto[j];
                        for (int m = 0; m < m_i_ocupados; m++)
                        {
                            if (puestos_del_dia_j[m].Min() != puestos_del_dia_j[m].Max())
                            {
                                puestos_del_dia_j[m].RemoveValue(trabajador.Value());
                                Constraint trabajador_no_trabaja_el_dia_j = m_s.MakeNonEquality(puestos_del_dia_j[m], trabajador);
                                SetLogConstraintPrefix("dia " + j.ToString() + " " + m_nombre_puestos[m] + ":");
                                m_s.Add(LogConstraint(trabajador_no_trabaja_el_dia_j, "err=1003;trabajador=" + i.ToString() + ";dia=" + j.ToString() + ";nombre=" + m_trabajadores[i].Nombre));
                            }
                            else if (trabajador.Value() == puestos_del_dia_j[m].Min())
                            {
                                // NO HAY SOLUCIÓN, QUEREMOS QUITAR DE UN PUESTO EL ÚNICO TRABAJADOR QUE PUEDE HACERLO
                                m_no_hay_solucion = true;
                            }
                        }
                    }
                }
            }

            // TRABAJADORES QUE TRABAJAN UN DIA DETERMINADO OBLIGATORIAMENTE
            // ESTA CONDICIÓN NO TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("TRABAJADORES QUE TRABAJAN UN DIA DETERMINADO OBLIGATORIAMENTE", 0);
            for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
            {
                for (int j = 0; j < m_i_dias; j++)
                {
                    if (m_trabajadores[i].Dias[j].Modo == RoboticsTrabajadorDiaModo.presente)
                    {
                        LogTitulo(m_nombre_trabajadores[i], 1);
                        /*
                        m_s.Add(LogConstraint(m_s.MakeEquality(m_ocupacion_por_trabajador_y_dia[i][j], 1), "err=1005;trabajador=" + i.ToString() + ";dia=" + j.ToString() + ";nombre=" + m_trabajadores[i].Nombre));
                        // QUITAMOS DE LOS PUESTOS LIBRES LA POSIBILIDAD DE QUE APAREZCA EL TRABAJADOR ESE DIA
                        IntVar[] trabajadores_del_dia_j = m_trabajador_por_dia_y_puesto[j];
                        for (int m = m_i_ocupados; m < m_i_trabajadores; m++)
                            trabajadores_del_dia_j[m].RemoveValue(i);
                        */

                        IntVar[] trabajadores_del_dia_j = m_trabajador_por_dia_y_puesto[j];

                        IntExpr tro = null;
                        Constraint c = null;

                        int puestos = 0;
                        for (int m = 0; m < m_i_ocupados; m++)
                        {
                            if (trabajadores_del_dia_j[m].Contains(i))
                            {
                                puestos++;
                                c = m_s.MakeEquality(trabajadores_del_dia_j[m], i);
                                if (tro == null)
                                    tro = c;
                                else
                                    tro = m_s.MakeMax(c, tro);
                            }
                        }

                        LogTitulo("puestos: " + puestos.ToString(), 2);

                        if (puestos == 1)
                            m_s.Add(LogConstraint(c, "err=1004;trabajador=" + i.ToString() + ";dia=" + j.ToString() + ";nombre=" + m_trabajadores[i].Nombre));
                        else if (puestos > 0)
                            m_s.Add(LogConstraint(m_s.MakeEquality(tro, 1), "err=1005;trabajador=" + i.ToString() + ";dia=" + j.ToString() + ";nombre=" + m_trabajadores[i].Nombre));
                        else if (puestos == 0)
                            LogLinea("ERROR");

                        // QUITAMOS DE LOS PUESTOS LIBRES LA POSIBILIDAD DE QUE APAREZCA EL TRABAJADOR ESE DIA
                        for (int m = m_i_ocupados; m < m_i_trabajadores; m++)
                            trabajadores_del_dia_j[m].RemoveValue(i);
                    }
                }
            }

            // UN TRABAJADOR COMO MÁXIMO XXX DIAS SEGUIDOS, POR TANTO, TOMANDO XXX+1 DIAS SEGUIDOS
            // EL PRODUCTO TIENE QUE SER 0
            // ESTA CONDICIÓN NO TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("UN TRABAJADOR COMO MÁXIMO XXX DIAS SEGUIDOS", 0);
            if (!m_desactivar_reglas.Contains(RoboticsReglaTipo.jornadas_seguidas_de_trabajo))
            {
                for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
                {
                    RoboticsTrabajador t = m_trabajadores[i];
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla d = t.Reglas[k];
                        if (d.Tipo == RoboticsReglaTipo.jornadas_seguidas_de_trabajo)
                        {
                            int dias_seguidos_de_trabajo = d.Valor;
                            int dias_seguidos_con_descanso = dias_seguidos_de_trabajo + 1;
                            LogTitulo(m_nombre_trabajadores[i] + " dias seguidos:" + dias_seguidos_de_trabajo.ToString(), 1);
                            for (int j = 0; j <= m_i_dias - dias_seguidos_con_descanso; j++)
                            {
                                DateTime d1 = m_calendario.FechaInicial.AddDays(j);
                                DateTime d2 = d1.AddDays(dias_seguidos_con_descanso);
                                if (d1 >= d.FechaInicio && d2 <= d.FechaFinal)
                                {
                                    IntExpr tro = null;
                                    for (int m = 1; m < dias_seguidos_con_descanso; m++)
                                    {
                                        if (tro == null)
                                            tro = m_s.MakeProd(m_ocupacion_por_trabajador_y_dia[i][j + m - 1], m_ocupacion_por_trabajador_y_dia[i][j + m]);
                                        else
                                            tro = m_s.MakeProd(tro, m_ocupacion_por_trabajador_y_dia[i][j + m]);
                                    }
                                    SetLogConstraintPrefix("inicio dia (" + j.ToString() + ") " + d1.ToString("ddd dd") + ":");
                                    m_s.Add(LogConstraint(m_s.MakeEquality(tro, 0), "err=1005;regla=" + RoboticsReglaTipo.jornadas_seguidas_de_trabajo.ToString() + ";trabajador=" + i.ToString() + ";dia=" + j.ToString() + ";nombre=" + t.Nombre));
                                }
                            }
                        }
                    }
                }
            }
            else
                LogTitulo("REGLA DESACTIVADA", 1);

            // LUNES FESTIVO SI TRABAJA EL DOMINGO
            // EL PRODUCTO DE DIA TRABAJADO DOMINGO CON DIA TRABAJADO LUNES TIENE QUE SER 0
            // ESTA CONDICIÓN NO TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("LUNES FESTIVO SI TRABAJA EL DOMINGO", 0);
            for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
            {
                for (int j = 0; j < m_i_dias; j++)
                {
                    // MIRAMOS SI ES DOMINGO-LUNES
                    if (m_calendario.FechaInicial.AddDays(j).DayOfWeek == 0 && j + 1 < m_i_dias)
                    {
                        if (m_trabajadores[i].Dias[j].Modo == RoboticsTrabajadorDiaModo.si_domingo_presente_lunes_ausente)
                        {
                            LogTitulo(m_nombre_trabajadores[i], 1);
                            IntExpr tro = m_s.MakeProd(m_ocupacion_por_trabajador_y_dia[i][j], m_ocupacion_por_trabajador_y_dia[i][j + 1]);
                            m_s.Add(LogConstraint(m_s.MakeEquality(tro, 0), "err=1006;trabajador=" + i.ToString() + ";dia=" + j.ToString() + ";nombre=" + m_trabajadores[i].Nombre));
                        }
                    }
                }
            }

            // DESCANSO OBLIGATORIO ENTRE JORNADAS
            // EL CÁLCULO DE 24 HORAS ES EL SIGUIENTE:
            //      24-(hora salida dia 1 - hora entrada dia 2) > horas de descanso
            // PERO PARA SOPORTAR LOS TURNOS NOCTURNOS, LO HACEMOS EN 48 HORAS
            //      48-(hola salida dia 1 - hora entrada dia 2 + 24 horas)
            // A LAS DOS OPCIONES, LE RESTAMOS 1 HORA PARA QUE SOLO TENGA EN CUENTA EL TIEMPO DE DESCANSO
            // ESTA CONDICIÓN NO TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            //
            // IMPORTANTE
            // LOS DIAS LIBRES, EL TRABAJADOR TIENE UN HORARIO DE JORNADA DE (ES DECIR, LA ENTRADA POR LA SALIDA)
            //  ENTRADA: 48
            //  SALIDA: 0
            LogTitulo("DESCANSO ENTRE JORNADAS", 0);
            if (!m_desactivar_reglas.Contains(RoboticsReglaTipo.descanso_entre_jornadas))
            {
                for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
                {
                    RoboticsTrabajador t = m_trabajadores[i];
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla d = m_trabajadores[i].Reglas[k];
                        if (d.Tipo == RoboticsReglaTipo.descanso_entre_jornadas && d.Valor != 24 * 60)
                        {
                            LogTitulo(m_nombre_trabajadores[i], 1);
                            for (int j = 1; j < m_i_dias; j++)   // DESCARTAMOS EL PRIMER DIA
                            {
                                DateTime d1 = m_calendario.FechaInicial.AddDays(j);
                                if (d1 >= d.FechaInicio && d1 <= d.FechaFinal)
                                {
                                    /*
                                     * // CÁLCULO SOBRE 24 HORAS - NO TIENE EN CUENTA LOS TURNOS DE NOCHE
                                    SetLogConstraintPrefix("dia " + j.ToString() + " " + d1.ToString("ddd dd"));
                                    IntExpr salida_menos_entrada = m_s.MakeDifference(m_salida_por_trabajador_y_dia[i][j - 1], m_entrada_por_trabajador_y_dia[i][j]);
                                    IntExpr resta_24 = m_s.MakeDifference(24 * 60, salida_menos_entrada);
                                    m_s.Add(LogConstraint(m_s.MakeGreaterOrEqual(resta_24, d.Valor)));
                                    */
                                    SetLogConstraintPrefix("dia " + j.ToString() + " " + d1.ToString("ddd dd"));
                                    IntExpr salida_menos_entrada = m_s.MakeDifference(m_salida_por_trabajador_y_dia[i][j - 1], m_entrada_por_trabajador_y_dia[i][j]);
                                    IntExpr turno_de_noche = m_s.MakeSum(salida_menos_entrada, 24 * 60);
                                    IntExpr resta_48 = m_s.MakeDifference(48 * 60, turno_de_noche);
                                    m_s.Add(LogConstraint(m_s.MakeGreaterOrEqual(resta_48, d.Valor), "err=1007;regla=" + RoboticsReglaTipo.descanso_entre_jornadas.ToString() + ";trabajador=" + i.ToString() + ";dia=" + j.ToString() + ";nombre=" + t.Nombre));
                                }
                            }
                        }
                    }
                }
            }
            else
                LogTitulo("REGLA DESACTIVADA", 1);

            // MÁXIMO TIEMPO DE TRABAJO TENIENDO EN CUENTA EL ACUMULADO
            // ESTA CONDICIÓN NO TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("MÁXIMO TIEMPO DE TRABAJO", 0);
            if (!m_desactivar_reglas.Contains(RoboticsReglaTipo.maximo_tiempo_de_trabajo))
            {
                for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
                {
                    RoboticsTrabajador t = m_trabajadores[i];
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla d = m_trabajadores[i].Reglas[k];
                        if (d.Tipo == RoboticsReglaTipo.maximo_tiempo_de_trabajo && d.Valor != 0)
                        {
                            LogTitulo(m_nombre_trabajadores[i], 1);
                            int dias = 0;
                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d1 = m_calendario.FechaInicial.AddDays(j);
                                if (d1 >= d.FechaInicio && d1 <= d.FechaFinal)
                                    dias++;
                            }
                            IntVar[] v = new IntVar[dias + 1];
                            v[0] = m_s.MakeIntVar(d.Valor2, d.Valor2, "acumulado");
                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d1 = m_calendario.FechaInicial.AddDays(j);
                                if (d1 >= d.FechaInicio && d1 <= d.FechaFinal)
                                    v[j + 1] = m_minutos_de_trabajo_por_trabajador_y_dia[i][j];
                            }
                            IntExpr exp = m_s.MakeSum(v);
                            m_s.Add(LogConstraint(m_s.MakeLessOrEqual(exp, d.Valor), "err=1008;regla=" + RoboticsReglaTipo.maximo_tiempo_de_trabajo.ToString() + ";trabajador=" + i.ToString() + ";nombre=" + t.Nombre));
                        }
                    }
                }
            }
            else
                LogTitulo("REGLA DESACTIVADA", 1);

            // JORNADAS DE DESCANSO NO SEGUIDAS
            // ESTA CONDICIÓN SIIIIII TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("JORNADAS DE DESCANSO SEGUIDAS O NO", 0);
            if (!m_desactivar_reglas.Contains(RoboticsReglaTipo.jornadas_de_descanso))
            {
                for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
                {
                    RoboticsTrabajador t = m_trabajadores[i];
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla d = m_trabajadores[i].Reglas[k];
                        if (d.Tipo == RoboticsReglaTipo.jornadas_de_descanso && d.Valor != 0)
                        {
                            LogTitulo(m_nombre_trabajadores[i], 1);
                            int dias = 0;
                            int descanso_anteriores = 0;
                            int descanso_posteriores = 0;

                            for (int j = 0; j < t.DiasAnteriores.Count; j++)
                            {
                                DateTime d1 = fecha_inicio_anterior.AddDays(j);
                                if (d1 >= d.FechaInicio && d1 <= d.FechaFinal && t.DiasAnteriores[j].Modo == RoboticsTrabajadorDiaModo.ausente)
                                {
                                    descanso_anteriores++;
                                }
                            }
                            for (int j = 0; j < t.DiasPosteriores.Count; j++)
                            {
                                DateTime d1 = fecha_inicio_posterior.AddDays(j);
                                if (d1 >= d.FechaInicio && d1 <= d.FechaFinal && t.DiasPosteriores[j].Modo == RoboticsTrabajadorDiaModo.ausente)
                                {
                                    descanso_posteriores++;
                                }
                            }

                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d1 = fecha_inicio.AddDays(j);
                                if (d1 >= d.FechaInicio && d1 <= d.FechaFinal)
                                    dias++;
                            }
                            IntVar[] v = new IntVar[dias];
                            int index = 0;
                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d1 = fecha_inicio.AddDays(j);
                                if (d1 >= d.FechaInicio && d1 <= d.FechaFinal)
                                {
                                    v[index] = m_ocupacion_por_trabajador_y_dia[i][j];
                                    index++;
                                }
                            }
                            IntExpr exp = m_s.MakeSum(v);
                            exp = m_s.MakeDifference(m_s.MakeIntConst(dias), exp);
                            exp = m_s.MakeSum(exp, m_s.MakeIntConst(descanso_anteriores));
                            exp = m_s.MakeSum(exp, m_s.MakeIntConst(descanso_posteriores));
                            m_s.Add(LogConstraint(m_s.MakeGreaterOrEqual(exp, d.Valor), "err=1009;regla=" + RoboticsReglaTipo.jornadas_de_descanso.ToString() + ";trabajador=" + i.ToString() + ";nombre=" + t.Nombre));
                        }
                    }
                }
            }
            else
                LogTitulo("REGLA DESACTIVADA", 1);

            // JORNADAS DE DESCANSO SEGUIDAS
            // ESTA CONDICIÓN NO TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("JORNADAS DE DESCANSO SEGUIDAS", 0);
            if (!m_desactivar_reglas.Contains(RoboticsReglaTipo.jornadas_seguidas_de_descanso_NO_USAR))
            {
                for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
                {
                    RoboticsTrabajador t = m_trabajadores[i];
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla d = m_trabajadores[i].Reglas[k];
                        if (d.Tipo == RoboticsReglaTipo.jornadas_seguidas_de_descanso_NO_USAR && d.Valor != 0 && d.Valor <= m_i_dias)
                        {
                            IntExpr tro = null;
                            LogTitulo(m_nombre_trabajadores[i] + " dias seguidos:" + d.Valor.ToString(), 1);
                            for (int j = 0; j <= m_i_dias - d.Valor; j++)
                            {
                                DateTime d1 = m_calendario.FechaInicial.AddDays(j);
                                DateTime d2 = d1.AddDays(d.Valor);
                                if (d1 >= d.FechaInicio && d2 <= d.FechaFinal)
                                {
                                    IntVar[] z = new IntVar[d.Valor];
                                    for (int m = 0; m < d.Valor; m++)
                                        z[m] = m_ocupacion_por_trabajador_y_dia[i][j + m];
                                    IntExpr tro2 = m_s.MakeSum(z);
                                    tro2 = m_s.MakeEquality(tro2, 0);
                                    if (tro == null)
                                        tro = tro2;
                                    else
                                        tro = m_s.MakeMax(tro2, tro);
                                }
                            }
                            if (tro != null)
                                m_s.Add(LogConstraint(m_s.MakeEquality(tro, 1), "err=1010;regla=" + RoboticsReglaTipo.jornadas_seguidas_de_descanso_NO_USAR.ToString() + ";trabajador=" + i.ToString() + ";nombre=" + t.Nombre));
                        }
                    }
                }
            }
            else
                LogTitulo("REGLA DESACTIVADA", 1);

            // JORNADAS DE TRABAJO NO SEGUIDAS
            // ESTA CONDICIÓN NO TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("JORNADAS DE TRABAJO NO SEGUIDAS", 0);
            if (!m_desactivar_reglas.Contains(RoboticsReglaTipo.jornadas_de_trabajo))
            {
                for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
                {
                    RoboticsTrabajador t = m_trabajadores[i];
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla d = m_trabajadores[i].Reglas[k];
                        if (d.Tipo == RoboticsReglaTipo.jornadas_de_trabajo && d.Valor != 0 && d.Valor <= m_i_dias)
                        {
                            LogTitulo(m_nombre_trabajadores[i], 1);
                            int dias = 0;
                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d1 = m_calendario.FechaInicial.AddDays(j);
                                if (d1 >= d.FechaInicio && d1 <= d.FechaFinal)
                                    dias++;
                            }
                            IntVar[] v = new IntVar[dias];
                            int index = 0;
                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d1 = m_calendario.FechaInicial.AddDays(j);
                                if (d1 >= d.FechaInicio && d1 <= d.FechaFinal)
                                {
                                    v[index] = m_ocupacion_por_trabajador_y_dia[i][j];
                                    index++;
                                }
                            }
                            IntExpr exp = m_s.MakeSum(v);
                            m_s.Add(LogConstraint(m_s.MakeGreaterOrEqual(exp, d.Valor), "err=1011;regla=" + RoboticsReglaTipo.jornadas_de_trabajo.ToString() + ";trabajador=" + i.ToString() + ";nombre=" + t.Nombre));
                        }
                    }
                }
            }
            else
                LogTitulo("REGLA DESACTIVADA", 1);

            // MÁXIMO DE TURNOS DE UN TIPO
            // ESTA CONDICIÓN SIIIII TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("MÁXIMO DE TURNOS DE UN TIPO", 0);
            if (!m_desactivar_reglas.Contains(RoboticsReglaTipo.maximo_jornadas_con_turno))
            {
                for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
                {
                    RoboticsTrabajador t = m_trabajadores[i];
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla d = m_trabajadores[i].Reglas[k];
                        if (d.Tipo == RoboticsReglaTipo.maximo_jornadas_con_turno)
                        {
                            LogTitulo(m_nombre_trabajadores[i] + " máximo de turno:" + m_turnos.BuscarTurno(d.Valor).Nombre + " " + d.Valor2.ToString(), 1);

                            int dias = m_trabajadores[i].Dias.ContarElementosEntreFechas(fecha_inicio, d.FechaInicio, d.FechaFinal);
                            int[] turnos_dias_anteriores = m_trabajadores[i].DiasAnteriores.TurnosEntreFechas(fecha_inicio_anterior, d.FechaInicio, d.FechaFinal, m_turnos);
                            int[] turnos_dias_posteriores = m_trabajadores[i].DiasPosteriores.TurnosEntreFechas(fecha_inicio_posterior, d.FechaInicio, d.FechaFinal, m_turnos);
                            IntVar[] v = new IntVar[dias + turnos_dias_anteriores.Length + turnos_dias_posteriores.Length];
                            int index = 0;
                            for (int j = 0; j < turnos_dias_anteriores.Length; j++)
                            {
                                v[index] = m_s.MakeIntConst(turnos_dias_anteriores[j]);
                                index++;
                            }
                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d1 = fecha_inicio.AddDays(j);
                                if (d1 >= d.FechaInicio && d1 <= d.FechaFinal)
                                {
                                    v[index] = m_turno_por_trabajador_y_dia[i][j];
                                    index++;
                                }
                            }
                            for (int j = 0; j < turnos_dias_posteriores.Length; j++)
                            {
                                v[index] = m_s.MakeIntConst(turnos_dias_posteriores[j]);
                                index++;
                            }
                            m_s.Add(LogConstraint(m_s.MakeAtMost(v, d.Valor, d.Valor2), "err=1012;regla=" + RoboticsReglaTipo.maximo_jornadas_con_turno.ToString() + ";trabajador=" + i.ToString() + ";nombre=" + t.Nombre));
                        }
                    }
                }
            }
            else
                LogTitulo("REGLA DESACTIVADA", 1);

            // MÍNIMO DE TURNOS DE UN TIPO
            // ESTA CONDICIÓN SIIIIII TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("MÍNIMO DE TURNOS DE UN TIPO", 0);
            if (!m_desactivar_reglas.Contains(RoboticsReglaTipo.minimo_jornadas_con_turno))
            {
                for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
                {
                    RoboticsTrabajador t = m_trabajadores[i];
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla d = m_trabajadores[i].Reglas[k];
                        if (d.Tipo == RoboticsReglaTipo.minimo_jornadas_con_turno)
                        {
                            IntExpr tro = null;
                            LogTitulo(m_nombre_trabajadores[i] + " mínimo de turno:" + m_turnos.BuscarTurno(d.Valor).Nombre + " " + d.Valor2.ToString(), 1);
                            int[] turnos_dias_anteriores = m_trabajadores[i].DiasAnteriores.TurnosEntreFechas(fecha_inicio_anterior, d.FechaInicio, d.FechaFinal, m_turnos);
                            int[] turnos_dias_posteriores = m_trabajadores[i].DiasPosteriores.TurnosEntreFechas(fecha_inicio_posterior, d.FechaInicio, d.FechaFinal, m_turnos);

                            for (int j = 0; j < turnos_dias_anteriores.Length; j++)
                            {
                                if (turnos_dias_anteriores[j] == d.Valor)
                                {
                                    IntVar turno_deseado = m_s.MakeIntConst(d.Valor, "anterior");
                                    if (tro == null)
                                        tro = m_s.MakeIntConst(1);
                                    else
                                        tro = m_s.MakeSum(m_s.MakeIntConst(1), tro);
                                }
                            }
                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d1 = fecha_inicio.AddDays(j);
                                if (d1 >= d.FechaInicio && d1 <= d.FechaFinal)
                                {
                                    IntExpr turno_deseado = m_s.MakeEquality(m_turno_por_trabajador_y_dia[i][j], d.Valor);
                                    if (tro == null)
                                        tro = m_s.MakeSum(turno_deseado, 0);
                                    else
                                        tro = m_s.MakeSum(turno_deseado, tro);
                                }
                            }
                            for (int j = 0; j < turnos_dias_posteriores.Length; j++)
                            {
                                if (turnos_dias_posteriores[j] == d.Valor)
                                {
                                    IntVar turno_deseado = m_s.MakeIntConst(d.Valor, "posterior");
                                    if (tro == null)
                                        tro = m_s.MakeIntConst(1);
                                    else
                                        tro = m_s.MakeSum(m_s.MakeIntConst(1), tro);
                                }
                            }
                            m_s.Add(LogConstraint(m_s.MakeGreaterOrEqual(tro, d.Valor2), "err=1013;regla=" + RoboticsReglaTipo.minimo_jornadas_con_turno.ToString() + ";trabajador=" + i.ToString() + ";nombre=" + t.Nombre));
                        }
                    }
                }
            }
            else
                LogTitulo("REGLA DESACTIVADA", 1);

            // MÍNIMO DE FINES DE SEMANA
            // ESTA CONDICIÓN SIIIIII TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("MÍNIMO DE FINES DE SEMANA", 0);
            if (!m_desactivar_reglas.Contains(RoboticsReglaTipo.minimo_de_fines_de_semana))
            {
                for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
                {
                    int inicio_fin_de_semana = 0;
                    int duracion_fin_de_semana = 0;

                    RoboticsTrabajador t = m_trabajadores[i];
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla r = t.Reglas[k];
                        if (r.Tipo == RoboticsReglaTipo.establecer_fin_de_semana)
                        {
                            inicio_fin_de_semana = r.Valor;
                            duracion_fin_de_semana = r.Valor2;
                            break;
                        }
                    }
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla r = t.Reglas[k];
                        if (r.Tipo == RoboticsReglaTipo.minimo_de_fines_de_semana)
                        {
                            LogTitulo(m_nombre_trabajadores[i], 1);
                            int dias = 0;
                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d = m_calendario.FechaInicial.AddDays(j);
                                if (d >= r.FechaInicio && d <= r.FechaFinal)
                                    dias++;
                            }
                            IntVar[] v = new IntVar[dias + 2];    // PONEMOS EL VALOR ANTERIOR Y EL POSTERIOR (+2)
                            v[0] = m_s.MakeIntConst(t.DiasAnteriores.FinDeSemanaEntreFechas(fecha_inicio_anterior, r.FechaInicio, r.FechaFinal, inicio_fin_de_semana, duracion_fin_de_semana), "anterior");
                            v[v.Length - 1] = m_s.MakeIntConst(t.DiasPosteriores.FinDeSemanaEntreFechas(fecha_inicio_posterior, r.FechaInicio, r.FechaFinal, inicio_fin_de_semana, duracion_fin_de_semana), "posterior");
                            int index = 1;
                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d = m_calendario.FechaInicial.AddDays(j);
                                if (d >= r.FechaInicio && d <= r.FechaFinal)
                                {
                                    v[index] = m_fin_de_semana_completo_por_trabajador_y_dia[i][j];
                                    index++;
                                }
                            }
                            IntExpr suma = m_s.MakeSum(v);
                            m_s.Add(LogConstraint(m_s.MakeGreaterOrEqual(suma, r.Valor), "err=1014;regla=" + RoboticsReglaTipo.minimo_de_fines_de_semana.ToString() + ";trabajador=" + i.ToString() + ";nombre=" + t.Nombre));
                        }
                    }
                }
            }
            else
                LogTitulo("REGLA DESACTIVADA", 1);

            // MÁXIMO DE FINES DE SEMANA
            // ESTA CONDICIÓN SIIIIII TIENE EN CUENTA LOS DATOS ANTERIORES O POSTERIORES
            LogTitulo("MÁXIMO DE FINES DE SEMANA", 0);
            if (!m_desactivar_reglas.Contains(RoboticsReglaTipo.maximo_de_fines_de_semana))
            {
                for (int i = 0; i < m_i_trabajadores - m_i_ocupados; i++)
                {
                    int inicio_fin_de_semana = 0;
                    int duracion_fin_de_semana = 0;

                    RoboticsTrabajador t = m_trabajadores[i];
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla r = t.Reglas[k];
                        if (r.Tipo == RoboticsReglaTipo.establecer_fin_de_semana)
                        {
                            inicio_fin_de_semana = r.Valor;
                            duracion_fin_de_semana = r.Valor2;
                            break;
                        }
                    }
                    for (int k = 0; k < t.Reglas.Count; k++)
                    {
                        RoboticsRegla r = t.Reglas[k];
                        if (r.Tipo == RoboticsReglaTipo.maximo_de_fines_de_semana)
                        {
                            LogTitulo(m_nombre_trabajadores[i], 1);
                            int dias = 0;
                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d = m_calendario.FechaInicial.AddDays(j);
                                if (d >= r.FechaInicio && d <= r.FechaFinal)
                                    dias++;
                            }
                            IntVar[] v = new IntVar[dias + 2];    // PONEMOS EL VALOR ANTERIOR Y EL POSTERIOR (+2)
                            v[0] = m_s.MakeIntConst(t.DiasAnteriores.FinDeSemanaEntreFechas(fecha_inicio_anterior, r.FechaInicio, r.FechaFinal, inicio_fin_de_semana, duracion_fin_de_semana), "anterior");
                            v[v.Length - 1] = m_s.MakeIntConst(t.DiasPosteriores.FinDeSemanaEntreFechas(fecha_inicio_posterior, r.FechaInicio, r.FechaFinal, inicio_fin_de_semana, duracion_fin_de_semana), "posterior");
                            int index = 1;
                            for (int j = 0; j < m_i_dias; j++)
                            {
                                DateTime d = m_calendario.FechaInicial.AddDays(j);
                                if (d >= r.FechaInicio && d <= r.FechaFinal)
                                {
                                    v[index] = m_fin_de_semana_completo_por_trabajador_y_dia[i][j];
                                    index++;
                                }
                            }
                            IntExpr suma = m_s.MakeSum(v);
                            m_s.Add(LogConstraint(m_s.MakeLessOrEqual(suma, r.Valor), "err=1015;regla=" + RoboticsReglaTipo.maximo_de_fines_de_semana.ToString() + ";trabajador=" + i.ToString() + ";nombre=" + t.Nombre));
                        }
                    }
                }
            }
            else
                LogTitulo("REGLA DESACTIVADA", 1);

            // CUSTOM
            //m_s.constra
        }

        /*
        /// <summary>
        /// FUNCIÓN DESCARTADA. NO FUNCIONA.
        /// </summary>
        /// <param name="file_costes"></param>
        /// <param name="file_puestos"></param>
        /// <param name="file_necesidades"></param>
        /// <returns></returns>
        private bool InicializarDatos(string file_costes, string file_puestos, string file_necesidades)
        {
            if (!System.IO.File.Exists(file_costes))
                return false;
            System.IO.StreamReader r = System.IO.File.OpenText(file_costes);
            string l;
            int dias = -1;
            int trabajadores = 0;
            while ((l = r.ReadLine()) != null)
            {
                dias++;
                if (dias == 1)
                    trabajadores = l.Split(';').Length - 1;
            }
            r.Close();
            r = null;

            if (!System.IO.File.Exists(file_puestos))
                return false;
            r = System.IO.File.OpenText(file_puestos);

            int puestos = -1;
            while ((l = r.ReadLine()) != null)
            {
                puestos++;
            }
            r.Close();
            r = null;

            // COMO SOLO SE PUEDE ASIGNAR UN TRABAJO A UN TRABAJADOR, CREAMOS TANTOS TRABAJADORES DUMMYS COMO
            // POSIBLES PUESTOS A CUBRIR. CADA PUESTO NO OCUPADO, TENDRÁ UN DUMMY A CUBRIR.
            trabajadores += puestos;

            if (!System.IO.File.Exists(file_necesidades))
                return false;

            CrearMatrices(dias, puestos, trabajadores);

            return true;
        }
        */
        /*
        /// <summary>
        /// FUNCIÓN DESCARTADA. NO FUNCIONA.
        /// </summary>
        /// <param name="file_costes"></param>
        /// <param name="file_puestos"></param>
        /// <param name="file_necesidades"></param>
        public void CargarDatos(string file_costes, string file_puestos, string file_necesidades)
        {
            if (!InicializarDatos(file_costes, file_puestos, file_necesidades))
                return;

            System.IO.StreamReader r = System.IO.File.OpenText(file_costes);
            string l;
            int j = -1;
            while ((l = r.ReadLine()) != null)
            {
                string[] op = l.Split(';');
                if (j == -1)
                {
                    // linea de cabecera, tomamos el nombre de los trabajadores
                    for (int i = 0; i < m_i_trabajadores; i++)
                    {
                        if (i + 1 < op.Length)
                            m_nombre_trabajadores[i] = op[i + 1];
                        else
                            m_nombre_trabajadores[i] = "DUMMY " + i.ToString(); // CREAMOS TODOS LOS TRABAJADORES DUMMYS PARA CUBRIR LOS POSIBLES PUESTOS EN LOS QUE NO SE TRABAJA
                    }
                }
                else
                {
                    for (int i = 0; i < m_i_trabajadores; i++)
                    {
                        if (i + 1 < op.Length)
                            m_coste_por_trabajador_y_dia[i][j] = int.Parse(op[i + 1]);
                        else
                            m_coste_por_trabajador_y_dia[i][j] = 1; // EL TRABAJADOR DUMMY, CUANDO TRABAJA ES GRATIS
                    }
                }
                j++;
            }
            r.Close();

            r = System.IO.File.OpenText(file_puestos);
            j = -1;
            while ((l = r.ReadLine()) != null)
            {
                string[] op = l.Split(';');
                if (j > -1)
                {
                    // el primer elemento es el nombre del puesto
                    m_nombre_puestos[j] = op[0];
                    for (int i = 0; i < m_i_trabajadores; i++)
                    {
                        if (i + 1 < op.Length)
                            m_disponibilidad_dia_y_trabajador_y_puesto[0][i][j] = int.Parse(op[i + 1]);
                        else
                            m_disponibilidad_dia_y_trabajador_y_puesto[0][i][j] = 0; // EL TRABAJADOR DUMMY, NO PUEDE HACER NINGÚN PUESTO
                    }
                }
                j++;
            }
            r.Close();
            j = 0;
            for (int i = m_i_ocupados; i < m_nombre_puestos.Length; i++, j++)
                m_nombre_puestos[i] = "libre " + j.ToString();

            r = System.IO.File.OpenText(file_necesidades);
            j = -1;
            r.ReadLine();   // quitamos la primera linea de cabecera (contiene el nombre de los puestos)
            while ((l = r.ReadLine()) != null)
            {
                string[] op = l.Split(';');
                if (j == -1)
                {
                    // segunda linea de cabecera, tomamos el turno (M,T,N,...)
                    for (int i = 0; i < m_i_puestos; i++)
                    {
                        long v = 0;
                        if (i + 1 < op.Length)
                            v = op[i][0];
                        m_horario_por_puesto[i] = m_s.MakeIntVar(v, v, "turno");
                    }
                }
                else
                {
                    for (int i = 1; i < op.Length; i++)
                    {
                        m_necesidad_por_dia_y_puesto[j][i - 1] = int.Parse(op[i]);
                    }
                }
                j++;
            }
            r.Close();

            SincronizarMatrices();
            CrearCondiciones();
        }
        */

        public void Solucionar()
        {
            //m_db = m_s.MakePhase(m_flat, Solver.CHOOSE_FIRST_UNBOUND, Solver.ASSIGN_MIN_VALUE);
            m_db = m_s.MakePhase(m_flat, Solver.CHOOSE_MIN_SIZE, Solver.ASSIGN_MIN_VALUE);
            //m_db = m_s.MakePhase(m_flat, Solver.CHOOSE_LOWEST_MIN, Solver.ASSIGN_MIN_VALUE);
            //m_db = m_s.MakePhase(m_flat, Solver.CHOOSE_PATH, Solver.ASSIGN_MIN_VALUE);
            //m_db = m_s.MakePhase(m_flat, Solver.CHOOSE_RANDOM, Solver.ASSIGN_MIN_VALUE);

            //m_db = m_s.MakePhase(m_flat, Solver.CHOOSE_MIN_SIZE_HIGHEST_MIN, Solver.ASSIGN_MIN_VALUE);
            //m_db = m_s.MakePhase(m_flat, Solver.CHOOSE_MIN_SIZE_LOWEST_MIN, Solver.ASSIGN_MIN_VALUE);
            // CHOOSE_MIN_SIZE_LOWEST_MAX

            m_objetivo_coste = m_s.MakeMinimize(m_coste, m_diferencia_coste_entre_soluciones);
            m_objetivo_num_trabajadores = m_s.MakeMinimize(m_trabajadores_trabajando, m_diferencia_coste_entre_soluciones);
            //m_s.NewSearch(m_db, m_objetivo);
            //m_lim = m_s.MakeTimeLimit(5 * 1000);

            SearchLimitParameters par = m_s.MakeDefaultSearchLimitParameters();
            par.Time = 60 * 1000;
            //par.SmartTimeCheck = true;
            m_lim = m_s.MakeLimit(par);
            m_init_search_time = Environment.TickCount;

            m_s.NewSearch(m_db, m_objetivo_coste, m_lim);
            //m_s.NewSearch(m_db, m_objetivo, m_lim, m_s.MakeConstantRestart(1000000));
        }

        public bool SiguienteSolucion()
        {
            return SiguienteSolucion(5);
        }

        public bool SiguienteSolucion(int segundos)
        {
            if (m_no_hay_solucion)
            {
                m_s.EndSearch();
                return false;
            }
            if (segundos < 5)
                segundos = 5;
            SearchLimitParameters par = m_s.MakeDefaultSearchLimitParameters();
            long time_delta = Environment.TickCount - m_init_search_time + segundos * 1000;
            m_s.UpdateLimits(time_delta, par.Branches, par.Failures, par.Solutions, m_lim);

            bool res = m_s.NextSolution();
            if (!res)
                m_s.EndSearch();
            else
                AlmacenarSolucion();

            return res;
        }

        private void AlmacenarSolucion()
        {
            m_ultima_solucion.failures = m_s.Failures();
            m_ultima_solucion.branches = m_s.Branches();
            m_ultima_solucion.solucion_coste = m_coste.Value();
            m_ultima_solucion.solucion_trabajadores = m_trabajadores_trabajando.Value();
            for (int i = 0; i < m_i_dias; i++)
                for (int j = 0; j < m_i_ocupados; j++)
                    m_ultima_solucion.solucion_trabajador_por_dia_y_puesto[i][j] = m_trabajador_por_dia_y_puesto[i][j].Value();

            for (int i = 0; i < m_i_trabajadores; i++)
                for (int j = 0; j < m_i_dias; j++)
                    m_ultima_solucion.solucion_ocupacion_por_trabajador_y_dia[i][j] = m_ocupacion_por_trabajador_y_dia[i][j].Value();
        }

        public int MejorSolucion(int segundos)
        {
            SearchLimitParameters par = m_s.MakeDefaultSearchLimitParameters();
            long time_inicial = Environment.TickCount;
            int c = 0;
            while (true)
            {
                long time_restante = segundos * 1000 - (Environment.TickCount - time_inicial);
                long time_delta = Environment.TickCount - m_init_search_time + time_restante;
                m_s.UpdateLimits(time_delta, par.Branches, par.Failures, par.Solutions, m_lim);
                bool res = m_s.NextSolution();
                if (!res)
                {
                    m_s.EndSearch();
                    break;
                }
                else
                {
                    AlmacenarSolucion();
                    c++;
                }
            }
            return c;
        }

        public void Close()
        {
            if (m_s == null)
                return;
            m_s.Dispose();
            m_s = null;
            m_trabajador_por_dia_y_puesto = null;
            m_puesto_por_dia_y_trabajador = null;
            m_puesto_por_trabajador_y_dia = null;
            m_ocupacion_por_trabajador_y_dia = null;
            m_coste_por_trabajador_y_dia = null;
            m_coste_total_por_trabajador_y_dia = null;
            m_flat = null;
            m_flat_coste_total = null;
            m_nombre_trabajadores = null;
        }

        /// <summary>
        /// Número de puestos que tienen que ser ocupados
        /// </summary>
        public int Ocupados { get { return m_i_ocupados; } }
        /// <summary>
        /// Número de "slots" libres para asignar trabajadores que no trabajan
        /// </summary>
        public int Libres { get { return m_i_libres; } }
        public int NumTrabajadores { get { return m_i_trabajadores; } }
        public int Dias { get { return m_i_dias; } }
        /// <summary>
        /// Coste de la solución propuesta
        /// </summary>
        public long SolucionCoste
        {
            get
            {
                int falsos = 0;
                int coste = 0;
                for (int i = 0; i < m_i_dias; i++)
                {
                    for (int j = 0; j < m_i_ocupados; j++)
                    {
                        //IntVar v = m_trabajador_por_dia_y_puesto[i][j];
                        long v = m_ultima_solucion.solucion_trabajador_por_dia_y_puesto[i][j];
                        if (v < m_trabajadores.Count)
                        {
                            RoboticsTrabajador tr = m_trabajadores[(int)v];
                            if (!tr.TrabajadorReal)
                            {
                                falsos++;
                                coste += tr.CosteMedioTrabajadorNoReal;
                            }
                        }
                    }
                }
                //int m = m_trabajadores.CosteMaximo() + 1;
                int m = 1000;
                return m_ultima_solucion.solucion_coste - m * falsos + coste;
            }
        }
        public long SolucionTrabajadores { get { return m_ultima_solucion.solucion_trabajadores; } }
        /*
        public string TrabajadorDiaPuesto(int dia, int puesto)
        {
            //return m_nombre_trabajadores[m_trabajador_por_dia_y_puesto[dia][puesto].Value()];
            return m_nombre_trabajadores[m_ultima_solucion.solucion_trabajador_por_dia_y_puesto[dia][puesto]];
        }
        */

        public RoboticsTrabajadorAsignado TrabajadorDiaPuesto(int dia, int puesto)
        {
            RoboticsTrabajadorAsignado t = new RoboticsTrabajadorAsignado();
            int tid = (int)m_ultima_solucion.solucion_trabajador_por_dia_y_puesto[dia][puesto];
            if (tid < m_trabajadores.Count)
            {
                t.Necesario = true;
                t.Nombre = m_trabajadores[tid].Nombre;
                t.Id = m_trabajadores[tid].Id;
                t.Modo = m_trabajadores[tid].Dias[dia].Modo;
                t.Puesto = m_todos_los_puestos[puesto].Puesto;
                t.Horario = m_todos_los_puestos[puesto].Turno;
                t.UP = m_todos_los_puestos[puesto].UnidadProductiva;
            }
            else
            {
                t.Necesario = false;
                t.Nombre = m_nombre_trabajadores[tid];
                t.Id = tid.ToString();
                t.Modo = RoboticsTrabajadorDiaModo.disponible;
                t.Puesto = m_nombre_puestos[puesto];
                t.Horario = "";
                t.UP = "";
            }
            return t;
        }

        public string[] TrabajadoresLibres(int dia)
        {
            string[] res = new string[m_i_libres];
            int index = 0;
            for (int i = 0; i < m_i_trabajadores; i++)
            {
                //if (m_ocupacion_por_trabajador_y_dia[i][dia].Value() == 0)
                if (m_ultima_solucion.solucion_ocupacion_por_trabajador_y_dia[i][dia] == 0)
                {
                    res[index] = m_nombre_trabajadores[i] + "-" + i.ToString();
                    index++;
                }
            }
            return res;
        }

        /*
        public string Puesto(int puesto)
        {
            return m_nombre_puestos[puesto];
        }
        */

        private void PrepararDatos()
        {
            /*
            for (int i = 0; i < m_turnos.Count; i++)
                m_turnos[i].Id = i;
            */

            m_trabajadores.Sort(new RoboticsTrabajadorListComparer(m_modo_solucion == 0));

            // COMPROBAMOS QUE LOS DIAS EN LOS QUE UN TRABAJADOR ESTA "PRESENTE" OBLIGATORIAMENTE, EXISTE UNA SOLICITUD DE PUESTO PARA ÉL
            for (int i = 0; i < m_trabajadores.Count; i++)
            {
                RoboticsTrabajador t = m_trabajadores[i];
                for (int j = 0; j < t.Dias.Count; j++)
                {
                    RoboticsTrabajadorDia td = t.Dias[j];
                    // MIRAMOS QUE EL TRABAJADOR ESTÉ PRESENTE Y QUE HAYAN SUFICIENTES DIAS EN EL CALENDARIO PARA COMPROBARLO
                    if (td.Modo == RoboticsTrabajadorDiaModo.presente && j < m_calendario.Count)
                    {
                        int ct = m_trabajadores.ContarPresentes(j, td.Puestos[0].Puesto, td.Turnos[0].Turno);
                        int cc = 0;
                        if (!m_presentes_siempre_crea_puestos)
                            cc = m_calendario[j].Puestos.SumarPuestosSinUnidadProductiva(td.Puestos[0].Puesto, td.Turnos[0].Turno);
                        else
                            cc = m_calendario[j].Puestos.SumarPuestos("-1", td.Puestos[0].Puesto, td.Turnos[0].Turno);
                        if (ct > cc)
                        {
                            // NO EXISTE EL PUESTO, CREAMOS LA NECESIDAD
                            m_calendario[j].Puestos.Add(new RoboticsCalendarioPuesto("-1", td.Puestos[0].Puesto, ct - cc, td.Turnos[0].Turno));
                        }
                    }
                }
            }

            /*
            // TENEMOS EN CUENTA QUE NOS SOLICITEN EL MISMO PUESTO PARA EL MISMO DIA Y HORARIOS EN MÁS DE UNA LINEA
            bool analizar = true;
            for (int i = 0; i < m_calendario.Count; i++)
            {
                RoboticsCalendarioPuestosList pdia = m_calendario[i].Puestos;
                analizar = true;
                while (analizar)
                {
                    analizar = false;
                    for (int j = 0; j < pdia.Count; j++)
                    {
                        int c = pdia.ContarPuestos(pdia[j].UnidadProductiva, pdia[j].Puesto, pdia[j].Turno);
                        if (c > 1)
                        {
                            int[] l = pdia.IndexPuestos(pdia[j].UnidadProductiva, pdia[j].Puesto, pdia[j].Turno);
                            RoboticsCalendarioPuesto p = pdia[l[0]];
                            for (int m = l.Length - 1; m > 0; m--)
                            {
                                p.Cantidad += pdia[l[m]].Cantidad;
                                pdia.RemoveAt(l[m]);
                            }
                            analizar = true;
                            break;
                        }
                    }
                }
            }
            */
        }

        private void PrepararPuestos()
        {
            // CONTAMOS LOS TRABAJADORES DISPONIBLES PARA UN PUESTO
            for (int i = 0; i < m_todos_los_puestos.Count; i++)
            {
                RoboticsCalendarioPuesto p = m_todos_los_puestos[i];
                int[] ts = m_trabajadores.TrabajadoresHabilitados(0, p.Puesto, p.Turno);
                p.TrabajadoresDisponibles = ts.Length;
            }
            m_todos_los_puestos.Sort(new RoboticsCalendarioPuestosListComparer(false));
        }

        public void CargarObjetos()
        {
            m_log.Clear();
            m_errores.Clear();
            m_todos_los_puestos.Clear();

            PrepararDatos();
            if (m_errores.Count > 0)
                return;

            int dias = m_calendario.Count;
            int trabajadores = m_trabajadores.Count;
            for (int i = 0; i < m_calendario.Count; i++)
            {
                RoboticsCalendarioPuestosList pdia = m_calendario[i].Puestos;
                for (int j = 0; j < m_todos_los_puestos.Count; j++)
                    m_todos_los_puestos[j].Procesado = false;
                for (int j = 0; j < pdia.Count; j++)
                {
                    int c = 0;
                    for (int m = 0; m < m_todos_los_puestos.Count; m++)
                    {
                        if (m_todos_los_puestos[m].UnidadProductiva == pdia[j].UnidadProductiva &&
                           m_todos_los_puestos[m].Puesto == pdia[j].Puesto &&
                           m_todos_los_puestos[m].Turno == pdia[j].Turno &&
                           !m_todos_los_puestos[m].Procesado)
                        {
                            c++;
                        }
                    }
                    for (int m = c; m < pdia[j].Cantidad; m++)
                    {
                        RoboticsCalendarioPuesto p = new RoboticsCalendarioPuesto(pdia[j].UnidadProductiva, pdia[j].Puesto, 0, pdia[j].Turno);
                        p.Procesado = true;
                        m_todos_los_puestos.Add(p);
                    }

                    /*
                    int c = m_todos_los_puestos.ContarPuestos(pdia[j].UnidadProductiva, pdia[j].Puesto, pdia[j].Turno);
                    for (int m = c; m < pdia[j].Cantidad; m++)
                        m_todos_los_puestos.Add(new RoboticsCalendarioPuesto(pdia[j].UnidadProductiva, pdia[j].Puesto, 0, pdia[j].Turno));
                    */
                }
            }

            PrepararPuestos();

            int puestos = m_todos_los_puestos.Count;

            // COMO SOLO SE PUEDE ASIGNAR UN TRABAJO A UN TRABAJADOR, CREAMOS TANTOS TRABAJADORES DUMMYS COMO
            // POSIBLES PUESTOS A CUBRIR. CADA PUESTO NO OCUPADO, TENDRÁ UN DUMMY A CUBRIR.
            trabajadores += puestos;
            CrearMatrices(dias, puestos, trabajadores);

            LogTitulo("LISTA DE TRABAJADORES", 0);
            for (int i = 0; i < m_i_trabajadores; i++)
            {
                if (i < m_trabajadores.Count)
                    m_nombre_trabajadores[i] = m_trabajadores[i].Nombre;
                else
                    m_nombre_trabajadores[i] = "DUMMY " + i.ToString(); // CREAMOS TODOS LOS TRABAJADORES DUMMYS PARA CUBRIR LOS POSIBLES PUESTOS EN LOS QUE NO SE TRABAJA
                LogTitulo(m_nombre_trabajadores[i], 1);
            }

            for (int j = 0; j < m_calendario.Count; j++)
            {
                for (int i = 0; i < m_i_trabajadores; i++)
                {
                    if (i < m_trabajadores.Count)
                        m_coste_por_trabajador_y_dia[i][j] = m_trabajadores[i].Dias[j].Coste;
                    else
                        m_coste_por_trabajador_y_dia[i][j] = 0; // EL TRABAJADOR DUMMY, CUANDO TRABAJA ES GRATIS
                }
            }

            for (int i = 0; i < m_i_dias; i++)
                for (int j = 0; j < m_i_trabajadores; j++)
                    for (int m = 0; m < m_i_puestos; m++)
                        m_disponibilidad_dia_y_trabajador_y_puesto[i][j][m] = 0;

            for (int i = 0; i < m_i_dias; i++)
            {
                for (int j = 0; j < m_i_trabajadores; j++)
                {
                    if (j < m_trabajadores.Count)
                    {
                        RoboticsTrabajador t = m_trabajadores[j];
                        for (int m = 0; m < m_i_puestos; m++)
                        {
                            if (m < m_todos_los_puestos.Count &&
                                t.Dias[i].Modo != RoboticsTrabajadorDiaModo.ausente)
                            {
                                if (m_presentes_siempre_crea_puestos && ((m_todos_los_puestos[m].UnidadProductiva != "-1" && t.Dias[i].Modo == RoboticsTrabajadorDiaModo.presente) ||
                                                                        (m_todos_los_puestos[m].UnidadProductiva == "-1" && t.Dias[i].Modo != RoboticsTrabajadorDiaModo.presente)))
                                {
                                    // LOS PUESTOS NO CREADOS EXPRESAMENTE PARA UN PRESENTE, NO LOS PUEDEN OCUPAR LOS TRABAJADORES CON EL MODO=PRESENTE
                                }
                                else
                                {
                                    RoboticsTrabajadorDia d = t.Dias[i];
                                    if (d.Puestos.Count == 0)
                                    {
                                        if (d.Turnos.BuscarTurno(m_todos_los_puestos[m].Turno) != null &&
                                            t.Puestos.BuscarPuesto(m_todos_los_puestos[m].Puesto) != null)
                                        {
                                            m_disponibilidad_dia_y_trabajador_y_puesto[i][j][m] = 1;
                                        }
                                    }
                                    else
                                    {
                                        int cantidad = 0;
                                        if (m_presentes_siempre_crea_puestos && m_todos_los_puestos[m].UnidadProductiva == "-1" && t.Dias[i].Modo == RoboticsTrabajadorDiaModo.presente)
                                        {
                                            // SI EL TRABAJADOR ESTÁ PRESENTE, FORZAMOS QUE SOLO PUEDA HACER UN PUESTO
                                            // POR TANTO, MIRAMOS SI YA TIENE UN PUESTO ESTE DIA Y SI LO TIENE NO LE ASIGNAMOS NINGÚN OTRO
                                            for (int k = 0; k < m_todos_los_puestos.Count; k++)
                                            {
                                                if (m_disponibilidad_dia_y_trabajador_y_puesto[i][j][k] == 1)
                                                    cantidad++;
                                            }

                                            // ADEMAS, NOS ASEGURAMOS QUE NINGÚN OTRO TRABAJADOR TIENE ESE MISMO PUESTO EL MISMO DIA
                                            for (int k = 0; k < m_i_trabajadores; k++)
                                            {
                                                if (m_disponibilidad_dia_y_trabajador_y_puesto[i][k][m] == 1)
                                                    cantidad++;
                                            }
                                        }

                                        if (cantidad == 0)
                                        {
                                            if (d.Turnos.BuscarTurno(m_todos_los_puestos[m].Turno) != null &&
                                                d.Puestos.BuscarPuesto(m_todos_los_puestos[m].Puesto) != null)
                                            {
                                                m_disponibilidad_dia_y_trabajador_y_puesto[i][j][m] = 1;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                /*
                                if (m == j + m_i_ocupados)
                                    m_disponibilidad_dia_y_trabajador_y_puesto[i][j][m] = 1;
                                */
                                // PERMITIMOS QUE TODOS LOS PUESTOS LIBRES ESTÉN DESBLOQUEADOS PARA TODOS LOS TRABAJADORES
                                // ANTERIORMENTE, FIJÁBAMOS QUE CADA TRABAJADOR SOLO PUDIERA ESTAR TRABAJANDO O EN UN PUESTO LIBRE DETERMINADO
                                // EL PROBLEMA ES QUE ES INCOMPATIBLE CON EL PROCESO QUE EVITA LAS PERMUTACIONES
                                if (m >= m_i_ocupados)
                                    m_disponibilidad_dia_y_trabajador_y_puesto[i][j][m] = 1;
                            }
                        }
                    }
                    else
                    {
                        // LOS TRABAJADORES DUMMY SOLO PUEDEN HACER UN PUESTO (EL QUE LE CORRESPONDE POR DUMMY)
                        //m_disponibilidad_dia_y_trabajador_y_puesto[i][j][j - (m_i_trabajadores - m_i_ocupados)] = 1;
                        for (int m = m_i_ocupados; m < m_i_puestos; m++)
                            m_disponibilidad_dia_y_trabajador_y_puesto[i][j][m] = 1;
                    }
                }
            }
            for (int i = 0; i < m_todos_los_puestos.Count; i++)
            {
                m_nombre_puestos[i] = m_todos_los_puestos[i].Puesto + " " + i.ToString() + " (" + m_todos_los_puestos[i].Turno + "-" + m_todos_los_puestos[i].UnidadProductiva + ")";
                RoboticsTurno t = m_turnos.BuscarTurno(m_todos_los_puestos[i].Turno);
                if (t != null)
                {
                    m_entrada_salida_total_por_puesto[0][i] = m_s.MakeIntConst(t.HoraEntrada, "entrada");
                    m_entrada_salida_total_por_puesto[1][i] = m_s.MakeIntConst(t.HoraSalida, "salida");
                    m_entrada_salida_total_por_puesto[2][i] = m_s.MakeIntConst(t.Horas, "horas");
                }
                else
                {
                    Errores.Add("el puesto " + m_nombre_puestos[i] + " no está correctamente informado");
                    return;
                }
            }

            for (int i = m_i_ocupados; i < m_nombre_puestos.Length; i++)
            {
                m_nombre_puestos[i] = "libre " + (i - m_i_ocupados).ToString();
                // IMPORTANTÍSIMO PARA QUE FUNCIONE ES QUE LOS LIBRES TENGAN HORA DE ENTRADA=48 Y SALIDA=0
                m_entrada_salida_total_por_puesto[0][i] = m_s.MakeIntConst(48 * 60, "entrada");
                m_entrada_salida_total_por_puesto[1][i] = m_s.MakeIntConst(0, "salida");
                m_entrada_salida_total_por_puesto[2][i] = m_s.MakeIntConst(0, "horas");
            }

            LogTitulo("LISTA DE PUESTOS", 0);
            for (int i = 0; i < m_nombre_puestos.Length; i++)
                LogTitulo(m_nombre_puestos[i], 1);

            LogTitulo("LISTA DE TURNOS", 0);
            for (int i = 0; i < m_turnos.Count; i++)
                LogTitulo(m_turnos[i].Nombre + " entrada:" + m_turnos[i].HoraEntrada + "(" + TimeSpan.FromMinutes(m_turnos[i].HoraEntrada).ToString() + ")" +
                                               " salida:" + m_turnos[i].HoraSalida + "(" + TimeSpan.FromMinutes(m_turnos[i].HoraSalida).ToString() + ")" +
                                               " horas:" + m_turnos[i].Horas + "(" + TimeSpan.FromMinutes(m_turnos[i].Horas).ToString() + ")", 1);

            for (int i = 0; i < m_i_puestos; i++)
            {
                long v = 0;
                if (i < m_todos_los_puestos.Count)
                {
                    RoboticsTurno t = m_turnos.BuscarTurno(m_todos_los_puestos[i].Turno);
                    if (t != null)
                        v = t.Id;
                }
                m_horario_por_puesto[i] = m_s.MakeIntConst(v, "turno");
            }

            for (int i = 0; i < m_i_dias; i++)
                for (int j = 0; j < m_i_puestos; j++)
                    m_necesidad_por_dia_y_puesto[i][j] = 0;
            for (int i = 0; i < m_calendario.Count; i++)
            {
                RoboticsCalendarioPuestosList pdia = m_calendario[i].Puestos;
                for (int j = 0; j < pdia.Count; j++)
                {
                    int[] index = m_todos_los_puestos.IndexPuestos(pdia[j].UnidadProductiva, pdia[j].Puesto, pdia[j].Turno);
                    if (index != null)
                    {
                        for (int m = 0; m < pdia[j].Cantidad; m++)
                        {
                            m_necesidad_por_dia_y_puesto[i][index[m]] = 1;
                        }
                    }
                }
                for (int j = m_i_ocupados; j < m_i_puestos; j++)
                    m_necesidad_por_dia_y_puesto[i][j] = 1;
            }

            SincronizarMatrices();
            CrearCondiciones();
        }

        public string ObtenerEstadisticas()
        {
            return "Failures: " + m_ultima_solucion.failures.ToString() +
                " Branches: " + m_ultima_solucion.branches.ToString();
        }

        private void LogLinea(string texto)
        {
            LogConsola(texto);
        }

        private void SetLogConstraintPrefix(string texto)
        {
            m_log_constraint_prefix = texto + " ";
        }

        private Constraint LogConstraint(Constraint c, string info)
        {
            LogLinea(m_log_constraint_prefix + c.ToString());
            m_log_constraint_prefix = "";
            if (c.ToString().ToLower().Contains("falseconstraint"))
                m_errores.Add("condición imposible:" + info);
            return c;
        }

        private void LogTitulo(string texto, int nivel)
        {
            string delta = "";
            if (nivel == 0)
                LogConsola("");
            else if (nivel == 1)
                delta = "--> ";
            else if (nivel == 2)
                delta = "----> ";
            LogConsola(delta + texto);
            if (nivel == 0)
            {
                string t = "";
                for (int i = 0; i < texto.Length; i++)
                    t += '-';
                LogConsola(t);
            }
        }

        /*
        /// <summary>
        /// Fecha correspondiente al dia[0] del calendario de necesidades
        /// </summary>
        public DateTime FechaInicial { get { return m_fecha_inicial; } set { m_fecha_inicial = value; } }
        /// <summary>
        /// Fecha inicial inclusive de todo el calendario de necesidades que el sistema buscará asignar puestos
        /// </summary>
        public DateTime FechaAnalisisInicial { get { return m_fecha_analisis_inicial; } set { m_fecha_analisis_inicial = value; } }
        /// <summary>
        /// Fecha final inclusive de todo el calendario de necesidades que el sistema buscará asignar puestos
        /// </summary>
        public DateTime FechaAnalisisFinal { get { return m_fecha_analisis_final; } set { m_fecha_analisis_final = value; } }
        */

        private void LogConsola(string texto)
        {
            //Console.WriteLine(texto);
            m_log.AppendLine(texto);
        }

        public string Log { get { return m_log.ToString(); } }

        public RoboticsTrabajador CrearTrabajadorQueFalta(string id, int coste_medio)
        {
            RoboticsTrabajador tr = new RoboticsTrabajador("FALTA " + m_trabajadores.Count.ToString());
            tr.TrabajadorReal = false;
            tr.CosteMedioTrabajadorNoReal = coste_medio;

            RoboticsTrabajadorTurnosList turnos = new RoboticsTrabajadorTurnosList();

            for (int i = 0; i < m_calendario.Count; i++)
            {
                RoboticsCalendarioDia d = m_calendario[i];
                for (int j = 0; j < d.Puestos.Count; j++)
                {
                    RoboticsCalendarioPuesto p = d.Puestos[j];
                    if (tr.Puestos.BuscarPuesto(p.Puesto) == null)
                        tr.Puestos.Add(new RoboticsTrabajadorPuesto(p.Puesto));
                    if (turnos.BuscarTurno(p.Turno) == null)
                        turnos.Add(new RoboticsTrabajadorTurno(p.Turno));
                }
            }

            string[] tu = new string[turnos.Count];
            for (int i = 0; i < tu.Length; i++)
                tu[i] = turnos[i].Turno;
            //int coste = m_trabajadores.CosteMaximo() + 1;
            int coste = 1000;
            for (int i = 0; i < m_calendario.Count; i++)
            {
                tr.Dias.Add(new RoboticsTrabajadorDia(coste, RoboticsTrabajadorDiaModo.disponible, tu));
            }

            m_trabajadores.Add(tr);
            return tr;
        }

        public void ExportarSolucion(string fichero)
        {
            RoboticsSerializar s = new RoboticsSerializar();
            s.Calendario = m_calendario;
            s.Trabajadores = m_trabajadores;
            s.Turnos = m_turnos;
            s.FechaInicioCalendario = m_calendario.FechaInicial;

            System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(RoboticsSerializar));
            System.IO.StreamWriter sw = System.IO.File.CreateText(fichero);
            xml.Serialize(sw, s);
            sw.Close();
        }

        public void ImportarSolucion(string fichero)
        {
            System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(RoboticsSerializar));
            System.IO.StreamReader sw = System.IO.File.OpenText(fichero);
            RoboticsSerializar s = (RoboticsSerializar)xml.Deserialize(sw);
            sw.Close();
            m_trabajadores = s.Trabajadores;
            m_turnos = s.Turnos;
            m_calendario = s.Calendario;
            m_calendario.FechaInicial = s.FechaInicioCalendario;
        }

        public int NumeroDeSoluciones { get { return 0; } }
        public List<string> Errores { get { return m_errores; } }

        /// <summary>
        /// Evalua para determinar si hay solución o no. Para ello, busca la solución más cara. En caso de que no haya, está claro que tampoco existirá una óptima.
        /// </summary>
        /// <returns></returns>
        public bool CheckHaySolucion()
        {
            Init(1);
            if (m_no_hay_solucion)
            {
                m_s.EndSearch();
                return false;
            }
            SearchLimitParameters par = m_s.MakeDefaultSearchLimitParameters();
            long time_delta = Environment.TickCount - m_init_search_time + 5 * 1000;
            m_s.UpdateLimits(time_delta, par.Branches, par.Failures, par.Solutions, m_lim);

            bool res = m_s.NextSolution();
            m_s.EndSearch();

            return res;
        }

        public List<RoboticsReglaTipo> DesactivarReglas { get { return m_desactivar_reglas; } }
        public bool PresentesSiempreCreaPuestos { get { return m_presentes_siempre_crea_puestos; } set { m_presentes_siempre_crea_puestos = value; } }
        public int MaximoDeFaltasEstimado { get { return m_todos_los_puestos.Count; } }
    }

    public class RoboticsTrabajadorAsignado
    {
        public string Nombre;
        public string Id;
        public RoboticsTrabajadorDiaModo Modo;
        public string UP;
        public string Puesto;
        public string Horario;
        public bool Necesario;
    }

    [Serializable]
    public class RoboticsSerializar
    {
        public DateTime FechaInicioCalendario;
        public RoboticsTurnosList Turnos;
        public RoboticsCalendarioDiasList Calendario;
        public RoboticsTrabajadorList Trabajadores;
    }

    public class RoboticsTrabajadorListComparer : IComparer<RoboticsTrabajador>
    {
        private bool m_ascendente;

        public RoboticsTrabajadorListComparer(bool ascendente)
        {
            m_ascendente = ascendente;
        }

        public int Compare(RoboticsTrabajador x, RoboticsTrabajador y)
        {
            int coste_x = CosteMedioTrabajador(x);
            int coste_y = CosteMedioTrabajador(y);
            int presentes_x = ContarModosTrabajador(x, RoboticsTrabajadorDiaModo.presente);
            int presentes_y = ContarModosTrabajador(y, RoboticsTrabajadorDiaModo.presente);

            // SIEMPRE PONEMOS PRIMERO A LOS QUE TIENEN DIAS OBLIGATORIOS

            if (m_ascendente)
            {
                if (presentes_x < presentes_y) return -1;
                else if (presentes_x > presentes_y) return 1;

                if (coste_x < coste_y) return -1;
                else if (coste_x > coste_y) return 1;
            }
            else
            {
                if (presentes_x < presentes_y) return -1;
                else if (presentes_x > presentes_y) return 1;

                if (coste_x > coste_y) return -1;
                else if (coste_x < coste_y) return 1;
            }
            // en este punto son iguales, ordenamos por el NOMBRE
            return string.Compare(x.Nombre, y.Nombre);
        }

        private int CosteMedioTrabajador(RoboticsTrabajador trabajador)
        {
            if (!trabajador.TrabajadorReal)
                return int.MaxValue;
            int res = 0;
            for (int i = 0; i < trabajador.Dias.Count; i++)
            {
                res += trabajador.Dias[i].Coste;
            }
            return res / trabajador.Dias.Count;
        }

        private int ContarModosTrabajador(RoboticsTrabajador trabajador, RoboticsTrabajadorDiaModo modo)
        {
            int res = 0;
            for (int i = 0; i < trabajador.Dias.Count; i++)
            {
                if (trabajador.Dias[i].Modo == modo)
                    res++;
            }
            return res;
        }
    }

    public class RoboticsTrabajadorList : List<RoboticsTrabajador>
    {
        public int CosteMaximo()
        {
            int res = 0;
            for (int i = 0; i < this.Count; i++)
            {
                RoboticsTrabajador t = this[i];
                if (t.TrabajadorReal)
                {
                    for (int j = 0; j < t.Dias.Count; j++)
                    {
                        if (t.Dias[j].Coste > res)
                            res = t.Dias[j].Coste;
                    }
                }
            }
            return res;
        }

        public int ContarPresentes(int dia, string puesto, string turno)
        {
            int res = 0;
            for (int i = 0; i < this.Count; i++)
            {
                RoboticsTrabajadorDia d = this[i].Dias[dia];
                if (d.Modo == RoboticsTrabajadorDiaModo.presente)
                {
                    bool hay_turno = false;
                    bool hay_puesto = false;

                    if (d.Turnos.BuscarTurno(turno) != null)
                        hay_turno = true;

                    if ((d.Puestos.Count > 0 && d.Puestos.BuscarPuesto(puesto) != null) ||
                        (d.Puestos.Count == 0 && this[i].Puestos.BuscarPuesto(puesto) != null))
                        hay_puesto = true;

                    if (hay_puesto && hay_turno)
                        res++;
                }
            }
            return res;
        }

        public int[] TrabajadoresHabilitados(int dia, string puesto, string turno)
        {
            List<int> l = new List<int>();
            for (int i = 0; i < this.Count; i++)
            {
                RoboticsTrabajador t = this[i];
                if (t.Dias[dia].Turnos.BuscarTurno(turno) != null)
                {
                    if ((t.Dias[dia].Puestos.Count > 0 && t.Dias[dia].Puestos.BuscarPuesto(puesto) != null) ||
                        (t.Dias[dia].Puestos.Count == 0 && t.Puestos.BuscarPuesto(puesto) != null))
                        l.Add(i);
                }
            }
            return l.ToArray();
        }
    }

    public class RoboticsTrabajador
    {
        private string m_nombre;
        private string m_id;
        private RoboticsTrabajadorDiasList m_dias;
        private RoboticsTrabajadorDiasList m_dias_anteriores;
        private RoboticsTrabajadorDiasList m_dias_posteriores;
        private RoboticsTrabajadorPuestosList m_puestos;
        private RoboticsReglasList m_reglas;
        private bool m_real = true;
        private int m_coste_medio_no_real = 0;

        public RoboticsTrabajador() : this("")
        {
        }

        public RoboticsTrabajador(string nombre) : this("", nombre)
        {
        }

        /// <summary>
        /// Información del trabajador
        /// </summary>
        /// <param name="nombre">Nombre del trabajador</param>
        public RoboticsTrabajador(string id, string nombre)
        {
            m_id = id;
            m_nombre = nombre;
            m_dias = new RoboticsTrabajadorDiasList();
            m_dias_anteriores = new RoboticsTrabajadorDiasList();
            m_dias_posteriores = new RoboticsTrabajadorDiasList();
            m_puestos = new RoboticsTrabajadorPuestosList();
            m_reglas = new RoboticsReglasList();
        }

        /// <summary>
        /// Nombre del trabajador
        /// </summary>
        public string Nombre { get { return m_nombre; } set { m_nombre = value; } }
        public string Id { get { return m_id; } set { m_id = value; } }
        /// <summary>
        /// Información de la disponibilidad de este trabajador para cada dia.
        /// El número de entradas (dias) en esta lista, tiene que ser igual en todos los trabajadores y con los días establecidos en las necesidades
        /// </summary>
        public RoboticsTrabajadorDiasList Dias { get { return m_dias; } }
        public RoboticsTrabajadorDiasList DiasAnteriores { get { return m_dias_anteriores; } }
        public RoboticsTrabajadorDiasList DiasPosteriores { get { return m_dias_posteriores; } }
        /// <summary>
        /// Información de los puestos que está capacitado el trabajador
        /// </summary>
        public RoboticsTrabajadorPuestosList Puestos { get { return m_puestos; } }
        public RoboticsReglasList Reglas { get { return m_reglas; } }
        internal bool TrabajadorReal { get { return m_real; } set { m_real = value; } }
        internal int CosteMedioTrabajadorNoReal { get { return m_coste_medio_no_real; } set { m_coste_medio_no_real = value; } }
    }

    public class RoboticsTrabajadorDiasList : List<RoboticsTrabajadorDia>
    {
        /// <summary>
        /// Contando desde la "fecha_inicial", retorna el número de elementos que se encuentran entre la "fecha1" y la "fecha2"
        /// </summary>
        /// <param name="fecha_inicial"></param>
        /// <param name="fecha1"></param>
        /// <param name="fecha2"></param>
        /// <returns></returns>
        public int ContarElementosEntreFechas(DateTime fecha_inicial, DateTime fecha1, DateTime fecha2)
        {
            int res = 0;
            for (int i = 0; i < this.Count; i++)
            {
                DateTime d1 = fecha_inicial.AddDays(i);
                if (d1 >= fecha1 && d1 <= fecha2)
                    res++;
            }
            return res;
        }

        public int[] TurnosEntreFechas(DateTime fecha_inicial, DateTime fecha1, DateTime fecha2, RoboticsTurnosList turnos)
        {
            int c = ContarElementosEntreFechas(fecha_inicial, fecha1, fecha2);
            if (c == 0)
                return new int[0];
            int[] res = new int[c];
            int index = 0;
            for (int i = 0; i < this.Count; i++)
            {
                DateTime d1 = fecha_inicial.AddDays(i);
                if (d1 >= fecha1 && d1 <= fecha2)
                {
                    RoboticsTurno t = turnos.BuscarTurno(this[i].Turnos[0].Turno);
                    res[index] = t.Id;
                    index++;
                }
            }
            return res;
        }

        public int FinDeSemanaEntreFechas(DateTime fecha_inicial, DateTime fecha1, DateTime fecha2, int dia_fin_de_semana_inicio, int duracion_fin_de_semana)
        {
            int res = 0;
            for (int i = 0; i < this.Count; i++)
            {
                DateTime d1 = fecha_inicial.AddDays(i);
                if (d1 >= fecha1 && d1 <= fecha2)
                {
                    bool fin_de_semana = true;
                    int index = 0;
                    for (int k = dia_fin_de_semana_inicio; k < dia_fin_de_semana_inicio + duracion_fin_de_semana; k++)
                    {
                        if (d1.AddDays(index).DayOfWeek != (DayOfWeek)(k % 7))
                        {
                            fin_de_semana = false;
                            break;
                        }
                        index++;
                    }
                    if (fin_de_semana)
                        res++;
                }
            }
            return res;
        }

        public int ContarDiasModo(RoboticsTrabajadorDiaModo modo)
        {
            int res = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Modo == modo)
                    res++;
            }
            return res;
        }
    }

    public enum RoboticsTrabajadorDiaModo
    {
        /// <summary>
        /// el trabajador está disponible este dia para trabajar. el motor puede asignalo o dejarlo libre en caso de que no haga falta
        /// </summary>
        disponible,
        /// <summary>
        /// el trabajador tiene que trabajar si-o-si ese dia. el motor tiene que asignarlo obligatoriamente.
        /// </summary>
        presente,
        /// <summary>
        /// el trabajador no puede trabajar si-o-si ese dia. el motor no´lo asignará obligatoriamente.
        /// </summary>
        ausente,
        /// <summary>
        /// el trabajador está disponible, pero si trabaja el domingo, el lunes inmediatamente seguido lo tendrá de fiesta. SOLO SE PUEDE ASIGNAR EN DOMINGOS.
        /// </summary>
        si_domingo_presente_lunes_ausente
    }

    public class RoboticsTrabajadorDia : ICloneable
    {
        private int m_coste;
        private RoboticsTrabajadorDiaModo m_modo;
        private RoboticsTrabajadorTurnosList m_turnos;
        private RoboticsTrabajadorPuestosList m_puestos;

        public RoboticsTrabajadorDia() : this(0, RoboticsTrabajadorDiaModo.disponible, null, null)
        {
        }

        public RoboticsTrabajadorDia(int coste, RoboticsTrabajadorDiaModo modo, string[] turnos) : this(coste, modo, turnos, null)
        {
        }

        public RoboticsTrabajadorDia(int coste, RoboticsTrabajadorDiaModo modo, string[] turnos, string[] puestos)
        {
            m_coste = coste;
            m_modo = modo;
            m_turnos = new RoboticsTrabajadorTurnosList();
            if (turnos != null)
            {
                for (int i = 0; i < turnos.Length; i++)
                    m_turnos.Add(new RoboticsTrabajadorTurno(turnos[i]));
            }
            m_puestos = new RoboticsTrabajadorPuestosList();
            if (puestos != null)
            {
                for (int i = 0; i < puestos.Length; i++)
                    m_puestos.Add(new RoboticsTrabajadorPuesto(puestos[i]));
            }
        }

        public int Coste { get { return m_coste; } set { m_coste = value; } }
        public RoboticsTrabajadorDiaModo Modo { get { return m_modo; } set { m_modo = value; } }
        public RoboticsTrabajadorTurnosList Turnos { get { return m_turnos; } }
        public RoboticsTrabajadorPuestosList Puestos { get { return m_puestos; } }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class RoboticsTrabajadorPuestosList : List<RoboticsTrabajadorPuesto>
    {
        public RoboticsTrabajadorPuesto BuscarPuesto(string puesto)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Puesto == puesto)
                    return this[i];
            }
            return null;
        }
    }

    public class RoboticsTrabajadorPuesto : ICloneable
    {
        private string m_puesto;

        public RoboticsTrabajadorPuesto() : this("")
        {
        }

        public RoboticsTrabajadorPuesto(string puesto)
        {
            m_puesto = puesto;
        }

        public string Puesto { get { return m_puesto; } set { m_puesto = value; } }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class RoboticsTrabajadorTurnosList : List<RoboticsTrabajadorTurno>
    {
        public RoboticsTrabajadorTurno BuscarTurno(string turno)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Turno == turno)
                    return this[i];
            }
            return null;
        }
    }

    public class RoboticsTrabajadorTurno
    {
        private string m_turno;

        public RoboticsTrabajadorTurno() : this("")
        {
        }

        public RoboticsTrabajadorTurno(string turno)
        {
            m_turno = turno;
        }

        public string Turno { get { return m_turno; } set { m_turno = value; } }
    }

    public class RoboticsCalendarioDiasList : List<RoboticsCalendarioDia>
    {
        private DateTime m_fecha_inicial;

        public DateTime FechaInicial { get { return m_fecha_inicial; } set { m_fecha_inicial = value; } }
    }

    public class RoboticsCalendarioDia : ICloneable
    {
        private RoboticsCalendarioPuestosList m_puestos;

        public RoboticsCalendarioDia()
        {
            m_puestos = new RoboticsCalendarioPuestosList();
        }

        public RoboticsCalendarioPuestosList Puestos { get { return m_puestos; } }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class RoboticsCalendarioPuestosList : List<RoboticsCalendarioPuesto>
    {
        public RoboticsCalendarioPuesto BuscarPuesto(string puesto, string turno)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Puesto == puesto && this[i].Turno == turno)
                    return this[i];
            }
            return null;
        }

        public int ContarPuestos(string puesto, string turno)
        {
            return ContarPuestos("", puesto, turno);
        }

        public int ContarPuestos(string puesto, string[] turnos)
        {
            return ContarPuestos("", puesto, turnos, false);
        }

        public int ContarPuestos(string unidad_productiva, string puesto, string turno)
        {
            return ContarPuestos(unidad_productiva, puesto, new string[] { turno }, false);
        }

        public int ContarPuestos(string unidad_productiva, string puesto, string[] turnos, bool sumar)
        {
            int c = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Puesto == puesto && turnos.Contains<string>(this[i].Turno) && this[i].UnidadProductiva == unidad_productiva)
                {
                    if (!sumar) c++;
                    else c += this[i].Cantidad;
                }
            }
            return c;
        }

        public int SumarPuestos(string puesto, string turno)
        {
            return SumarPuestos("", puesto, turno);
        }

        public int SumarPuestos(string unidad_productiva, string puesto, string turno)
        {
            return ContarPuestos(unidad_productiva, puesto, new string[] { turno }, true);
        }

        public int[] IndexPuestos(string puesto, string turno)
        {
            return IndexPuestos("", puesto, turno);
        }

        public int[] IndexPuestos(string unidad_productiva, string puesto, string turno)
        {
            List<int> l = new List<int>();
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Puesto == puesto && this[i].Turno == turno && this[i].UnidadProductiva == unidad_productiva)
                    l.Add(i);
            }
            if (l.Count > 0)
                return l.ToArray();
            else
                return null;
        }

        public string[] ListarPuestosUnicos()
        {
            List<string> l = new List<string>();
            for (int i = 0; i < this.Count; i++)
            {
                if (l.IndexOf(this[i].Puesto) < 0)
                    l.Add(this[i].Puesto);
            }
            return l.ToArray();
        }

        public int SumarPuestosSinUnidadProductiva(string puesto, string turno)
        {
            int c = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Puesto == puesto && this[i].Turno == turno)
                {
                    c += this[i].Cantidad;
                }
            }
            return c;
        }

        public RoboticsCalendarioPuesto BuscarPuestoPorTrabajador(string id_trabajador)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Trabajador == id_trabajador)
                    return this[i];
            }
            return null;
        }
    }

    public class RoboticsCalendarioPuestosListComparer : IComparer<RoboticsCalendarioPuesto>
    {
        private bool m_ascendente;

        public RoboticsCalendarioPuestosListComparer(bool ascendente)
        {
            m_ascendente = ascendente;
        }

        public int Compare(RoboticsCalendarioPuesto x, RoboticsCalendarioPuesto y)
        {
            if (m_ascendente)
            {
                if (x.TrabajadoresDisponibles < y.TrabajadoresDisponibles) return -1;
                else if (x.TrabajadoresDisponibles > y.TrabajadoresDisponibles) return 1;
            }
            else
            {
                if (x.TrabajadoresDisponibles > y.TrabajadoresDisponibles) return -1;
                else if (x.TrabajadoresDisponibles < y.TrabajadoresDisponibles) return 1;
            }
            return string.Compare(x.Puesto + x.Turno, y.Puesto + y.Turno);
        }
    }

    public class RoboticsCalendarioPuesto
    {
        private string m_puesto;
        private int m_cantidad;
        private string m_turno;
        private string m_unidad_productiva;
        private int m_trabajadores_disponibles;
        private bool m_procesado;
        private string m_trabajador;

        public RoboticsCalendarioPuesto() : this("", "", 0, "")
        {
        }

        public RoboticsCalendarioPuesto(string puesto, int cantidad, string turno) : this("", puesto, cantidad, turno)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="unidad_productiva">Unidad productiva a la que pertenece el puesto. Es importante que todos los puestos que pertenecen a una misma unidad productiva, tengan el mismo valor de unidad productiva. De esta forma, el sistema prioriza que las unidades productiva esten completas.</param>
        /// <param name="puesto">Puesto que tiene que ser ocupado (operador, etc).</param>
        /// <param name="cantidad">Cantidad de puestos a cubrir.</param>
        /// <param name="turno">Turno (horario) en el que tiene que ocuparse el puesto.</param>
        /// <param name="datos">Datos libres. Esta información no es tratada y es retornada cuando se consulta por el mismo puesto.</param>
        public RoboticsCalendarioPuesto(string unidad_productiva, string puesto, int cantidad, string turno)
        {
            m_unidad_productiva = unidad_productiva;
            //m_unidad_productiva = "";
            m_puesto = puesto;
            m_cantidad = cantidad;
            m_turno = turno;
            m_trabajadores_disponibles = -1;
            m_trabajador = null;
        }

        public string Puesto { get { return m_puesto; } set { m_puesto = value; } }
        public int Cantidad { get { return m_cantidad; } set { m_cantidad = value; } }
        public string Turno { get { return m_turno; } set { m_turno = value; } }
        public string UnidadProductiva { get { return m_unidad_productiva; } set { m_unidad_productiva = value; } }
        internal int TrabajadoresDisponibles { get { return m_trabajadores_disponibles; } set { m_trabajadores_disponibles = value; } }
        internal bool Procesado { get { return m_procesado; } set { m_procesado = value; } }
        public string Trabajador { get { return m_trabajador; } set { m_trabajador = value; } }
    }

    public class RoboticsTurnosList : List<RoboticsTurno>
    {
        public RoboticsTurno BuscarTurno(string nombre)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Nombre == nombre)
                    return this[i];
            }
            return null;
        }

        public RoboticsTurno BuscarTurno(int id)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Id == id)
                    return this[i];
            }
            return null;
        }

        public int PosicionTurno(string nombre)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Nombre == nombre)
                    return i;
            }
            return -1;
        }
    }

    public class RoboticsTurno
    {
        private string m_nombre;
        private int m_hora_entrada; // MINUTOS DESDE LAS 00:00
        private int m_hora_salida;  // MINUTOS DESDE LAS 00:00
        private int m_id;
        private int m_horas;

        public RoboticsTurno() : this("", 0, 0, 0, 0)
        {
        }

        public RoboticsTurno(string nombre, int id, int hora_entrada_en_minutos, int hora_salida_en_minutos) : this(nombre, id, hora_entrada_en_minutos, hora_salida_en_minutos, hora_salida_en_minutos - hora_entrada_en_minutos)
        {
        }

        /// <summary>
        /// Información del turno.
        /// </summary>
        /// <param name="nombre">Nombre del turno. todos los nombres tienen que ser diferentes</param>
        /// <param name="id">identificador del turno. todos los identificadores tienen que ser diferentes</param>
        /// <param name="hora_entrada_en_minutos">Hora de entrada del turno en minutos</param>
        /// <param name="hora_salida_en_minutos">Hora de salida del turno en minutos</param>
        /// <param name="horas_del_turno_en_minutos">Horas de duración del turno, con idenpendencia de la hora de entrada y salida</param>
        public RoboticsTurno(string nombre, int id, int hora_entrada_en_minutos, int hora_salida_en_minutos, int horas_del_turno_en_minutos)
        {
            m_nombre = nombre;
            m_hora_entrada = hora_entrada_en_minutos;
            m_hora_salida = hora_salida_en_minutos;
            m_id = id;
            m_horas = horas_del_turno_en_minutos;
        }

        public string Nombre { get { return m_nombre; } set { m_nombre = value; } }
        public int HoraEntrada { get { return m_hora_entrada; } set { m_hora_entrada = value; } }
        public int HoraSalida { get { return m_hora_salida; } set { m_hora_salida = value; } }
        public int Id { get { return m_id; } set { m_id = value; } }
        public int Horas { get { return m_horas; } set { m_horas = value; } }
    }

    public class RoboticsReglasList : List<RoboticsRegla> { }

    public enum RoboticsReglaTipo
    {
        /// <summary>
        /// Regla nula
        /// </summary>
        sin_regla,
        /// <summary>
        /// número de horas (en minutos) que tiene que descansar, como mínimo, el trabajador entre jornadas laborales.
        /// el trabajador puede descansar más horas que las indicadas.
        /// </summary>
        descanso_entre_jornadas,
        /// <summary>
        /// número de días de descanso que tiene que cumplir el trabajador como mínimo. Las jornadas no tienen porque ser seguidas.
        /// el trabajador puede hacer más descansos que los indicados.
        /// </summary>
        jornadas_de_descanso,
        /// <summary>
        /// número de días de descanso que tiene que cumplir el trabajador como mínimo. Las jornadas tienen que ser seguidas.
        /// el trabajador puede hacer más descansos que los indicados.
        /// </summary>
        jornadas_seguidas_de_descanso_NO_USAR,
        /// <summary>
        /// jornadas que el trabajador tiene que trabajar como mínimo. las jornadas no tienen que ser seguidas.
        /// el trabajador puede hacer más jornadas seguidas que las indicadas.
        /// </summary>
        jornadas_obligatorias_de_trabajo,
        /// <summary>
        /// jornadas que el trabajador puede trabajar como máximo. las jornadas no tienen que ser seguidas.
        /// el trabajador puede hacer menos jornadas seguidas que las indicadas.
        /// </summary>
        jornadas_de_trabajo,
        /// <summary>
        /// jornadas que el trabajador puede trabajar seguidas sin hacer un descanso como máximo.
        /// el trabajador puede hacer menos jornadas seguidas que las indicadas.
        /// </summary>
        jornadas_seguidas_de_trabajo,
        /// <summary>
        /// máximo de horas (en minutos) que el trabajador puede trabajar. Hay una propiedad del trabajador que permite inicializar el número de horas que ya tiene acumuladas.
        /// el trabajador puede hacer menos horas que las indicadas.
        /// </summary>
        maximo_tiempo_de_trabajo,
        /// <summary>
        /// máximo número de jornadas que el trabajador puede hacer del turno indicado.
        /// el id del turno se especifica en el parámetro valor
        /// el número máximo de turnos se especifica en el parámetro valor2
        /// </summary>
        maximo_jornadas_con_turno,
        /// <summary>
        /// mínimo número de jornadas que el trabajador puede hacer el turno indicado
        /// el id del turno se especifica en el parámetro valor
        /// el número mínimo de turnos se especifica en el parámetro valor2
        /// </summary>
        minimo_jornadas_con_turno,
        /// <summary>
        /// Establece los dias considerados "fin de semana". En "Valor" se introduce el número de dia en que comienza el fin de semana, inclusive. El domingo es el dia 0, el lunes el 1 y así sucesivamente.
        /// en "valor2" se introduce el número de dias que incluye el fin de semana.
        /// Por ejemplo, si el fin de semana del trabajador es "sábado y domingo", pondríamos "valor=6", "valor2=2"
        /// </summary>
        establecer_fin_de_semana,
        /// <summary>
        /// Establece el mínimo número de fines de semana que tiene que disfrutar el trabajador.
        /// Primero hay que establecer el parámetro "establecer_fin_de_semana"
        /// </summary>
        minimo_de_fines_de_semana,
        /// <summary>
        /// Establece el máximo número de fines de semana que puede disfrutar el trabajador.
        /// Primero hay que establecer el parámetro "establecer_fin_de_semana"
        /// </summary>
        maximo_de_fines_de_semana
    }

    public class RoboticsRegla : ICloneable
    {
        private DateTime m_fecha_inicio;
        private DateTime m_fecha_final;
        private int m_valor = 0;
        private int m_valor_2 = 0;
        private RoboticsReglaTipo m_tipo;

        public RoboticsRegla() : this(DateTime.Today, DateTime.Today, 0, 0, RoboticsReglaTipo.sin_regla)
        {
        }

        public RoboticsRegla(DateTime fecha_inicio, DateTime fecha_final, int valor, RoboticsReglaTipo modo) : this(fecha_inicio, fecha_final, valor, 0, modo)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fecha_inicio"></param>
        /// <param name="fecha_final"></param>
        /// <param name="valor"></param>
        /// <param name="valor2">En el caso de que el modo == maximo_tiempo_de_trabajo, valor2 indica el tiempo acumulado de trabajo del trabajador fuera del periodo a planificar
        ///                      En el caso de que el modo == maximo_jornadas_con_turno, valor2 indica el número máximo de jornadas que se puede estar asignado el turno
        ///                      En el caso de que el modo == minimo_jornadas_con_turno, valor2 indica el número mínimo de jornadas que se puede estar asignado el turno</param>
        /// <param name="modo"></param>
        public RoboticsRegla(DateTime fecha_inicio, DateTime fecha_final, int valor, int valor2, RoboticsReglaTipo modo)
        {
            m_fecha_inicio = fecha_inicio;
            m_fecha_final = fecha_final;
            m_valor = valor;
            m_valor_2 = valor2;
            m_tipo = modo;
        }

        public DateTime FechaInicio { get { return m_fecha_inicio; } set { m_fecha_inicio = value; } }
        public DateTime FechaFinal { get { return m_fecha_final; } set { m_fecha_final = value; } }
        public int Valor { get { return m_valor; } set { m_valor = value; } }
        public int Valor2 { get { return m_valor_2; } set { m_valor_2 = value; } }
        public RoboticsReglaTipo Tipo { get { return m_tipo; } set { m_tipo = value; } }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class RoboticsSoluciones
    {
        public long[][] solucion_trabajador_por_dia_y_puesto;
        public long[][] solucion_ocupacion_por_trabajador_y_dia;
        public long failures;
        public long branches;
        public long solucion_coste;
        public long solucion_trabajadores;

        public RoboticsSoluciones(int dias, int puestos, int trabajadores)
        {
            // ALMACENAMOS LA SOLUCIÓN. LO HACEMOS EN OTRA MATRIZ, PARA PODER ACCEDER CUANDO YA NO HAY MÁS SOLUCIONES
            // Y EL SISTEMA BORRA LAS TABLAS DE SOLUCIONES
            solucion_trabajador_por_dia_y_puesto = new long[dias][];
            for (int i = 0; i < dias; i++)
                solucion_trabajador_por_dia_y_puesto[i] = new long[puestos];

            // ALMACENAMOS LA SOLUCIÓN. LO HACEMOS EN OTRA MATRIZ, PARA PODER ACCEDER CUANDO YA NO HAY MÁS SOLUCIONES
            // Y EL SISTEMA BORRA LAS TABLAS DE SOLUCIONES
            solucion_ocupacion_por_trabajador_y_dia = new long[trabajadores][];
            for (int i = 0; i < trabajadores; i++)
                solucion_ocupacion_por_trabajador_y_dia[i] = new long[dias];
        }
    }

    public class MyCT : NetConstraint
    {
        private IntVar[] m_vector;
        private Demon m_demon;

        public MyCT(Solver s, IntVar[] vector) : base(s)
        {
            m_vector = vector;
        }

        public override void Post()
        {
            m_demon = solver().MakeConstraintInitialPropagateCallback(this);
            for (int i = 0; i < m_vector.Length; i++)
            {
                if (m_vector[i] != null)
                    m_vector[i].WhenBound(m_demon);
            }
        }

        public override void InitialPropagate()
        {
            for (int i = 1; i < m_vector.Length; i++)
            {
                if (m_vector[i - 1].Bound() && m_vector[i].Bound())
                {
                    long v1 = m_vector[i - 1].Value();
                    long v2 = m_vector[i].Value();
                    if (v1 > v2)
                    {
                        Console.WriteLine("BORRANDOOOOOOO");
                        m_vector[i - 1].RemoveValue(v1);
                    }
                }
            }
        }
    }
}