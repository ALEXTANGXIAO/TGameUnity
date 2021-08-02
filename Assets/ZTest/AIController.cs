using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

class AIController : MonoBehaviour
{
    private ThirdPersonCharacter m_Character;
    public bool LoopDefence;

    public float viewDistance = 3;
    public float viewAngle = 90;
    public GameObject target;

    private void Start()
    {
        m_Character = this.gameObject.GetComponent<ThirdPersonCharacter>();
    }

    private void Update()
    {
        UpdateDefence();
        UpdateView();
    }

    private void UpdateView()
    {
        if (target == null)
        {
            return;
        }

        if (Vector3.Distance(target.transform.position, transform.position) <= viewDistance)
        {
            Vector3 dir = target.transform.position - transform.position;
            float angle = Vector3.Angle(dir, transform.forward);
            if (angle <= viewAngle / 2)
            {
                Debug.Log("视野内");
            }
        }
    }

    private void UpdateDefence()
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
