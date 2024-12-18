# Pennsylvania University Capstone Project - Outdoor Augmented Reality for People with Disabilities

Capstone project for Outdoor AR team, fall of 2024.
End goals:
1. Save a point of interest within the application.
2. Choose from points of interest to navigate to.
3. Navigate to that point of interest using GPS, avoiding obstacles

#### Terms and vocabulary
        GET SEARCH FUZZY - the api that handles the conversion of query(ex: old main, penn state) to geolocation
        GET ROUTE DIRECTIONS - the api that gives route information based on two geolcoation, starting location and destination location.

#### Edit Logs 

10/24/2024
1. You don't need to switch the Starting Location variable  now.
2. Some tips: Build settings:
    1. in project, go to edit, project settings, player. Scroll down to Resolution and Presentation
    2. In Supported Aspect Ratio, click on Native Aspect Ratio
    3. In Default Orientation, click on Landscape Left. Previous team has it on Landscap Right.
    4. Go all the way up, in the Product Name. This name is the name that your app will show up as. Change to whatever  you want, I have it as AR_Bing.
3. Query to Destination  is now using fuzzy search,  with bias to the device's location. Tried to set the radius  of 5000 meters, don't know why it does not work.  
4. Camera should work now. Make sure to toggle app  permission like  this:
>settings -> security and privacy -> permission manager ->  camera -> scroll to not allowed -> app -> allow only while  using the app
 

# Jahson AR TESTING

DevLog
## 10/18
Talked to Amaan about GPSManager. Started work on NavArrow and AR system relation system. Created sphere to represent points in "AR Unity Space". Convert camera to AR camera with AR session.

## 0/22
Finished the NavArrow setup and provided it works with the GPS script. Currently working on testing AR Camera.

## 10/26
AR Anchor a point in space that a device tracks to ensure that virtual objects are accurately anchored to real-world locations. creates to origin at a point and use that point to objects. AR tracked Image Manager will create a virtual object when an image is detected. Uses teh ReferenceImageTracker to hold an array of objects that need to be detected.

### PlaceObjects
will place virtual object when they are detected by camera. However there is an issue with library and image recognition. Both script and manager was removed to prevent crashing. video referenced: https://www.youtube.com/watch?app=desktop&v=gpaq5bAjya8

### NavArrowMan
Updated to work with Geolocation and update script to work with the direction of the camera. Position is updated every frame. RotateNavArrow() has been updated to work with the new rotations.
referenced: https://www.youtube.com/watch?v=HkNVp04GOEI

