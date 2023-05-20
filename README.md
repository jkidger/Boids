My Final Year Project for my Bachelors degree at the University of Birmingham

[Link to project report](https://jkidger.co.uk/static/Report.pdf)

# Simulating Swarms of Drones in the Unity Engine using Swarm Intelligence

## Built Executables
I have provided a built copy of each simulation in the submission, for you to run and evauluate.

2D Simulation:
~/jjk043/Boids/Build/2D Boids.exe

2D Simulation notes:
- Obstacles can be created by clicking on the play-space (click once at start and once at end position)


3D Simulation (Forest):
~/jjk043/Boids 3D/Build/Boids 3D.exe

3D Simulation notes:
- You can switch between camera views using Numpad 1-4 (Default is camera 4 - Boid Chase Camera)
- You can toggle Boid camera tracking with the spacebar
    
```
3D Simulation Camera Views (Forest):
1: "Third-Person"
2: "Front Camera"
3: "Back Camera"
4: "Boid Chase Camera"
```

These are preloaded with the final parameter sets, and cannot be adjusted in the built versions.

## Unity Projects

I have also provided both of the Unity project folders that can be opened and experimented with in the Unity Editor.

Prerequisites:
- Latest version of Unity installed via the Unity Hub

There are two projects that you can open, one simulating in 2 dimensions and the other in 3 dimensions.
As these project folders are quite large, they are located here:

< LINK REMOVED >

Boids.7z SHA512 Hash: 62626E9ED15D7B6E2CE0A651D6E43E11D996C765B05CF5FDEDA51357115732009B3782CAAE73785330A525B888930D46EEB8BF57725D08CA8D46DF1E6D4AC6B5

Boids 3D.7z SHA512 Hash: 24BE1D7ECC3E5BB7317C8A2858BC30BB8DC6AA6C653D5918144D4F20EDE098E702F27AE6C42DF81F70B64C1915EFF5B8552F4BC93F7F06E72802C7D56AF6D722


## 2D Simulation
```
To run the 2D simulation:
    1: Open the Unity Hub
    2: Select "Open>Add project from disk"
    3: Select onedrive/Boids/
    4: Open the project
    5: Click on the "Main Camera" object in the scene list
    6: Parameters can be adjusted within the Inspector Window on the right-hand side of the screen
    7: Run the simulation via the play button at the top of the screen
    8: Adjust parameters if you would like and then press "Generate Boids" to begin the simulation
```

2D Simulation notes:
- Parameters can be adjusted live (however make sure to only use positive values)
- Boids can be generated / deleted live
- Obstacles can be created by clicking on the play-space (click once at start and once at end position)
- Obstacles can be deleted live


## 3D Simulation
```
To run the 3D simulation:
    1: Open the Unity Hub
    2: Select "Open>Add project from disk"
    3: Select onedrive/Boids 3D/
    4: Open the project
    5: Choose which scene you would like to run the play space in
        5.1: Right click the desired scene and select "Load Scene" (if applicable)
        5.2: Right click the other scene and select "Unload Scene" (if applicable)
    6: Click on the "Main Camera" object in the (desired) scene list
    7: Parameters can be adjusted within the Inspector Window on the right-hand side of the screen
    8: Run the simulation via the play button at the top of the screen
    9: Adjust parameters if you would like and then press "Generate Boids" to begin the simulation
```
3D Simulation notes:
- Parameters can be adjusted live (however make sure to only use positive values)
- Boids can be generated / deleted live
- You can switch between camera views using Numpad 1-4 (Default is camera 4 - Boid Chase Camera)
- You can toggle Boid camera tracking with the spacebar
- If you would like a more interactive camera view, select the scene tab at the top of the screen (instead of game) and move the camera around with left/right/middle mouse click
    
```
3D Simulation Camera Views (Forest):
1: "Third-Person"
2: "Front Camera"
3: "Back Camera"
4: "Boid Chase Camera"
```
