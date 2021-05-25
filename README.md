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

Note for people interested in the modding part: My original design was to assign white pixels to a country with white color and black pixels to a black color in history files but I did something wrong and the black provinces ended up being uncolonized instead which still worked for me so I didn't try to fix it. Keep this in mind if you want to modify the project.
