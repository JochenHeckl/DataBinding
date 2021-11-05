# Unity DataBinding

## What is Unity DataBinding?

Unity DataBinding brings data binding to unity game objects. DataBinding aiming to minimize the effort required to create dynamic, performant user interfaces.

With DataBinding you will always have your application logic separated from your User interface logic.

## Getting started (Step by Step Tutorial - 5 minutes to complete)

### Installing the package

- Open Unity and create a new Unity Project.

  It does not matter which kind of Unity project you choose DataBinding works with all flavours of Unity. For simplicity we will choose 3D Core.

  ![Create Project](/Documentation~/images/CreateProject.png)

- Install the data binding package using the Unity Package Manager [Installing a package from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html) from here:
  
  >[https://github.com/JochenHeckl/DataBinding.git](https://github.com/JochenHeckl/DataBinding.git)
  
  *If you should lose track after completing this step you can simply skip to the end by loading the MyFirstDataBoundView sample*

  ![Tutorial Scene](/Documentation~/images/ImportSample.png)

### Setting up the scene

- Add a New GameObject and name it `Setup`
- Add a New GameObject and name it `View`
- Right click the View GameObject and add a 3D Cube to it.
- Select the cube and **double tap the 'f' key** to focus the cube.
- Select the Main Camera and hit **CTRL + SHIFT + 'f'** to align the camera.
  
  Your scene should look something like this:
  
  ![Tutorial Scene](/Documentation~/images/InitialSetup.png)

- Create a new c# script ***CubeViewDataSource.cs***:
  
  <details>
  <summary>Show code</summary>

  ```csharp
  using de.JochenHeckl.Unity.DataBinding;
  using UnityEngine;

  public class CubeViewDataSource : DataSourceBase<CubeViewDataSource>
  {
    public Vector3 CubeScale { get; set; } = Vector3.one;
    public Color CubeColor { get; set; } = Color.grey;
  }
  ```

  </details>

- Create a new c# script ***PlaceholderApplicationLogic.cs***:

  <details>
  <summary>Show code</summary>

  ```csharp
  using UnityEngine;

  /// <summary>
  /// This class is here as a replacement for whatever
  /// application logic your application might implement.
  /// You application might be arbitrarily complex and expose
  /// many data sources - static data sources as well as dynamic ones.
  /// This application is about changing the scale and color of a cube.
  /// That's it for this tutorial.
  /// So the sole data source exposed is a simple CubeViewDataSource.
  /// </summary>
  public class PlaceholderApplicationLogic
  {
    public CubeViewDataSource CubeViewDataSource { get; set; }
    private float _nextCubeUpdateTimeSeconds;

    public void Initialize()
    {
        CubeViewDataSource = new CubeViewDataSource();
        _nextCubeUpdateTimeSeconds = 0f;
    }

    public void Update(float simulationTimeSeconds)
    {
        if ( _nextCubeUpdateTimeSeconds < simulationTimeSeconds )
        {
            _nextCubeUpdateTimeSeconds += 3.0f;

            CubeViewDataSource.NotifyChanges(x =>
            {
                x.CubeScale = Vector3.one + Random.insideUnitSphere;
                x.CubeColor = Random.ColorHSV(0, 1, 0, 1);
            });
        }
    }
  }
  ```

  </details>

- Create a new c# script ***MyFirstDataBoundViewSetup.cs***:
  
  <details>
  <summary>Show code</summary>

  ```csharp
  using de.JochenHeckl.Unity.DataBinding;
  using UnityEngine;

  public class MyFirstDataBoundViewSetup : MonoBehaviour
  {
    public View view;
    private PlaceholderApplicationLogic _placeholderApplicationLogic;
    public void Start()
    {
        _placeholderApplicationLogic = new PlaceholderApplicationLogic();
        _placeholderApplicationLogic.Initialize();

        view.DataSource = _placeholderApplicationLogic.CubeViewDataSource;
    }

    // Update is called once per frame
    public void Update()
    {
        _placeholderApplicationLogic.Update(Time.time);
    }
  }
  ```

  </details>

- Create a new c# script ***DynamicMaterialColor.cs***:

  <details>
  <summary>Show code</summary>
  
  ```csharp
  using UnityEngine;

  public class DynamicMaterialColor : MonoBehaviour
  {
    public Material material;
    public Color Color
    {
        set
        {
            material.color = value;
        }
    }
  }

  ```

  </details>

- Create a Material for the Cube.
  From the Menu select *Assets* --> *Create* --> *Material*. Name the Material "Cube Material" and drag it onto the Cube in your scene view.

### Configuring the ***Setup*** GameObject

- Add the MyFirstDataBoundViewSetup component to the Setup GameObject.
- Link the View GameObject to the View property of MyFirstDataBoundViewSetup.

In a real world scenario we would not *hard code* this relation but generate the view from some prefab. However this approach is very handy to develop views in isolation.

![Setup](/Documentation~/images/SetupInspector.png)

### Configuring the ***View*** GameObject

- Select the View GameObject and click the Add Component button.
  Type "View" into the search bar. Add a View Component.
- Choose CubeViewDataSource as the DataSource Type for this view.
- On the View component add two component property bindings by clicking the Add Binding button twice.
- Add a DynamicMaterialColor Component.
- Drag the Cube Material onto the Material Property field of the DynamicMaterialColor Component.
  
![Setup](/Documentation~/images/ViewInspector01.png)

- Now we are set to configure the two component property data bindings:
  
  - Select the ***Source Path*** for the first binding to be ***CubeScale***
  - The ***Target GameObject*** will be the ***Cube*** GameObject.
  - For the ***Target Component*** choose ***Cube::Transform***
  - The ***Target Path*** will be ***localScale***
  - Select the ***Source Path*** for the second binding to be ***CubeColor***
  - The ***Target GameObject*** will be the ***View*** GameObject itself.
  - For the ***Target Component*** choose ***View::DynamicMaterialColor***
  - And finally the ***Target Path*** will be ***Color***

  If the component property binding is valid, it will collapse to a condensed text description. You can toggle expanded and collapsed view
  using the ```…``` and ```↸``` toggle button.

### Running the Project

- That's it! We are done. Hit play and you should see the application logic change the appearance of the cube every other second.

![Setup](/Documentation~/images/ViewInspector02.png)
