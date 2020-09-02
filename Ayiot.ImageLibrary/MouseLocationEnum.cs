using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ayiot.ImageLibrary
{
    public enum MouseLocationEnum
    {
        /// <summary>
        /// 任意位置
        /// </summary>
        None,
        /// <summary>
        /// 左边框
        /// </summary>
        Left,
        /// <summary>
        /// 左上调整块
        /// </summary>
        LeftUp,
        /// <summary>
        /// 上边框
        /// </summary>
        Up,
        /// <summary>
        /// 右上调整块
        /// </summary>
        RightUp,
        /// <summary>
        /// 右边框
        /// </summary>
        Right,
        /// <summary>
        /// 右下调整块
        /// </summary>
        RightDown,
        /// <summary>
        /// 下边框
        /// </summary>
        Down,
        /// <summary>
        /// 左下调整块
        /// </summary>
        LeftDown
    }
}
