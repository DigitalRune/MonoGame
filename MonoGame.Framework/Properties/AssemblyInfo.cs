using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MonoGame.Framework")]
#if OUYA
[assembly: AssemblyDescription("MonoGame for OUYA")]
#elif ANDROID
[assembly: AssemblyDescription("MonoGame for Android")]
#elif WINDOWS_STOREAPP
[assembly: AssemblyDescription("MonoGame for Windows Store")]
#elif DESKTOPGL
[assembly: AssemblyDescription("MonoGame for all OpenGL Desktop Platforms")]
#elif WINDOWS
[assembly: AssemblyDescription("MonoGame for Windows Desktop (DirectX)")]
#elif MAC
[assembly: AssemblyDescription("MonoGame for Mac OS X")]
#elif IOS
[assembly: AssemblyDescription("MonoGame for iOS")]
#elif WINDOWS_PHONE
[assembly: AssemblyDescription("MonoGame for Windows Phone 8")]
#elif PORTABLE
[assembly: AssemblyDescription("MonoGame Portable Class Library (PCL)")]
#endif
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("MonoGame.Framework")]
[assembly: AssemblyCopyright("Copyright © 2011-2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Mark the assembly as CLS compliant so it can be safely used in other .NET languages
[assembly:CLSCompliant(true)]

// Allow the content pipeline assembly to access 
// some of our internal helper methods that it needs.
// [DIGITALRUNE] Needs to be public. Used in MonoGame.Framework.Content.Pipeline.
//[assembly: InternalsVisibleTo("MonoGame.Framework.Content.Pipeline")]
[assembly: InternalsVisibleTo("MonoGame.Framework.Net, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b16c5e641ffa9b282535924aef3e9bb12285997689da5ede9c8e3e04f72b02f435119e0a9eb5344de9503fd2477c53fee7e9905ca2bdb78bb8c9000a936a92b0460ff5d798741d1a7a4d587fe119ad08e8011e6454d4c7a6a436055bb6cd7436277cfc13b8b6d9ebdf891d1bca1ed5609c58b9ab52c7c3893c297a77301c43c7")]

//Tests projects need access too
[assembly: InternalsVisibleTo("MonoGameTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b16c5e641ffa9b282535924aef3e9bb12285997689da5ede9c8e3e04f72b02f435119e0a9eb5344de9503fd2477c53fee7e9905ca2bdb78bb8c9000a936a92b0460ff5d798741d1a7a4d587fe119ad08e8011e6454d4c7a6a436055bb6cd7436277cfc13b8b6d9ebdf891d1bca1ed5609c58b9ab52c7c3893c297a77301c43c7")]
[assembly: InternalsVisibleTo("MonoGame.Tests.Windows, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b16c5e641ffa9b282535924aef3e9bb12285997689da5ede9c8e3e04f72b02f435119e0a9eb5344de9503fd2477c53fee7e9905ca2bdb78bb8c9000a936a92b0460ff5d798741d1a7a4d587fe119ad08e8011e6454d4c7a6a436055bb6cd7436277cfc13b8b6d9ebdf891d1bca1ed5609c58b9ab52c7c3893c297a77301c43c7")]

#if !PORTABLE
// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("81119db2-82a6-45fb-a366-63a08437b485")]
#endif

// This was needed in WinRT releases to inform the system that we
// don't need to load any language specific resources.
[assembly: NeutralResourcesLanguageAttribute("en-US")]

// Version information for the assembly which is automatically 
// set by our automated build process.
[assembly: AssemblyVersion("0.0.0.0")]
[assembly: AssemblyFileVersion("0.0.0.0")]
