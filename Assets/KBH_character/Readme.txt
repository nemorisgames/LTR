Readme



Hi there and firstly thanks for your purchase and support. I intend to give this project my all,  so if you have any feedback, would like a custom texture or have any suggestions please don’t hesitate to contact me at

Kris.Hammes@googlemail.com

I have been recording some support videos explaining my work, these can be found at

http://www.youtube.com/user/KrisHammes/videos 

I will be adding to these constantly so if there is any problem that users find regularly then I will add a video with a solution here. So please do contact me with any issues as I doubt I can pre-empt them all!

I'm also happy to chat via Skype for any niggling tough issues, but please contact me via email first.


For those who don’t wish to view youtube (I get it.. I'm not the best speaker!), I will run you through the process of setting up the character for your game below (section 1). In section 2 I will go through the scripts that are attached to the character and try to summarise what each adjustable value does. 


Section 1 – Setup Inputs 

Open a new or existing project. 
Set up custom inputs. 
Edit – Project settings – input
add new input by changing axes – size to 16
Expand the lowest input (should be named Jump)
Rename the bottom inputs name to run (lower-case)
Change positive button to whatever button you wish to control in game sprinting, I suggest left shift. 
See unity documentation for list of assignable buttons (http://docs.unity3d.com/Documentation/Manual/Input.html )

Setup – Importing

Import my character assets into your project from the unity store.
Find the assets on the store
Click purchase and follow store instructions
Once purchased click download
Once downloaded click import
Choose all Files and click import again
Now all the files should unpack into your project. 
Heres a pic! (https://dl.dropboxusercontent.com/u/5999451/UnityChars/Documentation/Doc1.JPG)

Setup – Simple – Opening my example scene. 

Once the assets are imported and the inputs are setup I have an example scene you can open to have the character working quickly. 

File – Open Scene
Navigate to your project directory
Navigate to Project Directory – Assets – KBH_character – Scenes
you can open any of these to get a quick view of the third person and sidescroller controllers. 	
KBH_Char_Controller - Side Scroll – testscene – Side scroll scene with some geometry to jump on. 
KBH_Char_Controller - Side Scroll -Characteronly – Character only , no environment to play around with. Will need a plane under the character as a minimum. 
KBH_Char_Controller - ThirdPErson-Character Only – Third person controller with no environment. Will need a plane under the character as a minimum. 
KBH_Char_Controller – ThirdPErsonTestScene – Third person controller with some test geometry to play around with.

Open any scene and press play, it should work. Only error that I can really predict is the input run settings, but if that’s wrong go over the set-up inputs again to check for errors. 


Setup – More complex – setting up the character yourself. 

You may need to bring the character into your pre made scene. I don’t know of anyway to merge scenes (if you do let me know). So this is how to set up the character from scratch. 

If you don’t have a plane to walk on, create one to test. 
Game Objects – Create Other – Plane (scale plane up)
In the project view expand Assets – KBH_character – Characters – Hi poly
see pic (https://dl.dropboxusercontent.com/u/5999451/UnityChars/Documentation/Doc2.jpg_
Drag KBH_unityChar01 into the hierarchy window or straight into the scene. 
Make sure he is above the plane.
Set up the material 
Assets – KBH_character – Materials – Hipoly
Locate Char_mat
Drag that material onto the character in the scene view. 
Rename the character in the hierachy view to Character (this is needed for the idle cam)
see pic https://dl.dropboxusercontent.com/u/5999451/UnityChars/Documentation/Doc4.jpg 
Attach Scripts
In the project view expand Assets – KBH_character – Scripts
If your making a 3rd person game, open 3rd Person
If your making a sidescroll person game Open side scroller. 
From here on I will be assuming we are making a third person controller.
In Assets – KBH_character – Scripts - 3rd person find a script called “KBH_CharacterControler_States”.
Drag this script onto the Character in the hierarchy window. 
Set up the characters collision	
Click on “Character” in the hierarchy window. 
In the inspector a list of script are now attached. 
Find “Character Controller”
Change Centre to  X = 0 Y =1.05 Z=0
Change Radius to 0.2
Set up the controllers animations. 
In the project view navigate to Assets - KBH_character – Animations.
Find  the animator controller called KBH_CharControl – ThirdPerson (it should be at the top)
Click on “Character” in the hierarchy window.  
In the inspector a list of script are now attached. 
Find “Animator”
Drag the previously found file into the slot called Controllers
See pic (https://dl.dropboxusercontent.com/u/5999451/UnityChars/Documentation/Doc5.jpg)


Set up the Camera
Find Camera Scripts
Assets – KBH_character – Scripts - 3rd person 
Drag KBH_cam onto the Main Camera in the hierarchy window.
If your making a side scroll go to Assets – KBH_character – Scripts – SideScroller and use KBH_cam_sideScroll instead of KBH_cam. 
Click on KBH_cam in the hierarchy window to see camera options. 
Expand the script
Drag the “Character” from the heirachy window to the box besides Target
pic https://dl.dropboxusercontent.com/u/5999451/UnityChars/Documentation/Doc6.jpg 
Adding the Idle camera (if you want one)
Go to  Assets – KBH_character – Scripts - 3rd person 
Drag CameraSwitcher and Idle_Cam scripts onto the main camera.
Assign “Character” to Idle_cams Target like you did previously with KBH_Cam

Now if you press play you should be able to use the character. If he drops through the plane on start up just lift him off the ground slightly. If you don’t press any keys the character should play a second idle  after (default) 60 seconds. The Idle cam should then kick in after (Default) 80 seconds. 

If you cant follow these text based instructions ill be adding a video shortly when the character is on the store, so just check my channel ( http://www.youtube.com/user/KrisHammes/videos  )

There will be no lights when you set up a scene yourself, however I cant really direct on that aspect. In my standard test scene the character has a blob projector shadow, feel free to use that as your shadows if you want a simple solution. 






Section 2 – Script and Component Breakdown. 

I will go through all the options of the scripts in this section. I will stick to 3rd person however many are duplicated in the sidescroller scripts. I will Hi light any areas I feel are essential to adjust to tweak for your game. 

Character Scripts. 

Animator 
Controller – Put KBH_CharControl – ThirdPerson here.
Apply root motion should be ticked
Animate Physics I believe should be ticked. (honestly not 100% clear on that)
Culling mode, never touched. Leave to default based on renderers

Character Controller

Unity Documentation here http://docs.unity3d.com/Documentation/Components/class-CharacterController.html 

My preferred set-up

Slope Limit : 45
Step Offset : 0.1
Skin Width : 0.08
Min Move Distance : 0
Centre : X=0 Y=1.05 Z= 0
Radius : 0.2
Height : 2

Jump Booster – This script affects the height of the jumps and also the distance. See video http://youtu.be/x8jKebCBSr4

Vertical Run Multiplier (default 1) – Sets how high  the running jump will go 
Forward Run Boost (default 1) – Sets how far forward the running jump will go 
Vertical Walk Multiplier (default 1) – Sets how high the walking jump will go 
Forward Walk Boost (default 1) – Set how far forward the walking jump will go 
Run Curve -  Hard to explain. See Video http://youtu.be/Iu0VhOzyGJ8
Walk Curve – Hard to explain. See Video http://youtu.be/Iu0VhOzyGJ8
Height – None editable, shows character height to ground.
Fall status – None editable, needed by animator.
Short fall Height (default 5)– This is the height that the character need to be before the falling animation begins to play. If the character enters  an unwanted falling state often you may need to increase this number. 
Slope Gravity (default 5) – When running down a steep slope gravity is increased to “stick” the character to the surface and stop it essentially taking off. 
Falling direction (default 5) – when falling if your like the player to have control of where they land, this number controls how responsive direction controls are. 


Push Script.  This can be disabled if you don’t want or need your character to interact with rigid bodies. As Character Controllers doesn't  use physics this simulates a pushing motions on any rigid body the characters collides with. 

Push Power (default 2) – How hard the character collides with rigid bodies. 

Rotation Monitor 
	This script is needed to monitor how fast the character is turning on the spot so that the shuffling animation can blend correctly. Non editable

KBH_CharacterController_States

This is the main script that drives everything else. Most of my code is here if you open it up. Ive taught myself programming over the course of making this so honestly if a programmer open this up you may despair. 

If there’s any obvious improvement and they are easily explainable then please feel free to talk them through with me. I need to understand them though in order to provide support on it. 

Idle Timer (non editable) – timer of how long the player is inactive
Forward Multiplier (default 1) – Increases the speed of all forwards runs and walks. Be cautious of high numbers as the animations will look odd. If you set this higher consider  setting the jump boosts higher as well.
Backwards Multiplier (default 1) – Increase the speed of backward walk animations. 
Idle Threshold (default 60) – Time in seconds before the second Idle plays. 
Allow Cam Input – A check I needed a public variable to fix a camera bug. No need to change. 
 

Camera Scripts

These are more important than you may imagine as the character rotation is linked to the camera, so don’t neglect looking over these if you want to tweak the controller. 

KBH_Cam – This script is modified From  JesseEtzlers Camera , he has some great tutorials on youtube (http://www.youtube.com/user/JesseEtzler0?feature=watch )

Target – The character should be dragged into the box
Target Height (default 1.5) – How high the camera sits behind the character 
Distance (default 3.5) – How far the camera will be when the game starts. Lower number = close camera
Offset from wall (default 0.1) – The camera will collide with walls if the there is an object between the camera and character. This is how far it will move away from any object that get between the two. 
Max Distance (default 20) – This is how far the player is allowed to zoom out using mouse wheel in game. Set lower to restrict this zoom
Min Distance (default 0.6) – This is how far the player is allowed to Zoom in using the mouse wheel in game. Set higher to restrict the zoom. Note setting distance and min max zoom to the same height will effectively lock this zoom feature.
X speed (default  200) – When the player presses RMB the cam goes into mouse look mode. This is how sensitive the X (Left/Right) axis is. 
Y speed (default 200) -When the player presses RMB the cam goes into mouse look mode. This is how sensitive the Y (Up/Down) axis is. 
Y Min Limit (default -80) – When in Mouse Look mode, this controls how far under the character the camera will move. This will likely need adjusting to to fit your game. 
Y Max Limit Default 80) – When in Mouse Look mode, this controls how far above  the character the camera will move.
Zoom Rate (default 40) – This sets how sensitive the zoom is in game (when the player uses the mouse wheel to zoom)
Rotation Dampening (default 3)  - When you rotate the mouse in mouse look mode, if you release the mouse button the camera returns to the rear. This determines how quickly it does that. A low number equals a slow return.  
Zoom Dampening (Default 5) – This smooths out the mouse wheels zoom. A low number will make a slightly less responsive but smoother zoom, a high number will be more responsive but not as smooth. 
Mouse look return (default on) –  when this is turned off, if you move the camera in mouse look mode it will remain in place until you press either a horizontal axis or the mouse. I would recommend keeping this on however.  I plan to tweak this in a future update. 
Arc Turn speed (default 60) – This is important. It will control how fast the character rotates when you press the horizontal axis ( default likely set to A and D). This will be how fast the character rotates when its on the spot but also how fast the character rotates when walking or running if the player is using a keyboard key to control movement. Higher number = faster rotation. 

Idle_Cam (optional) 

If you wish for an orbiting cam if the player idles, use this. 

Target : Drag character here.
Distance (default 3) : how far the camera is 

Camera Switcher
If you intend to use the Idle_Cam script this script must also be attached. Only one important vlaue to enter here

Camera Switch time (default 80) : time in seconds the player has to remain idle before switching to idle cam. 






Thank you again for your purchase, as a reminder if you would like or need to contact me,  my email address is Kris.Hammes@googlemail.com

































