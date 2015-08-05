# DigitalRune Notes

This MonoGame version is used by the [DigitalRune Engine](http://www.digitalrune.com/). 

This fork contains minor changes compared to the [official MonoGame repository](https://github.com/mono/MonoGame).
It is planned to keep this fork close to the original MonoGame repository. Changes and bugfixes are submitted back to the MonoGame team to be included in the        original MonoGame version. 

Important differences: 

* At the moment we do not use Protobuild to create the project files automatically. The modified project files are in the repository. Do not call Protobuild to override these changes. 
* The MonoGame assemblies are signed with a strong name. The strong name key file is included in the repository: DigitalRuneMonoGame.snk. <br/>
We know that it is not recommended to share a strong name key file publicly. However, the MonoGame repositories must have a strong name because many DigitalRune users need strong-named assemblies. At the same time users should be able to modify MonoGame when needed. 


----------


# [MonoGame](http://monogame.net/) [![Build Status](http://teamcity.monogame.net/app/rest/builds/buildType:MonoGame_DevelopWin/statusIcon)](http://teamcity.monogame.net/project.html?projectId=MonoGame&guest=1)

[MonoGame](http://monogame.net) is an open source implementation of the Microsoft XNA 4.x Framework.

Our goal is to make it easy for XNA developers to create cross-platform games with extremely high code reuse.

## Supported Platforms

* iOS (including Retina displays)
* Android
* Windows (OpenGL & DirectX)
* Mac OS X
* Linux
* [Windows Store Apps](http://dev.windows.com) (for Windows 8 and Windows RT)
* [Windows Phone 8](http://dev.windowsphone.com)
* [Windows Phone 8.1](http://dev.windows.com)
* [Windows 10 UWP](http://dev.windows.com)
* [OUYA](http://ouya.tv), an Android-based gaming console

## Quick Start

There are a few pre-requisites that you should be aware of for the various platforms. Please check them over [MonoGame](http://monogame.net/downloads) website.

There are four quick start options available:
* Download the current stable release from [our website](http://monogame.net/downloads).
* Download the latest "unstable" installer from [our build server](http://teamcity.monogame.net/viewLog.html?buildTypeId=MonoGame_DevelopWin&buildId=lastSuccessful&tab=artifacts&buildBranch=%3Cdefault%3E&guest=1).
* Download the latest [source code](https://github.com/mono/MonoGame/archive/develop.zip).
* Fork and clone the repo: `https://github.com/mono/MonoGame.git`.

### Solutions & Projects

The solution and project files are generated by [Protobuild](https://github.com/hach-que/Protobuild) when you double-click `Protobuild.exe` on Windows or run `mono Protobuild.exe` under Mac OS or Linux.  To modify the projects you must edit the .definition file in the `Build/Projects/` folder and re-execute Protobuild.

For more information and advanced usage, please refer to the [Protobuild documentation](https://protobuild.readthedocs.org/).

### Samples

Once you have MonoGame, grab the [MonoGame StarterKits](https://github.com/kungfubanana/MonoGame-StarterKits) or [MonoGame Samples](https://github.com/Mono-Game/MonoGame.Samples) to help get you started on your first project.

## Bug Tracker

Have a bug or a feature request? [Please open a new issue](https://github.com/mono/MonoGame/issues). Before opening any issue, please search for existing issues and read the [Issue Guidelines](https://github.com/necolas/issue-guidelines).

## Community

Keep track of development and community news.

* Follow [@MonoGameTeam on Twitter](https://twitter.com/monogameteam).
* Have a question that's not a feature request or bug report? [Ask on our community site.](http://community.monogame.net)
* Join us over IRC - [irc://irc.gnome.org/#monogame](http://mibbit.com/?server=irc.gnome.org&channel=%23monogame).

## Contributing

Please read our [contributing](https://github.com/mono/MonoGame/blob/develop/CONTRIBUTING.md) guide.

## License

MonoGame is released under [Microsoft Public License (Ms-PL)](https://github.com/mono/MonoGame/blob/develop/LICENSE.txt).

## Current Roadmap

* The MonoGame 2.x series is now in maintenance mode.
* MonoGame 3.0 will mark the start of full support for the entire XNA API.
* The Content Pipeline is currently under development. This will allow compiling of assets to optimized formats for the MonoGame supported platforms in Visual Studio or MonoDevelop without requiring XNA Game Studio 4.0 installed.
* SuperGiant Games have a version working in Google Chrome Native Client which we will merge back into our tree.
* Raspberry Pi has been shown to be working, but still needs some work.