## 10/28
Data got corrupted, remade everything. :( 
    3 hours spent
## 10/30/24
updated Nav arrow to work and array of points. still need to test if objects placed before start can be found.
    3 hours spent

## 11/7/24
## Files Modified:
- **NavArrowMan.cs**
- **GPSObjectPlacer.cs**
- **GPS.cs**
- **UpdateCoordinate.cs**

### Changes in `NavArrowMan.cs`
- **Added** new method `WaitForMarkers()`:
  - Ensures markers are gathered after the end of the current frame.
  - Calls `GatherAllMarkers()` to update marker list.
  
- **Implemented** `AddMarker()`:
  - Added functionality to add markers to the `targets` or `obstacles` list based on the `isObstacle` flag.
  - Instantiates a new marker at the specified position.

- **Updated** the `Start()` method:
  - Initializes the GPS system.
  - Calls `WaitForMarkers()` to gather markers at the start.

- **Modified** the `Update()` method:
  - Checks player movement and updates markers accordingly.
  - Added logic to call `FindClosestTarget()` and `FindClosestObstacle()` based on player movement.

- **Added** marker removal functionality in `RemoveMarkersNearPlayer()`:
  - Removes markers that are too close to the player (within 10 units).

- **Refined** `FindClosestTarget()` and `FindClosestObstacle()` methods:
  - Updated logic to find the closest target and obstacle using `Vector3.Distance()`.

- **Updated** `RotateNavArrow()`:
  - Adjusted the rotation of the NavArrow to point towards the closest target. Without the camera dependency. 
  - Modified the rotation speed and color based on the distance to the target.

- **Modified** `SaveData()` and `LoadData()`:
  - Allows saving and loading of player position using `GameData`.

---

### Changes in `GPSObjectPlacer.cs`
- **Added** `PlaceObjectAtGPS()` method:
  - Converts GPS coordinates to Unity world position and places a marker at the corresponding location.

- **Updated** `Start()` method:
  - Iterates through the list of GPS coordinates and places markers at each location.

- **Modified** `GPSLocationToWorld()`:
  - Converts GPS latitude and longitude into Unity world coordinates using a `scaleFactor`.

- **Linked** with `NavArrowMan`:
  - Added a reference to `NavArrowMan` and called `AddMarker()` for placing markers in the game world.

---

### Changes in `GPS.cs`
- **Refined** GPS data fetching:
  - Ensures the GPS service is started and initializes properly.
  - Requests location and compass permissions at the start.

- **Added** `CaptureInitialHeading()`:
  - Captures the initial compass heading to be used for relative heading calculations.

- **Updated** `GetRelativeHeading()`:
  - Computes the relative heading between the initial heading and the current heading.

- **Improved** the `Update()` method:
  - Continuously updates GPS coordinates and heading data.
  - Ensures that the heading and location data are used for object placement and HUD updates.

---

### Changes in `UpdateCoordinate.cs`
- **Linked** GPS data to UI:
  - Continuously updates the UI with the current GPS coordinates and distance.
  - Displays latitude, longitude, altitude, and distance on the UI.
  
- **Added** `coordinates.text` update:
  - Updates the UI text with the latest GPS data in the `Update()` method.

- **Added** a null check for `GPS.Instance`:
  - Ensures the GPS instance is available before updating the UI.
  - Displays a warning if the GPS instance is not available.

## 11/14/24
## Files Modified:
- **NavArrowMan.cs**
- **GPSObjectPlacer.cs**
- **GPS.cs**
- **UpdateCoordinate.cs**  

  8 hours spent making sure the GPS to world it right. it isn't

  ### Created `GPSEncoder.cs`
  - **Linked** with `NavArrowMan` and `GPSObjectPlacer`:
    - Added a reference to `GPSToUCS()` method for placing markers in the game world as GeoLocation.

  ### Changes in `GPSObjectPlacer.cs`
- **Updated** `PlaceObjectAtGPS()` method:
  - Converts GPS coordinates to Unity world position and places a marker at the corresponding location with new function.
- **Set up** `PlaceObstacle()` method:
  - Use GPS of detected object to place a point in Unity using `GPSEncoder.cs`

- **replaced** `GPSLocationToWorld()` with `GPSEncoder.cs` `GPSToUCS()` method:
  - current conversion failed to accuracy place points. Created new script and methods to be more precise. 

  ### Changes in `UpdateCoordinate.cs`
  - **Updated** `updateText` method:
    - When called the display text will be updated.

## 11/29/24
### Changes to build settings
  - API level from 7.0 (API level 24) to 9.0 (API level 28)
  - ARMv7 was disabled and ARM64 was enable
  - Google Console Account needed

## 12/6/24
### Changes to build settings
- 3 hours of work
- Cesium set up
- Geospatial set up
- mini world made but only part is shown
- Archors on Marks
## 12/7/24
### Changes to build settings
- 2 hours of work
- Issues with Marks appearing under the model of the area
  - API level from 9.0 (API level 28) to 7.0 (API level 24)
  - ARMv7 and ARM64 was enable
## 12/8/24
### Changes to Georeference componts
- 4 hours of work
- created a GeoReference for pollock area on Penn State Campus
- objects spawn on the map as children of ObjectHolder in hierarchy
- objects spawn at the correct height but the model of the pollock area isn't shown on run in Unity Editor.
### Changes to Cesium 3D `Tileset`
Solution found [here](https://community.cesium.com/t/3d-tile-set-not-loading-during-runtime/28936/2)
- uncheck Enable Frustum Culling
  - Solved the issue of model of pollock area not loading correct. 
  - Corrected spawn location of object. 
### Changes to Cesium 3D `NavArrowMan.cs`
- Updated Rotation
  - points at Nav Points regardless of start up facing direction, true north, and 3d model

10/30/24
Merge request to main. 

Further tested the abnormal behavior of GET SEARCH FUZZY, fixed some of the parameters. 
Fixed the merged to main.

# To next Dev:
The following is list of instruction to help in the use of scripts written by my team:
## `DataPersistence`
- This folder hold all the scripts used to save and load data. Saving and loading should be called in the `Location Manager`
    - NOTE button layout was changed from the original project. the Create button adds a `NavPoint` object at your current location and navigates towards it. 

## `AR Testing`
- This folder hold the scripts used to detect object in space through the AR camera. It doesn't hold data about GeoLocations or the model of the Earth.
    - The GeoLocations are gather from `AzureMapsGeolocation.cs` 
    - The model of the Earth comes from [Google Geospatial Creator](https://www.youtube.com/watch?v=MDcyG9MAMAo) 
        - if you encounter troubles with AR object placement I would encourage you to watch the [link here](https://www.youtube.com/watch?v=MDcyG9MAMAo)
        - Any editing to the model placement is done through the Unity inspector
            - Currently objects place through the `GPSObjectPlacer.cs` place objects at the right location so they can be seen within the Unity Editor. However objects cannot be found once build to the device. 

## `Navigation`
- This folder hold the scripts used for navigation such as GPS data handling and API calls.
    - The main scripts being used is `NavArrowMan.cs` which controls the `compass` object and `UpdateCoordinate.cs` which controls the text on the screen. 
    - Objects are placed through the `GPSObjectPlacer.cs` which controls what type of object is placed and anchors it to the model of the Earth. With the scripts there is more details about use.
        - Since `Cesium` is used the conversation between Unity coordinates to real world coordinates is no longer necessary but might still be useful. 

## crashing/lagging
- Due to the use of 3D models and Plane detection the application has the possibly of crashing or freezing
    - for the best results use the application outside with one source of light. 
    ### crashing
    - Currently crashing is prevented by disabling the `Cesium` package. follow the video posted early for details on how to enable.
    ### lagging
    - caused by:
        - Moving the camera quickly
        - Multiple sources of light (this messes with the plane detections for placing points through the device)
