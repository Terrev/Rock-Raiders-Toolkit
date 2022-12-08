# Rock-Raiders-Toolkit

This is a Unity toolkit that exports models to .LWO, and optionally .UV as well. It means you can author Rock Raiders models free of LightWave, and use proper UV mapping without trying to hassle with UView. You'll probably just want to use planar textures most of the time though, as you'll see later...

Why Unity? So I didn't have to do a bunch of model importer and UI work, and cause there's been success using Unity for these sorts of things before (The Legend of Mata Nui - I made a dumb level editor for it - and someone also made a level editor toolkit in it for Dark Souls). Also, I *might* eventually look into LWS animation support, which could use Unity's built-in animation tools. No promises if that'd even work though. We'll see.

## TUTORIAL, PART 1: SOLID COLORS + PLANAR TEXTURES

First, download Unity. https://store.unity.com/download?ref=personal

Then download the toolkit - here's a direct link to a zip of it: https://github.com/Terrev/Rock-Raiders-Toolkit/archive/master.zip

Open the project folder in Unity, and off you go. (You'll probably have a newer version of Unity than the version last used for this project - that's fine, it'll automatically upgrade it. If it DOES start giving you trouble from the upgrading process, go back and grab Unity 2019.3.0f6 or some version from soon after, and open a fresh copy of the project with that.)

Once it's open, load the scene called... uh... "Scene". This scene has some lighting settings that I find convenient for this, already set up. (Default scenes in Unity have yellowish directional lighting and blueish ambient lighting, etc, to mimic an outdoors environment. I prefer to work with plain white lighting for this.)

![image](https://user-images.githubusercontent.com/21133460/206367371-a5e841cb-5959-459b-bfbc-3802871c1222.png)

The default Unity layout will look a bit different from this - I prefer the Tall layout (Window -> Layouts) so that's what you'll be seeing here. Oh, I also prefer a one-column project tab.

![image](https://user-images.githubusercontent.com/21133460/206367416-d721545f-9c07-4155-8e30-712300a42bf5.png)

Anyway.

Let's import our custom model. Make a new folder for it in the Project tab (use the + icon), then click and drag it in.

(Note that if your model has textures, they must be imported at the same time as or before the model for Unity to assign them properly. Otherwise you'll have to manually assign them later.)

You'll also want to make a "Materials" folder inside your new folder (it can be named anything, really). Then, in the model's import settings, click "Extract Materials..." and select the Materials folder you made a moment ago.

![image](https://user-images.githubusercontent.com/21133460/206367463-29e400c6-b7b4-4488-88f5-23e7e7f42d1f.png)

This will expose all the model's materials for you to edit. Select them all, and change the shader to Rock Raiders. You can also change the shader to Rock Raiders Transparent - there's no difference between the two when it comes to saving a LWO, it only affects how they're displayed in Unity. The transparent version of the shader will just display transparency in Unity, while the other one won't.

![image](https://user-images.githubusercontent.com/21133460/206367654-80e912fa-a478-4baa-b8cf-f98d2076a8d2.png)

Now click and drag your model into the scene. The position, rotation, and scale it's at in the scene will be applied to the final exported model, so if you drag it directly into the 3D scene view, make sure you set its position to 0, 0, 0.

Most models in Rock Raiders have a basic 1x1 LEGO brick as 2.5 units wide, and a plate is 1 unit high. Some things, like minifigures, are scaled differently in their LWOs, then brought into scale with the rest of the game by other means such as their animations, so you'll have to compare to vanilla models to get the right size: https://github.com/Terrev/LWO-to-OBJ

The model I'm importing here - a custom Electric Fence - is originally built in LEGO Digital Designer and then converted/exported to OBJ: https://github.com/Terrev/3DXML-to-OBJ In LDD, a brick is 0.8 units wide, and a plate is 0.32 units high (it's based on real life measurements in millimeters). So to bring it into scale with Rock Raiders, we have to scale it up by precisely 3.125.

![image](https://user-images.githubusercontent.com/21133460/206367991-50460f7b-d3b2-4a77-acde-6ee184dafdd4.png)

Now, let's set up planar textures. This is by far the most common texturing method Rock Raiders uses. The idea is you'll project textures from either the X, Y, or Z direction, like a film projector on a screen, and you can adjust the size and position of it. These will apply per material, so that's why I gave the tiles we want to texture their own unique materials on front.

Import the textures you want to use, if you haven't already - I'm just using ones present in the vanilla game. Also assign them to the materials if needed. Don't really worry about how they look in the editor; we'll be setting up their size/position in a moment.

![image](https://user-images.githubusercontent.com/21133460/206368043-6eea51c6-a35a-406d-acfc-57bf5ca468de.png)

But what values do we want for Texture Center and Texture Size to make it look right? Go to the Rock Raiders menu up top, and click Add Planar Texture Helper. Also do Add Measuring Tape - we'll use both together. Select the things you just added in the hierarchy tab, and move them wherever.

![image](https://user-images.githubusercontent.com/21133460/206368126-78bf9794-0c0c-4bbb-8e9d-05d541f7c1ac.png)

To use the measuring tape, select an end of it, then press and hold V to use vertex snapping. Grab it by the bottom vertex of the pyramid, and snap it to a vertex on your model. Repeat for the other side, and voila! You can measure how far it is from one vertex to the other. You can also look at the Measuring Tape script on each of the ends to see the distance in case it's hard to see in the scene view, or automatically align the ends on an axis.

![image](https://user-images.githubusercontent.com/21133460/206368170-707c366a-b4a3-491e-b155-c83e77da0d3b.png)

With a couple measuring tapes, we can see the area we want to align our texture to is 2.5 units high and 5 units across. If you remember the standard Rock Raiders scale mentioned earlier, you may not have even needed to measure it. So let's change the scale of our planar texture helper to 5 on x (width), and 2.5 on y (height), and vertex snap it to the tile.

Then, drag the material you want to set to this size/position into the Material slot in the Planar Texture Helper script, and click "Apply to material".

![image](https://user-images.githubusercontent.com/21133460/206368210-512cd290-31ef-494a-b920-964e8d328119.png)

We'll also do this to the material for the tile on the other side - we don't even have to move the planar texture helper in this case, since they're aligned on the axis we'll be projecting from anyway (z). If you look at the materials now, you'll see the values we want have been copied over. You can also change the axis the texture is projected from here - tick the box for x, y, or z. (The first one checked will apply; if none are selected it defaults to z.)

![image](https://user-images.githubusercontent.com/21133460/206368294-1eaa6335-bf4f-443a-b0e7-e4c618e692e9.png)

You won't see how the planar texture will truly look in-editor, but if you use the planar texture helper, you should know what to expect. You may also have to make rotated/flipped copies of textures sometimes to make them face the way you want, as the original developers did - that's just a limitation of planar textures.

Let's do some finishing touches, making the neon green bits transparent and luminescent.

![image](https://user-images.githubusercontent.com/21133460/206368354-022a0cc8-a779-4414-b82a-06936ec5e58e.png)

Now let's save it as an LWO from the Rock Raiders menu! It will export an LWO of whatever object you have selected, and its children. (This means you can also combine models by making them the child of the selected object; click and drag one model into another.)

If nothing happens, look at the log - chances are there's a material in the model that doesn't have a Rock Raiders shader, and it'll let you know there. The Rock Raiders shader is needed so materials can have all the info needed for the LWO.

![image](https://user-images.githubusercontent.com/21133460/206368409-44afc65e-5bc6-4177-93bf-84f55aae17d2.png)

And here's the model in-game!

![image](https://user-images.githubusercontent.com/21133460/206368459-d85bf92a-e5e7-4ad9-8c81-61f45cec05e5.png)

![image](https://user-images.githubusercontent.com/21133460/206368499-6399f2ab-7e2a-4d0d-90af-17bf48aa8c27.png)

![image](https://user-images.githubusercontent.com/21133460/206368521-91b2b6eb-84a9-4ccf-ac15-35b3001f4d8e.png)

## TUTORIAL, PART 2: UV FILES

You can also save a UV file along with your LWO. This lets you use the UV mapping from the model instead of planar textures - for example, here's Sparky's head from LEGO Racers 2 ported to Rock Raiders:

![image](https://user-images.githubusercontent.com/21133460/206368702-716626a8-0c52-45ee-b1bc-ebb0b36f0c90.png)

This method comes with various restrictions, however.

Which rolls into the next section.

## TUTORIAL, PART 3: THE FINE PRINT (WAIT THAT'S ALL FOR PART 2?)

Yeah.

So here's some quirks, limitations, and other bits of trivia you should know about:

* The color value is entirely overridden by the texture in-game, if the texture is present. In Unity the color will tint the texture, they'll be blended together - that's just because I'm not actually sure how to mimic the logic of how the game does it with shaders in Unity (if that's even possible lol).
* Be aware that the "diffuse" value simply darkens the surface as it approaches 0. This is different from a lot of other programs, where "diffuse" is what the color value is called. Go ask the LightWave folks why, I guess (though apparently they outright removed it in later versions of LightWave).
* Additive Transparency means transparency is determined by the brightness of the color, where black is entirely transparent. This is most commonly used with textures, for things like smoke particle effects - so only the lighter, colored portions of the texture are visible. It technically does work with solid colors/no texture though.
* The Transparency slider is only for use without textures - if combined with a texture, it won't look correct.
* There's another method of doing transparency in Rock Raiders, where a single specified color is treated as transparent and everything else is not (cutout transparency) - this isn't handled in the LWOs though, but instead, the texture names. File names beginning with "A###" - with # being numbers - tell the game engine the index of the color in the bmp's color table/palette to be replaced with transparency. For example, in A014_SlugEye.bmp, color 14 in the bmp's color palette is turned transparent, while in A000_rails2.bmp, it's color 0. You can typically view and edit a bmp's color table in programs like GIMP, but that's starting to get outside the scope of this tutorial.
* "Texture is a sequence" is for use with, well, sequenced textures. It simply adds the " (sequence)" text at the end of the texture path, which tells the game to animate it. For an example of how these textures should be set up, see Chief's face/head textures. Also note that sequenced textures only work if the .lwo is being directly loaded as part of a .lws animation; if the game uses the .lwo more directly (like it does for crystals, electric fences, vehicle wheels, etc) the sequences won't play.
* "Write relative texture paths" probably doesn't have any effect, but let me know if you find out that it does. It has an effect in LWS files regarding the shared folder, but I have yet to find an effect in LWO files - the game seems to check the local folder for the texture, then the shared folder, regardless. I'm including it just in case.
* Pixel Blending controls if the textures appear smoothed out, or pixelated (think Minecraft).
* If a texture is specified, but the game cannot find it, it will treat it as though it's using a solid white texture.
* Using a UV file will make every material on the model use a texture. If you haven't specified a texture for a material, but use a UV file, the game will see it as a texture it cannot find, and that material will be white in-game. (This actually happens on one of the Lava Monster legs in the vanilla game - one of the texture paths is just "null". Look at their legs as they walk and you won't be able to unsee it!) This means if you want a simple solid color on a UV mapped model, it'll still have to use a texture.
* This also means you can't use the transparency slider properly with UV mapped models, since everything requires a texture.
* If a model has no defined UV coordinates, but a UV file exported, that geometry will be given default UV coordinates of 0, 0.
* Using a UV file will force pixel blending to off; all textures on the model will be pixelated. This seems like either an oversight, bug, or something they were just okay with (the only prominent models in the vanilla game to use UV mapping are the lowest poly Rock Raider, and the Lava Monster).
* Seams/splits in UV maps will cause hard edges in the shading wherever they lie. This only applies to UV files, not planar textures. It's a pain in the ass. See the screenshot below.

![image](https://user-images.githubusercontent.com/21133460/206372652-0d4782a3-1642-4ed7-9763-1fa58879937c.png)

Why does this happen? LET ME TELL YOU ABOUT VERTEX NORMALS

Despite how the LWO and UV formats store things, at the end of the day, a vertex can only have one of each attribute - one position, one normal, one UV coordinate, etc. (Well there are such things as multiple UV sets but that's not relevant to how this works or the problem at hand pls don't nitpick I mean they're just yet another set of attributes anyway right so like it doesn't matter that they're UVs at all blarghghgh.)

So when you have to have a seam in your UVs, or a hard edge on a corner, you'll actually ultimately end up with split/duplicate vertices - identical in every way, sitting in the same place, except they might have different UV coordinates for a seam in the texture, or different normals to form a hard edge.

[Here's a really nice visual explanation I always point to.](https://i.imgur.com/ikZjE2B.png) Though in this case, the problem has more to do when this occurs with UV splits.

So what's the problem exactly? Well, the LWO format doesn't store vertex normals - the game calculates them upon loading. If vertices are split, that'll result in a visible hard edge. That's a-ok.

Except, when you add UV coordinates to the mix, suddenly you have to split some of your vertices for those texture seams. This is pretty much functionally identical to unwelding them to control the smoothing of your model.

If you do this step *after* you've already calculated your UV coordinates, you're fine. Your new split/duplicated vertices will still have normals that line up with each other, and so there won't be any visible seam. This is what the game does for planar textures - after all, planar textures are just info on how to calculate UV coordinates.

But when the game instead loads UV coordinates from a UV file, it splits vertices to accommodate them *before* calculating the normals. And so, there end up being a lot of split vertices - and thus hard edges - you didn't intend there to be, wherever there's seams in the UV map.

That's what seems to be happening anyway. There could be more weird factors to it. God knows what the game's really doing.

Have fun with that :DDD
