# ReactUnity Micro-Framework

![React Unity](https://cdn-images-1.medium.com/max/800/1*ib6BiApseraEnwnHttI5Rg.gif)


## Installation

1. Open your Unity Project
2. Target .NET 4.X Runtime, set API Compatability level to .NET Standard 2.0
3. Go the the [Zenject releases](https://github.com/modesttree/Zenject/releases) page, and import the bare Unity Package
4. Go to the [ReactUnity releases](https://github.com/lkuich/ReactUnity/releases) page and grab the latest Unity Package
5. Optionally, install the VS Templates by downloading them from the ReactUnity releases page, and pasting the ZIP files in:

> %USERPROFILE%\Documents\Visual Studio 2017\Templates\ItemTemplates\Visual C#\1033

***

## Getting Started

This guide explains the various components of our new React Unity micro-framework. The framework takes hints from React and MVC design patterns, with the goal of creating responsive, modular, and easily testable Unity applications. We plan to phase this into AoD incrementally over time.

At the core of this approach is the Reactive aspect. Each component has its own state, and components "react" to the changes in their state. This saves us time and complexity, and we end up with a less tightly coupled code base, since each component is responsible for itself.

### Controllers

Controllers handle the logic specific to the Component in question.

The most important job of Controllers is setting the state, as Presenters themselves cannot directly modify their own state. Controllers can also make calls to Services, gather the needed data to change the state, and apply the change. The Presenter will then react to said state changes.

Every Controller has a Start method. This method is called once your Controller is initialized, since you cannot make state changes until your Controller is initialized.

```csharp
class ExampleController : Controller<ExampleModel> {
    protected override void Start() {
        // Your controller has been created, you can set the state here!
        // SetState(new ExampleState());
    }
}
```

### Models

Models represent state, and can be passed between Controllers and Presenters. There is nothing special about Model objects and you can even serialize them, they all however must inherit IModel to be used to retain state. They are separate from, and have no awareness of the game thread (MonoBehaviour, GameObjects, ect), Controllers, or Presenters.

### Presenters

Presenters can be considered the "Views", driving the presentation of UI elements. Presenters extend MonoBehaviour. Each Presenter has its own Controller and Model.

Since Presenters inherit MonoBehaviour, you have access to the typical methods such as Awake, Enable, ect. However, you should not use the usual Start method. Instead, there is a new Render method that gets called everytime the state is changed from the Controller. It looks like this:

```csharp
class ExamplePresenter : Presenter<ExampleController, ExampleModel> {
    protected override void Render(ExampleModel state) {
        // Update your UI components here
        // state contains your new data
    }
}
```

The state parameter contains the new state of the Presenter, modified by the Controller. Presenters cannot modify their own state directly, however they can make calls to their Controller, which can modify state. Don't confuse Render with Start, as it can be called many times. Awake should be implemented if you want to ensure code is only called once when the Presenter is created.

It's recommended to use the Unity Reactive Presenter VS template, as it will create the basic structure for your Presenter, Model, and Controller for you, saving time.

### Services

Services are similar to Models in the fact that they are not special. They are separate from, and have no awareness of the game thread (MonoBehaviour, GameObjects, ect), Controllers, or Presenters. Services do the "heavy" logic, such as reading data from local or remote sources, managing the local database, managing scenes, ect.

Services can only be initialized from the Controller. To access a service, simply pass it's interface as a constructor parameter for the Controller:

```csharp
class ExampleController : Controller<ExampleModel> {
    ExampleController(IExample exampleService) {
        // ...
    }
}
```

We use reflection to find all the services that need binding, and use Zenject DI to bind the found Services to their Interfaces at runtime. By default the first time you call a service, its bound class will be initialized and any future calls are referring to the same object, so you don't have to worry about memory management or using too many services. The dependency injection system can also be configured to create a new instance for each injection, if that is prefered. We expect most of our services to be singletons though.

To create a service, it must be in the ReactUnity.Services namespace, and follow a simple, but strict naming convention. Every service must have an interface, the interface is named the same as the service class, but with a leading "I".

Define its interface and its class like so:

```csharp
namespace ReactUnity.Services {
    public interface IExampleService : IService { }
    class ExampleService : IExampleService { }
}
```

If you fail to follow this example naming convention, reflection will fail and the service will not be loaded. Be very cautious with this.

It's recommended you use the Unity Service VS Template, it will create the basic structure for your service automatically.

***

## Testing

### Unit Testing

One of our goals for this system is making a large portion of the code base easily unit testable. We want to run tests on UI logic as well as services, independent of the full Unity runtime. A number of decisions were made to support this.

### Avoid MonoBehaviors

We avoid inheriting from MonoBehaviour in Controllers, Models, and Services. This means that all of these objects can be used outside of the Unity runtime environment. It is especially significant that Controllers aren’t MonoBehaviours as it opens the bulk of UI logic to unit testing.

### Using Dependency Injection

We have integrated the Zenject Dependency Injection library into the framework. It works in manner very similar to what we use for Business Layers in the Mission Maker. When you need a service you add it’s interface as an argument to your controller or service’s constructor. The framework will provide the registered implementation of the interface when the controller is being built.

Dependency injection is important for testing since we can optionally register “mock” versions of services when testing. For example when testing a controller which usually reads something from disk using a service we could register a service which generates identical data in memory, to make the test repeatable.

The dependency injection system also enables our goal of testing outside of the Unity runtime. If you ever need to access a Unity API, try abstracting it in a service. That way we can register a mock version which doesn’t actually reference Unity for testing.
