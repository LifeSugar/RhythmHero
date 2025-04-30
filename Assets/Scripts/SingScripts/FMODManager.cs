using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class FMODManager : MonoBehaviour
{
    [Header("FMOD Event")]
    [SerializeField] private EventReference animalConcertEvent;
    
    public static FMODManager singleton;

    void Awake()
    {
        singleton = this;
    }

    public void StopMusic()
    {
        instance.stop(STOP_MODE.IMMEDIATE);
    }

    private EventInstance instance;

    // ��ǰÿһ�ֵ�ѡ��Ĭ��ֵΪ0��ʾδѡ��
    private Dictionary<int, int> roundSelections = new Dictionary<int, int>()
    {
        { 1, 0 },
        { 2, 0 },
        { 3, 0 },
        { 4, 0 },
    };

    void Start()
    {
        instance = RuntimeManager.CreateInstance(animalConcertEvent);

        // ��ʼ��������ȷ�������Զ�����
        for (int i = 1; i <= 4; i++)
        {
            instance.setParameterByName("Animal_R" + i, 0);
        }

        instance.start(); // Ĭ�����������������κι��
    }

    // �ⲿ��ť����������������� 1~12 �ı�ţ������ִ��붯������
    public void SetTrack(int trackCode)
    {
        int round = (trackCode - 1) / 3 + 1; // 1~3 -> Round 1, 4~6 -> Round 2, etc.

        // ��������ӳ�䣺���1�����3���ߡ�2
        int[] animalMap = { 1, 3, 2 };
        int animalIndex = animalMap[(trackCode - 1) % 3];

        Debug.Log( "  ���� Round = {round}, Animal = {animalIndex}");

        // �ؼ���ǿ���ز���Ƶ��ȷ���붯��ͬ����
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); // ����ֹͣ��ǰ����
        instance.start(); // ���²����¼�

        //  �������ò�����ȷ�����´��������仯
        instance.setParameterByName("Animal_R" + round, 0);
        instance.setParameterByName("Animal_R" + round, animalIndex);

        // ����ѡ���¼
        roundSelections[round] = animalIndex;
    }

    // �ⲿ�ɵ��ã���������ִε�ѡ��
    public void ResetAll()
    {
        for (int i = 1; i <= 4; i++)
        {
            instance.setParameterByName("Animal_R" + i, 0);
            roundSelections[i] = 0;
        }
    }
}
