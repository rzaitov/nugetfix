Nugetfix
========
Nugetfix was initially designed to update packages in `iOS/Android/WP/PCL` projects. Use it if you need to update packages in lots of projects with a single command. This tool just updates `*.csproj` and `packages.config` files from command line. It means that it can't be used as replacement for Nuget.exe or as nuget package manager. You must explicitly declare packages you want to be removed or added.

Instructions
------------

* First of all you need to realize what packages you want to update, then you need to describe your wish to `nugetfix`. By saying this I mean that you should specify fix steps for `nugetfix`(these steps will be applied to your project).
* Write fix steps in text file (e.x. `fix.steps`) using simple syntax. `fix.steps` file should contain steps for `*.csproj` and for `package.config` files.

For instance, you want to update `Xamarin.Forms 1.2` project to utilize `Xamarin.Forms 1.3`:

````bash
[ios csproj]
update "Xamarin.Forms.1.3\lib\Xamarin.iOS10\Xamarin.Forms.Core.dll"
update "Xamarin.Forms.1.3\build\portable-win+net45+wp80+Xamarin.iOS10\Xamarin.Forms.targets"

[ios packages.config]
update "Xamarin.Forms" version="1.3" targetFramework="xamarinios10"
```

This file contains instuctions for updating csproj file of iOS app and instuctions for patching packages.config.
In csproj file it's possible to change assembly references and targets.
In packages.config you can fix any package.
You will find explanation below.


Typical `fix.steps` file for random cross-platform project will look like this:
````bash
[ios csproj]
# here you should put migration steps for your iOSApp.csproj

[ios packages.config]
# here you should put migration steps for package.config file

[android csproj]
# here you should put migration steps for your AndroidApp.csproj

[android packages.config]
# here you should put migration steps for package.config file

[wp csproj]
# here you should put migration steps for your WindowsPhoneApp.csproj

[wp packages.config]
# here you should put migration steps for package.config file
````

fix.steps outline and syntax
----------------
Every step starts with `update` or `delete` instruction:
```bash
update "path/to/new/version/assembly.dll" # specify path to new assembly version
update "path/to/new.targets"
delete "assembly" # assembly is the assembly name (file without extension)
```

The same rule for patching `packages.config` file:  
```bash
update "packageId" version="newVersionString" targetFramework="specifyFrameworkHere"  
delete "packageId"
```

Here you can browse real-life [fix.step](https://github.com/rzaitov/nugetfix/blob/master/NugetFix/fix.steps) file

How to get migration steps
--------------------------
You can migrate some project manually (via XS or VS) and then browse what was changed. Based on this you can build your own `fix.steps` file.

How to run
----------
* Build NugetFix project with XS or VS
* Create `fix.steps` file
* Run from command line
````bash
cd path/to/your/repository
mono path/to/NugetFix.exe paht/to/SolutionFile.sln  path/to/fix.steps
```

You can run `nugetfix` on multiple projects with `xarg` util:
```bash
find . -name "*sln" -type f -print0 | xargs -0 -L 1 -J {} mono path/to/Nuget.exe {} path/to/fix.steps
```
