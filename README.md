# UIAnimation
Simply set unity3d UI animation in inspector. Include some common effects.

# Usage

## ButtonAnim
Add `ButtonAnim` to a GameObject, then you can set animation status in inspector. Animation will play when pointer click and release. Also apply to 3D GameObject if the 3D GameObject has `Collider` component and your camera which render 3d game object added `PhysicRaycaster` component.

## UIDOTween
Add `UIDOTween` to an UI GameObject, then you can set Open and Close animation status in inspector. You can call `DoStartTween` and `DoCloseTween` methods to play Open and Close animation.

![image](https://github.com/Mr-sB/UIAnimation/blob/master/Screenshots/Inspector.png)
# Note
All animations are require [DOTween](http://dotween.demigiant.com/) plugin.
