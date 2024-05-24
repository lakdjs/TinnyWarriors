using System;
namespace Core
{
    public interface IStateMachine
    {
        Type CurrentState { get; }
        bool ChangeState<T>();
        void Update();
    }
}