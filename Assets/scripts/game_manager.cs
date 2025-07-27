using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class game_manager : MonoBehaviour
{
    public int total_point;
    public int stage_point;
    public int stage_index;
    public int health;
    public move player_movee;
    public GameObject[] stages;
    //UI
    public Image[] UIhealth;
    public TextMeshProUGUI UIPoint;
    public TextMeshProUGUI UIStage;
    public GameObject restart;

    void Update()
    {
        UIPoint.text = (total_point + stage_point).ToString();
    }
    public void NextStage()
    {
        if(stage_index < stages.Length - 1) {
            //캐릭터 위치 속도 초기화
            stages[stage_index].SetActive(false);
            stage_index++;
            stages[stage_index].SetActive(true);
            PlayerResposition();
        }
        else {
            Time.timeScale = 0;
            Debug.Log("게임클리어");

            //UI재시작버튼 활성화
            TextMeshProUGUI bufText = restart.GetComponentInChildren<TextMeshProUGUI>();
            bufText.text = "Clear!";
            restart.SetActive(true);
        }
        //스테이지 UI변경 + 1
        UIStage.text = "STAGE " + (stage_index+1);

        //총 점수 계산
        total_point += stage_point;
        stage_point = 0;
    }
    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            //UI 생명그림
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            //죽음
            health--;
            player_movee.Ondie();
            //UI재시작버튼 활성화 + 마지막 생명그림
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            restart.SetActive(true);

            //3초후정지
            Invoke("StopGame", 3f);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player")
        {
            if (health > 1) {
                //캐릭터 다시 0,0으로 돌려놓기 +속도0
                PlayerResposition();
            }
            HealthDown();
        }
    }
    void PlayerResposition()
    {
        player_movee.transform.position = Vector2.zero;
        player_movee.VelocityZero();
    }
    void StopGame()
    {
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        //player_movee.PlaySound("UI");
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

}
