using Automatonymous;
using Play.Trading.Service.Contracts;
using System;

namespace Play.Trading.Service.StateMachines;

public class PurchaseStateMachine : MassTransitStateMachine<PurchaseState>
{
    public State Accepted { get; set; }
    public State ItemsGranted { get; set; }
    public State Completed { get; set; }
    public State Faulted { get; set; }

    public Event<PurchaseRequested> PurchaseRequested { get; }

    public PurchaseStateMachine()
    {
        InstanceState(state => state.CurrentState);
        ConfigureEvents();
        ConfigureInitialState();
    }

    private void ConfigureEvents() 
    {
        Event(() => PurchaseRequested);
    }

    private void ConfigureInitialState() 
    {
        Initially(
            When(PurchaseRequested)
                .Then(context =>
                {
                    context.Instance.UserId = context.Data.UserId;
                    context.Instance.ItemId = context.Data.ItemId;
                    context.Instance.Quantity = context.Data.Quantity;
                    context.Instance.Received = DateTimeOffset.UtcNow;
                    context.Instance.LastUpdated = context.Instance.Received;
                })
                .TransitionTo(Accepted)
        );
    }
}
