/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System.Threading.Tasks;

namespace Deveplex.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TEvent> : IIntegrationEventHandler
        where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
