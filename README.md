nugetfix
========
This util will update packages within iOS/Android/Wp/Pcl projects for you. You will find it convinient if you need update lots of project from command line.
This is not a replacement for Nuget.exe application. This means that util will not manage package's dependency for you. You must explicitly specify which package should be removed and which should be added.
Also `nugetfix` will not restore packages for you, it just update `csproj` file and `packages.config` properly.

Instructions
------------

First of all you need understand what you want, then you need translate you wish to `nugetfix`. This means you must specify fix steps to `nugetfix` these steps than will be applied to your project.
Describe fix steps within text file (e.x. `fix.steps`) with simple syntax. `fix.steps` file should contains steps for `csproj` file and for `package.config` files. Here an example. Let's imagine you need to update `Xamarin.Forms` project to version `1.3`:  

````
[ios csproj]
update "Xamarin.Forms.1.3\lib\Xamarin.iOS10\Xamarin.Forms.Core.dll"
update "Xamarin.Forms.1.3\build\portable-win+net45+wp80+Xamarin.iOS10\Xamarin.Forms.targets"

[ios packages.config]
update "Xamarin.Forms" version="1.3" targetFramework="xamarinios10"
```

The typical `fix.steps` file will looks like this:
````
[ios csproj]
// here you should put migration steps for your iPhoneApp.csproj

[ios packages.config]
// here you should put migration steps for package.config file

[android csproj]
// here you should put migration steps for your AndroidApp.csproj

[android packages.config]
// here you should put migration steps for package.config file

[wp csproj]
// here you should put migration steps for your WindowsPhoneApp.csproj

[wp packages.config]
// here you should put migration steps for package.config file


````
