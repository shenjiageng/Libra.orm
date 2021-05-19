using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.DbCache
{
    public enum LibraSqlCacheBuilderType
    {
        Insert,
        Delete,
        Update,
        Query,
        NoLockQuery
    }
}
