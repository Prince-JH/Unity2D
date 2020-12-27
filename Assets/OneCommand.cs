using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OneCommand : MonoBehaviour
{
    public GameObject Column, Floor, White, Quit, Booster;
    public AudioSource BirdSound_1, BirdSound_2, BirdSound_3, BirdSound_4, BoostSound;
    public Text Score;
    Rigidbody2D rig;
    float NextTime = 2.0f;
    float boostTime = 7.0f;
    int i, j, k, BestScore = 0;
    bool Stop = false;
    bool isBoost = false;
    [Header("파티클 시스템(부스터)")]
    [SerializeField] ParticleSystem boostEffect;
    GameObject[] gameObjects = new GameObject[3];
    GameObject[] boosters = new GameObject[3];

    
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        rig.AddForce(Vector3.up * 270);
        BirdSound_1.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        GetComponent<Animator>().SetFloat("Velocity", rig.velocity.y);

        if (gameObject.transform.position.y > 4.65f)
            transform.position = new Vector3(-1.5f, 4.65f, 0);
        if (gameObject.transform.position.y < -2.55f)
        {
            rig.simulated = false;
            GameOver();
        }
        if(rig.velocity.y > 0)
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(transform.rotation.z, 30f, rig.velocity.y / 8f));
        if(rig.velocity.y < 0)
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(transform.rotation.z, -80f, -rig.velocity.y / 8f));

        if (Stop)
            return;

        if ((Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            rig.velocity = Vector3.zero; //순간 속력이 0이 되어야 힘을 제대로 받음
            rig.AddForce(Vector3.up * 270);
            BirdSound_1.Play();
        }
        
        
        if (!isBoost)
            MoveIdle();
        else
            MoveFast();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        //부스터 먹음
        if (col.gameObject.tag == "Booster")
        {
            Destroy(col.gameObject);
            StartCoroutine(MakeHero());
            isBoost = true;
        }
        //칼럼, 바닥 부딪힘
        else if (col.gameObject.name == "Column(Clone)")
        {
            Score.text = (++i).ToString();
            BirdSound_2.Play();
        }
        else if (!Stop)
        {
            rig.velocity = Vector3.zero;
            BirdSound_4.Play();
            GameOver();
        }
    }
    //무적효과
    IEnumerator MakeHero()
    {
        this.gameObject.layer = 9;
        GetComponent<SpriteRenderer>().color = Color.red;
        boostEffect.Play();
        BoostSound.Play();
        yield return new WaitForSeconds(5.0f);
        for(int i = 0; i < 10; i++)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
        }
        this.gameObject.layer = 0;
        GetComponent<SpriteRenderer>().color = Color.white;
        boostEffect.Stop();
        BoostSound.Stop();
        MoveIdle();
        isBoost = false;
    }
    void MoveFast()
    {
        //기둥 생성기
        if (Time.time > NextTime)
        {
            NextTime = Time.time + 0.5f;
            gameObjects[j] = (GameObject)Instantiate(Column, new Vector3(4, Random.Range(-1f, 3.0f), 0), Quaternion.identity);
            if (++j == 3)
                j = 0;
        }
        for (int i = 0; i < 3; i++)
        {
            if (gameObjects[i])
            {
                gameObjects[i].transform.Translate(-0.09f, 0, 0);
                if (gameObjects[i].transform.position.x < -3)
                    Destroy(gameObjects[i]);
            }
        }
    }
    void MoveIdle()
    {
        //기둥 생성기
        if (Time.time > NextTime)
        {
            NextTime = Time.time + 2.0f;
            gameObjects[j] = (GameObject)Instantiate(Column, new Vector3(4, Random.Range(-1f, 3.0f), 0), Quaternion.identity);
            if (++j == 3)
                j = 0;
        }

        //부스터 생성
        if (Time.time > boostTime)
        {
            boostTime = Time.time + 10.0f;
            boosters[k] = (GameObject)Instantiate(Booster, new Vector3(4, Random.Range(-1f, 3.2f), 0), Quaternion.identity);
            if (++k == 3)
                k = 0;
        }
        for (int i = 0; i < 3; i++)
        {
            if (gameObjects[i])
            {
                gameObjects[i].transform.Translate(-0.03f, 0, 0);
                if (gameObjects[i].transform.position.x < -4)
                    Destroy(gameObjects[i]);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (boosters[i])
            {
                boosters[i].transform.Translate(-0.03f, 0, 0);
                if (boosters[i].transform.position.x < -4)
                    Destroy(boosters[i]);
            }
        }
    }
    void GameOver()
    {
        //게임 오버
        if (!Stop)
            BirdSound_3.Play();
        Debug.Log("주금");
        Stop = true;
        Floor.GetComponent<Animator>().enabled = false;
        White.SetActive(true);
        Score.gameObject.SetActive(false);
        if (PlayerPrefs.GetInt("BestScore", 0) < int.Parse(Score.text))
            PlayerPrefs.SetInt("BestScore", int.Parse(Score.text));
        if (transform.position.y < -2.55f)
        {
            Quit.SetActive(true);
            Quit.transform.Find("ScoreScreen").GetComponent<Text>().text = Score.text;
            Quit.transform.Find("BestScreen").GetComponent<Text>().text = PlayerPrefs.GetInt("BestScore").ToString();
        }
    }
    public void Restart()
    {
        //재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
