using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseWeather : MonoBehaviour
{
    public static HouseWeather Instance;

    ParticleSystem.MainModule rmain;
    ParticleSystem.MainModule smain;
    ParticleSystem.MainModule fmain;
    ParticleSystem.MainModule lmain;
    public Light mainlight;
    public Color32 cH = new Color32(255, 255, 218, 255);
    public Color32 cL = new Color32(0, 0, 0, 255);
    private string lastWeather;

    readonly string[] weather = new string[] {
    "��", "����", "����" , "������", "��" ,//4
    "�з�", "΢��", "�ͷ�", "���","ǿ��", "����", "���", "�ҷ�",//12
    "쫷�", "�����", "�ȴ��籩", "�񱩷�", "�籩",//17
    "��", "ëë��", "С��", "����", "����", "����",//23
    "����", "����", "�ش���", "ǿ����", "���˽���",//28
    "������", "ǿ������",//30
    "��", "����","��",//33
    "��������б���",//34
    "ѩ", "Сѩ", "��ѩ", "��ѩ", "��ѩ", "��ѩ",//40
    "���ѩ", "��ѩ����", "�����ѩ"//43
    };
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(LoadRain("rain"));
        StartCoroutine(LoadSnow("snow"));
        StartCoroutine(LoadFog("fog"));
        StartCoroutine(LoadLighting("lighting"));
    }

    public void SetTianGuang(float v)
    {
        mainlight.color = Color.Lerp(cL, cH, v);
        RenderSettings.skybox.SetFloat("_Exposure", v + 0.15f);
    }
    public void SetRain(float value)
    {
        rmain.maxParticles = (int)value;
    }
    public void SetSnow(float value)
    {
        smain.maxParticles = (int)value;
    }
    public void SetFog(float value)
    {
        fmain.maxParticles = (int)value;
    }
    public void SetLighting(float value)
    {
        lmain.maxParticles = (int)value;
    }

    internal void SetWeather(string str)
    {
        if (lastWeather == str)
        {
            return;
        }
        lastWeather = str;
        SetRain(0);
        SetSnow(0);
        SetLighting(0);
        SetFog(0);
        switch (str)
        {
            case "��":
            case "����":
            case "����":
            case "������":
            case "��":

                break;
            case "С��":
            case "��":
            case "ëë��":
                SetRain(200);
                break;
            case "����":
                SetRain(300);
                break;
            case "����":
                SetRain(400);
                break;
            case "����":
                SetRain(500);
                break;
            case "����":
            case "����":
                SetRain(700);
                break;
            case "�ش���":
                SetRain(800);
                break;
            case "ǿ����":
            case "���˽���":
                SetRain(1000);
                break;
            case "������":
            case "ǿ������":
                SetRain(700);
                SetLighting(3);
                break;
            case "ѩ":
            case "Сѩ":
                SetSnow(200);
                break;
            case "��ѩ":
                SetSnow(400);
                break;
            case "��ѩ":
                SetSnow(700);
                break;
            case "��ѩ":
            case "��ѩ":
                SetSnow(1000);
                break;
            case "���ѩ":
                SetSnow(300);
                SetRain(200);
                break;
            case "��ѩ����":
                SetSnow(400);
                SetRain(300);
                break;
            case "�����ѩ":
                SetSnow(700);
                SetRain(500);
                break;
            case "��":
                SetFog(40);  //80
                ChangeFogColor(Color.white);
                break;
            case "����":
                SetFog(20);  //80
                ChangeFogColor(Color.white);
                break;
            case "��":
                SetFog(80);  //80
                ChangeFogColor(Color.yellow);
                break;
            default:
                break;
        }
    }

    private void ChangeFogColor(Color color)
    {
        var c = fmain.startColor;
        var y = color;
        y.a = 0.19f;
        c.color = y;
        fmain.startColor = c;
    }

    public IEnumerator CreatSky(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            yield break;
        yield return Help.Instance.ABLoad("sky", str);
        var ab = Help.Instance.GetBundle("sky", str);
        if (ab)
        {
            var ma = ab.LoadAsset<Material>(str);
            if (ma)
                RenderSettings.skybox = ma;
            SetTianGuang(0.1f);
        }
    }

    IEnumerator LoadRain(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) goto err;
        yield return Help.Instance.ABLoad("weather", str);
        var ab = Help.Instance.GetBundle("weather", str);
        if (!ab) goto err;
        var parti = ab.LoadAsset<GameObject>(str);
        if (!parti) goto err;
        var ps = Instantiate(parti).GetComponent<ParticleSystem>();
        if (!ps) goto err;
        rmain = ps.main;
        SetRain(0);
        yield break;
    err:
        Debug.LogError("�������");
    }

    IEnumerator LoadSnow(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) goto err;
        yield return Help.Instance.ABLoad("weather", str);
        var ab = Help.Instance.GetBundle("weather", str);
        if (!ab) goto err;
        var parti = ab.LoadAsset<GameObject>(str);
        if (!parti) goto err;
        var ps = Instantiate(parti).GetComponent<ParticleSystem>();
        if (!ps) goto err;
        smain = ps.main;
        SetSnow(0);
        yield break;
    err:
        Debug.LogError("�������");
    }

    IEnumerator LoadFog(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) goto err;
        yield return Help.Instance.ABLoad("weather", str);
        var ab = Help.Instance.GetBundle("weather", str);
        if (!ab) goto err;
        var parti = ab.LoadAsset<GameObject>(str);
        if (!parti) goto err;
        var ps = Instantiate(parti).GetComponent<ParticleSystem>();
        if (!ps) goto err;
        fmain = ps.main;
        SetFog(0);
        yield break;
    err:
        Debug.LogError("�������");
    }

    IEnumerator LoadLighting(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) goto err;
        yield return Help.Instance.ABLoad("weather", str);
        var ab = Help.Instance.GetBundle("weather", str);
        if (!ab) goto err;
        var parti = ab.LoadAsset<GameObject>(str);
        if (!parti) goto err;
        var ps = Instantiate(parti).GetComponent<ParticleSystem>();
        if (!ps) goto err;
        lmain = ps.main;
        SetLighting(0);
        yield break;
    err:
        Debug.LogError("�������");
    }

}
