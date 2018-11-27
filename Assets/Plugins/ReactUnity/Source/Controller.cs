using System;

namespace ReactUnity
{
    public abstract class Controller<T> where T : IModel
    {
        public IApp App { get; set; }

        protected internal event Action<T> StateChanged;
        protected internal T State { get; set; }

        /// <summary>
        /// Sets the state
        /// </summary>
        /// <param name="model">The model to set</param>
        protected internal void SetState(T model)
        {
            if (this.StateChanged == null)
                throw new Exception(
                    "Tried to call SetState before the Controller was initialized! " +
                    "You cannot call SetState in the Controller constructor"
                );

            this.State = model;
            StateChanged?.Invoke(State);
        }

        /// <summary>
        /// Called when the Controller starts
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// OnDestroy is called when a presenter is destroyed
        /// Override to add custom OnDestroy behaviour
        /// </summary>
        public virtual void OnDestroy() { }
    }
}
