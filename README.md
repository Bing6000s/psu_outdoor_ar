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
