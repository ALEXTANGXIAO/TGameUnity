using SocketGameProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Characters.ThirdPerson;

class ActorMgr: BehaviourSingleton<ActorMgr>
{
    private Dictionary<string, GameActor> playersActors = new Dictionary<string, GameActor>();

    private GameObject m_goChararcter;
    private Transform m_spawnPos;
    private GameActor m_ownActor;
    public Transform ActorRoot;


    public override void Awake()
    {
        Init();
        RegisterEvent();
    }

    private void Init()
    {
        GameObject obj = new GameObject("ActorRoot");
        GameObject.DontDestroyOnLoad(obj);
        ActorRoot = obj.transform;
        m_goChararcter = Resources.Load("Prefab/Player") as GameObject;
    }

    private void RegisterEvent()
    {
        GameClient.Instance.RegActionHandle((int)ActionCode.RemoveCharacter, RemoveCharacter);
        GameClient.Instance.RegActionHandle((int)ActionCode.AddCharacter, AddCharacter);
        GameClient.Instance.RegActionHandle((int)ActionCode.UpCharacterList,RemoveCharacter);
    }

    private void AddCharacter(MainPack mainPack)
    {
        AddPlayerEntity(mainPack.Str);
    }
    private void RemoveCharacter(MainPack mainPack)
    {
        removePlayer(mainPack.Str);
    }

    public string CurPlayerID
    {
        get;
        private set;
    }

    public GameObject GetActorModel(string ActorId)
    {
        return Resources.Load("Prefab/Player") as GameObject;
    }

    public void AddPlayerEntity(MainPack pack)
    {
        //spawnPos = GameObject.Find("SpawnPos").transform;
        foreach (var p in pack.Playerpack)
        {
            if (playersActors.ContainsKey(p.Playername))
            {
                continue;
            }
            
            Vector3 pos = new Vector3(0,0,1);//spawnPos.position;
            GameObject g = GameObject.Instantiate(m_goChararcter, pos, Quaternion.identity);
            GameActor actor = new GameActor(g);
            g.name = string.Format("[{0}][{1}][{2}]", 1001, "ePlayer", p.Playername);
            if (p.Playername.Equals(LoginDataMgr.Instance.m_userName))
            {
                m_ownActor = actor;
                g.AddComponent<ThirdPersonUserControl>();
                g.AddComponent<ActorPosCmpt>();
                g.AddComponent<BubbleCmpt>();
                g.AddComponent<ActorNameCmpt>().Init(actor, p.Playername);

                GameObject.DestroyImmediate(Camera.main.gameObject);

                var Cam = ResourcesManager.Instance.Load("Prefab/FreeLookCameraRig") as GameObject;
                Cam.name = "FreeLookCameraRig";
                GameObject.DontDestroyOnLoad(Cam);
                Cam.GetComponent<FreeLookCam>().Target = g.transform;
            }
            else
            {
                g.AddComponent<ActorOnlineCmpt>();
                g.AddComponent<BubbleCmpt>();
                g.AddComponent<ActorNameCmpt>().Init(actor, p.Playername);
            }
            g.transform.SetParent(ActorRoot);
            playersActors.Add(p.Playername,actor);
        }
    }

    public void AddPlayerEntity(string id)
    {
        if (playersActors.ContainsKey(id))
        {
            return;
        }

        m_spawnPos = GameObject.Find("SpawnPos").transform;
        Vector3 pos = m_spawnPos.position;
        GameObject g = GameObject.Instantiate(m_goChararcter, pos, Quaternion.identity);
        GameActor actor = new GameActor(g);
        g.name = string.Format("[{0}][{1}][{2}]",1001,"ePlayer",id);
        if (id.Equals(LoginDataMgr.Instance.m_userName))
        {
            m_ownActor = actor;
            g.AddComponent<PlayerController>();
            g.AddComponent<ActorPosCmpt>();
            g.AddComponent<BubbleCmpt>();
        }
        else
        {
            g.AddComponent<ActorOnlineCmpt>();
            g.AddComponent<BubbleCmpt>();
        }
        g.transform.SetParent(ActorRoot);
        playersActors.Add(id, actor);
    }

    public void removePlayer(string id)
    {
        if (playersActors.TryGetValue(id, out GameActor g))
        {
            GameObject.Destroy(g.gameObject);
            g = null;
            playersActors.Remove(id);
        }
        else
        {
            TDebug.LogError("移除角色出错！没有这个id的Player");
        }
    }

    public void GameExit()
    {
        foreach (var player in playersActors.Values)
        {
            GameObject.Destroy(player.gameObject);
        }
        playersActors.Clear();
    }

    private int animation;
    private int Dirt;
    public void UpPos(MainPack pack)
    {
        if(pack == null)
        {
            return;
        }

        PosPack posPack = pack.Playerpack[0].PosPack;

        if (playersActors.TryGetValue(pack.Playerpack[0].Playername, out GameActor actor))
        {
            if (actor.GetActorOnlineCmpt == null)
            {
                return;
            }
            Vector3 Pos = new Vector3(posPack.PosX, posPack.PosY, posPack.PosZ);//角色位置
            Vector3 moveVector3 = new Vector3(posPack.MoveX, posPack.MoveY, posPack.MoveZ);
            animation = posPack.Animation;
            Dirt = posPack.Dirt;
   
            actor.GetActorOnlineCmpt.SetState(Pos, moveVector3, posPack.RotaY, animation, Dirt);
        }
    }

    public GameActor GetPlayerById(string id)
    {
        GameActor actor;

        if (playersActors.TryGetValue(id, out actor))
        {
            return actor;
        }

        return null;
    }

    public GameActor GetMainPlayer()
    {
        return m_ownActor;
    }

    public bool IsMainPlayer(GameActor actor)
    {
        if (actor == m_ownActor)
        {
            return true;
        }

        return false;
    }

    //----------------------------------------------------生命周期------------------------------------------------------------------//
    public override void LateUpdate()
    {

    }

    public override void Update()
    {

    }
}
