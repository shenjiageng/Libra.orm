using Libra.orm.LibraAttributes.DbFilter;
using Libra.orm.LibraAttributes.DbMapping;
using Libra.orm.LibraAttributes.DbValidata;
using Libra.orm.LibraAttributes.DbValiformat;
using System;

namespace Libra.orm.test
{
    [Serializable]
    [LibraTable("SYS_USER_INFO")]
    public class UserInfoModel
    {
        [LibraKey]
        public int ID { get; set; }

        /// <summary>
        /// 用户GUID
        /// </summary>
        [LibraRequired]
        public Guid UGuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 部门的GUID标志
        /// </summary>
        [LibraRequired]
        public Guid DeptGuid { get; set; }

        /// <summary>
        /// 用户部门全路径
        /// </summary>
        [LibraRequired, LibraNvarchar(200)]
        public string FullDepartment { get; set; }

        /// <summary>
        /// 用户的名称
        /// </summary>
        [LibraRequired, LibraNvarchar(50)]
        public string Username { get; set; }

        /// <summary>
        /// 所处职位
        /// </summary>
        [LibraRequired, LibraNvarchar(50)]
        public string Position { get; set; }

        /// <summary>
        /// 用户的账号信息
        /// </summary>
        [LibraRequired, LibraNvarchar(100)]
        public string Account { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [LibraRequired, LibraNvarchar(50)]
        public string Password { get; set; }

        /// <summary>
        /// 人员加入时间
        /// </summary>
        [LibraRequired, LibraDatetime]
        public DateTime RegisterTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 启用状态
        /// </summary>
        [LibraRequired]
        public bool IsEnable { get; set; } = true;

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        [LibraRequired]
        public bool IsAdministrator { get; set; } = false;

        /// <summary>
        /// 是否在线
        /// </summary>
        [LibraRequired]
        public bool NowOnLine { get; set; } = false;

        [LibraDatetime]
        public DateTime LastOffline { get; set; }

        /// <summary>
        /// 是否已被注销
        /// </summary>
        [LibraRequired]
        public bool IsCancellation { get; set; } = false;

        /// <summary>
        /// 注销时间
        /// </summary>
        [LibraDatetime]
        public DateTime CancellationTime { get; set; }
    }
}
