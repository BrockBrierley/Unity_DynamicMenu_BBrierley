VIVID Arts - Mountain Lake README

Initial import from Asset Store will only import the base meshes and textures that are shared between render pipelines. After initial import, navigate to the Assets\VIVID_Arts\Packages folder to find MLRT_HDRP.unitypackage, MLRT_URP.unitypackage, and MLRT_SD.unitypackage. Import the package appropriate for the render pipeline in your project.

Use HDRP with HDRP version of package for best quality. URP and built-in (SD) render pipelines are also supported, but quality will be lower due to the nature of the render pipelines.

For all render pipelines, in Player settings, set Color Space to Linear.

---HDRP---

It is recommended to use the included HD Render Pipeline assets for optimal quality and performance. Locate them in Assets\VIVID_Arts\RenderPipeline. Assign them to your project in your project's Graphics or Quality settings. Use RP_High.asset for highest quality.

You may need to manually assign the diffusion profiles located in Assets\VIVID_Arts\Mountain_Lake\Diffusion Profiles to your Render Pipeline asset.

You may need to enable Water in your Render Pipeline asset to view the lake.

---URP---

It is recommended to use the included Universal Render Pipeline.asset for optimal quality and performance. Locate VIVID_URP.asset in Assets\VIVID_Arts\RenderPipeline. Assign it to your project in your project's Graphics or Quality settings.

If using your own Universal Render Pipeline.asset, it's recommended to enable HDR and set Grading Mode to High Dynamic Range for best quality.

---Standard/Built-In---

Install Post Processing via the Package Manager. If there are issues with post processing in example scene, remove the Post-process Layer component from Camera and then add it again.

---Example Scene---

The example scene is located in Assets\VIVID_Arts\Mountain_Lake\Scenes