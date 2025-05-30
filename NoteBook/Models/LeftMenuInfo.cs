using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.Models
{
   public class LeftMenuInfo
   {
        public string Icon { get; set; }
        public string MenuName { get; set; }
        public string ViewName { get; set; }
    }
    /// <summary>
    /// 数据状态 0正常 1删除 2新增 3修改
    /// </summary>
    public enum DataStatus {
        Normal = 0,
        Delete = 1,
        Add = 2,
        Alter=3
    }

}
