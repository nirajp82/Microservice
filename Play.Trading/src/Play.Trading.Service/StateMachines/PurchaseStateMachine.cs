using System;
using Automatonymous;
using Play.Trading.Service.Contracts;

namespace Play.Trading.Service.StateMachines;

public class PurchaseStateMachine : MassTransitStateMachine<PurchaseState>
{
    public State Accepted { get; }
    public State ItemsGranted { get; }
    public State Completed { get; }
    public State Faulted { get; }

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
                .Then(context => {
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