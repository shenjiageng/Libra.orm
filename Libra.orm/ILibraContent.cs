using Libra.orm.DbModel;
using Libra.orm.LibraBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm
{
    public interface ILibraContent
    {
        public IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression) where T : LibraBaseModel, new();

        public IEnumerable<T> QueryRealtime<T>(Expression<Func<T, bool>> expression) where T : LibraBaseModel, new();

        public IEnumerable<dynamic> Query(string sql, params Expression<Func<string, object>>[] expressions);

        public IEnumerable<dynamic> QueryRealtime(string sql, params Expression<Func<string, object>>[] expressions);

        public bool Insert<T>(T t) where T : LibraBaseModel, new();

        public bool Delete<T>(Expression<Func<T, bool>> expression) where T : LibraBaseModel, new();

        public bool Update<T>(T t, Expression<Func<T, bool>> expression) where T : LibraBaseModel, new();
    }
}