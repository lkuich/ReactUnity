using System;

namespace ReactUnity
{
    public interface IApp
    {
        /// <param name="action">Action to run on the UI thread</param>
        void RunOnUnityThread(Action action);
        T Init<T>();
    }
}