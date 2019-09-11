using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_interface_close_open : MonoBehaviour {

    [SerializeField]
    menu_ctrl menu_Ctrl;

    [SerializeField]
    RectTransform RightPanel;
    [SerializeField]
    RectTransform LeftPanel;

    [SerializeField]
    float SdvigDown = 100;
    [SerializeField]
    float SdvigLeftAndRight = 100;

    //Обычные "видимые" позиции
    float NormalDown = 0;
    float NormalLeft = 0;
    float NormalRight = 0;

    //Позиция к которым движется
    float NeedDown = 0;
    float NeedLeft = 0;
    float NeedRight = 0;

    // Use this for initialization
    void Start() {
        //Запоминаем обычную позицию
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        NormalDown = rectTransform.position.y;

        NeedDown = NormalDown;

        //Ставим позицию ниже
        Vector3 vector3new = rectTransform.position;
        vector3new.y = NormalDown - SdvigDown;
        rectTransform.position = vector3new;
        

        if (RightPanel != null) {
            NormalRight = RightPanel.position.x;
            NeedRight = NormalRight;


            Vector3 vector3newlocal = RightPanel.position;
            vector3newlocal.x = NormalRight + SdvigLeftAndRight;
            RightPanel.position = vector3newlocal;
        }
        if (LeftPanel != null) {
            NormalLeft = LeftPanel.position.x;
            NeedLeft = NormalLeft;

            Vector3 vector3newlocal = LeftPanel.position;
            vector3newlocal.x = NormalLeft - SdvigLeftAndRight;
            LeftPanel.position = vector3newlocal;
        }
    }

    // Update is called once per frame
    void Update() {
        TestNeedPos();
        TestTransformToNeed();
    }

    //Двигаем если нужно
    void TestTransformToNeed() {
        //Проверяем позицию низ
        RectTransform mainPanel = gameObject.GetComponent<RectTransform>();
        if (mainPanel.position.y != NeedDown) {
            //Узнаем различие
            float raznica = NeedDown - mainPanel.position.y;
            Vector3 new_position = mainPanel.position;
            //Считаем новое положение
            new_position.y = new_position.y + (raznica * Time.deltaTime * 2);

            //Проверяем новую разницу
            //float raznica_new = NeedDown - new_position.y;
            //Если она слишком маленькая то равняем

            //Запоминаем
            mainPanel.position = new_position;

        }

        //Проверка правой панели
        if (RightPanel != null && RightPanel.position.x != NeedRight) {
            //Узнаем различие
            float raznica = NeedRight - RightPanel.position.x;
            Vector3 vector3new = RightPanel.position;
            vector3new.x = vector3new.x + (raznica * Time.deltaTime * 1.5f);
            RightPanel.position = vector3new;
        }
        //Проверка левой панели
        if (LeftPanel != null && LeftPanel.position.x != NeedLeft)
        {
            //Узнаем различие
            float raznica = NeedLeft - LeftPanel.position.x;
            Vector3 vector3new = LeftPanel.position;
            vector3new.x = vector3new.x + (raznica * Time.deltaTime * 1.5f);
            LeftPanel.position = vector3new;
        }
    }

    //Проверка требуемой позиции
    void TestNeedPos() {

        //Если меню активно
        if (menu_Ctrl != null && menu_Ctrl.menu.active)
        {
            NeedDown = NormalDown - SdvigDown;
            NeedLeft = NormalLeft - SdvigLeftAndRight;
            NeedRight = NormalRight + SdvigLeftAndRight;
        }
        else {
            NeedDown = NormalDown;
            NeedLeft = NormalLeft;
            NeedRight = NormalRight;
        }
    }
}
