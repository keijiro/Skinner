Skinner
=======

![gif](http://68.media.tumblr.com/6c5f9ef37b27048e406baf00c7ddd5d1/tumblr_oir3z03Vaf1qio469o3_320.gif)
![gif](http://68.media.tumblr.com/c0b9c3d6104449b5025132ef825eed01/tumblr_ohidp0DLyk1qio469o1_320.gif)

*Skinner* is a collection of special effects that use vertices of an animating
skinned mesh as emitting points. It uses a special [replacement shader]
[ShaderRep] to convert vertex positions into GPU-friendly data, and thus it
avoids spending extra memory and CPU time for handling them (uses GPU
resources instead).

[ShaderRep]: https://docs.unity3d.com/Manual/SL-ShaderReplacement.html

Skinner Asset Types
-------------------

*Skinner* provides some special asset types to preprocess relevant data.

### Skinner Model

A *Skinner model* is a simplified variant of a mesh asset that only has
vertices and skin weights. All topological data of triangles are removed, and
also overlapped vertices are stripped out when converted from an original mesh.

### Skinner Template

A *Skinner template* is a pre-built mesh asset that provides vertices and
topological data to the effect renderers. For instance, a *Skinner particle
template* has thousands of particle instances that are placed at the world
origin; A *Skinner particle renderer* will move them at run time.

Skinner Component Classes
-------------------------

*Skinner* also provides some component classes to handle these asset during
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

### Convert a skinned mesh into a skinner model.

Skinned mesh have to be converted into a *Skinner model* in advance to be
used in the *Skinner* system. This can be done from the context menu; click a
mesh asset to select, then choose *Skinner* -> *Convert Mesh* from the right
click menu.

Note that the mesh asset is usually located inside a fbx file. A few extra
clicks are needed to select it. See the GIF below.

![gif](http://i.giphy.com/26FLakB0pQ9nCxKY8.gif)

### Set up a character as usual.

Drag and drop a character prefab to the scene.

### Attach a *Skinner source* to a skinned mesh renderer.

Add a *Skinner source* component to the game object that has a skinned mesh
renderer component. Then set the *Skinner model* converted in the previous step
to the *model* property.  

![screenshot](http://i.imgur.com/sbBQROv.png)

This *Skinner source* will override the skinned mesh renderer and then use it to
convert vertex data. **Note that this character will disappear from the scene,**
because it will be exclusively used for vertex conversion. If it has to keep
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

Currently the *Skinner* package provides four types of renderers.

### Skinner Debug

The *Skinner Debug* renderer simply visualizes vertex data provided from a
source.

This component doesn't need a template asset.

### Skinner Glitch

The *Skinner Glitch* renderer draws triangles using randomly choosen vertices
in a source. The number of triangles is fixed (21,845 triangles), but triangles
with long edge or large area will be culled out. This behavior can be controlled
by threshold properties.

This component doesn't need a template asset.

### Skinner Particle

The *Skinner Particle* is a particle system that emits particles from vertices
in a source. The several parameters (duration, rotation, etc) of individual
particles can be changed using speed of vertices, and thus it can give some
emphasis to a character animation.

The shapes of the particles are given from a *Skinner Particle Template* asset.
Any shape can be used in the template, but it's recommended to use meshes with
very low poly count, because the number of particle instance is determined
from the number of vertices in the shapes (low poly == more particles!). 

### Skinner Trail

The *Skinner Trail* renderer draws trail lines from vertices in a source. The
width of the lines can be changed using speed of vertices, and thus it can give
some more emphasis to a character animation.

The length of the trail lines is pre-determined in the *Skinner Trail Template*
asset. The number of lines is also pre-determined by its length. The longer the
lines, the fewer the lines are drawn.

Compatibility
-------------

At the moment *Skinner* is only tested on Windows, macOS and iOS (metal).

License
-------

MIT.

[//]: # (9012345678901234567890123456789012345678901234567890123456789012345678)
