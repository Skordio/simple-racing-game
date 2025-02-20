# Simple Racing Game

This is a repo where my idea is to make a very simple racing game in Unity as a way to test training a neural network to play it.

## Plan

I want to make this game quickly as the main point of the project is to train an AI, but it has to be fun to play for me. 

I also want to eventually be able to challenge the ai, and hopefully get to a point where I am unable to beat it. 

I won't move forward with the AI before:

- the game is fun and movement feels good
- time trials work well
- I can have fun with repeated attempts to beat my own scores

## Movement

One thing that stands out to me about 2D racing games is that controlling them can often feel very unintuitive. The idea usually is pressing forward to increase your car's momentum, with the ability to use the arrow keys to turn the car. I do not like this control scheme.

I want to create a control scheme that feels intuitive to use to play. On this front I have two ideas: controller-based and mouse-based.

### Controller

My idea for a controller is to have the user point the control stick in the direction they want their car to be turning torwards in that moment, and to hold a button for acceleration, a button for decceleration, and maybe even a button for drifting. The drifting would allow the car to turn much faster, without being limited by the requirement of moving in the direction it is facing.

I feel as though this control scheme could feel unintuitive at first glance, but the addition of drifting would change that and make it feel much better to use. I will have to experiment with different settings for drifting until I can implement it in a way that feels good.

### Mouse

The idea for the mouse-based movement is that it would be similar to how the controller-based movement works, but instead of using a control stick to indicate the direction you want the car to be turning towards, you would indicate this direction through the position of your cursor in relation to where the car currently is. I imagine that the acceleration, decceleration, and drifting features could all work relatively similar this way.