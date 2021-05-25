# eu4badapple
EU4 Animator
Contains: A mod for provinces and history files used to animate Bad Apple
C# code for a program that generates the province and history files
A savegame that can be used to animate Bad Apple

How to install the mod & run:
Install it as you would any other mod. Then run the save game I provided and do a timelapse.
Program.cs explanation:
CreateProvinces creates a eu4 provinces.bmp file. 
CreateDefinitions creates a definition.csv file from the bmp created.
CreateHistory creates history files for the timelapse according to the images provided, in this case Bad Apple.
ScaleFrames is used to resize the frames I extracted from the original video.

