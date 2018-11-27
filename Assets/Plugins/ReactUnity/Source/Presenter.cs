using UnityEngine;

namespace ReactUnity
{
    public abstract class Presenter<C, M> : MonoBehaviour where C : Controller<M> where M : IModel
    {
        protected C Controller { get; set; }
        private IApp app { get; set; }

        private void Start()
        {
            if (Controller == null)
            {
                CreateController();
            }
        }

        private void SetupController()
        {
            if (app == null)
            {
                var context = GameObject.Find("ProjectContext(Clone)");
                app = context.GetComponent<IApp>();
                if (app == null)
                    throw new System.Exception("App was not added to this scene!");
            }

            Controller = app.Init<C>();
            Controller.App = app;
            Controller.StateChanged += model => Render(model);
        }

        private void CreateController()
        {
            SetupController();

            Controller.Start();
        }

        private void CreateController(M state)
        {
            SetupController();

            Controller.SetState(state);
            Controller.Start();
        }

        /// <summary>
        /// Called upon every state change
        /// </summary>
        /// <param name="model">Model to render</param>
        protected abstract void Render(M model);

        /// <summary>
        /// Creates new instance of a Presenter
        /// </summary>
        /// <typeparam name="TCont">Controller Type</typeparam>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="view">GameObject to apply to</param>
        /// <param name="state">Default state</param>
        /// <returns></returns>
        protected GameObject InstantiateView<TCont, TModel>(GameObject view, TModel state)
            where TCont : Controller<TModel>
            where TModel : IModel
        {
            var obj = Instantiate(view);
            var presenter = obj.GetComponent<Presenter<TCont, TModel>>();
            if (presenter != null)
            {
                presenter.CreateController(state);
            }
            return obj;
        }

        protected void OnDestroy()
        {
            OnPresenterDestroy();

            if (Controller != null)
                Controller.OnDestroy();
        }

        /// <summary>
        /// Called when the Presenter is destroyed
        /// Override to add your own additional logic
        /// </summary>
        protected virtual void OnPresenterDestroy() { }
    }
}