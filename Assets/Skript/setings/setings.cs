using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;

public class setings : MonoBehaviour {

    class stack_string {
        public string text;
        public stack_string stack_next;

        public stack_string(string new_text) {
            text = new_text;
        }

        public void set_new_stack_text(string new_text) {
            stack_string now = this;

            bool end_stack_yn = false;
            while (!end_stack_yn) {
                //Проверяем есть ли следующее звено
                if (now.stack_next != null)
                {
                    now = now.stack_next;
                }
                //Если звена нет то создаем новое и записываем текст
                else {
                    now.stack_next = new stack_string(new_text);
                    end_stack_yn = true;
                }
            }
        }

        //Получить количество звеньев
        public int get_stack_max() {
            stack_string now = this;
            int return_int = 1;

            //цикл пока не достигнем последнего звена
            bool end_stack_yn = false;
            while (!end_stack_yn)
            {
                //Проверяем есть ли следующее звено
                if (now.stack_next != null)
                {
                    now = now.stack_next;
                    return_int++;

                }
                else {
                    end_stack_yn = true;
                }
            }

            return return_int;
        }
    }

    public class Game_setings{

        string path_setings = Application.dataPath + "/setings.txt";
        string[] setings_str;
        
        //Главный ключ для поиска в тексте
        string main_key = ":=";

        //Говорит о том есть ли новые настройки?
        public bool new_data_setings_yn = true;

        string volume_all_key = "volume_all";
        public float volume_all = 0.5f;
        string volume_music_key = "volume_music";
        public float volume_music = 1f;
        string volume_sound_key = "volume_sound";
        public float volume_sound = 1f;

        string speedControllerKey = "speed_controller";
        public float speedController = 2f;

        //вынести данные из файла в найстройки
        void get_setings_from_file() {
            //пытаемся получить файл
            setings_str = File.ReadAllLines(path_setings);
            //Если не пустое то вытаскиваем значения
            if (setings_str != null && setings_str.Length > 0) {
                foreach (string text in setings_str)
                    test_text_to_key(text);

            }
            //зануляем значения
            setings_str = null;

        }
        void set_setings_to_file() {
            //Занулили старые данные
            if (setings_str != null)
                setings_str = null;

            stack_string stack_start = new stack_string("=====Main setings=====");

            //Заполняем новыми данными
            stack_start.set_new_stack_text(" ");
            stack_start.set_new_stack_text("          Volume");
            //громкость обшая
            stack_start.set_new_stack_text(volume_all_key + main_key + volume_all);
            //громкость музыки
            stack_start.set_new_stack_text(volume_music_key + main_key + volume_music);
            //громкость звуков
            stack_start.set_new_stack_text(volume_sound_key + main_key + volume_sound);

            stack_start.set_new_stack_text(" ");
            stack_start.set_new_stack_text("          Controller");
            //Скороcть мыши или контроллера
            stack_start.set_new_stack_text(speedControllerKey + main_key + speedController);

            //стек готов - узнаем число звеньев
            int stack_lenght = stack_start.get_stack_max();
            setings_str = new string[stack_lenght];
            //перебираем все звенья
            stack_string now_stack = stack_start;
            for (int num_now = 0; num_now < stack_lenght; num_now++) {
                //Записываем
                setings_str[num_now] = now_stack.text;
                //переключаем
                now_stack = now_stack.stack_next;
            }

            //Запись в файл того что есть
            File.WriteAllLines(path_setings, setings_str);



        }

        public void Reloading() {
            set_setings_to_file();
            get_setings_from_file();
        }

        //Проверка текста на ключ и значение
        void test_text_to_key(string new_str) {
            bool main_key_found_yn = false;
            int num_simbol_start_main_key = 0;
            int num_simbol_end_main_key = 0;

            //Ищем в тексте главный ключ
            int num_key_time = 0;
            for (int num_sumbol_now = 0; num_sumbol_now < new_str.Length && !main_key_found_yn; num_sumbol_now++) {
                //проверяем символ с символом ключа
                if (new_str[num_sumbol_now] == main_key[num_key_time]) {
                    //совпадение!
                    //переключаемся на следующий символ
                    num_key_time++;

                    //Если это первый символ ключа
                    if (num_key_time == 1) {
                        //запоминаем позицию старта
                        num_simbol_start_main_key = num_sumbol_now;
                    }
                    //если это последний символ ключа
                    if (num_key_time == main_key.Length) {
                        //Говорим что ключ был найден;
                        main_key_found_yn = true;
                        //Запоминаем позицию конца
                        num_simbol_end_main_key = num_sumbol_now + 1;
                    }
                }
                else {
                    //Сбрасываем счетчик совпадений ключа
                    num_key_time = 0;
                }
            }

            //Если ключ был найден
            if (main_key_found_yn) {
                //Разделяем ключ и значение
                string text_key = "";
                string text_value = "";

                //заполняем ключ
                for (int num = 0; num < num_simbol_start_main_key; num++) {
                    text_key = text_key + new_str[num];
                }
                //заполняем ключ
                for (int num = num_simbol_end_main_key; num < new_str.Length; num++) {
                    text_value = text_value + new_str[num];
                }

                //Обшая громкость
                if (volume_all_key == text_key) {
                    volume_all = (float)Convert.ToDouble(text_value);
                }
                //Громкость музыки
                else if (volume_music_key == text_key) {
                    volume_music = (float)Convert.ToDouble(text_value);
                }
                //Громкость звуков
                else if (volume_sound_key == text_key) {
                    volume_sound = (float)Convert.ToDouble(text_value);
                }
                //Скорость мыши
                else if (speedControllerKey == text_key) {
                    speedController = (float)Convert.ToDouble(text_value);
                }


            }
        }

        void test_new_setings() {
            
        }

        //Конструктор пытается вытащить данные из файла если нету, то создает файл
        public Game_setings() {
            get_setings_from_file();
        }
        ~Game_setings() {
            set_setings_to_file();
        }
    }

    public Game_setings game;

	// Use this for initialization
	void Start () {
        game = new Game_setings();
        
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
