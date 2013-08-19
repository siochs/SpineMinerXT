# SpineMinerXT

----------

Simon M. Ochs, 2013

##Abstract
SpineMinerXT is a graphical user interface (GUI) for Windows embedding the image overlay processor [SpineMiner](https://github.com/siochs/SpineMiner "SpineMiner"). Since [SpineMiner](https://github.com/siochs/SpineMiner "SpineMiner") runs only on command line, the GUI was created to make *SpineMining* more comfortable. The basic features are implemented as Windows Forms control elements and passed as command line arguments to [SpineMiner.exe](https://github.com/siochs/SpineMiner "SpineMiner"). If you are not aware what [SpineMiner](https://github.com/siochs/SpineMiner "SpineMiner") does, you should read [SpineMiner's](https://github.com/siochs/SpineMiner "SpineMiner") README first: [https://github.com/siochs/SpineMiner](https://github.com/siochs/SpineMiner "SpineMiner") 

##Building SpineMinerXT
SpineMinerXT was written in C# using [Microsoft's Visual Studio 2010 Ultimate](http://www.microsoft.com/visualstudio/ "Microsoft Visual Studio") IDE. It is a simple Windows Forms application. You can also build SpineMinerXT with Visual Studio 2010 Express edition which is freely available from Microsoft. 
To build SpineMinerXT, download and install Microsoft's Visual Studio and create SpineMiner from the build menu. After successfull build, the executable is located in the `bin/` directory. 

##Using SpineMinerXT
SpineMinerXT requires [Microsoft's .NET](http://www.microsoft.com/net "Microsofts .NET") Framework 4 or higher. If you don't have it installed you can download it from Microsoft's web page. 


###Running SpineMinerXT the first time
1. Place SpineMinerXT in the current working directory beside your `.ovl` files. Double click starts the Program, which extracts the embedded core application [SpineMiner.exe](https://github.com/siochs/SpineMiner "SpineMiner"). 
2. Give the group you are going to analyze a name. As default `Cohort1` is preselected. This will be the name of your database. 
3. Decide if you wish to ignore spine morphologies. If so, check the according checkbox.
4. Decide if you want filopodia be treated as if they are thin spines: If so, check the according checkbox.
5. By clicking on the scientist symbol, SpineMinerXT runs the core application until the analyis is complete. A `.sqlite` database file will be created, by default `Cohort1.sqlite`. Any errors will be reported.
6. If you wish to adapt some changes, then do so according to the control elements and click the refresh picture behind the setting. This processes the database. 
7. You can export your calculation results as Comma-separated values by clicking the truck. Your results will be stored as `.dendrites.csv`. Note: any existing file will be overwritten.

###Opening/Adapting an existing database
1. Place SpineMinerXT where your `.sqlite` database file is located.
2. Start SpineMinerXT.
3. Enter the name of your database (without the `.sqlite` filename extension, e.g. `Cohort1`).
4. Click the folder icon just next to it to open the database.
5. Adapt any changes you like by using the control elements. The refresh symbol updates the database.
6. Export your new calculations by clicking the truck. Note: any existing file will be overwritten.

##AutoUpdate
SpineMinerXT can auto update itself. For this purpose the updater class was created. On application startup the updater is initialized with a pointer to the main form, the URL where the binary and the `curr_vers.txt` is hosted, the aplication name, the current version number or a file to be deleted. The system works the following way:
 
1. The updater contacts the host and tries to read the version number from `curr_vers.txt`. The host is compiled into the application. Since SpineMinerXT is downloadable as release from GitHub, a `deploy` release folder was created which should be updated with the newest version of SpineMinerXT and `curr_vers.txt`. Therefore, the URL passed to the updater is [http://github.com/siochs/SpineMinerXT/releases/download/deploy/](http://github.com/siochs/SpineMinerXT/releases/download/deploy/ "http://github.com/siochs/SpineMinerXT/releases/download/deploy/").
2. If the version inside curr_vers.txt is greater than the current version number of the application, the new executable will be downloaded from the host. The new executable is identified by the version appendix of SpineMinerXT, e.g. `SpineMinerXT_1.25.exe`. 
3. When the download was successfull, the application starts the new SpineMinerXT with a `-d` option, telling the new application to delete the old one (`SpineMinerXT_1.25.exe` should delete `SpineMinerXT_1.24.exe`). When creation of the new process was successfull, the old application terminates itself so that it can be deleted. 
4. The new application received the `-d` option and the filename of the old application. After waiting a second, the old application name is deleted. NOTE: the updater also tries to delete SpineMiner.exe from the old application.
 
##Updating SpineMinerXT
When there will be a new release of SpineMinerXT, it will be named with the new version number appendix, e.g. `SpineMinerXT_1.25.exe`. `1.25` also has to be placed in the `curr_vers.txt` in the `releases/deploy/` folder and has to be adapted in the initialization method of the form, e.g. `const string app_version = "1.25";`.

##Known Issues
Well, if you watch the source you will agree that the whole thing is more or less hacked together to make the usage of [SpineMiner](https://github.com/siochs/SpineMiner "SpineMiner") comfortable for end users. Many control elements still have default names like "Button1" or "Label3"... 
In general I would appreciate if you have some better ideas to integrate [SpineMiner](https://github.com/siochs/SpineMiner "SpineMiner") in SpineMinerXT and piping it's output directly to the text box in SpineMinerXT. 

##Licensing
In line with the Gnu Public Licence I agree with redistribution and modification of my code as long as you do no harm.