using UnityEngine;
 
public class TestAnimations : MonoBehaviour
{

    public AnimationClip[] clips;
    public Animator anim;
    public int Index;
    private void Update()
    {
        AnimationChange();
    }

    public void AnimationChange()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Index > 0)
            {
                Index -= 1;
            }
            else
            {
                Index = clips.Length - 1;
            }
            Debug.Log(string.Format("CurrentClip:{0}", clips[Index].name));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {

            if (Index < clips.Length - 1)
            {
                Index += 1;
            }
            else
            {
                Index = 0;
            }
            Debug.Log(string.Format("CurrentClip:{0}", clips[Index].name));
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetState();
            anim.CrossFade(clips[Index].name, 0, 0, 0);
        }
    }

    protected void ResetState()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}
