Skinner
=======

![gif](http://i.imgur.com/k838bjc.gif)
![gif](http://i.imgur.com/Elfd8QE.gif)

*Skinner* is a collection of special effects that use vertices of an animating
skinned mesh as emitting points. It uses a special [replacement shader] to
convert vertex positions into GPU-friendly data, and thus it avoids spending
extra memory and CPU time for handling them (uses GPU resources instead).

Skinner Asset Types
-------------------

*Skinner* provides some special asset types to preprocess relevant data.

### Skinner Model

A *Skinner model* is a simplified variant of a mesh asset that only has
vertices and skin weights. When converted from an original mesh, all the
topological data of triangles is removed and overlapped vertices are
stripped out.

### Skinner Template

A *Skinner template* is a pre-built mesh asset that provides vertices and
topological data to the effect renderers. For instance, a *Skinner particle
template* has thousands of particle instances that are placed at the world
origin; A *Skinner particle renderer* will move them at run time.

Skinner Component Classes
-------------------------

*Skinner* also provides some component classes to handle these assets during
run time.

### Skinner Source

The *Skinner source* component is a subsystem that converts a deformed skinned
mesh into baked data. This data will be provided to multiple *Skinner
renderers*.

### Skinner Renderers

The *Skinner renderer* components are special types of mesh renderers that
deform a *Skinner template* based on data provided from a *Skinner source*.
Then it creates some interesting special effects.

How to Set Up
-------------

### Install the package.

Download one of the unitypackage files from the [Releases] page and import it
into a project.

### Convert a skinned mesh into a Skinner model.

A skinned mesh has to be converted into a *Skinner model* in advance to be
used in the *Skinner* system. This can be done from the context menu; click a
mesh asset to select it, then choose *Skinner* -> *Convert Mesh* from the right
click menu.

Note that the mesh asset is usually located inside an fbx file. A few extra
clicks are needed to select it. See the GIF below.

![gif](http://i.giphy.com/26FLakB0pQ9nCxKY8.gif)

### Set up a character as usual.

Drag and drop a character prefab into the scene.

### Attach a *Skinner source* to a skinned mesh renderer.

Add a *Skinner source* component to the game object that has a skinned mesh
renderer component. Then set the *Skinner model* converted in the previous step
to the *model* property.  

![screenshot](http://i.imgur.com/sbBQROv.png)

This *Skinner source* will override the skinned mesh renderer and then use it to
convert vertex data. **Note that this character will disappear from the scene,**
because it will be exclusively used for vertex conversion. If it has to stay
visible, another instance of the same character should be added to the scene as
a substitution.

### Create a *Skinner renderer* object.

Create an empty game object and add one of the *Skinner renderer* components to
it. Then set the *source* property in it to refer to the *Skinner source* object
created in the previous step.

For starters, it's recommended to use the *Skinner debug* component that simply
visualizes the vertex data provided from the source. If it shows nothing, there
may be something wrong in the previous steps.

Skinner Renderer Components
---------------------------

Currently, the *Skinner* package provides four types of renderers.

### Skinner Debug

![gif](http://68.media.tumblr.com/4272c0ca532b5081125e0a1b8c63cfe1/tumblr_oio9gyLsjC1qio469o1_320.gif)

The *Skinner debug* renderer simply visualizes vertex data provided from a
source.

This component doesn't need a template asset.

### Skinner Glitch

![gif](http://68.media.tumblr.com/74a888fdc96661fee217808fa250e33e/tumblr_ohgpbnM7ce1qio469o2_320.gif)

The *Skinner glitch* renderer draws triangles between randomly choosing vertices
in a source. Although the number of triangles is fixed (21,845 triangles), triangles
with long edges or a large area will be pulled out to maintain the silhouette. This behavior
can be controlled by the threshold properties of the component.

This component doesn't need a template asset.

### Skinner Particle

![gif](http://68.media.tumblr.com/c4c573ccfcf50011cdff66e3c7106a69/tumblr_oiup1kbJCp1qio469o2_320.gif)

The *Skinner particle* is a particle system that emits particles from vertices
in a source. Several parameters (duration, rotation, etc) of each particle
can be changed according to the speed of vertices, and thus it can be used to
give some emphasis to character movement and trajectory.

The shapes of particles are defined in a *Skinner particle template* asset.
Any shape can be used in a template, but it's recommended to use meshes with
the very low poly count because the number of particle instances is determined
from the number of vertices in the shapes (low poly == more particles!). 

### Skinner Trail

![gif](http://68.media.tumblr.com/712809e81cda209c86e9744ca54ea3d9/tumblr_oir3z03Vaf1qio469o2_320.gif)

The *Skinner trail* renderer draws trail lines from vertices in a source. The
width of the lines can be changed according to the speed of vertices, and thus
it can be used to give emphasis to movement too.

The length of the trail lines is pre-determined in a *Skinner trail template*
asset. The number of lines is also pre-determined by its length. The longer the
lines are, the fewer lines are drawn.

Compatibility
-------------

At the moment *Skinner* is only tested on Windows, macOS and iOS (metal).
Possibly it runs on PS4 and Xboxone, but not sure about GLES3 and WebGL.

License
-------

[MIT](LICENSE.md)

[replacement shader]: https://docs.unity3d.com/Manual/SL-ShaderReplacement.html
[Releases]: https://github.com/keijiro/Skinner/releases
