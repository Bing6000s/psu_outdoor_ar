# Pennsylvania University Capstone Project - Outdoor Augmented Reality for People with Disabilities

Capstone project for Outdoor AR team, fall of 2024.
End goals:
1. Save a point of interest within the application.
2. Choose from points of interest to navigate to.
3. Navigate to that point of interest using GPS, avoiding obstacles

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
