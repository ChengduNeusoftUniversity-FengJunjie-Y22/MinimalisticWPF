# MinimalisticWPF

#### Easier animation construction √
#### Support for conditional animation loading within the MVVM design pattern √
#### Aspect-oriented Programming simplification √
#### Dynamic Theme √
#### ObjectPool [ Beta…… ]

- [github](https://github.com/Axvser/MinimalisticWPF)
- [中文文档](https://axvser.github.io/MinimalisticWPFDoc/)

---

## Version
<details>
<summary>V1.5.6 - [ Release ] - Basic animation implementation component</summary>

  - Repair it.
    - Gradient results may be distorted when the frame rate is between 57 and 61
    - When the frame rate is lower than 100, high-speed State switching may cause startup failure of the state machine
    - When the frame rate is 0, an error occurs
    - An error occurs when the duration is 0
  - Adjust
    - The default frame rate is set to 120Hz
    - Frame rate is limited from 1 to 240, out of range will be corrected automatically
  - New
    - class.IsSatisfy() allows you to decide whether to initiate a pre-described transition based on whether the instance object meets a specified condition
    ```csharp
            var board = GD.Transition()
                .SetProperty(x => x.RenderTransform, rotateTransform, translateTransform, scaleTransform)
                .SetParams((x) =>
                {
                    x.Duration = 3;
                    x.Acceleration = 1;
                });

            var result = GD.IsSatisfy(x => x.Width < 1000, board, true);
            // Parameter 1. Condition (required)
            // Parameter 2. Perform this transition if the conditions are met (optional)
            // Parameter 3. If the transition effect is object-based, whether to enable the whitelist mechanism (optional)
    ```
  - 2.0.0 version preview
    - ★ Greatly optimize the document
    - ★ Open up more functions that can only operate inside the state machine system
    - Fix more potential issues
    - Try to optimize performance further
    - Add more common extension methods
    - ⚠ Remove all non-core components
</details>

<details>
<summary>V1.8.9 - [ Release ] - Animation system optimization and aspect-oriented programming support</summary>

  - [ AOP ] Add a delegate parameter to get the return value of the previous method
  ```csharp
  proxy.SetMethod(nameof(pro.GetName),
                object? (args, last) => { MessageBox.Show($"before default method"); return "AOP before\n"; },
                object? (args, last) => { return $"{last}AOP Coverage \n"; },
                object? (args, last) => { MessageBox.Show($"results :\n{last}AOP after\n"); return null; });
  ```

</details>

<details>
<summary>V1.9.5 - [ Release ] - ObjectPool And DynamicTheme</summary>

### Ⅰ ObjectPool Support
- Attribute
  - PoolAttribute
  - PoolFetchAttribute
  - PoolDisposeAttribute
  - PoolDisposeConditionAttribute
  ```csharp
    [Pool(5, 10)] //The initial number of units is 5. If resources are insufficient, the system automatically expands to a maximum of 10 units
    public class Unit
    {
        public static int count = 1;

        public Unit() { Value = count; count++; }

        public int Value { get; set; } = 1;

        [PoolFetch] //Triggered when the object is removed from the pool
        public void WhileFetch()
        {
            MessageBox.Show($"Fetch {Value}");
        }

        [PoolDispose] //Triggered when the object pool is automatically reclaimed
        public void WhileDispose()
        {
            MessageBox.Show($"Dispose {Value}");
        }

        [PoolDisposeCondition] //If the return value is true, the resource can be reclaimed automatically
        public bool CanDisposed()
        {
            return Value % 2 == 0 ? true : false;
        }
    }
  ```
- Static Method
  - Get Instance
  ```csharp
  var unit = Pool.Fetch(typeof(Unit)) as Unit;
  if (Pool.TryFetch(typeof(Unit), out var result))
  {
      var unit = result as Unit;
  }
  ```
  - Start Auto Dispose
  ```csharp
  Pool.RunAutoDispose(typeof(Unit), 5000);
  ```
  - End Auto Dispose
  ```csharp
  Pool.StopAutoDispose(typeof(Unit));
  ```
  - Force resource release
  ```csharp
  var unit = Pool.Fetch(typeof(Unit)) as Unit;
  unit.PoolDispose();
  ```

### Ⅱ Changes in mechanisms
- Dictionary __> ConcurrentDictionary
  - Try using thread-safe dictionaries
  - Has been applied to [ StateMahine ]
  - Has been applied to [ Pool ]
- Dictionary __> Grouping By Type
  - Use struction like " Dictionary&lt;Type,Dictionary&lt;object,StateMachine>> ".You can now dispose of all statemachines of a given type by using StateMachine.Dispose()
- State __> No check for type.Only propertyName & propertyValue
</summary>
</details>

<details>
<summary>V2.0.0 - [ Beta ] - ★ Roslyn Support - ★ This is a starting point, and as versions evolve, more and more features will be used in a way that will be simplified by using source generators</summary>

### Attribute => Generating source code
- This is the first version that uses the source generator
  - 1.Simplifies ViewModel creation
  - 2.Simple aspect-oriented programming implementation
  ```csharp
  using MinimalisticWPF;

  namespace WpfApp3
  { 
    [AspectOriented] //Use [ AspectOriented ] for the class
    internal partial class Class1 //Make sure it is [ partial ]
    {
        [VMProperty]
        private int _id = 1; //Use [ VMProperty ] for the field
    }
  }
  ```
  - After regenerating, you can use it like this
  ```csharp
            var c1 = new Class1();
            MessageBox.Show(c1.Id.ToString()); //The source generator has created the Id property for your _id field

            c1.Proxy.SetMethod(nameof(c1.TestMethod),
                (p, last) => { MessageBox.Show("before method"); return null; },
                null,
                null); //Intercepting method calls
            c1.Proxy.SetPropertyGetter(nameof(c1.Id),
                (p, last) => { MessageBox.Show("before getter"); return null; },
                null,
                null); //Intercept property Getter calls

            c1.Proxy.TestMethod(-1);//The source generator has created a Proxy property for you to access Class1's properties and methods, so the above interception takes effect
            var a = c1.Proxy.Id;
  ```
- Also, the library does not support user controls yet, as dynamic themes, animations, and many other features need a transformative upgrade that will be addressed within the next five releases

</details>

---

## Document

<details>
<summary>V2.0.x - [ Writing ] - Testing phase, no documentation, specific information please check the history of version changes [ Version ]</summary>

###

</details>


<details>
<summary>V1.8.x - [ Finished ]</summary>

## Ⅰ API
### 1. State - Keep track of the property values of an object at a time
|Method|Param|Return|Meaning|
|------|-----|------|-------|
|FromObject|object|TempState|Record all supported properties based on an object instance|
|FromType||TempState|Only attribute values can be recorded manually|
|SetName|string|TempState|Give the State a name|
|SetProperty|Expression , object|TempState|Logging attribute values|
|ToState||State ☆|Completion record|
### 2. StateVector - Describe the relation in which a condition corresponds to an animation
|Method|Param|Return|Meaning|
|------|-----|------|-------|
|Create||StateVector||
|AddCondition|Expression , State , Action&lt;TransitionParams>?|StateVector|Describes a mapping that automatically loads an object to a specified State animation when a specified condition is met|
|Check|T , StateMachine||Check if any of the conditions are met, and if so, call the specified StateMachine instance to load the corresponding animation|
### 3. Transition - Animation behavior
###### Transition
|Method|Param|Return|Meaning|
|------|-----|------|-------|
|CreateBoardFromObject|object|TransitionBoard|Creating a drawing board|
|CreateBoardFromType||TransitionBoard|Creating a drawing board|
###### TransitionBoard
|Method|Param|Return|Meaning|
|------|-----|------|-------|
|SetProperty|Expression , object|TransitionBoard|Set the target property value|
|SetParams|Action&lt;TransitionParams>|TransitionBoard|Set animation detail parameters|
|ReflectAny|object|TransitionBoard|Reflection specifies all attribute values of the target as the target|
|ReflectExcept|object , params Expression<Func<T, string>>[]|TransitionBoard|Reflection specifies a partial attribute value of the target as the target|
### 4. Any Class [Extension]
|Method|Overloading|Meaning|
|------|------|-------|
|Transition|+0|Quick-start animation|
|IsSatisfy|+4|Starts the animation with a conditional|
|BeginTransition|+3|Start the animation with State or TransitionBoard|
|FindStateMachine|+0|Finds whether the current object has a state machine instance|
### 5.TransitionParams
|Property|type|defualt|Meaning|
|--------|----|-------|-------|
|Start|Action|null|
|Update|Action|null|
|LateUpdate|Action|null|
|Completed|Action|null|
|StartAsync|Func&lt;Task>|null|
|UpdateAsync|Func&lt;Task>|null|
|LateUpdateAsync|Func&lt;Task>|null|
|CompletedAsync|Func&lt;Task>|null|
|FrameRate|int|120 HZ|
|Duration|double|0 s|
|IsAutoReverse|bool|false|
|LoopTime|int|0|
|Acceleration|double|0|
|IsUnSafe|bool|false|Whether to enable the UnSafe animation|
|IsQueue|bool|false|Whether to queue for execution|
|IsLast|bool|false|Whether to clear the animation queue at the end of this animation|
|IsUnique|bool|true|If an animation already exists that points to a State with the specified name, whether the animation should be added to the queue this time, i.e., whether the animation is unique|

## Ⅱ Example
#### 1. Quickly load an animation
```csharp
GD.Transition()
    .SetProperty(x => x.Opacity, 0.3)
    .SetProperty(x => x.Width, 200)
    .SetProperty(x => x.Height, 200)
    .SetParams((x) =>
    {
        x.Duration = 2;
    })
    .Start();
```
#### 2. Start the animation based on the [State]
```csharp
State _board = State.FromType<Grid>()
    .SetName("Animation1")
    .SetProperty(x => x.Opacity, 0.3)
    .SetProperty(x => x.Width, 200)
    .SetProperty(x => x.Height, 200)
    .ToState();

Action<TransitionParams> _params = (x) =>
{
    x.Duration = 2;
};

GD.BeginTransition(_board, _params);
```
#### 3. Start the animation based on the [TransitionBoard]
```csharp
TransitionBoard<Grid> _board = Transition.CreateBoardFromType<Grid>()
    .SetProperty(x => x.Opacity, 0.3)
    .SetProperty(x => x.Width, 200)
    .SetProperty(x => x.Height, 200)
    .SetParams((x) =>
    {
        x.Duration = 2;
    });

GD.BeginTransition(_board);
```
#### 4. UnSafe
- Predefined
```csharp
static TransitionBoard<Grid> Safe = Transition.CreateBoardFromType<Grid>()
    .SetProperty(x => x.Width, 100)
    .SetProperty(x => x.Height, 100)
    .SetParams((x) =>
    {
        x.Duration = 1;
    });
TransitionBoard<Grid> UnSafe_1 = Transition.CreateBoardFromType<Grid>()
    .SetProperty(x => x.Opacity, 1)
    .SetParams((x) =>
    {
        x.IsUnSafe = true;
        x.Duration = 1;
    });
TransitionBoard<Grid> UnSafe_2 = Transition.CreateBoardFromType<Grid>()
    .SetProperty(x => x.Opacity, 1)
    .SetParams((x) =>
    {
        x.IsUnSafe = true;
        x.Duration = 1;
    });
```
- Splicing
```csharp
if (GD1.Width > 1)
{
    UnSafe_1.SetProperty(x => x.Opacity, 0.8);
}

if (GD2.Height > 1)
{
    UnSafe_2.SetProperty(x => x.Opacity, 0.3);
}

GD1.BeginTransition(UnSafe_1);
GD2.BeginTransition(UnSafe_2);

GD1.BeginTransition(Safe);
GD2.BeginTransition(Safe);
```
- UnSafe must be executed before Safe

#### 5. LifeCycle
```csharp
Action<TransitionParams> _params = (x) =>
{
    x.Duration = 2;

    x.Start = () =>
    {

    };
    x.Update = () =>
    {

    };
    x.LateUpdate = () =>
    {

    };
    x.Completed = () =>
    {

    };

    x.StartAsync = () =>
    {

    };
    x.UpdateAsync = () =>
    {

    };
    x.LateUpdateAsync = () =>
    {

    };
    x.CompletedAsync = () =>
    {

    };
};
```
#### 6. Good practice in MVVM design pattern
- DataContext
```xml
<UserControl.DataContext>
    <local:MPasswordBoxViewModel x:Name="ViewModel"
                                 CornerRadius="10"
                                 FontSizeConvertRate="0.7"
                                 TextBrush="White"/>
</UserControl.DataContext>
```
- ViewModel
```csharp
/// <summary>
/// DataContext as password box
/// </summary>
public class MPasswordBoxViewModel : ViewModelBase<MPasswordBoxViewModel, MPasswordBoxModel>
{
    public MPasswordBoxViewModel() { }

    //Default color
    public static State Default = State.FromType<MPasswordBoxViewModel>()
        .SetName("default")
        .SetProperty(x => x.PasswordStrengthColor, Brushes.White)
        .ToState();

    //There are four levels of password strength, corresponding to four different colors
    public static State Level1 = State.FromType<MPasswordBoxViewModel>()
        .SetName("L1")
        .SetProperty(x => x.PasswordStrengthColor, Brushes.Tomato)
        .ToState();
    public static State Level2 = State.FromType<MPasswordBoxViewModel>()
        .SetName("L2")
        .SetProperty(x => x.PasswordStrengthColor, Brushes.Yellow)
        .ToState();
    public static State Level3 = State.FromType<MPasswordBoxViewModel>()
        .SetName("L3")
        .SetProperty(x => x.PasswordStrengthColor, Brushes.Cyan)
        .ToState();
    public static State Level4 = State.FromType<MPasswordBoxViewModel>()
        .SetName("L4")
        .SetProperty(x => x.PasswordStrengthColor, Brushes.Lime)
        .ToState();

    //Switches to the specified State when the specified password strength is reached
    public StateVector<MPasswordBoxViewModel> Condition { get; set; } = StateVector<MPasswordBoxViewModel>.Create()
        .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 0, Default, (x) => { x.Duration = 0.3; })
        .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 1, Level1, (x) => { x.Duration = 0.3; })
        .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 2, Level2, (x) => { x.Duration = 0.3; })
        .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 3, Level3, (x) => { x.Duration = 0.3; })
        .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 4, Level4, (x) => { x.Duration = 0.3; });

    //Real password
    public string TruePassword
    {
        get => Model.TruePassword;
        set
        {
            Model.TruePassword = value;
            string result = string.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                result += ReplacingCharacters;
            }
            UIPassword = result;
            OnPropertyChanged(nameof(TruePassword));

            OnConditionsChecked();
            // Methods specified by the IConditionalTransition interface
            // StateViewModelBase is the smallest unit that implements the MVVM and connects to the state machine. It implements the INotifyPropertyChanged and IConditionalTransition interfaces
            // This will animate the password strength when it changes
        }
    }

    /// <summary>
    /// Passwords that are visible to the user
    /// </summary>
    public string UIPassword
    {
        get => Model.UIPassword;
        set
        {
            Model.UIPassword = value;
            OnPropertyChanged(nameof(UIPassword));
        }
    }

    /// <summary>
    /// The character used to replace the real password
    /// </summary>
    public string ReplacingCharacters
    {
        get => Model.ReplacingCharacters;
        set
        {
            Model.ReplacingCharacters = value;
            string result = string.Empty;
            for (int i = 0; i < TruePassword.Length; i++)
            {
                result += ReplacingCharacters;
            }
            UIPassword = result;
            OnPropertyChanged(nameof(ReplacingCharacters));
        }
    }

    /// <summary>
    /// Border color corresponding to password strength
    /// </summary>
    public Brush PasswordStrengthColor
    {
        get => Model.PasswordStrengthColor;
        set
        {
            Model.PasswordStrengthColor = value;
            OnPropertyChanged(nameof(PasswordStrengthColor));
        }
    }
}
```
- Code-Behind
```csharp
public MPasswordBox()
{
    InitializeComponent();
    this.StateMachineLoading(ViewModel);
}
```

#### 7.AOP
- For types that need proxies, we need to create an interface first
```csharp
public interface IPropertyProxy : IProxy
{
    string Name { get; set; }
    string GetName();
}
public class TObj : IPropertyProxy
{
    public TObj() { }

    public string Name { get; set; } = "defaultValue";

    public string GetName()
    {
        return "defaultResult";
    }
}
```
- Create the proxy object [ proxy ]
  - Intercepting a specified method
  - Add custom logic before and after method execution
  - Override the default implementation of the method
```csharp
TObj obj = new TObj();
IPropertyProxy proxy = obj.CreateProxy<IPropertyProxy>();
proxy.SetMethod(nameof(pro.GetName),
              object? (args, last) => { MessageBox.Show($"before default method"); return "AOP before\n"; },
              object? (args, last) => { return $"{last}AOP Coverage \n"; },
              object? (args, last) => { MessageBox.Show($"results :\n{last}AOP after\n"); return null; });
```
- Tips
  - Passing null indicates no appending or overwriting
  - [ args ] Represents the params received when the method is called
  - [ last ] Represents the return value of the previous step

</details>

<details>
<summary>V1.5.x - [ Finished ]</summary>

## Key Features
- [State Machine System - Create linear transitions to specified properties of specified instances](#StateMachineSystem)
  - [StateMachine]()
  - [TransitionParams]()
  - [MVVM]()
  - Property types that can participate in state machine transitions
    - double
    - Brush
    - Transform
    - CornerRadius
    - Thickness
    - Point
    - ★ ILinearInterpolation (This interface allows any custom type to support state machine transitions)
## Auxiliary Features
- [ExtensionMethods](#ExtensionMethods)
  - [string]
    - value conversion
    - Fuzzy matching
    - Profiling tools (e.g. extracting resource addresses from html)
    - Password strength
  - [UserControls](#UserControls)
    - A uniform dark theme
    - Font size ADAPTS to control height
    - All animation effects are based on state machine systems, which are both directly available and practices of the state machine system
      - Notification - Glass style notification/select box
      - MProgressBar - Bar/ring free switching progress bar
      - mtopbar - Top sidebar of the program
## Non-core Components
The MinimalisticWPF namespace does not include the following services, which will be referenced separately
  - [Web Services](#WebServices)
    - [ Autonavi ]() WebApi
      - IP Services
      - Weather services

## Supporting frameworks
- [.NET6.0-windows]
- [.NET8.0-windows]
## Getting
- [github][1]
- [nuget][2]

[1]: https://github.com/ChengduNeusoftUniversity-FengJunjie-Y22/MinimalisticWPF
[2]: https://www.nuget.org/packages/MinimalisticWPF/

# StateMachineSystem
- State & StateVecotr is built for MVVM and allows for automatic state switching with preset conditions
- StateMachine theory can make linear transitions for any type of property. Almost all types non-MVVM include an extension method for quickly creating linear transitions for object instances. This is probably the library's most common method
  - ★ Advantages
    - Create complex transitions with little code
    - Update() allows you to decide what you want to do for each frame of the transition, just like in game development engines
    - The functionality is not limited to animations, it modifies any possible type, any possible property, for example testing the program with simulated data is one of the uses the authors tried
  - ⚠ ️ Disadvantages
    - Unstable performance (relative to components like StoryBoard and VisualState)
    - The types of properties that support transitions are very limited, and although an interface is provided to solve this problem, it is not convenient to leave the calculation of linear interpolation to the implementation class of the interface
[![pAu2vOP.md.png](https://s21.ax1x.com/2024/09/15/pAu2vOP.md.png)](https://imgse.com/i/pAu2vOP)

---

## StateMachine
- For any type [where T: class, new()] you can create a linear transition using the following code
- For example perform the following transition on a 100×100 Grid
```csharp
        private RotateTransform rotateTransform = new RotateTransform(-280, 50, 50);
        private TranslateTransform translateTransform = new TranslateTransform(-100, -50);
        private ScaleTransform scaleTransform = new ScaleTransform(2, 2, 50, 50);

        private void GD_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GD.Transition()
                .SetProperty(x => x.RenderTransform, rotateTransform, translateTransform, scaleTransform)
                .SetProperty(x => x.Opacity, 0.2)
                .SetProperty(x => x.CornerRadius,new CornerRadius(15))
                .SetParams((x) =>
                {
                    x.Duration = 0.4;
                    x.Completed = () =>
                    {
                        Notification.Message("Transition complete √");
                    };
                })
                .Start();
        }
```
- But the default properties available for state machine transitions are finite types, so how do you make custom types work with state machines?
  - Step 1. Implement a Class1, which is a custom type that supports state machine transitions
    - Here Class1 is the composition of Thickness and CornerRadius
    - Need to implement the interface method Interpolate (), steps is the number of interpolations, you need to customize how to split the two Class1 into steps to evenly interpolate
    ```csharp
    public class Class1 : ILinearInterpolation
    {
        public object Current { get; set; }
        public List<object?> Interpolate(object? current, object? target, int steps)
        {
            List<object?> result = new List<object?>();

            var v1 = current as Class1 ?? new Class1();
            var v2 = target as Class1 ?? new Class1();
            var itemsA = ILinearInterpolation.CornerRadiusComputing(v1.CornerRadius, v2.CornerRadius, steps);
            var itemsB = ILinearInterpolation.ThicknessComputing(v1.Thickness, v2.Thickness, steps);
            for (var i = 0; i < itemsA.Count; i++)
            {
                var temp = new Class1();
                temp.CornerRadius = itemsA[i] as CornerRadius? ?? new CornerRadius();
                temp.Thickness = itemsB[i] as Thickness? ?? new Thickness();
                result.Add(temp);
            }

            return result;
        }


        public Class1() { Current = this; }
        public Class1(CornerRadius cornerRadius, Thickness thickness) { Current = this; CornerRadius = cornerRadius; Thickness = thickness; }
        public CornerRadius CornerRadius { get; set; } = new CornerRadius();
        public Thickness Thickness { get; set; } = new Thickness();
    }
    ```
  - Step 2. Implement a Class2, which is the type that contains the Class1 property, is the type that actually needs to use the state machine
    ```csharp
    public class Class2
    {
        public Class2() { }
       
        public Class1 Class1 { get; set; } = new Class1();

        //…… other properties
    }
    ```
  - Step 3. At this point, you are ready to apply state machine transitions to Class2.Class1
    ```csharp
       Class1 T1 = new Class1();
       Class1 T2 = new Class1(new CornerRadius(10), new Thickness(2, 3, 1, 0));
  
       Class2 TargetClass2 = new Class2();
  
       TargetClass2.Transition()
           .SetProperty(x => x.Class1, T2)
           .SetParams((x) =>
           {
               x.Duration = 2;
               x.Start = () =>
               {
                    Notification.Message($"old Thickness {TargetClass2.Class1.Thickness}\n" +
                            $"old CornerRadius {TargetClass2.Class1.CornerRadius}");
               };
               x.Completed = () =>
               {
                    Notification.Message($"new Thickness {TargetClass2.Class1.Thickness}\n" +
                            $"new CornerRadius {TargetClass2.Class1.CornerRadius}");
               };
           })
           .Start();
    ```

---

## TransitionParams 
- Contains a number of parameters for the details of the transition
  - Transition parameters
  - Transition creation parameters
  - Lifecycle related parameters

| property | type | default | meaning |
|--------|-----|-------|-------|
|Duration|double|0| Animation duration (in s)|
|Start|Action|null| is executed once before the animation starts |
|Update|Action|null| is executed once before each frame of the animation starts |
|LateUpdate|Action|null| is executed once after each frame of the animation |
|Completed|Action|null| is executed once after the animation has finished |
|IsQueue|bool|false| Whether the newly enabled animation will be queued or not, otherwise the animation will be interrupted |
|IsLast|bool|false| Whether this is the last animation to be executed, if so it will clear the queued animation |
|IsUnique|bool|true| Should a transition animation that points to the same State continue if one exists |
|FrameRate|int|165| Animation frame rate |
|WaitTime|double|0.008| is rarely used, but if you find places where the probability doesn't animate or the probability is twitching, you can increase this value appropriately|
|Acceleration|double|0|The waiting time of each frame is shown as a straight line with slope [Acceleration] in the floor plan|

- Use cases
  - Set transition parameters (lambdas) for StateVector
  - Set a transition parameter (Lambda) for Transition()

```csharp
Set((x)=>
{
    x.Duration = 0.1;
    x.IsLast = true;
    x.Update = () =>
    {
        Notification.Message("Before the start of a frame");
    };
})
```

---

## State & StateVector & IConditionalTransition
- State describes the value of an object's property at a moment in time
- StateVector describes which transitions are created under which conditions
- IConditionalTransition allows you to automatically create a transition when a specified condition is met on an instance object
  - Examples
    - When the mouse is inside the control, make its background opacity transition to 0.2
    - Make the background opacity transition to 0 when the mouse leaves the control
    - Xaml - View
    ```xml
    <UserControl.DataContext>
        <local:MButtonViewModel x:Name="ViewModel"/>
    </UserControl.DataContext>
    ```
    - C# - View
    ```csharp
    public partial class MButton : UserControl
    {
        public MButton()
        {
            InitializeComponent();
            this.StateMachineLoading(ViewModel);
        }
    }
    ```
    - C# - ViewModel
    ```csharp
    public class MButtonViewModel : ViewModelBase<MButtonViewModel, MButtonModel>
    {
        public MButtonViewModel() { }

        public static State Start = State.FromObject(new MButtonViewModel())
            .SetName("defualt")
            .SetProperty(x => x.HoverBackgroundOpacity, 0)
            .ToState();
        public static State MouseIn = State.FromObject(new MButtonViewModel())
            .SetName("mouseInside")
            .SetProperty(x => x.HoverBackgroundOpacity, 0.2)
            .ToState();

        public StateVector<MButtonViewModel> Condition { get; set; } = StateVector<MButtonViewModel>.Create()
            .AddCondition(x => x.IsMouseInside, MouseIn, (x) => { x.Duration = 0.2; })
            .AddCondition(x => !x.IsMouseInside, Start, (x) => { x.Duration = 0.2; });

        public override bool IsMouseInside
        {
            get => base.IsMouseInside;
            set
            {
                base.IsMouseInside = value;
  
                OnConditionsChecked();
                // Change IsMouseInside when mouse in/out of control
                //IsMouseInside is modified to check if the condition is true, if so, switch State
            }
        }
    }
    ```

---
---

# ExtensionMethods
## string
- Value conversion
```csharp
   string valueA = "-123.7";
   string valueB = "TrUE";
   string valueC = "#1e1e1e";
   
   var result1 = valueA.ToInt();
   var result2 = valueA.ToDouble();
   var result3 = valueA.ToFloat();

   var result4 = valueB.ToBool();

   var result5 = valueC.ToBrush();
```
- Fuzzy matching
```csharp
   string template = "abcdefg";

   string sourceA = "abc";
   List<string> sourceB = new List<string>()
   {
       "abcdegf",
       "cbdgafe"
   };

   var similarity1 = sourceA.LevenshteinDistance(template)
   //Returns the shortest edit distance

   var similarity2 = sourceA.JaroWinklerDistance(template)
   //Returns approximation

   var result3 = template.BestMatch(sourceB, 3);
   //Edit the result with a minimum distance of less than 3

   var result4 = template.BestMatch(sourceB, 0.5);
   //The result with the approximation degree greater than 0.5 and the largest
```
- Folder generation operations
```csharp
   string folderNameA = "FF1";
   string folderNameB = "FF2";
   string folderNameC = "FF3";
   //The folder name

   var result1 = folderNameA.CreatFolder();
   //From the.exe location, create a folder named "FF1"

   var result2 = folderNameC.CreatFolder(folderNameA,folderNameB);
   //From the.exe location, create a folder named "FF1/FF2/FF3"
```
- Xml and Json serialization
```csharp
   string folderName = "Data";

   string fileName1 = "firstPersondata";
   string fileName2 = "secondPersondata";

   var target = new Person();

   var result1 = fileName1.CreatXmlFile(folderName.CreatFolder(), target);
   var result2 = fileName2.CreatJsonFile(folderName.CreatFolder(), target);
```
- Xml and Json deserialization
```csharp
   string folderName = "Data";

   string fileName1 = "firstPersondata";
   string fileName2 = "secondPersondata";

   string AbsPathA = Path.Combine(folderName.CreatFolder(), $"{fileName1}.xml");
   string AbsPathB = Path.Combine(folderName.CreatFolder(), $"{fileName2}.json");
   var dataA = File.ReadAllText(AbsPathA);
   var dataB = File.ReadAllText(AbsPathB);

   var result1 = dataA.XmlParse<Person>();
   var result2 = dataB.JsonParse<Person>();
```
- Regular operation
```csharp
   string sourceA = "[1]wkhdkjhk[a][F3]https:awijdioj.mp3fwafw";
   string sourceB = "awdhttps://aiowdjoajfo.comawd&*(&d)*dhttps://tn.comdawd";
   
   var resultA = sourceA.CaptureBetween("https:", ".mp3");

   var resultB = sourceB.CaptureLike("https://", "com");
```
- Password strength
```csharp
   string password = "12345678";
   int Level = password.CheckPasswordStrength(MinLength=8);
```

---
---

# UserControls
- ## ☆ Using
  - C#
    ```csharp
    using MinimalisticWPF;
    ```
  - XAML
    ```xml
    xmlns:mn="clr-namespace:MinimalisticWPF;assembly=MinimalisticWPF"
    ```
- ## ☆ MButton
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLPyQ.png)
  ## Property
  - Click
  - WiseHeight
  - WiseWidth
  - Text
  - TextBrush
  - FontSizeRatio
  - EdgeBrush
  - EdgeThickness
  - HoverBrush
  - CornerRadius
---
- ## ☆ MTopBar
  ![pAmMfv8.md.png](https://s21.ax1x.com/2024/09/09/pAmMfv8.md.png)
  ![pAmQnVH.md.png](https://s21.ax1x.com/2024/09/09/pAmQnVH.md.png)
  ## Property
  - WiseHeight
  - WiseWidth
  - Title
  - SizeRatio
  - EdgeBrush
  - EdgeThickness
  - HoverBrush
  - CornerRadius
  - Icon
---
- ## ☆ MPasswordBox
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLEoq.png)
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLQOJ.png)
  ## Property
  - WiseHeight
  - WiseWidth
  - FontSizeRatio
  - Password
  - Replace
---
- ### ☆ MProgressBar
  ![Effect](https://s21.ax1x.com/2024/09/09/pAeLkes.png)
  ## Property
  - Size
  - Value
  - Shape
  - Thickness
  - BottomBrush
  - FillBrush
  - TextBrush
  - FontSizeRatio
  - IsReverse
  - StartAngle
  - EndAngle
---
- ### ☆ Notification
  [![pAKM40S.png](https://s21.ax1x.com/2024/09/18/pAKM40S.png)](https://imgse.com/i/pAKM40S)
  [![pAKMhm8.png](https://s21.ax1x.com/2024/09/18/pAKMhm8.png)](https://imgse.com/i/pAKMhm8)
  ## 消息框调用示例
  ```csharp
            if (Notification.Select("Are you sure you want to check the weather ?"))
            {
                Notification.Message("Weather");
            }
  ```

---
---

# WebServices
- ##  Autonavi
  - using
    ```csharp
    using MinimalisticWPF.GaoDeServices;
    ```
  - Get ApiKey https://console.amap.com/dev/key/app
    ```csharp
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            GaoDeAPISet.Awake(Key: "Your_Api_Key", IsUpdateIP: true);
        }
    ```
  - IPService
    - Get IP
      ```csharp
      var ip = await IPService.GetIP();
      MessageBox.Show(ip.GetCombined());
      ```
    - Get AdCode
      ```csharp
      var adcode = await IPService.GetAdCode("都江堰");
      MessageBox.Show(adcode);
      ```
  - WeatherService
    - Get the weather based on the current IP address
      ```csharp
      var weathers = await WeatherService.GetWeathers();
      MessageBox.Show(weathers[0].GetCombined());
      ```
    - Get the weather by region name
      ```csharp
      var weathers = await WeatherService.GetWeathers("都江堰");
      MessageBox.Show(weathers[0].GetCombined());
      ```
    - weather[0] Is for today's weather
 
</details>