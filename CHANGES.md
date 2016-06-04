# Changes

This document contains a summary of the changes in this MonoGame fork.

The main reasons for changes were:

* Add support for PCL (Portable Class Libraries). (This was before MonoGame provided official PCL builds!)
* Add support for strong-named DLLs. (Not used in open source version of DigitalRune.)
* Experimental features.
* Minor bugfixes.

We did not yet have the time to contribute these changes back to the official MonoGame repository.
We hope this collection of notes help you make sense of the changes.


## General changes

* Submodules, .gitmodules: ThirdParty/Dependencies references our own MonoGame.Dependencies repo.
* .gitignore: Minor changes.
* Added DigitalRuneMonoGame.snk.
* README.md contains short note about DigitalRune.
* CHANGES.md added (= this file).

## Protobuild
* Changed XSLT file to support strong-name signing. (Commented out for open source version of DR.)
* Minor other changes.

## MonoGame.Framework

* Protobuild .definition files.
    * General changes
        * Added strong names. (Commented out for open source version of DR.)
        * Added OcclusionQuery in all platforms.
        * Added MessageBox and KeyboardInput in all platforms.
        * MGCB building for x64 instead of AnyCPU
        * Minor other changes.
    * Added PCL projects.
    * Mac OS: Using strong-named MonoMac.dll.
* Changes for PCL builds:
    * Added Portable.cs (with NotImplementedException).
    * Added many #ifs with symbol PORTABLE.
    * Added more .Null.cs files added: GamePlatform.Null.cs, SoundEffect.Null.cs, 
SoundEffectInstance.Null.cs, GraphicsDevice.Null.cs, RenderTarget2D.Null.cs, RenderTarget3D.Null.cs,
SamplerStateCollection.Null.cs, Texture.Null.cs, Texture2D.Null.cs, Texture3D.Null.cs, 
TextureCollection.Null.cs, TextureCube.Null.cs, ConstantBuffer.Null.cs, Shader.Null.cs, 
BlendState.Null.cs, DepthStencilState.Null.cs, RasterizerState.Null.cs, IndexBuffer.Null.cs, 
VertexBuffer.Null.cs, KeyboardInput.Null.cs, MessageBox.Null.cs, MediaPlayer.Null.cs, Song.Null.cs, 
Video.Null.cs, VideoPlayer.Null.cs
* Audio
    * Fixed AccessViolationException in X3DAudio.Calculate. 
      See SoundEffect.XAudio.cs and SoundEffectInstance.XAudio.cs
* Input
    * Touch also sets mouse input. 
      See Android/Input/Touch/AndroidTouchEventManager.cs, 
      iOS/iOSGameView_Touch.cs, Windows8/InputEvents.cs, WindowsPhone/XamlGame.cs, Windows8/InputEvents.cs
    * Fixed: Mouse position has to be converted from client coords to backbuffer coords.
    * Added support for ALT key.
    * Themes/generic.xaml: Set property MaxHeight of InputDialog.
    *  Input/KeyboardInput.cs: Removed use of async keyword.
    * Input/MessagBox.cs: Removed use of async keyword.
    * Using inputSink and forms handle in raw keyboard device.
This is necessary to receive input events in interop situations. See Windows/WinFormsGameWindow.cs.
    * Added Mouse.IsRelative, Mouse.DeltaX/DeltaY for relative mouse input. See Input/Mouse.cs, 
Input/MouseState.cs.
    * Fixed exception in Mouse.SetPosition on MacOS. See Input/Mouse.cs.

* Graphics
    * GraphicsDeviceManager.cs: Fix: GraphicsDeviceManager.ResetClientBounds() should also reset scissor rectangle.
    * SharedGraphicsDeviceManager.cs: Null implementations for device events (e.g. DeviceCreated)!?
    * Desktop/OpenTKWindow.cs:
        * Resetting scissor rect in OnResize.
        * Changed default window position.
    * Added GraphicsAdapater.UseDebugDevice. See Graphics/GraphicsAdapter.cs, 
Graphics/GraphicsDevice.DirectX.cs, WindowsPhone/DrawingSurfaceUpdateHandler.cs
    * Added GraphicsDevice.BlendFactor (not yet implemented). See Graphics/GraphicsDevice.cs.
    * Added support for PointList. See Graphics/GraphicsDevice.cs, Graphics/GraphicsDevice.DirectX.cs,
