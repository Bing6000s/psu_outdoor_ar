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
 

10/30/24
Merge request to main. 

Further tested the abnormal behavior of GET SEARCH FUZZY, fixed some of the parameters. 
Fixed the merged to main.


11/7/24
Getting hands on Google Geospatial API. Google has a demo that scene, and I've been playing with the demo scene
for now. You can place objects in Unity Editor, and anchoring it usign lontitude, latitude and altitude. In the app,
you can also tap with your finger to anchor a few markers on the ground, with the direction. 

Noticeable behaivor - Mapping a car model next to my door: 
1. Mapping an object strait to the map is slightly off. You can see this in the 5 minutes demo that it's a few buildings apart from where it's really located.
2. Mapping an object using the Geospatial mapping is really off. I mapped the object using geospatial and altitude 
to my apartment, but the thing looks like it's over to TJmax's direciton, I have drive about 2 minutes and it's not 
worth it to look where it landed at.
