using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    public static GameManger Instance{set;get;}
    private const float REQUIRED_SLICEFORCE = 1.0f;
    
    public GameObject MolyPrefab;
    public Transform trail;

    private bool isPaused;
    private List<Moly> molies = new List<Moly>();
    private float lastSpawn;
    private float deltaSpawn = 1.0f;
    private Vector3 lastMousePos;
    private Collider2D[] moliesCols;


    private int Score;
    private int Highscore;
    private int Lifepoint;
    public Text ScoreText;
    public Text HighscoreText;
    public Image[] Lifepoints;
    public GameObject pauseMenu;
    public GameObject deathMenu;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        moliesCols = new Collider2D[0];
        SoundManager.Instance.PlaySound(0);
        NewGame();
    }

    public void NewGame()
    {
        SoundManager.Instance.PlaySound(0);
        Score = 0;
        Lifepoint = 3;
        pauseMenu.SetActive(false);
        ScoreText.text = Score.ToString();
        Highscore = PlayerPrefs.GetInt("Score");
        HighscoreText.text = "BEST:" + Highscore.ToString();
        Time.timeScale = 1;
        isPaused = false;

        foreach(Image i in Lifepoints)
        {
            i.enabled = true;
        }

        foreach(Moly m in molies)
        {
            Destroy(m.gameObject);
        }
        molies.Clear();

        deathMenu.SetActive(false);
        SoundManager.Instance.PlaySound(0);
        
    }

    private void Update()
    {
        if(isPaused)
        {
            return;
        }
        if(Time.time - lastSpawn > deltaSpawn)
        {
            Moly m = GetMoly();
            float randomX = Random.Range(-1.65f, 1.65f);
            m.LaunchMoly(Random.Range(1.85f,2.75f), randomX, -randomX);
            lastSpawn = Time.time;
        }

        if(Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -1;
            trail.position = pos;

            Collider2D[] thisFramesMolies = Physics2D.OverlapPointAll(new Vector2(pos.x, pos.y), LayerMask.GetMask("Moly"));

            if((Input.mousePosition - lastMousePos).sqrMagnitude > REQUIRED_SLICEFORCE)
            {
                foreach (Collider2D c2 in thisFramesMolies)
                {
                    for(int i = 0; i < moliesCols.Length; i++)
                    {
                        if(c2 == moliesCols[i])
                        {
                            c2.GetComponent<Moly>().Slice();
                        }
                    }
                }
            }
            lastMousePos = Input.mousePosition;
            moliesCols = thisFramesMolies;
        }
    }


    private Moly GetMoly()
    {
        Moly m = molies.Find(x=> !x.IsActive);

        if(m == null)
        {
            m = Instantiate(MolyPrefab).GetComponent<Moly>();
            molies.Add(m);

        }
        return m;
    }

    public void IncrementScore(int scoreAmount)
    {
        Score += scoreAmount;
        ScoreText.text = Score.ToString();
        if(Score>Highscore)
        {
          Highscore = Score;
          HighscoreText.text = "BEST:" + Highscore.ToString();
          PlayerPrefs.SetInt("Score", Highscore);
        }
    }

    public void LoseLifepoint()
    {
        if(Lifepoint ==0)
        {
            return;
        }
        SoundManager.Instance.PlaySound(2);
        Lifepoint--;
        Lifepoints[Lifepoint].enabled = false;
        if(Lifepoint ==0)
        {
            SoundManager.Instance.PlaySound(3);
            Death();
        }
    }
    public void Death()
    {
        isPaused = true;
        deathMenu.SetActive(true);
    }

    public void PauseGame()
    {
        SoundManager.Instance.PlaySound(0);
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        isPaused = pauseMenu.activeSelf;
        Time.timeScale = (Time.timeScale == 0) ? 1:0 ;
        SoundManager.Instance.PlaySound(0);
    }

    public void ToMenu()
    {
        SoundManager.Instance.PlaySound(0);
        SceneManager.LoadScene("Menu");
        SoundManager.Instance.PlaySound(0);
    }

}
