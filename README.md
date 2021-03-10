# NavigationMeshShowcase
An example project showcasing the usage of the [NavigationMesh](https://github.com/pikol93/NavigationMesh) library.

Usage
--------------------
This example project was tested on the 3.2.3.stable.mono version of the [Godot Engine](https://godotengine.org/).
To open the project, add it to the Godot Engine's project list and run it. You will be presented with an example scene consisting of several StaticBody2D nodes that represent the obstacles that the NavMesh will try to avoid. You can adjust the properties of the Scene instance such as the agent's size or the amount of agents that are added to the scene during startup, right after the generation of the navigation mesh.

#### Creating the NavMesh
The NavMesh can be created by calling the `NavMeshFactory.Create()` method. It is required to provide the NavMesh bounds and the vertices of the obstacle shapes along with the agent size. The bounds vertices are to be ordered in a counter-clockwise way, while the obstacle shapes' vertices are to be ordered clockwise.
```
// As the NavMeshFactory class is refered to directly, it's important to mention that
// the Vector2 type is in fact the System.Numerics.Vector2 type in this snippet.

// Bounds are ordered counter-clockwise
Vector2[] bounds = new Vector2[]
{
  new Vector2(0f, 0f),
  new Vector2(0f, 100f),
  new Vector2(100f, 100f),
  new Vector2(100f, 0f),
}

// Each shape in the array is a different obstacle. All of them have to be ordered clockwise.
// Any shape containing less than 3 vertices will be ignored.
Vector2[][] shapes = new Vector2[][]
{
  new Vector2[] { new Vector2(20f, 20f), new Vector2(40f, 20f), new Vector2(40f, 40f), new Vector2(20f, 40f) },
  new Vector2[] { new Vector2(60f, 60f), new Vector2(80f, 60f), new Vector2(60f, 80f) },
}
// agentSize is the distance radius of the agent's collision shape 
// (or the distance to the farthest vertex from the agent's center)
double agentSize = 5.0;

// Create() returns a NavMesh object that's ready to use for pathfinding.
NavMeshFactory.Create(bounds, shapes, agentSize)
```

#### Finding a path
```
// Let Agent be a class containing a Position property of type Vector2
// agent is an instance of the Agent class

// navMesh is a NavMesh object that has been created earlier with the NavMeshFactory.Create() method

// targetPosition is a Vector2, representing the point in space that the agent must travel to

// To get a path, all we need to do is call FindPath() on the NavMesh object
List<Vector2> path = navMesh.FindPath(agent.Position, targetPosition, true);
```

Notes
--------------------
* This project uses a `NavigationMeshHelper` class. It is responsible for converting data between `Godot.Vector2` and `System.Numerics.Vector2`. The actual components that are present in the scene never call the NavMesh class directly, and instead call said class that first handles the data conversion and then proceedes to call the the actual NavMesh class.
* Agents only collide with the obstacles in the scene, they do not collide with each other. This is essential as the NavMesh doesn't account for dynamic objects.
* Every calculation regarding the NavMesh is done in a single thread. It is left up to the user to implement proper threading.

License
--------------------
This work is licensed under the MIT license. See the [LICENSE file](./LICENSE) for more information.
