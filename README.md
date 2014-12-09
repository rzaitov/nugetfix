nugetfix
========
This util will update packages within `iOS/Android/WP/PCL` projects for you. You will find it convinient if you need update lots of project from command line.
This is not a replacement for Nuget.exe application. This means that util will not manage package's dependency for you. You must explicitly specify which package should be removed and which should be added.
Also `nugetfix` will not restore packages for you, it just update `csproj` file and `packages.config` properly.

instructions
------------

First of all you need understand what you want, then you need translate you wish to `nugetfix`. This means you must specify fix steps to `nugetfix` these steps than will be applied to your project.
Describe fix steps within text file (e.x. `fix.steps`) with simple syntax. `fix.steps` file should contains steps for `csproj` file and for `package.config` files. Here an example. Let's imagine you need to update `Xamarin.Forms` project to version `1.3`:  

````bash
[ios csproj]
update "Xamarin.Forms.1.3\lib\Xamarin.iOS10\Xamarin.Forms.Core.dll"
update "Xamarin.Forms.1.3\build\portable-win+net45+wp80+Xamarin.iOS10\Xamarin.Forms.targets"

[ios packages.config]
update "Xamarin.Forms" version="1.3" targetFramework="xamarinios10"
```

This file contains instuctions for updating csproj file for iOS app and instuctions for patching packages.config file which locates in csproj's file folder.
In csproj file you can fix assembly references and targets files.
In packages.config you can fix any package.
You will find explanation below.


The typical `fix.steps` file for some cross-platform project will looks like this:
````bash
[ios csproj]
# here you should put migration steps for your iPhoneApp.csproj

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

fix.steps syntax
----------------
Any step starts with either `update` or `delete` work:  
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

where get migration steps ?
---------------------------
You can migrate some project manually (via XS or VS) and then browse what were changed. Based on this you can build your own `fix.steps` file.

how to run
----------
Build the util. Create your `fix.steps` file. Run from command line:  
````bash
cd path/to/your/repository
mono path/to/NugetFix.exe paht/to/SolutionFile.sln  path/to/fix.steps
```

You can run `nugetfix` on multiple projects via `xarg` util:  
```bash
find . -name "*sln" -type f -print0 | xargs -0 -L 1 -J {} mono path/to/Nuget.exe {} path/to/fix.steps
```