Graphics/PrimitiveType.cs.
    * Added Texture.Handle: See Graphics/Texture.DirectX.cs.
    * Added Texture2D.SaveAsImage: See Graphics/Texture2D.DirectX.cs.
    * Fix: Vertex textures must not be used in WP8.1 emulator. See Graphics/GraphicsDevice.DirectX.cs.
This is a temporary solutions. See https://github.com/mono/MonoGame/issues/3828. 
    * Added SwapChainRenderTarget for UWP.
	    Note: At the moment we cannot use the same code for Windows and UWP because UWP needs SwapChain1 and SharpDX 3.0. MonoGame for Windows (desktop) still uses SharpDX 2.6.3 which does not contain SwapChain1!
    *  SwapChainRenderTarget can be resized.
    *  Exception message may list SV_Position, instead of POSITION. See InputLayoutCache.cs.

* Content
    * Made types public which are needed in MonoGame.Framework.Content.Pipeline.
      This is necessary because the MonoGame DLL is strong name signed, the content pipeline DLL is not strong name signed (too unsigned many dependencies). InternalsInvisibleTo does not work in this case.
      See Content/ContentExtensions.cs, Graphics/GraphicsExtensions.cs, Utilities/ReflectionHelper.cs,
      Utilities/Hash.cs
    * Add comment for derivation in MeshHelper.TransformScene.
    * Content/ContentManager: Improved exception message.

* Other
    * Properties/AssemblyInfo.cs: InternalsVisibleTo changed to work with strong names.
    * Windows/WinFormsGameWindow.cs:  Changed start position of game window.
    * Windows8/MetroGameWindow.cs: Fixed: SetCursor() code cannot be executed in WP81.

* MonoGame.Framework.Content.Pipeline
    * DdsLoader.cs: Fix for importing dds texture. (output was only set for "complex" textures).
    * Graphics/MeshHelper.cs: Better debug assert message.
    * Graphics/PixelBitmapContent.cs Support for RG32 (see also TextureImporter.cs)
    *  Graphics/VertexChannelCollection.cs: Fixed: VertexChannelCollection.Insert throws AmbiguousMatchException.
    * Graphics/VertexContent.cs: Bug in RemoveRange
    * Serialization/Compiler/ContentWriter.cs: Added comments.
    * TextureImporter.cs
        * Fixed wrong usage of pitch
        * Support for FREE_IMAGE_TYPE.FIT_UINT16 images. (e.g. height fields) (see also pixelbitmapcontent)
        * Fix: Throwing meaningful ContentLoadException instead of NullReferenceException in TextureImporter.
    * OpenAssetImporter
        * Fix: ArgumentNullException when a model material has a texture but the texture path is not set.
        * Fix: Typo in exception message.
        * Fix: AnimationContent.Duration must not be rounded to milliseconds.
    * Builder/Pipelinemanager: Removing "Skipping" messages.

* Tools/MGCB
    * BuildContent.cs: Fix: Searching for assembly references in more paths.

* Tools/Pipeline
    * Common/PipelineController.cs: Added search paths for "x86"/"x64" in addition to "AnyCPU".
    * PipelineManager: Removing "Skipping" messages.


## MonoGame.Dependencies

The commercial release of the DigitalRune Engine used strong-named DLLs. Therefore, we had to fork
[MonoGame.Dependencies](https://github.com/DigitalRune/MonoGame.Dependencies). 

Following dependencies had to be changed to support strong names:

* In folder /SharpDX/Windows Phone/*: replaced DLLs with manually built and strong-named DLLs.
* In folder /SharpDX/Windows 8 Metro: replaced DLLs with official strong-named DLLs.
* In folder /SharpDX/Windows: replaced DLLs with official strong-named DLLs.
* /MonoMac.dll + .mdb was removed. Folder /MonoMac was added containing strong-named MonoMac.dll 
built from source.
* Added strong name to NVorbis DLL using a strong nameing tool.

Please note, strong names were disabled in the open source version of the DigitalRune Engine, and 
these changes are only relevant if you want re-enable strong names! 




