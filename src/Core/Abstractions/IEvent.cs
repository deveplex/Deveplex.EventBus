/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Deveplex.EventBus.Abstractions
{
    public interface IEvent
    {
        /// <summary>
        /// 
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// 1：非持久化，2：持久化
        /// </summary>
        byte Mode { get; }

        /// <summary>
        /// Gets or sets queue message automatic deletion time (in milliseconds). Default 0 ms (0 days).
        /// </summary>
        int ExpiresIn { get; }

    }
}