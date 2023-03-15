# AntiSkillIssue
My Discord is 32KZ#8255. Please message me 😭 😂

## Current Features

- Takes replay information from [BeatSavior](https://github.com/Mystogan98/BeatSaviorData) Found in Appdata For Display in a UI of my Creation 🙌 the UI contains Tabs For the following: 

  - Start & End Review Sliders , Pre-Swing & Post-Swing, Accuracy, Timing Dependence, Velocity
  
  - Each tab has a nested Left and Right tab to Display information Per hand for better Data Viewage

- Allows for refreshing of sessions and plays.

- has a validity checker to Declare if the Selected BSD play is a Valid, VRmode Score. (exception cases are if there is a 0 combo, aka noodle or desktop scores.)

## Planed Features

- Save config settings in  "UserData/AntiSkillIssue/Settings.json" For future use

- Per level, Recommend most Effective values in comparason to your actual data, eg Velocity based on selected level NJS and BPM. 

## How To Use

1> Set a Score for review, or Look at a Previous Score saved by BeatSavior Data

2> Go to the ASI Menu Button under the Mods Section.

3> Select the Session in question, from the Main BSMLView, on the Left Most List.

4> Select the score from that session, by clicking a score cell on the right most List. 

5> By Clicking through the tabs on the Left View, you can look at information from that play, Like what to change to improve, and your current data. 

5> Ajust the Start time Slider and End time Slider to review a more specific time range in the song, and then click the apply button.

6> Refer to Score Graph on the right for a more Visual representation on what happened During this Range. 

## Versions
All releases for BS 1.20.0 and on


## About

Grow your PP in style! 

i Decided to make a mod for Beatsaber For my Computer Science Alevel Project. 

Great idea 🥶

## Known Issues

- Works less than optimally

- Creates new ViewControllers and FlowCoordinators each time the Menu is loaded (memory leak 💀)

- Custom Cells may or maynot Recycle class instances. 

- Clicking the dummy cell after reloading the sessions, or on initial start causes an access Denied error for the "%appdata%/Beat Savior Data/" directory. 

- Does not support Scores that have a non-Positive Cut Count, or are noodle maps.

- the larger a session, the longer it takes to populate the right list on a session selected. (i need to make the method for it async.)

- Sliders Currently Do not Function. Same with the Button that is Supposed to be used to apply this.

- Score Graph on the right View is currently Not implemented. 

## Credits

[MatrikMoon](https://www.Github.com/MatrikMoon/) - Teaching me Most of what i Know when it comes to Modding the game, but mainly ViewControllers and FlowCoordinators, as well as the didFinishEvent.

[RedBrumbler](https://github.com/RedBrumbler) - Taught me how to make a Custom list, as well as a Normal List. TYSM

[Redageddon](https://github.com/Redageddon) & [Top_Cat](https://github.com/Top-Cat) - Taught me how to Assign UIValues Properly through getters and setters. 👏

[BSMG](https://discord.gg/beatsabermods) #pc-mod-dev - Checked the pins and Found  the BSPIA4 Unity Runtime Editor. super cool 😎 super wonderful community filled with Super Smart and Creative People.

[32KZ](https://www.twitter.com/32ksarah/) - Figuring out how to make mods, Running them, Developing them, programing this, Learning an entirely new language, ect.

[Kullly_](https://www.youtube.com/@kullly_7813) - Sourcing Some Test Data, helping with Paperwork 🙏


