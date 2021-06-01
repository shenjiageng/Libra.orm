using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Libra.orm
{
    internal static class DbTypeFormat
    {
        internal static SqlDbType TypeToDbType(this Type t)
        {
            return Type.GetTypeCode(t) switch
            {
                TypeCode.Boolean => SqlDbType.Bit,
                TypeCode.Byte => SqlDbType.TinyInt,
                TypeCode.DateTime => SqlDbType.DateTime,
                TypeCode.Decimal => SqlDbType.Decimal,
                TypeCode.Double => SqlDbType.Float,
                TypeCode.Int16 => SqlDbType.SmallInt,
                TypeCode.Int32 => SqlDbType.Int,
                TypeCode.Int64 => SqlDbType.BigInt,
                TypeCode.SByte => SqlDbType.TinyInt,
                TypeCode.Single => SqlDbType.Real,
                TypeCode.String => SqlDbType.NVarChar,
                TypeCode.UInt16 => SqlDbType.SmallInt,
                TypeCode.UInt32 => SqlDbType.Int,
                TypeCode.UInt64 => SqlDbType.BigInt,
                TypeCode.Char => SqlDbType.Char,
                _ => t == typeof(byte[]) ? SqlDbType.Binary : SqlDbType.Variant,
            };
        }

        internal static string ToTSqlText(this SqlDbType type)
        {
            return type switch
            {
                SqlDbType.BigInt => "long",
                SqlDbType.Binary => "binary",
                SqlDbType.Bit => "bit",
                SqlDbType.Char => "char",
                SqlDbType.DateTime => "datetime",
                SqlDbType.Decimal => "numeric",
                SqlDbType.Float => "float",
                SqlDbType.Image => "image",
                SqlDbType.Int => "int",
                SqlDbType.Money => "money",
                SqlDbType.NChar => "nchar(max)",
                SqlDbType.NText => "ntext",
                SqlDbType.NVarChar => "nvarchar(max)",
                SqlDbType.Real => "real",
                SqlDbType.UniqueIdentifier => "uniqueIdentifier",
                SqlDbType.SmallDateTime => "smalldatetime",
                SqlDbType.SmallInt => "smallint",
                SqlDbType.SmallMoney => "smallmoney",
                SqlDbType.Text => "text",
                SqlDbType.Timestamp => "timestamp",
                SqlDbType.TinyInt => "tinyint",
                SqlDbType.VarBinary => "varbinary",
                SqlDbType.VarChar => "varchar",
                SqlDbType.Variant => "sql_variant",
                SqlDbType.Xml => "xml",
                SqlDbType.Date => "date",
                SqlDbType.Time => "time",
                SqlDbType.DateTime2 => "datetime2",
                SqlDbType.DateTimeOffset => "datetimeoffset",
                _ => "",
            };
        }
    }
}
