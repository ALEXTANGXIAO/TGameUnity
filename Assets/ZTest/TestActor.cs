using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class TestActor : MonoBehaviour
{
    private ThirdPersonCharacter m_Character;
    public bool LoopDefence;
    void Start()
    {
        Cursor.visible = false;
        m_Character = this.gameObject.GetComponent<ThirdPersonCharacter>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            LoopDefence = true;
        }

        if (Input.GetKey(KeyCode.F))
        {
            LoopDefence = false;
        }

        if (LoopDefence)
        {
            m_Character.Move(Vector3.zero, false, false);
            m_Character.Battle(false, false, true);
        }
        else
        {
            m_Character.Move(Vector3.zero, false, false);
            m_Character.Battle(false, false, false);
        }
    }
}
