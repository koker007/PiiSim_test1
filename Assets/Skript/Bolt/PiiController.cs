using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Контроллер мочи
public class PiiController : MonoBehaviour {

    [SerializeField]
    private Player player;

    //Префаб мочи
    [SerializeField]
    private GameObject prefabPii;

    //Частицы мочи
    [SerializeField]
    private ParticleSystem particlePii;
    [SerializeField]
    private ParticleSystem particlePiiShadows;

    [SerializeField]
    private GameObject PiiParent;

    [SerializeField]
    private PiiSound piiSound;

    [SerializeField]
    private FaceController faceController;
    

    public float ChargeMax = 10;
    public float ChargeNow = 0;
    public bool ChargeOn = false;
    bool oldMouseClick = false;
    bool StartChargeNeed = false;

    int OldIdPii = 0;

    public float speed_pii = 1;

    private float TimeCreatePrefab = 0;

    const float conAmountNormal = 1.1f;
    const float conAmountPresure = 1.5f;

	// Use this for initialization
	void Start () {
        particlePii.Pause();
        particlePiiShadows.Pause();
	}
	
	// Update is called once per frame
	void Update () {
        PiiParticleTest();
        PiiPrefabSpavnTest();
        Calculation_pressure();
        TestMouseClick();
        TestCharge();
        CalcMainShaderEffect();
    }

    void PiiParticleTest() {
        //если надо включить
        if (player.PiiNow && particlePii.isPaused) {
            particlePii.Play();
            particlePiiShadows.Play();
        }
        else if (!player.PiiNow && particlePii.isPlaying) {
            particlePii.Stop();
            particlePiiShadows.Stop();
        }

        //корректируем скорость
        particlePii.startSpeed = 10 * speed_pii;
        particlePiiShadows.startSpeed = 10 * speed_pii;
        //Корректируем размер
        particlePii.startSize = speed_pii * 0.1f;
        particlePiiShadows.startSize = speed_pii * 0.1f;
    }

    void PiiPrefabSpavnTest() {
        if (TimeCreatePrefab > 0) {
            TimeCreatePrefab -= Time.deltaTime;
        }
        if (player.PiiNow && TimeCreatePrefab <= 0) {
            TimeCreatePrefab = 0.1f;
            GameObject NewPii = Instantiate(prefabPii);

            PiiPrefab NewPiiPrefab = NewPii.GetComponent<PiiPrefab>();
            NewPiiPrefab.SetPosition(particlePii.transform.position);
            NewPiiPrefab.SetSpeed(gameObject.transform.rotation, speed_pii);
            NewPiiPrefab.SetPlayer(player);
            NewPiiPrefab.SetSoundPii(piiSound);
            NewPiiPrefab.Presure = speed_pii;
            NewPiiPrefab.faceController = faceController;
            NewPiiPrefab.piiController = gameObject.GetComponent<PiiController>();

            NewPii.transform.parent = PiiParent.transform;

        }
    }

    //Расчет напора в зависимости от количества очков
    void Calculation_pressure() {
        if (ChargeOn) {
            //speed_pii = Calculations.coofing_num(player.ScoreTime * speed_pii * 0.05f, player.coof_pressure - player.coof_presure_plus);
            speed_pii = Calculations.coofing_num(player.ScoreTime * speed_pii * 0.05f, player.coof_pressure);
            float dolya = speed_pii / 4;
            speed_pii = speed_pii + dolya;
        }
        else {
            speed_pii = Calculations.coofing_num(player.ScoreTime * speed_pii * 0.05f, player.coof_pressure);
            if (speed_pii <= 0)
                speed_pii = 0.02f;
        }
    }

    //Проверка увеличен ли напор
    void TestCharge() {

        //Есть ли накопленный заряд?
        if (ChargeNow >= ChargeMax && !ChargeOn && !player.menu.active)
        {
            player.presureWindow.set_all_need(0.6f,0.6f,0.6f,0.6f);
            //проверка на нажатие пробела
            if (Input.GetKey(KeyCode.Space) || StartChargeNeed)
            {
                StartChargeNeed = false;
                //Запускаем слив
                ChargeOn = true;
                //Меняем радиус разброса
                var shape = particlePii.shape;
                shape.angle = 6f;
            }
        }
        else {
            StartChargeNeed = false;
        }

        //Если слив запущен
        if (ChargeOn)
        {
            //И бак еще не пуст
            if (ChargeNow > 0)
            {
                //Сливаем треть в секунду
                ChargeNow -= (ChargeMax / 3) * Time.deltaTime;
            }
            //бак пуст
            else
            {
                //выключаем слив
                ChargeNow = 0;
                ChargeOn = false;
                //Меняем радиус разброса
                var shape = particlePii.shape;
                shape.angle = 1.5f;
            }
        }

    }

    void TestMouseClick() {

        //Если сейчас нажата и раньше нет
        if (Input.GetKey(KeyCode.Mouse0) && !oldMouseClick) {
            oldMouseClick = true;
            StartChargeNeed = true;
        }
        //Если сейчас отжата и раньше да
        else if (!Input.GetKey(KeyCode.Mouse0) && oldMouseClick) {
            oldMouseClick = false;
            StartChargeNeed = false;
        }
    }

    void CalcMainShaderEffect() {
        if (player != null) {
            //Получаем контроллер шейдера
            FullscrinRenderImage MainShader = player.camera.GetComponent<FullscrinRenderImage>();

            float conAmountNeed = conAmountNormal;

            //Если сейчас напор активен
            if (ChargeOn)
            {
                conAmountNeed = conAmountNormal + ((ChargeNow / ChargeMax) * (conAmountPresure - conAmountNormal));
            }
            else {
                if (ChargeNow >= ChargeMax) {
                    conAmountNeed = conAmountPresure;
                }
                else{
                    conAmountNeed = conAmountNormal;
                }
            }

            //меняем значение
            if (MainShader.conAmount != conAmountNeed) {
                float smeshenie = conAmountNeed - MainShader.conAmount;
                MainShader.conAmount = MainShader.conAmount + (smeshenie / 4 * Time.deltaTime);
            }
        }
    }

}
