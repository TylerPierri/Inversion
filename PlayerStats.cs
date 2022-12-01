using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

[RequireComponent(typeof(AudioSource))]
public class PlayerStats : MonoBehaviourPunCallbacks
{
    private PhotonView view;
    private TeamIdentity identity;

    public string playerName;
    public int playerTeam;

    [SerializeField]
    private Text playerTextName;

    public PlayerGameInfo info;
    private SpawnPlayerManager spawnManager;

    [SerializeField] private float killStreakTimer = 5f;
    private float killTimer = 0;

    [SerializeField] private AudioClip[] killStreakTimedClips;
    [SerializeField] private AudioClip[] currentStreakClips;

    private AudioSource source;
    [SerializeField] private List<AudioClip> clipQueue;

    [Header("Kill List System")]
    public List<KillList> killLists = new List<KillList>();

    [System.Serializable]
    public class KillList
    {
        public string enemyName;
        public int killedByCount;
        public int killedThemCount;
    }


    private void Start()
    {
        source = GetComponent<AudioSource>();
        view = GetComponentInChildren<PhotonView>();
        spawnManager = GameObject.Find("Game Manager").GetComponent<SpawnPlayerManager>();
        spawnManager.playerListOBJ.Add(this.gameObject);

        identity = GetComponent<TeamIdentity>();


        if(!view.IsMine)
        {
            AudioListener listener = GetComponentInChildren<AudioListener>();
            listener.enabled = false;
        }

        setPlayerInfo();
    }

    private void setPlayerInfo()
    {
        info = new PlayerGameInfo();
        info.playerNickName = view.Owner.NickName;
    }
    private float KDCalculator()
    {
        return info.currentKills / info.currentDeaths;
    }

    private void Update()
    {
        if(playerName != string.Empty)
        {
            playerTextName.text = playerName;
            //playerTextName.color = identity.playerTeamColor;
        }


        if (!view.IsMine)
            return;

        if(killTimer < 0)
        {
            info.killStreakTimed = 0;
        }
        else
        {
            killTimer -= Time.deltaTime;
        }
    }

    public void awardKill(string enemyKilled)
    {
        info.currentKills++;
        killTimer = killStreakTimer;
        info.currentStreak++;
        info.killStreakTimed++;

        KillList enemylist = checkForEnemyName(enemyKilled);
        enemylist.killedThemCount++;

        if (info.currentStreak > info.highestStreak)
            info.highestStreak = info.currentStreak;

        if (!view.IsMine)
            return;

        checkForAudio();

        if(clipQueue != null)
        {
            StartCoroutine(sfxQueue());
        }
    }

    private IEnumerator sfxQueue()
    {
        for(int i = 0; i < clipQueue.Count; i++)
        {
            yield return new WaitUntil(() => source.isPlaying == false);
            source.clip = clipQueue[i];
            source.Play();
        }

        clipQueue.Clear();
    }

    public void deathReset(string enemyKiller)
    {
        killTimer = 0;
        info.currentStreak = 0;
        info.currentDeaths++;

        KillList enemylist = checkForEnemyName(enemyKiller);
        enemylist.killedByCount++;
    }

    public KillList checkForEnemyName(string enemyName)
    {
        if(killLists.Count == 0)
        {
            KillList enemyList = new KillList();
            enemyList.enemyName = enemyName;

            killLists.Add(enemyList);
            return killLists[0];
        }

        for (int i = 0; i < killLists.Count; i++)
        {
            if (killLists[i].enemyName == enemyName)
                return killLists[i];
        }

        KillList newEnemyList = new KillList();
        newEnemyList.enemyName = enemyName;

        killLists.Add(newEnemyList);
        return killLists[killLists.IndexOf(newEnemyList)];
    }

    private void checkForAudio()
    {
        switch(info.killStreakTimed)
        {
            case 2:
                clipQueue.Add(killStreakTimedClips[0]);
                break; 

            case 3:
                clipQueue.Add(killStreakTimedClips[1]);
                break;

            case 4:
                clipQueue.Add(killStreakTimedClips[2]);
                break;

            case 5:
                clipQueue.Add(killStreakTimedClips[3]);
                break;
        }

        switch (info.currentStreak)
        {
            default:
                return;

            case 5:
                clipQueue.Add(currentStreakClips[0]);
                break;

            case 10:
                clipQueue.Add(currentStreakClips[1]);
                break;

            case 20:
                clipQueue.Add(currentStreakClips[2]);
                break;
        }
    }
}
