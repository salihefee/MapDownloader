# MapDownloaderCore
 Automatic map downloader for the rhythm game osu!
# Instructions
 Download the exe from the releases tab, and run it as administrator. Click OK and choose your current default browser's executable. And on the next step, choose your osu! folder. Lastly, go to your PC's settings and change your default browser to MapDownloader. And now, the next time you click on a map while playing multiplayer, the map will install automatically.
# How does it work?
 The program registers itself as a browser, so when it's set as the default browser and the user clicks on a link, the link gets passed to the program as a command line argument. The program checks if the argument is an osu beatmap link or not, and downloads the map if it is.
