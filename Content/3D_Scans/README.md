# Scan yourself into the game
## Using Kinect v2, Win 10, Meshlab and Unity

### Scanning with Microsoft 3D Scan
- Best results with a fixed Kinect v2 on a tripod
- Resolution 2/3
- Duration 30s

### First steps in 3D Builder
- repair
- split mesh to remove unwanted body parts
- simplify to reduce mesh size (80% seems to be a good value)
- smooth mesh
- move mesh to center of system
- save as *.3mf and *.ply

### Convert in Meshlab
- import *.ply file
- convert vertex color points to a texture for use in Unity
    - Filters -> Normals, Curvatures and Orientation -> Computer Face Normals
    - Filters -> Texture -> Parametrization: Trivial Per-Triangle (Texture Dimension 4096, Method Basic)
    - Filters -> Texture -> Vertex Color To Texture
- Export mesh as Collada (*.dae)

### Import in Unity
- move *.dae and *.png to asset folder
- assign *.png as Albedo map of the material
- mesh has to be scaled down to factor 0.0035 to be useful with the QR codes of business cards