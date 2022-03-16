/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Deveplex.EventBus
{
    /// <summary>
    /// 
    /// </summary>
    public enum DeliveryMode : byte
    {
        /// <summary>
        /// 非持久化
        /// </summary>
        NonPersistent = 1,

        /// <summary>
        /// 持久化
        /// </summary>
        persistent = 2

    }
}
