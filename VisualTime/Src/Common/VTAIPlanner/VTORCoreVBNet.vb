Imports System.Text
Imports Google.OrTools.ConstraintSolver

Namespace VTORCoreVBNET

    Public Class RoboticsOT

        Private m_s As Solver = Nothing

        Private m_trabajador_por_dia_y_puesto As IntVar()()

        Private m_puesto_por_dia_y_trabajador As IntVar()()

        Private m_puesto_por_trabajador_y_dia As IntVar()()

        Private m_ocupacion_por_trabajador_y_dia As IntVar()()

        Private m_ocupacion_fin_de_semana_por_trabajador_y_dia As IntVar()()

        Private m_fin_de_semana_completo_por_trabajador_y_dia As IntVar()()

        Private m_entrada_por_trabajador_y_dia As IntVar()()

        Private m_salida_por_trabajador_y_dia As IntVar()()

        Private m_entrada_salida_total_por_puesto As IntVar()()

        Private m_minutos_de_trabajo_por_trabajador_y_dia As IntVar()()

        Private m_disponibilidad_dia_y_trabajador_y_puesto As Integer()()()

        Private m_coste_por_trabajador_y_dia As Integer()()

        Private m_coste_total_por_trabajador_y_dia As IntVar()()

        Private m_ocupacion_por_trabajador_en_toda_la_solucion As IntVar()

        Private m_necesidad_por_dia_y_puesto As Integer()()

        Private m_horario_por_puesto As IntVar()

        Private m_turno_por_trabajador_y_dia As IntVar()()

        Private m_flat As IntVar()

        Private m_flat_coste_total As IntVar()

        Private m_nombre_trabajadores As String()

        Private m_nombre_puestos As String()

        Private m_coste As IntVar

        Private m_trabajadores_trabajando As IntVar

        Private m_db As DecisionBuilder

        Private m_objetivo_coste As OptimizeVar

        Private m_objetivo_num_trabajadores As OptimizeVar

        Private m_lim As SearchLimit

        Private m_init_search_time As Long

        Private m_i_dias As Integer

        Private m_i_puestos As Integer

        Private m_i_trabajadores As Integer

        Private m_i_libres As Integer

        Private m_i_ocupados As Integer

        Private m_trabajadores As RoboticsTrabajadorList

        Private m_calendario As RoboticsCalendarioDiasList

        Private m_turnos As RoboticsTurnosList

        Private m_todos_los_puestos As RoboticsCalendarioPuestosList

        Private m_ultima_solucion As RoboticsSoluciones

        Private m_diferencia_coste_entre_soluciones As Integer = 1

        Private m_log_constraint_prefix As String = ""

        Private m_no_hay_solucion As Boolean

        Private m_modo_solucion As Integer = 0

        Private m_presentes_siempre_crea_puestos As Boolean

        Private m_log As StringBuilder

        Private m_errores As List(Of String)

        Private m_desactivar_reglas As List(Of RoboticsReglaTipo)

        Public Sub New()
            m_trabajadores = New RoboticsTrabajadorList()
            m_calendario = New RoboticsCalendarioDiasList()
            m_turnos = New RoboticsTurnosList()
            m_log = New StringBuilder()
            m_todos_los_puestos = New RoboticsCalendarioPuestosList()
            m_errores = New List(Of String)()
            m_desactivar_reglas = New List(Of RoboticsReglaTipo)()
            m_presentes_siempre_crea_puestos = True
        End Sub

        Public ReadOnly Property Trabajadores As RoboticsTrabajadorList
            Get
                Return m_trabajadores
            End Get
        End Property

        Public ReadOnly Property Calendario As RoboticsCalendarioDiasList
            Get
                Return m_calendario
            End Get
        End Property

        Public ReadOnly Property Turnos As RoboticsTurnosList
            Get
                Return m_turnos
            End Get
        End Property

        Public ReadOnly Property Puestos As RoboticsCalendarioPuestosList
            Get
                Return m_todos_los_puestos
            End Get
        End Property

        Public Sub Init()
            Init(0)
        End Sub

        Public Sub Init(ByVal modo As Integer)
            m_no_hay_solucion = False
            m_modo_solucion = modo
            If m_s IsNot Nothing Then
                m_s.Dispose()
                m_s = New Solver(Environment.TickCount.ToString())
                CargarObjetos()
                Solucionar()
                Return
            End If

            Dim pp As ConstraintSolverParameters = Solver.DefaultSolverParameters()
            m_s = New Solver("")
        End Sub

        Private Sub CrearMatrices(ByVal dias As Integer, ByVal puestos As Integer, ByVal trabajadores As Integer)
            m_i_dias = dias
            m_i_puestos = trabajadores
            m_i_trabajadores = trabajadores
            m_i_libres = trabajadores - puestos
            m_i_ocupados = puestos
            m_ultima_solucion = New RoboticsSoluciones(m_i_dias, m_i_puestos, m_i_trabajadores)
            m_nombre_trabajadores = New String(m_i_trabajadores - 1) {}
            m_nombre_puestos = New String(m_i_puestos - 1) {}
            m_horario_por_puesto = New IntVar(m_i_puestos - 1) {}
            m_trabajador_por_dia_y_puesto = New IntVar(m_i_dias - 1)() {}
            For i As Integer = 0 To m_i_dias - 1
                m_trabajador_por_dia_y_puesto(i) = New IntVar(m_i_puestos - 1) {}
            Next

            For i As Integer = 0 To m_i_dias - 1
                For j As Integer = 0 To m_i_puestos - 1
                    m_trabajador_por_dia_y_puesto(i)(j) = m_s.MakeIntVar(0, m_i_trabajadores - 1, "trabajador")
                Next
            Next

            m_flat = New IntVar(m_i_dias * m_i_puestos - 1) {}
            Dim n As Integer = 0
            For i As Integer = 0 To m_i_dias - 1
                For j As Integer = 0 To m_i_puestos - 1
                    m_flat(n) = m_trabajador_por_dia_y_puesto(i)(j)
                    n += 1
                Next
            Next

            m_puesto_por_dia_y_trabajador = New IntVar(m_i_dias - 1)() {}
            For i As Integer = 0 To m_i_dias - 1
                m_puesto_por_dia_y_trabajador(i) = New IntVar(m_i_trabajadores - 1) {}
            Next

            For i As Integer = 0 To m_i_dias - 1
                For j As Integer = 0 To m_i_trabajadores - 1
                    m_puesto_por_dia_y_trabajador(i)(j) = m_s.MakeIntVar(0, m_i_puestos - 1, "puesto")
                Next
            Next

            m_puesto_por_trabajador_y_dia = New IntVar(m_i_trabajadores - 1)() {}
            For i As Integer = 0 To m_i_trabajadores - 1
                m_puesto_por_trabajador_y_dia(i) = New IntVar(m_i_dias - 1) {}
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_puesto_por_trabajador_y_dia(i)(j) = m_s.MakeIntVar(0, m_i_puestos - 1, "puesto")
                Next
            Next

            m_turno_por_trabajador_y_dia = New IntVar(m_i_trabajadores - 1)() {}
            For i As Integer = 0 To m_i_trabajadores - 1
                m_turno_por_trabajador_y_dia(i) = New IntVar(m_i_dias - 1) {}
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_turno_por_trabajador_y_dia(i)(j) = m_s.MakeIntVar(0, 1000, "turno")
                Next
            Next

            m_ocupacion_por_trabajador_y_dia = New IntVar(m_i_trabajadores - 1)() {}
            For i As Integer = 0 To m_i_trabajadores - 1
                m_ocupacion_por_trabajador_y_dia(i) = New IntVar(m_i_dias - 1) {}
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_ocupacion_por_trabajador_y_dia(i)(j) = m_s.MakeBoolVar("trabaja")
                Next
            Next

            m_ocupacion_por_trabajador_en_toda_la_solucion = New IntVar(m_i_trabajadores - m_i_ocupados - 1) {}
            For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                m_ocupacion_por_trabajador_en_toda_la_solucion(i) = m_s.MakeBoolVar("trabaja")
            Next

            m_coste_por_trabajador_y_dia = New Integer(m_i_trabajadores - 1)() {}
            For i As Integer = 0 To m_i_trabajadores - 1
                m_coste_por_trabajador_y_dia(i) = New Integer(m_i_dias - 1) {}
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_coste_por_trabajador_y_dia(i)(j) = 1
                Next
            Next

            m_coste_total_por_trabajador_y_dia = New IntVar(m_i_trabajadores - 1)() {}
            For i As Integer = 0 To m_i_trabajadores - 1
                m_coste_total_por_trabajador_y_dia(i) = New IntVar(m_i_dias - 1) {}
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_coste_total_por_trabajador_y_dia(i)(j) = m_s.MakeIntVar(0, 10000, "coste total")
                Next
            Next

            m_flat_coste_total = New IntVar(m_i_dias * (m_i_trabajadores - m_i_ocupados) - 1) {}
            n = 0
            For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_flat_coste_total(n) = m_coste_total_por_trabajador_y_dia(i)(j)
                    n += 1
                Next
            Next

            m_disponibilidad_dia_y_trabajador_y_puesto = New Integer(m_i_dias - 1)()() {}
            For i As Integer = 0 To m_i_dias - 1
                m_disponibilidad_dia_y_trabajador_y_puesto(i) = New Integer(m_i_trabajadores - 1)() {}
                For j As Integer = 0 To m_i_trabajadores - 1
                    m_disponibilidad_dia_y_trabajador_y_puesto(i)(j) = New Integer(m_i_puestos - 1) {}
                Next
            Next

            For i As Integer = 0 To m_i_dias - 1
                For j As Integer = 0 To m_i_trabajadores - 1
                    For m As Integer = 0 To m_i_puestos - 1
                        m_disponibilidad_dia_y_trabajador_y_puesto(i)(j)(m) = 1
                    Next
                Next
            Next

            m_necesidad_por_dia_y_puesto = New Integer(m_i_dias - 1)() {}
            For i As Integer = 0 To m_i_dias - 1
                m_necesidad_por_dia_y_puesto(i) = New Integer(m_i_puestos - 1) {}
            Next

            For i As Integer = 0 To m_i_dias - 1
                For j As Integer = 0 To m_i_puestos - 1
                    m_necesidad_por_dia_y_puesto(i)(j) = 1
                Next
            Next

            m_entrada_salida_total_por_puesto = New IntVar(2)() {}
            For i As Integer = 0 To 3 - 1
                m_entrada_salida_total_por_puesto(i) = New IntVar(m_i_puestos - 1) {}
            Next

            m_entrada_por_trabajador_y_dia = New IntVar(m_i_trabajadores - 1)() {}
            For i As Integer = 0 To m_i_trabajadores - 1
                m_entrada_por_trabajador_y_dia(i) = New IntVar(m_i_dias - 1) {}
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_entrada_por_trabajador_y_dia(i)(j) = m_s.MakeIntVar(0, 48 * 60, "entrada")
                Next
            Next

            m_salida_por_trabajador_y_dia = New IntVar(m_i_trabajadores - 1)() {}
            For i As Integer = 0 To m_i_trabajadores - 1
                m_salida_por_trabajador_y_dia(i) = New IntVar(m_i_dias - 1) {}
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_salida_por_trabajador_y_dia(i)(j) = m_s.MakeIntVar(0, 48 * 60, "salida")
                Next
            Next

            m_minutos_de_trabajo_por_trabajador_y_dia = New IntVar(m_i_trabajadores - 1)() {}
            For i As Integer = 0 To m_i_trabajadores - 1
                m_minutos_de_trabajo_por_trabajador_y_dia(i) = New IntVar(m_i_dias - 1) {}
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_minutos_de_trabajo_por_trabajador_y_dia(i)(j) = m_s.MakeIntVar(0, 4000 * 60, "horas")
                Next
            Next

            m_ocupacion_fin_de_semana_por_trabajador_y_dia = New IntVar(m_i_trabajadores - 1)() {}
            For i As Integer = 0 To m_i_trabajadores - 1
                m_ocupacion_fin_de_semana_por_trabajador_y_dia(i) = New IntVar(m_i_dias - 1) {}
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_ocupacion_fin_de_semana_por_trabajador_y_dia(i)(j) = m_s.MakeBoolVar("descansa")
                Next
            Next

            m_fin_de_semana_completo_por_trabajador_y_dia = New IntVar(m_i_trabajadores - 1)() {}
            For i As Integer = 0 To m_i_trabajadores - 1
                m_fin_de_semana_completo_por_trabajador_y_dia(i) = New IntVar(m_i_dias - 1) {}
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_fin_de_semana_completo_por_trabajador_y_dia(i)(j) = m_s.MakeBoolVar("descansa")
                Next
            Next
        End Sub

        Private Sub SincronizarMatrices()
            For i As Integer = 0 To m_i_dias - 1
                Dim trabajadores_asignados_en_un_dia As IntVar() = m_trabajador_por_dia_y_puesto(i)
                For j As Integer = 0 To m_i_trabajadores - 1
                    Dim puesto_asignado_al_trabajador_j_el_dia_i As IntVar = m_puesto_por_dia_y_trabajador(i)(j)
                    Dim sincronizar As Constraint = m_s.MakeIndexOfConstraint(trabajadores_asignados_en_un_dia, puesto_asignado_al_trabajador_j_el_dia_i, j)
                    m_s.Add(sincronizar)
                Next
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    Dim puesto_del_trabajador_i_dia_j As IntVar = m_puesto_por_trabajador_y_dia(i)(j)
                    Dim puesto_del_dia_j_trabajador_i As IntVar = m_puesto_por_dia_y_trabajador(j)(i)
                    Dim sincronizar As Constraint = m_s.MakeEquality(puesto_del_trabajador_i_dia_j, puesto_del_dia_j_trabajador_i)
                    m_s.Add(sincronizar)
                Next
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                Dim turnos_asignados_al_trabajador_i As IntVar() = m_turno_por_trabajador_y_dia(i)
                Dim puestos_asignados_al_trabajador_i As IntVar() = m_puesto_por_trabajador_y_dia(i)
                For j As Integer = 0 To m_i_dias - 1
                    Dim turno_asignados_al_trabajador_i_el_dia_j As IntVar = turnos_asignados_al_trabajador_i(j)
                    Dim puestos_asignados_al_trabajador_i_el_dia_j As IntVar = puestos_asignados_al_trabajador_i(j)
                    Dim exp As IntExpr = m_s.MakeElement(m_horario_por_puesto, puestos_asignados_al_trabajador_i_el_dia_j)
                    Dim sincronizar As Constraint = m_s.MakeEquality(turno_asignados_al_trabajador_i_el_dia_j, exp)
                    m_s.Add(sincronizar)
                Next
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    Dim trabaja_el_trabajador_i_el_dia_j As IntVar = m_ocupacion_por_trabajador_y_dia(i)(j)
                    Dim puesto_asignado_el_dia_j_al_trabajador_i As IntVar = m_puesto_por_dia_y_trabajador(j)(i)
                    Dim sincronizar As Constraint = m_s.MakeEquality(trabaja_el_trabajador_i_el_dia_j, m_s.MakeLess(puesto_asignado_el_dia_j_al_trabajador_i, m_i_ocupados))
                    m_s.Add(sincronizar)
                Next
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    Dim ocupacion_trabajador_i_dia_j As IntVar = m_ocupacion_por_trabajador_y_dia(i)(j)
                    Dim coste_total_trabajador_i_dia_j As IntVar = m_coste_total_por_trabajador_y_dia(i)(j)
                    Dim coste_trabajador_i_dia_j As Integer = m_coste_por_trabajador_y_dia(i)(j)
                    Dim sincronizar As Constraint = m_s.MakeEquality(coste_total_trabajador_i_dia_j, m_s.MakeProd(ocupacion_trabajador_i_dia_j, coste_trabajador_i_dia_j))
                    m_s.Add(sincronizar)
                Next
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                Dim puestos_asignados_al_trabajador_i As IntVar() = m_puesto_por_trabajador_y_dia(i)
                Dim entradas_del_trabajador_i As IntVar() = m_entrada_por_trabajador_y_dia(i)
                Dim salidas_del_trabajador_i As IntVar() = m_salida_por_trabajador_y_dia(i)
                For j As Integer = 0 To m_i_dias - 1
                    Dim entrada_del_trabajador_i_el_dia_j As IntVar = entradas_del_trabajador_i(j)
                    Dim salida_del_trabajador_i_el_dia_j As IntVar = salidas_del_trabajador_i(j)
                    Dim horas_trabajadas_el_trabajador_i_el_dia_j As IntVar = m_minutos_de_trabajo_por_trabajador_y_dia(i)(j)
                    Dim puesto_asignado_al_trabajador_i_el_dia_j As IntVar = puestos_asignados_al_trabajador_i(j)
                    Dim exp As IntExpr = m_s.MakeElement(m_entrada_salida_total_por_puesto(0), puesto_asignado_al_trabajador_i_el_dia_j)
                    Dim sincronizar As Constraint = m_s.MakeEquality(entrada_del_trabajador_i_el_dia_j, exp)
                    m_s.Add(sincronizar)
                    exp = m_s.MakeElement(m_entrada_salida_total_por_puesto(1), puesto_asignado_al_trabajador_i_el_dia_j)
                    sincronizar = m_s.MakeEquality(salida_del_trabajador_i_el_dia_j, exp)
                    m_s.Add(sincronizar)
                    exp = m_s.MakeElement(m_entrada_salida_total_por_puesto(2), puesto_asignado_al_trabajador_i_el_dia_j)
                    sincronizar = m_s.MakeEquality(horas_trabajadas_el_trabajador_i_el_dia_j, exp)
                    m_s.Add(sincronizar)
                Next
            Next

            For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                Dim rs As RoboticsReglasList = m_trabajadores(i).Reglas
                For m As Integer = 0 To rs.Count - 1
                    Dim r As RoboticsRegla = rs(m)
                    If r.Tipo = RoboticsReglaTipo.establecer_fin_de_semana Then
                        For j As Integer = 0 To m_i_dias - 1
                            Dim fecha As DateTime = m_calendario.FechaInicial.AddDays(j)
                            If fecha >= r.FechaInicio AndAlso fecha <= r.FechaFinal Then
                                Dim fin_de_semana As Boolean = False
                                For k As Integer = r.Valor To r.Valor + r.Valor2 - 1
                                    If fecha.DayOfWeek = CType((k Mod 7), DayOfWeek) Then
                                        fin_de_semana = True
                                        Exit For
                                    End If
                                Next

                                Dim trabaja_el_trabajador_i_el_dia_j As IntVar = m_ocupacion_fin_de_semana_por_trabajador_y_dia(i)(j)
                                If fin_de_semana Then
                                    Dim puesto_asignado_el_dia_j_al_trabajador_i As IntVar = m_puesto_por_dia_y_trabajador(j)(i)
                                    Dim sincronizar As Constraint = m_s.MakeEquality(trabaja_el_trabajador_i_el_dia_j, m_s.MakeGreaterOrEqual(puesto_asignado_el_dia_j_al_trabajador_i, m_i_ocupados))
                                    m_s.Add(sincronizar)
                                Else
                                    trabaja_el_trabajador_i_el_dia_j.SetRange(0, 0)
                                End If
                            End If
                        Next

                        For j As Integer = 0 To m_i_dias - 1
                            Dim fecha As DateTime = m_calendario.FechaInicial.AddDays(j)
                            If fecha >= r.FechaInicio AndAlso fecha <= r.FechaFinal Then
                                If fecha.DayOfWeek = CType(r.Valor, DayOfWeek) AndAlso j + r.Valor2 < m_i_dias Then
                                    Dim tro As IntExpr = Nothing
                                    For k As Integer = j To j + r.Valor2 - 1
                                        Dim descansa_el_trabajador_i_el_dia_k As IntVar = m_ocupacion_fin_de_semana_por_trabajador_y_dia(i)(k)
                                        If tro Is Nothing Then tro = m_s.MakeProd(descansa_el_trabajador_i_el_dia_k, 1) Else tro = m_s.MakeProd(tro, descansa_el_trabajador_i_el_dia_k)
                                    Next

                                    Dim descansa_el_trabajador_i_el_fin_de_semana_j As IntVar = m_fin_de_semana_completo_por_trabajador_y_dia(i)(j)
                                    descansa_el_trabajador_i_el_fin_de_semana_j.SetName("descansa " & r.Valor2.ToString() & " dias")
                                    Dim sincronizar As Constraint = m_s.MakeEquality(descansa_el_trabajador_i_el_fin_de_semana_j, tro)
                                    m_s.Add(sincronizar)
                                Else
                                    m_fin_de_semana_completo_por_trabajador_y_dia(i)(j) = m_s.MakeIntConst(0)
                                End If
                            End If
                        Next
                    End If
                Next
            Next

            For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                Dim tro As IntExpr = m_s.MakeSum(m_ocupacion_por_trabajador_y_dia(i))
                tro = m_s.MakeGreater(tro, 0)
                m_s.Add(m_s.MakeEquality(m_ocupacion_por_trabajador_en_toda_la_solucion(i), tro))
            Next

            m_coste = m_s.MakeIntVar(0, 1000000, "coste de la solucion")
            m_trabajadores_trabajando = m_s.MakeIntVar(0, 1000, "trabajadores trabajando")
            m_s.Add(m_s.MakeEquality(m_coste, m_s.MakeSum(m_flat_coste_total)))
            m_s.Add(m_s.MakeEquality(m_trabajadores_trabajando, m_s.MakeSum(m_ocupacion_por_trabajador_en_toda_la_solucion)))
        End Sub

        Private Sub CrearCondiciones()
            Dim fecha_inicio As DateTime = m_calendario.FechaInicial
            Dim fecha_inicio_anterior As DateTime = m_calendario.FechaInicial.AddDays((-1) * m_trabajadores(0).DiasAnteriores.Count)
            Dim fecha_inicio_posterior As DateTime = m_calendario.FechaInicial.AddDays(m_calendario.Count)
            LogTitulo("CADA PUESTO SOLO CON TRABAJADORES HABILITADOS", 0)
            For i As Integer = 0 To m_i_dias - 1
                LogTitulo("dia " & i.ToString() & ":", 1)
                Dim trabajadores_del_dia_i As IntVar() = m_trabajador_por_dia_y_puesto(i)
                For j As Integer = 0 To m_i_puestos - 1
                    SetLogConstraintPrefix(m_nombre_puestos(j) & ":")
                    Dim disponibilidad As Integer = 0
                    For m As Integer = 0 To m_i_trabajadores - 1
                        If m_disponibilidad_dia_y_trabajador_y_puesto(i)(m)(j) > 0 Then disponibilidad += 1
                    Next

                    Dim disponibilidad_del_puesto_j As Integer() = New Integer(disponibilidad - 1) {}
                    disponibilidad = 0
                    For m As Integer = 0 To m_i_trabajadores - 1
                        If m_disponibilidad_dia_y_trabajador_y_puesto(i)(m)(j) > 0 Then
                            disponibilidad_del_puesto_j(disponibilidad) = m
                            disponibilidad += 1
                        End If
                    Next

                    Dim trabajador_del_dia_i_y_puesto_j As IntVar = trabajadores_del_dia_i(j)
                    If m_necesidad_por_dia_y_puesto(i)(j) = 1 Then
                        If disponibilidad_del_puesto_j.Length > 1 Then
                            For m As Integer = 0 To m_i_trabajadores - 1
                                If Not disponibilidad_del_puesto_j.Contains(m) Then trabajador_del_dia_i_y_puesto_j.RemoveValue(m)
                            Next
                        ElseIf disponibilidad_del_puesto_j.Length = 1 Then
                            m_trabajador_por_dia_y_puesto(i)(j) = m_s.MakeIntConst(disponibilidad_del_puesto_j(0), "trabajador")
                        Else
                            m_no_hay_solucion = True
                        End If
                    Else
                        Dim dummy_j As Integer = m_i_trabajadores - m_i_ocupados + j
                        m_trabajador_por_dia_y_puesto(i)(j) = m_s.MakeIntConst(dummy_j, "dummy" & dummy_j.ToString())
                        For m As Integer = m_i_ocupados To m_i_trabajadores - 1
                            trabajadores_del_dia_i(m).RemoveValue(dummy_j)
                        Next
                    End If
                Next
            Next

            For m As Integer = 0 To m_i_dias - 1
                For i As Integer = m_i_ocupados To m_i_puestos - 1
                    If m_trabajador_por_dia_y_puesto(m)(i).Size() <= 1 Then
                        Errores.Add("ERROR DE ASIGNACION")
                        Return
                    End If
                Next
            Next

            LogTitulo("CADA PUESTO SOLO UN TRABAJADOR Y SOLO TRABAJADORES HABILITADOS", 0)
            For i As Integer = 0 To m_i_dias - 1
                SetLogConstraintPrefix("dia " & i.ToString() & ":")
                m_s.Add(LogConstraint(m_s.MakeAllDifferent(m_trabajador_por_dia_y_puesto(i)), "err=1002;dia=" & i.ToString()))
            Next

            LogTitulo("ELIMINAR LAS PERMUTACIONES EN LOS PUESTOS OCUPADOS", 0)
            For m As Integer = 0 To m_i_dias - 1
                LogTitulo("Dia: " & m.ToString(), 1)
                For i As Integer = 0 To m_i_ocupados - 1
                    m_todos_los_puestos(i).Procesado = False
                Next

                For i As Integer = 0 To m_i_ocupados - 1
                    Dim p1 As RoboticsCalendarioPuesto = m_todos_los_puestos(i)
                    Dim p1_index As Integer = i
                    If Not p1.Procesado AndAlso m_trabajador_por_dia_y_puesto(m)(p1_index).Size() > 1 Then
                        p1.Procesado = True
                        For j As Integer = 0 To m_i_ocupados - 1
                            Dim p2 As RoboticsCalendarioPuesto = m_todos_los_puestos(j)
                            Dim p2_index As Integer = j
                            If Not p2.Procesado AndAlso m_trabajador_por_dia_y_puesto(m)(p2_index).Size() > 1 Then
                                If p2.Puesto = p1.Puesto AndAlso p2.Turno = p1.Turno Then
                                    p2.Procesado = True
                                    SetLogConstraintPrefix(p1.Puesto & " " & p1.Turno & "-" + p1_index.ToString() & "-" + p2_index.ToString() & ":")
                                    m_s.Add(LogConstraint(m_s.MakeLess(m_trabajador_por_dia_y_puesto(m)(p1_index), m_trabajador_por_dia_y_puesto(m)(p2_index)), "err=10022;puesto=" & p1.Puesto & " " & p1.Turno & ";dia=" + m.ToString()))
                                    p1 = p2
                                    p1_index = p2_index
                                End If
                            End If
                        Next
                    End If
                Next
            Next

            LogTitulo("ELIMINAR LAS PERMUTACIONES EN LOS PUESTOS LIBRES", 0)
            For m As Integer = 0 To m_i_dias - 1
                LogTitulo("Dia: " & m.ToString(), 1)
                For i As Integer = m_i_ocupados + 1 To m_i_puestos - 1
                    SetLogConstraintPrefix("Libre-" & (i - 1).ToString() & "-" + i.ToString() & ":")
                    m_s.Add(LogConstraint(m_s.MakeLess(m_trabajador_por_dia_y_puesto(m)(i - 1), m_trabajador_por_dia_y_puesto(m)(i)), "err=100222;puesto=libre;dia=" & m.ToString()))
                Next
            Next

            LogTitulo("TRABAJADORES QUE NO TRABAJAN UN DIA DETERMINADO OBLIGATORIAMENTE", 0)
            For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                For j As Integer = 0 To m_i_dias - 1
                    If m_trabajadores(i).Dias(j).Modo = RoboticsTrabajadorDiaModo.ausente Then
                        LogTitulo(m_nombre_trabajadores(i), 1)
                        Dim trabajador As IntVar = m_s.MakeIntVar(i, i, "t")
                        trabajador.SetValue(i)
                        Dim puestos_del_dia_j As IntVar() = m_trabajador_por_dia_y_puesto(j)
                        For m As Integer = 0 To m_i_ocupados - 1
                            If puestos_del_dia_j(m).Min() <> puestos_del_dia_j(m).Max() Then
                                puestos_del_dia_j(m).RemoveValue(trabajador.Value())
                                Dim trabajador_no_trabaja_el_dia_j As Constraint = m_s.MakeNonEquality(puestos_del_dia_j(m), trabajador)
                                SetLogConstraintPrefix("dia " & j.ToString() & " " & m_nombre_puestos(m) & ":")
                                m_s.Add(LogConstraint(trabajador_no_trabaja_el_dia_j, "err=1003;trabajador=" & i.ToString() & ";dia=" + j.ToString() & ";nombre=" + m_trabajadores(i).Nombre))
                            ElseIf trabajador.Value() = puestos_del_dia_j(m).Min() Then
                                m_no_hay_solucion = True
                            End If
                        Next
                    End If
                Next
            Next

            LogTitulo("TRABAJADORES QUE TRABAJAN UN DIA DETERMINADO OBLIGATORIAMENTE", 0)
            For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                For j As Integer = 0 To m_i_dias - 1
                    If m_trabajadores(i).Dias(j).Modo = RoboticsTrabajadorDiaModo.presente Then
                        LogTitulo(m_nombre_trabajadores(i), 1)
                        Dim trabajadores_del_dia_j As IntVar() = m_trabajador_por_dia_y_puesto(j)
                        Dim tro As IntExpr = Nothing
                        Dim c As Constraint = Nothing
                        Dim puestos As Integer = 0
                        For m As Integer = 0 To m_i_ocupados - 1
                            If trabajadores_del_dia_j(m).Contains(i) Then
                                puestos += 1
                                c = m_s.MakeEquality(trabajadores_del_dia_j(m), i)
                                If tro Is Nothing Then tro = c Else tro = m_s.MakeMax(c, tro)
                            End If
                        Next

                        LogTitulo("puestos: " & puestos.ToString(), 2)
                        If puestos = 1 Then
                            m_s.Add(LogConstraint(c, "err=1004;trabajador=" & i.ToString() & ";dia=" + j.ToString() & ";nombre=" + m_trabajadores(i).Nombre))
                        ElseIf puestos > 0 Then
                            m_s.Add(LogConstraint(m_s.MakeEquality(tro, 1), "err=1005;trabajador=" & i.ToString() & ";dia=" + j.ToString() & ";nombre=" + m_trabajadores(i).Nombre))
                        ElseIf puestos = 0 Then
                            LogLinea("ERROR")
                        End If

                        For m As Integer = m_i_ocupados To m_i_trabajadores - 1
                            trabajadores_del_dia_j(m).RemoveValue(i)
                        Next
                    End If
                Next
            Next

            LogTitulo("UN TRABAJADOR COMO MÁXIMO XXX DIAS SEGUIDOS", 0)
            If Not m_desactivar_reglas.Contains(RoboticsReglaTipo.jornadas_seguidas_de_trabajo) Then
                For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                    Dim t As RoboticsTrabajador = m_trabajadores(i)
                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim d As RoboticsRegla = t.Reglas(k)
                        If d.Tipo = RoboticsReglaTipo.jornadas_seguidas_de_trabajo Then
                            Dim dias_seguidos_de_trabajo As Integer = d.Valor
                            Dim dias_seguidos_con_descanso As Integer = dias_seguidos_de_trabajo + 1
                            LogTitulo(m_nombre_trabajadores(i) & " dias seguidos:" + dias_seguidos_de_trabajo.ToString(), 1)
                            For j As Integer = 0 To m_i_dias - dias_seguidos_con_descanso
                                Dim d1 As DateTime = m_calendario.FechaInicial.AddDays(j)
                                Dim d2 As DateTime = d1.AddDays(dias_seguidos_con_descanso)
                                If d1 >= d.FechaInicio AndAlso d2 <= d.FechaFinal Then
                                    Dim tro As IntExpr = Nothing
                                    For m As Integer = 1 To dias_seguidos_con_descanso - 1
                                        If tro Is Nothing Then tro = m_s.MakeProd(m_ocupacion_por_trabajador_y_dia(i)(j + m - 1), m_ocupacion_por_trabajador_y_dia(i)(j + m)) Else tro = m_s.MakeProd(tro, m_ocupacion_por_trabajador_y_dia(i)(j + m))
                                    Next

                                    SetLogConstraintPrefix("inicio dia (" & j.ToString() & ") " + d1.ToString("ddd dd") & ":")
                                    m_s.Add(LogConstraint(m_s.MakeEquality(tro, 0), "err=1005;regla=" & RoboticsReglaTipo.jornadas_seguidas_de_trabajo.ToString() & ";trabajador=" + i.ToString() & ";dia=" + j.ToString() & ";nombre=" & t.Nombre))
                                End If
                            Next
                        End If
                    Next
                Next
            Else
                LogTitulo("REGLA DESACTIVADA", 1)
            End If

            LogTitulo("LUNES FESTIVO SI TRABAJA EL DOMINGO", 0)
            For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                For j As Integer = 0 To m_i_dias - 1
                    If m_calendario.FechaInicial.AddDays(j).DayOfWeek = 0 AndAlso j + 1 < m_i_dias Then
                        If m_trabajadores(i).Dias(j).Modo = RoboticsTrabajadorDiaModo.si_domingo_presente_lunes_ausente Then
                            LogTitulo(m_nombre_trabajadores(i), 1)
                            Dim tro As IntExpr = m_s.MakeProd(m_ocupacion_por_trabajador_y_dia(i)(j), m_ocupacion_por_trabajador_y_dia(i)(j + 1))
                            m_s.Add(LogConstraint(m_s.MakeEquality(tro, 0), "err=1006;trabajador=" & i.ToString() & ";dia=" + j.ToString() & ";nombre=" + m_trabajadores(i).Nombre))
                        End If
                    End If
                Next
            Next

            LogTitulo("DESCANSO ENTRE JORNADAS", 0)
            If Not m_desactivar_reglas.Contains(RoboticsReglaTipo.descanso_entre_jornadas) Then
                For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                    Dim t As RoboticsTrabajador = m_trabajadores(i)
                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim d As RoboticsRegla = m_trabajadores(i).Reglas(k)
                        If d.Tipo = RoboticsReglaTipo.descanso_entre_jornadas AndAlso d.Valor <> 24 * 60 Then
                            LogTitulo(m_nombre_trabajadores(i), 1)
                            For j As Integer = 1 To m_i_dias - 1
                                Dim d1 As DateTime = m_calendario.FechaInicial.AddDays(j)
                                If d1 >= d.FechaInicio AndAlso d1 <= d.FechaFinal Then
                                    SetLogConstraintPrefix("dia " & j.ToString() & " " + d1.ToString("ddd dd"))
                                    Dim salida_menos_entrada As IntExpr = m_s.MakeDifference(m_salida_por_trabajador_y_dia(i)(j - 1), m_entrada_por_trabajador_y_dia(i)(j))
                                    Dim turno_de_noche As IntExpr = m_s.MakeSum(salida_menos_entrada, 24 * 60)
                                    Dim resta_48 As IntExpr = m_s.MakeDifference(48 * 60, turno_de_noche)
                                    m_s.Add(LogConstraint(m_s.MakeGreaterOrEqual(resta_48, d.Valor), "err=1007;regla=" & RoboticsReglaTipo.descanso_entre_jornadas.ToString() & ";trabajador=" + i.ToString() & ";dia=" + j.ToString() & ";nombre=" & t.Nombre))
                                End If
                            Next
                        End If
                    Next
                Next
            Else
                LogTitulo("REGLA DESACTIVADA", 1)
            End If

            LogTitulo("MÁXIMO TIEMPO DE TRABAJO", 0)
            If Not m_desactivar_reglas.Contains(RoboticsReglaTipo.maximo_tiempo_de_trabajo) Then
                For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                    Dim t As RoboticsTrabajador = m_trabajadores(i)
                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim d As RoboticsRegla = m_trabajadores(i).Reglas(k)
                        If d.Tipo = RoboticsReglaTipo.maximo_tiempo_de_trabajo AndAlso d.Valor <> 0 Then
                            LogTitulo(m_nombre_trabajadores(i), 1)
                            Dim dias As Integer = 0
                            For j As Integer = 0 To m_i_dias - 1
                                Dim d1 As DateTime = m_calendario.FechaInicial.AddDays(j)
                                If d1 >= d.FechaInicio AndAlso d1 <= d.FechaFinal Then dias += 1
                            Next

                            Dim v As IntVar() = New IntVar(dias + 1 - 1) {}
                            v(0) = m_s.MakeIntVar(d.Valor2, d.Valor2, "acumulado")
                            For j As Integer = 0 To m_i_dias - 1
                                Dim d1 As DateTime = m_calendario.FechaInicial.AddDays(j)
                                If d1 >= d.FechaInicio AndAlso d1 <= d.FechaFinal Then v(j + 1) = m_minutos_de_trabajo_por_trabajador_y_dia(i)(j)
                            Next

                            Dim exp As IntExpr = m_s.MakeSum(v)
                            m_s.Add(LogConstraint(m_s.MakeLessOrEqual(exp, d.Valor), "err=1008;regla=" & RoboticsReglaTipo.maximo_tiempo_de_trabajo.ToString() & ";trabajador=" + i.ToString() & ";nombre=" & t.Nombre))
                        End If
                    Next
                Next
            Else
                LogTitulo("REGLA DESACTIVADA", 1)
            End If

            LogTitulo("JORNADAS DE DESCANSO SEGUIDAS O NO", 0)
            If Not m_desactivar_reglas.Contains(RoboticsReglaTipo.jornadas_de_descanso) Then
                For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                    Dim t As RoboticsTrabajador = m_trabajadores(i)
                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim d As RoboticsRegla = m_trabajadores(i).Reglas(k)
                        If d.Tipo = RoboticsReglaTipo.jornadas_de_descanso AndAlso d.Valor <> 0 Then
                            LogTitulo(m_nombre_trabajadores(i), 1)
                            Dim dias As Integer = 0
                            Dim descanso_anteriores As Integer = 0
                            Dim descanso_posteriores As Integer = 0
                            For j As Integer = 0 To t.DiasAnteriores.Count - 1
                                Dim d1 As DateTime = fecha_inicio_anterior.AddDays(j)
                                If d1 >= d.FechaInicio AndAlso d1 <= d.FechaFinal AndAlso t.DiasAnteriores(j).Modo = RoboticsTrabajadorDiaModo.ausente Then
                                    descanso_anteriores += 1
                                End If
                            Next

                            For j As Integer = 0 To t.DiasPosteriores.Count - 1
                                Dim d1 As DateTime = fecha_inicio_posterior.AddDays(j)
                                If d1 >= d.FechaInicio AndAlso d1 <= d.FechaFinal AndAlso t.DiasPosteriores(j).Modo = RoboticsTrabajadorDiaModo.ausente Then
                                    descanso_posteriores += 1
                                End If
                            Next

                            For j As Integer = 0 To m_i_dias - 1
                                Dim d1 As DateTime = fecha_inicio.AddDays(j)
                                If d1 >= d.FechaInicio AndAlso d1 <= d.FechaFinal Then dias += 1
                            Next

                            Dim v As IntVar() = New IntVar(dias - 1) {}
                            Dim index As Integer = 0
                            For j As Integer = 0 To m_i_dias - 1
                                Dim d1 As DateTime = fecha_inicio.AddDays(j)
                                If d1 >= d.FechaInicio AndAlso d1 <= d.FechaFinal Then
                                    v(index) = m_ocupacion_por_trabajador_y_dia(i)(j)
                                    index += 1
                                End If
                            Next

                            Dim exp As IntExpr = m_s.MakeSum(v)
                            exp = m_s.MakeDifference(m_s.MakeIntConst(dias), exp)
                            exp = m_s.MakeSum(exp, m_s.MakeIntConst(descanso_anteriores))
                            exp = m_s.MakeSum(exp, m_s.MakeIntConst(descanso_posteriores))
                            m_s.Add(LogConstraint(m_s.MakeGreaterOrEqual(exp, d.Valor), "err=1009;regla=" & RoboticsReglaTipo.jornadas_de_descanso.ToString() & ";trabajador=" + i.ToString() & ";nombre=" & t.Nombre))
                        End If
                    Next
                Next
            Else
                LogTitulo("REGLA DESACTIVADA", 1)
            End If

            LogTitulo("JORNADAS DE DESCANSO SEGUIDAS", 0)
            If Not m_desactivar_reglas.Contains(RoboticsReglaTipo.jornadas_seguidas_de_descanso_NO_USAR) Then
                For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                    Dim t As RoboticsTrabajador = m_trabajadores(i)
                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim d As RoboticsRegla = m_trabajadores(i).Reglas(k)
                        If d.Tipo = RoboticsReglaTipo.jornadas_seguidas_de_descanso_NO_USAR AndAlso d.Valor <> 0 AndAlso d.Valor <= m_i_dias Then
                            Dim tro As IntExpr = Nothing
                            LogTitulo(m_nombre_trabajadores(i) & " dias seguidos:" + d.Valor.ToString(), 1)
                            For j As Integer = 0 To m_i_dias - d.Valor
                                Dim d1 As DateTime = m_calendario.FechaInicial.AddDays(j)
                                Dim d2 As DateTime = d1.AddDays(d.Valor)
                                If d1 >= d.FechaInicio AndAlso d2 <= d.FechaFinal Then
                                    Dim z As IntVar() = New IntVar(d.Valor - 1) {}
                                    For m As Integer = 0 To d.Valor - 1
                                        z(m) = m_ocupacion_por_trabajador_y_dia(i)(j + m)
                                    Next

                                    Dim tro2 As IntExpr = m_s.MakeSum(z)
                                    tro2 = m_s.MakeEquality(tro2, 0)
                                    If tro Is Nothing Then tro = tro2 Else tro = m_s.MakeMax(tro2, tro)
                                End If
                            Next

                            If tro IsNot Nothing Then m_s.Add(LogConstraint(m_s.MakeEquality(tro, 1), "err=1010;regla=" & RoboticsReglaTipo.jornadas_seguidas_de_descanso_NO_USAR.ToString() & ";trabajador=" + i.ToString() & ";nombre=" & t.Nombre))
                        End If
                    Next
                Next
            Else
                LogTitulo("REGLA DESACTIVADA", 1)
            End If

            LogTitulo("JORNADAS DE TRABAJO NO SEGUIDAS", 0)
            If Not m_desactivar_reglas.Contains(RoboticsReglaTipo.jornadas_de_trabajo) Then
                For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                    Dim t As RoboticsTrabajador = m_trabajadores(i)
                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim d As RoboticsRegla = m_trabajadores(i).Reglas(k)
                        If d.Tipo = RoboticsReglaTipo.jornadas_de_trabajo AndAlso d.Valor <> 0 AndAlso d.Valor <= m_i_dias Then
                            LogTitulo(m_nombre_trabajadores(i), 1)
                            Dim dias As Integer = 0
                            For j As Integer = 0 To m_i_dias - 1
                                Dim d1 As DateTime = m_calendario.FechaInicial.AddDays(j)
                                If d1 >= d.FechaInicio AndAlso d1 <= d.FechaFinal Then dias += 1
                            Next

                            Dim v As IntVar() = New IntVar(dias - 1) {}
                            Dim index As Integer = 0
                            For j As Integer = 0 To m_i_dias - 1
                                Dim d1 As DateTime = m_calendario.FechaInicial.AddDays(j)
                                If d1 >= d.FechaInicio AndAlso d1 <= d.FechaFinal Then
                                    v(index) = m_ocupacion_por_trabajador_y_dia(i)(j)
                                    index += 1
                                End If
                            Next

                            Dim exp As IntExpr = m_s.MakeSum(v)
                            m_s.Add(LogConstraint(m_s.MakeGreaterOrEqual(exp, d.Valor), "err=1011;regla=" & RoboticsReglaTipo.jornadas_de_trabajo.ToString() & ";trabajador=" + i.ToString() & ";nombre=" & t.Nombre))
                        End If
                    Next
                Next
            Else
                LogTitulo("REGLA DESACTIVADA", 1)
            End If

            LogTitulo("MÁXIMO DE TURNOS DE UN TIPO", 0)
            If Not m_desactivar_reglas.Contains(RoboticsReglaTipo.maximo_jornadas_con_turno) Then
                For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                    Dim t As RoboticsTrabajador = m_trabajadores(i)
                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim d As RoboticsRegla = m_trabajadores(i).Reglas(k)
                        If d.Tipo = RoboticsReglaTipo.maximo_jornadas_con_turno Then
                            LogTitulo(m_nombre_trabajadores(i) & " máximo de turno:" & m_turnos.BuscarTurno(d.Valor).Nombre & " " + d.Valor2.ToString(), 1)
                            Dim dias As Integer = m_trabajadores(i).Dias.ContarElementosEntreFechas(fecha_inicio, d.FechaInicio, d.FechaFinal)
                            Dim turnos_dias_anteriores As Integer() = m_trabajadores(i).DiasAnteriores.TurnosEntreFechas(fecha_inicio_anterior, d.FechaInicio, d.FechaFinal, m_turnos)
                            Dim turnos_dias_posteriores As Integer() = m_trabajadores(i).DiasPosteriores.TurnosEntreFechas(fecha_inicio_posterior, d.FechaInicio, d.FechaFinal, m_turnos)
                            Dim v As IntVar() = New IntVar(dias + turnos_dias_anteriores.Length + turnos_dias_posteriores.Length - 1) {}
                            Dim index As Integer = 0
                            For j As Integer = 0 To turnos_dias_anteriores.Length - 1
                                v(index) = m_s.MakeIntConst(turnos_dias_anteriores(j))
                                index += 1
                            Next

                            For j As Integer = 0 To m_i_dias - 1
                                Dim d1 As DateTime = fecha_inicio.AddDays(j)
                                If d1 >= d.FechaInicio AndAlso d1 <= d.FechaFinal Then
                                    v(index) = m_turno_por_trabajador_y_dia(i)(j)
                                    index += 1
                                End If
                            Next

                            For j As Integer = 0 To turnos_dias_posteriores.Length - 1
                                v(index) = m_s.MakeIntConst(turnos_dias_posteriores(j))
                                index += 1
                            Next

                            m_s.Add(LogConstraint(m_s.MakeAtMost(v, d.Valor, d.Valor2), "err=1012;regla=" & RoboticsReglaTipo.maximo_jornadas_con_turno.ToString() & ";trabajador=" + i.ToString() & ";nombre=" & t.Nombre))
                        End If
                    Next
                Next
            Else
                LogTitulo("REGLA DESACTIVADA", 1)
            End If

            LogTitulo("MÍNIMO DE TURNOS DE UN TIPO", 0)
            If Not m_desactivar_reglas.Contains(RoboticsReglaTipo.minimo_jornadas_con_turno) Then
                For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                    Dim t As RoboticsTrabajador = m_trabajadores(i)
                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim d As RoboticsRegla = m_trabajadores(i).Reglas(k)
                        If d.Tipo = RoboticsReglaTipo.minimo_jornadas_con_turno Then
                            Dim tro As IntExpr = Nothing
                            LogTitulo(m_nombre_trabajadores(i) & " mínimo de turno:" & m_turnos.BuscarTurno(d.Valor).Nombre & " " + d.Valor2.ToString(), 1)
                            Dim turnos_dias_anteriores As Integer() = m_trabajadores(i).DiasAnteriores.TurnosEntreFechas(fecha_inicio_anterior, d.FechaInicio, d.FechaFinal, m_turnos)
                            Dim turnos_dias_posteriores As Integer() = m_trabajadores(i).DiasPosteriores.TurnosEntreFechas(fecha_inicio_posterior, d.FechaInicio, d.FechaFinal, m_turnos)
                            For j As Integer = 0 To turnos_dias_anteriores.Length - 1
                                If turnos_dias_anteriores(j) = d.Valor Then
                                    Dim turno_deseado As IntVar = m_s.MakeIntConst(d.Valor, "anterior")
                                    If tro Is Nothing Then tro = m_s.MakeIntConst(1) Else tro = m_s.MakeSum(m_s.MakeIntConst(1), tro)
                                End If
                            Next

                            For j As Integer = 0 To m_i_dias - 1
                                Dim d1 As DateTime = fecha_inicio.AddDays(j)
                                If d1 >= d.FechaInicio AndAlso d1 <= d.FechaFinal Then
                                    Dim turno_deseado As IntExpr = m_s.MakeEquality(m_turno_por_trabajador_y_dia(i)(j), d.Valor)
                                    If tro Is Nothing Then tro = m_s.MakeSum(turno_deseado, 0) Else tro = m_s.MakeSum(turno_deseado, tro)
                                End If
                            Next

                            For j As Integer = 0 To turnos_dias_posteriores.Length - 1
                                If turnos_dias_posteriores(j) = d.Valor Then
                                    Dim turno_deseado As IntVar = m_s.MakeIntConst(d.Valor, "posterior")
                                    If tro Is Nothing Then tro = m_s.MakeIntConst(1) Else tro = m_s.MakeSum(m_s.MakeIntConst(1), tro)
                                End If
                            Next

                            m_s.Add(LogConstraint(m_s.MakeGreaterOrEqual(tro, d.Valor2), "err=1013;regla=" & RoboticsReglaTipo.minimo_jornadas_con_turno.ToString() & ";trabajador=" + i.ToString() & ";nombre=" & t.Nombre))
                        End If
                    Next
                Next
            Else
                LogTitulo("REGLA DESACTIVADA", 1)
            End If

            LogTitulo("MÍNIMO DE FINES DE SEMANA", 0)
            If Not m_desactivar_reglas.Contains(RoboticsReglaTipo.minimo_de_fines_de_semana) Then
                For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                    Dim inicio_fin_de_semana As Integer = 0
                    Dim duracion_fin_de_semana As Integer = 0
                    Dim t As RoboticsTrabajador = m_trabajadores(i)
                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim r As RoboticsRegla = t.Reglas(k)
                        If r.Tipo = RoboticsReglaTipo.establecer_fin_de_semana Then
                            inicio_fin_de_semana = r.Valor
                            duracion_fin_de_semana = r.Valor2
                            Exit For
                        End If
                    Next

                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim r As RoboticsRegla = t.Reglas(k)
                        If r.Tipo = RoboticsReglaTipo.minimo_de_fines_de_semana Then
                            LogTitulo(m_nombre_trabajadores(i), 1)
                            Dim dias As Integer = 0
                            For j As Integer = 0 To m_i_dias - 1
                                Dim d As DateTime = m_calendario.FechaInicial.AddDays(j)
                                If d >= r.FechaInicio AndAlso d <= r.FechaFinal Then dias += 1
                            Next

                            Dim v As IntVar() = New IntVar(dias + 2 - 1) {}
                            v(0) = m_s.MakeIntConst(t.DiasAnteriores.FinDeSemanaEntreFechas(fecha_inicio_anterior, r.FechaInicio, r.FechaFinal, inicio_fin_de_semana, duracion_fin_de_semana), "anterior")
                            v(v.Length - 1) = m_s.MakeIntConst(t.DiasPosteriores.FinDeSemanaEntreFechas(fecha_inicio_posterior, r.FechaInicio, r.FechaFinal, inicio_fin_de_semana, duracion_fin_de_semana), "posterior")
                            Dim index As Integer = 1
                            For j As Integer = 0 To m_i_dias - 1
                                Dim d As DateTime = m_calendario.FechaInicial.AddDays(j)
                                If d >= r.FechaInicio AndAlso d <= r.FechaFinal Then
                                    v(index) = m_fin_de_semana_completo_por_trabajador_y_dia(i)(j)
                                    index += 1
                                End If
                            Next

                            Dim suma As IntExpr = m_s.MakeSum(v)
                            m_s.Add(LogConstraint(m_s.MakeGreaterOrEqual(suma, r.Valor), "err=1014;regla=" & RoboticsReglaTipo.minimo_de_fines_de_semana.ToString() & ";trabajador=" + i.ToString() & ";nombre=" & t.Nombre))
                        End If
                    Next
                Next
            Else
                LogTitulo("REGLA DESACTIVADA", 1)
            End If

            LogTitulo("MÁXIMO DE FINES DE SEMANA", 0)
            If Not m_desactivar_reglas.Contains(RoboticsReglaTipo.maximo_de_fines_de_semana) Then
                For i As Integer = 0 To m_i_trabajadores - m_i_ocupados - 1
                    Dim inicio_fin_de_semana As Integer = 0
                    Dim duracion_fin_de_semana As Integer = 0
                    Dim t As RoboticsTrabajador = m_trabajadores(i)
                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim r As RoboticsRegla = t.Reglas(k)
                        If r.Tipo = RoboticsReglaTipo.establecer_fin_de_semana Then
                            inicio_fin_de_semana = r.Valor
                            duracion_fin_de_semana = r.Valor2
                            Exit For
                        End If
                    Next

                    For k As Integer = 0 To t.Reglas.Count - 1
                        Dim r As RoboticsRegla = t.Reglas(k)
                        If r.Tipo = RoboticsReglaTipo.maximo_de_fines_de_semana Then
                            LogTitulo(m_nombre_trabajadores(i), 1)
                            Dim dias As Integer = 0
                            For j As Integer = 0 To m_i_dias - 1
                                Dim d As DateTime = m_calendario.FechaInicial.AddDays(j)
                                If d >= r.FechaInicio AndAlso d <= r.FechaFinal Then dias += 1
                            Next

                            Dim v As IntVar() = New IntVar(dias + 2 - 1) {}
                            v(0) = m_s.MakeIntConst(t.DiasAnteriores.FinDeSemanaEntreFechas(fecha_inicio_anterior, r.FechaInicio, r.FechaFinal, inicio_fin_de_semana, duracion_fin_de_semana), "anterior")
                            v(v.Length - 1) = m_s.MakeIntConst(t.DiasPosteriores.FinDeSemanaEntreFechas(fecha_inicio_posterior, r.FechaInicio, r.FechaFinal, inicio_fin_de_semana, duracion_fin_de_semana), "posterior")
                            Dim index As Integer = 1
                            For j As Integer = 0 To m_i_dias - 1
                                Dim d As DateTime = m_calendario.FechaInicial.AddDays(j)
                                If d >= r.FechaInicio AndAlso d <= r.FechaFinal Then
                                    v(index) = m_fin_de_semana_completo_por_trabajador_y_dia(i)(j)
                                    index += 1
                                End If
                            Next

                            Dim suma As IntExpr = m_s.MakeSum(v)
                            m_s.Add(LogConstraint(m_s.MakeLessOrEqual(suma, r.Valor), "err=1015;regla=" & RoboticsReglaTipo.maximo_de_fines_de_semana.ToString() & ";trabajador=" + i.ToString() & ";nombre=" & t.Nombre))
                        End If
                    Next
                Next
            Else
                LogTitulo("REGLA DESACTIVADA", 1)
            End If
        End Sub

        Public Sub Solucionar()
            m_db = m_s.MakePhase(m_flat, Solver.CHOOSE_MIN_SIZE, Solver.ASSIGN_MIN_VALUE)
            m_objetivo_coste = m_s.MakeMinimize(m_coste, m_diferencia_coste_entre_soluciones)
            m_objetivo_num_trabajadores = m_s.MakeMinimize(m_trabajadores_trabajando, m_diferencia_coste_entre_soluciones)
            Dim par As SearchLimitParameters = m_s.MakeDefaultSearchLimitParameters()
            par.Time = 60 * 1000
            m_lim = m_s.MakeLimit(par)
            m_init_search_time = Environment.TickCount
            m_s.NewSearch(m_db, m_objetivo_coste, m_lim)
        End Sub

        Public Function SiguienteSolucion() As Boolean
            Return SiguienteSolucion(5)
        End Function

        Public Function SiguienteSolucion(ByVal segundos As Integer) As Boolean
            If m_no_hay_solucion Then
                m_s.EndSearch()
                Return False
            End If

            If segundos < 5 Then segundos = 5
            Dim par As SearchLimitParameters = m_s.MakeDefaultSearchLimitParameters()
            Dim time_delta As Long = Environment.TickCount - m_init_search_time + segundos * 1000
            m_s.UpdateLimits(time_delta, par.Branches, par.Failures, par.Solutions, m_lim)
            Dim res As Boolean = m_s.NextSolution()
            If Not res Then m_s.EndSearch() Else AlmacenarSolucion()
            Return res
        End Function

        Private Sub AlmacenarSolucion()
            m_ultima_solucion.failures = m_s.Failures()
            m_ultima_solucion.branches = m_s.Branches()
            m_ultima_solucion.solucion_coste = m_coste.Value()
            m_ultima_solucion.solucion_trabajadores = m_trabajadores_trabajando.Value()
            For i As Integer = 0 To m_i_dias - 1
                For j As Integer = 0 To m_i_ocupados - 1
                    m_ultima_solucion.solucion_trabajador_por_dia_y_puesto(i)(j) = m_trabajador_por_dia_y_puesto(i)(j).Value()
                Next
            Next

            For i As Integer = 0 To m_i_trabajadores - 1
                For j As Integer = 0 To m_i_dias - 1
                    m_ultima_solucion.solucion_ocupacion_por_trabajador_y_dia(i)(j) = m_ocupacion_por_trabajador_y_dia(i)(j).Value()
                Next
            Next
        End Sub

        Public Function MejorSolucion(ByVal segundos As Integer) As Integer
            Dim par As SearchLimitParameters = m_s.MakeDefaultSearchLimitParameters()
            Dim time_inicial As Long = Environment.TickCount
            Dim c As Integer = 0
            While True
                Dim time_restante As Long = segundos * 1000 - (Environment.TickCount - time_inicial)
                Dim time_delta As Long = Environment.TickCount - m_init_search_time + time_restante
                m_s.UpdateLimits(time_delta, par.Branches, par.Failures, par.Solutions, m_lim)
                Dim res As Boolean = m_s.NextSolution()
                If Not res Then
                    m_s.EndSearch()
                    Exit While
                Else
                    AlmacenarSolucion()
                    c += 1
                End If
            End While

            Return c
        End Function

        Public Sub Close()
            If m_s Is Nothing Then Return
            m_s.Dispose()
            m_s = Nothing
            m_trabajador_por_dia_y_puesto = Nothing
            m_puesto_por_dia_y_trabajador = Nothing
            m_puesto_por_trabajador_y_dia = Nothing
            m_ocupacion_por_trabajador_y_dia = Nothing
            m_coste_por_trabajador_y_dia = Nothing
            m_coste_total_por_trabajador_y_dia = Nothing
            m_flat = Nothing
            m_flat_coste_total = Nothing
            m_nombre_trabajadores = Nothing
        End Sub

        Public ReadOnly Property Ocupados As Integer
            Get
                Return m_i_ocupados
            End Get
        End Property

        Public ReadOnly Property Libres As Integer
            Get
                Return m_i_libres
            End Get
        End Property

        Public ReadOnly Property NumTrabajadores As Integer
            Get
                Return m_i_trabajadores
            End Get
        End Property

        Public ReadOnly Property Dias As Integer
            Get
                Return m_i_dias
            End Get
        End Property

        Public ReadOnly Property SolucionCoste As Long
            Get
                Dim falsos As Integer = 0
                Dim coste As Integer = 0
                For i As Integer = 0 To m_i_dias - 1
                    For j As Integer = 0 To m_i_ocupados - 1
                        Dim v As Long = m_ultima_solucion.solucion_trabajador_por_dia_y_puesto(i)(j)
                        If v < m_trabajadores.Count Then
                            Dim tr As RoboticsTrabajador = m_trabajadores(CInt(v))
                            If Not tr.TrabajadorReal Then
                                falsos += 1
                                coste += tr.CosteMedioTrabajadorNoReal
                            End If
                        End If
                    Next
                Next

                Dim m As Integer = 1000
                Return m_ultima_solucion.solucion_coste - m * falsos + coste
            End Get
        End Property

        Public ReadOnly Property SolucionTrabajadores As Long
            Get
                Return m_ultima_solucion.solucion_trabajadores
            End Get
        End Property

        Public Function TrabajadorDiaPuesto(ByVal dia As Integer, ByVal puesto As Integer) As RoboticsTrabajadorAsignado
            Dim t As RoboticsTrabajadorAsignado = New RoboticsTrabajadorAsignado()
            Dim tid As Integer = CInt(m_ultima_solucion.solucion_trabajador_por_dia_y_puesto(dia)(puesto))
            If tid < m_trabajadores.Count Then
                t.Necesario = True
                t.Nombre = m_trabajadores(tid).Nombre
                t.Id = m_trabajadores(tid).Id
                t.Modo = m_trabajadores(tid).Dias(dia).Modo
                t.Puesto = m_todos_los_puestos(puesto).Puesto
                t.Horario = m_todos_los_puestos(puesto).Turno
                t.UP = m_todos_los_puestos(puesto).UnidadProductiva
            Else
                t.Necesario = False
                t.Nombre = m_nombre_trabajadores(tid)
                t.Id = tid.ToString()
                t.Modo = RoboticsTrabajadorDiaModo.disponible
                t.Puesto = m_nombre_puestos(puesto)
                t.Horario = ""
                t.UP = ""
            End If

            Return t
        End Function

        Public Function TrabajadoresLibres(ByVal dia As Integer) As String()
            Dim res As String() = New String(m_i_libres - 1) {}
            Dim index As Integer = 0
            For i As Integer = 0 To m_i_trabajadores - 1
                If m_ultima_solucion.solucion_ocupacion_por_trabajador_y_dia(i)(dia) = 0 Then
                    res(index) = m_nombre_trabajadores(i) & "-" + i.ToString()
                    index += 1
                End If
            Next

            Return res
        End Function

        Private Sub PrepararDatos()
            m_trabajadores.Sort(New RoboticsTrabajadorListComparer(m_modo_solucion = 0))
            For i As Integer = 0 To m_trabajadores.Count - 1
                Dim t As RoboticsTrabajador = m_trabajadores(i)
                For j As Integer = 0 To t.Dias.Count - 1
                    Dim td As RoboticsTrabajadorDia = t.Dias(j)
                    If td.Modo = RoboticsTrabajadorDiaModo.presente AndAlso j < m_calendario.Count Then
                        Dim ct As Integer = m_trabajadores.ContarPresentes(j, td.Puestos(0).Puesto, td.Turnos(0).Turno)
                        Dim cc As Integer = 0
                        If Not m_presentes_siempre_crea_puestos Then cc = m_calendario(j).Puestos.SumarPuestosSinUnidadProductiva(td.Puestos(0).Puesto, td.Turnos(0).Turno) Else cc = m_calendario(j).Puestos.SumarPuestos("-1", td.Puestos(0).Puesto, td.Turnos(0).Turno)
                        If ct > cc Then
                            m_calendario(j).Puestos.Add(New RoboticsCalendarioPuesto("-1", td.Puestos(0).Puesto, ct - cc, td.Turnos(0).Turno))
                        End If
                    End If
                Next
            Next
        End Sub

        Private Sub PrepararPuestos()
            For i As Integer = 0 To m_todos_los_puestos.Count - 1
                Dim p As RoboticsCalendarioPuesto = m_todos_los_puestos(i)
                Dim ts As Integer() = m_trabajadores.TrabajadoresHabilitados(0, p.Puesto, p.Turno)
                p.TrabajadoresDisponibles = ts.Length
            Next

            m_todos_los_puestos.Sort(New RoboticsCalendarioPuestosListComparer(False))
        End Sub

        Public Sub CargarObjetos()
            m_log.Clear()
            m_errores.Clear()
            m_todos_los_puestos.Clear()
            PrepararDatos()
            If m_errores.Count > 0 Then Return
            Dim dias As Integer = m_calendario.Count
            Dim trabajadores As Integer = m_trabajadores.Count
            For i As Integer = 0 To m_calendario.Count - 1
                Dim pdia As RoboticsCalendarioPuestosList = m_calendario(i).Puestos
                For j As Integer = 0 To m_todos_los_puestos.Count - 1
                    m_todos_los_puestos(j).Procesado = False
                Next

                For j As Integer = 0 To pdia.Count - 1
                    Dim c As Integer = 0
                    For m As Integer = 0 To m_todos_los_puestos.Count - 1
                        If m_todos_los_puestos(m).UnidadProductiva = pdia(j).UnidadProductiva AndAlso m_todos_los_puestos(m).Puesto = pdia(j).Puesto AndAlso m_todos_los_puestos(m).Turno = pdia(j).Turno AndAlso Not m_todos_los_puestos(m).Procesado Then
                            c += 1
                        End If
                    Next

                    For m As Integer = c To pdia(j).Cantidad - 1
                        Dim p As RoboticsCalendarioPuesto = New RoboticsCalendarioPuesto(pdia(j).UnidadProductiva, pdia(j).Puesto, 0, pdia(j).Turno)
                        p.Procesado = True
                        m_todos_los_puestos.Add(p)
                    Next
                Next
            Next

            PrepararPuestos()
            Dim puestos As Integer = m_todos_los_puestos.Count
            trabajadores += puestos
            CrearMatrices(dias, puestos, trabajadores)
            LogTitulo("LISTA DE TRABAJADORES", 0)
            For i As Integer = 0 To m_i_trabajadores - 1
                If i < m_trabajadores.Count Then m_nombre_trabajadores(i) = m_trabajadores(i).Nombre Else m_nombre_trabajadores(i) = "DUMMY " & i.ToString()
                LogTitulo(m_nombre_trabajadores(i), 1)
            Next

            For j As Integer = 0 To m_calendario.Count - 1
                For i As Integer = 0 To m_i_trabajadores - 1
                    If i < m_trabajadores.Count Then m_coste_por_trabajador_y_dia(i)(j) = m_trabajadores(i).Dias(j).Coste Else m_coste_por_trabajador_y_dia(i)(j) = 0
                Next
            Next

            For i As Integer = 0 To m_i_dias - 1
                For j As Integer = 0 To m_i_trabajadores - 1
                    For m As Integer = 0 To m_i_puestos - 1
                        m_disponibilidad_dia_y_trabajador_y_puesto(i)(j)(m) = 0
                    Next
                Next
            Next

            For i As Integer = 0 To m_i_dias - 1
                For j As Integer = 0 To m_i_trabajadores - 1
                    If j < m_trabajadores.Count Then
                        Dim t As RoboticsTrabajador = m_trabajadores(j)
                        For m As Integer = 0 To m_i_puestos - 1
                            If m < m_todos_los_puestos.Count AndAlso t.Dias(i).Modo <> RoboticsTrabajadorDiaModo.ausente Then
                                If m_presentes_siempre_crea_puestos AndAlso ((m_todos_los_puestos(m).UnidadProductiva <> "-1" AndAlso t.Dias(i).Modo = RoboticsTrabajadorDiaModo.presente) OrElse (m_todos_los_puestos(m).UnidadProductiva = "-1" AndAlso t.Dias(i).Modo <> RoboticsTrabajadorDiaModo.presente)) Then
                                Else
                                    Dim d As RoboticsTrabajadorDia = t.Dias(i)
                                    If d.Puestos.Count = 0 Then
                                        If d.Turnos.BuscarTurno(m_todos_los_puestos(m).Turno) IsNot Nothing AndAlso t.Puestos.BuscarPuesto(m_todos_los_puestos(m).Puesto) IsNot Nothing Then
                                            m_disponibilidad_dia_y_trabajador_y_puesto(i)(j)(m) = 1
                                        End If
                                    Else
                                        Dim cantidad As Integer = 0
                                        If m_presentes_siempre_crea_puestos AndAlso m_todos_los_puestos(m).UnidadProductiva = "-1" AndAlso t.Dias(i).Modo = RoboticsTrabajadorDiaModo.presente Then
                                            For k As Integer = 0 To m_todos_los_puestos.Count - 1
                                                If m_disponibilidad_dia_y_trabajador_y_puesto(i)(j)(k) = 1 Then cantidad += 1
                                            Next

                                            For k As Integer = 0 To m_i_trabajadores - 1
                                                If m_disponibilidad_dia_y_trabajador_y_puesto(i)(k)(m) = 1 Then cantidad += 1
                                            Next
                                        End If

                                        If cantidad = 0 Then
                                            If d.Turnos.BuscarTurno(m_todos_los_puestos(m).Turno) IsNot Nothing AndAlso d.Puestos.BuscarPuesto(m_todos_los_puestos(m).Puesto) IsNot Nothing Then
                                                m_disponibilidad_dia_y_trabajador_y_puesto(i)(j)(m) = 1
                                            End If
                                        End If
                                    End If
                                End If
                            Else
                                If m >= m_i_ocupados Then m_disponibilidad_dia_y_trabajador_y_puesto(i)(j)(m) = 1
                            End If
                        Next
                    Else
                        For m As Integer = m_i_ocupados To m_i_puestos - 1
                            m_disponibilidad_dia_y_trabajador_y_puesto(i)(j)(m) = 1
                        Next
                    End If
                Next
            Next

            For i As Integer = 0 To m_todos_los_puestos.Count - 1
                m_nombre_puestos(i) = m_todos_los_puestos(i).Puesto & " " + i.ToString() & " (" + m_todos_los_puestos(i).Turno & "-" + m_todos_los_puestos(i).UnidadProductiva & ")"
                Dim t As RoboticsTurno = m_turnos.BuscarTurno(m_todos_los_puestos(i).Turno)
                If t IsNot Nothing Then
                    m_entrada_salida_total_por_puesto(0)(i) = m_s.MakeIntConst(t.HoraEntrada, "entrada")
                    m_entrada_salida_total_por_puesto(1)(i) = m_s.MakeIntConst(t.HoraSalida, "salida")
                    m_entrada_salida_total_por_puesto(2)(i) = m_s.MakeIntConst(t.Horas, "horas")
                Else
                    Errores.Add("el puesto " & m_nombre_puestos(i) & " no está correctamente informado")
                    Return
                End If
            Next

            For i As Integer = m_i_ocupados To m_nombre_puestos.Length - 1
                m_nombre_puestos(i) = "libre " & (i - m_i_ocupados).ToString()
                m_entrada_salida_total_por_puesto(0)(i) = m_s.MakeIntConst(48 * 60, "entrada")
                m_entrada_salida_total_por_puesto(1)(i) = m_s.MakeIntConst(0, "salida")
                m_entrada_salida_total_por_puesto(2)(i) = m_s.MakeIntConst(0, "horas")
            Next

            LogTitulo("LISTA DE PUESTOS", 0)
            For i As Integer = 0 To m_nombre_puestos.Length - 1
                LogTitulo(m_nombre_puestos(i), 1)
            Next

            LogTitulo("LISTA DE TURNOS", 0)
            For i As Integer = 0 To m_turnos.Count - 1
                LogTitulo(m_turnos(i).Nombre & " entrada:" + m_turnos(i).HoraEntrada & "(" + TimeSpan.FromMinutes(m_turnos(i).HoraEntrada).ToString() & ")" & " salida:" + m_turnos(i).HoraSalida & "(" + TimeSpan.FromMinutes(m_turnos(i).HoraSalida).ToString() & ")" & " horas:" + m_turnos(i).Horas & "(" + TimeSpan.FromMinutes(m_turnos(i).Horas).ToString() & ")", 1)
            Next

            For i As Integer = 0 To m_i_puestos - 1
                Dim v As Long = 0
                If i < m_todos_los_puestos.Count Then
                    Dim t As RoboticsTurno = m_turnos.BuscarTurno(m_todos_los_puestos(i).Turno)
                    If t IsNot Nothing Then v = t.Id
                End If

                m_horario_por_puesto(i) = m_s.MakeIntConst(v, "turno")
            Next

            For i As Integer = 0 To m_i_dias - 1
                For j As Integer = 0 To m_i_puestos - 1
                    m_necesidad_por_dia_y_puesto(i)(j) = 0
                Next
            Next

            For i As Integer = 0 To m_calendario.Count - 1
                Dim pdia As RoboticsCalendarioPuestosList = m_calendario(i).Puestos
                For j As Integer = 0 To pdia.Count - 1
                    Dim index As Integer() = m_todos_los_puestos.IndexPuestos(pdia(j).UnidadProductiva, pdia(j).Puesto, pdia(j).Turno)
                    If index IsNot Nothing Then
                        For m As Integer = 0 To pdia(j).Cantidad - 1
                            m_necesidad_por_dia_y_puesto(i)(index(m)) = 1
                        Next
                    End If
                Next

                For j As Integer = m_i_ocupados To m_i_puestos - 1
                    m_necesidad_por_dia_y_puesto(i)(j) = 1
                Next
            Next

            SincronizarMatrices()
            CrearCondiciones()
        End Sub

        Public Function ObtenerEstadisticas() As String
            Return "Failures: " & m_ultima_solucion.failures.ToString() & " Branches: " + m_ultima_solucion.branches.ToString()
        End Function

        Private Sub LogLinea(ByVal texto As String)
            LogConsola(texto)
        End Sub

        Private Sub SetLogConstraintPrefix(ByVal texto As String)
            m_log_constraint_prefix = texto & " "
        End Sub

        Private Function LogConstraint(ByVal c As Constraint, ByVal info As String) As Constraint
            LogLinea(m_log_constraint_prefix & c.ToString())
            m_log_constraint_prefix = ""
            If c.ToString().ToLower().Contains("falseconstraint") Then m_errores.Add("condición imposible:" & info)
            Return c
        End Function

        Private Sub LogTitulo(ByVal texto As String, ByVal nivel As Integer)
            Dim delta As String = ""
            If nivel = 0 Then
                LogConsola("")
            ElseIf nivel = 1 Then
                delta = "--> "
            ElseIf nivel = 2 Then
                delta = "----> "
            End If

            LogConsola(delta & texto)
            If nivel = 0 Then
                Dim t As String = ""
                For i As Integer = 0 To texto.Length - 1
                    t += "-"c
                Next

                LogConsola(t)
            End If
        End Sub

        Private Sub LogConsola(ByVal texto As String)
            m_log.AppendLine(texto)
        End Sub

        Public ReadOnly Property Log As String
            Get
                Return m_log.ToString()
            End Get
        End Property

        Public Function CrearTrabajadorQueFalta(ByVal id As String, ByVal coste_medio As Integer) As RoboticsTrabajador
            Dim tr As RoboticsTrabajador = New RoboticsTrabajador("FALTA " & m_trabajadores.Count.ToString())
            tr.TrabajadorReal = False
            tr.CosteMedioTrabajadorNoReal = coste_medio
            Dim turnos As RoboticsTrabajadorTurnosList = New RoboticsTrabajadorTurnosList()
            For i As Integer = 0 To m_calendario.Count - 1
                Dim d As RoboticsCalendarioDia = m_calendario(i)
                For j As Integer = 0 To d.Puestos.Count - 1
                    Dim p As RoboticsCalendarioPuesto = d.Puestos(j)
                    If tr.Puestos.BuscarPuesto(p.Puesto) Is Nothing Then tr.Puestos.Add(New RoboticsTrabajadorPuesto(p.Puesto))
                    If turnos.BuscarTurno(p.Turno) Is Nothing Then turnos.Add(New RoboticsTrabajadorTurno(p.Turno))
                Next
            Next

            Dim tu As String() = New String(turnos.Count - 1) {}
            For i As Integer = 0 To tu.Length - 1
                tu(i) = turnos(i).Turno
            Next

            Dim coste As Integer = 1000
            For i As Integer = 0 To m_calendario.Count - 1
                tr.Dias.Add(New RoboticsTrabajadorDia(coste, RoboticsTrabajadorDiaModo.disponible, tu))
            Next

            m_trabajadores.Add(tr)
            Return tr
        End Function

        Public Sub ExportarSolucion(ByVal fichero As String)
            Dim s As RoboticsSerializar = New RoboticsSerializar()
            s.Calendario = m_calendario
            s.Trabajadores = m_trabajadores
            s.Turnos = m_turnos
            s.FechaInicioCalendario = m_calendario.FechaInicial
            Dim xml As System.Xml.Serialization.XmlSerializer = New System.Xml.Serialization.XmlSerializer(GetType(RoboticsSerializar))
            Dim sw As System.IO.StreamWriter = System.IO.File.CreateText(fichero)
            xml.Serialize(sw, s)
            sw.Close()
        End Sub

        Public Sub ImportarSolucion(ByVal fichero As String)
            Dim xml As System.Xml.Serialization.XmlSerializer = New System.Xml.Serialization.XmlSerializer(GetType(RoboticsSerializar))
            Dim sw As System.IO.StreamReader = System.IO.File.OpenText(fichero)
            Dim s As RoboticsSerializar = CType(xml.Deserialize(sw), RoboticsSerializar)
            sw.Close()
            m_trabajadores = s.Trabajadores
            m_turnos = s.Turnos
            m_calendario = s.Calendario
            m_calendario.FechaInicial = s.FechaInicioCalendario
        End Sub

        Public ReadOnly Property NumeroDeSoluciones As Integer
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property Errores As List(Of String)
            Get
                Return m_errores
            End Get
        End Property

        Public Function CheckHaySolucion() As Boolean
            Init(1)
            If m_no_hay_solucion Then
                m_s.EndSearch()
                Return False
            End If

            Dim par As SearchLimitParameters = m_s.MakeDefaultSearchLimitParameters()
            Dim time_delta As Long = Environment.TickCount - m_init_search_time + 5 * 1000
            m_s.UpdateLimits(time_delta, par.Branches, par.Failures, par.Solutions, m_lim)
            Dim res As Boolean = m_s.NextSolution()
            m_s.EndSearch()
            Return res
        End Function

        Public ReadOnly Property DesactivarReglas As List(Of RoboticsReglaTipo)
            Get
                Return m_desactivar_reglas
            End Get
        End Property

        Public Property PresentesSiempreCreaPuestos As Boolean
            Get
                Return m_presentes_siempre_crea_puestos
            End Get

            Set(ByVal value As Boolean)
                m_presentes_siempre_crea_puestos = value
            End Set
        End Property

        Public ReadOnly Property MaximoDeFaltasEstimado As Integer
            Get
                Return m_todos_los_puestos.Count
            End Get
        End Property
    End Class

    Public Class RoboticsTrabajadorAsignado

        Public Nombre As String

        Public Id As String

        Public Modo As RoboticsTrabajadorDiaModo

        Public UP As String

        Public Puesto As String

        Public Horario As String

        Public Necesario As Boolean
    End Class

    <Serializable>
    Public Class RoboticsSerializar

        Public FechaInicioCalendario As DateTime

        Public Turnos As RoboticsTurnosList

        Public Calendario As RoboticsCalendarioDiasList

        Public Trabajadores As RoboticsTrabajadorList
    End Class

    Public Class RoboticsTrabajadorListComparer
        Implements IComparer(Of RoboticsTrabajador)

        Private m_ascendente As Boolean

        Public Sub New(ByVal ascendente As Boolean)
            m_ascendente = ascendente
        End Sub

        Private Function CosteMedioTrabajador(ByVal trabajador As RoboticsTrabajador) As Integer
            If Not trabajador.TrabajadorReal Then Return Integer.MaxValue
            Dim res As Integer = 0
            For i As Integer = 0 To trabajador.Dias.Count - 1
                res += trabajador.Dias(i).Coste
            Next

            Return res / trabajador.Dias.Count
        End Function

        Private Function ContarModosTrabajador(ByVal trabajador As RoboticsTrabajador, ByVal modo As RoboticsTrabajadorDiaModo) As Integer
            Dim res As Integer = 0
            For i As Integer = 0 To trabajador.Dias.Count - 1
                If trabajador.Dias(i).Modo = modo Then res += 1
            Next

            Return res
        End Function

        Private Function IComparer_Compare(x As RoboticsTrabajador, y As RoboticsTrabajador) As Integer Implements IComparer(Of RoboticsTrabajador).Compare
            Dim coste_x As Integer = CosteMedioTrabajador(x)
            Dim coste_y As Integer = CosteMedioTrabajador(y)
            Dim presentes_x As Integer = ContarModosTrabajador(x, RoboticsTrabajadorDiaModo.presente)
            Dim presentes_y As Integer = ContarModosTrabajador(y, RoboticsTrabajadorDiaModo.presente)
            If m_ascendente Then
                If presentes_x < presentes_y Then
                    Return -1
                ElseIf presentes_x > presentes_y Then
                    Return 1
                End If

                If coste_x < coste_y Then
                    Return -1
                ElseIf coste_x > coste_y Then
                    Return 1
                End If
            Else
                If presentes_x < presentes_y Then
                    Return -1
                ElseIf presentes_x > presentes_y Then
                    Return 1
                End If

                If coste_x > coste_y Then
                    Return -1
                ElseIf coste_x < coste_y Then
                    Return 1
                End If
            End If

            Return String.Compare(x.Nombre, y.Nombre)

        End Function

    End Class

    Public Class RoboticsTrabajadorList
        Inherits List(Of RoboticsTrabajador)

        Public Function CosteMaximo() As Integer
            Dim res As Integer = 0
            For i As Integer = 0 To Me.Count - 1
                Dim t As RoboticsTrabajador = Me(i)
                If t.TrabajadorReal Then
                    For j As Integer = 0 To t.Dias.Count - 1
                        If t.Dias(j).Coste > res Then res = t.Dias(j).Coste
                    Next
                End If
            Next

            Return res
        End Function

        Public Function ContarPresentes(ByVal dia As Integer, ByVal puesto As String, ByVal turno As String) As Integer
            Dim res As Integer = 0
            For i As Integer = 0 To Me.Count - 1
                Dim d As RoboticsTrabajadorDia = Me(i).Dias(dia)
                If d.Modo = RoboticsTrabajadorDiaModo.presente Then
                    Dim hay_turno As Boolean = False
                    Dim hay_puesto As Boolean = False
                    If d.Turnos.BuscarTurno(turno) IsNot Nothing Then hay_turno = True
                    If (d.Puestos.Count > 0 AndAlso d.Puestos.BuscarPuesto(puesto) IsNot Nothing) OrElse (d.Puestos.Count = 0 AndAlso Me(i).Puestos.BuscarPuesto(puesto) IsNot Nothing) Then hay_puesto = True
                    If hay_puesto AndAlso hay_turno Then res += 1
                End If
            Next

            Return res
        End Function

        Public Function TrabajadoresHabilitados(ByVal dia As Integer, ByVal puesto As String, ByVal turno As String) As Integer()
            Dim l As List(Of Integer) = New List(Of Integer)()
            For i As Integer = 0 To Me.Count - 1
                Dim t As RoboticsTrabajador = Me(i)
                If t.Dias(dia).Turnos.BuscarTurno(turno) IsNot Nothing Then
                    If (t.Dias(dia).Puestos.Count > 0 AndAlso t.Dias(dia).Puestos.BuscarPuesto(puesto) IsNot Nothing) OrElse (t.Dias(dia).Puestos.Count = 0 AndAlso t.Puestos.BuscarPuesto(puesto) IsNot Nothing) Then l.Add(i)
                End If
            Next

            Return l.ToArray()
        End Function

    End Class

    Public Class RoboticsTrabajador
        Implements IComparer

        Private m_nombre As String

        Private m_id As String

        Private m_dias As RoboticsTrabajadorDiasList

        Private m_dias_anteriores As RoboticsTrabajadorDiasList

        Private m_dias_posteriores As RoboticsTrabajadorDiasList

        Private m_puestos As RoboticsTrabajadorPuestosList

        Private m_reglas As RoboticsReglasList

        Private m_real As Boolean = True

        Private m_coste_medio_no_real As Integer = 0

        Public Sub New()
        End Sub

        Public Sub New(ByVal nombre As String)
        End Sub

        Public Sub New(ByVal id As String, ByVal nombre As String)
            m_id = id
            m_nombre = nombre
            m_dias = New RoboticsTrabajadorDiasList()
            m_dias_anteriores = New RoboticsTrabajadorDiasList()
            m_dias_posteriores = New RoboticsTrabajadorDiasList()
            m_puestos = New RoboticsTrabajadorPuestosList()
            m_reglas = New RoboticsReglasList()
        End Sub

        Public Property Nombre As String
            Get
                Return m_nombre
            End Get

            Set(ByVal value As String)
                m_nombre = value
            End Set
        End Property

        Public Property Id As String
            Get
                Return m_id
            End Get

            Set(ByVal value As String)
                m_id = value
            End Set
        End Property

        Public ReadOnly Property Dias As RoboticsTrabajadorDiasList
            Get
                Return m_dias
            End Get
        End Property

        Public ReadOnly Property DiasAnteriores As RoboticsTrabajadorDiasList
            Get
                Return m_dias_anteriores
            End Get
        End Property

        Public ReadOnly Property DiasPosteriores As RoboticsTrabajadorDiasList
            Get
                Return m_dias_posteriores
            End Get
        End Property

        Public ReadOnly Property Puestos As RoboticsTrabajadorPuestosList
            Get
                Return m_puestos
            End Get
        End Property

        Public ReadOnly Property Reglas As RoboticsReglasList
            Get
                Return m_reglas
            End Get
        End Property

        Friend Property TrabajadorReal As Boolean
            Get
                Return m_real
            End Get

            Set(ByVal value As Boolean)
                m_real = value
            End Set
        End Property

        Friend Property CosteMedioTrabajadorNoReal As Integer
            Get
                Return m_coste_medio_no_real
            End Get

            Set(ByVal value As Integer)
                m_coste_medio_no_real = value
            End Set
        End Property

        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            Throw New NotImplementedException()
        End Function

    End Class

    Public Class RoboticsTrabajadorDiasList
        Inherits List(Of RoboticsTrabajadorDia)

        Public Function ContarElementosEntreFechas(ByVal fecha_inicial As DateTime, ByVal fecha1 As DateTime, ByVal fecha2 As DateTime) As Integer
            Dim res As Integer = 0
            For i As Integer = 0 To Me.Count - 1
                Dim d1 As DateTime = fecha_inicial.AddDays(i)
                If d1 >= fecha1 AndAlso d1 <= fecha2 Then res += 1
            Next

            Return res
        End Function

        Public Function TurnosEntreFechas(ByVal fecha_inicial As DateTime, ByVal fecha1 As DateTime, ByVal fecha2 As DateTime, ByVal turnos As RoboticsTurnosList) As Integer()
            Dim c As Integer = ContarElementosEntreFechas(fecha_inicial, fecha1, fecha2)
            If c = 0 Then Return New Integer(-1) {}
            Dim res As Integer() = New Integer(c - 1) {}
            Dim index As Integer = 0
            For i As Integer = 0 To Me.Count - 1
                Dim d1 As DateTime = fecha_inicial.AddDays(i)
                If d1 >= fecha1 AndAlso d1 <= fecha2 Then
                    Dim t As RoboticsTurno = turnos.BuscarTurno(Me(i).Turnos(0).Turno)
                    res(index) = t.Id
                    index += 1
                End If
            Next

            Return res
        End Function

        Public Function FinDeSemanaEntreFechas(ByVal fecha_inicial As DateTime, ByVal fecha1 As DateTime, ByVal fecha2 As DateTime, ByVal dia_fin_de_semana_inicio As Integer, ByVal duracion_fin_de_semana As Integer) As Integer
            Dim res As Integer = 0
            For i As Integer = 0 To Me.Count - 1
                Dim d1 As DateTime = fecha_inicial.AddDays(i)
                If d1 >= fecha1 AndAlso d1 <= fecha2 Then
                    Dim fin_de_semana As Boolean = True
                    Dim index As Integer = 0
                    For k As Integer = dia_fin_de_semana_inicio To dia_fin_de_semana_inicio + duracion_fin_de_semana - 1
                        If d1.AddDays(index).DayOfWeek <> CType((k Mod 7), DayOfWeek) Then
                            fin_de_semana = False
                            Exit For
                        End If

                        index += 1
                    Next

                    If fin_de_semana Then res += 1
                End If
            Next

            Return res
        End Function

        Public Function ContarDiasModo(ByVal modo As RoboticsTrabajadorDiaModo) As Integer
            Dim res As Integer = 0
            For i As Integer = 0 To Me.Count - 1
                If Me(i).Modo = modo Then res += 1
            Next

            Return res
        End Function

    End Class

    Public Enum RoboticsTrabajadorDiaModo
        disponible
        presente
        ausente
        si_domingo_presente_lunes_ausente
    End Enum

    Public Class RoboticsTrabajadorDia
        Implements ICloneable

        Private m_coste As Integer

        Private m_modo As RoboticsTrabajadorDiaModo

        Private m_turnos As RoboticsTrabajadorTurnosList

        Private m_puestos As RoboticsTrabajadorPuestosList

        Public Sub New()
        End Sub

        Public Sub New(ByVal coste As Integer, ByVal modo As RoboticsTrabajadorDiaModo, ByVal turnos As String())
        End Sub

        Public Sub New(ByVal coste As Integer, ByVal modo As RoboticsTrabajadorDiaModo, ByVal turnos As String(), ByVal puestos As String())
            m_coste = coste
            m_modo = modo
            m_turnos = New RoboticsTrabajadorTurnosList()
            If turnos IsNot Nothing Then
                For i As Integer = 0 To turnos.Length - 1
                    m_turnos.Add(New RoboticsTrabajadorTurno(turnos(i)))
                Next
            End If

            m_puestos = New RoboticsTrabajadorPuestosList()
            If puestos IsNot Nothing Then
                For i As Integer = 0 To puestos.Length - 1
                    m_puestos.Add(New RoboticsTrabajadorPuesto(puestos(i)))
                Next
            End If
        End Sub

        Public Property Coste As Integer
            Get
                Return m_coste
            End Get

            Set(ByVal value As Integer)
                m_coste = value
            End Set
        End Property

        Public Property Modo As RoboticsTrabajadorDiaModo
            Get
                Return m_modo
            End Get

            Set(ByVal value As RoboticsTrabajadorDiaModo)
                m_modo = value
            End Set
        End Property

        Public ReadOnly Property Turnos As RoboticsTrabajadorTurnosList
            Get
                Return m_turnos
            End Get
        End Property

        Public ReadOnly Property Puestos As RoboticsTrabajadorPuestosList
            Get
                Return m_puestos
            End Get
        End Property

        Private Function ICloneable_Clone() As Object Implements ICloneable.Clone
            Return Me.MemberwiseClone()
        End Function

    End Class

    Public Class RoboticsTrabajadorPuestosList
        Inherits List(Of RoboticsTrabajadorPuesto)

        Public Function BuscarPuesto(ByVal puesto As String) As RoboticsTrabajadorPuesto
            For i As Integer = 0 To Me.Count - 1
                If Me(i).Puesto = puesto Then Return Me(i)
            Next

            Return Nothing
        End Function

    End Class

    Public Class RoboticsTrabajadorPuesto
        Implements ICloneable

        Private m_puesto As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal puesto As String)
            m_puesto = puesto
        End Sub

        Public Property Puesto As String
            Get
                Return m_puesto
            End Get

            Set(ByVal value As String)
                m_puesto = value
            End Set
        End Property

        Private Function ICloneable_Clone() As Object Implements ICloneable.Clone
            Return Me.MemberwiseClone()
        End Function

    End Class

    Public Class RoboticsTrabajadorTurnosList
        Inherits List(Of RoboticsTrabajadorTurno)

        Public Function BuscarTurno(ByVal turno As String) As RoboticsTrabajadorTurno
            For i As Integer = 0 To Me.Count - 1
                If Me(i).Turno = turno Then Return Me(i)
            Next

            Return Nothing
        End Function

    End Class

    Public Class RoboticsTrabajadorTurno

        Private m_turno As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal turno As String)
            m_turno = turno
        End Sub

        Public Property Turno As String
            Get
                Return m_turno
            End Get

            Set(ByVal value As String)
                m_turno = value
            End Set
        End Property
    End Class

    Public Class RoboticsCalendarioDiasList
        Inherits List(Of RoboticsCalendarioDia)

        Private m_fecha_inicial As DateTime

        Public Property FechaInicial As DateTime
            Get
                Return m_fecha_inicial
            End Get

            Set(ByVal value As DateTime)
                m_fecha_inicial = value
            End Set
        End Property
    End Class

    Public Class RoboticsCalendarioDia
        Implements ICloneable

        Private m_puestos As RoboticsCalendarioPuestosList

        Public Sub New()
            m_puestos = New RoboticsCalendarioPuestosList()
        End Sub

        Public ReadOnly Property Puestos As RoboticsCalendarioPuestosList
            Get
                Return m_puestos
            End Get
        End Property

        Private Function ICloneable_Clone() As Object Implements ICloneable.Clone
            Return Me.MemberwiseClone()
        End Function

    End Class

    Public Class RoboticsCalendarioPuestosList
        Inherits List(Of RoboticsCalendarioPuesto)

        Public Function BuscarPuesto(ByVal puesto As String, ByVal turno As String) As RoboticsCalendarioPuesto
            For i As Integer = 0 To Me.Count - 1
                If Me(i).Puesto = puesto AndAlso Me(i).Turno = turno Then Return Me(i)
            Next

            Return Nothing
        End Function

        Public Function ContarPuestos(ByVal puesto As String, ByVal turno As String) As Integer
            Return ContarPuestos("", puesto, turno)
        End Function

        Public Function ContarPuestos(ByVal puesto As String, ByVal turnos As String()) As Integer
            Return ContarPuestos("", puesto, turnos, False)
        End Function

        Public Function ContarPuestos(ByVal unidad_productiva As String, ByVal puesto As String, ByVal turno As String) As Integer
            Return ContarPuestos(unidad_productiva, puesto, New String() {turno}, False)
        End Function

        Public Function ContarPuestos(ByVal unidad_productiva As String, ByVal puesto As String, ByVal turnos As String(), ByVal sumar As Boolean) As Integer
            Dim c As Integer = 0
            For i As Integer = 0 To Me.Count - 1
                If Me(i).Puesto = puesto AndAlso turnos.Contains(Me(i).Turno) AndAlso Me(i).UnidadProductiva = unidad_productiva Then
                    If Not sumar Then c += 1 Else c += Me(i).Cantidad
                End If
            Next

            Return c
        End Function

        Public Function SumarPuestos(ByVal puesto As String, ByVal turno As String) As Integer
            Return SumarPuestos("", puesto, turno)
        End Function

        Public Function SumarPuestos(ByVal unidad_productiva As String, ByVal puesto As String, ByVal turno As String) As Integer
            Return ContarPuestos(unidad_productiva, puesto, New String() {turno}, True)
        End Function

        Public Function IndexPuestos(ByVal puesto As String, ByVal turno As String) As Integer()
            Return IndexPuestos("", puesto, turno)
        End Function

        Public Function IndexPuestos(ByVal unidad_productiva As String, ByVal puesto As String, ByVal turno As String) As Integer()
            Dim l As List(Of Integer) = New List(Of Integer)()
            For i As Integer = 0 To Me.Count - 1
                If Me(i).Puesto = puesto AndAlso Me(i).Turno = turno AndAlso Me(i).UnidadProductiva = unidad_productiva Then l.Add(i)
            Next

            If l.Count > 0 Then Return l.ToArray() Else Return Nothing
        End Function

        Public Function ListarPuestosUnicos() As String()
            Dim l As List(Of String) = New List(Of String)()
            For i As Integer = 0 To Me.Count - 1
                If l.IndexOf(Me(i).Puesto) < 0 Then l.Add(Me(i).Puesto)
            Next

            Return l.ToArray()
        End Function

        Public Function SumarPuestosSinUnidadProductiva(ByVal puesto As String, ByVal turno As String) As Integer
            Dim c As Integer = 0
            For i As Integer = 0 To Me.Count - 1
                If Me(i).Puesto = puesto AndAlso Me(i).Turno = turno Then
                    c += Me(i).Cantidad
                End If
            Next

            Return c
        End Function

        Public Function BuscarPuestoPorTrabajador(ByVal id_trabajador As String) As RoboticsCalendarioPuesto
            For i As Integer = 0 To Me.Count - 1
                If Me(i).Trabajador = id_trabajador Then Return Me(i)
            Next

            Return Nothing
        End Function

    End Class

    Public Class RoboticsCalendarioPuestosListComparer
        Implements IComparer(Of RoboticsCalendarioPuesto)

        Private m_ascendente As Boolean

        Public Sub New(ByVal ascendente As Boolean)
            m_ascendente = ascendente
        End Sub

        Private Function IComparer_Compare(x As RoboticsCalendarioPuesto, y As RoboticsCalendarioPuesto) As Integer Implements IComparer(Of RoboticsCalendarioPuesto).Compare
            If m_ascendente Then
                If x.TrabajadoresDisponibles < y.TrabajadoresDisponibles Then
                    Return -1
                ElseIf x.TrabajadoresDisponibles > y.TrabajadoresDisponibles Then
                    Return 1
                End If
            Else
                If x.TrabajadoresDisponibles > y.TrabajadoresDisponibles Then
                    Return -1
                ElseIf x.TrabajadoresDisponibles < y.TrabajadoresDisponibles Then
                    Return 1
                End If
            End If

            Return String.Compare(x.Puesto & x.Turno, y.Puesto & y.Turno)
        End Function

    End Class

    Public Class RoboticsCalendarioPuesto

        Private m_puesto As String

        Private m_cantidad As Integer

        Private m_turno As String

        Private m_unidad_productiva As String

        Private m_trabajadores_disponibles As Integer

        Private m_procesado As Boolean

        Private m_trabajador As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal puesto As String, ByVal cantidad As Integer, ByVal turno As String)
        End Sub

        Public Sub New(ByVal unidad_productiva As String, ByVal puesto As String, ByVal cantidad As Integer, ByVal turno As String)
            m_unidad_productiva = unidad_productiva
            m_puesto = puesto
            m_cantidad = cantidad
            m_turno = turno
            m_trabajadores_disponibles = -1
            m_trabajador = Nothing
        End Sub

        Public Property Puesto As String
            Get
                Return m_puesto
            End Get

            Set(ByVal value As String)
                m_puesto = value
            End Set
        End Property

        Public Property Cantidad As Integer
            Get
                Return m_cantidad
            End Get

            Set(ByVal value As Integer)
                m_cantidad = value
            End Set
        End Property

        Public Property Turno As String
            Get
                Return m_turno
            End Get

            Set(ByVal value As String)
                m_turno = value
            End Set
        End Property

        Public Property UnidadProductiva As String
            Get
                Return m_unidad_productiva
            End Get

            Set(ByVal value As String)
                m_unidad_productiva = value
            End Set
        End Property

        Friend Property TrabajadoresDisponibles As Integer
            Get
                Return m_trabajadores_disponibles
            End Get

            Set(ByVal value As Integer)
                m_trabajadores_disponibles = value
            End Set
        End Property

        Friend Property Procesado As Boolean
            Get
                Return m_procesado
            End Get

            Set(ByVal value As Boolean)
                m_procesado = value
            End Set
        End Property

        Public Property Trabajador As String
            Get
                Return m_trabajador
            End Get

            Set(ByVal value As String)
                m_trabajador = value
            End Set
        End Property
    End Class

    Public Class RoboticsTurnosList
        Inherits List(Of RoboticsTurno)

        Public Function BuscarTurno(ByVal nombre As String) As RoboticsTurno
            For i As Integer = 0 To Me.Count - 1
                If Me(i).Nombre = nombre Then Return Me(i)
            Next

            Return Nothing
        End Function

        Public Function BuscarTurno(ByVal id As Integer) As RoboticsTurno
            For i As Integer = 0 To Me.Count - 1
                If Me(i).Id = id Then Return Me(i)
            Next

            Return Nothing
        End Function

        Public Function PosicionTurno(ByVal nombre As String) As Integer
            For i As Integer = 0 To Me.Count - 1
                If Me(i).Nombre = nombre Then Return i
            Next

            Return -1
        End Function

    End Class

    Public Class RoboticsTurno

        Private m_nombre As String

        Private m_hora_entrada As Integer

        Private m_hora_salida As Integer

        Private m_id As Integer

        Private m_horas As Integer

        Public Sub New()
        End Sub

        Public Sub New(ByVal nombre As String, ByVal id As Integer, ByVal hora_entrada_en_minutos As Integer, ByVal hora_salida_en_minutos As Integer)
        End Sub

        Public Sub New(ByVal nombre As String, ByVal id As Integer, ByVal hora_entrada_en_minutos As Integer, ByVal hora_salida_en_minutos As Integer, ByVal horas_del_turno_en_minutos As Integer)
            m_nombre = nombre
            m_hora_entrada = hora_entrada_en_minutos
            m_hora_salida = hora_salida_en_minutos
            m_id = id
            m_horas = horas_del_turno_en_minutos
        End Sub

        Public Property Nombre As String
            Get
                Return m_nombre
            End Get

            Set(ByVal value As String)
                m_nombre = value
            End Set
        End Property

        Public Property HoraEntrada As Integer
            Get
                Return m_hora_entrada
            End Get

            Set(ByVal value As Integer)
                m_hora_entrada = value
            End Set
        End Property

        Public Property HoraSalida As Integer
            Get
                Return m_hora_salida
            End Get

            Set(ByVal value As Integer)
                m_hora_salida = value
            End Set
        End Property

        Public Property Id As Integer
            Get
                Return m_id
            End Get

            Set(ByVal value As Integer)
                m_id = value
            End Set
        End Property

        Public Property Horas As Integer
            Get
                Return m_horas
            End Get

            Set(ByVal value As Integer)
                m_horas = value
            End Set
        End Property
    End Class

    Public Class RoboticsReglasList
        Inherits List(Of RoboticsRegla)

    End Class

    Public Enum RoboticsReglaTipo
        sin_regla
        descanso_entre_jornadas
        jornadas_de_descanso
        jornadas_seguidas_de_descanso_NO_USAR
        jornadas_obligatorias_de_trabajo
        jornadas_de_trabajo
        jornadas_seguidas_de_trabajo
        maximo_tiempo_de_trabajo
        maximo_jornadas_con_turno
        minimo_jornadas_con_turno
        establecer_fin_de_semana
        minimo_de_fines_de_semana
        maximo_de_fines_de_semana
    End Enum

    Public Class RoboticsRegla
        Implements ICloneable

        Private m_fecha_inicio As DateTime

        Private m_fecha_final As DateTime

        Private m_valor As Integer = 0

        Private m_valor_2 As Integer = 0

        Private m_tipo As RoboticsReglaTipo

        Public Sub New()
        End Sub

        Public Sub New(ByVal fecha_inicio As DateTime, ByVal fecha_final As DateTime, ByVal valor As Integer, ByVal modo As RoboticsReglaTipo)
        End Sub

        Public Sub New(ByVal fecha_inicio As DateTime, ByVal fecha_final As DateTime, ByVal valor As Integer, ByVal valor2 As Integer, ByVal modo As RoboticsReglaTipo)
            m_fecha_inicio = fecha_inicio
            m_fecha_final = fecha_final
            m_valor = valor
            m_valor_2 = valor2
            m_tipo = modo
        End Sub

        Public Property FechaInicio As DateTime
            Get
                Return m_fecha_inicio
            End Get

            Set(ByVal value As DateTime)
                m_fecha_inicio = value
            End Set
        End Property

        Public Property FechaFinal As DateTime
            Get
                Return m_fecha_final
            End Get

            Set(ByVal value As DateTime)
                m_fecha_final = value
            End Set
        End Property

        Public Property Valor As Integer
            Get
                Return m_valor
            End Get

            Set(ByVal value As Integer)
                m_valor = value
            End Set
        End Property

        Public Property Valor2 As Integer
            Get
                Return m_valor_2
            End Get

            Set(ByVal value As Integer)
                m_valor_2 = value
            End Set
        End Property

        Public Property Tipo As RoboticsReglaTipo
            Get
                Return m_tipo
            End Get

            Set(ByVal value As RoboticsReglaTipo)
                m_tipo = value
            End Set
        End Property

        Private Function ICloneable_Clone() As Object Implements ICloneable.Clone
            Return Me.MemberwiseClone()
        End Function

    End Class

    Public Class RoboticsSoluciones

        Public solucion_trabajador_por_dia_y_puesto As Long()()

        Public solucion_ocupacion_por_trabajador_y_dia As Long()()

        Public failures As Long

        Public branches As Long

        Public solucion_coste As Long

        Public solucion_trabajadores As Long

        Public Sub New(ByVal dias As Integer, ByVal puestos As Integer, ByVal trabajadores As Integer)
            solucion_trabajador_por_dia_y_puesto = New Long(dias - 1)() {}
            For i As Integer = 0 To dias - 1
                solucion_trabajador_por_dia_y_puesto(i) = New Long(puestos - 1) {}
            Next

            solucion_ocupacion_por_trabajador_y_dia = New Long(trabajadores - 1)() {}
            For i As Integer = 0 To trabajadores - 1
                solucion_ocupacion_por_trabajador_y_dia(i) = New Long(dias - 1) {}
            Next
        End Sub

    End Class

    Public Class MyCT
        Inherits NetConstraint

        Private m_vector As IntVar()

        Private m_demon As Demon

        Public Sub New(ByVal s As Solver, ByVal vector As IntVar())
            MyBase.New(s)
            m_vector = vector
        End Sub

        Public Overrides Sub Post()
            m_demon = solver().MakeConstraintInitialPropagateCallback(Me)
            For i As Integer = 0 To m_vector.Length - 1
                If m_vector(i) IsNot Nothing Then m_vector(i).WhenBound(m_demon)
            Next
        End Sub

        Public Overrides Sub InitialPropagate()
            For i As Integer = 1 To m_vector.Length - 1
                If m_vector(i - 1).Bound() AndAlso m_vector(i).Bound() Then
                    Dim v1 As Long = m_vector(i - 1).Value()
                    Dim v2 As Long = m_vector(i).Value()
                    If v1 > v2 Then
                        Console.WriteLine("BORRANDOOOOOOO")
                        m_vector(i - 1).RemoveValue(v1)
                    End If
                End If
            Next
        End Sub

    End Class

End Namespace