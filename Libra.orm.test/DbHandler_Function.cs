using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public class DbHandler_Function : IDisposable
    {
        string ConnetcionString = string.Empty; //ConfigurationManager.ConnectionStrings["DbName"].ConnectionString;

        public DbHandler_Function()
        {
            if (string.IsNullOrEmpty(ConnetcionString))
            {
                throw new Exception("错误的链接字符串！");
            }
        }

        public DbHandler_Function(string _connetcionString)
        {
            this.ConnetcionString = _connetcionString;
            if (string.IsNullOrEmpty(ConnetcionString))
            {
                throw new Exception("错误的链接字符串！");
            }
        }

        /// <summary>
        /// 执行无返回结果的存储过程。
        /// </summary>
        /// <param name="sqlProcedure">存储过程名称</param>
        /// <param name="pms">存储过程参数</param>
        public void ExecuteProcedure(string sqlProcedure, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(ConnetcionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlProcedure, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 180;
                    if (pms.Length > 0)
                        cmd.Parameters.AddRange(pms);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行无返回结果的存储过程。并附带输出参数
        /// </summary>
        /// <param name="sqlProcedure">存储过程名称</param>
        /// <param name="pms">存储过程参数</param>
        public void ExecuteProcedure(string sqlProcedure, ref SqlParameter[] outPar, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(ConnetcionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlProcedure, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 180;
                    if (pms.Length > 0)
                        cmd.Parameters.AddRange(pms);
                    if (outPar.Length > 0)
                        cmd.Parameters.AddRange(outPar);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行有返回结果的存储过程
        /// </summary>
        /// <param name="sqlProcedure">存储过程名称</param>
        /// <param name="pms">存储过程参数</param>
        public T ExecuteProcedure<T>(string sqlProcedure, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(ConnetcionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlProcedure, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 180;
                    if (pms.Length > 0)
                        cmd.Parameters.AddRange(pms);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                        return default(T);
                    return (T)result;
                }
            }
        }


        /// <summary>
        /// 执行有返回结果的存储过程。并附带输出参数
        /// </summary>
        /// <param name="sqlProcedure">存储过程名称</param>
        /// <param name="pms">存储过程参数</param>
        public T ExecuteProcedure<T>(string sqlProcedure, ref SqlParameter[] outPar, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(ConnetcionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlProcedure, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 180;
                    if (pms.Length > 0)
                        cmd.Parameters.AddRange(pms);
                    if (outPar.Length > 0)
                        cmd.Parameters.AddRange(outPar);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                        return default(T);
                    return (T)result;
                }
            }
        }

        /// <summary>
        /// 执行带返回参数的存储过程
        /// </summary>
        /// <param name="sqlProcedure">存储过程名称</param>
        /// <param name="outPar">返回的存储过程参数</param>
        /// <param name="pms">存储过程参数</param>
        /// <returns>如果有数据，则返回SqlDataReader对象,在外层using对其释放内存。不存在符合数据则返回null</returns>
        public SqlDataReader ExecuteReaderProcedure(string sqlProcedure, out SqlConnection con, ref SqlParameter[] outPar, params SqlParameter[] pms)
        {
            con = new SqlConnection(ConnetcionString);
            using (SqlCommand cmd = new SqlCommand(sqlProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 180;
                if (pms.Length > 0)
                    cmd.Parameters.AddRange(pms);
                if (outPar.Length > 0)
                    cmd.Parameters.AddRange(outPar);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    return reader;
                }
                else
                {
                    reader.Close();
                    con.Close();
                    con.Dispose();
                }
            }
            return null;
        }

        /// <summary>
        /// 执行带返回参数的存储过程返回数据表
        /// </summary>
        /// <param name="sqlProcedure">存储过程名称</param>
        /// <param name="outPar">返回参数的参数组</param>
        /// <param name="pms">其他参数组</param>
        /// <returns></returns>
        public DataTable ExecuteDataTableProcedure(string sqlProcedure, ref SqlParameter[] outPar, params SqlParameter[] pms)
        {
            using (SqlDataReader reader = ExecuteReaderProcedure(sqlProcedure, out SqlConnection con, ref outPar, pms))
            {
                if (reader != null)
                    return DataReaderToDataTable(reader, con);
                else
                    return null;
            }
        }

        /// <summary>
        /// 执行无返回参数的存储过程
        /// </summary>
        /// <param name="sqlProcedure">存储过程名称</param>
        /// <param name="pms">存储过程参数</param>
        /// <returns>如果有数据，则返回SqlDataReader对象,在外层using对其释放内存。不存在符合数据则返回null</returns>
        public SqlDataReader ExecuteReaderProcedure(string sqlProcedure, out SqlConnection con, params SqlParameter[] pms)
        {
            con = new SqlConnection(ConnetcionString);
            using (SqlCommand cmd = new SqlCommand(sqlProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 180;
                if (pms.Length > 0)
                    cmd.Parameters.AddRange(pms);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    return reader;
                }
                else
                {
                    reader.Close();
                    con.Close();
                    con.Dispose();
                }
            }
            return null;
        }

        /// <summary>
        /// 执行存储过程，返回多个表
        /// </summary>
        /// <param name="sqlProcedure"></param>
        /// <param name="con"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSetProcedure(string sqlProcedure, out SqlConnection con, params SqlParameter[] pms)
        {
            con = new SqlConnection(ConnetcionString);
            DataSet ds = new DataSet();
            using (SqlDataAdapter sda = new SqlDataAdapter(sqlProcedure, con))
            {
                sda.SelectCommand.CommandTimeout = 180;
                sda.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (pms.Length > 0)
                    sda.SelectCommand.Parameters.AddRange(pms);
                sda.Fill(ds);
            }
            return ds;
        }

        /// <summary>
        /// 执行无返回参数的存储过程返回数据表
        /// </summary>
        /// <param name="sqlProcedure">存储过程名称</param>
        /// <param name="pms">存储过程参数</param>
        /// <returns></returns>
        public DataTable ExecuteDataTableProcedure(string sqlProcedure, params SqlParameter[] pms)
        {
            using (SqlDataReader reader = ExecuteReaderProcedure(sqlProcedure, out SqlConnection con, pms))
            {
                if (reader != null)
                    return DataReaderToDataTable(reader, con);
                else
                    return null;
            }
        }

        /// <summary>
        /// 数据库增删改操作方法
        /// </summary>
        /// <param name="sql">需要执行的sql语句</param>
        /// <param name="pms">sql语句中的参数数组</param>
        /// <returns>int类型，返回影响行数</returns>
        public int ExecuteNonQuery(string sql, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(ConnetcionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 180;
                    if (pms != null) cmd.Parameters.AddRange(pms);
                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    return result;
                }
            }
        }

        /// <summary>
        /// 执行单一语句的增删改事务
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="pms">参数</param>
        /// <returns></returns>
        public int ExecuteNonQueryTran(string sql, params SqlParameter[] pms)
        {
            SqlCommand[] commands = { new SqlCommand(sql) };
            List<SqlParameter[]> list = new List<SqlParameter[]>();
            if (pms.Length > 0)
                list.Add(pms);
            else
                list.Add(null);
            int result = ExecuteNonQuery(commands, list);
            return result;
        }

        /// <summary>
        ///  数据库增删改多操作处理事务
        /// </summary>
        /// <param name="cmd">SqlCommand数组，需要一次性全部执行成功的sql</param>
        /// <param name="pms">SqlParameter，参数数组</param>
        /// <returns>返回0，则表示操作失败。</returns>
        public int ExecuteNonQuery(SqlCommand[] cmd, List<SqlParameter[]> pms)
        {
            SqlConnection con = new SqlConnection(ConnetcionString);
            int row = 0;
            con.Open();
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                for (int i = 0; i < cmd.Length; i++)
                {
                    if (cmd[i] == null) continue;
                    cmd[i].CommandTimeout = 180;
                    cmd[i].Connection = con;
                    cmd[i].Transaction = tran;
                    if (pms[i] != null)
                        cmd[i].Parameters.AddRange(pms[i]);
                    row = cmd[i].ExecuteNonQuery();
                    cmd[i].Clone();
                    cmd[i].Dispose();
                }
                tran.Commit();
            }
            catch
            {
                con.Close();
                tran.Rollback();
            }
            finally
            {
                tran.Dispose();
            }
            return row;
        }

        /// <summary>
        /// 数据库查询返回一行一列操作方法 <T>表示返回的数据类型
        /// </summary>
        /// <param name="sql">需要执行的sql语句</param>
        /// <param name="pms">sql语句中的参数数组</param>
        /// <returns>object类型，接收后强转</returns>
        public T ExecuteScalar<T>(string sql, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(ConnetcionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 180;
                    if (pms != null) cmd.Parameters.AddRange(pms);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    con.Close();
                    con.Dispose();
                    if (result == null || result == DBNull.Value)
                        return default(T);
                    return (T)result;
                }
            }
        }

        /// <summary>
        /// 数据库查询返回多行多列操作方法
        /// </summary>
        /// <param name="sql">需要执行的sql语句</param>
        /// <param name="pms">sql语句中的参数数组</param>
        /// <returns>SqlDataReader对象，请声明SqlDataReader接收返回值</returns>
        public SqlDataReader ExecuteReader(string sql, out SqlConnection con, params SqlParameter[] pms)
        {
            con = new SqlConnection(ConnetcionString);
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.CommandTimeout = 180;
                if (pms != null) cmd.Parameters.AddRange(pms);
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    return reader;
                }
                catch
                {
                    con.Close();
                    con.Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// DataReader转DataTable
        /// </summary>
        /// <param name="reader">reader对象</param>
        /// <returns></returns>
        public DataTable DataReaderToDataTable(SqlDataReader reader, SqlConnection con)
        {
            try
            {
                DataTable objDataTable = new DataTable();
                int intFieldCount = reader.FieldCount;
                for (int intCounter = 0; intCounter < intFieldCount; ++intCounter)
                {
                    objDataTable.Columns.Add(reader.GetName(intCounter), reader.GetFieldType(intCounter));
                }

                objDataTable.BeginLoadData();
                object[] objValues = new object[intFieldCount];
                while (reader.Read())
                {
                    reader.GetValues(objValues);
                    objDataTable.LoadDataRow(objValues, true);
                }
                reader.Close();
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }
                objDataTable.EndLoadData();
                return objDataTable;
            }
            catch (Exception ex)
            {
                reader.Close();
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }
                throw new Exception("转换出错!", ex);
            }
        }

        /// <summary>
        /// 通过SqlBulkCopy复制table数据到数据库
        /// </summary>
        /// <param name="dataset"></param>
        public void SqlbulkcopyInsert(DataTable datatable, string tablename)
        {
            if (datatable.Rows.Count > 0)
            {
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(ConnetcionString, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    sqlbulkcopy.DestinationTableName = tablename;//数据库中的表名
                    sqlbulkcopy.WriteToServer(datatable);
                    sqlbulkcopy.Close();
                }
            }
        }

        /// <summary>
        /// 数据库查询返回一个数据表
        /// </summary>
        /// <param name="sql">需要执行的sql语句</param>
        /// <param name="pms">sql语句中的参数数组</param>
        /// <returns>DataTable对象，请声明DataTable接收返回值</returns>
        public DataTable ExecuteDataTable(string sql, params SqlParameter[] pms)
        {
            using (SqlDataReader reader = ExecuteReader(sql, out SqlConnection con, pms))
            {
                DataTable table = DataReaderToDataTable(reader, con);
                return table;
            }
        }

        /// <summary>
        /// 设配器方式查询返回一个数据表
        /// </summary>
        /// <param name="sql">需要执行的sql语句</param>
        /// <param name="pms">sql语句中的参数数组</param>
        /// <returns>DataTable对象，请声明DataTable接收返回值</returns>
        public DataTable AdtExecuteDataTable(string sql, params SqlParameter[] pms)
        {
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnetcionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 180;
                    if (pms != null) cmd.Parameters.AddRange(pms);
                    con.Open();
                    using (SqlDataAdapter dap = new SqlDataAdapter())
                    {
                        dap.SelectCommand = cmd;
                        dap.Fill(table);
                        con.Close();
                        return table.DefaultView.Table;
                    }
                }
            }
        }

        /// <summary>
        /// 释放当前对象
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~DbHandler_Function()
        {
            Dispose();
        }
    }
}
