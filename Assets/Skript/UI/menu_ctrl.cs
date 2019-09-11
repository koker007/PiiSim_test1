using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menu_ctrl : MonoBehaviour {

    bool Press_old_yn = false;

    [SerializeField]
    public GameObject menu;
    [SerializeField]
    public GameObject setings;
    [SerializeField]
    public GameObject scoreTab;
    [SerializeField]
    public GameObject tutorial;

    GameplayParametrs gameplayParametrs;
    Player player;

	// Use this for initialization
	void Start () {
        get_player();
        gameplayParametrs = gameObject.GetComponent<GameplayParametrs>();
        
    }
	
	// Update is called once per frame
	void Update () {
        Test_press_esc();
        test_mouse_ctrl();
    }

    void get_player() {
        //Если настроек нет
        if (player == null)
        {
            //ищем обьект по тегу
            GameObject obj = GameObject.FindWithTag("Player");
            //вытаскиваем настройки
            if (obj != null) player = obj.GetComponent<Player>();
        }
    }

    //Проверка нажатия эксита
    void Test_press_esc() {
        //Если сейчас нажат а раньще нет
        if (!Press_old_yn && Input.GetKeyDown(KeyCode.Escape)) {
            Press_old_yn = true;

            OpenOrClose_menu();
        }
        else if (Press_old_yn && !Input.GetKeyDown(KeyCode.Escape))
        {
            Press_old_yn = false;
        }
    }

    void OpenOrClose_menu() {
        //Открываем
        if (!menu.active) {
            menu.SetActive(true);
        }
        //Закрываем
        else if(menu.active && gameplayParametrs != null && !gameplayParametrs.GameOver) {
            closeAll();
            menu.SetActive(false);
        }
    }

    //Проверка перехвата мыши
    void test_mouse_ctrl() {
        if (player != null)
        {
            //если меню открыто а мышь не свободна
            if (menu.active && player.mouse_control_on)
            {
                Cursor.lockState = CursorLockMode.None;
                player.mouse_control_on = false;
                Cursor.visible = true;
            }
            //если меню закрыто а мышь свободна
            else if (!menu.active && !player.mouse_control_on)
            {
                player.mouse_control_on = true;
                Cursor.visible = false;
            }
        }
    }


    /////////////////////////////////////////////////////////////////
    //Управление меню
    public void OpenStaticticsTab() {
        if (!menu.active)
            menu.active = true;

        //Нужно закрыть все открытые таблицы
        closeAll();

        //Открываем таблицу
        if (!scoreTab.active) {
            scoreTab.SetActive(true);
        }

    }
    public void OpenSetings() {
        if (!menu.active)
            menu.active = true;

        //Нужно закрыть все открытые таблицы
        closeAll();

        //Открываем таблицу
        if (!setings.active)
        {
            setings.SetActive(true);
        }
    }
    public void OpenTutorial() {
        if (!menu.active)
            menu.active = true;

        //Нужно закрыть все открытые таблицы
        closeAll();

        if (!tutorial.active) {
            tutorial.SetActive(true);
        }
    }

    //Закрытие
    public void closeAll() {
        closeScoreTab();
        closeSetings();
        closeTutorial();
    }
    public void closeScoreTab() {
        if (scoreTab.active) {
            scoreTab.SetActive(false);
        }
    }
    public void closeSetings() {
        if (setings.active)
            setings.SetActive(false);
    }

    public void closeTutorial() {
        if (tutorial.active) {
            tutorial.SetActive(false);
        }
    }
    
}
