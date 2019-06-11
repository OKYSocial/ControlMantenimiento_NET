﻿/*=================================================================================================================
  Bien como se puede apreciar no se disparan Querys desde la aplicación, todo se realiza en el lado del servidor,
  lo que se traduce en mayor eficiencia y velocidad en tiempo de respuesta. Todo lo que tiene que ver con acceso
  a datos está centralizado en este módulo, que bien podría separarse aún más si se quisiera, diseñando un módulo
  DAL para cada una de las estructuras de la BD, pero para este caso se puede dejar así y funciona bastante bien
 =================================================================================================================*/

using System;
using System.Data;
using MySql.Data.MySqlClient; // Requiere descargar mysql-connector-net-6.8.3
using System.Collections;
using ControlMantenimiento_NetDesktop.BO;

namespace ControlMantenimiento_NetDesktop.DAL
{
    public class AccesoDatos : IAccesoDatos
    {
        // Default Constructor
        public AccesoDatos() { }


        private MySqlConnection Cn;
        private MySqlDataReader sdr;
        private MySqlCommand Cmd;

        private void BuscarRegistro(string Tabla, int DatoBuscar)
        {
            try
            {
                Cn = new MySqlConnection(Conexion.ObtenerConexion);
                Cmd = new MySqlCommand("SPR_R_BuscarRegistro", Cn);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.Add("p_TABLA", MySqlDbType.VarChar, 20).Value = Tabla;
                Cmd.Parameters.Add("p_DATOBUSCAR", MySqlDbType.Int32, 4).Value = DatoBuscar;
                Cn.Open();
                sdr = Cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ArrayList CargarListas(string Tabla)
        {
            try
            {
                ArrayList arlLista = new ArrayList();
                using (MySqlConnection Cn = new MySqlConnection(Conexion.ObtenerConexion))
                {
                    Cmd = new MySqlCommand("SPR_R_CargarCombosListas", Cn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add("p_TABLA", MySqlDbType.VarChar, 30).Value = Tabla;
                    Cn.Open();
                    sdr = Cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        arlLista.Add(new CargaCombosListas(sdr.GetValue(0).ToString(), sdr.GetValue(0).ToString() + " " + sdr.GetValue(1).ToString()));
                    }
                    sdr.Close();
                    LiberarRecursos();
                    return arlLista;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ArrayList ControlProgramacion(string Tabla)
        {
            try
            {
                ArrayList arlListControlProgramacion = new ArrayList();
                using (MySqlConnection Cn = new MySqlConnection(Conexion.ObtenerConexion))
                {
                    Cmd = new MySqlCommand("SPR_R_CargarCombosListas", Cn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add("p_TABLA", MySqlDbType.VarChar, 30).Value = Tabla;
                    Cn.Open();
                    sdr = Cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        arlListControlProgramacion.Add(sdr.GetValue(2).ToString());
                        arlListControlProgramacion.Add(sdr.GetValue(0).ToString());
                        arlListControlProgramacion.Add(sdr.GetValue(1).ToString());
                    }
                    sdr.Close();
                    LiberarRecursos();
                    return arlListControlProgramacion;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /*
         =======================================================================================================================================================
         Inicio Operaciones sobre estructura Operarios
         =======================================================================================================================================================
         */
        public Operario ObtenerAcceso(string Documento, string Clave)
        {
            Operario operario = new Operario();
            try
            {
                Cn = new MySqlConnection(Conexion.ObtenerConexion);
                Cmd = new MySqlCommand("SPR_R_ObtenerAcceso", Cn);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.Add("p_DOCUMENTO", MySqlDbType.VarChar, 10).Value = Documento;
                Cmd.Parameters.Add("p_CLAVE", MySqlDbType.VarChar, 20).Value = Clave;
                Cn.Open();
                sdr = Cmd.ExecuteReader();
                if (sdr.Read())
                {
                    operario.Operario_id = Convert.ToInt32(sdr["OPERARIO_ID"].ToString());
                    operario.Nombres = sdr["NOMBRES"].ToString();
                    operario.Apellidos = sdr["APELLIDOS"].ToString();
                    operario.Perfil = Convert.ToInt32(sdr["PERFIL"].ToString());
                    sdr.Close();
                    LiberarRecursos();
                    return operario;
                }
                else
                {
                    sdr.Close();
                    LiberarRecursos();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Operario ObtenerOperario(int DatoBuscar)
        {
            Operario operario = new Operario();
            try
            {
                BuscarRegistro("TBL_OPERARIOS", DatoBuscar);
                if (sdr.Read())
                {
                    operario.Operario_id = Convert.ToInt32(sdr["OPERARIO_ID"].ToString());
                    operario.Documento = sdr["DOCUMENTO"].ToString();
                    operario.Nombres = sdr["NOMBRES"].ToString();
                    operario.Apellidos = sdr["APELLIDOS"].ToString();
                    operario.Correo = sdr["CORREO"].ToString();
                    operario.Telefono = sdr["TELEFONO"].ToString();
                    operario.Foto = sdr["FOTO"].ToString();
                    sdr.Close();
                    LiberarRecursos();
                    return operario;
                }
                else
                {
                    sdr.Close();
                    LiberarRecursos();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GuardarOperario(Operario operario, int Usuario)
        {
            try
            {
                using (MySqlConnection Cn = new MySqlConnection(Conexion.ObtenerConexion))
                {
                    Cmd = new MySqlCommand("SPR_IU_Operarios", Cn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add("p_OPERARIO_ID", MySqlDbType.Int32, 4).Value = operario.Operario_id;
                    Cmd.Parameters.Add("p_DOCUMENTO", MySqlDbType.VarChar, 10).Value = operario.Documento;
                    Cmd.Parameters.Add("p_NOMBRES", MySqlDbType.VarChar, 25).Value = operario.Nombres;
                    Cmd.Parameters.Add("p_APELLIDOS", MySqlDbType.VarChar, 25).Value = operario.Apellidos;
                    Cmd.Parameters.Add("p_CORREO", MySqlDbType.VarChar, 50).Value = operario.Correo;
                    Cmd.Parameters.Add("p_TELEFONO", MySqlDbType.Int32, 10).Value = operario.Telefono;                  
                    Cmd.Parameters.Add("p_FOTO", MySqlDbType.VarChar, 50).Value = operario.Foto;
                    Cmd.Parameters.Add("p_USUARIOCONECTADO", MySqlDbType.Int32, 4).Value = Usuario;
                    Cmd.Parameters.Add("p_RESULTADO", MySqlDbType.Int32, 1).Direction = ParameterDirection.Output;
                    Cn.Open();
                    Cmd.ExecuteNonQuery();
                    int Resultado = Convert.ToInt32(Cmd.Parameters["p_RESULTADO"].Value);
                    LiberarRecursos();
                    return Resultado;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public int GuardarCambioClave(int Usuario, string ClaveAnterior, string ClaveNueva)
        {
            try
            {
                using (MySqlConnection Cn = new MySqlConnection(Conexion.ObtenerConexion))
                {
                    Cmd = new MySqlCommand("SPR_U_CambioClave", Cn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add("p_OPERARIO_ID", MySqlDbType.Int32, 4).Value = Usuario;
                    Cmd.Parameters.Add("p_CLAVE_ANTERIOR", MySqlDbType.VarChar, 20).Value = ClaveAnterior;
                    Cmd.Parameters.Add("p_CLAVE_NUEVA", MySqlDbType.VarChar, 20).Value = ClaveNueva;
                    Cmd.Parameters.Add("p_RESULTADO", MySqlDbType.Int32, 1).Direction = ParameterDirection.Output;
                    Cn.Open();
                    Cmd.ExecuteNonQuery();
                    int Resultado = Convert.ToInt32(Cmd.Parameters["p_RESULTADO"].Value);
                    LiberarRecursos();
                    return Resultado;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*
         =======================================================================================================================================================
         Fin Operaciones sobre estructura Operarios
         =======================================================================================================================================================
         */

        /*
        =======================================================================================================================================================
        Inicio Operaciones sobre estructura ListaValores
        =======================================================================================================================================================
        */
        public ListaValores ObtenerListaValores(int DatoBuscar)
        {
            ListaValores listavalores = new ListaValores();
            try
            {

                BuscarRegistro("TBL_LISTAVALORES", DatoBuscar);
                if (sdr.Read())
                {
                    listavalores.Listavalores_id = Convert.ToInt32(sdr["LISTAVALORES_ID"].ToString());
                    listavalores.Nombre = sdr["NOMBRE"].ToString();
                    listavalores.Descripcion = sdr["DESCRIPCION"].ToString();
                    sdr.Close();
                    LiberarRecursos();
                    return listavalores;
                }
                else
                {
                    sdr.Close();
                    LiberarRecursos();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GuardarListaValores(ListaValores listavalores, int Usuario)
        {
            try
            {
                using (MySqlConnection Cn = new MySqlConnection(Conexion.ObtenerConexion))
                {
                    Cmd = new MySqlCommand("SPR_IU_ListaValores", Cn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add("p_LISTAVALORES_ID", MySqlDbType.Int32, 10).Value = listavalores.Listavalores_id;
                    Cmd.Parameters.Add("p_NOMBRE", MySqlDbType.VarChar, 50).Value = listavalores.Nombre;
                    Cmd.Parameters.Add("p_DESCRIPCION", MySqlDbType.VarChar, 255).Value = listavalores.Descripcion;
                    Cmd.Parameters.Add("p_TIPO", MySqlDbType.VarChar, 50).Value = listavalores.Tipo;
                    Cmd.Parameters.Add("p_USUARIOCONECTADO", MySqlDbType.Int32, 4).Value = Usuario;
                    Cmd.Parameters.Add("p_RESULTADO", MySqlDbType.Int32, 1).Direction = ParameterDirection.Output;
                    Cn.Open();
                    Cmd.ExecuteNonQuery();
                    int Resultado = Convert.ToInt32(Cmd.Parameters["p_RESULTADO"].Value);
                    LiberarRecursos();
                    return Resultado;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        /*
        =======================================================================================================================================================
        Fin Operaciones sobre estructura ListaValores
        =======================================================================================================================================================
        */

        /*
        =======================================================================================================================================================
        Inicio Operaciones sobre estructura Equipos
        =======================================================================================================================================================
        */
        public Equipo ObtenerEquipo(int DatoBuscar)
        {
            Equipo equipo = new Equipo();
            try
            {
                BuscarRegistro("TBL_EQUIPOS", DatoBuscar);
                if (sdr.Read())
                {
                    equipo.Equipo_id = Convert.ToInt32(sdr["EQUIPO_ID"].ToString());
                    equipo.Nombre_equipo = sdr["NOMBRE_EQUIPO"].ToString();
                    equipo.Marca = Convert.ToInt32(sdr["MARCA"].ToString());
                    equipo.Serie = sdr["SERIE"].ToString();
                    equipo.Linea = Convert.ToInt32(sdr["LINEA"].ToString());
                    if (sdr.GetBoolean(sdr.GetOrdinal("LUBRICACION"))) equipo.Lubricacion = 1;
                    sdr.Close();
                    LiberarRecursos();
                    return equipo;
                }
                else
                {
                    sdr.Close();
                    LiberarRecursos();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GuardarEquipo(Equipo equipo, int Usuario)
        {
            try
            {
                using (MySqlConnection Cn = new MySqlConnection(Conexion.ObtenerConexion))
                {
                    Cmd = new MySqlCommand("SPR_IU_Equipos", Cn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add("p_EQUIPO_ID", MySqlDbType.Int32, 10).Value = equipo.Equipo_id;
                    Cmd.Parameters.Add("p_NOMBRE_EQUIPO", MySqlDbType.VarChar, 50).Value = equipo.Nombre_equipo;
                    Cmd.Parameters.Add("p_MARCA", MySqlDbType.Int32, 10).Value = equipo.Marca;
                    Cmd.Parameters.Add("p_SERIE", MySqlDbType.VarChar, 20).Value = equipo.Serie;
                    Cmd.Parameters.Add("p_LINEA", MySqlDbType.Int32, 10).Value = equipo.Linea;
                    Cmd.Parameters.Add("p_LUBRICACION", MySqlDbType.Int32, 1).Value = equipo.Lubricacion;
                    Cmd.Parameters.Add("p_USUARIOCONECTADO", MySqlDbType.Int32, 4).Value = Usuario;
                    Cmd.Parameters.Add("p_RESULTADO", MySqlDbType.Int32, 1).Direction = ParameterDirection.Output;
                    Cn.Open();
                    Cmd.ExecuteNonQuery();
                    int Resultado = Convert.ToInt32(Cmd.Parameters["p_RESULTADO"].Value);
                    LiberarRecursos();
                    return Resultado;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*
       =======================================================================================================================================================
       Fin Operaciones sobre estructura Equipos
       =======================================================================================================================================================
       */

        /*
       =======================================================================================================================================================
       Inicio Operaciones sobre estructura Mantenimiento
       =======================================================================================================================================================
       */
        public Mantenimiento ObtenerMantenimiento(int DatoBuscar)
        {
            Mantenimiento mantenimiento = new Mantenimiento();
            try
            {
                BuscarRegistro("TBL_MANTENIMIENTO", DatoBuscar);
                if (sdr.Read())
                {
                    mantenimiento.Mantenimiento_id = Convert.ToInt32(sdr["MANTENIMIENTO_ID"].ToString());
                    mantenimiento.Equipo_id = Convert.ToInt32(sdr["EQUIPO_ID"].ToString());
                    mantenimiento.Operario_id = Convert.ToDouble(sdr["OPERARIO_ID"].ToString());
                    mantenimiento.Fecha = Convert.ToDateTime(sdr["FECHA"].ToString());
                    mantenimiento.Observaciones = sdr["OBSERVACIONES"].ToString();
                    sdr.Close();
                    LiberarRecursos();
                    return mantenimiento;
                }
                else
                {
                    sdr.Close();
                    LiberarRecursos();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GuardarMantenimiento(Mantenimiento mantenimiento, int Usuario)
        {
            try
            {
                using (MySqlConnection Cn = new MySqlConnection(Conexion.ObtenerConexion))
                {
                    Cmd = new MySqlCommand("SPR_IU_Mantenimiento", Cn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add("p_MANTENIMIENTO_ID", MySqlDbType.Int32, 4).Value = mantenimiento.Mantenimiento_id;
                    Cmd.Parameters.Add("p_EQUIPO_ID", MySqlDbType.Int32, 10).Value = mantenimiento.Equipo_id;
                    Cmd.Parameters.Add("p_OPERARIO_ID", MySqlDbType.VarChar, 10).Value = mantenimiento.Operario_id;
                    Cmd.Parameters.Add("p_FECHA", MySqlDbType.Date, 10).Value = mantenimiento.Fecha;
                    Cmd.Parameters.Add("p_OBSERVACIONES", MySqlDbType.VarChar, 255).Value = mantenimiento.Observaciones;
                    Cmd.Parameters.Add("p_USUARIOCONECTADO", MySqlDbType.Int32, 4).Value = Usuario;
                    Cmd.Parameters.Add("p_RESULTADO", MySqlDbType.Int32, 1).Direction = ParameterDirection.Output;
                    Cn.Open();
                    Cmd.ExecuteNonQuery();
                    int Resultado = Convert.ToInt32(Cmd.Parameters["p_RESULTADO"].Value);
                    LiberarRecursos();
                    return Resultado;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*
       =======================================================================================================================================================
       Fin Operaciones sobre estructura Mantenimiento
       =======================================================================================================================================================
       */


        public int EliminarRegistro(int DatoEliminar, string Tabla)
        {
            int Resultado = 0;
            try
            {

                using (MySqlConnection Cn = new MySqlConnection(Conexion.ObtenerConexion))
                {
                    Cmd = new MySqlCommand("SPR_D_Registro", Cn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.Add("p_TABLA", MySqlDbType.VarChar, 20).Value = Tabla;
                    Cmd.Parameters.Add("p_CONDICION", MySqlDbType.Int32, 4).Value = DatoEliminar;
                    Cmd.Parameters.Add("p_RESULTADO", MySqlDbType.Int32, 1).Direction = ParameterDirection.Output;
                    Cn.Open();
                    Cmd.ExecuteNonQuery();
                    Resultado = Convert.ToInt32(Cmd.Parameters["p_RESULTADO"].Value);
                    LiberarRecursos();
                    return Resultado;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void LiberarRecursos()
        {
            Cmd.Dispose();
            if (Cn != null)
            {
                Cn.Close();
                Cn.Dispose();
            }
        }
    }
}
