/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deveplex.EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler : IIntegrationEventHandler
    {
        Task HandleAsync(dynamic eventData);
    }
}
