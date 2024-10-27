## DevLog
# 10/18
Talked to Amaan about GPSManager. Started work on NavArrow and AR system relation system. Created sphere to represent points in "AR Unity Space". Convert camera to AR camera with AR session.

# 10/22
Finished the NavArrow setup and provided it works with the GPS script. Currently working on testing AR Camera.

# 10/26
AR Anchor a point in space that a device tracks to ensure that virtual objects are accurately anchored to real-world locations. creates to origin at a point and use that point to objects. 
AR tracked Image Manager will create a virtual object when an image is detected. Uses teh ReferenceImageTracker to hold an array of objects that need to be detected. 

## PlaceObjects 
will place virtual object when they are detected by camera. However there is an issue with library and image recognition. Both script and manager was removed to prevent crashing.
video referenced: https://www.youtube.com/watch?app=desktop&v=gpaq5bAjya8 

## NavArrowMan
Updated to work with Geolocation and update script to work with the direction of the camera. Position is updated every frame. RotateNavArrow() has been updated to work with the new rotations.  