# MapDownloader
 Automatic map downloader for the rhythm game osu!
# Instructions
 Run the exe file that you downloaded from the releases page as administrator after placing it where you want it. Select the executable for your current default browser then click OK. And then select your osu! folder on the following screen. Finally, change MapDownloader to be your default browser in your computer's settings. The map will now download automatically the next time you click on a map while playing multiplayer. Keep in mind that you will need to redo the configuration by executing the executable as administrator if you ever decide to modify the executable's directory.
# How does it work?
 The program registers itself as a browser, so when it's set as the default browser and the user clicks on a link, the link gets passed to the program as a command line argument. The program checks if the argument is an osu beatmap link or not, and downloads the map if it is.
