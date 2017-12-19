using UnityEngine;
using System.Collections;

public class QualityByFPS : MonoBehaviour {
	float timeA;
	public int fps;
	public int lastFPS;
	public GUIStyle textStyle;
    int indice = 0;
    int[] mediciones = new int[10];
	// Use this for initialization
	void Start () {
        if(PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel()) != QualitySettings.GetQualityLevel())
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel()));
        
        timeA = Time.timeSinceLevelLoad;
		//DontDestroyOnLoad (this);
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log(Time.timeSinceLevelLoad+" "+timeA);
		if(Time.timeSinceLevelLoad  - timeA <= 1)
		{
			fps++;
		}
		else
		{
            
			lastFPS = fps + 1;
			timeA = Time.timeSinceLevelLoad;
			fps = 0;
            mediciones[indice % 10] = lastFPS;
            indice++;
            if(indice % 10 == 0 && indice > 0)
            {
                float promedioFPS = 0f;
                for(int i = 0; i < 10; i++)
                {
                    promedioFPS += mediciones[i];
                }
                promedioFPS = promedioFPS / 10f;
                print(promedioFPS);

                if (promedioFPS < 24f)
                {
                    if (QualitySettings.GetQualityLevel() != 0) {
                        QualitySettings.SetQualityLevel(QualitySettings.GetQualityLevel() - 1);
                        print("bajando calidad");
                        PlayerPrefs.SetInt("quality", QualitySettings.GetQualityLevel());
                    }
                }
                else
                {
                    if (promedioFPS > 40f)
                    {
                        if (QualitySettings.GetQualityLevel() != 5) {
                            QualitySettings.SetQualityLevel(QualitySettings.GetQualityLevel() + 1);
                            print("subiendo calidad");
                            PlayerPrefs.SetInt("quality", QualitySettings.GetQualityLevel());

                        }
                    }
                }
            }
        }
	}
	void OnGUI()
	{
		//GUI.Label(new Rect( Screen.width / 2 - 50 ,5 , 100, 30), Application.loadedLevelName+" "+lastFPS, textStyle);
	}
}