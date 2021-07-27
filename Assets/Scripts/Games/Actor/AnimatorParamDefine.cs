using UnityEngine;

public static class AnimatorParamDefine
{
    public static int back = Animator.StringToHash("back");
    public static int forward = Animator.StringToHash("forward");
    public static int running = Animator.StringToHash("running");


    public static int Forward = Animator.StringToHash("Forward");
    public static int Turn = Animator.StringToHash("Turn");
    public static int Crouch = Animator.StringToHash("Crouch");
    public static int OnGround = Animator.StringToHash("OnGround");
    public static int Jump = Animator.StringToHash("Jump");
    public static int JumpLeg = Animator.StringToHash("JumpLeg");

    public static int IsAttack = Animator.StringToHash("IsAttack");
    public static int AttackType = Animator.StringToHash("AttackType");
    public static int IsSkill = Animator.StringToHash("IsSkill");
    public static int IsHurt = Animator.StringToHash("IsHurt");
}
